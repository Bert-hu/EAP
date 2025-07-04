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
using EAP.Client.Models;
using System.Threading.Tasks;
using System.Windows.Ink;
using Newtonsoft.Json.Linq;
using System.Windows.Shapes;
using System.Configuration;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

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
        ConfigManager<MoldingConfig> manager = new ConfigManager<MoldingConfig>();

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


        private System.Windows.Forms.Timer adminTimer = new System.Windows.Forms.Timer { Interval = 120000 }; // 2分钟
        public void RefreshTimer()
        {
            adminTimer.Stop();
            adminTimer.Start(); // 重置倒计时
        }

        bool _isAdmin;
        bool isAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;
                this.Invoke(new Action(() =>
                {
                    uiLabel_admin.Text = value ? "管理员" : "普通用户";
                    uiLabel_admin.BackColor = value ? System.Drawing.Color.Green : Color.Transparent;
                    if (value)
                    {
                        uiButton_login.Text = "登出";
                        uiTextBox_modelName.ReadOnly = false;
                        uiTextBox_line.ReadOnly = false;
                        checkBox_checkrecipe.ReadOnly = false;
                        uiCheckBox_checkRecipeBody.ReadOnly = false;
                        adminTimer.Start();
                    }
                    else
                    {
                        adminTimer.Stop();
                        traLog.Info("用户登出");
                        uiButton_login.Text = "登录";
                        uiTextBox_modelName.ReadOnly = true;
                        uiTextBox_line.ReadOnly = true;
                        checkBox_checkrecipe.ReadOnly = true;
                        uiCheckBox_checkRecipeBody.ReadOnly = true;
                    }
                }));
            }
        }


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

            adminTimer.Tick += (sender, e) =>
            {
                isAdmin = false;
            };
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
            this.Invoke(new Action(() =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                this.Text = _commonLibrary.CustomSettings["EquipmentId"] + " " + _commonLibrary.CustomSettings["EquipmentType"] + " Version: " + assembly.GetName().Version.ToString();
                notifyIcon.Text = _commonLibrary.CustomSettings["EquipmentType"] + " " + _commonLibrary.CustomSettings["EquipmentId"];
                label_conn_status.Text = showtext;
                label_conn_status.BackColor = backcolor;
            }));

            string sfisIp = _commonLibrary.CustomSettings["SfisIp"];
            int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);
            //BaymaxService baymax = new BaymaxService();
            //baymax.OnBaymaxTransCompleted += Baymax_OnBaymaxTrans;
            //baymax.StartBaymaxForwardingService(_secsConnection.IpAddress.ToString(), 21347, sfisIp, sfisPort, HandleBaymaxResponse);

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

            var config = manager.LoadConfig();
            uiTextBox_empNo.Text = config.EmpNo;
            uiTextBox_line.Text = config.Line;
            uiTextBox_modelName.Text = config.ModelName;
            uiTextBox_groupName.Text = config.GroupName;
            uiTextBox_reelId.Text = config.ReelId;
            checkBox_checkrecipe.Checked = config.AutoCheckRecipeName;
            uiCheckBox_checkRecipeBody.Checked = config.AutoCheckRecipeBody;
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

        public class CompareRecipeBodyResponse
        {
            public bool Result { get; set; } = false;
            public string Message { get; set; }

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

        //public bool isAutoCheckRecipeName { get; set; } = true;
        private void checkBox_checkrecipe_CheckedChanged(object sender, EventArgs e)
        {
            var config = manager.LoadConfig();
            config.AutoCheckRecipeName = checkBox_checkrecipe.Checked;
            manager.SaveConfig(config);
        }
        //public bool isAutoCheckRecipeBody { get; set; } = true;

        private void uiCheckBox_checkRecipeBody_CheckedChanged(object sender, EventArgs e)
        {           
            var config = manager.LoadConfig();
            config.AutoCheckRecipeBody = uiCheckBox_checkRecipeBody.Checked;
            manager.SaveConfig(config);

        }
        private void button_CompareRecipe_Click(object sender, EventArgs e)
        {
            button_CompareRecipe.Enabled = false;
            Task.Run(() =>
            {

                try
                {

                    var recipeName = textBox_machinerecipe.Text;

                    string comparemsg = string.Empty;
                    if (!string.IsNullOrEmpty(recipeName))
                    {
                        string equipmentId = _commonLibrary.CustomSettings["EquipmentId"];

                        var rabbitTrans = new RabbitMqTransaction()
                        {
                            TransactionName = "CompareRecipeBody",
                            EquipmentID = equipmentId,
                            Parameters = new Dictionary<string, object>()
                                        {
                                            {"EquipmentId",equipmentId},
                                            {"RecipeName",recipeName},
                                        },
                        };
                        var repTrans = _rabbitMq.ProduceWaitReply("Rms.Service", rabbitTrans);
                        if (repTrans != null)
                        {
                            var result = false;
                            var message = string.Empty;
                            if (repTrans.Parameters.TryGetValue("Result", out object _result)) result = (bool)_result;
                            if (repTrans.Parameters.TryGetValue("Message", out object _message)) message = _message?.ToString();
                            if (!result)
                            {
                                comparemsg = "Compare recipe fail: " + message;
                                traLog.Error(comparemsg);
                            }
                            else
                            {
                                traLog.Info("Compare recipe success!");
                            }
                        }
                        else
                        {
                            comparemsg = "Compare recipe fail: Timeout";
                            traLog.Error(comparemsg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    traLog.Error(ex);
                }


                this.Invoke(() =>
                {
                    button_CompareRecipe.Enabled = true;
                });
            });


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
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void uiButton_login_Click(object sender, EventArgs e)
        {
            if (!isAdmin)
            {
                //PasswordForm form = new PasswordForm();
                //if (form.ShowDialog() == DialogResult.OK)
                //{
                //    if (form.Value == _commonLibrary.CustomSettings["Password"])
                //    {
                //        isAdmin = true;
                //    }
                //    else
                //    {
                //        UIMessageBox.ShowError("密码错误");
                //    }
                //}


                LoginForm loginForm = new LoginForm();
                loginForm.ShowDialog();
                if (loginForm.IsLogin)
                {
                    UIMessageTip.ShowOk("登录成功");
                    traLog.Info($"用户{loginForm.UserName}登入");
                    isAdmin = true;
                }
                loginForm.Dispose();

            }
            else
            {
                isAdmin = false;
            }
        }




        private void uiTextBox_empNo_TextChanged(object sender, EventArgs e)
        {
            var config = manager.LoadConfig();
            config.EmpNo = uiTextBox_empNo.Text;
            manager.SaveConfig(config);
        }

        private void uiTextBox_line_TextChanged(object sender, EventArgs e)
        {
            var config = manager.LoadConfig();
            config.Line = uiTextBox_line.Text;
            manager.SaveConfig(config);
        }

        private void uiTextBox_sn_TextChanged(object sender, EventArgs e)
        {

        }

        private void uiTextBox_reelId_TextChanged(object sender, EventArgs e)
        {
            var config = manager.LoadConfig();
            config.ReelId = uiTextBox_reelId.Text;
            manager.SaveConfig(config);
        }


        private void uiTextBox_modelName_TextChanged(object sender, EventArgs e)
        {
            var config = manager.LoadConfig();
            config.ModelName = uiTextBox_modelName.Text;
            manager.SaveConfig(config);
        }

        private void uiTextBox_groupName_TextChanged(object sender, EventArgs e)
        {
            var config = manager.LoadConfig();
            config.GroupName = uiTextBox_groupName.Text;
            manager.SaveConfig(config);
        }

        private async void uiButton_downloadRecipe_Click(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            ScanBarcodeForm form = new ScanBarcodeForm();
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                (string recipeName, string modelName, string errMsg) = await GetRecipeNameBySn(form.Value);
                if (string.IsNullOrEmpty(recipeName))
                {
                    traLog.Warn($"Fail, Check Model Name Fail: {errMsg}");
                    return;
                }

                if (UIMessageBox.ShowAsk($"Download Recipe: {recipeName} to Machine?"))
                {
                    var (downloadresult, message) = DownloadRecipeToMachine(recipeName);
                    if (downloadresult)
                    {
                        traLog.Info($"Download success. Barcode: {form.Value}, Recipe: {message}");
                    }
                    else
                    {
                        traLog.Error($"Download fail. Barcode: {form.Value}, Message: {message}");
                    }
                }
            }
        }



        private async Task<(string recipeName, string modelname, string errMsg)> GetRecipeNameBySn(string panelid)
        {
            var site = _configuration.GetSection("Custom")["Site"] ?? "HPH";
            var sfisIp = _configuration.GetSection("Custom")["SfisIp"];
            var sfisPort = int.Parse(_configuration.GetSection("Custom")["SfisPort"] ?? "21347");
            var rmsUrl = _configuration.GetSection("Custom")["RmsApiUrl"];
            var equipmentId = _configuration.GetSection("Custom")["EquipmentId"];
            var getModelnameReq = string.Empty;
            if (site == "JQ")
            {
                getModelnameReq = $"SMD_SPC_QUERY,{panelid},7,M090696,JQ01-3FAP-12,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";//JQ
            }
            else
            {
                getModelnameReq = $"EQXXXXXX01,{panelid},7,M001603,V98,,OK,SN_MODEL_NAME_INFO=???";//HPH
            }

            //var getModelnameRes = string.Empty;
            //var getModelnameErr = string.Empty;
            string recipeName = null;
            string modelname = null;
            string errMsg = null;

            BaymaxService baymax = new BaymaxService();
            var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelnameReq);

            if (trans.Result)
            {
                if (trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {
                    Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                  .Where(keyValueArray => keyValueArray.Length == 2)
                                  .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);

                    if (site == "JQ")
                    {
                        //JQ
                        modelname = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                        string projectName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];
                        string groupName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[2];
                    }
                    else
                    {
                        //HPH
                        modelname = sfisParameters["SN_MODEL_NAME_INFO"];
                    }
                    uiTextBox_modelName.Text = modelname;


                    (recipeName, errMsg) = CheckRecipeGroup(rmsUrl, equipmentId, modelname);
                    return (recipeName, modelname, errMsg);
                }
                else
                {
                    return (recipeName, modelname, "SFIS Fail: " + trans.BaymaxResponse);
                }
            }
            else
            {
                return (recipeName, modelname, trans.BaymaxResponse);
            }
        }

        private (string recipeName, string errMsg) CheckRecipeGroup(string rmsUrl, string equipmentId, string recipeGroupName)
        {
            string recipeName = null;
            string errMsg = string.Empty;

            try
            {
                string url = rmsUrl.TrimEnd('/') + "/api/checkrecipegroup";
                var req = new
                {
                    EquipmentId = equipmentId,
                    RecipeGroupName = recipeGroupName,
                    CheckLastRecipe = false //不检查最后一次使用的Recipe
                };
                var reqstr = JsonConvert.SerializeObject(req);
                using (var httpClient = new HttpClient())
                {
                    var httpResponse = httpClient.PostAsync(url, new StringContent(reqstr, Encoding.UTF8, "application/json")).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                        var repobj = JObject.Parse(responseContent);
                        if ((bool)repobj["Result"])
                        {
                            recipeName = repobj["RecipeName"].ToString();
                            return (recipeName, errMsg);
                        }
                        else
                        {
                            errMsg = repobj["Message"].ToString();
                        }
                    }
                    else
                    {
                        errMsg = $"HTTP请求失败，状态码：{httpResponse.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = $"发生异常：{ex.Message}";
            }

            return (recipeName, errMsg);
        }

        private (bool result, string message) DownloadRecipeToMachine(string recipeName)
        {
            try
            {
                var rmsUrl = _configuration.GetSection("Custom")["RmsApiUrl"];
                var equipmentId = _configuration.GetSection("Custom")["EquipmentId"];

                string url = rmsUrl.TrimEnd('/') + "/api/downloadeffectiverecipetomachine";
                var reqstr = JsonConvert.SerializeObject(new { TrueName = uiTextBox_empNo.Text, EquipmentId = equipmentId, RecipeName = recipeName });
                using (var httpClient = new HttpClient())
                {
                    var httpResponse = httpClient.PostAsync(url, new StringContent(reqstr, Encoding.UTF8, "application/json")).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                        var reply = JsonConvert.DeserializeObject<DownloadEffectiveRecipeToMachineResponse>(responseContent);
                        return (reply.Result, reply.Result ? reply.RecipeName : reply.Message);
                    }
                    else
                    {
                        return (false, $"EAP Error: HTTP request fail：{httpResponse.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"EAP Error: HTTP request fail：{ex.Message}");
            }

        }

        private async void uiSymbolButton_changeReel_Click(object sender, EventArgs e)
        {
            ScanBarcodeForm form = new ScanBarcodeForm(uiTextBox_reelId.Text);
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (form.Value == uiTextBox_reelId.Text)
                {
                    traLog.Error("NEW REEL_ID = OLD REEL_ID, Please scan new REEL_ID  again.");
                }
                // ask sfis
                var sfisresult = false;

                var sfisIp = _configuration.GetSection("Custom")["SfisIp"];
                var sfisPort = int.Parse(_configuration.GetSection("Custom")["SfisPort"] ?? "21347");
                var rmsUrl = _configuration.GetSection("Custom")["RmsApiUrl"];
                var equipmentId = _configuration.GetSection("Custom")["EquipmentId"];


                var checkreel = $"{equipmentId},{form.Value},1,{uiTextBox_empNo.Text},{uiTextBox_line.Text},,OK,,,ACTUAL_GROUP={uiTextBox_groupName.Text},,,,,,{uiTextBox_modelName.Text},{form.Value}";

                BaymaxService baymax = new BaymaxService();
                var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, checkreel);
                if (trans.Result)
                {
                    if (trans.BaymaxResponse.StartsWith("OK"))
                    {
                        uiTextBox_reelId.Text = form.Value;

                        traLog.Info("OK, open the tank.");

                        Task.Run(() => ControlTank(true));
                        return;
                    }
                    else
                    {
                        traLog.Error("Check Reel ID Fail, do not open the tank.");
                    }
                }
                else
                {
                    traLog.Error(trans.BaymaxResponse);
                }
                // Task.Run(() => ControlTank(false));
            }

        }


        [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
        private static extern int Input(int address);
        [DllImport("inpoutx64.dll")]
        private static extern void Out32(int address, int value);

        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Out32_x64(int address, int value);
        private const short LPT1_Port = 0x378;
        private const byte LPT_Pin7 = 0x80;
        //private const byte LPT_Pin7 = 0x00;

        private async Task ControlTank(bool openorclose)
        {
            try
            {
                var readpin = Input(LPT1_Port);
                traLog.Debug("Read value:" + Convert.ToString(readpin, 2));
                //var writedata = int.Parse(Math.Pow(2, variables.PinNum).ToString());
                var writedata = int.Parse(Math.Pow(2, 5).ToString());
                // var writedata = int.Parse(Math.Pow(2,int.Parse( textBox1.Text)).ToString());
                // 写入data 6
                Out32(LPT1_Port, writedata);
                traLog.Debug("Write value:" + Convert.ToString(writedata, 2));
                readpin = Input(LPT1_Port);
                traLog.Debug("Read value:" + Convert.ToString(readpin, 2));
                Thread.Sleep(300);
                //Thread.Sleep(int.Parse(textBox2.Text));
                // 写入data 0
                writedata = 0;//拉低
                traLog.Debug("Write value:" + Convert.ToString(writedata, 2));
                Out32(LPT1_Port, writedata);

                readpin = Input(LPT1_Port);
                traLog.Debug("Read value:" + Convert.ToString(readpin, 2));
                await Task.Delay(3000);
            }
            catch (Exception ex)

            {
                traLog.Error(ex);
            }
            finally
            {
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!IsRunAsAdmin()) { traLog.Error($"Do not run as administrator, program can not control tank."); }
        }

        bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            var isadmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            string compatLayer = Environment.GetEnvironmentVariable("__COMPAT_LAYER");
            bool isRUNASINVOKER = false;
            if (compatLayer != null && compatLayer.ToUpper().Equals("RUNASINVOKER"))
            {
                isRUNASINVOKER = true;
            }
            traLog.Debug("Current Program Role: " + compatLayer?.ToString());
            return isadmin || isRUNASINVOKER;
        }
    }
}
