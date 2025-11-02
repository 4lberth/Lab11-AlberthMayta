// --- AÑADE TODOS ESTOS USINGS ---

using Lab10_AlberthMayta.Application.Interfaces;
using Lab10_AlberthMayta.Domain.Ports;
using Lab10_AlberthMayta.Infrastructure.Adapters;
using Lab10_AlberthMayta.Infrastructure.Data.Context;
using Lab10_AlberthMayta.Infrastructure.Services;
using Lab11_AlberthMayta.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// --- CORRIGE EL NAMESPACE ---
namespace Lab10_AlberthMayta.Infrastructure.Configuration;


public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 3. Ahora 'AddDbContext' y 'Lab11ARMCContext' funcionarán
            services.AddDbContext<Lab11ARMCContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                
                // 4. 'UseMySql' y 'ServerVersion' funcionarán
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Registrar el generador de tokens JWT
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
