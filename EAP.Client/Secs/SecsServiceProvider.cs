using EAP.Client.RabbitMq;
using EAP.Client.Secs.PrimaryMessageHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Secs4Net;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EAP.Client.Secs
{
    public static class SecsServiceProvider
    {
        public static IServiceCollection AddSecs4Net<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TLogger>(this IServiceCollection services, IConfiguration configuration)
            where TLogger : class, ISecsGemLogger
        {
            var configSection = configuration.GetSection("secs4net");
            services.Configure<SecsGemOptions>(configSection);
            services.AddSingleton<ISecsConnection, HsmsConnection>();
            services.AddSingleton<ISecsGem, SecsGem>();
            services.AddSingleton<ISecsGemLogger, TLogger>();


            var handerTypes1 = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IPrimaryMessageHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var handlerType in handerTypes1)
            {
                services.AddTransient(handlerType);
                services.AddTransient(typeof(IPrimaryMessageHandler), handlerType);
            }

            var handerTypes2 = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IEventHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var handlerType in handerTypes2)
            {
                services.AddTransient(handlerType);
                services.AddTransient(typeof(IEventHandler), handlerType);
            }

            services.AddSingleton<CommonLibrary>();
            return services;
        }
    }
}