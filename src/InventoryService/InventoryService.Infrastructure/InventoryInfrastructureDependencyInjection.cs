using InventoryService.Infrastructure.Context;
using InventoryService.Infrastructure.Interfaces;
using InventoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Infrastructure
{
    public static class InventoryInfrastructureDependencyInjection
    {
        public static void AddInventoryInfrastructurefDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InventoryDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("InventoryConnection"),
                    sqlServerOption => sqlServerOption.EnableRetryOnFailure());
            });

            // DI
            services.AddScoped<IInventoryRepository, InventoryRepository>();

        }
    }
}
