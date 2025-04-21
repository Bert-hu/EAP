using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using static Secs4Net.Item;
using ICSharpCode.SharpZipLib.Zip;

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

                var guid = Guid.NewGuid().ToString();//
                var machineRecipeFile = guid + "\\Machine\\" + recipename + ".zip";
                Directory.CreateDirectory(Path.GetDirectoryName(machineRecipeFile));
                using (FileStream fs = new FileStream(machineRecipeFile, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                }

                var fastzip = new FastZip();
                var machinePath = Path.GetDirectoryName(machineRecipeFile);
                fastzip.ExtractZip(machineRecipeFile, machinePath, null);
                var machineJsonFile = Directory.GetFiles(machinePath, "*.json", SearchOption.AllDirectories).FirstOrDefault();
                var machineRecipeText = System.IO.File.ReadAllText(machineJsonFile);


                reptrans.Parameters.Add("Result", true);
                reptrans.Parameters.Add("RecipeName", reprecipename);
                reptrans.Parameters.Add("RecipeBody", strbody);
                reptrans.Parameters.Add("RecipeParameters", machineRecipeText);
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
