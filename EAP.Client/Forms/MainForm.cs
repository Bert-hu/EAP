using AutoUpdaterDotNET;
using EAP.Client.RabbitMq;
using EAP.Client.Secs;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Secs4Net;
using Sunny.UI;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Threading;
using static Secs4Net.Item;

namespace EAP.Client.Forms
{
    public partial class MainForm : UIForm
    {
        private static MainForm instance;
        private readonly IConfiguration configuration;
        private readonly ISecsConnection secsConnection;
        private readonly CommonLibrary commonLibrary;
        private readonly ISecsGem secsGem;
        private readonly RabbitMq.RabbitMqService rabbitMqservice;
        internal static ILog traLog = LogManager.GetLogger("Trace");
        internal static ILog dbgLog = LogManager.GetLogger("Debug");
        int inputTrayCount = 0;
        int outputTrayCount = 0;
        public int InputTrayCount
        {
            get { return inputTrayCount; }
            set
            {
                inputTrayCount = value;
                this.Invoke(new Action(() =>
                {
                    uiLedLabel_inputTrayCount.Text = inputTrayCount.ToString();
                }));
            }
        }
        public int OutputTrayCount
        {
            get { return outputTrayCount; }
            set
            {
                outputTrayCount = value;
                this.Invoke(new Action(() =>
                {
                    uiLedLabel_outputTrayCount.Text = outputTrayCount.ToString();
                }));
            }
        }

        public static MainForm Instance
        {
            get
            {
                if (instance == null)
                {
                    //instance = new MainForm();
                }
                return instance;
            }
        }

        public MainForm(IConfiguration configuration, ISecsConnection secsConnection, CommonLibrary commonLibrary, ISecsGem secsGem, RabbitMq.RabbitMqService rabbitMq)
        {
            this.configuration = configuration;
            this.secsConnection = secsConnection;
            this.commonLibrary = commonLibrary;
            this.secsGem = secsGem;
            rabbitMqservice = rabbitMq;
            instance = this;
            InitializeComponent();

            this.secsConnection.ConnectionChanged += _secsConnection_ConnectionChanged;
            var appender = LogManager.GetRepository().GetAppenders().First(it => it.Name == "TraceLog") as RichTextBoxAppender;
            appender.RichTextBox = this.richTextBox1;

        }

        private void _secsConnection_ConnectionChanged(object? sender, ConnectionState e)
        {
            string showtext = "Connecting";
            var backcolor = Color.Gray;
            switch (e)
            {
                case ConnectionState.Retry:
                case ConnectionState.Connecting:
                    showtext = "Connecting";
                    backcolor = Color.Gray;
                    break;
                case ConnectionState.Connected:
                    showtext = "Connected";
                    backcolor = Color.Yellow;
                    break;
                case ConnectionState.Selected:
                    showtext = "Connected";
                    backcolor = Color.Green;
                    break;
            }
            this.Invoke(new Action(() =>
            {
                label_conn_status.Text = showtext;
                label_conn_status.BackColor = backcolor;
            }));
        }
        public void UpdateMachineRecipe(string recipename)
        {
            this.Invoke(new Action(() =>
            {
                this.textBox_machinerecipe.Text = recipename;
            }));
        }
        public void UpdateAoiPanelAndModelname(string panelid, string modelname)
        {
            this.Invoke(new Action(() =>
            {

                this.label_updatetime_aoi.Text = "Update Time: " + DateTime.Now.ToString("MM-dd HH:mm:ss");
            }));
        }

        public void UpdateState(string state)
        {
            this.Invoke(new Action(() =>
            {
                var backcolor = Color.Gray;
                switch (state)
                {
                    case "Run":
                        backcolor = Color.Green;
                        break;
                    case "Alarm":
                        backcolor = Color.Red;
                        break;
                    case "Idle":
                        backcolor = Color.Yellow;
                        break;
                    default:
                        backcolor = Color.Gray;
                        break;
                }
                this.label_ProcessState.Text = state;
                this.label_ProcessState.BackColor = backcolor;

            }));
        }

        public bool ConfirmMessageBox(string showtext)
        {
            var confirm = UIMessageBox.ShowAsk2($"{showtext}");
            return confirm;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string showtext = "Connecting";
            var backcolor = Color.Gray;
            switch (secsConnection?.State)
            {
                case ConnectionState.Retry:
                case ConnectionState.Connecting:
                    showtext = "Connecting";
                    backcolor = Color.Gray;
                    break;
                case ConnectionState.Connected:
                    showtext = "Connected";
                    backcolor = Color.Yellow;
                    break;
                case ConnectionState.Selected:
                    showtext = "Connected";
                    backcolor = Color.Green;
                    break;
            }
            this.Invoke(new Action(() =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                this.Text = commonLibrary.CustomSettings["EquipmentId"] + " " + commonLibrary.CustomSettings["EquipmentType"] + " Version: " + assembly.GetName().Version.ToString();
                notifyIcon.Text = commonLibrary.CustomSettings["EquipmentType"] + " " + commonLibrary.CustomSettings["EquipmentId"];
                label_conn_status.Text = showtext;
                label_conn_status.BackColor = backcolor;
            }));

            string sfisIp = commonLibrary.CustomSettings["SfisIp"];
            int sfisPort = Convert.ToInt32(commonLibrary.CustomSettings["SfisPort"]);
            //BaymaxService baymax = new BaymaxService();
            //baymax.OnBaymaxTransCompleted += Baymax_OnBaymaxTrans;
            //baymax.StartBaymaxForwardingService(secsConnection.IpAddress.ToString(), 21347, sfisIp, sfisPort, HandleBaymaxResponse);

            var updateUrl = configuration.GetSection("Custom")["UpdateUrl"].TrimEnd('/') + "/" + commonLibrary.CustomSettings["EquipmentType"] + "/AutoUpdate.xml";
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("zh");
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 1;
            AutoUpdater.ReportErrors = true;

            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;

            AutoUpdater.Start(updateUrl);
            UpdateMachineIP(); //更新机器IP

            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };//定时去检测更新根据自己业务需求
            timer.Tick += delegate
            {
                AutoUpdater.Start(updateUrl);
                UpdateMachineIP(); //更新机器IP
            };
            timer.Start();
        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    var eqpType = commonLibrary.CustomSettings["EquipmentType"];
                    this.Text = commonLibrary.CustomSettings["EquipmentId"] + " " + commonLibrary.CustomSettings["EquipmentType"] + " Version: " + args.InstalledVersion + " 需要更新";
                    //bool dialogResult =
                    //        UIMessageBox.ShowAsk2(
                    //            $@"新版本 {eqpType + ":" + args.CurrentVersion} 可用. 当前版本 {eqpType + ":" + args.InstalledVersion}. 如果设备空闲请点击确认更新并重启，否则点击取消");


                    // Uncomment the following line if you want to show standard update dialog instead.
                    //AutoUpdater.ShowUpdateForm(args);

                    if (true)
                    {
                        try
                        {
                            if (AutoUpdater.DownloadUpdate(args))
                            {
                                Environment.Exit(0);
                            }
                        }
                        catch (Exception exception)
                        {
                            UIMessageBox.ShowError(exception.Message);
                        }
                    }
                }
                else
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    this.Text = commonLibrary.CustomSettings["EquipmentId"] + " " + commonLibrary.CustomSettings["EquipmentType"] + " Version: " + args.InstalledVersion + " 最新版本";
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    //UIMessageBox.ShowError(@"There is a problem reaching update server. Please check your internet connection and try again later.");
                    this.Text = configuration.GetSection("Custom")["EquipmentId"] + " " + configuration.GetSection("Custom")["EquipmentType"];
                }
                else
                {
                    UIMessageBox.ShowError(args.Error.Message);
                }
            }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //确认后关闭
            var dr = UIMessageBox.ShowAsk2("是否关闭程序？");
            if (dr)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }



        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    this.Hide();
            //}
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            GetMachineInfo();
        }

        public void GetMachineInfo()
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = commonLibrary.CustomSettings["EquipmentId"],
                    TransactionName = "GetMachineInfo",
                    NeedReply = true,
                    ExpireSecond = 3,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"]
                };

                var info = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (info != null)
                {
                    //AGV Enabled, Loader Count, Unloader Count
                    InputTrayCount = info.Parameters.ContainsKey("InputTrayCount") ? Convert.ToInt32(info.Parameters["InputTrayCount"]) : 0;
                    OutputTrayCount = info.Parameters.ContainsKey("OutputTrayCount") ? Convert.ToInt32(info.Parameters["OutputTrayCount"]) : 0;

                }
                else
                {
                    traLog.Warn($"获取设备信息超时。");
                }

            }
            catch (Exception ex)
            {
                traLog.Error($"获取设备信息失败： {ex.ToString()}");
            }
        }

        public void UpdateMachineIP()
        {
            var ips = Dns.GetHostAddresses(Dns.GetHostName());
            // 过滤测试网段的Ipv4地址
            var ipv4 = ips.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork &&
                    ip.ToString().StartsWith("10.6"));
            if (ipv4 != null)
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = commonLibrary.CustomSettings["EquipmentId"],
                    TransactionName = "UpdateMachineIP",
                    NeedReply = true,
                    ExpireSecond = 5,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "MachineIP", ipv4.ToString() }
                    }
                };
                rabbitMqservice.Produce("HandlerAgv.Service", trans);
            }
        }

        public bool UpdateMachineInputTrayCount(int count)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = commonLibrary.CustomSettings["EquipmentId"],
                    TransactionName = "UpdateMachineInputTrayCount",
                    NeedReply = true,
                    ExpireSecond = 5,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "InputTrayCount", count }
                    }
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        return true;
                    }
                    else
                    {
                        traLog.Warn($"更新入料口盘数失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    traLog.Warn($"更新入料口盘数超时。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"更新入料口盘数失败: {ex.ToString()}");
                return false;
            }
        }

        public bool UpdateMachineOutputTrayCount(int count)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = commonLibrary.CustomSettings["EquipmentId"],
                    TransactionName = "UpdateMachineOutputTrayCount",
                    NeedReply = true,
                    ExpireSecond = 5,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "OutputTrayCount", count }
                    }
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        return true;
                    }
                    else
                    {
                        traLog.Warn($"更新出料口盘数失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    traLog.Warn($"更新出料口盘数超时。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"更新出料口盘数失败: {ex.ToString()}");
                return false;
            }

        }

        private void uiButton_inputTrayCount_Click(object sender, EventArgs e)
        {
            EditValueForm editValueForm = new EditValueForm("修改入料口盘数", InputTrayCount.ToString());
            if (editValueForm.ShowDialog() == DialogResult.OK)
            {
                if (int.TryParse(editValueForm.Value, out int newCount) && newCount >= 0)
                {
                    if (UpdateMachineInputTrayCount(newCount))
                    {
                        InputTrayCount = newCount;
                        traLog.Info("入料口盘数更新成功");
                    }
                    else
                    {
                        traLog.Warn("入料口盘数更新失败");
                    }
                }
                else
                {
                    UIMessageBox.ShowWarning("请输入有效的盘数");
                }
            }
        }

        private void uiButton_outputTrayCount_Click(object sender, EventArgs e)
        {
            EditValueForm editValueForm = new EditValueForm("修改出料口盘数", OutputTrayCount.ToString());
            if (editValueForm.ShowDialog() == DialogResult.OK)
            {
                if (int.TryParse(editValueForm.Value, out int newCount) && newCount >= 0)
                {
                    if (UpdateMachineOutputTrayCount(newCount))
                    {
                        OutputTrayCount = newCount;
                        traLog.Info("出料口盘数更新成功");
                    }
                    else
                    {
                        traLog.Warn("出料口盘数更新失败");
                    }
                }
                else
                {
                    UIMessageBox.ShowWarning("请输入有效的盘数");
                }
            }
        }
    }
}
