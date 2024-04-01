using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Web;

namespace Kore.Web.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public string GetToken(HttpContextBase httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("bearer", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                return token;
            }

            return string.Empty;
        }

        public string GenerateJwtToken(string userId, string userName, HttpResponseBase httpResponseBase)
        {
            string refreshToken = GenerateRefreshToken();
            SetRefreshTokenCookie(httpResponseBase, refreshToken);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["AudienceSecret"]));
            var credentails = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, userId),
                new Claim(ClaimTypes.Name, userName)
            };

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                issuer: ConfigurationManager.AppSettings["Issuer"],
                audience: ConfigurationManager.AppSettings["AudienceId"],
                expires: DateTime.Now.AddHours(7),
                signingCredentials: credentails
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public bool ValidateToken(HttpContextBase httpContext)
        {
            var token = GetToken(httpContext);

            if (token == null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["AudienceSecret"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = ConfigurationManager.AppSettings["Issuer"],
                    ValidAudience = ConfigurationManager.AppSettings["AudienceId"],
                    // Set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);
                string token = Convert.ToBase64String(randomNumber);
                return token;
            }
        }

        private void SetRefreshTokenCookie(HttpResponseBase httpResponseBase, string refreshToken)
        {
            var refreshCookie = new HttpCookie("refreshToken", refreshToken)
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            httpResponseBase.Cookies.Add(refreshCookie);
        }
    }
}
