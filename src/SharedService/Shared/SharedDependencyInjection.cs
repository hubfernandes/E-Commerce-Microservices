using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Bases;
using Shared.Interfaces;
using Shared.Repository;
using Shared.Resources;
using System.Globalization;

public static class SharedDependencyInjection
{
    public static void AddSharedDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        //services.AddAutoMapper(Assembly.GetExecutingAssembly());
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient<ResponseHandler>();
        services.AddSingleton<SharedResource>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddLocalization(options => options.ResourcesPath = "");

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
    public static void UseSharedCulture(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var culture = context.Request.Query["culture"]; // Read culture from query
            if (!string.IsNullOrWhiteSpace(culture))
            {
                CultureInfo cultureInfo = new CultureInfo(culture!);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            }
            await next();
        });
    }
}

/*
 TypeLoadException or Missing Type: “Cannot find type ValidationBehavior<,>” because it’s not in Shared.Infrastructure.
No Validators Found: ValidationBehavior runs but _validators is empty because Shared.Infrastructure has no validators.
*/