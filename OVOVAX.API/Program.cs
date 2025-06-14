
using Microsoft.EntityFrameworkCore;
using OVOVAX.Core.Interfaces;
using OVOVAX.Repository;
using OVOVAX.Repository.Data;
using OVOVAX.Services;
using OVOVAX.API.Mapping;

namespace OVOVAX.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Configure Entity Framework
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Generic Repository Pattern
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();            // Business Services
            builder.Services.AddScoped<IScannerService, ScannerService>();
            builder.Services.AddScoped<IInjectionService, InjectionService>();
            builder.Services.AddScoped<IMovementService, MovementService>();
            
            // ESP32 Communication
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IEsp32Service, Esp32Service>();
            
            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));            // CORS for React frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5173",
                             "https://mohamed-badr555.github.io",
                            "http://localhost:3000",
                            "https://localhost:5173",
                            "https://localhost:3000"
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Allow credentials if needed
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            // Enable CORS
            app.UseCors("AllowReactApp");
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
