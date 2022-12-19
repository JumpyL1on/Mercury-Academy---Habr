using Habr.Common.DTOs;
using System.Security.Claims;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IJwtService
    {
        public TokensDTO GenerateAccessAndRefreshTokens(List<Claim> claims);
    }
}