namespace FlowerSys.Frontend.Models
{
    public class AuthResponse
    {
        public string Nombre { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        public string Token { get; set; } = string.Empty;

    }
}
