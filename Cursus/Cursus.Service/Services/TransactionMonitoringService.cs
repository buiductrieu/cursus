using Cursus.RepositoryContract.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class TransactionMonitoringService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public TransactionMonitoringService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                    var pendingTransactions = await transactionRepository.GetPendingTransactions();

                    foreach (var transaction in pendingTransactions)
                    {
                        
                        if (transaction.DateCreated <= DateTime.UtcNow.AddMinutes(-10))
                        {
                            await transactionRepository.UpdateTransactionStatus(transaction.TransactionId, Data.Enums.TransactionStatus.Failed);
                        }
                    }
                }               
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }

}
