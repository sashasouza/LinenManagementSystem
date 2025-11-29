using LinenManagement.Models;
using LinenManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinenManagement.Controllers
{
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService) 
        {
            _cartService = cartService;
        }

        [HttpPost("api/[controller]/cartlogs/upsert")]
        public async Task<IActionResult> CreateUpdateCartLog([FromBody]CartLog cartlog)
        {
            var res = await _cartService.CreateUpdateCartLog(cartlog);

            if (res != null)
                return Ok(res);
            else
                return BadRequest();
        }

        [HttpGet("api/[controller]/cartlogs")]
        public async Task<IActionResult> GetAllCartLogs(string? cartType, int? employeeId, int? locationId)
        {
            var res = await _cartService.GetAllCartLogs(cartType, employeeId, locationId);

            if (res == null || !res.Any())
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpGet("api/[controller]/cartlogs/cartlogid")]
        public async Task<IActionResult> GetCartLog(int cartLogId)
        {
            var res = await _cartService.GetCartLog(cartLogId);

            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpDelete("api/[controller]/cartlogs/cartlogid")]
        public async Task<IActionResult> DeleteCartLog(int cartLogId, int employeeId)
        {
           // we are only deleting the cartlog and not changing/deleting any data from the other tables
           var res = await _cartService.DeleteCartLog(cartLogId, employeeId);

            if (res)
                return Ok(res);
            else
                return BadRequest();
        }
    }
}
