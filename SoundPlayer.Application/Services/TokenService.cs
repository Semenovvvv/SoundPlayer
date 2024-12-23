using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using SoundPlayer.Domain.Entities;
using SoundPlayer.Application.Extensions;
using Microsoft.Extensions.Configuration;

namespace SoundPlayer.Application.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public string CreateToken(ApplicationUser user, List<IdentityRole<int>> roles)
        //{
        //    var token = user
        //        .CreateClaims(roles).ToList()
        //        .CreateJwtToken(_configuration);

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    return tokenHandler.WriteToken(token);
        //}
    }
}
