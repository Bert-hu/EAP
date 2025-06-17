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
        public void UpdateAoiPanelAndModelname(string panelid, string modelname)
        {
            this.Invoke(new Action(() =>
            {
                // if (string.IsNullOrEmpty(panelid))
                this.textBox_panelid.Text = panelid;
                // if (string.IsNullOrEmpty(modelname))
                this.textBox_modelname.Text = modelname;
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
        public bool isAutoCheckRecipePara { get; set; } = true;
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

        private void uiCheckBox_checkRecipePara_CheckedChanged(object sender, EventArgs e)
        {
            if (uiCheckBox_checkRecipePara.Checked == false)
            {
                PasswordForm form = new PasswordForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.Value == _commonLibrary.CustomSettings["Password"])
                    {
                        uiCheckBox_checkRecipePara.Checked = false;
                    }
                    else
                    {
                        UIMessageBox.ShowError("密码错误");
                        uiCheckBox_checkRecipePara.Checked = true;
                    }
                }
                else
                {
                    uiCheckBox_checkRecipePara.Checked = true;
                }
            }
            isAutoCheckRecipePara = uiCheckBox_checkRecipePara.Checked;
        }

        private void button_CompareRecipe_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {

                (bool result, string message) = CheckRecipePara(this.textBox_machinerecipe.Text);

                if (result)
                {
                    traLog.Info(message);
                }
                else
                {
                    traLog.Error(message);

                }
            });
        }
        public class DownloadEffectiveRecipeToMachineResponse
        {
            public bool Result { get; set; } = false;
            public string Message { get; set; }
            public string RecipeName { get; set; }
        }


        internal string? GetAoiRelatedRecipe(string recipeGroupName)
        {
            SecsMessage s7f19 = new(7, 19, true)
            {
            };
            var rep = _secsGem.SendAsync(s7f19).Result;
            List<string> EPPD = new List<string>();//2103-19010X-XXT.recipe
            foreach (var item in rep.SecsItem.Items)
            {
                EPPD.Add(item.GetString());
            }
            var relatedRecipe = EPPD.FirstOrDefault(it => it.Substring(0, it.Length > 10 ? 10 : it.Length) == recipeGroupName.Substring(0, recipeGroupName.Length > 10 ? 10 : recipeGroupName.Length));
            return relatedRecipe;
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

        private void uiButton_ppSelect_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                this.Invoke(new Action(async () =>
                {
                    try
                    {
                        uiButton_PpSelect.Enabled = false;
                        PpSelectForm form = new PpSelectForm();
                        DialogResult dr = form.ShowDialog();
                        if (dr == DialogResult.OK)
                        {
                            var panelSn = form.Value;
                            string sfisIp = _commonLibrary.CustomSettings["SfisIp"];
                            int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);

                            var getModelnameReq = $"EQXXXXXX01,{panelSn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???";
                            traLog.Info($"扫码切换程式:{panelSn}");
                            BaymaxService baymax = new BaymaxService();
                            var trans = await baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelnameReq);
                            if (trans.Result || trans.BaymaxResponse.ToLower().StartsWith("fail"))
                            {
                                Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                          .Where(keyValueArray => keyValueArray.Length == 2)
                          .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                                var modelname = sfisParameters["SN_MODEL_NAME_INFO"];
                                var recipeName = GetAoiRelatedRecipe(modelname);//不同的设备类型，需要获取不同的程式

                                if (recipeName != null)
                                {
                                    var stop = new SecsMessage(2, 41)
                                    {
                                        SecsItem = L(
                                        A("STOP"),
                                        L(
                                            L(

                                                )
                                            ))
                                    };
                                    var aa = await _secsGem.SendAsync(stop);

                                    //等2秒
                                    Thread.Sleep(5000);


                                    var s2f41load = new SecsMessage(2, 41)
                                    {
                                        SecsItem = L(
                                            A("PPSELECT"),
                                            L(
                                                L(
                                                      A("PPID"),
                                                      A(recipeName)
                                                    )
                                                ))
                                    };
                                    await _secsGem.SendAsync(s2f41load);
                                    traLog.Info($"发送切换到 {recipeName} 指令完成");

                                }
                                else
                                {
                                    traLog.Error($"设备中找不到与'{modelname}'匹配的程式！");
                                }
                            }
                            else
                            {
                                traLog.Error($"Sfis error: {trans.BaymaxResponse}");
                            }

                        }

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    finally
                    {
                        uiButton_PpSelect.Enabled = true;
                    }
                }
                ));
            });
        }

        private (bool result, string message) CheckRecipePara(string recipename)
        {
            bool result = false;
            string message = string.Empty;
            try
            {
                var EquipmentId = _commonLibrary.CustomSettings["EquipmentId"];
                var trans = new RabbitMqTransaction
                {
                    TransactionName = "CompareRecipeBody",
                    EquipmentID = EquipmentId,
                    NeedReply = true,
                    ExpireSecond = 5,
                    Parameters = new Dictionary<string, object>() { { "EquipmentId", EquipmentId }, { "RecipeName", recipename } }
                };
                var rabbitRes = _rabbitMq.ProduceWaitReply("Rms.Service", trans);
                if (rabbitRes != null)
                {
                    result = rabbitRes.Parameters["Result"].ToString().ToUpper() == "TRUE";
                    rabbitRes.Parameters.TryGetValue("Message", out object messageObj);
                    message = messageObj.ToString();
                }
                else
                {
                    result = false;
                    message = "RabbitMq Trans CompareRecipeBody Time out";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;

            }

            return (result, message);
        }


    }
}
