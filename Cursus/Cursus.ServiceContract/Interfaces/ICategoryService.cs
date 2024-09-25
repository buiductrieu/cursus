using Cursus.Data.DTO.Category;
using Cursus.Data.Entities;
using System.Linq.Expressions;

namespace Cursus.ServiceContract.Interfaces
{
    public interface ICategoryService
    {
        //Task<IEnumerable<Category>> GetAllCategories(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null);
        //Task<Category> GetCategory(Expression<Func<Category,bool>> filter, string? includeProperties = null);
        Task<IEnumerable<CategoryDTO>> GetAllCategory();
        Task<CategoryDTO> UpdateCategory(int id, UpdateCategoryDTO dto);
        Task<CategoryDTO> CreateCategory(CreateCategoryDTO dto);
        Task<CategoryDTO> DeleteCategory(int id);
    }
}
