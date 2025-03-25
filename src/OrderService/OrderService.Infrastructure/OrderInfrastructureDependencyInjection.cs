using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Infrastructure.Context;
using OrderService.Infrastructure.Interfaces;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure
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

            // services.AddScoped<IMessageBroker, RabbitMQMessageBroker>();

        }
    }
}
