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
        public int icosCount { get; set; }
        public int mCount { get; set; }
        public int ohCount { get; set; }

        public MixPackageSettingForm(int icosCount, int mCount, int ohCount)
        {
            InitializeComponent();
            textBox_icos.Text = icosCount.ToString();
            uiTextBox_m.Text = mCount.ToString();
            uiTextBox_oh.Text = ohCount.ToString();
        }

        private void button_confirm_Click(object sender, EventArgs e)
        {
            try
            {
                icosCount = int.Parse(textBox_icos.Text);
                mCount = int.Parse(uiTextBox_m.Text);
                ohCount = int.Parse(uiTextBox_oh.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            catch (Exception)
            {
                UIMessageBox.ShowError($"请输入正确数字");
            }
        }
    }
}
