using AuthService.Models.Enums;

namespace AuthService.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public DateTime? NotifiedDate { get; set; }
        public ReservationStatus Status { get; set; }
        public Book? Book { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
