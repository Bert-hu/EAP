namespace EAP.Client.Forms
{
    partial class MessageTestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            uiRichTextBox_sendMessage = new Sunny.UI.UIRichTextBox();
            uiRichTextBox_receiveMessage = new Sunny.UI.UIRichTextBox();
            uiButton_send = new Sunny.UI.UIButton();
            QuickTestS1F11 = new Sunny.UI.UIButton();
            QuickTestS5F11 = new Sunny.UI.UIButton();
            QuickTestS1F3 = new Sunny.UI.UIButton();
            SuspendLayout();
            // 
            // uiRichTextBox_sendMessage
            // 
            uiRichTextBox_sendMessage.FillColor = Color.White;
            uiRichTextBox_sendMessage.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiRichTextBox_sendMessage.Location = new Point(25, 56);
            uiRichTextBox_sendMessage.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox_sendMessage.MinimumSize = new Size(1, 1);
            uiRichTextBox_sendMessage.Name = "uiRichTextBox_sendMessage";
            uiRichTextBox_sendMessage.Padding = new Padding(2);
            uiRichTextBox_sendMessage.ShowText = false;
            uiRichTextBox_sendMessage.Size = new Size(583, 216);
            uiRichTextBox_sendMessage.TabIndex = 0;
            uiRichTextBox_sendMessage.Text = "{\n\t\"Stream\": 1,\n\t\"Function\": 3,\n\t\"List\": [\"1001\", \"1002\", \"1003\"]\n}";
            uiRichTextBox_sendMessage.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiRichTextBox_receiveMessage
            // 
            uiRichTextBox_receiveMessage.FillColor = Color.White;
            uiRichTextBox_receiveMessage.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiRichTextBox_receiveMessage.Location = new Point(25, 323);
            uiRichTextBox_receiveMessage.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox_receiveMessage.MinimumSize = new Size(1, 1);
            uiRichTextBox_receiveMessage.Name = "uiRichTextBox_receiveMessage";
            uiRichTextBox_receiveMessage.Padding = new Padding(2);
            uiRichTextBox_receiveMessage.ReadOnly = true;
            uiRichTextBox_receiveMessage.ShowText = false;
            uiRichTextBox_receiveMessage.Size = new Size(583, 207);
            uiRichTextBox_receiveMessage.TabIndex = 0;
            uiRichTextBox_receiveMessage.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiButton_send
            // 
            uiButton_send.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_send.Location = new Point(508, 280);
            uiButton_send.MinimumSize = new Size(1, 1);
            uiButton_send.Name = "uiButton_send";
            uiButton_send.Size = new Size(100, 35);
            uiButton_send.TabIndex = 1;
            uiButton_send.Text = "Send";
            uiButton_send.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_send.Click += uiButton_send_Click;
            // 
            // QuickTestS1F11
            // 
            QuickTestS1F11.FillColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F11.FillColor2 = Color.FromArgb(255, 192, 192);
            QuickTestS1F11.FillHoverColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F11.FillPressColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F11.FillSelectedColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F11.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            QuickTestS1F11.Location = new Point(157, 280);
            QuickTestS1F11.MinimumSize = new Size(1, 1);
            QuickTestS1F11.Name = "QuickTestS1F11";
            QuickTestS1F11.Size = new Size(100, 35);
            QuickTestS1F11.TabIndex = 2;
            QuickTestS1F11.Text = "S1F11";
            QuickTestS1F11.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            QuickTestS1F11.Click += QuickTestS1F11_Click;
            // 
            // QuickTestS5F11
            // 
            QuickTestS5F11.FillColor = Color.FromArgb(255, 128, 128);
            QuickTestS5F11.FillColor2 = Color.FromArgb(255, 192, 192);
            QuickTestS5F11.FillHoverColor = Color.FromArgb(255, 128, 128);
            QuickTestS5F11.FillPressColor = Color.FromArgb(255, 128, 128);
            QuickTestS5F11.FillSelectedColor = Color.FromArgb(255, 128, 128);
            QuickTestS5F11.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            QuickTestS5F11.Location = new Point(289, 280);
            QuickTestS5F11.MinimumSize = new Size(1, 1);
            QuickTestS5F11.Name = "QuickTestS5F11";
            QuickTestS5F11.Size = new Size(100, 35);
            QuickTestS5F11.TabIndex = 3;
            QuickTestS5F11.Text = "S5F11";
            QuickTestS5F11.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            QuickTestS5F11.Click += QuickTestS5F11_Click;
            // 
            // QuickTestS1F3
            // 
            QuickTestS1F3.FillColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F3.FillColor2 = Color.FromArgb(255, 128, 128);
            QuickTestS1F3.FillHoverColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F3.FillPressColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F3.FillSelectedColor = Color.FromArgb(255, 128, 128);
            QuickTestS1F3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            QuickTestS1F3.Location = new Point(25, 280);
            QuickTestS1F3.MinimumSize = new Size(1, 1);
            QuickTestS1F3.Name = "QuickTestS1F3";
            QuickTestS1F3.Size = new Size(100, 35);
            QuickTestS1F3.TabIndex = 4;
            QuickTestS1F3.Text = "S1F3";
            QuickTestS1F3.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            QuickTestS1F3.Click += QuickTestS1F3_Click;
            // 
            // MessageTestForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(630, 547);
            Controls.Add(QuickTestS1F3);
            Controls.Add(QuickTestS5F11);
            Controls.Add(QuickTestS1F11);
            Controls.Add(uiButton_send);
            Controls.Add(uiRichTextBox_receiveMessage);
            Controls.Add(uiRichTextBox_sendMessage);
            MaximizeBox = false;
            Name = "MessageTestForm";
            ShowIcon = false;
            Text = "MessageTestForm";
            ZoomScaleRect = new Rectangle(15, 15, 800, 450);
            ResumeLayout(false);
        }

        #endregion

        private Sunny.UI.UIRichTextBox uiRichTextBox_sendMessage;
        private Sunny.UI.UIRichTextBox uiRichTextBox_receiveMessage;
        private Sunny.UI.UIButton uiButton_send;
        private Sunny.UI.UIButton QuickTestS1F11;
        private Sunny.UI.UIButton QuickTestS5F11;
        private Sunny.UI.UIButton QuickTestS1F3;
    }
}