namespace Biblioteca.Aplication.DTOs.Auth
{
    public class TokenResultDTO
    {
        public string RefreshToken { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public short DiasParaExpirar {  get; set; }
    }
}
