using LinenManagement.Models;

namespace LinenManagement.Services
{
    public interface IEmployeeService
    {
        public Task SaveRefreshToken(int employeeId, string refreshToken);
        public Task<string?> GetRefreshToken(int employeeId);
        public Task<string> CreatePassword(int employeeId, string password);
        public Task<Employee?> GetEmployeeByEmail(string email);
        public Task<bool> ClearRefreshToken(int employeeId);
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string storedHash);
    }
}
