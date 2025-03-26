using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Infrastructure.Context;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Repositories;
namespace Payment.Infrastructure
{
    public static class PaymentInfrastructureDependencyInjection
    {
        public static void AddPaymentInfrastructureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Db
            services.AddDbContext<PaymentDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("PaymentConnection"),
                    sqlServerOption => sqlServerOption.EnableRetryOnFailure());
            });

            //DI
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentProcessor, PaymentProcessor>();

        }
    }
}
