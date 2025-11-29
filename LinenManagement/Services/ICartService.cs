using LinenManagement.Models;

namespace LinenManagement.Services
{
    public interface ICartService
    {
        public Task<CartLog?> CreateUpdateCartLog(CartLog cartLog);
        public Task<bool> DeleteCartLog(int cartLogId, int employeeId);
        public Task<IEnumerable<CartLogDTO>?> GetAllCartLogs(string? cartType, int? employeeId, int? locationId);
        public Task<CartLogDTO?> GetCartLog(int cartLogId);
    }
}
