using AuthService.Models;
using AuthService.Models.Enums;

namespace AuthService.Repositoriess.Interfaces
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetAllWithDetailsAsync();
        Task<Reservation?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Reservation>> GetByStatusAsync(ReservationStatus status);
        Task<Reservation?> GetNextInQueueAsync(int bookId);
        Task<IEnumerable<Reservation>> GetExpiredNotifiedReservationsAsync();
        Task<bool> HasActiveReservationAsync(string userId, int bookId);
    }
}
