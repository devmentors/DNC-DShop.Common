using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace DShop.Common.Authentication
{
    public class JwtHandler : IJwtHandler
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly JwtOptions _options;
        private readonly SigningCredentials _signingCredentials;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtHandler(JwtOptions options)
        {
            _options = options;
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            _signingCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256);
            _tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = issuerSigningKey,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.ValidAudience,
                ValidateAudience = _options.ValidateAudience,
                ValidateLifetime = _options.ValidateLifetime
            }; 
        }

        public JsonWebToken CreateToken(Guid userId, string role)
        {
            var now = DateTime.UtcNow;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToTimestamp().ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var expires = now.AddMinutes(_options.ExpiryMinutes);
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JsonWebToken
            {
                AccessToken = token,
                Expires = expires.ToTimestamp()
            };
        }

        public JsonWebTokenPayload GetTokenPayload(string accessToken)
        {

            _jwtSecurityTokenHandler.ValidateToken(accessToken, _tokenValidationParameters, 
                out SecurityToken validatedSecurityToken);
            if (!(validatedSecurityToken is JwtSecurityToken jwt))
            {
                return null;
            }
    
            return new JsonWebTokenPayload
            {
                Subject = jwt.Subject,
                Role = jwt.Claims.Single(x => x.Type == ClaimTypes.Role).Value,
                Expires = jwt.ValidTo.ToTimestamp()
            };
        }
    }
}