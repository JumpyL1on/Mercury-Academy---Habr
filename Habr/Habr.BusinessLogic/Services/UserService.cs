using AutoMapper;
using Habr.BusinessLogic.Interfaces;
using Habr.Common.DTOs;
using Habr.Common.Exceptions;
using Habr.Common.Requests;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<User> userManager,
            IMapper mapper,
            IJwtService jwtService,
            IConfiguration configuration,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<UserDTO> RegisterAsync(RegisterUserRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new BusinessException(ExceptionMessageResource.EmailIsAlreadyTaken);
            }

            await _userManager.AddToRoleAsync(user, request.Role);

            var claims = await _userManager.GetClaimsForAccessTokenGenerationAsync(user);

            var dto = _mapper.Map<UserDTO>(user);

            dto.Tokens = _jwtService.GenerateAccessAndRefreshTokens(claims);

            await _userManager.UpdateRefreshTokenInfoAsync(user, dto.Tokens.RefreshToken, _configuration);

            _logger.LogInformation(LoggerMessageResource.UserWasSuccessfullyRegistered);

            return dto;
        }

        public async Task<UserDTO> LoginAsync(LoginUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new BusinessException(ExceptionMessageResource.EmailIsIncorrect);
            }

            var claims = await _userManager.GetClaimsForAccessTokenGenerationAsync(user);

            var dto = _mapper.Map<UserDTO>(user);

            dto.Tokens = _jwtService.GenerateAccessAndRefreshTokens(claims);

            await _userManager.UpdateRefreshTokenInfoAsync(user, dto.Tokens.RefreshToken, _configuration);

            _logger.LogInformation(LoggerMessageResource.UserWasSuccessfullyLogined);

            return dto;
        }

        public async Task<TokensDTO> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _userManager.FindByRefreshTokenAsync(request.RefreshToken);

            if (user == null || user.RefreshToken != request.RefreshToken)
            {
                throw new BusinessException(ExceptionMessageResource.IncorrectRefreshToken);
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new BusinessException(ExceptionMessageResource.ExpiredRefreshToken);
            }

            var claims = await _userManager.GetClaimsForAccessTokenGenerationAsync(user);

            var tokens = _jwtService.GenerateAccessAndRefreshTokens(claims);

            await _userManager.UpdateRefreshTokenInfoAsync(user, tokens.RefreshToken, _configuration);

            return tokens;
        }
    }
}