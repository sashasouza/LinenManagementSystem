using LinenManagement.Models;
using LinenManagement.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LinenManagement.Services
{
    public class AppAuthenticationService : IAppAuthenticationService
    {
        public IEmployeeRepository _employeeRepository;
        public IEmployeeService _employeeService;
        public AppAuthenticationService(IEmployeeRepository employeeRepository, IEmployeeService employeeService) 
        {
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
        }
        
        public async Task<Employee?> Authenticate(string email, string password)
        {
            var employee = await _employeeService.GetEmployeeByEmail(email);

            if(employee != null)
            {
                var res = _employeeService.VerifyPassword(password, employee.Password);

                if(res)
                    return employee;
            }

            return null;
        }


        // Token related methods can also be separated into a different class, as it is only used for generating, validating and refreshing
        // we have kept it in the same class
        public string GenerateAccessToken(LoginRequest request, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim("email", request.Email.ToString())]),
                Expires = DateTime.UtcNow.AddMinutes(15), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateAccessTokenFromRefreshToken(RefreshRequest refreshRequest, string secret)
        {
            var principal = GetPrincipalFromExpiredToken(refreshRequest.AccessToken, secret);

            var emailClaim = principal?.FindFirst(ClaimTypes.Email);

            if (emailClaim != null)
            {
                var res = await _employeeService.GetEmployeeByEmail(emailClaim.Value);

                var storedRefreshToken = await _employeeService.GetRefreshToken(refreshRequest.EmployeeId);

                if (res != null && storedRefreshToken == refreshRequest.RefreshToken)
                {
                    var newClaims = new ClaimsIdentity(principal?.Claims);

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(secret);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = newClaims, 
                        Expires = DateTime.UtcNow.AddMinutes(15), 
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return tokenHandler.WriteToken(token);
                }
            }
            return "";
        }

        private static ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, string secret)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                ValidateIssuer = false, 
                ValidateAudience = false, 
                ValidateLifetime = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken
            );

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
    }
}
