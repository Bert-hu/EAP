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
    public partial class MessageTestForm : UIForm
    {
        private readonly NonSecsService nonSecsService;

        public MessageTestForm()
        {
            InitializeComponent();
        }

        public MessageTestForm(NonSecsService nonSecsService)
        {
            this.nonSecsService = nonSecsService;
            InitializeComponent();
        }

        private async void uiButton_send_Click(object sender, EventArgs e)
        {
            try
            {
                var sendmessage = uiRichTextBox_sendMessage.Text;
                var reply = await nonSecsService.SendMessage(sendmessage);
                uiRichTextBox_receiveMessage.Text = reply.SecondaryMessageString;
            }
            catch (Exception ex)
            {
                uiRichTextBox_receiveMessage.Text = ex.ToString();
            }
        }
    }
}
