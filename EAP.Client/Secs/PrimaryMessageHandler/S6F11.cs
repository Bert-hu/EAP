using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using Microsoft.Extensions.DependencyInjection;
using Secs4Net;
using System.Reflection;

namespace EAP.Client.Secs.PrimaryMessageHandler
{
    internal class S6F11 : IPrimaryMessageHandler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly CommonLibrary commonLibrary;

        public S6F11(IServiceProvider serviceProvider, CommonLibrary commonLibrary)
        {
            this.serviceProvider = serviceProvider;
            this.commonLibrary = commonLibrary;
        }
        public async Task HandlePrimaryMessage(PrimaryMessageWrapper wrapper)
        {
            //答复S6F12
            if (wrapper.PrimaryMessage.ReplyExpected)
            {
                var streamfunction = $"S{wrapper.PrimaryMessage.S}F{wrapper.PrimaryMessage.F + 1}";
                var secondaryMessage = commonLibrary.GetSecsMessageByName(streamfunction);
                if (secondaryMessage == null)
                {
                    secondaryMessage = new SecsMessage(wrapper.PrimaryMessage.S, (byte)(wrapper.PrimaryMessage.F + 1))
                    {
                    };
                }
                await wrapper.TryReplyAsync(secondaryMessage);
            }
            //调用和appsetting中event name相同的类（IEventHandler）
            commonLibrary.Ceids.TryGetValue((int)wrapper.PrimaryMessage.SecsItem[1].FirstValue<uint>(), out GemCeid ceid);
            if (ceid != null)
            {
                var interfaceType = typeof(IEventHandler);
                var type = Assembly.GetExecutingAssembly().GetTypes().Where(t => interfaceType.IsAssignableFrom(t) && t.Name == ceid.Name).FirstOrDefault();
                if (type != null)
                {
                    //IEventHandler obj = (IEventHandler)Activator.CreateInstance(type);
                    //await obj.HandleEvent(ceid, wrapper);

                    using (var scope = serviceProvider.CreateAsyncScope())
                    {
                        var handler = (IEventHandler)scope.ServiceProvider.GetRequiredService(type);
                        handler.HandleEvent(ceid, wrapper);
                    }
                }
            }


        }
    }
}
