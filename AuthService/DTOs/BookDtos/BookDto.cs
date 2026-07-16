namespace AuthService.DTOs.BookDtos
{
    public class BookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Copies { get; set; }
        public int CategoryId { get; set; }
    }
}
