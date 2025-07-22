using System.Reflection;

namespace HandlerAgv.Service.RabbitMq
{
    public static class RabbitMqServiceProvider
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)   
        {
            services.AddSingleton<RabbitMqService>();
            services.AddHostedService<RabbitMqWorker>();

            var handerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(ITransactionHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var handlerType in handerTypes)
            {
                services.AddTransient(handlerType);
                services.AddTransient(typeof(ITransactionHandler), handlerType);
            }

            return services;
        }
    }
}
