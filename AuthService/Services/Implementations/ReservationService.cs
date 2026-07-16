using AuthService.DTOs.EmailDtos;
using AuthService.DTOs.ReservationDto;
using AuthService.Models;
using AuthService.Models.Enums;
using AuthService.Repositoriess.Interfaces;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userManager = userManager;
        }
        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null) return false;

            if(reservation.Status == ReservationStatus.Notified)
            {
                var hasNext = await NotifyNextInQueueAsync(reservation.BookId);
                if (!hasNext)
                {
                    var book = await _unitOfWork.Books.GetByIdAsync(reservation.BookId);
                    if(book != null)
                    {
                        book.AvailableCopies++;
                        _unitOfWork.Books.Update(book);
                    }
                }
            }
            reservation.Status = ReservationStatus.Cancelled;
            _unitOfWork.Reservations.Update(reservation);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(string userId, ReservationRequestDto dto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(dto.BookId);
            if(book == null)
                throw new KeyNotFoundException("Book not found.");
            if (book.AvailableCopies > 0)
                throw new InvalidOperationException("Book has available copies. Please a loan instead");

            var active = await _unitOfWork.Reservations.HasActiveReservationAsync(userId, dto.BookId);
            if (active)
                throw new InvalidOperationException("You already have an active reservation for this book");


            var reservation = new Reservation
            {
                BookId = dto.BookId,
                UserId = userId,
                ReservationDate = DateTime.UtcNow,
                Status = ReservationStatus.Waiting
            };

            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.CompleteAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if(user?.Email != null)
            {
                await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Reservation Confirmed — LibraGo",
                    Body = $"""
                        <h3>Hello {user.FullName ?? user.UserName},</h3>
                        <p>Your reservation for <strong>{book.Title}</strong> has been placed.</p>
                        <p>You are now in the queue. We will notify you as soon as a copy becomes available.</p>
                        <br/>
                        <p>Thank you,<br/><strong>QuickLib Library</strong></p>
                    """
                });
            }
            var created = await _unitOfWork.Reservations.GetByIdWithDetailsAsync(reservation.Id);
            return MapToDto(created!);
        }

        public async Task<bool> FulfilReservationAsync(int reservationId, int dueDays = 14)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdWithDetailsAsync(reservationId);
            if (reservation == null) return false;
            if (reservation.Status != ReservationStatus.Notified)
                throw new InvalidOperationException("Only notified reservations can be fulfilled.");
            

            var loan = new Loan
            {
                BookId = reservation.BookId,
                UserId = reservation.UserId,
                RequestDate = reservation.ReservationDate,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(dueDays),
                Status = LoanStatus.Active
            };

            await _unitOfWork.Loans.AddAsync(loan);

            reservation.Status = ReservationStatus.Fulfilled;
            _unitOfWork.Reservations.Update(reservation);
            await _unitOfWork.CompleteAsync();

            var user = await _userManager.FindByIdAsync(reservation.UserId);
            if(user?.Email != null)
            {
                await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Reservation Fulfilled — LibraGo",
                    Body = $"""
                <h3>Hello {user.FullName ?? user.UserName},</h3>
                <p>Your reservation for <strong>{reservation.Book!.Title}</strong> has been fulfilled.</p>
                <p>Please return the book by <strong>{loan.DueDate:MMMM dd, yyyy}</strong>.</p>
                <br/>
                <p>Thank you,<br/><strong>LibraGo Library</strong></p>
            """
                });
            } 

            return true;
        }

        public async Task<IEnumerable<ReservationResponseDto>> GetAllAsync()
        {
            var reservations = await _unitOfWork.Reservations.GetAllWithDetailsAsync();
            return reservations.Select(MapToDto);
        }

        public async Task<ReservationResponseDto?> GetByIdAsync(int id)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdWithDetailsAsync(id);
            return reservation == null ? null : MapToDto(reservation);
        }

        public async Task<IEnumerable<ReservationResponseDto>> GetByStatusAsync(ReservationStatus status)
        {
            var reservations = await _unitOfWork.Reservations.GetByStatusAsync(status);
            return reservations.Select(MapToDto);
        }

        public async Task<IEnumerable<ReservationResponseDto>> GetMyReservationsAsync(string userId)
        {
            var reservations = await _unitOfWork.Reservations.GetByUserIdAsync(userId);
            return reservations.Select(MapToDto);
        }

        public async Task<bool> NotifyNextInQueueAsync(int bookId)
        {
            var next = await _unitOfWork.Reservations.GetNextInQueueAsync(bookId);
            if (next == null) return false;

            next.Status = ReservationStatus.Notified;
            next.NotifiedDate = DateTime.UtcNow;
            _unitOfWork.Reservations.Update(next);
            await _unitOfWork.CompleteAsync();

            var user = await _userManager.FindByIdAsync(next.UserId);
            if (user?.Email != null)
            {
                await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Your Reserved Book Is Available — LibraGo",
                    Body = $"""
                        <h3>Hello {user.FullName ?? user.UserName},</h3>
                        <p>Great news! A copy of <strong>{next.Book!.Title}</strong> is now available for you.</p>
                        <p>Please visit the library within <strong>24 hours</strong> to collect it.</p>
                        <p>If you do not collect it within 24 hours, your reservation will be
                        cancelled and the next person in the queue will be notified.</p>
                        <br/>
                        <p><strong>QuickLib Library</strong></p>
                    """
                });
            }
            return true;
        }

        public async Task ProcessExpiredNotifiedReservationAsync()
        {
            var expired = await _unitOfWork.Reservations.GetExpiredNotifiedReservationsAsync();
            
            foreach(var reservation in expired)
            {
                reservation.Status = ReservationStatus.Cancelled;
                _unitOfWork.Reservations.Update(reservation);

                var user = await _userManager.FindByIdAsync(reservation.UserId);
                if(user?.Email != null)
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        To = user.Email,
                        Subject = "Reservation Expired — LibraGo",
                        Body = $"""
                            <h3>Hello {user.FullName ?? user.UserName},</h3>
                            <p>Your reservation for <strong>{reservation.Book!.Title}</strong> has
                            expired because the book was not collected within 24 hours.</p>
                            <p>You are welcome to place a new reservation at any time.</p>
                            <br/>
                            <p><strong>QuickLib Library</strong></p>
                        """
                    });
                }
                var hasNext = await NotifyNextInQueueAsync(reservation.BookId);

                if (!hasNext)
                {
                    var book = await _unitOfWork.Books.GetByIdAsync(reservation.BookId);
                    if(book != null)
                    {
                        book.AvailableCopies++;
                        _unitOfWork.Books.Update(book);
                    }
                }
            }
            await _unitOfWork.CompleteAsync();

        }

        private static ReservationResponseDto MapToDto(Reservation reservation) => new ReservationResponseDto
        {
            Id = reservation.Id,
            BookId = reservation.BookId,
            BookTitle = reservation.Book?.Title ?? string.Empty,
            BookAuthor = reservation.Book?.Author ?? string.Empty,
            Username = reservation.User?.FullName ?? string.Empty,
            ReservationDate = reservation.ReservationDate,
            NotifiedDate = reservation.NotifiedDate,
            Status = reservation.Status

        };
    }
}
