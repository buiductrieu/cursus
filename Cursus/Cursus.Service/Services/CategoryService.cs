using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.DTO.Category;
using Cursus.Data.Entities;
using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Cursus.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService( IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PageListResponse<CategoryDTO>> GetCategoriesAsync(string? searchTerm, string? sortColumn, string? sortOrder, int page = 1, int pageSize = 20)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();


            // Apply search filter if searchTerm is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                categories = categories.Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Apply sorting if sortOrder is provided
            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                if (sortOrder?.ToLower() == "desc")
                {
                    categories = categories.OrderByDescending(GetSortProperty(sortColumn));
                }
                else
                {
                    categories = categories.OrderBy(GetSortProperty(sortColumn)).ToList();
                }
            }

            var totalCount = categories.Count();


            var paginatedCategpries = categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return new PageListResponse<CategoryDTO>
            {
                Items = MapCategoriesToDTOs(paginatedCategpries),//map to dto
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasNextPage = (page * pageSize) < totalCount,
                HasPreviousPage = pageSize > 1
            };
        }
        //public async Task<IEnumerable<Category>> GetAllCategories(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null)
        //{
        //    var output = await _categoryRepository.GetAllAsync(filter, includeProperties);
        //    return output;
        //}
        
        public async Task<CategoryDTO> GetCategoryById(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(c => c.Id == id) ?? throw new KeyNotFoundException("Category is not found");
            return _mapper.Map<CategoryDTO>(category);
        }
        public async Task<CategoryDTO> CreateCategory(CreateCategoryDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name)) throw new BadHttpRequestException("Category Name is required.");
            if (string.IsNullOrEmpty(dto.Description)) throw new BadHttpRequestException("Category Description is required.");
            var existingCategory = await _unitOfWork.CategoryRepository.AnyAsync(x => x.Name.Equals(dto.Name));

            if (existingCategory)
            {
                throw new BadHttpRequestException("A category with this name already exists.");
            }

            // Map CreateCategoryDTO to Category entity
            var newCategory = _mapper.Map<Category>(dto);

            // Add the new category to the database
            await _unitOfWork.CategoryRepository.AddAsync(newCategory);
            await _unitOfWork.SaveChanges();

            // Map the new entity to a CategoryDTO and return it
            var categoryDto = new CategoryDTO
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                Description = newCategory.Description,
                Status = newCategory.Status,
                ParentCategory = newCategory.ParentCategory
            };

            return categoryDto;
        }
        public async Task<CategoryDTO> UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(x => x.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            // Map the updated fields from UpdateCategoryDTO to the existing category
            _mapper.Map(dto, category);

            await _unitOfWork.SaveChanges();

            // Map updated category to CategoryDTO
            var updatedCategoryDto = _mapper.Map<CategoryDTO>(category);
            return updatedCategoryDto;
        }
        public async Task<bool> DeleteCategory(int id)
        {
            var Category = await _unitOfWork.CategoryRepository.GetAsync(c => c.Id == id);

            if (Category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            // Remove the category from the database
            await _unitOfWork.CategoryRepository.DeleteAsync(Category);
            await _unitOfWork.SaveChanges();


            return true;
        }

        

        // Query Categories
        private static Func<Category, object> GetSortProperty(string SortColumn)
        {
            return SortColumn?.ToLower() switch
            {
                "name" => category => category.Name,
                "description" => category => category.Description,
                "dateCreated" => category => category.DateCreated,
                _ => category => category.Id

            };
        }
        private List<CategoryDTO> MapCategoriesToDTOs(List<Category> categories)
        {
            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id, 
                Name = c.Name,
                Description = c.Description,
                DateCreated = c.DateCreated,
                Status = c.Status,
                IsParent = c.IsParent,
                ParentCategory = c.ParentCategory 
            }).ToList();
        }

        //
    }
}
