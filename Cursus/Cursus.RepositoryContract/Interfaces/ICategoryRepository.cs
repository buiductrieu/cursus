using Cursus.Data.DTO.Category;
using Cursus.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategory();
        Task<CategoryDTO> UpdateCategory(int id, UpdateCategoryDTO dto);
        Task<CategoryDTO> CreateCategory(CreateCategoryDTO dto);
        Task<CategoryDTO> DeleteCategory(int id);
    }
}
