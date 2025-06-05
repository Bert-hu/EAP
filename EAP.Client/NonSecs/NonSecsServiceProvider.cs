using EAP.Client.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs
{
    public static class NonSecsServiceProvider
    {
        public static IServiceCollection AddNonSecs(this IServiceCollection services)
        {
            services.AddSingleton<NonSecsService>();
            services.AddHostedService<NonSecsWorker>();

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
