
using Microsoft.Extensions.Logging;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Data.Enums;
using Cursus.Data.Entities;
using Cursus.ServiceContract.Interfaces;
using PayPalCheckoutSdk.Orders;
using Microsoft.AspNetCore.Http;
using Cursus.Data.DTO;
using AutoMapper;


namespace Demo_PayPal.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PayPalClient _payPalClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;
        public PaymentService(PayPalClient payPalClient, ILogger<PaymentService> logger, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _payPalClient = payPalClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<string> CreatePayment(int orderId, string returnUrl = "https://your-default-return-url.com", string cancelUrl = "https://your-default-cancel-url.com")
        {



            var order = await _unitOfWork.OrderRepository.GetOrderWithCartAndItemsAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("The order does not exist.");
            }

            if (order.Cart.IsPurchased)
            {
                throw new BadHttpRequestException("The cart has already been purchased. You cannot proceed with the payment.");
            }

           
            if (order.Status != OrderStatus.PendingPayment)
            {
                throw new BadHttpRequestException("Order is not in a pending payment state.");
            }

            var amount = order.PaidAmount;
            if (amount <= 0)
            {
               
                throw new ArgumentException("The payment amount must be greater than 0.");
            }

          
            var pendingTransaction = await _unitOfWork.TransactionRepository.GetPendingTransaction(orderId);
            if (pendingTransaction != null)
            {
               
                throw new BadHttpRequestException("A pending transaction already exists for this order.");
            }

           

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = "USD",
                    Value = amount.ToString("F2")
                }
            }
        },
                ApplicationContext = new ApplicationContext
                {
                    CancelUrl = cancelUrl,
                    ReturnUrl = returnUrl
                }
            });

          
            var response = await _payPalClient.Client().Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

          
            if (result == null || result.Links == null || !result.Links.Any())
            {
               
                throw new InvalidOperationException("Invalid response from PayPal. No approval link found.");
            }

            var approvalUrl = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
            if (string.IsNullOrEmpty(approvalUrl))
            {
               
                throw new InvalidOperationException("Unable to generate PayPal payment link.");
            }

            var token = result.Id;


           



            var transactionEntry = new Transaction
            {
                UserId = order.Cart.UserId,
                OrderId = orderId,
                PaymentMethod = "PayPal",
                DateCreated = DateTime.Now,
                Status = TransactionStatus.Pending,
                Token = token
            };

            await _unitOfWork.TransactionRepository.AddAsync(transactionEntry);
            await _unitOfWork.SaveChanges();

            string paymentUrl = $"{approvalUrl}";
            return paymentUrl;
        }


        public async Task<TransactionDTO> CapturePayment(string token, string payerId, int orderId)
        {
            // Tìm giao dịch đang chờ xử lý
            var pendingTransaction = await _unitOfWork.TransactionRepository.GetPendingTransaction(orderId);
            if (pendingTransaction == null)
                throw new KeyNotFoundException("Pending transaction not found.");

            // Kiểm tra tính hợp lệ của giao dịch
            if (pendingTransaction.Token != token)
                throw new ArgumentException("Token does not match the transaction.");
            if (pendingTransaction.OrderId != orderId)
                throw new ArgumentException("OrderId does not match the transaction.");

            // Kiểm tra giao dịch có hết hạn không
            if (IsTransactionExpired(pendingTransaction))
            {
                await UpdateFailedTransaction(pendingTransaction);
                throw new BadHttpRequestException("Transaction has expired.");
            }

            // Nếu payerId rỗng, nghĩa là người dùng đã hủy giao dịch
            if (string.IsNullOrEmpty(payerId))
            {
                await UpdateFailedTransaction(pendingTransaction);
                throw new BadHttpRequestException("Payment was cancelled by the user.");
            }

            try
            {
                // Gửi yêu cầu bắt giao dịch đến PayPal
                var result = await CapturePayPalPayment(token);

                // Xử lý kết quả trả về từ PayPal
                await HandleTransactionResult(pendingTransaction, result, payerId);

                // Ánh xạ từ Transaction sang TransactionDTO trước khi trả về
                var transactionDTO = _mapper.Map<TransactionDTO>(pendingTransaction);
                return transactionDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while capturing payment for TransactionId: {TransactionId}", pendingTransaction.TransactionId);
                throw new InvalidOperationException($"An error occurred: {ex.Message}");
            }
        }




        

        
        private bool IsTransactionExpired(Transaction transaction)
        {
            return transaction.DateCreated <= DateTime.Now.AddMinutes(-10); 
        }

       
        private async Task UpdateFailedTransaction(Transaction transaction)
        {
            await _unitOfWork.TransactionRepository.UpdateTransactionStatus(transaction.TransactionId, TransactionStatus.Failed);
            await _unitOfWork.OrderRepository.UpdateOrderStatus(transaction.OrderId, OrderStatus.Failed);
            await _unitOfWork.SaveChanges();
          
        }

       
        private async Task<PayPalCheckoutSdk.Orders.Order> CapturePayPalPayment(string token)
        {
            var request = new OrdersCaptureRequest(token);
            request.RequestBody(new OrderActionRequest());
            var response = await _payPalClient.Client().Execute(request);
            return response.Result<PayPalCheckoutSdk.Orders.Order>();
        }

       
        private async Task HandleTransactionResult(Transaction transaction, PayPalCheckoutSdk.Orders.Order result, string payerId)
        {
            if (result.Status == "COMPLETED")
            {
                await _unitOfWork.TransactionRepository.UpdateTransactionStatus(transaction.TransactionId, TransactionStatus.Completed);
                await _unitOfWork.OrderRepository.UpdateOrderStatus(transaction.OrderId, OrderStatus.Paid);
                await _unitOfWork.CartRepository.UpdateIsPurchased(transaction.Order.CartId, true);
            }
            else
            {
                await UpdateFailedTransaction(transaction);
            }

            await _unitOfWork.SaveChanges();
        }

        
    }
}
