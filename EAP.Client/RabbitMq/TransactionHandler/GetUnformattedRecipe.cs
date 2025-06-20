﻿using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using static Secs4Net.Item;

namespace EAP.Client.RabbitMq
{
    internal class GetUnformattedRecipe : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;

        public GetUnformattedRecipe(RabbitMqService rabbitMq, ISecsGem secsGem) 
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipename = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();

                SecsMessage s7f5 = new(7, 5, true)
                {
                    SecsItem = A(recipename)
                };
                var rep = await secsGem.SendAsync(s7f5);
                rep.Name = null;
                var reprecipename = rep.SecsItem[0].GetString();
                var data = rep.SecsItem[1].GetMemory<byte>().ToArray();

                var strbody = Convert.ToBase64String(data);
                reptrans.Parameters.Add("Result", true);
                reptrans.Parameters.Add("RecipeName", reprecipename);
                reptrans.Parameters.Add("RecipeBody", strbody);
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error: {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }
    }
}
