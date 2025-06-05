using log4net;

namespace EAP.Client.RabbitMq
{
    public abstract class TransactionHandler1 : ITransactionHandler
    {
        internal readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly RabbitMqService rabbitMq;


        public TransactionHandler1(RabbitMqService rabbitMq) 
        {
            this.rabbitMq = rabbitMq;
        }
        public abstract Task HandleTransaction(RabbitMqTransaction trans);
    }
}
