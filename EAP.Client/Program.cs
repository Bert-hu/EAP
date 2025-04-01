using EAP.Client.Forms;
using EAP.Client.RabbitMq;
using EAP.Client.Secs;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Secs4Net;
using System.Diagnostics;

namespace EAP.Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string configFilePath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
            XmlConfigurator.Configure(new FileInfo(configFilePath));
            ILog dbgLog = LogManager.GetLogger("Debug");

            string currentProcessPath = Process.GetCurrentProcess().MainModule.FileName;
            string currentProcessName = Process.GetCurrentProcess().ProcessName;

            var runningProcesses = Process.GetProcessesByName(currentProcessName)
                .Where(p =>
                {
                    try
                    {
                        return p.MainModule.FileName == currentProcessPath;
                    }
                    catch
                    {
                        return false; // ��Щϵͳ�����޷����� MainModule�������쳣
                    }
                })
                .ToList();

            if (runningProcesses.Count > 1) // ˵��������ͬ·���ĳ���������
            {
                dbgLog.Error("��ͬ·���ĳ����������У������ظ�������");
                return;
            }


            var host = Host.CreateDefaultBuilder(args).UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ILog>(sp => LogManager.GetLogger(typeof(Program)));

                    //Secs
                    services.AddSecs4Net<SecsLogger>(hostContext.Configuration);
                    services.AddSingleton<CommonLibrary>();


                    //RabbitMqService
                    services.AddRabbitMq();


                    //Http
                    //services.AddHostedService<HttpListenerService>();
                    //services.AddHostedService<WinFormWorker>();
                    services.AddHostedService<SecsWorker>();
                    services.AddSingleton<MainForm>();

                });
            var _host = host.Build();
            var a = _host.StartAsync().IsCompleted;
            var mainForm = _host.Services.GetRequiredService<MainForm>();
            Application.Run(mainForm);

        }
    }
}