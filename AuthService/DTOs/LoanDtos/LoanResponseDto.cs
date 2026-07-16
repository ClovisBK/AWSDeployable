using AuthService.Models.Enums;

namespace AuthService.DTOs.LoanDtos
{
    public class LoanResponseDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public string Username {  get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? DueDate { get; set ; }
        public DateTime? ReturnDate { get; set ; }
        public LoanStatus Status { get; set; }
    }
}
