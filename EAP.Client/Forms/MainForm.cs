using EAP.Client.RabbitMq;
using EAP.Client.Secs;
using EAP.Client.Secs.PrimaryMessageHandler.EventHandler;
using EAP.Client.Sfis;
using EAP.Client.Utils;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Secs4Net;
using Sunny.UI;
using static Secs4Net.Item;

namespace EAP.Client.Forms
{
    public partial class MainForm : UIForm
    {
        private static MainForm? instance;
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

        public void UpdateSpiPanelAndModelname(string panelid, string modelname)
        {
            this.Invoke(new Action(() =>
            {
                this.textBox_spipanelid.Text = panelid;
                this.textBox_spimodelname.Text = modelname;
                this.label_updatetime_spi.Text = "Update Time: " + DateTime.Now.ToString("MM-dd HH:mm:ss");
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
                this.Text = _commonLibrary.CustomSettings["EquipmentId"] + " " + _commonLibrary.CustomSettings["EquipmentType"];
                label_conn_status.Text = showtext;
                label_conn_status.BackColor = backcolor;
            }));

            string sfisIp = _commonLibrary.CustomSettings["SfisIp"];
            int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);
            BaymaxService baymax = new BaymaxService();
            baymax.OnBaymaxTransCompleted += Baymax_OnBaymaxTrans;
            baymax.StartBaymaxForwardingService(_secsConnection.IpAddress.ToString(), 21347, sfisIp, sfisPort, HandleBaymaxResponse);

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
                    int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);
                    //var getModelNameReq = $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???";
                    var getModelProjextReq = $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_PROJECT_NAME_INFO=???";
                    var trans = sender.GetBaymaxTrans(sfisIp, sfisPort, getModelProjextReq);
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
                            var req = new { EquipmentTypeId = "Hanmi_jigsaw", RecipeName = recipeName };
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
            DialogResult dr = MessageBox.Show("是否关闭程序？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
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
                    var s1f3 = new SecsMessage(1, 3)
                    {
                        SecsItem = L(U2(1013), U2(1106))
                    };
                    var s1f4 = _secsGem.SendAsync(s1f3).Result;
                    var recipeName = s1f4.SecsItem.Items[0].GetString();

                    string comparemsg = string.Empty;
                    if (!string.IsNullOrEmpty(recipeName))
                    {
                        this.Invoke(() =>
                        {
                            this.textBox_machinerecipe.Text = recipeName;
                        });
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
                        var lotno = form.Value;
                        string sfisIp = _commonLibrary.CustomSettings["SfisIp"];
                        int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);

                        var getLotGrpInfo = $"EQXXXXXX01,{lotno},7,Admin,JORDAN,,OK,LOT_GRP_INFO_V2=???";
                        BaymaxService baymax = new BaymaxService();
                        traLog.Info($"Send to SFIS: {getLotGrpInfo}");
                        var trans = baymax.GetBaymaxTrans(sfisIp, sfisPort, getLotGrpInfo);
                        if (trans.Result && trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                        {
                            traLog.Info(trans.BaymaxResponse);
                            var equipmentId = _commonLibrary.CustomSettings["EquipmentId"];
                            Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                                      .Where(keyValueArray => keyValueArray.Length == 2)
                                      .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                            //string modelName = sfisParameters["SN_MODEL_NAME_INFO"];//第一种
                            string projectName = sfisParameters["LOT_GRP_INFO_V2"].TrimEnd(';').Split(';')[0].Split(':')[0];
                            string productName = sfisParameters["LOT_GRP_INFO_V2"].TrimEnd(';').Split(';')[0].Split(':')[1];
                            string modelName = sfisParameters["LOT_GRP_INFO_V2"].TrimEnd(';').Split(';')[0].Split(':')[2];
                            string GroupName = sfisParameters["LOT_GRP_INFO_V2"].TrimEnd(';').Split(';')[0].Split(':')[3];

                            var rmsUrl = _commonLibrary.CustomSettings["RmsApiUrl"];
                            var reqUrl = rmsUrl.TrimEnd('/') + "/api/GetRecipeName";
                            var req = new { EquipmentTypeId = "Hanmi_jigsaw", RecipeNameAlias = projectName };
                            var rep = HttpClientHelper.HttpPostRequestAsync<GetRecipeNameResponse>(reqUrl, req).Result;
                            if (rep != null)
                            {
                                if (rep.Result)
                                {
                                    var recipeName = rep.RecipeName;

                                    var s1f3 = new SecsMessage(1, 3)
                                    {
                                        SecsItem = L(U2(1013), U2(1106))
                                    };
                                    var s1f4 = _secsGem.SendAsync(s1f3).Result;
                                    var machineRecipeName = s1f4.SecsItem.Items[0].GetString();

                                    if (machineRecipeName == recipeName)
                                    {
                                        UIMessageBox.ShowSuccess($"当前程式已是{recipeName},无需重新下载，请继续！");
                                    }
                                    else
                                    {
                                        var deleteRecipe = UIMessageBox.ShowAsk("确认删除当前所有MP的Recipe吗？");
                                        if (deleteRecipe)
                                        {
                                            //DeleteAllRecipes deleteAll = new DeleteAllRecipes();
                                            //await deleteAll.HandleTransaction(new RabbitMqTransaction(), null, _secsGem, null, commonLibrary);
                                            SecsMessage s7f19 = new(7, 19, true)
                                            {
                                            };
                                            var s7f20 = await _secsGem.SendAsync(s7f19);
                                            var mpRecipes = new List<string>();
                                            foreach (var item in s7f20.SecsItem.Items)
                                            {
                                                var rec = item.GetString();
                                                if (rec.StartsWith("MP\\"))
                                                {
                                                    mpRecipes.Add(rec);
                                                }
                                            }

                                            if (mpRecipes.Count > 0)
                                            {
                                                SecsMessage s7f17 = new(7, 17, true)
                                                {
                                                    SecsItem = L(
                                        from recipe in mpRecipes select A(recipe)
                                        )
                                                };
                                                var s7f18 = await _secsGem.SendAsync(s7f17);
                                                var s7f18ack = s7f18.SecsItem.FirstValue<byte>();
                                                //await Task.Delay(2000);
                                            }
                                        }
                                        this.Invoke(() =>
                                        {
                                            Task.Run(async () =>
                                            {
                                                string url = _commonLibrary.CustomSettings["RmsApiUrl"].TrimEnd('/') + "/api/downloadeffectiverecipetomachine";
                                                var req = new { TrueName = "EAP", EquipmentId = equipmentId, RecipeName = recipeName };

                                                var rep = HttpClientHelper.HttpPostRequestAsync<DownloadEffectiveRecipeToMachineResponse>(url, req).Result;

                                                if (rep != null && rep.Result)
                                                {
                                                    var waitSeconds = 5;
                                                    var _waitSeconds = waitSeconds;
                                                    while (_waitSeconds-- > 0)
                                                    {
                                                        SecsMessage s7f19 = new(7, 19, true)
                                                        {
                                                        };
                                                        var s7f20 = _secsGem.SendAsync(s7f19).Result;
                                                        List<string> EPPD = new List<string>();
                                                        foreach (var item in s7f20.SecsItem.Items)
                                                        {
                                                            EPPD.Add(item.GetString());
                                                        }
                                                        if (EPPD.Contains(recipeName))
                                                        {

                                                            var s2f41 = new SecsMessage(2, 41, true)
                                                            {
                                                                SecsItem = L(
                                                                    A("PP_SELECT_S"),
                                                                    L(L(A("Port"), B(1)),
                                                                    L(A("DEV_NO"), A(recipeName))))
                                                            };
                                                            var s2f42 = _secsGem.SendAsync(s2f41).Result;
                                                            var s2f42ack = s2f42.SecsItem[0].FirstValue<byte>();

                                                            if (s2f42ack == 0)
                                                            {
                                                                var message = $"{lotno},{projectName}下载程式{recipeName}成功，并且自动切换成功";
                                                                traLog.Info(message);
                                                                UIMessageBox.ShowSuccess(message);
                                                            }
                                                            else
                                                            {
                                                                var message = $"{lotno},{projectName}下载程式{recipeName}成功，自动切换失败，请手动切换";
                                                                traLog.Info(message);
                                                                UIMessageBox.ShowWarning(message);
                                                            }
                                                            var s1f3 = new SecsMessage(1, 3)
                                                            {
                                                                SecsItem = L(U2(1013), U2(1106))
                                                            };
                                                            var s1f4 = _secsGem.SendAsync(s1f3).Result; //注意这里一定要await
                                                            var machinerecipeName = s1f4.SecsItem.Items[0].GetString();

                                                            this.textBox_machinerecipe.Text = machinerecipeName;

                                                            return;
                                                        }
                                                        await Task.Delay(1000);
                                                    }
                                                    var errmsg = $"{lotno},{projectName}下载程式{recipeName}超时：{waitSeconds}s,请检查DISCO状态";
                                                    traLog.Error(errmsg);
                                                    UIMessageBox.ShowError(errmsg);

                                                }
                                                else
                                                {
                                                    var message = $"{lotno},{projectName}下载程式{recipeName}失败：{rep.Message}";
                                                    traLog.Error(message);
                                                    UIMessageBox.ShowError(message);
                                                }
                                            });
                                        });
                                    }
                                }
                                else
                                {
                                    var errMsg = $"RMS未找到'{projectName}'对应的程式，请联系ME设定";
                                    traLog.Error(errMsg);
                                    UIMessageBox.ShowError(errMsg);
                                }
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

        private void button_getModelName_Click(object sender, EventArgs e)
        {
            var panelsn = textBox_spipanelid.Text;
            string sfisIp = _commonLibrary.CustomSettings["SfisIp"];
            int sfisPort = Convert.ToInt32(_commonLibrary.CustomSettings["SfisPort"]);
            BaymaxService baymax = new BaymaxService();
            var trans = baymax.GetBaymaxTrans(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???");

            //(bool success, string sfisResponse, string errorMessage) = SendMessageToSfis(sfisIp, sfisPort, $"EQXXXXXX01,{panelsn},7,M001603,JORDAN,,OK,SN_MODEL_NAME_INFO=???");


            if (trans.Result)
            {
                if (trans.BaymaxResponse.ToUpper().StartsWith("OK"))
                {
                    // Parse SFIS response to get model name and wafer IDs
                    Dictionary<string, string> sfisParameters = trans.BaymaxResponse.Split(',')[1].Split(' ').Select(keyValueString => keyValueString.Split('='))
                        .Where(keyValueArray => keyValueArray.Length == 2)
                        .ToDictionary(keyValueArray => keyValueArray[0], keyValueArray => keyValueArray[1]);
                    string modelName = sfisParameters["SN_MODEL_NAME_INFO"];
                    MainForm.Instance.UpdateSpiPanelAndModelname(panelsn, modelName);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //是否需要切换recipe
            DialogResult dialogResult = MessageBox.Show("是否需要切换recipe？将发送停止指令给AOI", "提示", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var relatedRecipe = GetAoiRelatedRecipe(textBox_spimodelname.Text);
                if (string.IsNullOrEmpty(relatedRecipe))
                {
                    MessageBox.Show($"未找到设备与'{textBox_spimodelname.Text}'相关的recipe");
                    return;
                }
                //var processStateVID = _commonLibrary.GetGemSvid("ProcessState");
                //SecsMessage s1f3 = new(1, 3, true)
                //{
                //    SecsItem = L(
                //      U4((uint)processStateVID.ID)
                //      )
                //};
                //var s1f4 = _secsGem.SendAsync(s1f3).Result;
                //var processStateCode = s1f4.SecsItem[0].FirstValue<byte>();
                //if (processStateCode == 1)
                //{
                //    var s2f41ppselect = new SecsMessage(2, 41)
                //    {
                //        SecsItem = L(
                //                  A("PP-SELECT"),
                //                  L(
                //                      L(
                //                      A("PPID"),
                //                      A(relatedRecipe)
                //                      )
                //                      )
                //                  )
                //    };
                //    _ = _secsGem.SendAsync(s2f41ppselect).Result;
                //}
                //else
                {
                    var s2f41ppselect = new SecsMessage(2, 41)
                    {
                        SecsItem = L(A("STOP"), L())
                    };
                    _ = _secsGem.SendAsync(s2f41ppselect).Result;
                    ProcessStateChanged.NeedChangeRecipe = true;
                    ProcessStateChanged.ChangeRecipeName = relatedRecipe;
                    ProcessStateChanged.ChangeDateTime = DateTime.Now;

                    MessageBox.Show($"已发送停止指令，2分钟内设备进入Idle状态将自动切换到{relatedRecipe}");
                }
            }

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
    }
}
