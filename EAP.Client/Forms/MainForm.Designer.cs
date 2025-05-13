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
            textBox_packageName = new UITextBox();
            label2 = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            uiLedLabel_totalMax = new UILedLabel();
            uiButton_clearCount = new UIButton();
            uiLedLabel_total = new UILedLabel();
            uiButton_loadSetting = new UIButton();
            uiButton_modifySetting = new UIButton();
            uiLedLabel_ohMax = new UILedLabel();
            uiLedLabel_oh = new UILedLabel();
            uiLedLabel_mMax = new UILedLabel();
            uiLedLabel_icosMax = new UILedLabel();
            uiLedLabel_m = new UILedLabel();
            uiLedLabel_icos = new UILedLabel();
            label_ProcessState = new UILabel();
            uiLabel6 = new UILabel();
            uiLabel3 = new UILabel();
            uiLabel2 = new UILabel();
            uiLabel5 = new UILabel();
            uiLabel4 = new UILabel();
            uiLabel1 = new UILabel();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            uiCheckBox_isLocked = new UICheckBox();
            uiRichTextBox_lockMessage = new UIRichTextBox();
            uiTabControl1 = new UITabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            groupBox_1.SuspendLayout();
            uiTabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_packageName
            // 
            textBox_packageName.Font = new Font("宋体", 9.75F);
            textBox_packageName.Location = new Point(91, 65);
            textBox_packageName.Margin = new Padding(4, 5, 4, 5);
            textBox_packageName.MinimumSize = new Size(1, 16);
            textBox_packageName.Name = "textBox_packageName";
            textBox_packageName.Padding = new Padding(5);
            textBox_packageName.ReadOnly = true;
            textBox_packageName.ShowText = false;
            textBox_packageName.Size = new Size(147, 21);
            textBox_packageName.TabIndex = 0;
            textBox_packageName.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_packageName.Watermark = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("宋体", 9.75F);
            label2.ForeColor = Color.FromArgb(48, 48, 48);
            label2.Location = new Point(15, 72);
            label2.Name = "label2";
            label2.Size = new Size(59, 13);
            label2.TabIndex = 1;
            label2.Text = "设备程式";
            // 
            // label_conn_status
            // 
            label_conn_status.AutoSize = true;
            label_conn_status.BackColor = Color.Gray;
            label_conn_status.Font = new Font("宋体", 9.75F);
            label_conn_status.ForeColor = Color.White;
            label_conn_status.Location = new Point(21, 32);
            label_conn_status.Name = "label_conn_status";
            label_conn_status.Size = new Size(77, 13);
            label_conn_status.TabIndex = 3;
            label_conn_status.Text = "Connecting";
            // 
            // groupBox_1
            // 
            groupBox_1.Controls.Add(uiLedLabel_totalMax);
            groupBox_1.Controls.Add(uiButton_clearCount);
            groupBox_1.Controls.Add(uiLedLabel_total);
            groupBox_1.Controls.Add(uiButton_loadSetting);
            groupBox_1.Controls.Add(uiButton_modifySetting);
            groupBox_1.Controls.Add(uiLedLabel_ohMax);
            groupBox_1.Controls.Add(uiLedLabel_oh);
            groupBox_1.Controls.Add(uiLedLabel_mMax);
            groupBox_1.Controls.Add(uiLedLabel_icosMax);
            groupBox_1.Controls.Add(uiLedLabel_m);
            groupBox_1.Controls.Add(uiLedLabel_icos);
            groupBox_1.Controls.Add(label_ProcessState);
            groupBox_1.Controls.Add(label_conn_status);
            groupBox_1.Controls.Add(uiLabel6);
            groupBox_1.Controls.Add(textBox_packageName);
            groupBox_1.Controls.Add(uiLabel3);
            groupBox_1.Controls.Add(uiLabel2);
            groupBox_1.Controls.Add(uiLabel5);
            groupBox_1.Controls.Add(uiLabel4);
            groupBox_1.Controls.Add(uiLabel1);
            groupBox_1.Controls.Add(label2);
            groupBox_1.Font = new Font("宋体", 10F);
            groupBox_1.Location = new Point(4, 5);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(423, 254);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "Info";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiLedLabel_totalMax
            // 
            uiLedLabel_totalMax.Font = new Font("宋体", 8.25F);
            uiLedLabel_totalMax.ForeColor = Color.FromArgb(255, 128, 128);
            uiLedLabel_totalMax.IntervalOn = 1;
            uiLedLabel_totalMax.Location = new Point(82, 198);
            uiLedLabel_totalMax.MinimumSize = new Size(1, 1);
            uiLedLabel_totalMax.Name = "uiLedLabel_totalMax";
            uiLedLabel_totalMax.Size = new Size(91, 18);
            uiLedLabel_totalMax.TabIndex = 6;
            uiLedLabel_totalMax.Text = "0";
            // 
            // uiButton_clearCount
            // 
            uiButton_clearCount.Font = new Font("宋体", 10F);
            uiButton_clearCount.Location = new Point(303, 99);
            uiButton_clearCount.MinimumSize = new Size(1, 1);
            uiButton_clearCount.Name = "uiButton_clearCount";
            uiButton_clearCount.Size = new Size(82, 24);
            uiButton_clearCount.TabIndex = 11;
            uiButton_clearCount.Text = "计数清零";
            uiButton_clearCount.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_clearCount.Click += uiButton_clearCount_Click;
            // 
            // uiLedLabel_total
            // 
            uiLedLabel_total.Font = new Font("宋体", 8.25F);
            uiLedLabel_total.IntervalOn = 1;
            uiLedLabel_total.Location = new Point(163, 198);
            uiLedLabel_total.MinimumSize = new Size(1, 1);
            uiLedLabel_total.Name = "uiLedLabel_total";
            uiLedLabel_total.Size = new Size(91, 18);
            uiLedLabel_total.TabIndex = 6;
            uiLedLabel_total.Text = "0";
            // 
            // uiButton_loadSetting
            // 
            uiButton_loadSetting.Font = new Font("宋体", 10F);
            uiButton_loadSetting.Location = new Point(303, 35);
            uiButton_loadSetting.MinimumSize = new Size(1, 1);
            uiButton_loadSetting.Name = "uiButton_loadSetting";
            uiButton_loadSetting.Size = new Size(82, 24);
            uiButton_loadSetting.TabIndex = 10;
            uiButton_loadSetting.Text = "加载设定";
            uiButton_loadSetting.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_loadSetting.Click += uiButton_loadSetting_Click;
            // 
            // uiButton_modifySetting
            // 
            uiButton_modifySetting.Font = new Font("宋体", 10F);
            uiButton_modifySetting.Location = new Point(303, 68);
            uiButton_modifySetting.MinimumSize = new Size(1, 1);
            uiButton_modifySetting.Name = "uiButton_modifySetting";
            uiButton_modifySetting.Size = new Size(82, 24);
            uiButton_modifySetting.TabIndex = 10;
            uiButton_modifySetting.Text = "修改设定";
            uiButton_modifySetting.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_modifySetting.Click += uiButton_modifySetting_Click;
            // 
            // uiLedLabel_ohMax
            // 
            uiLedLabel_ohMax.Font = new Font("宋体", 8.25F);
            uiLedLabel_ohMax.ForeColor = Color.FromArgb(255, 128, 128);
            uiLedLabel_ohMax.IntervalOn = 1;
            uiLedLabel_ohMax.Location = new Point(82, 171);
            uiLedLabel_ohMax.MinimumSize = new Size(1, 1);
            uiLedLabel_ohMax.Name = "uiLedLabel_ohMax";
            uiLedLabel_ohMax.Size = new Size(91, 18);
            uiLedLabel_ohMax.TabIndex = 6;
            uiLedLabel_ohMax.Text = "0";
            // 
            // uiLedLabel_oh
            // 
            uiLedLabel_oh.Font = new Font("宋体", 8.25F);
            uiLedLabel_oh.IntervalOn = 1;
            uiLedLabel_oh.Location = new Point(163, 171);
            uiLedLabel_oh.MinimumSize = new Size(1, 1);
            uiLedLabel_oh.Name = "uiLedLabel_oh";
            uiLedLabel_oh.Size = new Size(91, 18);
            uiLedLabel_oh.TabIndex = 6;
            uiLedLabel_oh.Text = "0";
            // 
            // uiLedLabel_mMax
            // 
            uiLedLabel_mMax.Font = new Font("宋体", 8.25F);
            uiLedLabel_mMax.ForeColor = Color.FromArgb(255, 128, 128);
            uiLedLabel_mMax.IntervalOn = 1;
            uiLedLabel_mMax.Location = new Point(82, 145);
            uiLedLabel_mMax.MinimumSize = new Size(1, 1);
            uiLedLabel_mMax.Name = "uiLedLabel_mMax";
            uiLedLabel_mMax.Size = new Size(91, 18);
            uiLedLabel_mMax.TabIndex = 6;
            uiLedLabel_mMax.Text = "0";
            // 
            // uiLedLabel_icosMax
            // 
            uiLedLabel_icosMax.Font = new Font("宋体", 8.25F);
            uiLedLabel_icosMax.ForeColor = Color.FromArgb(255, 128, 128);
            uiLedLabel_icosMax.IntervalOn = 1;
            uiLedLabel_icosMax.Location = new Point(82, 117);
            uiLedLabel_icosMax.MinimumSize = new Size(1, 1);
            uiLedLabel_icosMax.Name = "uiLedLabel_icosMax";
            uiLedLabel_icosMax.Size = new Size(91, 18);
            uiLedLabel_icosMax.TabIndex = 6;
            uiLedLabel_icosMax.Text = "0";
            // 
            // uiLedLabel_m
            // 
            uiLedLabel_m.Font = new Font("宋体", 8.25F);
            uiLedLabel_m.IntervalOn = 1;
            uiLedLabel_m.Location = new Point(163, 145);
            uiLedLabel_m.MinimumSize = new Size(1, 1);
            uiLedLabel_m.Name = "uiLedLabel_m";
            uiLedLabel_m.Size = new Size(91, 18);
            uiLedLabel_m.TabIndex = 6;
            uiLedLabel_m.Text = "0";
            // 
            // uiLedLabel_icos
            // 
            uiLedLabel_icos.Font = new Font("宋体", 8.25F);
            uiLedLabel_icos.IntervalOn = 1;
            uiLedLabel_icos.Location = new Point(163, 117);
            uiLedLabel_icos.MinimumSize = new Size(1, 1);
            uiLedLabel_icos.Name = "uiLedLabel_icos";
            uiLedLabel_icos.Size = new Size(91, 18);
            uiLedLabel_icos.TabIndex = 6;
            uiLedLabel_icos.Text = "0";
            // 
            // label_ProcessState
            // 
            label_ProcessState.AutoSize = true;
            label_ProcessState.BackColor = Color.Gray;
            label_ProcessState.Font = new Font("宋体", 9.75F);
            label_ProcessState.ForeColor = Color.White;
            label_ProcessState.Location = new Point(131, 32);
            label_ProcessState.Name = "label_ProcessState";
            label_ProcessState.Size = new Size(56, 13);
            label_ProcessState.TabIndex = 3;
            label_ProcessState.Text = "Unknown";
            // 
            // uiLabel6
            // 
            uiLabel6.AutoSize = true;
            uiLabel6.Font = new Font("宋体", 9.75F);
            uiLabel6.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel6.Location = new Point(15, 198);
            uiLabel6.Name = "uiLabel6";
            uiLabel6.Size = new Size(33, 13);
            uiLabel6.TabIndex = 1;
            uiLabel6.Text = "总计";
            // 
            // uiLabel3
            // 
            uiLabel3.AutoSize = true;
            uiLabel3.Font = new Font("宋体", 9.75F);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(15, 175);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(49, 13);
            uiLabel3.TabIndex = 1;
            uiLabel3.Text = "MIX-OH";
            // 
            // uiLabel2
            // 
            uiLabel2.AutoSize = true;
            uiLabel2.Font = new Font("宋体", 9.75F);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(15, 145);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(42, 13);
            uiLabel2.TabIndex = 1;
            uiLabel2.Text = "MIX-M";
            // 
            // uiLabel5
            // 
            uiLabel5.AutoSize = true;
            uiLabel5.Font = new Font("宋体", 9.75F);
            uiLabel5.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel5.Location = new Point(175, 101);
            uiLabel5.Name = "uiLabel5";
            uiLabel5.Size = new Size(46, 13);
            uiLabel5.TabIndex = 1;
            uiLabel5.Text = "当前值";
            // 
            // uiLabel4
            // 
            uiLabel4.AutoSize = true;
            uiLabel4.Font = new Font("宋体", 9.75F);
            uiLabel4.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel4.Location = new Point(100, 101);
            uiLabel4.Name = "uiLabel4";
            uiLabel4.Size = new Size(46, 13);
            uiLabel4.TabIndex = 1;
            uiLabel4.Text = "设定值";
            // 
            // uiLabel1
            // 
            uiLabel1.AutoSize = true;
            uiLabel1.Font = new Font("宋体", 9.75F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(15, 117);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(63, 13);
            uiLabel1.TabIndex = 1;
            uiLabel1.Text = "MIX-ICOS";
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            richTextBox1.FillColor = Color.White;
            richTextBox1.Font = new Font("宋体", 10F);
            richTextBox1.Location = new Point(-227, -125);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(2);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(423, 180);
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
            // uiCheckBox_isLocked
            // 
            uiCheckBox_isLocked.AutoSize = true;
            uiCheckBox_isLocked.Font = new Font("宋体", 9.75F);
            uiCheckBox_isLocked.ForeColor = Color.FromArgb(48, 48, 48);
            uiCheckBox_isLocked.Location = new Point(4, 16);
            uiCheckBox_isLocked.MinimumSize = new Size(1, 1);
            uiCheckBox_isLocked.Name = "uiCheckBox_isLocked";
            uiCheckBox_isLocked.ReadOnly = true;
            uiCheckBox_isLocked.Size = new Size(56, 21);
            uiCheckBox_isLocked.TabIndex = 4;
            uiCheckBox_isLocked.Text = "锁定";
            uiCheckBox_isLocked.CheckedChanged += uiCheckBox_isLocked_CheckedChanged;
            // 
            // uiRichTextBox_lockMessage
            // 
            uiRichTextBox_lockMessage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            uiRichTextBox_lockMessage.FillColor = Color.White;
            uiRichTextBox_lockMessage.Font = new Font("宋体", 9.75F);
            uiRichTextBox_lockMessage.Location = new Point(-164, 16);
            uiRichTextBox_lockMessage.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox_lockMessage.MinimumSize = new Size(1, 1);
            uiRichTextBox_lockMessage.Name = "uiRichTextBox_lockMessage";
            uiRichTextBox_lockMessage.Padding = new Padding(2);
            uiRichTextBox_lockMessage.ReadOnly = true;
            uiRichTextBox_lockMessage.ShowText = false;
            uiRichTextBox_lockMessage.Size = new Size(286, 53);
            uiRichTextBox_lockMessage.TabIndex = 9;
            uiRichTextBox_lockMessage.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiTabControl1
            // 
            uiTabControl1.Controls.Add(tabPage1);
            uiTabControl1.Controls.Add(tabPage2);
            uiTabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            uiTabControl1.Font = new Font("宋体", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiTabControl1.ItemSize = new Size(150, 20);
            uiTabControl1.Location = new Point(3, 38);
            uiTabControl1.MainPage = "";
            uiTabControl1.MenuStyle = UIMenuStyle.Custom;
            uiTabControl1.Name = "uiTabControl1";
            uiTabControl1.SelectedIndex = 0;
            uiTabControl1.Size = new Size(431, 284);
            uiTabControl1.SizeMode = TabSizeMode.Fixed;
            uiTabControl1.TabIndex = 12;
            uiTabControl1.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            uiTabControl1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.FromArgb(243, 249, 255);
            tabPage1.Controls.Add(groupBox_1);
            tabPage1.Location = new Point(0, 20);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(431, 264);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "状态";
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.FromArgb(243, 249, 255);
            tabPage2.Controls.Add(richTextBox1);
            tabPage2.Controls.Add(uiRichTextBox_lockMessage);
            tabPage2.Controls.Add(uiCheckBox_isLocked);
            tabPage2.Location = new Point(0, 40);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(200, 60);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "日志";
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(437, 325);
            Controls.Add(uiTabControl1);
            Font = new Font("宋体", 8F);
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
            uiTabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private UITextBox textBox_packageName;
        private UILabel label2;
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
        private UICheckBox uiCheckBox_isLocked;
        private UIRichTextBox uiRichTextBox_lockMessage;
        private UILabel uiLabel3;
        private UILabel uiLabel2;
        private UILabel uiLabel1;
        private UILedLabel uiLedLabel_ohMax;
        private UILedLabel uiLedLabel_oh;
        private UILedLabel uiLedLabel_mMax;
        private UILedLabel uiLedLabel_icosMax;
        private UILedLabel uiLedLabel_m;
        private UILedLabel uiLedLabel_icos;
        private UIButton uiButton_modifySetting;
        private UILabel uiLabel5;
        private UILabel uiLabel4;
        private UILedLabel uiLedLabel_totalMax;
        private UILedLabel uiLedLabel_total;
        private UILabel uiLabel6;
        private UIButton uiButton_clearCount;
        private UITabControl uiTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private UIButton uiButton_loadSetting;
    }
}