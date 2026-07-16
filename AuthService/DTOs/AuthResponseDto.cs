namespace AuthService.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToKen { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
    }
}
