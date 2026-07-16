using AuthService.Models;

namespace AuthService.Repositoriess.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> ExistsAsync(int id);
    }
}
