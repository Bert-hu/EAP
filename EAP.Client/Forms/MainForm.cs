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
        ConfigManager<SputtereConfig> manager = new ConfigManager<SputtereConfig>();

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
                        uiButton_allowInput.Visible = true;
                        adminTimer.Start();
                    }
                    else
                    {
                        adminTimer.Stop();
                        traLog.Info("用户登出");
                        uiButton_login.Text = "登录";
                        uiButton_allowInput.Visible = false;
                    }
                }));
            }
        }

        public enum InputStatus
        {
            Wait = 0,
            Allow = 1,
            Reject = 2
        }

        InputStatus _allowInput = InputStatus.Wait;
        public InputStatus AllowInput
        {
            get { return _allowInput; }
            set
            {
                _allowInput = value;
                this.Invoke(new Action(() =>
                {
                    if (_allowInput == InputStatus.Allow)
                    {
                        traLog.Info("允许入料");
                        uiLabel_inputStatus.Text = "允许入料";
                        uiLabel_inputStatus.BackColor = System.Drawing.Color.Green;
                    }
                    else if (_allowInput == InputStatus.Reject)
                    {
                        traLog.Warn("禁止入料");
                        uiLabel_inputStatus.Text = "禁止入料";
                        uiLabel_inputStatus.BackColor = System.Drawing.Color.Red;
                    }
                    else if (_allowInput == InputStatus.Wait)
                    {
                        uiLabel_inputStatus.Text = "等待中";
                        uiLabel_inputStatus.BackColor = System.Drawing.Color.Orange;
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
            uiDataGridView_Material.DataSource = config.CathodeSettings;
            uiDataGridView_Material.Refresh();
            uiTextBox_empNo.Text = config.EmpNo;
            uiTextBox_line.Text = config.Line;
            uiTextBox_modelName.Text = config.ModelName;
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

        public bool isAutoCheckRecipe { get; set; } = true;
        private void checkBox_checkrecipe_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_checkrecipe.Checked == false)
            {
                PasswordForm form = new PasswordForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.Value == _commonLibrary.CustomSettings["Password"])
                    {
                        checkBox_checkrecipe.Checked = false;
                    }
                    else
                    {
                        UIMessageBox.ShowError("密码错误");
                        checkBox_checkrecipe.Checked = true;
                    }
                }
                else
                {
                    checkBox_checkrecipe.Checked = true;
                }
            }
            isAutoCheckRecipe = checkBox_checkrecipe.Checked;
        }

        private void button_CompareRecipe_Click(object sender, EventArgs e)
        {
            button_CompareRecipe.Enabled = false;
            Task.Run(() =>
            {

                try
                {
                    var s1f3 = new SecsMessage(1, 3)
                    {
                        SecsItem = L(U4(650), U4(651))
                    };
                    var s1f4 = _secsGem.SendAsync(s1f3).Result;
                    var recipeName = s1f4.SecsItem.Items[0].GetString();
                    var recipeNum = s1f4.SecsItem.Items[1].GetString();
                    var fullRecipeName = recipeNum + "_" + recipeName;

                    string comparemsg = string.Empty;
                    if (!string.IsNullOrEmpty(fullRecipeName))
                    {
                        this.Invoke(() =>
                        {
                            this.textBox_machinerecipe.Text = fullRecipeName;
                        });
                        string equipmentId = _commonLibrary.CustomSettings["EquipmentId"];

                        var rabbitTrans = new RabbitMqTransaction()
                        {
                            TransactionName = "CompareRecipeBody",
                            EquipmentID = equipmentId,
                            Parameters = new Dictionary<string, object>()
                                        {
                                            {"EquipmentId",equipmentId},
                                            {"RecipeName",fullRecipeName},
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

        private void uiButton_add_Click(object sender, EventArgs e)
        {
            if (isAdmin)
            {
                RefreshTimer();
                SputterCathodeSettingForm form = new SputterCathodeSettingForm();
                DialogResult dr = form.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    //TODO SFIS Step3
                    if (true)
                    {
                        try
                        {
                            var config = manager.LoadConfig();

                            if (config.CathodeSettings.Any(it => it.Seq == form.cathodeSetting.Seq))
                            {
                                this.ShowWarningDialog("Seq已存在");
                            }
                            else
                            {
                                config.CathodeSettings.Add(form.cathodeSetting);
                                manager.SaveConfig(config);
                                uiDataGridView_Material.DataSource = config.CathodeSettings;
                                uiDataGridView_Material.Refresh();
                                traLog.Info($"添加成功{form.cathodeSetting.Seq},{form.cathodeSetting.CathodeId}");
                            }

                        }
                        catch (Exception ex)
                        {
                            this.ShowErrorDialog(ex.Message);
                        }
                    }
                }

            }
            else
            {
                this.ShowErrorDialog("无权限");
            }
        }
        private void uiButton_del_Click(object sender, EventArgs e)
        {
            if (isAdmin)
            {
                RefreshTimer();
                var selectRows = uiDataGridView_Material.SelectedRows;
                if (selectRows.Count == 1)
                {
                    var selectRow = (CathodeSetting)selectRows[0].DataBoundItem;

                    var confirm = this.ShowAskDialog("确定删除吗?");
                    if (confirm)
                    {
                        var config = manager.LoadConfig();

                        try
                        {
                            config.CathodeSettings = config.CathodeSettings.Where(it => it.Seq != selectRow.Seq && it.CathodeId != selectRow.CathodeId).ToList();
                            manager.SaveConfig(config);
                            uiDataGridView_Material.DataSource = config.CathodeSettings;
                            uiDataGridView_Material.Refresh();
                        }
                        catch (Exception ex)
                        {
                            this.ShowErrorDialog(ex.Message);
                        }
                    }
                }
                else
                {
                    this.ShowWarningDialog("请选择一行");
                }
            }
            else
            {
                this.ShowErrorDialog("无权限");
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

        public List<SnInfo> snInfos { get; set; } = new List<SnInfo>();

        private void uiButton_ScanSn_Click(object sender, EventArgs e)
        {
            uiButton_ScanSn.Enabled = false;
            Task.Run(async () =>
            {
                try
                {

                    ScanBarcodeForm form = new ScanBarcodeForm();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var sn = form.Value;
                        var equipmentId = _configuration.GetSection("Custom")["EquipmentId"];
                        var baymaxIp = _configuration.GetSection("Custom")["SfisIp"];
                        var baymaxPort = int.Parse(_configuration.GetSection("Custom")["SfisPort"] ?? "21347");
                        var empNo = uiTextBox_empNo.Text;
                        var step7Req = $"{equipmentId},{sn},7,{empNo},JORDAN,,OK,CARRIER_ID=???";
                        BaymaxService baymaxService = new BaymaxService();
                        var step7Res = await baymaxService.GetBaymaxTrans(baymaxIp, baymaxPort, step7Req);
                        if (step7Res.Result && step7Res.BaymaxResponse.ToUpper().StartsWith("OK"))
                        {
                            Dictionary<string, string> sfisParameters = step7Res.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                     .Where(keyValueArray => keyValueArray.Length == 2)
                     .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                            var CarrierId = sfisParameters["CARRIER_ID"].Trim();
                            if (!snInfos.Any(it => it.CarrierId == CarrierId))
                            {
                                //TODO Get Model Name
                                var getLotGrpInfo = $"EQXXXXXX01,{sn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";
                                var trans = baymaxService.GetBaymaxTrans(baymaxIp, baymaxPort, getLotGrpInfo).Result;
                                if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                                {
                                    Dictionary<string, string> sfisParameters1 = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                              .Where(keyValueArray => keyValueArray.Length == 2)
                                              .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                                    string modelName = sfisParameters1["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                                    string projectName = sfisParameters1["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];
                                    //string GroupName = sfisParameters1["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[2];

                                    var s1f3 = new SecsMessage(1, 3)
                                    {
                                        SecsItem = L(U4(650), U4(651))
                                    };
                                    var s1f4 = _secsGem.SendAsync(s1f3).Result;
                                    var recipeName = s1f4.SecsItem.Items[0].GetString();
                                    var recipeNum = s1f4.SecsItem.Items[1].GetString();
                                    var machineRecipeName = recipeNum + "_" + recipeName;

                                    var rmsUrl = _configuration.GetSection("Custom")["RmsApiUrl"];
                                    var getRecipeNameUrl = rmsUrl.TrimEnd('/') + "/api/GetRecipeName";
                                    var getRecipeNameReq = new
                                    {
                                        EquipmentTypeId = _configuration.GetSection("Custom")["EquipmentType"],
                                        RecipeNameAlias = projectName
                                    };
                                    var getRecipeNameRes = HttpClientHelper.HttpPostRequestAsync<GetRecipeNameResponse>(getRecipeNameUrl, getRecipeNameReq).Result;
                                    if (getRecipeNameRes != null && getRecipeNameRes.Result && !string.IsNullOrEmpty(getRecipeNameRes.RecipeName))
                                    {
                                        if (machineRecipeName == getRecipeNameRes.RecipeName)
                                        {
                                            var compareBodyUrl = rmsUrl.TrimEnd('/') + "/api/CompareRecipeBody";
                                            var compareRecipeBodyReeq = new
                                            {
                                                EquipmentId = equipmentId,
                                                RecipeName = getRecipeNameRes.RecipeName
                                            };
                                            var compareRecipeBodyRes = HttpClientHelper.HttpPostRequestAsync<CompareRecipeBodyResponse>(compareBodyUrl, compareRecipeBodyReeq).Result;
                                            if (compareRecipeBodyRes != null && compareRecipeBodyRes.Result)
                                            {
                                                snInfos.Add(new SnInfo
                                                {
                                                    SN = sn,
                                                    CarrierId = CarrierId
                                                });
                                                AllowInput = InputStatus.Allow;
                                                this.Invoke(() =>
                                                {
                                                    uiDataGridView_snInfo.DataSource = snInfos.ToList();
                                                    uiDataGridView_snInfo.Refresh();

                                                    uiTextBox_modelName.Text = modelName;
                                                });
                                            }
                                            else
                                            {
                                                var message = $"Recipe Body不一致：{compareRecipeBodyRes.Message}";
                                                traLog.Error(message);
                                                UIMessageBox.ShowError2(message);
                                                AllowInput = InputStatus.Reject;
                                            }
                                        }
                                        else
                                        {
                                            var message = $"设备当前Recipe {machineRecipeName}与{projectName}绑定的Recipe {getRecipeNameRes.RecipeName}不匹配";
                                            traLog.Error(message);
                                            UIMessageBox.ShowError2(message);

                                            AllowInput = InputStatus.Reject;
                                        }
                                    }
                                    else
                                    {
                                        var message = $"获取'{projectName}'绑定Recipe失败：{getRecipeNameRes?.Message ?? "网络异常"}";
                                        traLog.Error(message);
                                        UIMessageBox.ShowError2(message);
                                        AllowInput = InputStatus.Reject;
                                    }
                                }
                            }
                            else
                            {
                                var message = $"Carrier ID已存在：{CarrierId}";
                                traLog.Error(message);
                                UIMessageBox.ShowError2(message);
                            }
                        }
                        else
                        {
                            var message = $"获取Carrier ID失败：{step7Res.BaymaxResponse}";
                            traLog.Error(message);
                            UIMessageBox.ShowError2(message);
                        }
                    }

                }
                catch (Exception ex)
                {
                    traLog.Error(ex.ToString());
                }
                finally
                {
                    this.Invoke(() =>
                    {
                        uiButton_ScanSn.Enabled = true;

                    });
                }
            });
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

        private void uiTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void uiTextBox_modelName_TextChanged(object sender, EventArgs e)
        {
            var config = manager.LoadConfig();
            config.ModelName = uiTextBox_modelName.Text;
            manager.SaveConfig(config);
        }

        private void uiButton_update_Click(object sender, EventArgs e)
        {
            uiButton_update.Enabled = false;
            Task.Run(async () =>
            {
                try
                {
                    var config = manager.LoadConfig();

                    List<CathodeSetting> settings = config.CathodeSettings;
                    if (settings.Count > 0)
                    {
                        var equipmentId = _configuration.GetSection("Custom")["EquipmentId"];
                        var empNo = uiTextBox_empNo.Text;
                        var line = uiTextBox_line.Text;
                        var modelName = uiTextBox_modelName.Text;
                        string cathodeStr = string.Join(" ", settings.Select(it => $"CATHODE_{it.Seq}={it.CathodeId}"));
                        var sfiscommand = $"{equipmentId},{settings.First().CathodeId},3,{empNo},{line},,OK,,,{cathodeStr},,,,,,{modelName}";
                        BaymaxService baymaxService = new BaymaxService();
                        var step3Res = await baymaxService.GetBaymaxTrans(_configuration.GetSection("Custom")["SfisIp"], int.Parse(_configuration.GetSection("Custom")["SfisPort"] ?? "21347"), sfiscommand);
                        if (step3Res.Result && step3Res.BaymaxResponse.ToUpper().StartsWith("OK"))
                        {
                            UIMessageBox.ShowInfo("更新成功");
                        }
                        else
                        {
                            UIMessageBox.ShowError2($"更新失败：{step3Res.BaymaxResponse}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    traLog.Error(ex.ToString());
                }
                this.Invoke(() =>
                {
                    uiButton_update.Enabled = true;
                });
            });
        }

        private void uiButton_clearSn_Click(object sender, EventArgs e)
        {
            var confirm = this.ShowAskDialog("确定清空吗?");
            if (confirm)
            {
                ClearInfos();
            }

        }

        public void ClearInfos()
        {
            snInfos.Clear();
            this.Invoke(() =>
            {
                uiDataGridView_snInfo.DataSource = snInfos.ToList();
                uiDataGridView_snInfo.Refresh();
                AllowInput = InputStatus.Wait;
                uiTextBox_trayId.Text = string.Empty;
                uiTextBox_sn.ReadOnly = true;
            });
        }

        private void uiButton_scanTray_Click(object sender, EventArgs e)
        {
            uiButton_scanTray.Enabled = false;
            ScanBarcodeForm form = new ScanBarcodeForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                uiTextBox_trayId.Text = form.Value;
                uiTextBox_sn.ReadOnly = false;
                uiTextBox_sn.Focus();
            }
            uiButton_scanTray.Enabled = true;

        }

        private async void uiTextBox_sn_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    var equipmentId = _configuration.GetSection("Custom")["EquipmentId"];
                    var baymaxIp = _configuration.GetSection("Custom")["SfisIp"];
                    var baymaxPort = int.Parse(_configuration.GetSection("Custom")["SfisPort"] ?? "21347");
                    var empNo = uiTextBox_empNo.Text;
                    var sn = uiTextBox_sn.Text;
                    BaymaxService baymaxService = new BaymaxService();

                    var getLotGrpInfo = $"EQXXXXXX01,{sn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";
                    var trans = await baymaxService.GetBaymaxTrans(baymaxIp, baymaxPort, getLotGrpInfo);
                    if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                    {
                        Dictionary<string, string> sfisParameters1 = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                  .Where(keyValueArray => keyValueArray.Length == 2)
                                  .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                        string modelName = sfisParameters1["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                        string projectName = sfisParameters1["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];


                        var step7Req = $"{equipmentId},{sn},7,{empNo},JORDAN,,OK,CARRIER_ID=???";
                        var step7Res = await baymaxService.GetBaymaxTrans(baymaxIp, baymaxPort, step7Req);
                        if (step7Res.Result && step7Res.BaymaxResponse.ToUpper().StartsWith("OK"))
                        {
                            Dictionary<string, string> sfisParameters = step7Res.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
      .Where(keyValueArray => keyValueArray.Length == 2)
      .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                            var CarrierId = sfisParameters["CARRIER_ID"].Trim();
                            if (!snInfos.Any(it => it.CarrierId == CarrierId))
                            {
                                snInfos.Add(new SnInfo
                                {
                                    SN = sn,
                                    CarrierId = CarrierId,
                                    ModelName = modelName,
                                    ProjectName = projectName,
                                });
                                uiDataGridView_snInfo.DataSource = snInfos.ToList();
                                uiDataGridView_snInfo.Refresh();
                                uiTextBox_modelName.Text = modelName;
                            }
                            else
                            {
                                var message = $"Carrier ID 重复:{CarrierId}";
                                traLog.Warn(message);
                            }
                        }
                        else
                        {
                            var message = $"无法获取绑定Carrier: {sn}";
                            traLog.Warn(message);
                        }
                    }

                    uiTextBox_sn.Text = string.Empty;
                    if (snInfos.Count >= 6)
                    {
                        uiTextBox_sn.ReadOnly = true;

                        uiButton_endScan_Click(sender, e);
                    }
                    else
                    {
                        uiTextBox_sn.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                traLog.Error(ex.ToString());
            }
        }

        private async void uiButton_endScan_Click(object sender, EventArgs e)
        {
            try
            {
                var equipmentId = _configuration.GetSection("Custom")["EquipmentId"];

                if (snInfos.Count == 0)
                {
                    UIMessageBox.ShowError("请先扫描SN");
                    return;
                }

                if (isAutoCheckRecipe)
                {
                    var s1f3 = new SecsMessage(1, 3)
                    {
                        SecsItem = L(U4(650), U4(651))
                    };
                    var s1f4 = await _secsGem.SendAsync(s1f3);
                    var recipeName = s1f4.SecsItem.Items[0].GetString();
                    var recipeNum = s1f4.SecsItem.Items[1].GetString();
                    var machineRecipeName = recipeNum + "_" + recipeName;
                    var projectNameList = snInfos.Select(x => x.ProjectName).Distinct().ToList();
                    var rmsUrl = _configuration.GetSection("Custom")["RmsApiUrl"];
                    var getRecipeNameUrl = rmsUrl.TrimEnd('/') + "/api/GetRecipeName";
                    foreach (var projectName in projectNameList)
                    {
                        var getRecipeNameReq = new
                        {
                            EquipmentTypeId = _configuration.GetSection("Custom")["EquipmentType"],
                            RecipeNameAlias = projectName
                        };
                        var getRecipeNameRes = await HttpClientHelper.HttpPostRequestAsync<GetRecipeNameResponse>(getRecipeNameUrl, getRecipeNameReq);
                        if (getRecipeNameRes != null && getRecipeNameRes.Result && !string.IsNullOrEmpty(getRecipeNameRes.RecipeName))
                        {
                            if (machineRecipeName != getRecipeNameRes.RecipeName)
                            {
                                AllowInput = InputStatus.Reject;
                                var sninfo = snInfos.FirstOrDefault(x => x.ProjectName == projectName);
                                var message = $"该项目找不到匹配的程式，请联系ME。项目名：{projectName},SN:{sninfo.SN},CARRIER_ID:{sninfo.CarrierId},MODEL_NAME:{sninfo.ModelName},PROJECT_NAME:{sninfo.ProjectName}";
                                traLog.Error(message);
                                UIMessageBox.ShowError2(message);
                                return;
                            }
                        }
                        else
                        {
                            AllowInput = InputStatus.Reject;
                            var message = $"{projectName}获取绑定Recipe失败，{getRecipeNameRes.Message}";
                            traLog.Error(message);
                            UIMessageBox.ShowError2(message);
                            return;
                        }
                    }

                    var compareBodyUrl = rmsUrl.TrimEnd('/') + "/api/CompareRecipeBody";
                    var compareRecipeBodyReeq = new
                    {
                        EquipmentId = equipmentId,
                        RecipeName = machineRecipeName
                    };
                    var compareRecipeBodyRes = await HttpClientHelper.HttpPostRequestAsync<CompareRecipeBodyResponse>(compareBodyUrl, compareRecipeBodyReeq);
                    if (compareRecipeBodyRes == null || !compareRecipeBodyRes.Result)
                    {
                        AllowInput = InputStatus.Reject;
                        var message = $"程式{machineRecipeName}内容不匹配，请联系ME/EQ:{compareRecipeBodyRes.Message}";
                        traLog.Error(message);
                        UIMessageBox.ShowError2(message);
                        return;
                    }
                }

                //过站
                var empno = uiTextBox_empNo.Text;
                var line = uiTextBox_line.Text;
                ConfigManager<SputtereConfig> manager = new ConfigManager<SputtereConfig>();
                var config = manager.LoadConfig().CathodeSettings;
                var modelName = uiTextBox_modelName.Text;
                var trayId = uiTextBox_trayId.Text;
                var baymaxIp = _configuration.GetSection("Custom")["SfisIp"];
                var baymaxPort = int.Parse(_configuration.GetSection("Custom")["SfisPort"] ?? "21347");

                string cathodeStr = string.Join(" ", config.Select(it => $"CATHODE_{it.Seq}={it.CathodeId}"));
                string step2Req = $@"{equipmentId},{snInfos.First().CarrierId},2,{empno},{line},,OK,,,ACTUAL_GROUP=SPUTTER {cathodeStr} ,,,{string.Join(";", snInfos.Select(it => it.CarrierId))},{trayId},,{modelName}";
                BaymaxService baymaxService = new BaymaxService();
                var trans = await baymaxService.GetBaymaxTrans(baymaxIp, baymaxPort, step2Req);
                if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {
                    AllowInput = InputStatus.Allow;
                }
                else
                {
                    AllowInput = InputStatus.Reject;
                    var message = $"SFIS过站失败：{trans.BaymaxResponse}";
                    traLog.Error(message);
                    UIMessageBox.ShowError2(message);
                }
            }
            catch (Exception ex)
            {
                traLog.Error(ex.ToString());
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            traLog.Warn("强制允许入料");
            RefreshTimer();
            AllowInput = InputStatus.Allow;
        }
    }
}
