
using Microsoft.Extensions.Logging;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Data.Enums;
using Cursus.Data.Entities;
using Cursus.ServiceContract.Interfaces;
using PayPalCheckoutSdk.Orders;

namespace Demo_PayPal.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PayPalClient _payPalClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger; 
        public PaymentService(PayPalClient payPalClient, ILogger<PaymentService> logger, IUnitOfWork unitOfWork)
        {
            _payPalClient = payPalClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        private async Task<(Cursus.Data.Entities.Order order, Cart cart, string userId, List<int> courseIds)> PreparePaymentData(int orderId)
        {
           
            var order = await _unitOfWork.OrderRepository.GetOrderWithCartAndItemsAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("The order does not exist.");
            }

            
            var cart = order.Cart;
            if (cart == null)
            {
                throw new InvalidOperationException("The cart does not exist for this order.");
            }

            
            var userId = cart.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("The UserId associated with the cart is invalid.");
            }

            
            var cartItems = cart.CartItems;
            if (cartItems == null || !cartItems.Any())
            {
                throw new InvalidOperationException("There are no items in the cart.");
            }

           
            var courseIds = cartItems.Select(ci => ci.CourseId).ToList();

           
            return (order, cart, userId, courseIds);
        }




        public async Task<string> CreatePayment(int orderId, string returnUrl = "https://your-default-return-url.com", string cancelUrl = "https://your-default-cancel-url.com")
        {
           

           
            var (order, cart, userId, courseIds) = await PreparePaymentData(orderId);

            var isCompleted = await _unitOfWork.TransactionRepository.IsOrderCompleted(orderId);
            if (isCompleted)
            {
                throw new InvalidOperationException("The order has already been completed.");
            }
            
            var amount = order.PaidAmount;
            if (amount <= 0)
            {
               
                throw new ArgumentException("The payment amount must be greater than 0.");
            }

          
            var pendingTransaction = await _unitOfWork.TransactionRepository.GetPendingTransaction(userId, orderId);
            if (pendingTransaction != null)
            {
               
                throw new InvalidOperationException("A pending transaction already exists for this order.");
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
                _logger.LogError("Invalid response from PayPal for OrderId {OrderId}. No approval link found.", orderId);
                throw new InvalidOperationException("Invalid response from PayPal. No approval link found.");
            }

            var approvalUrl = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
            if (string.IsNullOrEmpty(approvalUrl))
            {
                _logger.LogError("Unable to generate PayPal payment link for OrderId {OrderId}.", orderId);
                throw new InvalidOperationException("Unable to generate PayPal payment link.");
            }

            var token = result.Id;

           
            var transactionEntry = new Transaction
            {
                UserId = userId,
                OrderId = orderId,
                PaymentMethod = "PayPal",
                DateCreated = DateTime.UtcNow,
                Status = TransactionStatus.Pending,
                Token = token
            };

            await _unitOfWork.TransactionRepository.AddAsync(transactionEntry);
            await _unitOfWork.SaveChanges();


            return approvalUrl;
        }










        public async Task<Transaction> CapturePayment(string token, string userId, int orderId)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Invalid token.");

            // Retrieve pending transaction
            var pendingTransaction = await _unitOfWork.TransactionRepository.GetPendingTransaction(userId, orderId);
            if (pendingTransaction == null)
                throw new KeyNotFoundException("Pending transaction not found.");

            if (pendingTransaction.Token != token)
                throw new ArgumentException("Token does not match the transaction.");

            // Check for transaction expiration
            if (pendingTransaction.DateCreated <= DateTime.UtcNow.AddMinutes(-10))
            {
                await _unitOfWork.TransactionRepository.UpdateTransactionStatus(pendingTransaction.TransactionId, TransactionStatus.Failed);
                await _unitOfWork.SaveChanges();
                throw new InvalidOperationException("Transaction has expired and was canceled.");
            }

            // Send capture request to PayPal
            var request = new OrdersCaptureRequest(token);
            request.RequestBody(new OrderActionRequest());
            var response = await _payPalClient.Client().Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            // Check payment status
            if (result.Status == "COMPLETED")
            {
                await _unitOfWork.TransactionRepository.UpdateTransactionStatus(pendingTransaction.TransactionId, TransactionStatus.Completed);
            }
            else
            {
                await _unitOfWork.TransactionRepository.UpdateTransactionStatus(pendingTransaction.TransactionId, TransactionStatus.Failed);
                throw new InvalidOperationException("Payment was not successful.");
            }

          
            await _unitOfWork.SaveChanges();

            return pendingTransaction;
        }


    }
}
