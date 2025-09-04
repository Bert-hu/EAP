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
                if (trans?.Parameters.TryGetValue("InputTrayCount", out object inputTrayCount) ?? false)
                {
                    MainForm.Instance.InputTrayCount = int.Parse(inputTrayCount.ToString());
                }

                if (trans?.Parameters.TryGetValue("OutputTrayCount", out object outputTrayCount) ?? false)
                {
                    MainForm.Instance.OutputTrayCount = int.Parse(outputTrayCount.ToString());
                }

                if (trans?.Parameters.TryGetValue("AgvEnabled", out object agvEnabled) ?? false)
                {
                    MainForm.Instance.AgvEnabled = bool.Parse(agvEnabled.ToString());
                }

                if(trans?.Parameters.TryGetValue("CurrentTaskState", out object currentTaskState) ?? false)
                {
                    MainForm.Instance.CurrentTaskState = currentTaskState.ToString();
                }

                if(trans?.Parameters.TryGetValue("CurrentLot", out object currentLot) ?? false)
                {
                    MainForm.Instance.CurrentLot = currentLot?.ToString();
                }

                if(trans?.Parameters.TryGetValue("GroupName", out object groupName) ?? false)
                {
                    MainForm.Instance.GroupName = groupName?.ToString();
                }

                if(trans?.Parameters.TryGetValue("MaterialName", out object materialName) ?? false)
                {
                    MainForm.Instance.MaterialName = materialName?.ToString();
                }
                if (trans?.Parameters.TryGetValue("AgvInventory", out object agvInventory) ?? false)
                {
                    MainForm.Instance.AgvInventory = agvInventory?.ToString();
                }
                if (trans?.Parameters.TryGetValue("StockInventory", out object stockerInventory) ?? false)
                {
                    MainForm.Instance.StockerInventory = stockerInventory?.ToString();
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error($"Error updating client info: {ex.Message}", ex);
            }
            return Task.CompletedTask;
        }
    }
}
