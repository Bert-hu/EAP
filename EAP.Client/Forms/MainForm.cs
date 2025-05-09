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
using System.Reflection;
using System.Windows.Threading;
using static Secs4Net.Item;

namespace EAP.Client.Forms
{
    public partial class MainForm : UIForm
    {
        private static MainForm instance;
        private readonly IConfiguration _configuration;
        private readonly ISecsConnection _secsConnection;
        private readonly CommonLibrary _commonLibrary;
        private readonly ISecsGem _secsGem;
        private readonly RabbitMq.RabbitMqService _rabbitMq;
        internal static ILog traLog = LogManager.GetLogger("Trace");
        internal static ILog dbgLog = LogManager.GetLogger("Debug");

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
        //public MainForm()
        //{
        //    InitializeComponent();
        //}


        public MainForm(IConfiguration configuration, ISecsConnection secsConnection, CommonLibrary commonLibrary, ISecsGem secsGem, RabbitMq.RabbitMqService rabbitMq)
        {
            _configuration = configuration;
            _secsConnection = secsConnection;
            _commonLibrary = commonLibrary;
            _secsGem = secsGem;
            _rabbitMq = rabbitMq;
            instance = this;
            InitializeComponent();

            _secsConnection.ConnectionChanged += _secsConnection_ConnectionChanged;
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
            DialogResult dr = MessageBox.Show($"{showtext}", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string showtext = "Connecting";
            var backcolor = Color.Gray;
            switch (_secsConnection?.State)
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
            var equipmentId = _commonLibrary.CustomSettings["EquipmentId"];
            this.Invoke(new Action(() =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                this.Text = equipmentId + " " + _commonLibrary.CustomSettings["EquipmentType"] + " Version: " + assembly.GetName().Version.ToString();
                notifyIcon.Text = _commonLibrary.CustomSettings["EquipmentType"] + " " + _commonLibrary.CustomSettings["EquipmentId"];
                label_conn_status.Text = showtext;
                label_conn_status.BackColor = backcolor;
            }));

            string sfisIp = _commonLibrary.CustomSettings["SfisIp"];
            int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);
            BaymaxService baymax = new BaymaxService();
            baymax.OnBaymaxTransCompleted += Baymax_OnBaymaxTrans;
            baymax.StartBaymaxForwardingService(_secsConnection.IpAddress.ToString(), 21347, sfisIp, sfisPort, HandleBaymaxResponse);

            var updateUrl = _configuration.GetSection("Custom")["UpdateUrl"].TrimEnd('/') + "/" + _commonLibrary.CustomSettings["EquipmentType"] + "/AutoUpdate.xml";
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("zh");
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 1;
            AutoUpdater.ReportErrors = true;

            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;

            AutoUpdater.Start(updateUrl);

            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };//定时去检测更新根据自己业务需求
            timer.Tick += delegate
            {
                AutoUpdater.Start(updateUrl);
            };
            timer.Start();

            DispatcherTimer locktimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };//定时去检测更新根据自己业务需求
            locktimer.Tick += delegate
            {

                try
                {
                    var trans = new RabbitMqTransaction()
                    {
                        TransactionName = "GetAllConfiguration",
                        EquipmentID = equipmentId,
                        NeedReply = true,
                        ReplyChannel = $"EAP.SecsClient.{equipmentId}",
                    };
                    var repTrans = _rabbitMq.ProduceWaitReply("EAP.Services", trans);
                    var message = string.Empty;
                    if (repTrans.Parameters.TryGetValue("Message", out object _message)) message = _message?.ToString();

                    string isheld = string.Empty;
                    if (repTrans.Parameters.TryGetValue("IsHeld", out object _isheld)) isheld = _isheld?.ToString();
                    MainForm.Instance.UpdateMachineLock(isheld.ToUpper() == "TRUE", message);

                    if (MainForm.Instance.machineLocked)
                    {
                        var s2f41 = new SecsMessage(2, 41)
                        {
                            SecsItem = L(
                         A("STOP"),
                         L()
                   )
                        };
                        _secsGem.SendAsync(s2f41);
                    }
                }
                catch (Exception ex)
                {
                    dbgLog.Error(ex.ToString());
                }
            };
            locktimer.Start();

        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    var eqpType = _commonLibrary.CustomSettings["EquipmentType"];
                    this.Text = _commonLibrary.CustomSettings["EquipmentId"] + " " + _commonLibrary.CustomSettings["EquipmentType"] + " Version: " + args.InstalledVersion + " 需要更新";
                    bool dialogResult =
                            UIMessageBox.ShowAsk2(
                                $@"新版本 {eqpType + ":" + args.CurrentVersion} 可用. 当前版本 {eqpType + ":" + args.InstalledVersion}. 如果设备空闲请点击确认更新并重启，否则点击取消");


                    // Uncomment the following line if you want to show standard update dialog instead.
                    //AutoUpdater.ShowUpdateForm(args);

                    if (dialogResult)
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
                    this.Text = _commonLibrary.CustomSettings["EquipmentId"] + " " + _commonLibrary.CustomSettings["EquipmentType"] + " Version: " + args.InstalledVersion + " 最新版本";
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    //UIMessageBox.ShowError(@"There is a problem reaching update server. Please check your internet connection and try again later.");
                    this.Text = _configuration.GetSection("Custom")["EquipmentId"] + " " + _configuration.GetSection("Custom")["EquipmentType"];
                }
                else
                {
                    UIMessageBox.ShowError(args.Error.Message);
                }
            }
        }



        private void Baymax_OnBaymaxTrans(object? sender, BaymaxService.BaymaxTrans e)
        {

        }
        public class GetRecipeNameAliasResponse
        {
            public bool Result { get; set; } = false;
            public string Message { get; set; }
            public string Id { get; set; }
            public string EquipmentTypeId { get; set; }
            public string RecipeName { get; set; }
            public List<string> RecipeAlias { get; set; }
        }

        public class GetRecipeNameResponse
        {
            public bool Result { get; set; } = false;
            public string Message { get; set; }
            public string Id { get; set; }
            public string EquipmentTypeId { get; set; }
            public string RecipeName { get; set; }
        }
        private string HandleBaymaxResponse(BaymaxService sender, string machineRequest, string baymaxResponse)
        {
            try
            {
                var stepid = machineRequest.Split(',')[2];
                var panelsn = machineRequest.Split(',')[1];

                if (stepid == "1" && baymaxResponse.ToUpper().StartsWith("OK"))
                {
                    Dictionary<string, string> sfisParameters = baymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                 .Where(keyValueArray => keyValueArray.Length == 2)
                                 .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                    var groupRecord = sfisParameters["GROUP_RECORD"].Trim();

                    switch (groupRecord)
                    {
                        case "MIX-ICOS":
                            if (icosCount >= icosMaxCount)
                            {
                                return $"FAIL1,MIX-ICOS ≥ {icosMaxCount}";
                            }
                            else
                            {
                                icosCount++;
                            }
                            break;
                        case "MIX-M":
                            if (mCount >= mMaxCount)
                            {
                                return $"FAIL1,MIX-M ≥ {mMaxCount}";
                            }
                            else
                            {
                                mCount++;
                            }
                            break;
                        case "MIX-OH":
                            if (ohCount >= ohMaxCount)
                            {
                                return $"FAIL1,MIX-OH ≥ {ohMaxCount}";
                            }
                            else
                            {
                                ohCount++;
                            }
                            break;
                        default:
                            break;
                    }
                    return baymaxResponse;
                }
                else
                {
                    return baymaxResponse;
                }
            }
            catch (Exception ex)
            {
                dbgLog.Error(ex.ToString());
                return baymaxResponse;
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


        public class DownloadEffectiveRecipeToMachineResponse
        {
            public bool Result { get; set; } = false;
            public string Message { get; set; }
            public string RecipeName { get; set; }
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

        public bool machineLocked { get; set; } = false;
        public string lockMessage { get; set; } = string.Empty;
        private void uiCheckBox_isLocked_CheckedChanged(object sender, EventArgs e)
        {
            //if (uiCheckBox_isLocked.Checked == false)
            //{
            //    PasswordForm form = new PasswordForm();
            //    if (form.ShowDialog() == DialogResult.OK)
            //    {
            //        if (form.Value == _commonLibrary.CustomSettings["Password"])
            //        {
            //            uiCheckBox_isLocked.Checked = false;
            //        }
            //        else
            //        {
            //            UIMessageBox.ShowError("密码错误");
            //            uiCheckBox_isLocked.Checked = true;
            //        }
            //    }
            //    else
            //    {
            //        uiCheckBox_isLocked.Checked = true;
            //    }
            //}
            machineLocked = uiCheckBox_isLocked.Checked;
        }
        public void UpdateMachineLock(bool locked, string message)
        {
            this.Invoke(new Action(() =>
            {
                uiCheckBox_isLocked.Checked = locked;
                if (!string.IsNullOrEmpty(message))
                {
                    uiRichTextBox_lockMessage.Text = message;
                }
            }));

        }

        private void uiButton_modifySetting_Click(object sender, EventArgs e)
        {
            PasswordForm form = new PasswordForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (form.Value == _commonLibrary.CustomSettings["Password"])
                {
                    MixPackageSettingForm settingForm = new MixPackageSettingForm(icosMaxCount, mMaxCount, ohMaxCount);
                    if (settingForm.ShowDialog() == DialogResult.OK)
                    {
                        icosMaxCount = settingForm.icosCount;
                        mMaxCount = settingForm.mCount;
                        ohMaxCount = settingForm.ohCount;
                    }
                }
                else
                {
                    UIMessageBox.ShowError("密码错误");
                }
            }
        }
        private void UpdateControlValue(Control control, int value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() =>
                {
                    control.Text = value.ToString();
                    uiLedLabel_total.Text = (_icosCount + _mCount + _ohCount).ToString();
                    uiLedLabel_totalMax.Text = (_icosMaxCount + _mMaxCount + _ohMaxCount).ToString();
                }));
            }
            else
            {
                control.Text = value.ToString();
                uiLedLabel_total.Text = (_icosCount + _mCount + _ohCount).ToString();
                uiLedLabel_totalMax.Text = (_icosMaxCount + _mMaxCount + _ohMaxCount).ToString();
            }
        }

        private int _icosCount;
        public int icosCount { get { return _icosCount; } set { _icosCount = value; UpdateControlValue(uiLedLabel_icos, value); } }
        private int _mCount;
        public int mCount { get { return _mCount; } set { _mCount = value; UpdateControlValue(uiLedLabel_m, value); } }
        private int _ohCount;
        public int ohCount { get { return _ohCount; } set { _ohCount = value; UpdateControlValue(uiLedLabel_oh, value); } }
        private int _icosMaxCount;
        public int icosMaxCount { get { return _icosMaxCount; } set { _icosMaxCount = value; UpdateControlValue(uiLedLabel_icosMax, value); } }
        private int _mMaxCount;
        public int mMaxCount { get { return _mMaxCount; } set { _mMaxCount = value; UpdateControlValue(uiLedLabel_mMax, value); } }
        private int _ohMaxCount;
        public int ohMaxCount { get { return _ohMaxCount; } set { _ohMaxCount = value; UpdateControlValue(uiLedLabel_ohMax, value); } }

        private void uiButton_clearCount_Click(object sender, EventArgs e)
        {
            var confirm = UIMessageBox.ShowAsk("是否清零?");
            if (confirm)
            {
                ClearCount();
            }
        }

        public void ClearCount()
        {
            traLog.Info($"上次计数值为MIX-ICOS:{_icosCount},MIX-M:{_mCount},MIX-OH:{_ohCount},计数清零");
            icosCount = 0;
            mCount = 0;
            ohCount = 0;
        }
    }
}
