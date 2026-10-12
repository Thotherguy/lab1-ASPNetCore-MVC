using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Lab1.Models;

namespace Lab1.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Supervisor", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var configartion = serviceProvider.GetRequiredService<IConfiguration>();

            string supervisorPassword = configartion["SeedUsers:SupervisorPassword"] ?? throw new InvalidOperationException("Supervisor password not found in configuration.");
            string employeePassword = configartion["SeedUsers:EmployeePassword"] ?? throw new InvalidOperationException("Employee password not found in configuration.");

            var supervisorEmail = "supervisor@example.com";
            var supervisor = await userManager.FindByEmailAsync(supervisorEmail);
            if (supervisor == null)
            {
                supervisor = new ApplicationUser
                {
                    UserName = supervisorEmail,
                    Email = supervisorEmail,
                    FirstName = "John",
                    LastName = "Doe",
                    City = "New York",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(supervisor, supervisorPassword);
                if (result.Succeeded) {
                    await userManager.AddToRoleAsync(supervisor, "Supervisor");
                }
            }

            var employeeEmail = "employee@example.com";
            var employee = await userManager.FindByEmailAsync(employeeEmail);
            if (employee == null)
            {
                employee = new ApplicationUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    FirstName = "Jane",
                    LastName = "Smith",
                    City = "Los Angeles",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(employee, employeePassword);
                await userManager.AddToRoleAsync(employee, "Employee");
            }
        }
    }
}
