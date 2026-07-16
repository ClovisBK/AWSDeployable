using AuthService.Models;

namespace AuthService.Repositoriess.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<IEnumerable<Book>> GetAllWithCategoryAsync();
        Task<Book?> GetByIdWithCategoryAsync(int id);
        Task<IEnumerable<Book>> SearchAsync(string searchTerm);
        Task<IEnumerable<Book>> GetByCategoryAsync(int CategoryId);
        Task<bool> ExistsAsync(int id);
    }
}
