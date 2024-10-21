
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
        public PaymentService(PayPalClient payPalClient, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _payPalClient = payPalClient;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Transaction> CreateTransaction(string userId, string paymentMethod, string description)
        {
            var transaction = new Transaction()
            {
                UserId = userId,
                PaymentMethod = paymentMethod,
                DateCreated = DateTime.Now,
                Status = TransactionStatus.Pending,   
                Description = description
            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);

            await _unitOfWork.SaveChanges();

            return transaction;
        }

        public async Task<string> CreatePaymentOrder(int orderId)
        {
            string returnUrl = _configuration["PayPalSettings:ReturnUrl"];
            string cancelUrl = _configuration["PayPalSettings:CancelUrl"];

            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderId == orderId, includeProperties: "Cart,Cart.CartItems");

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


            var transaction = await _unitOfWork.TransactionRepository.GetAsync(t => t.TransactionId == order.TransactionId);

            transaction.Token = token;

            transaction.Amount = amount;

            await _unitOfWork.SaveChanges();

            string paymentUrl = $"{approvalUrl}";
            return paymentUrl;
        }


        public async Task<TransactionDTO> CapturePayment(string token, string payerId)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(t => t.Token == token);

            try
            {
                var result = await CapturePayPalPayment(token);

                await HandleTransactionResult(transaction, result, payerId);

                var transactionDTO = _mapper.Map<TransactionDTO>(transaction);

                return transactionDTO;
            }
            catch (Exception ex)
            {
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
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.TransactionId == transaction.TransactionId);
            await _unitOfWork.OrderRepository.UpdateOrderStatus(order.OrderId, OrderStatus.Failed);
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
                var order = await _unitOfWork.OrderRepository.GetAsync(o => o.TransactionId == transaction.TransactionId);
                await _unitOfWork.OrderRepository.UpdateOrderStatus(order.OrderId, OrderStatus.Paid);
                await _unitOfWork.CartRepository.UpdateIsPurchased(order.CartId, true);
            }
            else
            {
                await UpdateFailedTransaction(transaction);
                await _unitOfWork.SaveChanges();
            }


        }

    }
}
