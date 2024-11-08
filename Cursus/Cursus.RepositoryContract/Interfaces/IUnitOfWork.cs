using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IUserRepository UserRepository { get; }
        IInstructorInfoRepository InstructorInfoRepository { get; }
        ICourseRepository CourseRepository { get; }
        IStepRepository StepRepository { get; }
        IStepContentRepository StepContentRepository { get; }
        ICourseCommentRepository CourseCommentRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IProgressRepository ProgressRepository { get; }
        IStepCommentRepository StepCommentRepository { get; }
        ICartItemsRepository CartItemsRepository { get; }
        IBookmarkRepository BookmarkRepository { get; }
        ICartRepository CartRepository { get; }
        IOrderRepository OrderRepository { get; }
        ICourseProgressRepository CourseProgressRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IReasonRepository ReasonRepository { get; }
        IWalletRepository WalletRepository { get; }
        IPlatformWalletRepository PlatformWalletRepository { get; }
        ICertificateRepository CertificateRepository { get; }
        IPayoutRequestRepository PayoutRequestRepository { get; }
        IWalletHistoryRepository WalletHistoryRepository { get; }
        IInstructorDashboardRepository InstructorDashboardRepository { get; }
        IArchivedTransactionRepository ArchivedTransactionRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IAdminDashboardRepository AdminDashboardRepository { get; }
        Task SaveChanges();
    }
}
