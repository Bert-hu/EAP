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

namespace EAP.Client.Forms
{
    public partial class MixPackageSettingForm : UIForm
    {

        public MixPackageSettingForm(string packageName)
        {
            InitializeComponent();
        }

        private void button_confirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch (Exception)
            {
                UIMessageBox.ShowError($"请输入正确数字");
            }
        }

        private void uiComboBox_packageName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
