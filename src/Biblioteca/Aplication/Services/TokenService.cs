using Biblioteca.Aplication.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Biblioteca.Aplication.Services
{

    public class MapSecrets
    {
        public string Issuer { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessTokenMinutes { get; set; }
    }

    public class TokenService : ITokenService
    {
        private readonly MapSecrets _mapSecrets;
        private readonly IConfiguration _config;

        // Intentamos obtener TODOS los secretos desde IConfiguration.
        // En caso de que algun secreto no exista en User Secrets,
        // la aplicacion lanza una exepcion para notificarlo.
        // Sus valores son necesario para la creacion del JWT y el hash al Refresh Token para su guardado
        // en la base de datos.
        private MapSecrets GetSecrets()
        {
            var jwtKey = _config["Jwt:Key"];
            var audience = _config["Jwt:Audience"];
            var issuer = _config["Jwt:Issuer"];
            var accessTokenMinutes = _config["Jwt:AccessTokenMinutes"];

            if (
                string.IsNullOrWhiteSpace(jwtKey) ||
                string.IsNullOrWhiteSpace(accessTokenMinutes) ||
                string.IsNullOrWhiteSpace(audience) ||
                string.IsNullOrWhiteSpace(issuer)
                ) throw new InvalidOperationException("Secretos no configurados.");

            return new MapSecrets()
            {
                AccessTokenMinutes = int.Parse(accessTokenMinutes),
                Issuer = issuer,
                Audience = audience,
                Key = jwtKey
            };
        }

        public TokenService(IConfiguration _configuration)
        {
            _config = _configuration;
            _mapSecrets = GetSecrets();
        }
        // ACCESS TOKEN (JWT)
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            //Console.WriteLine("AQUI ciego KUN: " + _mapSecrets.Key);
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_mapSecrets.Key)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _mapSecrets.Issuer,
                audience: _mapSecrets.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _mapSecrets.AccessTokenMinutes
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // REFRESH TOKEN (random seguro)
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes);
        }

        // HASH del refresh token para guardar en BD
        public string HashToken(string token)
        {
            // Necesita la llave secreta para realizar un hash unico con el token y la clave secreta.
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_mapSecrets.Key));
            var bytes = Convert.FromBase64String(token);
            var hash = hmac.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
