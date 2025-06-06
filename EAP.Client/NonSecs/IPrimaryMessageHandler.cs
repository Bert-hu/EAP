using EAP.Client.NonSecs.Message;

namespace EAP.Client.NonSecs
{
    internal interface IPrimaryMessageHandler
    {
        Task HandlePrimaryMessage(NonSecsMessageWrapper wrapper);
    }
}
