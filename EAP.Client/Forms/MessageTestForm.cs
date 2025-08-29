using EAP.Client.NonSecs.Message;
using Microsoft.Extensions.Configuration;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
        private readonly IConfiguration configuration;

        public MessageTestForm()
        {
            InitializeComponent();
        }

        public MessageTestForm(IConfiguration configuration,NonSecsService nonSecsService)
        {
            this.nonSecsService = nonSecsService;
            this.configuration = configuration;
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

        private async void QuickTestS1F11_Click(object sender, EventArgs e)
        {
            try
            {
                var s1f11 = new NonSecsMessage() { Stream = 1, Function = 11 };
                var reply = await nonSecsService.SendMessage(s1f11);
                uiRichTextBox_receiveMessage.Text = reply.SecondaryMessageString;
            }
            catch (Exception ex)
            {
                uiRichTextBox_receiveMessage.Text = ex.ToString();
            }

        }

        private async void QuickTestS5F5_Click(object sender, EventArgs e)
        {
            try
            {
                var s5f5 = new NonSecsMessage() { Stream = 5, Function = 5 };
                var reply = await nonSecsService.SendMessage(s5f5);
                uiRichTextBox_receiveMessage.Text = reply.SecondaryMessageString;
            }
            catch (Exception ex)
            {
                uiRichTextBox_receiveMessage.Text = ex.ToString();
            }

        }

        private async void QuickTestS1F3_Click(object sender, EventArgs e)
        {
            try
            {
                var paramsVid = configuration.GetSection("NonSecs:ParamsVid").Get<List<string>>();
                var s1f3 = new S1F3() { List = paramsVid };
                var reply = await nonSecsService.SendMessage(s1f3);
                uiRichTextBox_receiveMessage.Text = reply.SecondaryMessageString;
            }
            catch (Exception ex)
            {
                uiRichTextBox_receiveMessage.Text = ex.ToString();
            }

        }
    }
}

