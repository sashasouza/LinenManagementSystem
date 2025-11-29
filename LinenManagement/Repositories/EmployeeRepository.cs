using LinenManagement.Context;
using LinenManagement.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LinenManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteRefreshToken(int employeeId)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                if (employee != null)
                {
                    employee.RefreshToken = null;

                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error deleting RefreshToken for EmployeeId {EmployeeId}", employeeId);
                return false;
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByEmail(string email)
        {
            return await _context.Employees.Where(e => e.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            return await _context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetRefreshToken(int employeeId)
        {
            return await _context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => e.RefreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task SavePassword(int employeeId, string password)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

                if (employee != null)
                {
                    employee.Password = password;

                    await _context.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Failed to save password for Employee ID: {EmployeeId}", employeeId);
            }
        }

        public async Task SaveRefreshToken(int employeeId, string refreshToken)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee != null)
            {
                employee.RefreshToken = refreshToken;

                await _context.SaveChangesAsync();
            }
        }
    }
}
