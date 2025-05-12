
using EAP.Client.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAP.Client.Forms
{
    public partial class SputterCathodeSettingForm : UIForm
    {
        //public int seq;
        //public string cathodeId;

        public CathodeSetting cathodeSetting;
        public SputterCathodeSettingForm(CathodeSetting _cathodeSetting = null)
        {   
            InitializeComponent();
            if (_cathodeSetting == null) _cathodeSetting = new CathodeSetting();
            uiTextBox_seq.Text = _cathodeSetting.Seq.ToString();
            uiTextBox_cathodeid.Text = _cathodeSetting.CathodeId;
            cathodeSetting = _cathodeSetting;
        }

        private void uiButton_confirm_Click(object sender, EventArgs e)
        {

            try
            {
                this.DialogResult = DialogResult.OK;

                cathodeSetting.Seq = int.Parse(uiTextBox_seq.Text);
                cathodeSetting.CathodeId = uiTextBox_cathodeid.Text;
            }
            catch (Exception ex)
            {
                this.DialogResult = DialogResult.Cancel;
                this.ShowErrorDialog(ex.ToString());
            }
            this.Close();
        }

        private void SputterCathodeSettingForm_Shown(object sender, EventArgs e)
        {
            uiTextBox_cathodeid.SelectAll();
            //uiTextBox_seq.SelectAll();
        }
    }
}
