using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.RabbitMq
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
