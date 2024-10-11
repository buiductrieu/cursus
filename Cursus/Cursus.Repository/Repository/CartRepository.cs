using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;

namespace Cursus.Repository.Repository
{
	public class CartRepository : Repository<Cart>, ICartRepository
	{
		private readonly CursusDbContext _db;
		public CartRepository(CursusDbContext db) : base(db) => _db = db;
	}
}
