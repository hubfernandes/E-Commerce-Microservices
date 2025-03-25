using FluentValidation;
using InventoryService.Application.BackgroundServices;
using InventoryService.Application.Handlers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behavoir;
using System.Reflection;

namespace InventoryService.Application
{
    public static class InventoryApplicationDependencyInjection
    {
        public static void AddInventoryApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedDependencyInjection(configuration);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InventoryQueryHandler).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InventoryCommandHandler).Assembly));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            //services.AddSingleton<IMessageBroker>(new RabbitMQMessageBroker());
            //  services.AddScoped<ProductCreatedEventHandler>();

            services.AddHostedService<ProductCreatedEventConsumer>();
        }

        public static void UserSharedMiddleWare(this IApplicationBuilder app)
        {
            app.UseExceptionHandlingMiddleware();
            app.UseSharedCulture();
        }
    }
}
