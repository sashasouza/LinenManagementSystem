using LinenManagement.Models;
using LinenManagement.Repositories;
using Serilog;

namespace LinenManagement.Services
{
    public class CartService : ICartService
    {
        public ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<CartLog?> CreateUpdateCartLog(CartLog cartLog)
        {
            var res = await _cartRepository.CreateUpdateCartLog(cartLog);
            if (res != null)
            {
                return res;
            }

            return null;
        }

        public async Task<bool> DeleteCartLog(int cartLogId, int employeeId)
        {
            try
            {
                var res = await _cartRepository.DeleteCartLog(cartLogId, employeeId);

                if (res)
                {
                    Log.Information("CartLog deleted successfully");

                    return true;
                }
                else
                {
                    Log.Information("Error deleting CartLog");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return false;
        }

        public async Task<IEnumerable<CartLogDTO>?> GetAllCartLogs(string? cartType, int? employeeId, int? locationId)
        {
            try
            {
                var res = await _cartRepository.GetAllCartLogs(cartType, employeeId, locationId);
                if (res != null)
                {
                    Log.Information("CartLogs retrieved successfully");
                    return res;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return null;
        }

        public async Task<CartLogDTO?> GetCartLog(int cartLogId)
        {
            try
            {
                var res = await _cartRepository.GetCartLog(cartLogId);
                if (res != null)
                {
                    Log.Information("CartLog retrieved successfully");
                    return res;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return null;
        }

    }
}
