using LinenManagement.Context; 
using LinenManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;

namespace LinenManagementTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)); 

                if (dbContextDescriptor != null)
                {
                services.Remove(services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)));
                }

                services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("TestLinenManagementDb"));

                services.RemoveAll<IAuthenticationService>();
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    SeedData(db);
                }
            });

            builder.UseEnvironment("Testing");
        }

        private void SeedData(AppDbContext dbContext)
        {
            dbContext.Set<Employee>().Add(new Employee
            {
                EmployeeId = 1,
                Name = "Admin User",
                Email = "test@g.com",       
                Password = "hashedpassword",     
                RefreshToken = null
            });

            dbContext.Set<Carts>().Add(new Carts
            {
                CartId = 1,
                Name = "Test Cart 1",
                Weight = 20,                     
                Type = "CLEAN"
            });

            dbContext.Set<Locations>().Add(new Locations
            {
                LocationId = 1,
                Name = "Room 101",
                Type = "CLEAN_ROOM"              
            });
            dbContext.SaveChanges();

            dbContext.CartLog.Add(new CartLog
            {
                CartLogId = 1,
                ReceiptNumber = "SEEDED1",
                ReportedWeight = 10,
                ActualWeight = 10,
                DateWeighed = DateTime.Parse("2024-10-01T10:00:00Z"),
                CartId = 1,
                LocationId = 1,
                EmployeeId = 1
            });
            dbContext.SaveChanges();
        }
    }
}
