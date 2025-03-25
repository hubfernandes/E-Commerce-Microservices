using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging;

namespace Shared.Extensions
{
    public static class ExtentionMethods
    {
        public static void RegisterSharedService(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQConnection>(new RabbitMQConnection());

            services.AddScoped<IMessageProducer, RabbitMQProducer>();
        }
    }
}
