using Sunny.UI;
using System.Drawing;
using System.Windows.Forms;

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
            topPanel = new UIPanel();
            label_conn_status = new UILabel();
            label_ProcessState = new UILabel();
            uiButton_swichAgvMode = new UIButton();
            uiCheckBox_agvEnabled = new UICheckBox();
            uiButton_refresh = new UIButton();
            groupBox_1 = new UIGroupBox();
            uiSymbolButton_updateMaterialName = new UISymbolButton();
            uiTextBox_materialName = new UITextBox();
            uiSymbolButton_updateGroupName = new UISymbolButton();
            uiTextBox_groupName = new UITextBox();
            uiSymbolButton_updateLot = new UISymbolButton();
            uiTextBox_currentLot = new UITextBox();
            taskLabel = new UILabel();
            uiButton_sendInputTask = new UIButton();
            uiButton_sendOutputTask = new UIButton();
            uiButton_sendInputOutputTask = new UIButton();
            uiLabel_currenttaskState = new UILabel();
            countAndLockGroup = new UIGroupBox();
            uiButton_inputTrayCount = new UIButton();
            uiLedLabel_inputTrayCount = new UILedLabel();
            uiButton_outputTrayCount = new UIButton();
            uiLedLabel_outputTrayCount = new UILedLabel();
            uiCheckBox_agvLocked = new UICheckBox();
            uiButton_lockAgv = new UIButton();
            uiButton_unlockAgv = new UIButton();
            uiCheckBox_loaderEmpty = new UICheckBox();
            inventoryGroup = new UIGroupBox();
            uiLabel_agvInventoryTitle = new UILabel();
            uiLedLabel_agvInventory = new UILedLabel();
            uiLedLabel_stockerInventory = new UILedLabel();
            uiLabel_stockerInventoryTitle = new UILabel();
            uiLabel_taskRequestTime = new UILabel();
            logGroup = new UIGroupBox();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            uiButton_loaderEmpty = new UIButton();
            topPanel.SuspendLayout();
            groupBox_1.SuspendLayout();
            countAndLockGroup.SuspendLayout();
            inventoryGroup.SuspendLayout();
            logGroup.SuspendLayout();
            SuspendLayout();
            // 
            // topPanel
            // 
            topPanel.Controls.Add(label_conn_status);
            topPanel.Controls.Add(label_ProcessState);
            topPanel.Dock = DockStyle.Top;
            topPanel.FillColor = Color.White;
            topPanel.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            topPanel.Location = new Point(0, 35);
            topPanel.Margin = new Padding(4, 5, 4, 5);
            topPanel.MinimumSize = new Size(1, 1);
            topPanel.Name = "topPanel";
            topPanel.RectColor = Color.FromArgb(220, 223, 230);
            topPanel.Size = new Size(800, 60);
            topPanel.TabIndex = 0;
            topPanel.Text = null;
            topPanel.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // label_conn_status
            // 
            label_conn_status.AutoSize = true;
            label_conn_status.BackColor = Color.Transparent;
            label_conn_status.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            label_conn_status.ForeColor = Color.FromArgb(54, 59, 69);
            label_conn_status.Location = new Point(20, 20);
            label_conn_status.Name = "label_conn_status";
            label_conn_status.Size = new Size(102, 22);
            label_conn_status.TabIndex = 3;
            label_conn_status.Text = "Connecting";
            // 
            // label_ProcessState
            // 
            label_ProcessState.AutoSize = true;
            label_ProcessState.BackColor = Color.Transparent;
            label_ProcessState.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            label_ProcessState.ForeColor = Color.FromArgb(54, 59, 69);
            label_ProcessState.Location = new Point(150, 20);
            label_ProcessState.Name = "label_ProcessState";
            label_ProcessState.Size = new Size(87, 22);
            label_ProcessState.TabIndex = 3;
            label_ProcessState.Text = "Unknown";
            // 
            // uiButton_swichAgvMode
            // 
            uiButton_swichAgvMode.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_swichAgvMode.Location = new Point(15, 38);
            uiButton_swichAgvMode.MinimumSize = new Size(1, 1);
            uiButton_swichAgvMode.Name = "uiButton_swichAgvMode";
            uiButton_swichAgvMode.Size = new Size(120, 35);
            uiButton_swichAgvMode.Style = UIStyle.Custom;
            uiButton_swichAgvMode.TabIndex = 7;
            uiButton_swichAgvMode.Text = "开启AGV模式";
            uiButton_swichAgvMode.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_swichAgvMode.Click += uiButton_swichAgvMode_Click;
            // 
            // uiCheckBox_agvEnabled
            // 
            uiCheckBox_agvEnabled.Font = new Font("Microsoft YaHei UI", 10F);
            uiCheckBox_agvEnabled.ForeColor = Color.FromArgb(54, 59, 69);
            uiCheckBox_agvEnabled.Location = new Point(145, 43);
            uiCheckBox_agvEnabled.MinimumSize = new Size(1, 1);
            uiCheckBox_agvEnabled.Name = "uiCheckBox_agvEnabled";
            uiCheckBox_agvEnabled.ReadOnly = true;
            uiCheckBox_agvEnabled.Size = new Size(90, 25);
            uiCheckBox_agvEnabled.TabIndex = 4;
            uiCheckBox_agvEnabled.Text = "AGV模式";
            // 
            // uiButton_refresh
            // 
            uiButton_refresh.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_refresh.Location = new Point(666, 38);
            uiButton_refresh.MinimumSize = new Size(1, 1);
            uiButton_refresh.Name = "uiButton_refresh";
            uiButton_refresh.Size = new Size(80, 35);
            uiButton_refresh.Style = UIStyle.Custom;
            uiButton_refresh.TabIndex = 13;
            uiButton_refresh.Text = "刷新";
            uiButton_refresh.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_refresh.Click += uiButton_refresh_Click;
            // 
            // groupBox_1
            // 
            groupBox_1.Controls.Add(uiSymbolButton_updateMaterialName);
            groupBox_1.Controls.Add(uiTextBox_materialName);
            groupBox_1.Controls.Add(uiSymbolButton_updateGroupName);
            groupBox_1.Controls.Add(uiTextBox_groupName);
            groupBox_1.Controls.Add(uiSymbolButton_updateLot);
            groupBox_1.Controls.Add(uiTextBox_currentLot);
            groupBox_1.FillColor = Color.White;
            groupBox_1.Font = new Font("Microsoft YaHei UI", 10F);
            groupBox_1.Location = new Point(20, 286);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(15, 35, 15, 15);
            groupBox_1.RectColor = Color.FromArgb(220, 223, 230);
            groupBox_1.Size = new Size(765, 138);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "物料信息";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiSymbolButton_updateMaterialName
            // 
            uiSymbolButton_updateMaterialName.FillColor = Color.FromArgb(64, 158, 255);
            uiSymbolButton_updateMaterialName.FillColor2 = Color.FromArgb(64, 158, 255);
            uiSymbolButton_updateMaterialName.FillHoverColor = Color.FromArgb(84, 171, 255);
            uiSymbolButton_updateMaterialName.FillPressColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateMaterialName.FillSelectedColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateMaterialName.Font = new Font("Microsoft YaHei UI", 10F);
            uiSymbolButton_updateMaterialName.LightColor = Color.FromArgb(230, 242, 255);
            uiSymbolButton_updateMaterialName.Location = new Point(18, 38);
            uiSymbolButton_updateMaterialName.MinimumSize = new Size(1, 1);
            uiSymbolButton_updateMaterialName.Name = "uiSymbolButton_updateMaterialName";
            uiSymbolButton_updateMaterialName.RectColor = Color.FromArgb(64, 158, 255);
            uiSymbolButton_updateMaterialName.RectHoverColor = Color.FromArgb(84, 171, 255);
            uiSymbolButton_updateMaterialName.RectPressColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateMaterialName.RectSelectedColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateMaterialName.Size = new Size(100, 35);
            uiSymbolButton_updateMaterialName.Style = UIStyle.Custom;
            uiSymbolButton_updateMaterialName.Symbol = 0;
            uiSymbolButton_updateMaterialName.TabIndex = 12;
            uiSymbolButton_updateMaterialName.Text = "物料名称";
            uiSymbolButton_updateMaterialName.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateMaterialName.Click += uiSymbolButton_updateMaterialName_Click;
            // 
            // uiTextBox_materialName
            // 
            uiTextBox_materialName.Font = new Font("Microsoft YaHei UI", 10F);
            uiTextBox_materialName.Location = new Point(128, 38);
            uiTextBox_materialName.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_materialName.MinimumSize = new Size(1, 16);
            uiTextBox_materialName.Name = "uiTextBox_materialName";
            uiTextBox_materialName.Padding = new Padding(5);
            uiTextBox_materialName.ReadOnly = true;
            uiTextBox_materialName.ShowText = false;
            uiTextBox_materialName.Size = new Size(346, 35);
            uiTextBox_materialName.TabIndex = 11;
            uiTextBox_materialName.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_materialName.Watermark = "";
            // 
            // uiSymbolButton_updateGroupName
            // 
            uiSymbolButton_updateGroupName.FillColor = Color.FromArgb(64, 158, 255);
            uiSymbolButton_updateGroupName.FillColor2 = Color.FromArgb(64, 158, 255);
            uiSymbolButton_updateGroupName.FillHoverColor = Color.FromArgb(84, 171, 255);
            uiSymbolButton_updateGroupName.FillPressColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateGroupName.FillSelectedColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateGroupName.Font = new Font("Microsoft YaHei UI", 10F);
            uiSymbolButton_updateGroupName.LightColor = Color.FromArgb(230, 242, 255);
            uiSymbolButton_updateGroupName.Location = new Point(481, 38);
            uiSymbolButton_updateGroupName.MinimumSize = new Size(1, 1);
            uiSymbolButton_updateGroupName.Name = "uiSymbolButton_updateGroupName";
            uiSymbolButton_updateGroupName.RectColor = Color.FromArgb(64, 158, 255);
            uiSymbolButton_updateGroupName.RectHoverColor = Color.FromArgb(84, 171, 255);
            uiSymbolButton_updateGroupName.RectPressColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateGroupName.RectSelectedColor = Color.FromArgb(46, 137, 255);
            uiSymbolButton_updateGroupName.Size = new Size(100, 35);
            uiSymbolButton_updateGroupName.Style = UIStyle.Custom;
            uiSymbolButton_updateGroupName.Symbol = 0;
            uiSymbolButton_updateGroupName.TabIndex = 12;
            uiSymbolButton_updateGroupName.Text = "站别";
            uiSymbolButton_updateGroupName.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateGroupName.Click += uiSymbolButton_updateGroupName_Click;
            // 
            // uiTextBox_groupName
            // 
            uiTextBox_groupName.Font = new Font("Microsoft YaHei UI", 10F);
            uiTextBox_groupName.Location = new Point(588, 38);
            uiTextBox_groupName.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_groupName.MinimumSize = new Size(1, 16);
            uiTextBox_groupName.Name = "uiTextBox_groupName";
            uiTextBox_groupName.Padding = new Padding(5);
            uiTextBox_groupName.ReadOnly = true;
            uiTextBox_groupName.ShowText = false;
            uiTextBox_groupName.Size = new Size(160, 35);
            uiTextBox_groupName.TabIndex = 11;
            uiTextBox_groupName.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_groupName.Watermark = "";
            // 
            // uiSymbolButton_updateLot
            // 
            uiSymbolButton_updateLot.Font = new Font("Microsoft YaHei UI", 10F);
            uiSymbolButton_updateLot.Location = new Point(18, 83);
            uiSymbolButton_updateLot.MinimumSize = new Size(1, 1);
            uiSymbolButton_updateLot.Name = "uiSymbolButton_updateLot";
            uiSymbolButton_updateLot.Size = new Size(100, 35);
            uiSymbolButton_updateLot.Symbol = 61561;
            uiSymbolButton_updateLot.TabIndex = 12;
            uiSymbolButton_updateLot.Text = "LOT";
            uiSymbolButton_updateLot.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_updateLot.Click += uiSymbolButton_updateLot_Click;
            // 
            // uiTextBox_currentLot
            // 
            uiTextBox_currentLot.Font = new Font("Microsoft YaHei UI", 10F);
            uiTextBox_currentLot.Location = new Point(128, 83);
            uiTextBox_currentLot.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_currentLot.MinimumSize = new Size(1, 16);
            uiTextBox_currentLot.Name = "uiTextBox_currentLot";
            uiTextBox_currentLot.Padding = new Padding(5);
            uiTextBox_currentLot.ReadOnly = true;
            uiTextBox_currentLot.ShowText = false;
            uiTextBox_currentLot.Size = new Size(620, 35);
            uiTextBox_currentLot.TabIndex = 11;
            uiTextBox_currentLot.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_currentLot.Watermark = "";
            // 
            // taskLabel
            // 
            taskLabel.AutoSize = true;
            taskLabel.BackColor = Color.Transparent;
            taskLabel.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            taskLabel.ForeColor = Color.FromArgb(54, 59, 69);
            taskLabel.Location = new Point(15, 86);
            taskLabel.Name = "taskLabel";
            taskLabel.Size = new Size(65, 19);
            taskLabel.TabIndex = 3;
            taskLabel.Text = "任务操作";
            // 
            // uiButton_sendInputTask
            // 
            uiButton_sendInputTask.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_sendInputTask.Location = new Point(15, 111);
            uiButton_sendInputTask.MinimumSize = new Size(1, 1);
            uiButton_sendInputTask.Name = "uiButton_sendInputTask";
            uiButton_sendInputTask.Size = new Size(149, 40);
            uiButton_sendInputTask.Style = UIStyle.Custom;
            uiButton_sendInputTask.TabIndex = 8;
            uiButton_sendInputTask.Text = "发送Input任务";
            uiButton_sendInputTask.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendInputTask.Click += uiButton_sendInputTask_Click;
            // 
            // uiButton_sendOutputTask
            // 
            uiButton_sendOutputTask.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_sendOutputTask.Location = new Point(181, 111);
            uiButton_sendOutputTask.MinimumSize = new Size(1, 1);
            uiButton_sendOutputTask.Name = "uiButton_sendOutputTask";
            uiButton_sendOutputTask.Size = new Size(149, 40);
            uiButton_sendOutputTask.Style = UIStyle.Custom;
            uiButton_sendOutputTask.TabIndex = 8;
            uiButton_sendOutputTask.Text = "发送Output任务";
            uiButton_sendOutputTask.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendOutputTask.Click += uiButton_sendOutputTask_Click;
            // 
            // uiButton_sendInputOutputTask
            // 
            uiButton_sendInputOutputTask.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_sendInputOutputTask.Location = new Point(351, 111);
            uiButton_sendInputOutputTask.MinimumSize = new Size(1, 1);
            uiButton_sendInputOutputTask.Name = "uiButton_sendInputOutputTask";
            uiButton_sendInputOutputTask.Size = new Size(149, 40);
            uiButton_sendInputOutputTask.Style = UIStyle.Custom;
            uiButton_sendInputOutputTask.TabIndex = 8;
            uiButton_sendInputOutputTask.Text = "发送InputOutput任务";
            uiButton_sendInputOutputTask.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_sendInputOutputTask.Click += uiButton_sendInputOutputTask_Click;
            // 
            // uiLabel_currenttaskState
            // 
            uiLabel_currenttaskState.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            uiLabel_currenttaskState.ForeColor = Color.FromArgb(46, 137, 255);
            uiLabel_currenttaskState.Location = new Point(517, 111);
            uiLabel_currenttaskState.Name = "uiLabel_currenttaskState";
            uiLabel_currenttaskState.Size = new Size(228, 40);
            uiLabel_currenttaskState.TabIndex = 9;
            uiLabel_currenttaskState.Text = "无AGV任务";
            uiLabel_currenttaskState.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // countAndLockGroup
            // 
            countAndLockGroup.Controls.Add(uiButton_inputTrayCount);
            countAndLockGroup.Controls.Add(uiLedLabel_inputTrayCount);
            countAndLockGroup.Controls.Add(uiButton_outputTrayCount);
            countAndLockGroup.Controls.Add(uiLedLabel_outputTrayCount);
            countAndLockGroup.Controls.Add(uiCheckBox_agvLocked);
            countAndLockGroup.Controls.Add(uiButton_lockAgv);
            countAndLockGroup.Controls.Add(uiButton_loaderEmpty);
            countAndLockGroup.Controls.Add(uiButton_unlockAgv);
            countAndLockGroup.Controls.Add(uiCheckBox_loaderEmpty);
            countAndLockGroup.FillColor = Color.White;
            countAndLockGroup.Font = new Font("Microsoft YaHei UI", 10F);
            countAndLockGroup.Location = new Point(20, 434);
            countAndLockGroup.Margin = new Padding(4, 5, 4, 5);
            countAndLockGroup.MinimumSize = new Size(1, 1);
            countAndLockGroup.Name = "countAndLockGroup";
            countAndLockGroup.Padding = new Padding(15, 35, 15, 15);
            countAndLockGroup.RectColor = Color.FromArgb(220, 223, 230);
            countAndLockGroup.Size = new Size(765, 95);
            countAndLockGroup.TabIndex = 7;
            countAndLockGroup.TabStop = false;
            countAndLockGroup.Text = "计数与锁定控制";
            countAndLockGroup.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiButton_inputTrayCount
            // 
            uiButton_inputTrayCount.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_inputTrayCount.Location = new Point(15, 38);
            uiButton_inputTrayCount.MinimumSize = new Size(1, 1);
            uiButton_inputTrayCount.Name = "uiButton_inputTrayCount";
            uiButton_inputTrayCount.Size = new Size(92, 35);
            uiButton_inputTrayCount.Style = UIStyle.Custom;
            uiButton_inputTrayCount.TabIndex = 6;
            uiButton_inputTrayCount.Text = "入料口盘数";
            uiButton_inputTrayCount.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_inputTrayCount.Click += uiButton_inputTrayCount_Click;
            // 
            // uiLedLabel_inputTrayCount
            // 
            uiLedLabel_inputTrayCount.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            uiLedLabel_inputTrayCount.ForeColor = Color.LimeGreen;
            uiLedLabel_inputTrayCount.Location = new Point(113, 38);
            uiLedLabel_inputTrayCount.MinimumSize = new Size(1, 1);
            uiLedLabel_inputTrayCount.Name = "uiLedLabel_inputTrayCount";
            uiLedLabel_inputTrayCount.Size = new Size(80, 35);
            uiLedLabel_inputTrayCount.TabIndex = 5;
            uiLedLabel_inputTrayCount.Text = "0";
            // 
            // uiButton_outputTrayCount
            // 
            uiButton_outputTrayCount.FillColor = Color.FromArgb(110, 190, 40);
            uiButton_outputTrayCount.FillColor2 = Color.FromArgb(110, 190, 40);
            uiButton_outputTrayCount.FillHoverColor = Color.FromArgb(139, 203, 83);
            uiButton_outputTrayCount.FillPressColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.FillSelectedColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_outputTrayCount.LightColor = Color.FromArgb(245, 251, 241);
            uiButton_outputTrayCount.Location = new Point(199, 38);
            uiButton_outputTrayCount.MinimumSize = new Size(1, 1);
            uiButton_outputTrayCount.Name = "uiButton_outputTrayCount";
            uiButton_outputTrayCount.RectColor = Color.FromArgb(110, 190, 40);
            uiButton_outputTrayCount.RectHoverColor = Color.FromArgb(139, 203, 83);
            uiButton_outputTrayCount.RectPressColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.RectSelectedColor = Color.FromArgb(88, 152, 32);
            uiButton_outputTrayCount.Size = new Size(92, 35);
            uiButton_outputTrayCount.Style = UIStyle.Custom;
            uiButton_outputTrayCount.TabIndex = 6;
            uiButton_outputTrayCount.Text = "出料口盘数";
            uiButton_outputTrayCount.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_outputTrayCount.Click += uiButton_outputTrayCount_Click;
            // 
            // uiLedLabel_outputTrayCount
            // 
            uiLedLabel_outputTrayCount.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            uiLedLabel_outputTrayCount.ForeColor = Color.LimeGreen;
            uiLedLabel_outputTrayCount.Location = new Point(297, 38);
            uiLedLabel_outputTrayCount.MinimumSize = new Size(1, 1);
            uiLedLabel_outputTrayCount.Name = "uiLedLabel_outputTrayCount";
            uiLedLabel_outputTrayCount.Size = new Size(80, 35);
            uiLedLabel_outputTrayCount.TabIndex = 5;
            uiLedLabel_outputTrayCount.Text = "0";
            // 
            // uiCheckBox_agvLocked
            // 
            uiCheckBox_agvLocked.Font = new Font("Microsoft YaHei UI", 10F);
            uiCheckBox_agvLocked.ForeColor = Color.FromArgb(54, 59, 69);
            uiCheckBox_agvLocked.Location = new Point(383, 56);
            uiCheckBox_agvLocked.MinimumSize = new Size(1, 1);
            uiCheckBox_agvLocked.Name = "uiCheckBox_agvLocked";
            uiCheckBox_agvLocked.ReadOnly = true;
            uiCheckBox_agvLocked.Size = new Size(131, 25);
            uiCheckBox_agvLocked.TabIndex = 4;
            uiCheckBox_agvLocked.Text = "锁定进出盘";
            uiCheckBox_agvLocked.CheckedChanged += uiCheckBox_agvLocked_CheckedChanged;
            // 
            // uiButton_lockAgv
            // 
            uiButton_lockAgv.FillColor = Color.FromArgb(230, 80, 80);
            uiButton_lockAgv.FillColor2 = Color.FromArgb(230, 80, 80);
            uiButton_lockAgv.FillHoverColor = Color.FromArgb(235, 115, 115);
            uiButton_lockAgv.FillPressColor = Color.FromArgb(184, 64, 64);
            uiButton_lockAgv.FillSelectedColor = Color.FromArgb(184, 64, 64);
            uiButton_lockAgv.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_lockAgv.LightColor = Color.FromArgb(253, 243, 243);
            uiButton_lockAgv.Location = new Point(522, 38);
            uiButton_lockAgv.MinimumSize = new Size(1, 1);
            uiButton_lockAgv.Name = "uiButton_lockAgv";
            uiButton_lockAgv.RectColor = Color.FromArgb(230, 80, 80);
            uiButton_lockAgv.RectHoverColor = Color.FromArgb(235, 115, 115);
            uiButton_lockAgv.RectPressColor = Color.FromArgb(184, 64, 64);
            uiButton_lockAgv.RectSelectedColor = Color.FromArgb(184, 64, 64);
            uiButton_lockAgv.Size = new Size(70, 35);
            uiButton_lockAgv.Style = UIStyle.Custom;
            uiButton_lockAgv.TabIndex = 10;
            uiButton_lockAgv.Text = "锁定";
            uiButton_lockAgv.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_lockAgv.Click += uiButton_lockAgv_Click;
            // 
            // uiButton_unlockAgv
            // 
            uiButton_unlockAgv.FillColor = Color.FromArgb(110, 190, 40);
            uiButton_unlockAgv.FillColor2 = Color.FromArgb(110, 190, 40);
            uiButton_unlockAgv.FillHoverColor = Color.FromArgb(139, 203, 83);
            uiButton_unlockAgv.FillPressColor = Color.FromArgb(88, 152, 32);
            uiButton_unlockAgv.FillSelectedColor = Color.FromArgb(88, 152, 32);
            uiButton_unlockAgv.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_unlockAgv.LightColor = Color.FromArgb(245, 251, 241);
            uiButton_unlockAgv.Location = new Point(602, 38);
            uiButton_unlockAgv.MinimumSize = new Size(1, 1);
            uiButton_unlockAgv.Name = "uiButton_unlockAgv";
            uiButton_unlockAgv.RectColor = Color.FromArgb(110, 190, 40);
            uiButton_unlockAgv.RectHoverColor = Color.FromArgb(139, 203, 83);
            uiButton_unlockAgv.RectPressColor = Color.FromArgb(88, 152, 32);
            uiButton_unlockAgv.RectSelectedColor = Color.FromArgb(88, 152, 32);
            uiButton_unlockAgv.Size = new Size(70, 35);
            uiButton_unlockAgv.Style = UIStyle.Custom;
            uiButton_unlockAgv.TabIndex = 10;
            uiButton_unlockAgv.Text = "解锁";
            uiButton_unlockAgv.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_unlockAgv.Click += uiButton_unlockAgv_Click;
            // 
            // uiCheckBox_loaderEmpty
            // 
            uiCheckBox_loaderEmpty.Font = new Font("Microsoft YaHei UI", 10F);
            uiCheckBox_loaderEmpty.ForeColor = Color.FromArgb(54, 59, 69);
            uiCheckBox_loaderEmpty.Location = new Point(383, 28);
            uiCheckBox_loaderEmpty.MinimumSize = new Size(1, 1);
            uiCheckBox_loaderEmpty.Name = "uiCheckBox_loaderEmpty";
            uiCheckBox_loaderEmpty.ReadOnly = true;
            uiCheckBox_loaderEmpty.Size = new Size(131, 25);
            uiCheckBox_loaderEmpty.TabIndex = 11;
            uiCheckBox_loaderEmpty.Text = "Loader为空";
            // 
            // inventoryGroup
            // 
            inventoryGroup.Controls.Add(uiLabel_agvInventoryTitle);
            inventoryGroup.Controls.Add(uiButton_swichAgvMode);
            inventoryGroup.Controls.Add(uiLedLabel_agvInventory);
            inventoryGroup.Controls.Add(uiCheckBox_agvEnabled);
            inventoryGroup.Controls.Add(uiButton_refresh);
            inventoryGroup.Controls.Add(uiLedLabel_stockerInventory);
            inventoryGroup.Controls.Add(taskLabel);
            inventoryGroup.Controls.Add(uiButton_sendInputTask);
            inventoryGroup.Controls.Add(uiLabel_stockerInventoryTitle);
            inventoryGroup.Controls.Add(uiButton_sendOutputTask);
            inventoryGroup.Controls.Add(uiLabel_currenttaskState);
            inventoryGroup.Controls.Add(uiButton_sendInputOutputTask);
            inventoryGroup.Controls.Add(uiLabel_taskRequestTime);
            inventoryGroup.FillColor = Color.White;
            inventoryGroup.Font = new Font("Microsoft YaHei UI", 10F);
            inventoryGroup.Location = new Point(20, 105);
            inventoryGroup.Margin = new Padding(4, 5, 4, 5);
            inventoryGroup.MinimumSize = new Size(1, 1);
            inventoryGroup.Name = "inventoryGroup";
            inventoryGroup.Padding = new Padding(15, 35, 15, 15);
            inventoryGroup.RectColor = Color.FromArgb(220, 223, 230);
            inventoryGroup.Size = new Size(765, 171);
            inventoryGroup.TabIndex = 14;
            inventoryGroup.TabStop = false;
            inventoryGroup.Text = "AGV功能";
            inventoryGroup.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiLabel_agvInventoryTitle
            // 
            uiLabel_agvInventoryTitle.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            uiLabel_agvInventoryTitle.ForeColor = Color.FromArgb(54, 59, 69);
            uiLabel_agvInventoryTitle.Location = new Point(243, 38);
            uiLabel_agvInventoryTitle.Name = "uiLabel_agvInventoryTitle";
            uiLabel_agvInventoryTitle.Size = new Size(76, 35);
            uiLabel_agvInventoryTitle.TabIndex = 15;
            uiLabel_agvInventoryTitle.Text = "AGV库存:";
            uiLabel_agvInventoryTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // uiLedLabel_agvInventory
            // 
            uiLedLabel_agvInventory.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            uiLedLabel_agvInventory.ForeColor = Color.LimeGreen;
            uiLedLabel_agvInventory.Location = new Point(329, 38);
            uiLedLabel_agvInventory.MinimumSize = new Size(1, 1);
            uiLedLabel_agvInventory.Name = "uiLedLabel_agvInventory";
            uiLedLabel_agvInventory.Size = new Size(80, 35);
            uiLedLabel_agvInventory.TabIndex = 16;
            uiLedLabel_agvInventory.Text = "0";
            // 
            // uiLedLabel_stockerInventory
            // 
            uiLedLabel_stockerInventory.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            uiLedLabel_stockerInventory.ForeColor = Color.LimeGreen;
            uiLedLabel_stockerInventory.Location = new Point(559, 38);
            uiLedLabel_stockerInventory.MinimumSize = new Size(1, 1);
            uiLedLabel_stockerInventory.Name = "uiLedLabel_stockerInventory";
            uiLedLabel_stockerInventory.Size = new Size(80, 35);
            uiLedLabel_stockerInventory.TabIndex = 18;
            uiLedLabel_stockerInventory.Text = "0";
            // 
            // uiLabel_stockerInventoryTitle
            // 
            uiLabel_stockerInventoryTitle.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            uiLabel_stockerInventoryTitle.ForeColor = Color.FromArgb(54, 59, 69);
            uiLabel_stockerInventoryTitle.Location = new Point(456, 38);
            uiLabel_stockerInventoryTitle.Name = "uiLabel_stockerInventoryTitle";
            uiLabel_stockerInventoryTitle.Size = new Size(93, 35);
            uiLabel_stockerInventoryTitle.TabIndex = 17;
            uiLabel_stockerInventoryTitle.Text = "Stocker库存:";
            uiLabel_stockerInventoryTitle.TextAlign = ContentAlignment.MiddleRight;
            // 
            // uiLabel_taskRequestTime
            // 
            uiLabel_taskRequestTime.Font = new Font("Microsoft YaHei UI", 10F);
            uiLabel_taskRequestTime.ForeColor = Color.FromArgb(54, 59, 69);
            uiLabel_taskRequestTime.Location = new Point(517, 86);
            uiLabel_taskRequestTime.Name = "uiLabel_taskRequestTime";
            uiLabel_taskRequestTime.Size = new Size(228, 25);
            uiLabel_taskRequestTime.TabIndex = 19;
            uiLabel_taskRequestTime.Text = "请求时间: 无";
            uiLabel_taskRequestTime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // logGroup
            // 
            logGroup.Controls.Add(richTextBox1);
            logGroup.FillColor = Color.White;
            logGroup.Font = new Font("Microsoft YaHei UI", 10F);
            logGroup.Location = new Point(20, 539);
            logGroup.Margin = new Padding(4, 5, 4, 5);
            logGroup.MinimumSize = new Size(1, 1);
            logGroup.Name = "logGroup";
            logGroup.Padding = new Padding(15, 35, 15, 15);
            logGroup.RectColor = Color.FromArgb(220, 223, 230);
            logGroup.Size = new Size(765, 165);
            logGroup.TabIndex = 8;
            logGroup.TabStop = false;
            logGroup.Text = "日志";
            logGroup.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // richTextBox1
            // 
            richTextBox1.FillColor = Color.White;
            richTextBox1.Font = new Font("Consolas", 9F);
            richTextBox1.Location = new Point(10, 30);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(5);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(745, 124);
            richTextBox1.TabIndex = 8;
            richTextBox1.TextAlignment = ContentAlignment.TopLeft;
            // 
            // notifyIcon
            // 
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "EAP.Client";
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            // 
            // uiButton_loaderEmpty
            // 
            uiButton_loaderEmpty.Font = new Font("Microsoft YaHei UI", 10F);
            uiButton_loaderEmpty.Location = new Point(685, 38);
            uiButton_loaderEmpty.MinimumSize = new Size(1, 1);
            uiButton_loaderEmpty.Name = "uiButton_loaderEmpty";
            uiButton_loaderEmpty.Size = new Size(70, 35);
            uiButton_loaderEmpty.Style = UIStyle.Custom;
            uiButton_loaderEmpty.TabIndex = 10;
            uiButton_loaderEmpty.Text = "Loader空";
            uiButton_loaderEmpty.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_loaderEmpty.Click += uiButton_loaderEmpty_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(800, 721);
            Controls.Add(topPanel);
            Controls.Add(groupBox_1);
            Controls.Add(countAndLockGroup);
            Controls.Add(inventoryGroup);
            Controls.Add(logGroup);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            Text = "EAP Client";
            ZoomScaleRect = new Rectangle(15, 15, 800, 770);
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Shown += MainForm_Shown;
            SizeChanged += MainForm_SizeChanged;
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            groupBox_1.ResumeLayout(false);
            countAndLockGroup.ResumeLayout(false);
            inventoryGroup.ResumeLayout(false);
            inventoryGroup.PerformLayout();
            logGroup.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private UILabel label_conn_status;
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
        private UIPanel topPanel;
        private UILabel taskLabel;
        private UIGroupBox countAndLockGroup;
        private UIGroupBox logGroup;

        // 新增库存相关控件声明
        private UIGroupBox inventoryGroup;
        private UILabel uiLabel_agvInventoryTitle;
        private UILedLabel uiLedLabel_agvInventory;
        private UILabel uiLabel_stockerInventoryTitle;
        private UILedLabel uiLedLabel_stockerInventory;

        // 新增控件声明
        private UICheckBox uiCheckBox_loaderEmpty;
        private UILabel uiLabel_taskRequestTime;
        private UIButton uiButton_loaderEmpty;
    }
}
