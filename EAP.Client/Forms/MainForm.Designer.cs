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
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            uiButton_refresh = new UIButton();
            uiSymbolButton_updateGroupName = new UISymbolButton();
            uiSymbolButton_updateMaterialName = new UISymbolButton();
            uiSymbolButton_updateLot = new UISymbolButton();
            uiButton_unlockAgv = new UIButton();
            uiTextBox_groupName = new UITextBox();
            uiTextBox_materialName = new UITextBox();
            uiTextBox_currentLot = new UITextBox();
            uiButton_lockAgv = new UIButton();
            uiLabel_currenttaskState = new UILabel();
            uiButton_sendInputOutputTask = new UIButton();
            uiButton_sendOutputTask = new UIButton();
            uiButton_sendInputTask = new UIButton();
            uiButton_swichAgvMode = new UIButton();
            uiButton_outputTrayCount = new UIButton();
            uiButton_inputTrayCount = new UIButton();
            uiLedLabel_outputTrayCount = new UILedLabel();
            uiLedLabel_inputTrayCount = new UILedLabel();
            uiCheckBox_agvLocked = new UICheckBox();
            uiCheckBox_agvEnabled = new UICheckBox();
            label_ProcessState = new UILabel();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            groupBox_1.SuspendLayout();
            SuspendLayout();
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
            groupBox_1.Controls.Add(uiButton_refresh);
            groupBox_1.Controls.Add(uiSymbolButton_updateGroupName);
            groupBox_1.Controls.Add(uiSymbolButton_updateMaterialName);
            groupBox_1.Controls.Add(uiSymbolButton_updateLot);
            groupBox_1.Controls.Add(uiButton_unlockAgv);
            groupBox_1.Controls.Add(uiTextBox_groupName);
            groupBox_1.Controls.Add(uiTextBox_materialName);
            groupBox_1.Controls.Add(uiTextBox_currentLot);
            groupBox_1.Controls.Add(uiButton_lockAgv);
            groupBox_1.Controls.Add(uiLabel_currenttaskState);
            groupBox_1.Controls.Add(uiButton_sendInputOutputTask);
            groupBox_1.Controls.Add(uiButton_sendOutputTask);
            groupBox_1.Controls.Add(uiButton_sendInputTask);
            groupBox_1.Controls.Add(uiButton_swichAgvMode);
            groupBox_1.Controls.Add(uiButton_outputTrayCount);
            groupBox_1.Controls.Add(uiButton_inputTrayCount);
            groupBox_1.Controls.Add(uiLedLabel_outputTrayCount);
            groupBox_1.Controls.Add(uiLedLabel_inputTrayCount);
            groupBox_1.Controls.Add(uiCheckBox_agvLocked);
            groupBox_1.Controls.Add(uiCheckBox_agvEnabled);
            groupBox_1.Controls.Add(label_ProcessState);
            groupBox_1.Controls.Add(label_conn_status);
            groupBox_1.Font = new Font("宋体", 12F);
            groupBox_1.Location = new Point(10, 40);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(539, 285);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "Info";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiButton_refresh
            // 
            uiButton_refresh.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_refresh.Location = new Point(473, 26);
            uiButton_refresh.MinimumSize = new Size(1, 1);
            uiButton_refresh.Name = "uiButton_refresh";
            uiButton_refresh.Size = new Size(56, 35);
            uiButton_refresh.TabIndex = 13;
            uiButton_refresh.Text = "刷新";
            uiButton_refresh.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_refresh.Click += uiButton_refresh_Click;
            // 
            // uiSymbolButton_updateGroupName
            // 
            uiSymbolButton_updateGroupName.FillColor = Color.FromArgb(0, 150, 136);
            uiSymbolButton_updateGroupName.FillColor2 = Color.FromArgb(0, 150, 136);
            uiSymbolButton_updateGroupName.FillHoverColor = Color.FromArgb(51, 171, 160);
            uiSymbolButton_updateGroupName.FillPressColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateGroupName.FillSelectedColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateGroupName.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateGroupName.LightColor = Color.FromArgb(238, 248, 248);
            uiSymbolButton_updateGroupName.Location = new Point(309, 73);
            uiSymbolButton_updateGroupName.MinimumSize = new Size(1, 1);
            uiSymbolButton_updateGroupName.Name = "uiSymbolButton_updateGroupName";
            uiSymbolButton_updateGroupName.RectColor = Color.FromArgb(0, 150, 136);
            uiSymbolButton_updateGroupName.RectHoverColor = Color.FromArgb(51, 171, 160);
            uiSymbolButton_updateGroupName.RectPressColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateGroupName.RectSelectedColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateGroupName.Size = new Size(70, 35);
            uiSymbolButton_updateGroupName.Style = UIStyle.Custom;
            uiSymbolButton_updateGroupName.Symbol = 0;
            uiSymbolButton_updateGroupName.TabIndex = 12;
            uiSymbolButton_updateGroupName.Text = "Group";
            uiSymbolButton_updateGroupName.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateGroupName.Click += uiSymbolButton_updateGroupName_Click;
            // 
            // uiSymbolButton_updateMaterialName
            // 
            uiSymbolButton_updateMaterialName.FillColor = Color.FromArgb(0, 150, 136);
            uiSymbolButton_updateMaterialName.FillColor2 = Color.FromArgb(0, 150, 136);
            uiSymbolButton_updateMaterialName.FillHoverColor = Color.FromArgb(51, 171, 160);
            uiSymbolButton_updateMaterialName.FillPressColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateMaterialName.FillSelectedColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateMaterialName.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateMaterialName.LightColor = Color.FromArgb(238, 248, 248);
            uiSymbolButton_updateMaterialName.Location = new Point(21, 74);
            uiSymbolButton_updateMaterialName.MinimumSize = new Size(1, 1);
            uiSymbolButton_updateMaterialName.Name = "uiSymbolButton_updateMaterialName";
            uiSymbolButton_updateMaterialName.RectColor = Color.FromArgb(0, 150, 136);
            uiSymbolButton_updateMaterialName.RectHoverColor = Color.FromArgb(51, 171, 160);
            uiSymbolButton_updateMaterialName.RectPressColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateMaterialName.RectSelectedColor = Color.FromArgb(0, 120, 109);
            uiSymbolButton_updateMaterialName.Size = new Size(70, 35);
            uiSymbolButton_updateMaterialName.Style = UIStyle.Custom;
            uiSymbolButton_updateMaterialName.Symbol = 0;
            uiSymbolButton_updateMaterialName.TabIndex = 12;
            uiSymbolButton_updateMaterialName.Text = "Material";
            uiSymbolButton_updateMaterialName.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateMaterialName.Click += uiSymbolButton_updateMaterialName_Click;
            // 
            // uiSymbolButton_updateLot
            // 
            uiSymbolButton_updateLot.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateLot.Location = new Point(21, 114);
            uiSymbolButton_updateLot.MinimumSize = new Size(1, 1);
            uiSymbolButton_updateLot.Name = "uiSymbolButton_updateLot";
            uiSymbolButton_updateLot.Size = new Size(70, 35);
            uiSymbolButton_updateLot.Symbol = 61561;
            uiSymbolButton_updateLot.TabIndex = 12;
            uiSymbolButton_updateLot.Text = "LOT";
            uiSymbolButton_updateLot.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateLot.Click += uiSymbolButton_updateLot_Click;
            // 
            // uiButton_unlockAgv
            // 
            uiButton_unlockAgv.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_unlockAgv.Location = new Point(287, 228);
            uiButton_unlockAgv.MinimumSize = new Size(1, 1);
            uiButton_unlockAgv.Name = "uiButton_unlockAgv";
            uiButton_unlockAgv.Size = new Size(42, 23);
            uiButton_unlockAgv.TabIndex = 10;
            uiButton_unlockAgv.Text = "解锁";
            uiButton_unlockAgv.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_unlockAgv.Click += uiButton_unlockAgv_Click;
            // 
            // uiTextBox_groupName
            // 
            uiTextBox_groupName.Font = new Font("微软雅黑", 11F, FontStyle.Bold);
            uiTextBox_groupName.Location = new Point(386, 74);
            uiTextBox_groupName.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_groupName.MinimumSize = new Size(1, 16);
            uiTextBox_groupName.Name = "uiTextBox_groupName";
            uiTextBox_groupName.Padding = new Padding(5);
            uiTextBox_groupName.ReadOnly = true;
            uiTextBox_groupName.ShowText = false;
            uiTextBox_groupName.Size = new Size(143, 35);
            uiTextBox_groupName.TabIndex = 11;
            uiTextBox_groupName.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_groupName.Watermark = "";
            // 
            // uiTextBox_materialName
            // 
            uiTextBox_materialName.Font = new Font("微软雅黑", 11F, FontStyle.Bold);
            uiTextBox_materialName.Location = new Point(98, 74);
            uiTextBox_materialName.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_materialName.MinimumSize = new Size(1, 16);
            uiTextBox_materialName.Name = "uiTextBox_materialName";
            uiTextBox_materialName.Padding = new Padding(5);
            uiTextBox_materialName.ReadOnly = true;
            uiTextBox_materialName.ShowText = false;
            uiTextBox_materialName.Size = new Size(204, 35);
            uiTextBox_materialName.TabIndex = 11;
            uiTextBox_materialName.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_materialName.Watermark = "";
            // 
            // uiTextBox_currentLot
            // 
            uiTextBox_currentLot.Font = new Font("微软雅黑", 11F, FontStyle.Bold);
            uiTextBox_currentLot.Location = new Point(98, 114);
            uiTextBox_currentLot.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_currentLot.MinimumSize = new Size(1, 16);
            uiTextBox_currentLot.Name = "uiTextBox_currentLot";
            uiTextBox_currentLot.Padding = new Padding(5);
            uiTextBox_currentLot.ReadOnly = true;
            uiTextBox_currentLot.ShowText = false;
            uiTextBox_currentLot.Size = new Size(431, 35);
            uiTextBox_currentLot.TabIndex = 11;
            uiTextBox_currentLot.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_currentLot.Watermark = "";
            // 
            // uiButton_lockAgv
            // 
            uiButton_lockAgv.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_lockAgv.Location = new Point(239, 228);
            uiButton_lockAgv.MinimumSize = new Size(1, 1);
            uiButton_lockAgv.Name = "uiButton_lockAgv";
            uiButton_lockAgv.Size = new Size(42, 23);
            uiButton_lockAgv.TabIndex = 10;
            uiButton_lockAgv.Text = "锁定";
            uiButton_lockAgv.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_lockAgv.Click += uiButton_lockAgv_Click;
            // 
            // uiLabel_currenttaskState
            // 
            uiLabel_currenttaskState.Font = new Font("微软雅黑", 15F, FontStyle.Bold);
            uiLabel_currenttaskState.ForeColor = Color.FromArgb(0, 0, 192);
            uiLabel_currenttaskState.Location = new Point(335, 228);
            uiLabel_currenttaskState.Name = "uiLabel_currenttaskState";
            uiLabel_currenttaskState.Size = new Size(201, 35);
            uiLabel_currenttaskState.TabIndex = 9;
            uiLabel_currenttaskState.Text = "无AGV任务";
            uiLabel_currenttaskState.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // uiButton_sendInputOutputTask
            // 
            uiButton_sendInputOutputTask.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendInputOutputTask.Location = new Point(365, 161);
            uiButton_sendInputOutputTask.MinimumSize = new Size(1, 1);
            uiButton_sendInputOutputTask.Name = "uiButton_sendInputOutputTask";
            uiButton_sendInputOutputTask.Size = new Size(164, 35);
            uiButton_sendInputOutputTask.TabIndex = 8;
            uiButton_sendInputOutputTask.Text = "发送InputOutput任务";
            uiButton_sendInputOutputTask.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendInputOutputTask.Click += uiButton_sendInputOutputTask_Click;
            // 
            // uiButton_sendOutputTask
            // 
            uiButton_sendOutputTask.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendOutputTask.Location = new Point(193, 161);
            uiButton_sendOutputTask.MinimumSize = new Size(1, 1);
            uiButton_sendOutputTask.Name = "uiButton_sendOutputTask";
            uiButton_sendOutputTask.Size = new Size(164, 35);
            uiButton_sendOutputTask.TabIndex = 8;
            uiButton_sendOutputTask.Text = "发送Output任务";
            uiButton_sendOutputTask.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendOutputTask.Click += uiButton_sendOutputTask_Click;
            // 
            // uiButton_sendInputTask
            // 
            uiButton_sendInputTask.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendInputTask.Location = new Point(21, 161);
            uiButton_sendInputTask.MinimumSize = new Size(1, 1);
            uiButton_sendInputTask.Name = "uiButton_sendInputTask";
            uiButton_sendInputTask.Size = new Size(164, 35);
            uiButton_sendInputTask.TabIndex = 8;
            uiButton_sendInputTask.Text = "发送Input任务";
            uiButton_sendInputTask.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendInputTask.Click += uiButton_sendInputTask_Click;
            // 
            // uiButton_swichAgvMode
            // 
            uiButton_swichAgvMode.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_swichAgvMode.Location = new Point(270, 26);
            uiButton_swichAgvMode.MinimumSize = new Size(1, 1);
            uiButton_swichAgvMode.Name = "uiButton_swichAgvMode";
            uiButton_swichAgvMode.Size = new Size(100, 35);
            uiButton_swichAgvMode.Style = UIStyle.Custom;
            uiButton_swichAgvMode.TabIndex = 7;
            uiButton_swichAgvMode.Text = "开启AGV模式";
            uiButton_swichAgvMode.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_swichAgvMode.Click += uiButton_swichAgvMode_Click;
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
            uiButton_outputTrayCount.Location = new Point(127, 202);
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
            uiButton_inputTrayCount.Location = new Point(21, 202);
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
            uiLedLabel_outputTrayCount.Location = new Point(127, 231);
            uiLedLabel_outputTrayCount.MinimumSize = new Size(1, 1);
            uiLedLabel_outputTrayCount.Name = "uiLedLabel_outputTrayCount";
            uiLedLabel_outputTrayCount.Size = new Size(100, 35);
            uiLedLabel_outputTrayCount.TabIndex = 5;
            uiLedLabel_outputTrayCount.Text = "0";
            // 
            // uiLedLabel_inputTrayCount
            // 
            uiLedLabel_inputTrayCount.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLedLabel_inputTrayCount.Location = new Point(21, 231);
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
            uiCheckBox_agvLocked.Location = new Point(237, 202);
            uiCheckBox_agvLocked.MinimumSize = new Size(1, 1);
            uiCheckBox_agvLocked.Name = "uiCheckBox_agvLocked";
            uiCheckBox_agvLocked.ReadOnly = true;
            uiCheckBox_agvLocked.Size = new Size(180, 29);
            uiCheckBox_agvLocked.TabIndex = 4;
            uiCheckBox_agvLocked.Text = "锁定禁止进出盘";
            // 
            // uiCheckBox_agvEnabled
            // 
            uiCheckBox_agvEnabled.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiCheckBox_agvEnabled.ForeColor = Color.FromArgb(48, 48, 48);
            uiCheckBox_agvEnabled.Location = new Point(376, 26);
            uiCheckBox_agvEnabled.MinimumSize = new Size(1, 1);
            uiCheckBox_agvEnabled.Name = "uiCheckBox_agvEnabled";
            uiCheckBox_agvEnabled.ReadOnly = true;
            uiCheckBox_agvEnabled.Size = new Size(106, 29);
            uiCheckBox_agvEnabled.TabIndex = 4;
            uiCheckBox_agvEnabled.Text = "AGV模式";
            // 
            // label_ProcessState
            // 
            label_ProcessState.AutoSize = true;
            label_ProcessState.BackColor = Color.Gray;
            label_ProcessState.Font = new Font("Microsoft YaHei UI", 15F);
            label_ProcessState.ForeColor = Color.White;
            label_ProcessState.Location = new Point(150, 32);
            label_ProcessState.Name = "label_ProcessState";
            label_ProcessState.Size = new Size(103, 27);
            label_ProcessState.TabIndex = 3;
            label_ProcessState.Text = "Unknown";
            // 
            // richTextBox1
            // 
            richTextBox1.FillColor = Color.White;
            richTextBox1.Font = new Font("宋体", 10F);
            richTextBox1.Location = new Point(12, 335);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(2);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(539, 230);
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
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(565, 579);
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
        private UIButton uiButton_sendInputOutputTask;
        private UIButton uiButton_sendOutputTask;
        private UIButton uiButton_sendInputTask;
        private UILabel uiLabel_currenttaskState;
        private UIButton uiButton_unlockAgv;
        private UIButton uiButton_lockAgv;
        private UITextBox uiTextBox_currentLot;
        private UISymbolButton uiSymbolButton_updateGroupName;
        private UISymbolButton uiSymbolButton_updateMaterialName;
        private UISymbolButton uiSymbolButton_updateLot;
        private UITextBox uiTextBox_groupName;
        private UITextBox uiTextBox_materialName;
        private UIButton uiButton_refresh;
    }
}