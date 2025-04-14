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
        private string HandleBaymaxResponse(BaymaxService sender, string machineRequest, string baymaxResponse)
        {
            try
            {
                var stepid = machineRequest.Split(',')[2];
                var panelsn = machineRequest.Split(',')[1];
                this.Invoke(() =>
                {
                    this.textBox_panelid.Text = panelsn;
                });

                if (isAutoCheckRecipe && stepid == "1" && _secsConnection.State == ConnectionState.Selected)
                {
                    string sfisIp = _commonLibrary.CustomSettings["SfisIp"];
                    string equipmentId = _commonLibrary.CustomSettings["EquipmentId"];
                    string equipmentType = _commonLibrary.CustomSettings["EquipmentType"];
                    int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);
                    //var getModelNameReq = $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???";
                    var getModelProjextReq = $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";
                    var trans = sender.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq).Result;
                    if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                    {
                        Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                  .Where(keyValueArray => keyValueArray.Length == 2)
                                  .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                        //string modelName = sfisParameters["SN_MODEL_NAME_INFO"];//第一种
                        string modelName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[0];
                        string projectName = sfisParameters["SN_MODEL_NAME_PROJECT_NAME_INFO"].TrimEnd(';').Split(':')[1];
                        this.Invoke(() =>
                        {
                            this.textBox_modelname.Text = modelName;
                            this.textBox_projectname.Text = projectName;
                        });
                        var s1f3 = new SecsMessage(1, 3)
                        {
                            SecsItem = L(U2(1013), U2(1106))
                        };
                        var s1f4 = _secsGem.SendAsync(s1f3).Result;
                        var recipeName = s1f4.SecsItem.Items[0].GetString();

                        var compareresult = false;
                        string comparemsg = string.Empty;
                        if (!string.IsNullOrEmpty(recipeName))
                        {
                            this.Invoke(() =>
                            {
                                this.textBox_machinerecipe.Text = recipeName;
                            });

                            var rmsUrl = _commonLibrary.CustomSettings["RmsApiUrl"];
                            var reqUrl = rmsUrl.TrimEnd('/') + "/api/GetRecipeNameAlias";
                            var req = new { EquipmentTypeId = equipmentType, RecipeName = recipeName };
                            var rep = HttpClientHelper.HttpPostRequestAsync<GetRecipeNameAliasResponse>(reqUrl, req).Result;
                            if (rep != null)
                            {
                                if (rep.Result && rep.RecipeAlias.Contains(projectName))
                                {
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
                                            compareresult = true;
                                        }
                                    }
                                    else
                                    {
                                        comparemsg = "Compare recipe fail: Timeout";
                                        traLog.Error(comparemsg);
                                    }
                                }
                                else
                                {
                                    comparemsg = $"Compare recipe fail: {rep.RecipeAlias} do not match";
                                    traLog.Error(comparemsg);
                                }
                            }
                        }
                        else
                        {
                            return "Fail,Recipe Name is Null";
                        }

                        if (compareresult)
                        {
                            return baymaxResponse;
                        }
                        else
                        {
                            //SecsMessage s10f3 = new(10, 3, false)
                            //{
                            //    SecsItem = L(B(0x00), A($"Recipe compare fail: {recipeName},{modelName},{projectName},{comparemsg} "))
                            //};
                            //_secsGem.SendAsync(s10f3);
                            return "Fail,Recipe mismatch";
                        }
                    }
                    else
                    {
                        return baymaxResponse;
                    }
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

        public bool isAutoCheckRecipe = true;
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
                    var recipeName = textBox_machinerecipe.Text;
                    string comparemsg = string.Empty;
                    if (!string.IsNullOrEmpty(recipeName))
                    {
                        string equipmentId = _commonLibrary.CustomSettings["EquipmentId"];

                        var rabbitTrans = new RabbitMqTransaction()
                        {
                            TransactionName = "CompareRecipeBody",
                            EquipmentID = equipmentId,
                            NeedReply = true,
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


        private void uiButton_ScanToDownloadRecipe_Click(object sender, EventArgs e)
        {
            uiButton_ScanToDownloadRecipe.Enabled = false;

            Task.Run(async () =>
            {
                try
                {
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
                        var trans = baymax.GetBaymaxTrans(sfisIp, sfisPort, getModelnameReq).Result;
                        if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                        {
                            var equipmentId = _commonLibrary.CustomSettings["EquipmentId"];
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
                                var aa = _secsGem.SendAsync(stop).Result;

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
                                _secsGem.SendAsync(s2f41load);
                            }
                            else
                            {
                                traLog.Error($"设备中找不到与'{modelname}'匹配的程式！");
                            }
                        }
                        else
                        {
                            traLog.Error(trans.BaymaxResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UIMessageBox.ShowError(ex.Message);
                }
                this.Invoke(() =>
                {
                    uiButton_ScanToDownloadRecipe.Enabled = true;
                });

            });
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
    }
}
