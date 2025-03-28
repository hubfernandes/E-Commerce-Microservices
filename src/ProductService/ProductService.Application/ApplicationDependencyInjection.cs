using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Handlers;
using ProductService.Application.Validators;
using Shared.Behavoir;
using Shared.Extensions;
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

            services.RegisterSharedService(); // for rabbitmq connection

        }



        public static void UserSharedMiddleWare(this IApplicationBuilder app)
        {
            app.UseExceptionHandlingMiddleware();
            app.UseSharedCulture();
        }
    }
}
