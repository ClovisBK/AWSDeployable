using AuthService.Data;
using AuthService.Models;
using AuthService.Models.Enums;
using AuthService.Repositoriess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositoriess.Implementations
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;
        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Loan entity)
            => await _context.Loans.AddAsync(entity);

        public async Task<IEnumerable<Loan>> GetAllAsync()
         => await _context.Loans
            .Include(i => i.Book)
            .Include(i => i.User)
            .ToListAsync();

        public async Task<IEnumerable<Loan>> GetAllWithDetailsAsync()
        => await _context.Loans
            .Include(i => i.Book)
            .Include(i => i.User)
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync();

        public async Task<Loan?> GetByIdAsync(int id)
        => await _context.Loans.FindAsync(id);

        public async Task<Loan?> GetByIdWithDetailsAsync(int id)
            => await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status)
            => await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .Where(l => l.Status == status)
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync();

        public async Task<IEnumerable<Loan>> GetByUserIdAsync(string userId)
            => await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.UserId == userId)
            .ToListAsync();

        public async Task<IEnumerable<Loan>> GetExpiredPendingLoansAsync()
            => await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.User)
            .Where(l => l.Status == LoanStatus.Pending &&
                      l.RequestDate <= DateTime.UtcNow.AddHours(-24))
            .ToListAsync();

        public async Task<bool> HasActiveLoansAsync(string userId, int bookId)
            => await _context.Loans
            .AnyAsync(l => l.UserId == userId && l.BookId == bookId && 
                (l.Status == LoanStatus.Pending || l.Status == LoanStatus.Active));

        public void Remove(Loan entity)
        => _context.Loans.Remove(entity);

        public void Update(Loan entity)
            => _context.Loans.Update(entity);
    }
}
