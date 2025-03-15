using EAP.Client.Secs.Models;
using log4net;
using Microsoft.Extensions.Configuration;
using Secs4Net;
using Secs4Net.Sml;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace EAP.Client.Secs
{
    public class CommonLibrary
    {
        private readonly List<SecsMessage> LibraryMessages;
        private readonly ILog traLog = LogManager.GetLogger("Trace");
        public SecsConfigs SecsConfigs { get; set; }
        public Dictionary<int, GemCeid> Ceids { get; init; }
        public Dictionary<int, GemReport> Reports { get; init; }
        public Dictionary<int, GemSvid> Svids { get; init; }

        public NameValueCollection CustomSettings { get; init; } = new NameValueCollection();
        public CommonLibrary(IConfiguration configuration)
        {
            //SecsLibrary.sml
            try
            {
                traLog.Info("Start to parsing SecsLibrary.sml");
                string filePath = Path.Combine(AppContext.BaseDirectory, "Secs\\SecsLibrary.sml");
                TextReader reader = new StreamReader(filePath);
                using (TextReader reader1 = RemoveEmptyLines(reader))
                {
                    using (TextReader reader2 = AddSingleQuotesToSxFy(reader1))
                    {
                        IAsyncEnumerable<SecsMessage> _secsMessages = reader2.ToSecsMessages();

                        LibraryMessages = _secsMessages.ToListAsync().Result;
                        traLog.Info($"Parsing SecsLibrary.sml succeed, total {LibraryMessages.Count} messages");
                    }
                }
            }
            catch (Exception ex)
            {
                traLog.Error(ex.Message, ex);
            }

            //Secs config
            try
            {
                traLog.Info("Start to get secs config");
                SecsConfigs = configuration.GetSection("Secs").Get<SecsConfigs>();

                Svids = SecsConfigs.SVIDList.ToDictionary(it => it.ID, it => new GemSvid { ID = it.ID, Name = it.Name });
                Reports = SecsConfigs.ReportList.ToDictionary(it => it.ID, it => new GemReport { ID = it.ID, Svids = it.SvidNames.Select(v => Svids.Values.First(sv => sv.Name == v)).ToList() });
                Ceids = SecsConfigs.CEIDList.ToDictionary(it => it.ID, it => new GemCeid { ID = it.ID, Name = it.Name, Reports = it.ReportIds?.Select(v => Reports.Values.First(rp => rp.ID == v)).ToList() ?? new List<GemReport>() });

            }
            catch (Exception ex)
            {
                traLog.Error(ex.Message, ex);
            }

            //
            var settings = configuration.GetSection("Custom").Get<Dictionary<string, string>>() ?? default;
            foreach (var kvp in settings)
            {
                CustomSettings.Add(kvp.Key, kvp.Value);
            }

        }
        public SecsMessage StringToSecsMessage(string str)
        {
            using var sr = new StringReader(str);
            using (TextReader reader1 = RemoveEmptyLines(sr))
            {
                using (TextReader reader2 = AddSingleQuotesToSxFy(reader1))
                {
                    return reader2.ToSecsMessage();
                }
            }
        }

        public GemSvid GetGemSvid(string svidname)
        {
            return Svids.FirstOrDefault(it => it.Value.Name == svidname).Value;
        }

        private TextReader RemoveEmptyLines(TextReader reader)
        {
            return new StringReader(
                string.Join(Environment.NewLine,
                    reader.ReadToEnd()
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                )
            );
        }

        private TextReader AddSingleQuotesToSxFy(TextReader inputReader)
        {
            StringBuilder outputText = new StringBuilder();
            string line;
            while ((line = inputReader.ReadLine()) != null)
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex != -1)
                {
                    string beforeColon = line.Substring(0, colonIndex + 1);  // 包括冒号
                    string afterColon = line.Substring(colonIndex + 1).Trim();  // 去掉冒号后的空格

                    if (afterColon.StartsWith("S"))
                    {
                        // 如果冒号后面以S开头，则在SxFy后添加单引号
                        int spaceIndex = afterColon.IndexOf(' ');
                        if (spaceIndex != -1)
                        {
                            string sxFyPart = afterColon.Substring(0, spaceIndex);
                            string restPart = afterColon.Substring(spaceIndex);
                            line = beforeColon + "'" + sxFyPart + "'" + restPart;
                        }
                        else
                        {
                            line = beforeColon + "'" + afterColon + "'";
                        }
                    }
                }
                else
                {
                    string pattern = @"S\d+F\d+"; // 使用正则表达式匹配SxFy格式，其中x和y是任意数字
                    line = Regex.Replace(line, pattern, "'$0'"); // 将匹配到的内容加上单引号
                }
                outputText.AppendLine(line);
            }

            return new StringReader(outputText.ToString());
        }
        public SecsMessage? GetSecsMessageByName(string name)
        {
            return LibraryMessages?.FirstOrDefault(it => (it.Name ?? "").StartsWith(name));
        }

    }
}
