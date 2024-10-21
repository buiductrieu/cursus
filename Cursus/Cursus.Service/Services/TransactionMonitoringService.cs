using Cursus.RepositoryContract.Interfaces;
using Demo_PayPal.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayPalCheckoutSdk.Orders;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class TransactionMonitoringService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TransactionMonitoringService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        public TransactionMonitoringService(IServiceScopeFactory scopeFactory, ILogger<TransactionMonitoringService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var payPalClient = scope.ServiceProvider.GetRequiredService<PayPalClient>(); // Inject PayPal client

                    _logger.LogInformation("Starting to check pending transactions at {Time}", DateTime.Now);

                    try
                    {
                        var pendingTransactions = await unitOfWork.TransactionRepository.GetPendingTransactions();

                        if (!pendingTransactions.Any())
                        {
                            _logger.LogInformation("No pending transactions found at {Time}", DateTime.Now);
                        }

                        foreach (var transaction in pendingTransactions)
                        {                            
                            if (transaction.DateCreated <= DateTime.Now.AddMinutes(-1))
                            {
                                try
                                {
                                    var request = new OrdersGetRequest(transaction.Token);
                                    var response = await payPalClient.Client().Execute(request);
                                    var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

                                    // Kiểm tra trạng thái đơn hàng từ PayPal
                                    if (result.Status == "CREATED" || result.Status == "APPROVED")
                                    {
                                        _logger.LogInformation("Transaction {TransactionId} is in a cancellable state on PayPal.", transaction.TransactionId);

                                        // Hủy hoặc không thực hiện capture thanh toán
                                        // Hãy quyết định bỏ qua việc capture này hoặc thông báo lỗi cho người dùng
                                        _logger.LogInformation("Skipping capture for transaction {TransactionId} as it has expired.", transaction.TransactionId);

                                        // Cập nhật trạng thái giao dịch trên hệ thống của bạn thành Failed nếu chưa làm
                                        await unitOfWork.TransactionRepository.UpdateTransactionStatus(transaction.TransactionId, Data.Enums.TransactionStatus.Failed);
                                        await unitOfWork.SaveChanges();

                                        _logger.LogInformation("Transaction {TransactionId} has been marked as failed due to expiration.", transaction.TransactionId);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Transaction {TransactionId} is not in a cancellable state on PayPal.", transaction.TransactionId);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "An error occurred while retrieving the transaction {TransactionId} from PayPal.", transaction.TransactionId);
                                }                              
                                await unitOfWork.TransactionRepository.UpdateTransactionStatus(transaction.TransactionId, Data.Enums.TransactionStatus.Failed);

                                var order = await unitOfWork.OrderRepository.GetAsync(o => o.TransactionId == transaction.TransactionId);
                                await unitOfWork.OrderRepository.UpdateOrderStatus(order.OrderId, Data.Entities.OrderStatus.Failed);
                                await unitOfWork.SaveChanges();

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while checking pending transactions at {Time}", DateTime.Now);
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
