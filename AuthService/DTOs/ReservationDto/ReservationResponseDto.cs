using AuthService.Models.Enums;

namespace AuthService.DTOs.ReservationDto
{
    public class ReservationResponseDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public string Username {  get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }   
        public DateTime? NotifiedDate { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
