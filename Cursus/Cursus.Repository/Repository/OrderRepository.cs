using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;

namespace Cursus.Repository.Repository
{
	public class OrderRepository : Repository<Order>, IOrderRepository
	{
		private readonly CursusDbContext _db;
		public OrderRepository(CursusDbContext db) : base(db) => _db = db;

	}
}
