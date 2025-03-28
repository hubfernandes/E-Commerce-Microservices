using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behavoir;
using System.Reflection;
using WishlistService.Application.Handlers;
using WishlistService.Application.Validators;

namespace WishlistService.Application
{

    public static class WishListApplicationDependencyInjection
    {
        public static void AddWishListApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedDependencyInjection(configuration);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(WishlistQueryHandler).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(WishlistCommandHandler).Assembly));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped<IValidateWishlistExists, ValidateWishlistExists>();

            // services.AddHostedService<CartExpirationBackgroundService>();
            services.AddHttpContextAccessor();


            //services.AddHostedService<StockEventConsumer>();

        }

        public static void UserSharedMiddleWare(this IApplicationBuilder app)
        {
            app.UseExceptionHandlingMiddleware();
            app.UseSharedCulture();
        }
    }
}
