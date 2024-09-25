using Cursus.Data.DTO.Category;
using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly CursusDbContext _db;
        public CategoryRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllCategory()
        {
            var Categories = await _db.Categories.ToListAsync();

            // Map to CategoryDTO
            var CategoryDTOs = Categories.Select(Category => new CategoryDTO
            {
                Id = Category.Id,
                Name = Category.Name,
                Description = Category.Description,
                Status = Category.Status,
                ParentCategory = Category.ParentCategory
            });

            return CategoryDTOs;
        }
        public async Task<CategoryDTO> CreateCategory(CreateCategoryDTO dto)
        {
            var existingCategory = await _db.Categories.FirstOrDefaultAsync(x => x.Name.Equals(dto.Name));

            if (existingCategory != null)
            {
                throw new Exception("A category with this name already exists.");
            }

            // Create a new category entity
            var newCategory = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status,
                ParentCategory = dto.ParentCategory
            };

            // Add the new category to the database
            _db.Categories.Add(newCategory);
            await _db.SaveChangesAsync();

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
            var Category = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (Category == null)
            {
                throw new Exception("Category not found.");
            }

            // Update the category fields
            Category.Name = dto.Name;
            Category.Description = dto.Description;
            Category.Status = dto.Status;
            Category.ParentCategory = dto.ParentCategory;

            await _db.SaveChangesAsync();

            // Map updated category to CategoryDTO
            var updatedCategoryDto = new CategoryDTO
            {
                Id = Category.Id,
                Name = Category.Name,
                Description = Category.Description,
                Status = Category.Status,
                ParentCategory = Category.ParentCategory
            };

            return updatedCategoryDto;
        }

        public async Task<CategoryDTO> DeleteCategory(int id)
        {
            var Category = await _db.Categories.FindAsync(id);

            if (Category == null)
            {
                throw new Exception("Category not found.");
            }

            // Remove the category from the database
            _db.Categories.Remove(Category);
            await _db.SaveChangesAsync();

            // Map deleted category to CategoryDTO
            var DeletedCategoryDto = new CategoryDTO
            {
                Id = Category.Id,
                Name = Category.Name,
                Description = Category.Description,
                Status = Category.Status,
                ParentCategory = Category.ParentCategory
            };

            return DeletedCategoryDto;
        } 
    }
}
