using EAP.Client.Secs;
using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.RabbitMq
{
    public abstract class TransactionHandler1 : ITransactionHandler
    {
        internal readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly ISecsConnection hsmsConnection;
        internal readonly CommonLibrary commonLibrary;

        public TransactionHandler1(RabbitMqService rabbitMq, ISecsGem secsGem, ISecsConnection hsmsConnection, CommonLibrary commonLibrary) 
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.hsmsConnection = hsmsConnection;
            this.commonLibrary = commonLibrary;
        }
        public abstract Task HandleTransaction(RabbitMqTransaction trans);
    }
}
