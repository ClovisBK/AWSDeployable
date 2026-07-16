using AuthService.Models;
using AuthService.Models.Enums;

namespace AuthService.Repositoriess.Interfaces
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetAllWithDetailsAsync();
        Task<Loan?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Loan>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status);
        Task<IEnumerable<Loan>> GetExpiredPendingLoansAsync();
        Task<bool> HasActiveLoansAsync(string userId, int bookId);
    }
}
