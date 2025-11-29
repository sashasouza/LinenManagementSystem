using LinenManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Serilog;
using LinenManagement.Repositories;

namespace LinenManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        public IEmployeeRepository _employeeRepository;

        public IConfiguration _config;

        private byte[] salt;

        private const int iterations = 5;

        private const int keySize = 32;

        public EmployeeService(IConfiguration config, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _config  = config;
            salt = Encoding.UTF8.GetBytes(_config["PasswordSalt"]); 
        }

        public async Task<string> CreatePassword(int employeeId, string password)
        {
            string hashedPassword = HashPassword(password);

            try
            {
                await _employeeRepository.SavePassword(employeeId, hashedPassword);
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }

            return hashedPassword;
        }

        public async Task<Employee?> GetEmployeeByEmail(string email)
        {
            try
            {
                return await _employeeRepository.GetEmployeeByEmail(email);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return null;
        }

        public async Task<string?> GetRefreshToken(int employeeId)
        {
            try
            {
                string refreshToken = await _employeeRepository.GetRefreshToken(employeeId);
                return refreshToken;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public async Task SaveRefreshToken(int employeeId, string refreshToken)
        {
            try
            {
                await _employeeRepository.SaveRefreshToken(employeeId, refreshToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public async Task<bool> ClearRefreshToken(int employeeId)
        {
            try
            {
                var res = await _employeeRepository.DeleteRefreshToken(employeeId);
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public string HashPassword(string password)
        {
            try
            {
                using var pbkdf2 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(keySize);

                return Convert.ToBase64String(hash);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            string computedHash = HashPassword(password);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(computedHash),
                Convert.FromBase64String(storedHash)
            );
        }
    }
}
