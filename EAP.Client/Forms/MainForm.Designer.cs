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
            label_updatetime_aoi = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            uiButton_outputTrayCount = new UIButton();
            uiButton_inputTrayCount = new UIButton();
            uiLedLabel_outputTrayCount = new UILedLabel();
            uiLedLabel_inputTrayCount = new UILedLabel();
            uiCheckBox_agvLocked = new UICheckBox();
            uiCheckBox_agvEnabled = new UICheckBox();
            label_ProcessState = new UILabel();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            uiButton_swichAgvMode = new UIButton();
            groupBox_1.SuspendLayout();
            SuspendLayout();
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
            groupBox_1.Controls.Add(uiButton_swichAgvMode);
            groupBox_1.Controls.Add(uiButton_outputTrayCount);
            groupBox_1.Controls.Add(uiButton_inputTrayCount);
            groupBox_1.Controls.Add(uiLedLabel_outputTrayCount);
            groupBox_1.Controls.Add(uiLedLabel_inputTrayCount);
            groupBox_1.Controls.Add(uiCheckBox_agvLocked);
            groupBox_1.Controls.Add(uiCheckBox_agvEnabled);
            groupBox_1.Controls.Add(label_ProcessState);
            groupBox_1.Controls.Add(label_conn_status);
            groupBox_1.Controls.Add(label_updatetime_aoi);
            groupBox_1.Font = new Font("宋体", 12F);
            groupBox_1.Location = new Point(12, 40);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(590, 306);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "Info";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiButton_outputTrayCount
            // 
            uiButton_outputTrayCount.FillColor = Color.FromArgb(110, 190, 40);
            uiButton_outputTrayCount.FillColor2 = Color.FromArgb(110, 190, 40);
            uiButton_outputTrayCount.FillHoverColor = Color.FromArgb(139, 203, 83);
            uiButton_outputTrayCount.FillPressColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.FillSelectedColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_outputTrayCount.LightColor = Color.FromArgb(245, 251, 241);
            uiButton_outputTrayCount.Location = new Point(238, 161);
            uiButton_outputTrayCount.MinimumSize = new Size(1, 1);
            uiButton_outputTrayCount.Name = "uiButton_outputTrayCount";
            uiButton_outputTrayCount.RectColor = Color.FromArgb(110, 190, 40);
            uiButton_outputTrayCount.RectHoverColor = Color.FromArgb(139, 203, 83);
            uiButton_outputTrayCount.RectPressColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.RectSelectedColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.Size = new Size(100, 23);
            uiButton_outputTrayCount.Style = UIStyle.Custom;
            uiButton_outputTrayCount.TabIndex = 6;
            uiButton_outputTrayCount.Text = "出料口盘数";
            uiButton_outputTrayCount.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_outputTrayCount.Click += uiButton_outputTrayCount_Click;
            // 
            // uiButton_inputTrayCount
            // 
            uiButton_inputTrayCount.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_inputTrayCount.Location = new Point(15, 161);
            uiButton_inputTrayCount.MinimumSize = new Size(1, 1);
            uiButton_inputTrayCount.Name = "uiButton_inputTrayCount";
            uiButton_inputTrayCount.Size = new Size(100, 23);
            uiButton_inputTrayCount.TabIndex = 6;
            uiButton_inputTrayCount.Text = "入料口盘数";
            uiButton_inputTrayCount.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_inputTrayCount.Click += uiButton_inputTrayCount_Click;
            // 
            // uiLedLabel_outputTrayCount
            // 
            uiLedLabel_outputTrayCount.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLedLabel_outputTrayCount.ForeColor = Color.Lime;
            uiLedLabel_outputTrayCount.Location = new Point(238, 190);
            uiLedLabel_outputTrayCount.MinimumSize = new Size(1, 1);
            uiLedLabel_outputTrayCount.Name = "uiLedLabel_outputTrayCount";
            uiLedLabel_outputTrayCount.Size = new Size(100, 35);
            uiLedLabel_outputTrayCount.TabIndex = 5;
            uiLedLabel_outputTrayCount.Text = "0";
            // 
            // uiLedLabel_inputTrayCount
            // 
            uiLedLabel_inputTrayCount.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLedLabel_inputTrayCount.Location = new Point(15, 190);
            uiLedLabel_inputTrayCount.MinimumSize = new Size(1, 1);
            uiLedLabel_inputTrayCount.Name = "uiLedLabel_inputTrayCount";
            uiLedLabel_inputTrayCount.Size = new Size(100, 35);
            uiLedLabel_inputTrayCount.TabIndex = 5;
            uiLedLabel_inputTrayCount.Text = "0";
            // 
            // uiCheckBox_agvLocked
            // 
            uiCheckBox_agvLocked.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiCheckBox_agvLocked.ForeColor = Color.FromArgb(48, 48, 48);
            uiCheckBox_agvLocked.Location = new Point(238, 115);
            uiCheckBox_agvLocked.MinimumSize = new Size(1, 1);
            uiCheckBox_agvLocked.Name = "uiCheckBox_agvLocked";
            uiCheckBox_agvLocked.ReadOnly = true;
            uiCheckBox_agvLocked.Size = new Size(203, 29);
            uiCheckBox_agvLocked.TabIndex = 4;
            uiCheckBox_agvLocked.Text = "AGV锁定(禁止进出盘)";
            // 
            // uiCheckBox_agvEnabled
            // 
            uiCheckBox_agvEnabled.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiCheckBox_agvEnabled.ForeColor = Color.FromArgb(48, 48, 48);
            uiCheckBox_agvEnabled.Location = new Point(15, 115);
            uiCheckBox_agvEnabled.MinimumSize = new Size(1, 1);
            uiCheckBox_agvEnabled.Name = "uiCheckBox_agvEnabled";
            uiCheckBox_agvEnabled.ReadOnly = true;
            uiCheckBox_agvEnabled.Size = new Size(150, 29);
            uiCheckBox_agvEnabled.TabIndex = 4;
            uiCheckBox_agvEnabled.Text = "AGV模式";
            // 
            // label_ProcessState
            // 
            label_ProcessState.AutoSize = true;
            label_ProcessState.BackColor = Color.Gray;
            label_ProcessState.Font = new Font("Microsoft YaHei UI", 15F);
            label_ProcessState.ForeColor = Color.White;
            label_ProcessState.Location = new Point(178, 32);
            label_ProcessState.Name = "label_ProcessState";
            label_ProcessState.Size = new Size(103, 27);
            label_ProcessState.TabIndex = 3;
            label_ProcessState.Text = "Unknown";
            label_ProcessState.Visible = false;
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
            // uiButton_swichAgvMode
            // 
            uiButton_swichAgvMode.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_swichAgvMode.Location = new Point(15, 74);
            uiButton_swichAgvMode.MinimumSize = new Size(1, 1);
            uiButton_swichAgvMode.Name = "uiButton_swichAgvMode";
            uiButton_swichAgvMode.Size = new Size(100, 35);
            uiButton_swichAgvMode.Style = UIStyle.Custom;
            uiButton_swichAgvMode.TabIndex = 7;
            uiButton_swichAgvMode.Text = "开启AGV模式";
            uiButton_swichAgvMode.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_swichAgvMode.Click += uiButton_swichAgvMode_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(614, 609);
            Controls.Add(richTextBox1);
            Controls.Add(groupBox_1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            Text = "EAP Client";
            ZoomScaleRect = new Rectangle(15, 15, 614, 638);
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Shown += MainForm_Shown;
            SizeChanged += MainForm_SizeChanged;
            groupBox_1.ResumeLayout(false);
            groupBox_1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private UILabel label3;
        private UILabel label4;
        private UILabel label_updatetime_aoi;
        private UILabel label_conn_status;
        private UIButton button1;
        private UIButton button_getModelName;
        private UIGroupBox groupBox1;
        private UIGroupBox groupBox_1;
        private UILabel label_ProcessState;
        private UIRichTextBox richTextBox1;
        private NotifyIcon notifyIcon;
        private UICheckBox uiCheckBox_agvEnabled;
        private UICheckBox uiCheckBox_agvLocked;
        private UILedLabel uiLedLabel_outputTrayCount;
        private UILedLabel uiLedLabel_inputTrayCount;
        private UIButton uiButton_outputTrayCount;
        private UIButton uiButton_inputTrayCount;
        private UIButton uiButton_swichAgvMode;
    }
}