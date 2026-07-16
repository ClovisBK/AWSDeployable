using System.Runtime.InteropServices;
using System.Security.AccessControl;
using AuthService.DTOs.CategoryDtos;
using AuthService.Models;
using AuthService.Repositoriess.Interfaces;
using AuthService.Services.Interfaces;

namespace AuthService.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
            };
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return category;
        }
            
        

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if(category == null) return false;
           
            category.Name = dto.Name;
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
