using EAP.Client.RabbitMq;
using log4net;
using Secs4Net;

namespace EAP.Client.Secs.PrimaryMessageHandler
{
    internal class S1F13 : IPrimaryMessageHandler
    {
        private  ISecsGem _secsGem;
        private readonly CommonLibrary commonLibrary
            
            ;
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");

        public S1F13(ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            _secsGem = secsGem;
            this.commonLibrary = commonLibrary;
        }

        public async Task HandlePrimaryMessage(PrimaryMessageWrapper wrapper)
        {


            if (wrapper.PrimaryMessage.ReplyExpected)
            {
                var streamfunction = $"S{wrapper.PrimaryMessage.S}F{wrapper.PrimaryMessage.F + 1}";
                var secondaryMessage = commonLibrary.GetSecsMessageByName(streamfunction) ?? new SecsMessage(wrapper.PrimaryMessage.S, (byte)(wrapper.PrimaryMessage.F + 1));
                await wrapper.TryReplyAsync(secondaryMessage);
            }
            //var s1f13 =  commonLibrary.GetSecsMessageByName("S1F13");
            //_  = _secsGem.SendAsync(s1f13);
            //if (rep.F == 14)
            //{
            //    await Task.Run(() => SecsInitialization.Initialization(_secsGem, commonLibrary));
            //}
        }

      
    }
}
