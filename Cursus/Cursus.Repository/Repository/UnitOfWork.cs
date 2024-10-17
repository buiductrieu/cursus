using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly CursusDbContext _db;

        public ICategoryRepository CategoryRepository { get; }
        public IInstructorInfoRepository InstructorInfoRepository { get; private set; }
        public ICourseRepository CourseRepository { get; }
        public IStepRepository StepRepository { get; }
        public IUserRepository UserRepository { get; }
        public IStepContentRepository StepContentRepository { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public ICourseCommentRepository CourseCommentRepository { get; }
        public ITransactionRepository TransactionRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IStepCommentRepository StepCommentRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IProgressRepository ProgressRepository { get; }
        public ICartRepository CartRepository { get; }
        public ICourseProgressRepository CourseProgressRepository { get; }
        public ICartItemsRepository CartItemsRepository { get; }
        public IBookmarkRepository BookmarkRepository { get; }
        public IReasonRepository ReasonRepository{ get; }

        public UnitOfWork(CursusDbContext db, ICategoryRepository categoryRepository, ICourseRepository courseRepository, IStepRepository stepRepository, IUserRepository userRepository, IStepContentRepository stepContentRepository, IInstructorInfoRepository instructorInfoRepository, UserManager<ApplicationUser> userManager, ICourseCommentRepository courseCommentRepository, IRefreshTokenRepository refreshTokenRepository, IStepCommentRepository stepCommentRepository, IProgressRepository progressRepository, ICartRepository cartRepository, IOrderRepository orderRepository, ICourseProgressRepository courseProgressRepository, IBookmarkRepository bookmarkRepository, ICartItemsRepository cartItemsRepository, ITransactionRepository transactionRepository, IReasonRepository reasonRepository)
        {
            _db = db;
            CategoryRepository = categoryRepository;
            CourseRepository = courseRepository;
            StepRepository = stepRepository;
            UserRepository = userRepository;
            InstructorInfoRepository = instructorInfoRepository;
            UserManager = userManager;
            StepContentRepository = stepContentRepository;
            CourseCommentRepository = courseCommentRepository;
            RefreshTokenRepository = refreshTokenRepository;
            StepCommentRepository = stepCommentRepository;
            RefreshTokenRepository = refreshTokenRepository;
            ProgressRepository = progressRepository;
            TransactionRepository = transactionRepository;
            OrderRepository = orderRepository;
            CartRepository = cartRepository;
            CourseProgressRepository = courseProgressRepository;
            CartItemsRepository = cartItemsRepository;
            BookmarkRepository = bookmarkRepository;
            CartItemsRepository = cartItemsRepository;
            TransactionRepository = transactionRepository;
            ReasonRepository = reasonRepository;
        }


        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }
    }
}

