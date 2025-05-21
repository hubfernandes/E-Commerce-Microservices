using CartService.Infrastructure.Context;
using CartService.Infrastructure.Interfaces;
using CartService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.RedisCache;

namespace CartService.Infrastructure
{
    public static class CartInfrastructureDependencyInjection
    {
        public static void AddCartInfrastructureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Db
            services.AddDbContext<CartContext>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("CartConnection"),
                    option => option.EnableRetryOnFailure());
            });

            //DI
            services.AddScoped<ICartRepository, CartRepository>();

            // redis
            services.AddScoped<ICacheService, CacheService>();
            services.AddStackExchangeRedisCache(options =>
             {
                 options.Configuration = configuration.GetConnectionString("redis-cart");
             });

        }
    }
}
