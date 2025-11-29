using LinenManagement.Models;
using LinenManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LinenManagement.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAppAuthenticationService _authenticationService;
        private readonly IEmployeeService _employeeService;

        public AuthController(IConfiguration config, IAppAuthenticationService authenticationService, IEmployeeService employeeService) 
        {
            _config = config;
            _authenticationService = authenticationService;
            _employeeService = employeeService;
        }

        [HttpPost("api/[controller]/login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var employee = await _authenticationService.Authenticate(request.Email, request.Password); // Using email and password as login creds

            if (employee == null)
            {
                Log.Error("User is not authorized");
                return Unauthorized();
            }

            var accessToken = _authenticationService.GenerateAccessToken(request, _config["JwtSecret"]);
            var refreshToken = _authenticationService.GenerateRefreshToken();

            await _employeeService.SaveRefreshToken(employee.EmployeeId, refreshToken);

            var response = new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            Log.Information("Access Token and Refresh Token generated successfully");
            return Ok(response);
        }

        [HttpPost("api/[controller]/refresh")]
        public async Task<IActionResult> Refresh([FromBody]RefreshRequest refreshRequest)
        {
            var newAccessToken = _authenticationService.GenerateAccessTokenFromRefreshToken(refreshRequest, _config["JwtSecret"]);

            if (newAccessToken.Result != "")
            {
                var newRefreshToken = _authenticationService.GenerateRefreshToken();
                var response = new TokenResponse
                {
                    AccessToken = newAccessToken.Result,
                    RefreshToken = newRefreshToken
                };

                await _employeeService.SaveRefreshToken(refreshRequest.EmployeeId, newRefreshToken);

                Log.Information("New Access Token generated successfully");
                return Ok(response);
            }
             return BadRequest();
        }

        [Authorize]
        [HttpPost("api/[controller]/logout")]
        public async Task<IActionResult> Logout(int employeeId)
        {
            try
            {
                var res = await _employeeService.ClearRefreshToken(employeeId);
                if (res)
                {
                    Log.Information("Refresh Token cleared successfully and user logged out");
                    return Ok();
                }
                Log.Error("Refresh Token clearing failed");
                return BadRequest();
            }
            catch (Exception ex)
            { 
                Log.Error(ex.Message);
                return BadRequest();
            }
        }

    }
}
