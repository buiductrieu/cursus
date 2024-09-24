using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<Category> AddCategory(Category category)
        {
            await _categoryRepository.AddAsync(category);
            return category;
        }

        public async Task<Category> DeleteCategory(Category category)
        {
            await _categoryRepository.DeleteAsync(category);
            return category;
        }

        public async Task<IEnumerable<Category>> GetAllCategories(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null)
        {
            var output = await _categoryRepository.GetAllAsync(filter, includeProperties);
            return output;
        }

        public async Task<Category> GetCategory(Expression<Func<Category, bool>> filter, string? includeProperties = null)
        {
            var output = await _categoryRepository.GetAsync(filter, includeProperties);
            return output;
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
            return category;

        }
    }
}
