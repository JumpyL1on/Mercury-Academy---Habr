using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Habr.BusinessLogic.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokensDTO GenerateAccessAndRefreshTokens(List<Claim> claims)
        {
            return new TokensDTO
            {
                AccessToken = GenerateAccessToken(claims),
                RefreshToken = GenerateRefreshToken()
            };
        }

        private string GenerateAccessToken(List<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                null,
                DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:AccessTokenExpiresInHours"])),
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var data = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            return Convert.ToBase64String(data);
        }
    }
}