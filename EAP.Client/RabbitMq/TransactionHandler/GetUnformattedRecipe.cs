using log4net;
using Newtonsoft.Json;
using Secs4Net;
using Secs4Net.Sml;
using EAP.Client.Secs;
using static Secs4Net.Item;
using Microsoft.Extensions.Configuration;
using System.IO.Compression;
using EAP.Client.Model;
using System.Text.RegularExpressions;

namespace EAP.Client.RabbitMq
{
    internal class GetUnformattedRecipe : ITransactionHandler
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        internal readonly RabbitMqService rabbitMq;
        internal readonly IConfiguration configuration;
        public static RecipePara lastReadRecipePara;

        public GetUnformattedRecipe(RabbitMqService rabbitMq, IConfiguration configuration)
        {
            this.rabbitMq = rabbitMq;
            this.configuration = configuration;
        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            var recipename = string.Empty;

            try
            {
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();

                var recipePath = configuration.GetSection("Custom")["MachineRecipePath"];
                var filePath = Path.Combine(recipePath, recipename);
                if (!System.IO.File.Exists(filePath))
                {
                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"Recipe {recipename} not found");
                }
                else
                {

                    reptrans.Parameters.Add("RecipeName", recipename);
                    //文件Byte
                    var bytebody = CompressFile(filePath);
                    reptrans.Parameters.Add("RecipeBody", Convert.ToBase64String(bytebody));


                    //TODO：解析的参数，暂时使用源文件文本
                    //var recipeParameters = GetRecipeParameters(filePath);
                    var recipeText = System.IO.File.ReadAllText(filePath);
                    var recipeParameters = GetRecipePara(recipename, recipeText);
                    reptrans.Parameters.Add("RecipeParameters", recipeParameters);
                    reptrans.Parameters.Add("Result", true);
                }
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"解析recipe {recipename}异常:{ex.Message}");
                traLog.Warn($"解析recipe {recipename}异常:{ex.Message}");
                traLog.Error(ex.Message);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }

        byte[] CompressFile(string filePath)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    string entryName = Path.GetFileName(filePath);
                    ZipArchiveEntry entry = archive.CreateEntry(entryName);
                    entry.LastWriteTime = System.IO.File.GetLastWriteTime(filePath);
                    using (Stream entryStream = entry.Open())
                    using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
                return memoryStream.ToArray();
            }
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

        public static RecipePara? GetRecipePara(string recipeName, string recipeText)
        {

            if (recipeName == "mykonos_uf.rcp")
            {
                RecipePara para = new RecipePara();
                // 将内容按换行符分割成行
                string[] lines = recipeText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                //文本行数，换算成index需要-1
                var preheatTimeLineIndex = 1685;
                var nozzleTempLineIndex = 1506;
                var preheatTempLineIndex = 1706;
                var heatTempLineIndex = 1753;
                // 解析 Preheat Time
                ParseAndSetValue(lines, preheatTimeLineIndex - 1, "Preheat Time", value => para.PreheatTime = value, recipeName);
                // 解析 Nozzle Temp
                ParseAndSetValue(lines, nozzleTempLineIndex - 1, "Set Point", value => para.NozzleTemp = value, recipeName);
                // 解析 Preheat Temp
                ParseAndSetValue(lines, preheatTempLineIndex - 1, "Set Point", value => para.PreheatTemp = value, recipeName);
                // 解析 Heat Temp
                ParseAndSetValue(lines, heatTempLineIndex - 1, "Set Point", value => para.HeatTemp = value, recipeName);

                int glueStartLine = 240;
                int guleEndLine = 262;
                var glueConfigs = ExtractGlueConfigs(lines.Skip(glueStartLine - 1).Take(guleEndLine - glueStartLine).ToArray());
                para.GlueConfigs = glueConfigs;
                lastReadRecipePara = para;
                return para;
            }
            else
            {
                return null;
            }

        }

        private static void ParseAndSetValue(string[] lines, int lineIndex, string expectedKey, Action<double> setValue, string recipeName)
        {
            try
            {
                if (lineIndex < 0 || lineIndex >= lines.Length)
                {
                    throw new IndexOutOfRangeException($"行索引 {lineIndex} 超出了 lines 数组的范围。");
                }

                var parts = lines[lineIndex].Split('=');
                if (parts.Length != 2)
                {
                    throw new FormatException($"行 {lineIndex} 格式错误，未找到 '=' 分隔符。");
                }

                var key = parts[0].Trim();
                var valueStr = parts[1].Trim();

                if (key != expectedKey)
                {
                    throw new FormatException($"行 {lineIndex} 预期的键是 '{expectedKey}'，但实际是 '{key}'。");
                }

                if (double.TryParse(valueStr, out double value))
                {
                    setValue(value);
                }
                else
                {
                    throw new FormatException($"行 {lineIndex} 的值 '{valueStr}' 无法解析为 double 类型。");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"解析 recipe {recipeName} 异常: {ex.Message}", ex);
            }
        }

        static List<GlueConfig> ExtractGlueConfigs(string[] lines)
        {
            List<GlueConfig> configs = new List<GlueConfig>();
            int currentPass = 0;
            int table = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("START PASS: FOR PASS"))
                {
                    currentPass = int.Parse(Regex.Match(line, @"\d+").Value);
                    table = 0;
                }
                else if (line.StartsWith("WEIGHT CONTROL:"))
                {
                    table++;
                    string[] parts = line.Split(new[] { ':', ',', '(', ')' });
                    GlueConfig config = new GlueConfig
                    {
                        Pass = currentPass,
                        Table = table,
                        Weight = double.Parse(parts[2]),
                        StartX = double.Parse(parts[9]),
                        StartY = double.Parse(parts[10]),
                        EndX = double.Parse(parts[13]),
                        EndY = double.Parse(parts[14])
                    };
                    configs.Add(config);
                }
            }

            return configs;
        }
    }
}
