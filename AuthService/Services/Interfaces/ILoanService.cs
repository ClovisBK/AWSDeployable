using AuthService.DTOs.LoanDtos;
using AuthService.Models;
using AuthService.Models.Enums;

namespace AuthService.Services.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanResponseDto>> GetAllAsync();
        Task<LoanResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<LoanResponseDto>> GetMyLoansAsync(string userId);
        Task<IEnumerable<LoanResponseDto>> GetByStatusAsync(LoanStatus status);
        Task<LoanResponseDto> RequestLoanAsync(string userId, LoanRequestDto dto);
        Task<bool> ApproveLoanAsync(int loanId, int dueDays = 14);
        Task<bool> ReturnLoanAsync(int loanId);
        Task<bool> CancellationAsync(int loanId);
        Task CancelExpiredLoansAsync();
    }
}
