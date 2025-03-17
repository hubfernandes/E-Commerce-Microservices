using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Handlers;
using Order.Application.Validators;
using Shared.Behavoir;
using System.Reflection;

namespace Order.Application
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

        }

        public static void UserSharedMiddleWare(this IApplicationBuilder app)
        {
            app.UseExceptionHandlingMiddleware();
            app.UseSharedCulture();
        }
    }
}
