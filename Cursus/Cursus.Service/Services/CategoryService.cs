using Cursus.Data.DTO.Category;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System.Linq.Expressions;

namespace Cursus.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<CategoryDTO> CreateCategory(CreateCategoryDTO dto)
        {
            return _categoryRepository.CreateCategory(dto);
        }

        public Task<CategoryDTO> DeleteCategory(int id)
        {
            return _categoryRepository.DeleteCategory(id);
        }

        public Task<IEnumerable<CategoryDTO>> GetAllCategory()
        {
            return _categoryRepository.GetAllCategory();
        }

        public Task<CategoryDTO> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            return _categoryRepository.UpdateCategory(id, dto);
        }
    }
}
