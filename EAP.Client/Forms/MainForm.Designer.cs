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
            textBox_machinerecipe = new UITextBox();
            label2 = new UILabel();
            label_updatetime_aoi = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            checkBox_checkrecipe = new UICheckBox();
            uiButton_ScanToDownloadRecipe = new UIButton();
            label_ProcessState = new UILabel();
            textBox_projectname = new UITextBox();
            label5 = new UILabel();
            richTextBox1 = new UIRichTextBox();
            textBox_spipanelid = new UITextBox();
            textBox_spimodelname = new UITextBox();
            label_updatetime_spi = new UILabel();
            uiGroupBox1 = new UIGroupBox();
            button_changeToSpiRecipe = new UIButton();
            button_getModelName = new UIButton();
            uiLabel2 = new UILabel();
            uiLabel1 = new UILabel();
            notifyIcon = new NotifyIcon(components);
            uiButton_ScanToDownloadRecipe = new UIButton();
            notifyIcon = new NotifyIcon(components);
            groupBox_1.SuspendLayout();
            uiGroupBox1.SuspendLayout();
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
            // textBox_machinerecipe
            // 
            textBox_machinerecipe.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_machinerecipe.Location = new Point(202, 69);
            textBox_machinerecipe.Margin = new Padding(4, 5, 4, 5);
            textBox_machinerecipe.MinimumSize = new Size(1, 16);
            textBox_machinerecipe.Name = "textBox_machinerecipe";
            textBox_machinerecipe.Padding = new Padding(5);
            textBox_machinerecipe.ShowText = false;
            textBox_machinerecipe.Size = new Size(256, 33);
            textBox_machinerecipe.TabIndex = 0;
            textBox_machinerecipe.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_machinerecipe.Watermark = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 15F);
            label2.ForeColor = Color.FromArgb(48, 48, 48);
            label2.Location = new Point(15, 72);
            label2.Name = "label2";
            label2.Size = new Size(162, 27);
            label2.TabIndex = 1;
            label2.Text = "Machine Recipe";
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
            groupBox_1.Controls.Add(checkBox_checkrecipe);
            groupBox_1.Controls.Add(uiButton_ScanToDownloadRecipe);
            groupBox_1.Controls.Add(label_ProcessState);
            groupBox_1.Controls.Add(label_conn_status);
            groupBox_1.Controls.Add(textBox_panelid);
            groupBox_1.Controls.Add(textBox_projectname);
            groupBox_1.Controls.Add(textBox_modelname);
            groupBox_1.Controls.Add(label_updatetime_aoi);
            groupBox_1.Controls.Add(textBox_machinerecipe);
            groupBox_1.Controls.Add(label2);
            groupBox_1.Controls.Add(label_1);
            groupBox_1.Controls.Add(label5);
            groupBox_1.Controls.Add(label1);
            groupBox_1.Font = new Font("宋体", 12F);
            groupBox_1.Location = new Point(12, 40);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(586, 302);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "Info";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // checkBox_checkrecipe
            // 
            checkBox_checkrecipe.AutoSize = true;
            checkBox_checkrecipe.Checked = true;
            checkBox_checkrecipe.Font = new Font("Microsoft YaHei UI", 15F);
            checkBox_checkrecipe.ForeColor = Color.FromArgb(48, 48, 48);
            checkBox_checkrecipe.Location = new Point(21, 265);
            checkBox_checkrecipe.MinimumSize = new Size(1, 1);
            checkBox_checkrecipe.Name = "checkBox_checkrecipe";
            checkBox_checkrecipe.Size = new Size(250, 32);
            checkBox_checkrecipe.TabIndex = 4;
            checkBox_checkrecipe.Text = "MP模式（检查Recipe）";
            checkBox_checkrecipe.CheckedChanged += checkBox_checkrecipe_CheckedChanged;
            // 
            // uiButton_ScanToDownloadRecipe
            // 
            uiButton_ScanToDownloadRecipe.Font = new Font("Microsoft YaHei UI", 11F);
            uiButton_ScanToDownloadRecipe.Location = new Point(472, 32);
            uiButton_ScanToDownloadRecipe.MinimumSize = new Size(1, 1);
            uiButton_ScanToDownloadRecipe.Name = "uiButton_ScanToDownloadRecipe";
            uiButton_ScanToDownloadRecipe.Size = new Size(108, 33);
            uiButton_ScanToDownloadRecipe.TabIndex = 5;
            uiButton_ScanToDownloadRecipe.Text = "扫码下载程式";
            uiButton_ScanToDownloadRecipe.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_ScanToDownloadRecipe.Visible = false;
            uiButton_ScanToDownloadRecipe.Click += uiButton_ScanToDownloadRecipe_Click;
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
            // textBox_projectname
            // 
            textBox_projectname.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_projectname.Location = new Point(202, 219);
            textBox_projectname.Margin = new Padding(4, 5, 4, 5);
            textBox_projectname.MinimumSize = new Size(1, 16);
            textBox_projectname.Name = "textBox_projectname";
            textBox_projectname.Padding = new Padding(5);
            textBox_projectname.ShowText = false;
            textBox_projectname.Size = new Size(256, 33);
            textBox_projectname.TabIndex = 0;
            textBox_projectname.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_projectname.Watermark = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft YaHei UI", 15F);
            label5.ForeColor = Color.FromArgb(48, 48, 48);
            label5.Location = new Point(15, 222);
            label5.Name = "label5";
            label5.Size = new Size(141, 27);
            label5.TabIndex = 1;
            label5.Text = "Project Name";
            // 
            // richTextBox1
            // 
            richTextBox1.FillColor = Color.White;
            richTextBox1.Font = new Font("宋体", 10F);
            richTextBox1.Location = new Point(10, 527);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(2);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(588, 185);
            richTextBox1.TabIndex = 8;
            richTextBox1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // textBox_spipanelid
            // 
            textBox_spipanelid.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_spipanelid.Location = new Point(204, 37);
            textBox_spipanelid.Margin = new Padding(4, 5, 4, 5);
            textBox_spipanelid.MinimumSize = new Size(1, 16);
            textBox_spipanelid.Name = "textBox_spipanelid";
            textBox_spipanelid.Padding = new Padding(5);
            textBox_spipanelid.ShowText = false;
            textBox_spipanelid.Size = new Size(256, 33);
            textBox_spipanelid.TabIndex = 9;
            textBox_spipanelid.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_spipanelid.Watermark = "";
            // 
            // textBox_spimodelname
            // 
            textBox_spimodelname.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_spimodelname.Location = new Point(204, 85);
            textBox_spimodelname.Margin = new Padding(4, 5, 4, 5);
            textBox_spimodelname.MinimumSize = new Size(1, 16);
            textBox_spimodelname.Name = "textBox_spimodelname";
            textBox_spimodelname.Padding = new Padding(5);
            textBox_spimodelname.ShowText = false;
            textBox_spimodelname.Size = new Size(256, 33);
            textBox_spimodelname.TabIndex = 10;
            textBox_spimodelname.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_spimodelname.Watermark = "";
            // 
            // label_updatetime_spi
            // 
            label_updatetime_spi.AutoSize = true;
            label_updatetime_spi.Font = new Font("宋体", 12F);
            label_updatetime_spi.ForeColor = Color.FromArgb(48, 48, 48);
            label_updatetime_spi.Location = new Point(227, 121);
            label_updatetime_spi.Name = "label_updatetime_spi";
            label_updatetime_spi.Size = new Size(103, 16);
            label_updatetime_spi.TabIndex = 11;
            label_updatetime_spi.Text = "Update Time:";
            // 
            // uiGroupBox1
            // 
            uiGroupBox1.Controls.Add(button_changeToSpiRecipe);
            uiGroupBox1.Controls.Add(button_getModelName);
            uiGroupBox1.Controls.Add(textBox_spipanelid);
            uiGroupBox1.Controls.Add(label_updatetime_spi);
            uiGroupBox1.Controls.Add(textBox_spimodelname);
            uiGroupBox1.Controls.Add(uiLabel2);
            uiGroupBox1.Controls.Add(uiLabel1);
            uiGroupBox1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiGroupBox1.Location = new Point(10, 352);
            uiGroupBox1.Margin = new Padding(4, 5, 4, 5);
            uiGroupBox1.MinimumSize = new Size(1, 1);
            uiGroupBox1.Name = "uiGroupBox1";
            uiGroupBox1.Padding = new Padding(0, 32, 0, 0);
            uiGroupBox1.Size = new Size(588, 165);
            uiGroupBox1.TabIndex = 12;
            uiGroupBox1.Text = "SPI信息";
            uiGroupBox1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // button_changeToSpiRecipe
            // 
            button_changeToSpiRecipe.Font = new Font("宋体", 12F);
            button_changeToSpiRecipe.Location = new Point(474, 85);
            button_changeToSpiRecipe.MinimumSize = new Size(1, 1);
            button_changeToSpiRecipe.Name = "button_changeToSpiRecipe";
            button_changeToSpiRecipe.Size = new Size(105, 33);
            button_changeToSpiRecipe.TabIndex = 13;
            button_changeToSpiRecipe.Text = "切换到SPI机种";
            button_changeToSpiRecipe.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_changeToSpiRecipe.Click += button1_Click;
            // 
            // button_getModelName
            // 
            button_getModelName.Font = new Font("宋体", 12F);
            button_getModelName.Location = new Point(474, 37);
            button_getModelName.MinimumSize = new Size(1, 1);
            button_getModelName.Name = "button_getModelName";
            button_getModelName.Size = new Size(105, 33);
            button_getModelName.TabIndex = 12;
            button_getModelName.Text = "识别机种";
            button_getModelName.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_getModelName.Click += button_getModelName_Click;
            // 
            // uiLabel2
            // 
            uiLabel2.AutoSize = true;
            uiLabel2.Font = new Font("Microsoft YaHei UI", 15F);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(17, 91);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(137, 27);
            uiLabel2.TabIndex = 1;
            uiLabel2.Text = "Model Name";
            // 
            // uiLabel1
            // 
            uiLabel1.AutoSize = true;
            uiLabel1.Font = new Font("Microsoft YaHei UI", 15F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(17, 37);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(90, 27);
            uiLabel1.TabIndex = 1;
            uiLabel1.Text = "Panel ID";
            // 
            // notifyIcon
            // 
            notifyIcon.Text = "EAP.Client";
            notifyIcon.Visible = true;
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
            ClientSize = new Size(614, 717);
            Controls.Add(uiGroupBox1);
            Controls.Add(richTextBox1);
            Controls.Add(groupBox_1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "EAP Client";
            ZoomScaleRect = new Rectangle(15, 15, 614, 638);
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            groupBox_1.ResumeLayout(false);
            groupBox_1.PerformLayout();
            uiGroupBox1.ResumeLayout(false);
            uiGroupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private UITextBox textBox_panelid;
        private UILabel label_1;
        private UILabel label1;
        private UITextBox textBox_modelname;
        private UITextBox textBox_machinerecipe;
        private UILabel label2;
        private UILabel label3;
        private UILabel label4;
        private UILabel label_updatetime_aoi;
        private UILabel label_conn_status;
        private UIButton button_changeToSpiRecipe;
        private UIButton button_getModelName;
        private UIGroupBox groupBox1;
        private UIGroupBox groupBox_1;
        private UILabel label_ProcessState;
        private UICheckBox checkBox_checkrecipe;
        private UIRichTextBox richTextBox1;
        private UITextBox textBox_projectname;
        private UILabel label5;
        private UIButton uiButton_ScanToDownloadRecipe;
        private UITextBox textBox_spipanelid;
        private UITextBox textBox_spimodelname;
        private UILabel label_updatetime_spi;
        private UIGroupBox uiGroupBox1;
        private UILabel uiLabel2;
        private UILabel uiLabel1;
        private NotifyIcon notifyIcon;
    }
}