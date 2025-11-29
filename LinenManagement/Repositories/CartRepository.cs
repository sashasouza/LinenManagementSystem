using LinenManagement.Context;
using LinenManagement.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LinenManagement.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartLog?> CreateUpdateCartLog(CartLog cartLog)
        {
            if (cartLog.DateWeighed.Kind == DateTimeKind.Unspecified || cartLog.DateWeighed.Kind == DateTimeKind.Local)
            {
                cartLog.DateWeighed = cartLog.DateWeighed.ToUniversalTime();
            }

            if (cartLog.CartLogId == null || cartLog.CartLogId == 0)
            {
                try
                {
                    await _context.CartLog.AddAsync(cartLog);

                    await _context.SaveChangesAsync();

                    Log.Information("Successfully added new CartLog having cartLogId " + cartLog.CartLogId);

                    return cartLog;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed adding new CartLog");
                }
            }
            else
            {
                    try
                    {
                        _context.CartLog.Attach(cartLog);

                        _context.Entry(cartLog).State = EntityState.Modified;

                        await _context.SaveChangesAsync();

                        Log.Information("Successfully updated CartLog with cartLogId: " + cartLog.CartLogId);
                        return cartLog;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to update CartLog with cartLogId: {CartLogId}", cartLog.CartLogId);
                    }
            }

            return null;
        }

        public async Task<bool> DeleteCartLog(int cartLogId, int employeeId)
        {
            try
            {
                var cartLog = await _context.CartLog
                    .Where(c => c.CartLogId == cartLogId && c.EmployeeId == employeeId)
                    .FirstOrDefaultAsync();

                if (cartLog != null)
                {
                    _context.CartLog.Remove(cartLog);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete CartLog ID {CartLogId} by Employee ID {EmployeeId}", cartLogId, employeeId);
                return false;
            }
        }

        public async Task<IEnumerable<CartLogDTO>> GetAllCartLogs(string? cartType, int? employeeId, int? locationId)
        {
            /*if (cartType != null)
            {
                IQueryable<CartLog> cartlogs = _context.CartLog.Include(log => log.Cart);

                try
                {
                    var res = await _context.CartLog
                         .Include(c => c.Cart)
                         .Include(c => c.Location)
                         .Include(c => c.Employee)
                         .Include(c => c.Details)
                         .ThenInclude(d => d.Linen)
                         .Where(log => log.Cart != null
                                 && log.Cart.Type == cartType)
                         .OrderByDescending(c => c.DateWeighed)
                         .ThenByDescending(c => c.ActualWeight)
                         .Select(c => new CartLogDTO 
                         {
                             CartLogId = c.CartLogId,
                             ReceiptNumber = c.ReceiptNumber,
                             ReportedWeight = c.ReportedWeight,
                             ActualWeight = c.ActualWeight,
                             Comments = c.Comments,
                             DateWeighed = c.DateWeighed,

                             Cart = new { c.Cart.CartId, c.Cart.Name, c.Cart.Weight, c.Cart.Type },
                             Location = new { c.Location.LocationId, c.Location.Name, c.Location.Type },
                             Employee = new { c.Employee.EmployeeId, c.Employee.Name },

                             Linen = c.Details.Select(d => new CartLogLinenDTO
                             {
                                 CartLogDetailId = d.CartLogDetailId,
                                 LinenId = d.LinenId,
                                 Count = d.Count,
                                 Name = d.Linen.Name
                             }).ToList()
                         })

                         .ToListAsync();
                    return res;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
            else if (employeeId != null)
            {
                try
                {
                    var res = await _context.CartLog
                        .Include(c => c.Cart)
                        .Include(c => c.Location)
                        .Include(c => c.Employee)
                        .Include(c => c.Details)
                        .ThenInclude(d => d.Linen)
                        .Where(c => c.EmployeeId == employeeId)
                        .OrderByDescending(c => c.DateWeighed)
                        .ThenByDescending(c => c.ActualWeight)
                        .Select(c => new CartLogDTO
                        {
                            CartLogId = c.CartLogId,
                            ReceiptNumber = c.ReceiptNumber,
                            ReportedWeight = c.ReportedWeight,
                            ActualWeight = c.ActualWeight,
                            Comments = c.Comments,
                            DateWeighed = c.DateWeighed,

                            Cart = new { c.Cart.CartId, c.Cart.Name, c.Cart.Weight, c.Cart.Type },
                            Location = new { c.Location.LocationId, c.Location.Name, c.Location.Type },
                            Employee = new { c.Employee.EmployeeId, c.Employee.Name },

                            Linen = c.Details.Select(d => new CartLogLinenDTO
                            {
                                CartLogDetailId = d.CartLogDetailId,
                                LinenId = d.LinenId,
                                Count = d.Count,
                                Name = d.Linen.Name
                            }).ToList()
                        })

                        .ToListAsync();
                    return res;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
            else if(locationId != null)
            {
                try
                {

                    var res = await _context.CartLog
                            .Include(c => c.Cart)
                            .Include(c => c.Location)
                            .Include(c => c.Employee)
                            .Include(c => c.Details)
                            .ThenInclude(d => d.Linen)
                            .Where(c => c.LocationId == locationId)
                            .OrderByDescending(c => c.DateWeighed)
                            .ThenByDescending(c => c.ActualWeight)
                            .Select(c => new CartLogDTO
                            {
                                CartLogId = c.CartLogId,
                                ReceiptNumber = c.ReceiptNumber,
                                ReportedWeight = c.ReportedWeight,
                                ActualWeight = c.ActualWeight,
                                Comments = c.Comments,
                                DateWeighed = c.DateWeighed,

                                Cart = new { c.Cart.CartId, c.Cart.Name, c.Cart.Weight, c.Cart.Type },
                                Location = new { c.Location.LocationId, c.Location.Name, c.Location.Type },
                                Employee = new { c.Employee.EmployeeId, c.Employee.Name },

                                Linen = c.Details.Select(d => new CartLogLinenDTO
                                {
                                    CartLogDetailId = d.CartLogDetailId,
                                    LinenId = d.LinenId,
                                    Count = d.Count,
                                    Name = d.Linen.Name
                                }).ToList()
                            })

                            .ToListAsync();
                    return res;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
            else
            {
                try
                {

                    var res = await _context.CartLog
                            .Include(c => c.Cart)
                            .Include(c => c.Location)
                            .Include(c => c.Employee)
                            .Include(c => c.Details)
                            .ThenInclude(d => d.Linen)
                            .OrderByDescending(c => c.DateWeighed)
                            .ThenByDescending(c => c.ActualWeight)
                            .Select(c => new CartLogDTO
                            {
                                CartLogId = c.CartLogId,
                                ReceiptNumber = c.ReceiptNumber,
                                ReportedWeight = c.ReportedWeight,
                                ActualWeight = c.ActualWeight,
                                Comments = c.Comments,
                                DateWeighed = c.DateWeighed,

                                Cart = new { c.Cart.CartId, c.Cart.Name, c.Cart.Weight, c.Cart.Type },
                                Location = new { c.Location.LocationId, c.Location.Name, c.Location.Type },
                                Employee = new { c.Employee.EmployeeId, c.Employee.Name },

                                Linen = c.Details.Select(d => new CartLogLinenDTO
                                {
                                    CartLogDetailId = d.CartLogDetailId,
                                    LinenId = d.LinenId,
                                    Count = d.Count,
                                    Name = d.Linen.Name
                                }).ToList()
                            })

                            .ToListAsync();
                    return res;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

                return null;*/

            IQueryable<CartLog> query = _context.CartLog
                .Include(c => c.Cart)
                .Include(c => c.Location)
                .Include(c => c.Employee)
                .Include(c => c.Details).ThenInclude(d => d.Linen);

            if (cartType != null)
            {
                query = query.Where(log => log.Cart != null && log.Cart.Type == cartType);
            }
            else if (employeeId != null)
            {
                query = query.Where(c => c.EmployeeId == employeeId);
            }
            else if (locationId != null)
            {
                query = query.Where(c => c.LocationId == locationId);
            }

            query = query
                .OrderByDescending(c => c.DateWeighed)
                .ThenByDescending(c => c.ActualWeight);

            try
            {
                var res = await query.Select(c => new CartLogDTO
                {
                    CartLogId = c.CartLogId,
                    ReceiptNumber = c.ReceiptNumber,
                    ReportedWeight = c.ReportedWeight,
                    ActualWeight = c.ActualWeight,
                    Comments = c.Comments,
                    DateWeighed = c.DateWeighed,

                    Cart = new { c.Cart.CartId, c.Cart.Name, c.Cart.Weight, c.Cart.Type },
                    Location = new { c.Location.LocationId, c.Location.Name, c.Location.Type },
                    Employee = new { c.Employee.EmployeeId, c.Employee.Name },

                    Linen = c.Details.Select(d => new CartLogLinenDTO
                    {
                        CartLogDetailId = d.CartLogDetailId,
                        LinenId = d.LinenId,
                        Count = d.Count,
                        Name = d.Linen.Name
                    }).ToList()
                }).ToListAsync();

                return res;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve CartLogs");
                return null; 
            }
        }

        public async Task<CartLogDTO?> GetCartLog(int cartLogId)
        {
            return await _context.CartLog
                        .Include(c => c.Cart)
                        .Include(c => c.Location)
                        .Include(c => c.Employee)
                        .Include(c => c.Details)
                        .ThenInclude(d => d.Linen)
                        .Where(log => log.CartLogId == cartLogId && log.Cart.Type == "CLEAN")
                        .OrderByDescending(c => c.DateWeighed)
                        .ThenByDescending(c => c.ActualWeight)
                        .Select(c => new CartLogDTO
                        {
                            CartLogId = c.CartLogId,
                            ReceiptNumber = c.ReceiptNumber,
                            ReportedWeight = c.ReportedWeight,
                            ActualWeight = c.ActualWeight,
                            Comments = c.Comments,
                            DateWeighed = c.DateWeighed,

                            Cart = new { c.Cart.CartId, c.Cart.Name, c.Cart.Weight, c.Cart.Type },
                            Location = new { c.Location.LocationId, c.Location.Name, c.Location.Type },
                            Employee = new { c.Employee.EmployeeId, c.Employee.Name },

                            Linen = c.Details.Select(d => new CartLogLinenDTO
                            {
                                CartLogDetailId = d.CartLogDetailId,
                                LinenId = d.LinenId,
                                Count = d.Count,
                                Name = d.Linen.Name
                            }).ToList()
                        })
                        .FirstOrDefaultAsync();
        }

    }
}
