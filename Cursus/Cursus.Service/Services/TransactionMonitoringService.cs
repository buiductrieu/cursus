using Cursus.RepositoryContract.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                           

                            if (transaction.DateCreated <= DateTime.Now.AddMinutes(-10))
                            {
                                _logger.LogInformation("Transaction {TransactionId} has expired. Updating status to Failed.", transaction.TransactionId);

                                // Update status of the transaction and order
                                await unitOfWork.TransactionRepository.UpdateTransactionStatus(transaction.TransactionId, Data.Enums.TransactionStatus.Failed);
                                await unitOfWork.OrderRepository.UpdateOrderStatus(transaction.OrderId, Data.Entities.OrderStatus.Failed);
                                await unitOfWork.SaveChanges();

                                _logger.LogInformation("Transaction {TransactionId} and Order {OrderId} have been marked as Failed", transaction.TransactionId, transaction.OrderId);
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
