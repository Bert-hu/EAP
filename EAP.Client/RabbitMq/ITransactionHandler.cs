using EAP.Client.Secs;
using Secs4Net;

namespace EAP.Client.RabbitMq
{
    internal interface ITransactionHandler
    {
        Task HandleTransaction(RabbitMqTransaction trans);
        //Task HandleTransaction(RabbitMqTransaction trans, RabbitMqService rabbitMq, ISecsGem secsGem, ISecsConnection hsmsConnection, CommonLibrary commonLibrary);
    }
}
