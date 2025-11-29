using LinenManagement.Models;

namespace LinenManagement.Repositories
{
    public interface IEmployeeRepository
    {
        public Task SavePassword(int employeeId, string password);
        public Task SaveRefreshToken(int employeeId, string refreshToken);
        public Task<string> GetRefreshToken(int employeeId);
        public Task<bool> DeleteRefreshToken(int employeeId);
        public Task<IEnumerable<Employee>> GetAllEmployees();
        public Task<Employee?> GetEmployeeByEmail(string email);
        public Task<Employee> GetEmployee(int employeeId);
    }
}
