using AuthService.DTOs.EmailDtos;
using AuthService.DTOs.LoanDtos;
using AuthService.Models;
using AuthService.Models.Enums;
using AuthService.Repositoriess.Interfaces;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReservationService _reservationService;

        public LoanService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            IReservationService reservationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userManager = userManager;
            _reservationService = reservationService;
        }
        public async Task<bool> ApproveLoanAsync(int loanId, int dueDays = 14)
        {
            var loan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loanId);
            if (loan == null) return false;

            if (loan.Status != LoanStatus.Pending)
                throw new InvalidOperationException("Only pending loans can be approved.");

            loan.Status = LoanStatus.Active;
            loan.BorrowDate = DateTime.UtcNow;
            loan.DueDate = DateTime.UtcNow.AddDays(dueDays);

            _unitOfWork.Loans.Update(loan);
            await _unitOfWork.CompleteAsync();

            var user = await _userManager.FindByIdAsync(loan.UserId);
            if(user?.Email != null)
            {
                await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Loan Approved - QuickLib",
                    Body = $"""
                    <h3>Hello {user.FullName ?? user.UserName},</h3>
                    <p>Your loan for <strong>{loan.Book!.Title}</strong> has been approved.</p>
                    <p>Please return the book by <strong>{loan.DueDate:MMMM dd, yyyy}</strong>.</p>
                    <br/>
                    <p>Thank you,<br/><strong>LibraGo Library</strong></p>
                    """
                });
            }
            return true;
        }

        public async Task CancelExpiredLoansAsync()
        {
            var expiredLoans = await _unitOfWork.Loans.GetExpiredPendingLoansAsync();

            foreach(var loan in expiredLoans)
            {
                loan.Status = LoanStatus.Cancelled;

                var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
                if(book != null)
                {
                    book.AvailableCopies++;
                    _unitOfWork.Books.Update(book);
                }
                _unitOfWork.Loans.Update(loan);

                var user = await _userManager.FindByIdAsync(loan.UserId);
                if(user?.Email != null)
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        To = user.Email,
                        Subject = "Loan Request Cancelled — LibraGo",
                        Body = $"""
                            <h3>Hello {user.FullName ?? user.UserName},</h3>
                            <p>Your loan request for <strong>{loan.Book!.Title}</strong> has been
                            automatically cancelled because the book was not collected within 24 hours.</p>
                            <p>You are welcome to make a new request at any time.</p>
                            <br/>
                            <p><strong>QuickLib Library</strong></p>
                        """
                    });
                }
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> CancellationAsync(int loanId)
        {
            var loan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loanId);
            if (loan == null) return false;
            if (loan.Status != LoanStatus.Pending)
                throw new InvalidOperationException("Only pending loans can be cancelled");

            loan.Status = LoanStatus.Cancelled;

            var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
            if(book != null)
            {
                book.AvailableCopies++;
                _unitOfWork.Books.Update(book);
            }
            _unitOfWork.Loans.Update(loan);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<LoanResponseDto>> GetAllAsync()
        {
          var loans =  await _unitOfWork.Loans.GetAllWithDetailsAsync();
            return loans.Select(MapToDto);

        }

        public async Task<LoanResponseDto?> GetByIdAsync(int id)
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(id);
            return loan == null ? null : MapToDto(loan);
        }
        

        public async Task<IEnumerable<LoanResponseDto>> GetByStatusAsync(LoanStatus status)
        {
            var loans = await _unitOfWork.Loans.GetByStatusAsync(status);
            return loans.Select(MapToDto);
        }

        public async Task<IEnumerable<LoanResponseDto>> GetMyLoansAsync(string userId)
        {
             var loans = await _unitOfWork.Loans.GetByUserIdAsync(userId);
            return loans.Select(MapToDto);
        }
          

        public async Task<LoanResponseDto> RequestLoanAsync(string userId, LoanRequestDto dto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(dto.BookId);
            if (book == null)
                throw new KeyNotFoundException("Book not found.");
            if (book.AvailableCopies == 0)
            {
                throw new InvalidOperationException("No available copies to loan");
            }
            var existingLoans = await _unitOfWork.Loans.HasActiveLoansAsync(userId, dto.BookId);
            if (existingLoans)
                throw new InvalidOperationException("You already have a pending or active loan for this book.");

            book.AvailableCopies--;
            _unitOfWork.Books.Update(book);

            var loan = new Loan
            {
                BookId = dto.BookId,
                UserId = userId,
                RequestDate = DateTime.UtcNow,
                Status = LoanStatus.Pending
            };
            await _unitOfWork.Loans.AddAsync(loan);
            await _unitOfWork.CompleteAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if(user?.Email != null)
            {
                await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Loan Request Received - QuickLib",
                    Body = $"""
 <h3>Hello {user.FullName ?? user.UserName},</h3>
 <p>Your loan request for <strong>{book.Title} has been received.</strong></p>
 <p>Please visit the library with <strong>24 hours</strong> to collect your book.</p>
 <p>IF you do not collect it in time, the request will be automatically cancelled
 and the copy will be released to others.</p>
 <br/>
 <p>Thank you, <br/><strong>QuickLib</strong></p>

"""
                });
            }
            var createdLoan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loan.Id);
            return MapToDto(createdLoan!);
        }

        public async Task<bool> ReturnLoanAsync(int loanId)
        {
            var loan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loanId);
            if (loan == null) return false;
            if (loan.Status != LoanStatus.Active)
                throw new InvalidOperationException("Only active loans can be returned.");

            loan.Status = LoanStatus.Returned;
            loan.ReturnDate = DateTime.UtcNow;
            _unitOfWork.Loans.Update(loan);

            var reservationFound = await _reservationService.NotifyNextInQueueAsync(loan.BookId);
            if (!reservationFound)
            {
                var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
                if(book != null)
                {
                    book.AvailableCopies++;
                    _unitOfWork.Books.Update(book);
                }

            }
            await _unitOfWork.CompleteAsync();
            var user = await _userManager.FindByIdAsync(loan.UserId);
            if(user?.Email != null)
            {
                await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = user.Email,
                    Subject = "Book Returned - QuickLib",
                    Body = $"""
                    <h3>Hello {user.FullName ?? user.UserName},</h3>
                    <p>Your return of <strong>{loan.Book!.Title}</strong> has been recorded.</p>
                    <p>Thank you for returning it on time. We hope to see you again soon.</p>
                    <br/>
                    <p><strong>QuickLib Library</strong></p>

                    """
                });
            }
            return true;
        }
        private static LoanResponseDto MapToDto(Loan loan) => new LoanResponseDto
        {
            Id = loan.Id,
            BookId = loan.BookId,
            BookTitle = loan.Book?.Title ?? string.Empty,
            BookAuthor = loan.Book?.Author?? string.Empty,
            Username = loan.User?.FullName ?? string.Empty,
            RequestDate = loan.RequestDate,
            BorrowDate = loan.BorrowDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status
        };
    }
}
