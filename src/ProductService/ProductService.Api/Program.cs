using Microsoft.Extensions.Options;
using ProductService.Application.Handlers;
using ProductService.Infrastructure;

namespace ProductService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructurefDependencyInjection(builder.Configuration);
            builder.Services.AddSharedDependencyInjection(builder.Configuration);
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductQueryHandler).Assembly));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductCommandHandler).Assembly));


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value); // for localization

            app.UseExceptionHandlingMiddleware();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
