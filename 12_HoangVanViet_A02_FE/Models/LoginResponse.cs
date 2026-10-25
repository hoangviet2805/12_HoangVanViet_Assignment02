namespace _12_HoangVanViet_A02_FE.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public SystemAccount? Account { get; set; }
    }
}
