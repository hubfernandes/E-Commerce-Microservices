using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Infrastructure.Context;
using Order.Infrastructure.Interfaces;
using Order.Infrastructure.Repositories;

namespace Order.Infrastructure
{
    public static class OrderInfrastructureDependencyInjection
    {
        public static void AddOrderInfrastructureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Db
            services.AddDbContext<OrderDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("OrderConnection"),
                    sqlServerOption => sqlServerOption.EnableRetryOnFailure());
            });

            //DI
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
