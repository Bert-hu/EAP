namespace HandlerAgv.Service.RabbitMq
{
    internal interface ITransactionHandler
    {
        Task HandleTransaction(RabbitMqTransaction trans);

    }
}
