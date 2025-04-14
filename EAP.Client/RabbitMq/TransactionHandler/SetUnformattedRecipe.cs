using log4net;
using Newtonsoft.Json;
using Secs4Net;
using EAP.Client.Secs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Secs4Net.Item;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;

namespace EAP.Client.RabbitMq
{
    internal class SetUnformattedRecipe : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        internal readonly RabbitMqService rabbitMq;
        internal readonly IConfiguration configuration;
        public SetUnformattedRecipe(RabbitMqService rabbitMq, IConfiguration configuration)
        {
            this.rabbitMq = rabbitMq;
            this.configuration = configuration; 
        }

        public  async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var Message = string.Empty;
                var recipename = string.Empty;
                byte[] recipebody = new byte[0];
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) recipebody = Convert.FromBase64String(_body.ToString());

                var recipePath = configuration.GetSection("Custom")["MachineRecipePath"];
                var filePath = Path.Combine(recipePath, recipename);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                DecompressFile(recipebody, filePath);
                reptrans.Parameters.Add("Result", true);
                reptrans.Parameters.Add("Message", $"Success");
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error: {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);

        }
        void DecompressFile(byte[] data, string outputFilePath)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
            {
                if (archive.Entries.Count != 1)
                {
                    // Handle error: The archive should contain exactly one entry for a single file
                    throw new InvalidOperationException("Invalid archive format for a single file decompression.");
                }
                ZipArchiveEntry entry = archive.Entries[0];
                using (Stream entryStream = entry.Open())
                using (FileStream fileStream = System.IO.File.Create(outputFilePath))
                {
                    entryStream.CopyTo(fileStream);
                }
                System.IO.File.SetLastWriteTime(outputFilePath, entry.LastWriteTime.DateTime);
            }
        }
    }
}
