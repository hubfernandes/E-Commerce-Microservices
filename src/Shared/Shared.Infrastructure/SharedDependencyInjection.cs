using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Bases;
using Shared.Behavoir;
using Shared.Resources;
using System.Globalization;
using System.Reflection;

public static class SharedDependencyInjection
{
    public static void AddSharedDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());// Without this, you would need to manually register each validator services.AddTransient<IValidator<AddStudentCommandWithResponse>, AddStudentValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient<ResponseHandler>(); // 
        services.AddSingleton<SharedResource>();

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG"),
                    new CultureInfo("fr-FR"),
                 };

            options.DefaultRequestCulture = new RequestCulture("ar-EG");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
    }
}
