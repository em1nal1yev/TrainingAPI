using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Services;
using TelimAPI.Domain.Entities;
using TelimAPI.Persistence.Options;

namespace TelimAPI.Persistence.Services
{
    public class TokenService:ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public TokenService(IConfiguration configrat, IOptions<JwtSettings> jwtOptions)
        {
            _configuration = configrat;
            _jwtSettings = jwtOptions.Value;
        }

        public string CreateAccessToken(User user, IList<string> roles)
        {

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken CreateRefreshToken(Guid userId)
        {

            var refreshTokenExpirationInDays = double.Parse(_configuration.GetSection("JwtSettings")["RefreshTokenExpirationInDays"]);

            return new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationInDays),
                IsRevoked = false
            };
        }
    }
}
