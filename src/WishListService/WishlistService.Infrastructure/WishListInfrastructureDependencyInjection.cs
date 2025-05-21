using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.RedisCache;
using WishlistService.Infrastructure.Context;
using WishlistService.Infrastructure.Interfaces;
using WishlistService.Infrastructure.Repositories;

namespace WishlistService.Infrastructure
{

    public static class WishListInfrastructureDependencyInjection
    {
        public static void AddWishListInfrastructureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Db
            services.AddDbContext<WishlistDbContext>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("WishListConnection"),
                    option => option.EnableRetryOnFailure());
            });

            //DI
            services.AddScoped<IWishlistRepository, WishlistRepository>();

            // redis
            services.AddScoped<ICacheService, CacheService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("redis-WishList");
            });

        }
    }
}
