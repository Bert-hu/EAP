using Sunny.UI;

namespace EAP.Client.Forms
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            textBox_panelid = new UITextBox();
            label_1 = new UILabel();
            label1 = new UILabel();
            textBox_modelname = new UITextBox();
            label_updatetime_aoi = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            uiButton_Test = new UIButton();
            groupBox_1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_panelid
            // 
            textBox_panelid.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_panelid.Location = new Point(202, 119);
            textBox_panelid.Margin = new Padding(4, 5, 4, 5);
            textBox_panelid.MinimumSize = new Size(1, 16);
            textBox_panelid.Name = "textBox_panelid";
            textBox_panelid.Padding = new Padding(5);
            textBox_panelid.ShowText = false;
            textBox_panelid.Size = new Size(256, 33);
            textBox_panelid.TabIndex = 0;
            textBox_panelid.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_panelid.Watermark = "";
            // 
            // label_1
            // 
            label_1.AutoSize = true;
            label_1.Font = new Font("Microsoft YaHei UI", 15F);
            label_1.ForeColor = Color.FromArgb(48, 48, 48);
            label_1.Location = new Point(15, 122);
            label_1.Name = "label_1";
            label_1.Size = new Size(90, 27);
            label_1.TabIndex = 1;
            label_1.Text = "Panel ID";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 15F);
            label1.ForeColor = Color.FromArgb(48, 48, 48);
            label1.Location = new Point(15, 170);
            label1.Name = "label1";
            label1.Size = new Size(137, 27);
            label1.TabIndex = 1;
            label1.Text = "Model Name";
            // 
            // textBox_modelname
            // 
            textBox_modelname.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_modelname.Location = new Point(202, 167);
            textBox_modelname.Margin = new Padding(4, 5, 4, 5);
            textBox_modelname.MinimumSize = new Size(1, 16);
            textBox_modelname.Name = "textBox_modelname";
            textBox_modelname.Padding = new Padding(5);
            textBox_modelname.ShowText = false;
            textBox_modelname.Size = new Size(256, 33);
            textBox_modelname.TabIndex = 0;
            textBox_modelname.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_modelname.Watermark = "";
            // 
            // label_updatetime_aoi
            // 
            label_updatetime_aoi.AutoSize = true;
            label_updatetime_aoi.Font = new Font("宋体", 12F);
            label_updatetime_aoi.ForeColor = Color.FromArgb(48, 48, 48);
            label_updatetime_aoi.Location = new Point(296, 40);
            label_updatetime_aoi.Name = "label_updatetime_aoi";
            label_updatetime_aoi.Size = new Size(103, 16);
            label_updatetime_aoi.TabIndex = 2;
            label_updatetime_aoi.Text = "Update Time:";
            // 
            // label_conn_status
            // 
            label_conn_status.AutoSize = true;
            label_conn_status.BackColor = Color.Gray;
            label_conn_status.Font = new Font("Microsoft YaHei UI", 15F);
            label_conn_status.ForeColor = Color.White;
            label_conn_status.Location = new Point(21, 32);
            label_conn_status.Name = "label_conn_status";
            label_conn_status.Size = new Size(120, 27);
            label_conn_status.TabIndex = 3;
            label_conn_status.Text = "Connecting";
            // 
            // groupBox_1
            // 
            groupBox_1.Controls.Add(label_conn_status);
            groupBox_1.Controls.Add(textBox_panelid);
            groupBox_1.Controls.Add(textBox_modelname);
            groupBox_1.Controls.Add(label_updatetime_aoi);
            groupBox_1.Controls.Add(label_1);
            groupBox_1.Controls.Add(label1);
            groupBox_1.Font = new Font("宋体", 12F);
            groupBox_1.Location = new Point(12, 40);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(471, 302);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "Info";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // richTextBox1
            // 
            richTextBox1.FillColor = Color.White;
            richTextBox1.Font = new Font("宋体", 10F);
            richTextBox1.Location = new Point(14, 368);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(2);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(588, 223);
            richTextBox1.TabIndex = 8;
            richTextBox1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "EAP.Client";
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            // 
            // uiButton_Test
            // 
            uiButton_Test.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_Test.Location = new Point(490, 142);
            uiButton_Test.MinimumSize = new Size(1, 1);
            uiButton_Test.Name = "uiButton_Test";
            uiButton_Test.Size = new Size(100, 35);
            uiButton_Test.TabIndex = 9;
            uiButton_Test.Text = "Test";
            uiButton_Test.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_Test.Click += uiButton_Test_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(614, 609);
            Controls.Add(uiButton_Test);
            Controls.Add(richTextBox1);
            Controls.Add(groupBox_1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            Text = "EAP Client";
            ZoomScaleRect = new Rectangle(15, 15, 614, 638);
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            groupBox_1.ResumeLayout(false);
            groupBox_1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private UITextBox textBox_panelid;
        private UILabel label_1;
        private UILabel label1;
        private UITextBox textBox_modelname;
        private UILabel label3;
        private UILabel label4;
        private UILabel label_updatetime_aoi;
        private UILabel label_conn_status;
        private UIButton button1;
        private UIButton button_getModelName;
        private UIGroupBox groupBox1;
        private UIGroupBox groupBox_1;
        private UIRichTextBox richTextBox1;
        private NotifyIcon notifyIcon;
        private UIButton uiButton_Test;
    }
}