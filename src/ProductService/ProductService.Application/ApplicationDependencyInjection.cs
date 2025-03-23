using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductService.Application.EventHandlers;
using ProductService.Application.Handlers;
using ProductService.Application.Validators;
using Shared.Behavoir;
using Shared.Messaging;
using System.Reflection;

namespace ProductService.Application
{
    public static class ApplicationDependencyInjection
    {
        public static void AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedDependencyInjection(configuration);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductQueryHandler).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductCommandHandler).Assembly));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped<IValidateProductExists, ValidateProductExists>();

            //
            services.AddSingleton<IMessageBroker>(new RabbitMQMessageBroker("localhost"));



        }

        public class ProductCreatedEventConsumer : BackgroundService
        {
            private readonly ProductCreatedEventHandler _handler;

            public ProductCreatedEventConsumer(ProductCreatedEventHandler handler)
            {
                _handler = handler;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                await _handler.StartListening();
            }
        }


        public static void UserSharedMiddleWare(this IApplicationBuilder app)
        {
            app.UseExceptionHandlingMiddleware();
            app.UseSharedCulture();
        }
    }
}
