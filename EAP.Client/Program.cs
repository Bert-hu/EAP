using EAP.Client.File;
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
using System.Reflection;

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
                        return false; // 有些系统进程无法访问 MainModule，会抛异常
                    }
                })
                .ToList();

            if (runningProcesses.Count > 1) // 说明已有相同路径的程序在运行
            {
                Console.WriteLine("相同路径的程序已在运行，不能重复启动！");
                return;
            }

            string configFilePath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
            XmlConfigurator.Configure(new FileInfo(configFilePath));


            var host = Host.CreateDefaultBuilder(args).UseWindowsService()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("Machine.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //Secs
                    services.AddSecs4Net<SecsLogger>(hostContext.Configuration);

                    //RabbitMqService
                    services.AddRabbitMq();

                    //services.AddHostedService<SecsWorker>();
                    services.AddHostedService<FileSfisWorker>();

                    services.AddSingleton<MainForm>();

                });
            var _host = host.Build();
            var a = _host.StartAsync().IsCompleted;
            var mainForm = _host.Services.GetRequiredService<MainForm>();
            Application.Run(mainForm);

        }
    }
}