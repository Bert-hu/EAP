using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using EAP.Client.Models;
using EAP.Client.Utils;

namespace EAP.Client.Forms
{
    public partial class MixPackageSettingForm : UIForm
    {
        ConfigManager<MixPackageSetting> configManager = new ConfigManager<MixPackageSetting>();
        MixPackageSetting config;
        public MixPackageSettingForm(string packageName)
        {
            InitializeComponent();
            config = configManager.LoadConfig();
            if (string.IsNullOrEmpty(packageName))
            {
                if (config.Settings.Count > 0)
                {
                    packageName = config.Settings.Keys.First();
                }
            }
            if (!string.IsNullOrEmpty(packageName))
            {
                if (!config.Settings.ContainsKey(packageName))
                {
                    config.Settings.Add(packageName, new CountSetting());
                }
                uiTextBox_icos.Text = config.Settings[packageName].IcosCount.ToString();
                uiTextBox_m.Text = config.Settings[packageName].MCount.ToString();
                uiTextBox_oh.Text = config.Settings[packageName].OhCount.ToString();
                uiComboBox_packageName.Items.Clear();
                uiComboBox_packageName.DataSource = config.Settings.Keys.ToList();
                index = uiComboBox_packageName.Items.IndexOf(packageName);
                uiComboBox_packageName.SelectedIndex = uiComboBox_packageName.Items.IndexOf(packageName);
            }
        }

        private void button_confirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.OK;
                try
                {
                    config.Settings[uiComboBox_packageName.SelectedItem.ToString()].IcosCount = int.Parse(uiTextBox_icos.Text);
                    config.Settings[uiComboBox_packageName.SelectedItem.ToString()].MCount = int.Parse(uiTextBox_m.Text);
                    config.Settings[uiComboBox_packageName.SelectedItem.ToString()].OhCount = int.Parse(uiTextBox_oh.Text);
                    configManager.SaveConfig(config);
                }
                catch (Exception)
                {
                    UIMessageBox.ShowError($"请输入正确数字");
                }
                this.Close();

            }
            catch (Exception)
            {
                UIMessageBox.ShowError($"请输入正确数字");
            }
        }


        private int index = -1;

        private void uiComboBox_packageName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (index != -1 && index != this.uiComboBox_packageName.SelectedIndex)
            {
                try
                {
                    var lastPackageName = uiComboBox_packageName.Items[index].ToString();         

                    var newIcosCount = int.Parse(uiTextBox_icos.Text);
                    var newMCount = int.Parse(uiTextBox_m.Text);
                    var newOhCount = int.Parse(uiTextBox_oh.Text);
                    if (config.Settings[lastPackageName].IcosCount != newIcosCount
                        || config.Settings[lastPackageName].MCount != newMCount
                        || config.Settings[lastPackageName].OhCount != newOhCount)
                    {
                        var askMessage = $"是否保存{lastPackageName}的设定？";
                        if (UIMessageBox.ShowAsk(askMessage) == true)
                        { 
                            config.Settings[lastPackageName].IcosCount = newIcosCount;
                            config.Settings[lastPackageName].MCount = newMCount;
                            config.Settings[lastPackageName].OhCount = newOhCount;
                            configManager.SaveConfig(config);
                        }
                    }    
                }
                catch (Exception ex)
                {
                    UIMessageBox.ShowError(ex.ToString());
                }
            }
            index = this.uiComboBox_packageName.SelectedIndex;
            var newPackageName = uiComboBox_packageName.Items[index].ToString();
            uiTextBox_icos.Text = config.Settings[newPackageName].IcosCount.ToString();
            uiTextBox_m.Text = config.Settings[newPackageName].MCount.ToString();
            uiTextBox_oh.Text = config.Settings[newPackageName].OhCount.ToString();
        }

        private void uiButton_save_Click(object sender, EventArgs e)
        {
            try
            {
                config.Settings[uiComboBox_packageName.SelectedItem.ToString()].IcosCount = int.Parse(uiTextBox_icos.Text);
                config.Settings[uiComboBox_packageName.SelectedItem.ToString()].MCount = int.Parse(uiTextBox_m.Text);
                config.Settings[uiComboBox_packageName.SelectedItem.ToString()].OhCount = int.Parse(uiTextBox_oh.Text);
                configManager.SaveConfig(config);
            }
            catch (Exception)
            {
                UIMessageBox.ShowError($"请输入正确数字");
            }
            UIMessageBox.ShowSuccess($"保存成功");
        }
    }
}
