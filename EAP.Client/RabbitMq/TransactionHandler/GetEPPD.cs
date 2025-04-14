using log4net;
using Newtonsoft.Json;
using Secs4Net;
using EAP.Client.Secs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;
using Microsoft.Extensions.Configuration;

namespace EAP.Client.RabbitMq
{
    internal class GetEPPD : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        private readonly IConfiguration configuration;
        public GetEPPD(RabbitMqService rabbitMq, IConfiguration configuration) 
        {
            this.rabbitMq = rabbitMq;
            this.configuration = configuration;
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                List<string> EPPD = new List<string>();
                var recipePath = configuration.GetSection("Custom")["MachineRecipePath"];
                //找到当前路径下所有.rcp文件
                var rcpFiles = Directory.GetFiles(recipePath, "*.rcp", SearchOption.TopDirectoryOnly);
                foreach (var rcpFile in rcpFiles)
                {
                    var rcpName = Path.GetFileName(rcpFile);
                    EPPD.Add(rcpName);
                }
                reptrans.Parameters.Add("Result", true);
                reptrans.Parameters.Add("EPPD", EPPD);

            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
