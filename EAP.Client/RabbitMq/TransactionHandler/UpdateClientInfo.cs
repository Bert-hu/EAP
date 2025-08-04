using EAP.Client.Forms;
using log4net;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.RabbitMq.TransactionHandler
{
    internal class UpdateClientInfo : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");


        public UpdateClientInfo(RabbitMqService rabbitMq, ISecsGem secsGem)
        {
        }

        public Task HandleTransaction(RabbitMqTransaction trans)
        {
            try
            {
                var inputTrayCount =  int.Parse(trans.Parameters["InputTrayCount"].ToString());
                var outputTrayCount = int.Parse(trans.Parameters["OutputTrayCount"].ToString());
                var agvEnabled = bool.Parse(trans.Parameters["AgvEnabled"].ToString());
                var CurrentTaskState = trans.Parameters["CurrentTaskState"].ToString();

                MainForm.Instance.InputTrayCount = inputTrayCount;
                MainForm.Instance.OutputTrayCount = outputTrayCount;
                MainForm.Instance.AgvEnabled = agvEnabled;
                MainForm.Instance.CurrentTaskState = CurrentTaskState;
            }
            catch (Exception ex)
            {
                dbgLog.Error($"Error updating client info: {ex.Message}", ex);
            }
            return Task.CompletedTask;
        }
    }
}
