using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;
using SoundPlayer.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SoundPlayer.Domain.Common;
using SoundPlayer.Domain.Constants;

namespace SoundPlayer.Application.Services
{
    /// <inheritdoc cref="IAuthService"/>
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IPlaylistService _playlistService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IPlaylistService playlistService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _playlistService = playlistService;
        }

        public async Task<BaseResponse> RegisterUser(UserDto dto)
        {
            try
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUserByEmail != null) throw new Exception("User with this email already exists.");

                var existingUserByLogin = await _userManager.FindByNameAsync(dto.Login);
                if (existingUserByLogin != null) throw new Exception("User with this login already exists.");

                var user = new ApplicationUser
                {
                    UserName = dto.Login,
                    Email = dto.Email,
                    CreatedTime = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded) throw new Exception("Invalid data");

                await _userManager.AddToRoleAsync(user, Role.User);
                _logger.LogInformation($"User {dto.Email} registered.");
                
                var response = await _playlistService.CreateFavoritePlaylist(user.Id);
                _logger.LogInformation($"User favorite playlist{(response.IsSuccess ? "" : " not")} created. {(response.IsSuccess ? "" : response.Message)}");

                return new BaseResponse(true);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"User {dto.Email} not registered. Exception : {e.Message}");
                return new BaseResponse(false, e.Message);
            }
        }

        public async Task<BaseResponse<(UserDto, string)>> LoginUser(UserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email) ?? throw new Exception("User not found!");
                var result = await _signInManager.PasswordSignInAsync(user, userDto.Password, false, false);
                if (!result.Succeeded) throw new Exception("Invalid password");
                var token = GenerateJwtToken(user);
                _logger.LogInformation($"User '{userDto.Email}' did login.");

                return new BaseResponse<(UserDto, string)>()
                {
                    IsSuccess = true,
                    Result = (
                        new UserDto()
                        {
                            Id = user.Id,
                            Email = user.Email,
                            Login = user.UserName,
                            CreatedAt = user.CreatedTime
                        }, await token)
                };
            }
            catch (Exception e)
            {
                _logger.LogWarning($"User '{userDto.Email}' didn't' login. Exception : {e.Message}");
                return new BaseResponse<(UserDto, string)>()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _signInManager.UserManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Name, user.UserName!),
                new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new (JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
