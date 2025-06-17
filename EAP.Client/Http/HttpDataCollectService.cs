using EAP.Client.Model.Database;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EAP.Client.Http
{
    class HttpDataCollectService : BackgroundService
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");

        private readonly IConfiguration configuration;
        private readonly ISqlSugarClient sqlSugarClient;

        public HttpDataCollectService(IConfiguration configuration, ISqlSugarClient sqlSugarClient)
        {
            this.configuration = configuration;
            this.sqlSugarClient = sqlSugarClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    traLog.Info("Collect");
                    var lnbUrl = configuration.GetSection("Custom")["LnbUrl"].TrimEnd('/');
                    var handler = new HttpClientHandler
                    {
                        CookieContainer = new CookieContainer(),
                        UseCookies = true
                    };
                    using var httpClient = new HttpClient(handler);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    await httpClient.GetAsync($"{lnbUrl}/lws/ChangeChinese.do");


                    var loginData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("userid", "pfsc"),
                    new KeyValuePair<string, string>("password", "1qaz2wsx"),
                    new KeyValuePair<string, string>("btnLogin", "登录"),
                    new KeyValuePair<string, string>("operation","0"),
                    new KeyValuePair<string, string>("icuserid","")
                });

                    var loginResponse = await httpClient.PostAsync($"{lnbUrl}/lws/Login.do", loginData);
                    loginResponse.EnsureSuccessStatusCode();

                    var uri = new Uri("http://192.168.100.3:18580/lws");
                    handler.CookieContainer.Add(uri, new Cookie("language", "zh")
                    {
                        Domain = uri.Host,         // 设置Cookie的域名
                        Path = "/",               // 设置Cookie的路径
                        Expires = DateTime.Now.AddDays(7)  // 设置Cookie有效期
                    });

                    // 4. 获取并显示Cookie
                    var cookies = handler.CookieContainer.GetAllCookies();
                    var sessionId = cookies.FirstOrDefault(it => it.Name == "JSESSIONID");


                    // 5. 切换页面
                    await httpClient.GetAsync($"{lnbUrl}/lws/ReferMachineEventLog.do?startup=menu");

                    var startTime = sqlSugarClient.Queryable<PanasonicEventData>().Max(it => it.EventTime);
                    startTime = startTime > DateTime.Now.AddDays(-3) ? startTime : DateTime.Now.AddDays(-3);
                    var endTime = DateTime.Now.AddHours(1);

                    var queryData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("mode", "1"),
                    new KeyValuePair<string, string>("machinePeriod", "ALL"),
                    new KeyValuePair<string, string>("dateBegin", startTime.ToString("yyyyMMdd")),
                    new KeyValuePair<string, string>("hourBegin", startTime.ToString("HH")),
                    new KeyValuePair<string, string>("dateEnd", endTime.ToString("yyyyMMdd")),
                    new KeyValuePair<string, string>("hourEnd", endTime.ToString("HH")),
                    new KeyValuePair<string, string>("command", "ProcessEventCondition"),
                    new KeyValuePair<string, string>("changeFilter", ""),
                    new KeyValuePair<string, string>("time", "change")
                });
                    var queryDataResponse = await httpClient.PostAsync($"{lnbUrl}/lws/DispatchEventLog.do", queryData);

                    var content = await queryDataResponse.Content.ReadAsStringAsync();

                    //找到出现table的index
                    var tableStartIndex = content.IndexOf("<TABLE BORDER=\"1\" BORDERCOLOR=\"#dddddd\" WIDTH=\"800\">");
                    //找到从tableStartIndex开始第一个出现</TABLE>的位置
                    var tableEndIndex = content.Substring(tableStartIndex).IndexOf("</TABLE>", StringComparison.Ordinal);
                    var tableContent = content.Substring(tableStartIndex, tableEndIndex + "</TABLE>".Length);
                    //xml读取
                    var xml = new System.Xml.XmlDocument();
                    xml.LoadXml(tableContent);
                    var trNodes = xml.SelectNodes("/TABLE/TR");
                    //反向遍历
                    List<PanasonicEventData> datas = new List<PanasonicEventData>();
                    for (int i = trNodes.Count - 1; i >= 0; i--)
                    {
                        var tdNodes = trNodes[i].SelectNodes("TD");
                        var time = tdNodes[0].InnerText.Trim();
                        var machineNo = tdNodes[1].InnerText.Trim();
                        var code = tdNodes[2].InnerText.Trim();
                        var subCode = tdNodes[3].InnerText.Trim();
                        var message = tdNodes[4].InnerText.Trim();
                        var subMessage = tdNodes[5].InnerText.Trim();

                        var data = new PanasonicEventData()
                        {
                            EventTime = DateTime.Parse(time),
                            MachineNo = machineNo,
                            Code = code,
                            SubCode = subCode,
                            Message = message,
                            SubMessage = subMessage
                        };
                        datas.Add(data);
                    }

                    var machineNos = datas.Select(it => it.MachineNo).Distinct();
                    foreach (var machineNo in machineNos)
                    {
                        var machineLatestDate = sqlSugarClient.Queryable<PanasonicEventData>().Where(it => it.MachineNo == machineNo).Max(it => it.EventTime);
                        var latestData = datas.Where(it => it.MachineNo == machineNo && it.EventTime > machineLatestDate).ToList();
                        if (latestData.Count > 0)
                        {
                            sqlSugarClient.Insertable<PanasonicEventData>(latestData).ExecuteCommand();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(10000);
            }
        }

    }
}
