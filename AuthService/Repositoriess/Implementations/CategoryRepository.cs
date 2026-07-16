using System.Threading.Tasks;
using AuthService.Data;
using AuthService.Models;
using AuthService.Repositoriess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AuthService.Repositoriess.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

         public async Task<IEnumerable<Category>> GetAllAsync()
               => await _context.Categories.ToListAsync();
        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories.FindAsync(id);
        public async Task AddAsync(Category entity)
            => await _context.Categories.AddAsync(entity);
        public  void Update(Category entity)
            => _context.Categories.Update(entity);

        public void Remove(Category entity)
            => _context.Categories.Remove(entity);
      
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }
    }
}
