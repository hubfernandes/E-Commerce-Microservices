﻿using InventoryService.Infrastructure.Context;
using InventoryService.Infrastructure.Interfaces;
using InventoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.RedisCache;

namespace InventoryService.Infrastructure
{
    public static class InventoryInfrastructureDependencyInjection
    {
        public static void AddInventoryInfrastructurefDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InventoryDbContext>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("InventoryConnection"),
                    option => option.EnableRetryOnFailure());
            });

            // DI
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            // redis
            services.AddScoped<ICacheService, CacheService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("redis-inventory");
            });
        }
    }
}
