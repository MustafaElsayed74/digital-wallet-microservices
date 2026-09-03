using Auth.Api.Data;
using Auth.Api.Entities;
using Auth.Api.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            // Configure the identity
            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();


            builder.Services.AddDbContext<AuthDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection String not found");
                options.UseSqlServer(connectionString);
            });


            //Mapping Jwt settings in the JWT helper class so we can inject it whereever we want;
            builder.Services.Configure<JWT>(
                builder.Configuration.GetSection("Jwt")

                );

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
