
using Microsoft.Extensions.Logging;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Data.Enums;
using Cursus.Data.Entities;
using Cursus.ServiceContract.Interfaces;
using PayPalCheckoutSdk.Orders;
using Microsoft.AspNetCore.Http;
using Cursus.Data.DTO;
using AutoMapper;
using Microsoft.Extensions.Configuration;


namespace Demo_PayPal.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PayPalClient _payPalClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(
            PayPalClient payPalClient,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<PaymentService> logger) 
        {
            _payPalClient = payPalClient;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreatePayment(int orderId)
        {
            string returnUrl = _configuration["PayPalSettings:ReturnUrl"];
            string cancelUrl = _configuration["PayPalSettings:CancelUrl"];


            var order = await _unitOfWork.OrderRepository.GetOrderWithCartAndItemsAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("The order does not exist.");
            }

            if (order.Cart.IsPurchased)
            {
                throw new BadHttpRequestException("The cart has already been purchased. You cannot proceed with the payment.");
            }

           
            if (order.Status != Cursus.Data.Entities.OrderStatus.PendingPayment)
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
           
            var transaction = await _unitOfWork.TransactionRepository.GetTransactionByOrder(orderId);

            if (transaction == null)
            {
                throw new BadHttpRequestException("Transaction not found.");
            }

           
            if (transaction.Token != token)
            {
                throw new BadHttpRequestException("Token does not match the transaction.");
            }

           
            var request = new OrdersGetRequest(transaction.Token);
            var response = await _payPalClient.Client().Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

          

         
            if (result.Status == "APPROVED" || result.Status == "COMPLETED")
            {
                
                await UpdateTransactionToCompleted(transaction);
                return _mapper.Map<TransactionDTO>(transaction);
            }
            else if (result.Status == "CREATED")
            {
               
               
                await _unitOfWork.TransactionRepository.UpdateTransactionStatus(transaction.TransactionId, TransactionStatus.Failed);
                await _unitOfWork.OrderRepository.UpdateOrderStatus(transaction.OrderId, OrderStatus.Failed);
                await _unitOfWork.SaveChanges();
                return _mapper.Map<TransactionDTO>(transaction);
            }
            else
            {
                
               
                throw new BadHttpRequestException($"Payment failed or incomplete on PayPal. Status: {result.Status}");
            }
        }

       
        private async Task UpdateTransactionToCompleted(Transaction transaction)
        {
           
            await _unitOfWork.TransactionRepository.UpdateTransactionStatus(transaction.TransactionId, TransactionStatus.Completed);
            await _unitOfWork.OrderRepository.UpdateOrderStatus(transaction.OrderId, Cursus.Data.Entities.OrderStatus.Paid);
            await _unitOfWork.CartRepository.UpdateIsPurchased(transaction.Order.CartId, true);
            await _unitOfWork.SaveChanges();
        }




    }
}
