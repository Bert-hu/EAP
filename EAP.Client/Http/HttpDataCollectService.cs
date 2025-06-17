using log4net;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EAP.Client.Http
{
    class HttpDataCollectService : BackgroundService
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    traLog.Info("Collect");
                    var handler = new HttpClientHandler
                    {
                        CookieContainer = new CookieContainer(),
                        UseCookies = true
                    };
                    using var httpClient = new HttpClient(handler);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    await httpClient.GetAsync("http://192.168.100.3:18580/lws/ChangeChinese.do");


                    var loginData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("userid", "pfsc"),
                    new KeyValuePair<string, string>("password", "1qaz2wsx"),
                    new KeyValuePair<string, string>("btnLogin", "登录"),
                    new KeyValuePair<string, string>("operation","0"),
                    new KeyValuePair<string, string>("icuserid","")
                });

                    var loginResponse = await httpClient.PostAsync("http://192.168.100.3:18580/lws/Login.do", loginData);
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
                    await httpClient.GetAsync("http://192.168.100.3:18580/lws/ReferMachineEventLog.do?startup=menu");

                    var queryData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("mode", "1"),
                    new KeyValuePair<string, string>("machinePeriod", "ALL"),
                    new KeyValuePair<string, string>("dateBegin", "20250615"),
                    new KeyValuePair<string, string>("hourBegin", "15"),
                    new KeyValuePair<string, string>("dateEnd", "20250616"),
                    new KeyValuePair<string, string>("hourEnd", "15"),
                    new KeyValuePair<string, string>("command", "ProcessEventCondition"),
                    new KeyValuePair<string, string>("changeFilter", ""),
                    new KeyValuePair<string, string>("time", "change")
                });
                    var queryDataResponse = await httpClient.PostAsync("http://192.168.100.3:18580/lws/DispatchEventLog.do", queryData);

                    var content = await queryDataResponse.Content.ReadAsStringAsync();


                    //xml读取
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(1000);
            }
        }

    }
}
