using AutoUpdaterDotNET;
using EAP.Client.RabbitMq;
using EAP.Client.Secs;
using EAP.Client.Service;
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
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Threading;
using static Secs4Net.Item;

namespace EAP.Client.Forms
{
    [SupportedOSPlatform("windows")]
    public partial class MainForm : UIForm
    {
        private static MainForm instance;
        private readonly IConfiguration configuration;
        private readonly ISecsConnection secsConnection;
        private readonly CommonLibrary commonLibrary;
        private readonly ISecsGem secsGem;
        private readonly RabbitMq.RabbitMqService rabbitMqservice;


        private readonly JhtHanderService jhtHanderService;
        internal static ILog traLog = LogManager.GetLogger("Trace");
        internal static ILog dbgLog = LogManager.GetLogger("Debug");
        int inputTrayCount = 0;
        int outputTrayCount = 0;
        bool agvEnabled = false;
        bool agvLocked = false;
        string currentTaskState = "无AGV任务";
        string currentLot = string.Empty;
        string materialName = string.Empty;
        string groupName = string.Empty;
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

        public string CurrentLot
        {
            get { return currentLot; }
            set
            {
                currentLot = value;
                this.Invoke(new Action(() =>
                {
                    uiTextBox_currentLot.Text = currentLot;
                }));
            }
        }

        public string MaterialName
        {
            get { return materialName; }
            set
            {
                materialName = value;
                this.Invoke(new Action(() =>
                {
                    uiTextBox_materialName.Text = materialName;
                }));
            }
        }

        public string GroupName
        {
            get { return groupName; }
            set
            {
                groupName = value;
                this.Invoke(new Action(() =>
                {
                    uiTextBox_groupName.Text = groupName;
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

            jhtHanderService = new JhtHanderService(rabbitMqservice, configuration);
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

        //public bool ConfirmMessageBox(string showtext)
        //{
        //    var confirm = UIMessageBox.ShowAsk2($"{showtext}");
        //    return confirm;
        //}

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
            jhtHanderService.UpdateMachineIP(); //更新机器IP

            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };//定时去检测更新根据自己业务需求
            timer.Tick += delegate
            {
                AutoUpdater.Start(updateUrl);
                jhtHanderService.UpdateMachineIP(); //更新机器IP
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
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"],
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
                        CurrentLot = info.Parameters.ContainsKey("CurrentLot") ? info.Parameters["CurrentLot"]?.ToString() : string.Empty;
                        MaterialName = info.Parameters.ContainsKey("MaterialName") ? info.Parameters["MaterialName"]?.ToString() : string.Empty;
                        GroupName = info.Parameters.ContainsKey("GroupName") ? info.Parameters["GroupName"]?.ToString() : string.Empty;
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
                        bool updateResult = await Task.Run(() => jhtHanderService.UpdateMachineInputTrayCount(newCount));

                        if (updateResult)
                        {
                            InputTrayCount = newCount;
                            traLog.Info($"入料口盘数更新成功:{InputTrayCount}");
                        }
                        else
                        {
                            traLog.Warn("入料口盘数更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        traLog.Error($"更新入料口盘数时发生异常: {ex.Message}");
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
                        bool updateResult = await Task.Run(() => jhtHanderService.UpdateMachineOutputTrayCount(newCount));

                        if (updateResult)
                        {
                            OutputTrayCount = newCount;
                            traLog.Info($"出料口盘数更新成功:{OutputTrayCount}");
                        }
                        else
                        {
                            traLog.Warn("出料口盘数更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        traLog.Error($"更新出料口盘数时发生异常: {ex.Message}");
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

                if (newState && (InputTrayCount < 0 || OutputTrayCount > 17))
                {
                    UIMessageBox.ShowWarning("入料口盘数不能小于0，出料口盘数不能大于17，请检查盘数设置。");
                    return;
                }

                string confirmMsg = newState
                    ? $"确定要开启AGV模式吗？当前入料口盘数{InputTrayCount}，出料口盘数{OutputTrayCount}，请确认盘数正确。"
                    : "确定要关闭AGV模式吗？";

                if (UIMessageBox.ShowAsk2($"{confirmMsg}"))
                {
                    bool updateResult = await Task.Run(() => jhtHanderService.UpdateAgvEnabled(newState));
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
                if (UIMessageBox.ShowAsk2(confirmMsg))
                {
                    var (result, message) = await Task.Run(() => jhtHanderService.SemdAgvTask("Input"));
                    if (result)
                    {
                        traLog.Info($"Input任务发送成功: {message}");
                    }
                    else
                    {
                        traLog.Error($"Input任务发送失败: {message}");
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
                if (UIMessageBox.ShowAsk2(confirmMsg))
                {
                    var (result, message) = await Task.Run(() => jhtHanderService.SemdAgvTask("Output"));
                    if (result)
                    {
                        traLog.Info($"Output任务发送成功: {message}");
                    }
                    else
                    {
                        traLog.Error($"Output任务发送失败: {message}");
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
                if (UIMessageBox.ShowAsk2(confirmMsg))
                {
                    var (result, message) = await Task.Run(() => jhtHanderService.SemdAgvTask("InputOutput"));
                    if (result)
                    {
                        traLog.Info($"InputOutput任务发送成功: {message}");
                    }
                    else
                    {
                        traLog.Error($"InputOutput任务发送失败: {message}");
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

        private async void uiButton_lockAgv_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            uiButton_lockAgv.Enabled = false;
            await Task.Run(async () =>
            {
                try
                {
                    var lockStateSvid = commonLibrary.GetGemSvid("LockState").ID;

                    var s1f3 = new SecsMessage(1, 3)
                    {
                        SecsItem = L(U4((uint)lockStateSvid))
                    };
                    var s1f4 = await secsGem.SendAsync(s1f3);

                    AgvLocked = s1f4.SecsItem[0].GetString().ToUpper() == "TRUE";
                    if (AgvLocked)
                    {
                        traLog.Warn($"已处于锁定状态，请先解锁");
                    }
                    else
                    {
                        var s2f41 = new SecsMessage(2, 41)
                        {
                            SecsItem = L(A("LOCKAGV"), L())
                        };
                        var s2f42 = await secsGem.SendAsync(s2f41);
                        var rcmdAck = s2f42.SecsItem[0].FirstValue<byte>() == 0;
                        if (rcmdAck)
                        {
                            AgvLocked = true;
                            traLog.Info("AGV锁定成功");
                        }
                        else
                        {
                            traLog.Error($"AGV锁定失败, 错误代码: {s2f42.SecsItem.FirstValue<byte>()}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    traLog.Error($"AGV锁定操作失败: {ex.ToString()}");
                }
                finally
                {
                    this.Invoke(new Action(() =>
                    {
                        Cursor = Cursors.Default;
                        uiButton_lockAgv.Enabled = true;
                    }));
                }

            });
        }

        private async void uiButton_unlockAgv_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            uiButton_unlockAgv.Enabled = false;
            await Task.Run(async () =>
            {
                try
                {
                    var lockStateSvid = commonLibrary.GetGemSvid("LockState").ID;

                    // 先查询当前锁定状态
                    var s1f3 = new SecsMessage(1, 3)
                    {
                        SecsItem = L(U4((uint)lockStateSvid))
                    };
                    var s1f4 = await secsGem.SendAsync(s1f3);

                    AgvLocked = s1f4.SecsItem[0].GetString().ToUpper() == "TRUE";
                    if (!AgvLocked)
                    {
                        traLog.Warn($"已处于解锁状态，无需重复解锁");
                    }
                    else
                    {
                        // 发送解锁命令
                        var s2f41 = new SecsMessage(2, 41)
                        {
                            SecsItem = L(A("LOCKAGV_OFF"), L())
                        };
                        var s2f42 = await secsGem.SendAsync(s2f41);
                        var rcmdAck = s2f42.SecsItem[0].FirstValue<byte>() == 0;
                        if (rcmdAck)
                        {
                            AgvLocked = false;
                            traLog.Info("AGV解锁成功");
                        }
                        else
                        {
                            traLog.Error($"AGV解锁失败, 错误代码: {s2f42.SecsItem.FirstValue<byte>()}");
                        }
                    }

                }
                catch (Exception ex)
                {
                    traLog.Error($"AGV解锁操作失败: {ex.ToString()}");
                }
                finally
                {
                    this.Invoke(new Action(() =>
                    {
                        Cursor = Cursors.Default;
                        uiButton_unlockAgv.Enabled = true;
                    }));
                }

            });
        }

        private async void uiSymbolButton_updateLot_Click(object sender, EventArgs e)
        {
            EditValueForm editValueForm = new EditValueForm("修改当前Lot", CurrentLot);
            if (editValueForm.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(editValueForm.Value))
                {
                    Cursor = Cursors.WaitCursor;
                    uiSymbolButton_updateLot.Enabled = false;
                    try
                    {
                        // 在后台线程执行更新Lot号操作
                        bool updateResult = await Task.Run(() => jhtHanderService.UpdateLot(editValueForm.Value.Trim()));
                        if (updateResult)
                        {
                            CurrentLot = editValueForm.Value.Trim();
                            traLog.Info($"Lot号更新成功: {CurrentLot}");
                        }
                        else
                        {
                            traLog.Warn("Lot号更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        traLog.Error($"更新Lot号时发生异常: {ex.Message}");
                    }
                    finally
                    {
                        // 恢复界面状态
                        Cursor = Cursors.Default;
                        uiSymbolButton_updateLot.Enabled = true;
                    }

                }
                else
                {
                    UIMessageBox.ShowWarning("请输入有效的Lot号");
                }
            }
        }

        private async void uiSymbolButton_updateMaterialName_Click(object sender, EventArgs e)
        {
            // 创建编辑表单，标题为"修改物料名称"，初始值为当前物料名称
            EditValueForm editValueForm = new EditValueForm("修改物料名称（UPN前11码）", MaterialName);
            if (editValueForm.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(editValueForm.Value))
                {
                    // 显示等待光标，禁用按钮防止重复操作
                    Cursor = Cursors.WaitCursor;
                    uiSymbolButton_updateMaterialName.Enabled = false;
                    try
                    {
                        // 在后台线程执行更新物料名称操作
                        bool updateResult = await Task.Run(() =>
                            jhtHanderService.UpdateMaterialName(editValueForm.Value.Trim()));

                        if (updateResult)
                        {
                            MaterialName = editValueForm.Value.Trim();
                            traLog.Info($"物料名称更新成功: {MaterialName}");
                        }
                        else
                        {
                            traLog.Warn("物料名称更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        traLog.Error($"更新物料名称时发生异常: {ex.Message}");
                    }
                    finally
                    {
                        // 恢复界面状态
                        Cursor = Cursors.Default;
                        uiSymbolButton_updateMaterialName.Enabled = true;
                    }
                }
                else
                {
                    UIMessageBox.ShowWarning("请输入有效的物料名称");
                }
            }
        }

        private async void uiSymbolButton_updateGroupName_Click(object sender, EventArgs e)
        {
            // 创建编辑表单，标题为"修改站别名称"，初始值为当前站别名称
            EditValueForm editValueForm = new EditValueForm("修改站别名称", GroupName);
            if (editValueForm.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(editValueForm.Value))
                {
                    // 显示等待光标，禁用按钮防止重复操作
                    Cursor = Cursors.WaitCursor;
                    uiSymbolButton_updateGroupName.Enabled = false;
                    try
                    {
                        // 在后台线程执行更新站别名称操作
                        bool updateResult = await Task.Run(() =>
                            jhtHanderService.UpdateGroupName(editValueForm.Value.Trim()));

                        if (updateResult)
                        {
                            GroupName = editValueForm.Value.Trim();
                            traLog.Info($"站别名称更新成功: {GroupName}");
                        }
                        else
                        {
                            traLog.Warn("站别名称更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        traLog.Error($"更新站别名称时发生异常: {ex.Message}");
                    }
                    finally
                    {
                        // 恢复界面状态
                        Cursor = Cursors.Default;
                        uiSymbolButton_updateGroupName.Enabled = true;
                    }
                }
                else
                {
                    UIMessageBox.ShowWarning("请输入有效的站别名称");
                }
            }
        }

        private async void uiButton_refresh_Click(object sender, EventArgs e)
        {
            await Task.Run(() => GetMachineInfo());
        }
    }
}
