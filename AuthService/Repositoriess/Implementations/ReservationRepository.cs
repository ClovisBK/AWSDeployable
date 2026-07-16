using AuthService.Data;
using AuthService.Models;
using AuthService.Models.Enums;
using AuthService.Repositoriess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositoriess.Implementations
{

    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Reservation entity)
            => await _context.Reservations.AddAsync(entity);

        public async Task<IEnumerable<Reservation>> GetAllAsync()
            => await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .ToListAsync();

        public async Task<IEnumerable<Reservation>> GetAllWithDetailsAsync()
            => await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();

        public async Task<Reservation?> GetByIdAsync(int id)
            => await _context.Reservations.FindAsync(id);

        public async Task<Reservation?> GetByIdWithDetailsAsync(int id)
            => await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Reservation>> GetByStatusAsync(ReservationStatus status)
            => await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();

        public async Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId)
            => await _context.Reservations
            .Include(r => r.Book)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();

        public async Task<IEnumerable<Reservation>> GetExpiredNotifiedReservationsAsync()
            => await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .Where(r => r.Status == ReservationStatus.Notified &&
            r.ReservationDate == DateTime.UtcNow.AddHours(-24))
            .ToListAsync();

        public async Task<Reservation?> GetNextInQueueAsync(int bookId)
            => await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Waiting)
            .OrderBy(r => r.ReservationDate)
            .FirstOrDefaultAsync();

        public async Task<bool> HasActiveReservationAsync(string userId, int bookId)
            => await _context.Reservations
            .AnyAsync(
            r => r.UserId == userId &&
            r.BookId == bookId &&
            (r.Status == ReservationStatus.Waiting || r.Status == ReservationStatus.Notified));

        public void Remove(Reservation entity)
            => _context.Reservations.Remove(entity);

        public void Update(Reservation entity)
            => _context.Reservations.Update(entity);
    }
}
