using Cursus.Data.DTO;

namespace Cursus.ServiceContract.Interfaces
{
	public interface ICartService
	{
		public Task AddCourseToCartAsync(int courseId, string userId);

		public Task<CartDTO> GetCartByUserIdAsync(string userId);
	}
}
