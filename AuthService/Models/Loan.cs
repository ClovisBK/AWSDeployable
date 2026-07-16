

using AuthService.Models.Enums;

namespace AuthService.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; }

        public Book? Book { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
