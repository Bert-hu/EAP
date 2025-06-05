namespace EAP.Client.RabbitMq
{
    internal interface ITransactionHandler
    {
        Task HandleTransaction(RabbitMqTransaction trans);

    }
}
