using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Entities;
using SoundPlayer.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SoundPlayer.Domain.BE;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SoundPlayer.Domain.Common;

namespace SoundPlayer.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<BaseResponse<bool>> RegisterUser(UserDto dto)
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

                return new BaseResponse<bool>()
                {
                    Result = result.Succeeded,
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                _logger.LogInformation($"User {dto.Email} not registered. Exception : {e.Message}");
                return new BaseResponse<bool>()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
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
                    ErrorMessage = e.Message
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
                new (ClaimTypes.Name, user.UserName!)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
