using Microsoft.Extensions.DependencyInjection;
using System.Reflection; // <-- 1. Necesitas este using para "Assembly"

namespace Lab10_AlberthMayta.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registra AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            // "Forma 2"
            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            
            return services;
        }
    }
}