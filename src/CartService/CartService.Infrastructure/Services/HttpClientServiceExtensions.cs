using Microsoft.Extensions.DependencyInjection;

namespace CartService.Infrastructure.Services
{

    public static class HttpClientServiceExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            var time = TimeSpan.FromSeconds(30);
            services.AddHttpClient("ProductService", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5279/");
                client.Timeout = time;
            });

            services.AddHttpClient("AuthService", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5089/");
                client.Timeout = time;
            });

            return services;
        }
    }
}
