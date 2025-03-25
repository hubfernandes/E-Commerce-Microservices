using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.EventHandlers;
using OrderService.Application.Handlers;
using OrderService.Application.Validators;
using Shared.Behavoir;
using System.Reflection;

namespace OrderService.Application
{
    public static class ApplicationDependencyInjection
    {
        public static void AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSharedDependencyInjection(configuration);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderQueryHandler).Assembly));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderCommandHandler).Assembly));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped<IValidateOrderExists, ValidateOrderExists>();

            //
            services.AddScoped<OrderCanceledEventHandler>();
            //  services.AddSingleton<IMessageBroker>(new RabbitMQMessageBroker("localhost"));

        }

        //public class OrderCanceledEventConsumer : BackgroundService
        //{
        //    private readonly OrderCanceledEventHandler _handler;

        //    public OrderCanceledEventConsumer(OrderCanceledEventHandler handler)
        //    {
        //        _handler = handler;
        //    }

        //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //    {
        //        await _handler.s();
        //    }
        //}     
        public static void UserSharedMiddleWare(this IApplicationBuilder app)
        {
            app.UseExceptionHandlingMiddleware();
            app.UseSharedCulture();
        }
    }
}
