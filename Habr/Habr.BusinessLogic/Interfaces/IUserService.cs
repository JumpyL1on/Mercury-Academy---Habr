using Habr.Common.DTOs;
using Habr.Common.Requests;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        public Task<UserDTO> RegisterAsync(RegisterUserRequest request);
        public Task<UserDTO> LoginAsync(LoginUserRequest request);
        public Task<TokensDTO> RefreshTokenAsync(RefreshTokenRequest request);
    }
}