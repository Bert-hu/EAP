using EAP.Client.RabbitMq;
using Secs4Net;

namespace EAP.Client.Secs.PrimaryMessageHandler
{
    internal class S5F1 : IPrimaryMessageHandler
    {
        private readonly RabbitMqService rabbitMq;
        private readonly CommonLibrary commonLibrary;
        public S5F1(RabbitMqService rabbitMq, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.commonLibrary = commonLibrary;
        }


        public async Task HandlePrimaryMessage(PrimaryMessageWrapper wrapper)
        {
            var alarmset = wrapper.PrimaryMessage.SecsItem[0].FirstValue<byte>() >= 128;
            var equipmentId = commonLibrary.CustomSettings["EquipmentId"];
            var alarmcode = wrapper.PrimaryMessage.SecsItem[1].FirstValue<byte>().ToString();
            var alarmtext = wrapper.PrimaryMessage.SecsItem[2].GetString();
            var para = new Dictionary<string, object> {
                            { "AlarmEqp", equipmentId},
                            { "AlarmCode",alarmcode},
                            { "AlarmText",alarmtext},
                            { "AlarmSource", "EAP"},
                            { "AlarmTime",DateTime.Now},
                            { "AlarmSet",alarmset}
                        };
            var trans = new RabbitMqTransaction
            {
                TransactionName = "EquipmentAlarm",
                Parameters = para,
            };
            rabbitMq.Produce("EAP.Services", trans);
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
        }
    }
}
