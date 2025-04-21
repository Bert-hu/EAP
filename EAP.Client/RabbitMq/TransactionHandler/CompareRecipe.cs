using EAP.Client.Secs;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Newtonsoft.Json;
using Secs4Net;
using System.Text;
using System.IO;
using static Secs4Net.Item;
using Newtonsoft.Json.Linq;

namespace EAP.Client.RabbitMq
{
    internal class CompareRecipe : ITransactionHandler
    {
        internal readonly ILog dbgLog = LogManager.GetLogger("Debug");

        internal readonly RabbitMqService rabbitMq;
        internal readonly ISecsGem secsGem;
        internal readonly ISecsConnection hsmsConnection;
        internal readonly CommonLibrary commonLibrary;

        public CompareRecipe(RabbitMqService rabbitMq, ISecsGem secsGem, ISecsConnection hsmsConnection, CommonLibrary commonLibrary)
        {
            this.rabbitMq = rabbitMq;
            this.secsGem = secsGem;
            this.hsmsConnection = hsmsConnection;
            this.commonLibrary = commonLibrary;


        }

        public async Task HandleTransaction(RabbitMqTransaction trans)
        {
            var reptrans = trans.GetReplyTransaction();
            try
            {
                var recipename = string.Empty;
                byte[] recipebody = new byte[0];
                var recipeParameters = string.Empty;
                if (trans.Parameters.TryGetValue("RecipeName", out object _rec)) recipename = _rec?.ToString();
                if (trans.Parameters.TryGetValue("RecipeBody", out object _body)) recipebody = Convert.FromBase64String(_body.ToString());
                if (trans.Parameters.TryGetValue("RecipeParameters", out object _parameters)) recipeParameters = _parameters.ToString();

                var guid = Guid.NewGuid().ToString();
                var fastzip = new FastZip();

                //Machine Recipe
                SecsMessage s7f5 = new(7, 5, true)
                {
                    SecsItem = A(recipename)
                };
                var rep = await secsGem.SendAsync(s7f5);
                rep.Name = null;
                var reprecipename = rep.SecsItem[0].GetString();
                var data = rep.SecsItem[1].GetMemory<byte>().ToArray();
                //var strbody = Convert.ToBase64String(data);
                var machineRecipeFile = guid + "\\Machine\\" + recipename + ".zip";
                Directory.CreateDirectory(Path.GetDirectoryName(machineRecipeFile));
                using (FileStream fs = new FileStream(machineRecipeFile, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                }
                var machinePath = Path.GetDirectoryName(machineRecipeFile);
                fastzip.ExtractZip(machineRecipeFile, machinePath, null);
                var machineJsonFile = Directory.GetFiles(machinePath, "*.json", SearchOption.AllDirectories).FirstOrDefault();
                var machineRecipeText = System.IO.File.ReadAllText(machineJsonFile);

                //Server Recipe
                var serverRecipeFile = guid + "\\Server\\" + recipename + ".zip";
                Directory.CreateDirectory(Path.GetDirectoryName(serverRecipeFile));
                using (FileStream fs = new FileStream(serverRecipeFile, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(recipebody, 0, recipebody.Length);
                }
                var serverPath = Path.GetDirectoryName(serverRecipeFile);
                fastzip.ExtractZip(serverRecipeFile, serverPath, null);
                var serverJsonFile = Directory.GetFiles(serverPath, "*.json", SearchOption.AllDirectories).FirstOrDefault();
                var serverRecipeText = System.IO.File.ReadAllText(serverJsonFile);

                var comPareResult = CompareJson(serverRecipeText, serverRecipeText);


                if (comPareResult.Count == 0)
                {
                    reptrans.Parameters.Add("Result", true);
                    reptrans.Parameters.Add("Message", $"Recipe内容一致");
                }
                else
                {
                    var differencesString = string.Join(Environment.NewLine, comPareResult.Select(kvp => $"Key: {kvp.Key}, Server: {kvp.Value.Item1}, Machine: {kvp.Value.Item2}"));

                    reptrans.Parameters.Add("Result", false);
                    reptrans.Parameters.Add("Message", $"recipe内容不一致:{differencesString}");
                }

                Directory.Delete(guid, true);
            }
            catch (Exception ex)
            {
                reptrans.Parameters.Add("Result", false);
                reptrans.Parameters.Add("Message", $"EAP Error: {ex.Message}");
                dbgLog.Error(ex.Message, ex);
            }
            rabbitMq.Produce(trans.ReplyChannel, reptrans);
        }


        Dictionary<string, (string, string)> CompareJson(string json1, string json2)
        {
            var differences = new Dictionary<string, (string, string)>();
            try
            {
                JToken token1 = JToken.Parse(json1);
                JToken token2 = JToken.Parse(json2);
                CompareTokens(token1, token2, "", differences);
            }
            catch (Exception)
            {
                // 处理解析异常
            }
            return differences;
        }

        void CompareTokens(JToken token1, JToken token2, string path, Dictionary<string, (string, string)> differences)
        {
            if (JToken.DeepEquals(token1, token2))
            {
                return;
            }

            if (token1.Type != token2.Type)
            {
                differences[path] = (token1.ToString(), token2.ToString());
                return;
            }

            switch (token1.Type)
            {
                case JTokenType.Object:
                    var jObject1 = (JObject)token1;
                    var jObject2 = (JObject)token2;
                    foreach (var property in jObject1.Properties())
                    {
                        string newPath = string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}";
                        JToken value1 = property.Value;
                        JToken value2 = jObject2[property.Name];
                        if (value2 == null)
                        {
                            differences[newPath] = (value1.ToString(), "null");
                        }
                        else
                        {
                            CompareTokens(value1, value2, newPath, differences);
                        }
                    }
                    foreach (var property in jObject2.Properties())
                    {
                        if (!jObject1.ContainsKey(property.Name))
                        {
                            string newPath = string.IsNullOrEmpty(path) ? property.Name : $"{path}.{property.Name}";
                            differences[newPath] = ("null", property.Value.ToString());
                        }
                    }
                    break;
                case JTokenType.Array:
                    var jArray1 = (JArray)token1;
                    var jArray2 = (JArray)token2;
                    int maxLength = Math.Max(jArray1.Count, jArray2.Count);
                    for (int i = 0; i < maxLength; i++)
                    {
                        string newPath = string.IsNullOrEmpty(path) ? $"[{i}]" : $"{path}[{i}]";
                        JToken value1 = i < jArray1.Count ? jArray1[i] : null;
                        JToken value2 = i < jArray2.Count ? jArray2[i] : null;
                        if (value1 == null)
                        {
                            differences[newPath] = ("null", value2.ToString());
                        }
                        else if (value2 == null)
                        {
                            differences[newPath] = (value1.ToString(), "null");
                        }
                        else
                        {
                            CompareTokens(value1, value2, newPath, differences);
                        }
                    }
                    break;
                default:
                    differences[path] = (token1.ToString(), token2.ToString());
                    break;
            }
        }
    }
}
