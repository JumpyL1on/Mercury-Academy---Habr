using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Habr.BusinessLogic
{
    public static class UserManagerExtensions
    {
        public static async Task<List<Claim>> GetClaimsForAccessTokenGenerationAsync(
            this UserManager<User> userManager,
            User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public static async Task UpdateRefreshTokenInfoAsync(
            this UserManager<User> userManager,
            User user,
            string refreshToken,
            IConfiguration configuration)
        {
            user.RefreshToken = refreshToken;

            var days = int.Parse(configuration["Jwt:RefreshTokenExpiresInDays"]);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(days);

            await userManager.UpdateAsync(user);
        }

        public static async Task<User?> FindByRefreshTokenAsync(
            this UserManager<User> userManager,
            string refreshToken)
        {
            return await userManager.Users
                .FirstOrDefaultAsync(user => user.RefreshToken == refreshToken);
        }
    }
}