using Cursus.Data.Entities;
using System.Linq.Expressions;

namespace Cursus.ServiceContract.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null);
        Task<Category> GetCategory(Expression<Func<Category,bool>> filter, string? includeProperties = null);
        Task<Category> AddCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task<Category> DeleteCategory(Category category);
    }
}
