using AuthService.Data;
using AuthService.Models;
using AuthService.Repositoriess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositoriess.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Book entity)
         => await _context.Books.AddAsync(entity);

        public async Task<bool> ExistsAsync(int id)
            => await _context.Books.AnyAsync(c => c.Id == id);



        public async Task<IEnumerable<Book>> GetAllAsync()
          => await _context.Books.Include(b => b.Category).ToListAsync();

        public async Task<IEnumerable<Book>> GetAllWithCategoryAsync()
            => await _context.Books.Include(b => b.Category).ToListAsync();
       
        public async Task<IEnumerable<Book>> GetByCategoryAsync(int CategoryId)
          => await _context.Books
            .Include(b => b.Category)
            .Where(b => b.CategoryId == CategoryId)
            .ToListAsync();

        public async Task<Book?> GetByIdAsync(int id)
         => await _context.Books.FindAsync(id);

        public async Task<Book?> GetByIdWithCategoryAsync(int id)
         => await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        public void Remove(Book entity)
            => _context.Books.Remove(entity);

        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
         => await _context.Books
            .Include(b => b.Category)
            .Where(b => b.Title.Contains(searchTerm) ||
            b.Author.Contains(searchTerm) ||
            b.Category!.Name.Contains(searchTerm))
            .ToListAsync();
        public void Update(Book entity)
          => _context.Books.Update(entity);
          
        
    }
}
