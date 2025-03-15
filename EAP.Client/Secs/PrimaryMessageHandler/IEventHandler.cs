using EAP.Client.RabbitMq;
using EAP.Client.Secs.Models;
using Secs4Net;

namespace EAP.Client.Secs.PrimaryMessageHandler
{
    internal interface IEventHandler
    {
        Task HandleEvent(GemCeid ceid, PrimaryMessageWrapper wrapper, RabbitMq.RabbitMqService rabbitMq, ISecsGem secsGem, CommonLibrary commonLibrary);
    }
}
