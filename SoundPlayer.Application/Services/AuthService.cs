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
                var user = new ApplicationUser
                {
                    UserName = dto.Username, 
                    Email = dto.Email,
                    CreatedTime = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, dto.Password);

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

        public async Task<BaseResponse<string>> LoginUser(LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user is null)
                {
                    throw new Exception("User not found!");
                }

                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(user);
                    _logger.LogInformation($"User '{dto.Email}' logined.");
                    return new BaseResponse<string>()
                    {
                        IsSuccess = true,
                        Result = token
                    };
                }

                throw new Exception();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"User '{dto.Email}' not logined. Exception : {e.Message}");
                return new BaseResponse<string>()
                {
                    IsSuccess = false,
                    ErrorMessage = e.Message
                };
            }
        }


        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
