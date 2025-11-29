using LinenManagement.Models;

namespace LinenManagement.Repositories
{
    public interface ICartRepository
    {
        public Task<CartLogDTO?> GetCartLog(int cartLogId);
        public Task<IEnumerable<CartLogDTO>> GetAllCartLogs(string? cartType, int? employeeId, int? locationId);
        public Task<CartLog?> CreateUpdateCartLog(CartLog cartLog);
        public Task<bool> DeleteCartLog(int cartLogId, int employeeId);
    }
}
