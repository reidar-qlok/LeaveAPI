
using LeaveAPI.Data;
using LeaveAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            // Return all employees
            app.MapGet("/employees", async (ApplicationDbContext context) =>
            {
                var employees = await context.Employees.ToListAsync();
                if (employees == null || !employees.Any())
                {
                    return Results.NotFound("Hittade inga employees");
                }
                return Results.Ok(employees);
            });

            // Create a employee
            app.MapPost("/employees", async (Employee employee, ApplicationDbContext context) =>
            {
                context.Employees.Add(employee);
                await context.SaveChangesAsync();
                return Results.Created($"/employees/{employee.EmployeeId}", employee);
            });


            app.Run();
        }
    }
}
