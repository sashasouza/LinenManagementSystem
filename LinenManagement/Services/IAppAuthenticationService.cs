using LinenManagement.Models;

namespace LinenManagement.Services
{
    public interface IAppAuthenticationService
    {
        public Task<Employee?> Authenticate(string email, string password);
        public string GenerateAccessToken(LoginRequest request, string secret);
        public string GenerateRefreshToken();
        public Task<string> GenerateAccessTokenFromRefreshToken(RefreshRequest refreshRequest, string secret);
    }
}
