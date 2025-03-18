using EAP.Client.Forms;
using EAP.Client.Secs;
using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.RabbitMq.TransactionHandler
{
    internal class UpdateOuterSnInfo : ITransactionHandler
    {
        internal readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly ISecsConnection hsmsConnection;
        internal readonly CommonLibrary commonLibrary;

        public UpdateOuterSnInfo(RabbitMqService rabbitMq, ISecsGem secsGem, ISecsConnection hsmsConnection, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.hsmsConnection = hsmsConnection;
            this.commonLibrary = commonLibrary;


        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var panelid = string.Empty;
                var modelname = string.Empty;
                if (trans.Parameters.TryGetValue("PanelId", out object _rec)) panelid = _rec?.ToString();
                if (trans.Parameters.TryGetValue("ModelName", out object _modelname)) modelname = _modelname?.ToString();
                MainForm.Instance?.UpdateSpiPanelAndModelname(panelid, modelname);
            }
            catch (Exception ex)
            {

                dbgLog.Error(ex.Message, ex);
            }


        }
    }
}