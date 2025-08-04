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
        bool agvEnabled = false;
        bool agvLocked = false;
        string currentTaskState = "无AGV任务";
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
        public bool AgvEnabled
        {
            get { return agvEnabled; }
            set
            {
                agvEnabled = value;
                this.Invoke(new Action(() =>
                {
                    uiCheckBox_agvEnabled.Checked = agvEnabled;
                    uiButton_swichAgvMode.Text = agvEnabled ? "关闭AGV模式" : "开启AGV模式";
                    uiButton_swichAgvMode.Style = agvEnabled ? UIStyle.Orange : UIStyle.Blue;
                }));
            }
        }
        /// <summary>
        /// 通过SVID查询设备是否AGV锁定
        /// </summary>
        public bool AgvLocked
        {
            get { return agvLocked; }
            set
            {
                agvLocked = value;
                this.Invoke(new Action(() =>
                {
                    uiCheckBox_agvLocked.Checked = agvLocked;
                }));
            }
        }

        public string CurrentTaskState
        {
            get { return currentTaskState; }
            set
            {
                currentTaskState = value;
                this.Invoke(new Action(() =>
                {
                    uiLabel_currenttaskState.Text = currentTaskState;
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

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            await Task.Run(() => GetMachineInfo());
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
                    if (info.Parameters.ContainsKey("Result") && !Convert.ToBoolean(info.Parameters["Result"]))
                    {
                        traLog.Warn($"获取设备信息失败: {(info.Parameters.ContainsKey("Message") ? info.Parameters["Message"].ToString() : "未知错误")}");
                    }
                    else
                    {
                        //AGV Enabled, Loader Count, Unloader Count
                        AgvEnabled = info.Parameters.ContainsKey("AgvEnabled") ? Convert.ToBoolean(info.Parameters["AgvEnabled"]) : false;
                        InputTrayCount = info.Parameters.ContainsKey("InputTrayCount") ? Convert.ToInt32(info.Parameters["InputTrayCount"]) : 0;
                        OutputTrayCount = info.Parameters.ContainsKey("OutputTrayCount") ? Convert.ToInt32(info.Parameters["OutputTrayCount"]) : 0;
                        CurrentTaskState = info.Parameters.ContainsKey("CurrentTaskState") ? info.Parameters["CurrentTaskState"].ToString() : "无AGV任务";
                    }
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
            var ipv4ips = ips.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
            var ipv4 = ipv4ips.FirstOrDefault(ip => ip.ToString().StartsWith("10.6"));
            if (ipv4 == null)
            {
                // 如果没有找到10.6开头的IP，取第一个IPv4地址
                ipv4 = ipv4ips.FirstOrDefault();
            }

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
        public bool UpdateAgvEnabled(bool enabled)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = commonLibrary.CustomSettings["EquipmentId"],
                    TransactionName = "UpdateAgvEnabled",
                    NeedReply = true,
                    ExpireSecond = 3,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "AgvEnabled", enabled }
                    }
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        traLog.Info($"更新AGV模式成功: {(enabled ? "开启" : "关闭")}");
                        return true;
                    }
                    else
                    {
                        traLog.Warn($"更新AGV模式失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    traLog.Warn($"更新AGV模式超时。");
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public (bool, string) SemdAgvTask(string taskType)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = commonLibrary.CustomSettings["EquipmentId"],
                    TransactionName = $"Send{taskType}Task",
                    NeedReply = true,
                    ExpireSecond = 6,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"]
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    if (reply.Parameters.ContainsKey("Result") && Convert.ToBoolean(reply.Parameters["Result"]))
                    {
                        traLog.Info($"{taskType}任务发送成功");
                        return (true, $"{taskType}任务发送成功");
                    }
                    else
                    {
                        string message = reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误";
                        traLog.Warn($"{taskType}任务发送失败: {message}");
                        return (false, $"{taskType}任务发送失败: {message}");
                    }
                }
                else
                {
                    traLog.Warn($"{taskType}任务发送超时。");
                    return (false, $"{taskType}任务发送超时");
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"发送AGV任务失败: {ex.ToString()}");
                return (false, "发送AGV任务失败");
            }
        }

        private async void uiButton_inputTrayCount_Click(object sender, EventArgs e)
        {
            EditValueForm editValueForm = new EditValueForm("修改入料口盘数", InputTrayCount.ToString());
            if (editValueForm.ShowDialog() == DialogResult.OK)
            {
                if (int.TryParse(editValueForm.Value, out int newCount) && newCount >= 0)
                {
                    // 显示等待状态
                    Cursor = Cursors.WaitCursor;
                    uiButton_inputTrayCount.Enabled = false;

                    try
                    {
                        // 在后台线程执行耗时操作
                        bool updateResult = await Task.Run(() => UpdateMachineInputTrayCount(newCount));

                        if (updateResult)
                        {
                            InputTrayCount = newCount;
                            traLog.Info("入料口盘数更新成功");
                        }
                        else
                        {
                            traLog.Warn("入料口盘数更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        traLog.Error($"更新入料口盘数时发生异常: {ex.Message}");
                        UIMessageBox.ShowError("更新过程中发生错误");
                    }
                    finally
                    {
                        // 恢复界面状态
                        Cursor = Cursors.Default;
                        uiButton_inputTrayCount.Enabled = true;
                    }
                }
                else
                {
                    UIMessageBox.ShowWarning("请输入有效的盘数");
                }
            }
        }

        private async void uiButton_outputTrayCount_Click(object sender, EventArgs e)
        {
            EditValueForm editValueForm = new EditValueForm("修改出料口盘数", OutputTrayCount.ToString());
            if (editValueForm.ShowDialog() == DialogResult.OK)
            {
                if (int.TryParse(editValueForm.Value, out int newCount) && newCount >= 0)
                {
                    // 显示等待状态
                    Cursor = Cursors.WaitCursor;
                    uiButton_outputTrayCount.Enabled = false;

                    try
                    {
                        // 在后台线程执行耗时操作
                        bool updateResult = await Task.Run(() => UpdateMachineOutputTrayCount(newCount));

                        if (updateResult)
                        {
                            OutputTrayCount = newCount;
                            traLog.Info("出料口盘数更新成功");
                        }
                        else
                        {
                            traLog.Warn("出料口盘数更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        traLog.Error($"更新出料口盘数时发生异常: {ex.Message}");
                        UIMessageBox.ShowError("更新过程中发生错误");
                    }
                    finally
                    {
                        // 恢复界面状态
                        Cursor = Cursors.Default;
                        uiButton_outputTrayCount.Enabled = true;
                    }
                }
                else
                {
                    UIMessageBox.ShowWarning("请输入有效的盘数");
                }
            }
        }

        private async void uiButton_swichAgvMode_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            uiButton_swichAgvMode.Enabled = false;

            try
            {
                bool newState = !AgvEnabled;
                string confirmMsg = newState
                    ? $"确定要开启AGV模式吗？当前入料口盘数{InputTrayCount}，出料口盘数{OutputTrayCount}，请确认盘数正确。"
                    : "确定要关闭AGV模式吗？";

                if (ConfirmMessageBox(confirmMsg))
                {
                    bool updateResult = await Task.Run(() => UpdateAgvEnabled(newState));
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"切换AGV模式时发生异常: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
                uiButton_swichAgvMode.Enabled = true;
            }
        }

        private async void uiButton_sendInputTask_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            uiButton_sendInputTask.Enabled = false;

            try
            {
                string confirmMsg = "确定要发送Input任务吗？请确认入料口盘数正确。";
                if (ConfirmMessageBox(confirmMsg))
                {
                    var (result, message) = await Task.Run(() => SemdAgvTask("Input"));
                    if (result)
                    {
                        UIMessageBox.ShowSuccess(message);
                    }
                    else
                    {
                        UIMessageBox.ShowError(message);
                    }
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"发送Input任务时发生异常: {ex.Message}");
                UIMessageBox.ShowError("发送Input任务时发生错误");
            }
            finally
            {
                Cursor = Cursors.Default;
                uiButton_sendInputTask.Enabled = true;
            }
        }

        private async void uiButton_sendOutputTask_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            uiButton_sendOutputTask.Enabled = false;

            try
            {
                string confirmMsg = "确定要发送Output任务吗？请确认出料口盘数正确。";
                if (ConfirmMessageBox(confirmMsg))
                {
                    var (result, message) = await Task.Run(() => SemdAgvTask("Output"));
                    if (result)
                    {
                        UIMessageBox.ShowSuccess(message);
                    }
                    else
                    {
                        UIMessageBox.ShowError(message);
                    }
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"发送Output任务时发生异常: {ex.Message}");
                UIMessageBox.ShowError("发送Output任务时发生错误");
            }
            finally
            {
                Cursor = Cursors.Default;
                uiButton_sendOutputTask.Enabled = true;
            }

        }

        private async void uiButton_sendInputOutputTask_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            uiButton_sendInputOutputTask.Enabled = false;

            try
            {
                string confirmMsg = "确定要发送InputOutput任务吗？请确认入料口盘数和出料口盘数正确。";
                if (ConfirmMessageBox(confirmMsg))
                {
                    var (result, message) = await Task.Run(() => SemdAgvTask("InputOutput"));
                    if (result)
                    {
                        UIMessageBox.ShowSuccess(message);
                    }
                    else
                    {
                        UIMessageBox.ShowError(message);
                    }
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"发送InputOutput任务时发生异常: {ex.Message}");
                UIMessageBox.ShowError("发送InputOutput任务时发生错误");
            }
            finally
            {
                Cursor = Cursors.Default;
                uiButton_sendInputOutputTask.Enabled = true;
            }
        }
    }
}
