using LinenManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LinenManagement.Controllers
{
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService) {
            _employeeService = employeeService;
        }

        [HttpPost("[controller]/generatePasswordHash")]
        public async Task<IActionResult> GeneratePassword(int employeeId, string password)
        {
            var generatedPassword = await _employeeService.CreatePassword(employeeId, password);

            if (generatedPassword == null)
            {
                Log.Error("Error generating and storing password");
                return BadRequest();
            }

            Log.Information("Successfully generated and stored password");
            return Ok(generatedPassword);
        }

    }
}
