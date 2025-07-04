﻿using EAP.Client.RabbitMq;
using Secs4Net;

namespace EAP.Client.Secs
{
    internal interface IPrimaryMessageHandler
    {
        Task HandlePrimaryMessage(PrimaryMessageWrapper wrapper);
    }
}
