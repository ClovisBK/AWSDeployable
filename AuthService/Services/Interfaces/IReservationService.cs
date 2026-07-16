using AuthService.DTOs.ReservationDto;
using AuthService.Models.Enums;

namespace AuthService.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationResponseDto>> GetAllAsync();
        Task<ReservationResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ReservationResponseDto>> GetMyReservationsAsync(string userId);
        Task<IEnumerable<ReservationResponseDto>> GetByStatusAsync(ReservationStatus status);
        Task<ReservationResponseDto> CreateReservationAsync(string userId, ReservationRequestDto dto);
        Task<bool> FulfilReservationAsync(int reservationId, int dueDays = 14);
        Task<bool> CancelReservationAsync(int reservationId);
        Task<bool> NotifyNextInQueueAsync(int bookId);
        Task ProcessExpiredNotifiedReservationAsync();
    }
}
