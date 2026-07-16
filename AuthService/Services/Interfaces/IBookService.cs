using AuthService.DTOs.BookDtos;
using AuthService.Models;

namespace AuthService.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<IEnumerable<Book>> SearchAsync(string searchTerm);
        Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId);
        Task<Book> CreateAsync(BookDto dto);
        Task<bool> UpdateAsync(int id, BookDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
