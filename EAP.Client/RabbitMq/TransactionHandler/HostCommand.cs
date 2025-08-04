using EAP.Client.Forms;
using EAP.Client.NonSecs.Message;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Runtime.Intrinsics.X86;

namespace EAP.Client.RabbitMq
{
    public class HostCommand : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly IConfiguration configuration;
        private readonly RabbitMqService rabbitMq;
        private readonly NonSecsService nonSecsService;



        public HostCommand(IConfiguration configuration, RabbitMqService rabbitMq, NonSecsService nonSecsService)
        {
            this.configuration = configuration;
            this.rabbitMq = rabbitMq;
            this.nonSecsService = nonSecsService;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var reptrans = trans?.GetReplyTransaction();
               

                try
                {

                    var equipmentId = configuration.GetSection("Custom")["EquipmentId"];
                    var command = string.Empty;
                    Dictionary<string, object> upperCaseDict = new Dictionary<string, object>();
                    foreach (var pair in trans.Parameters)
                    {
                        upperCaseDict[pair.Key.ToUpper()] = pair.Value;
                    }
                    if (upperCaseDict.TryGetValue("COMMAND", out object _c))
                    {
                        var commandJson = _c?.ToString();
                        var cmd = JsonConvert.DeserializeObject<Command>(commandJson);

                        var s2f41 = new S2F41
                        {
                            EQID = equipmentId,
                            Command = cmd
                        };

                        var reply = await nonSecsService.SendMessage(s2f41);
                        var s2f42 = reply.SecondaryMessage as S2F42;

                        reptrans?.Parameters?.Add("Result", s2f42.Result);
                    }
                }
                  
                catch (Exception ex)
                {
                    
                    reptrans?.Parameters.Add("Result", false);
                    reptrans?.Parameters.Add("Message", $"EAP Error {ex.Message}");
                    dbgLog.Error(ex.Message, ex);
                }
               
                if (reptrans != null && reptrans.NeedReply) rabbitMq.Produce(trans!.ReplyChannel, reptrans);
            
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex);
            }
        }

        
    }
}
