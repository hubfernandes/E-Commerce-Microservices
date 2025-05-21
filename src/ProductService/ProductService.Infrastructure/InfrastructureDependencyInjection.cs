using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Infrastructure.Context;
using ProductService.Infrastructure.Interfaces;
using ProductService.Infrastructure.Repositories;
using Shared.RedisCache;

namespace ProductService.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static void AddInfrastructurefDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProductContext>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("ProductConnection"),
                    option => option.EnableRetryOnFailure());
            });

            // DI
            services.AddScoped<IProductRepository, ProductRepository>();

            // redis 
            services.AddScoped<ICacheService, CacheService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("redis-products");
            });


        }
    }
}
