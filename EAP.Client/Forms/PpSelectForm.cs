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
    public partial class PpSelectForm : UIForm
    {
        public string Value { get; set; }

        public PpSelectForm()
        {
            InitializeComponent();
        }

        private void button_confirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                UIMessageBox.ShowWarning("Can not be null");
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                Value = textBox1.Text;
                this.Close();
            }
        }
    }
}
