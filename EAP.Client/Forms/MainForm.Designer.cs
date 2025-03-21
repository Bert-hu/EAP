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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            textBox_LotId = new UITextBox();
            label_1 = new UILabel();
            label1 = new UILabel();
            textBox_modelname = new UITextBox();
            textBox_machinerecipe = new UITextBox();
            label2 = new UILabel();
            label_updatetime_aoi = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            checkBox_checkrecipe = new UICheckBox();
            label_ProcessState = new UILabel();
            textBox_waferId = new UITextBox();
            label5 = new UILabel();
            button_CompareRecipe = new UIButton();
            richTextBox1 = new UIRichTextBox();
            uiButton_ScanToDownloadRecipe = new UIButton();
            uiButton_WaferOut = new UIButton();
            uiButton_WaferIn = new UIButton();
            groupBox_1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_LotId
            // 
            textBox_LotId.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_LotId.Location = new Point(202, 119);
            textBox_LotId.Margin = new Padding(4, 5, 4, 5);
            textBox_LotId.MinimumSize = new Size(1, 16);
            textBox_LotId.Name = "textBox_LotId";
            textBox_LotId.Padding = new Padding(5);
            textBox_LotId.ShowText = false;
            textBox_LotId.Size = new Size(256, 33);
            textBox_LotId.TabIndex = 0;
            textBox_LotId.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_LotId.Watermark = "";
            // 
            // label_1
            // 
            label_1.AutoSize = true;
            label_1.Font = new Font("Microsoft YaHei UI", 15F);
            label_1.ForeColor = Color.FromArgb(48, 48, 48);
            label_1.Location = new Point(15, 122);
            label_1.Name = "label_1";
            label_1.Size = new Size(96, 27);
            label_1.TabIndex = 1;
            label_1.Text = "Wafer ID";
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
            groupBox_1.Controls.Add(label_ProcessState);
            groupBox_1.Controls.Add(label_conn_status);
            groupBox_1.Controls.Add(textBox_LotId);
            groupBox_1.Controls.Add(textBox_waferId);
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
            groupBox_1.Size = new Size(471, 302);
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
            checkBox_checkrecipe.Visible = false;
            checkBox_checkrecipe.CheckedChanged += checkBox_checkrecipe_CheckedChanged;
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
            // textBox_waferId
            // 
            textBox_waferId.Font = new Font("Microsoft YaHei UI", 15F);
            textBox_waferId.Location = new Point(202, 219);
            textBox_waferId.Margin = new Padding(4, 5, 4, 5);
            textBox_waferId.MinimumSize = new Size(1, 16);
            textBox_waferId.Name = "textBox_waferId";
            textBox_waferId.Padding = new Padding(5);
            textBox_waferId.ShowText = false;
            textBox_waferId.Size = new Size(256, 33);
            textBox_waferId.TabIndex = 0;
            textBox_waferId.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_waferId.Watermark = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft YaHei UI", 15F);
            label5.ForeColor = Color.FromArgb(48, 48, 48);
            label5.Location = new Point(15, 222);
            label5.Name = "label5";
            label5.Size = new Size(94, 27);
            label5.TabIndex = 1;
            label5.Text = "Wafer Id";
            // 
            // button_CompareRecipe
            // 
            button_CompareRecipe.Enabled = false;
            button_CompareRecipe.Font = new Font("Microsoft YaHei UI", 11F);
            button_CompareRecipe.Location = new Point(490, 101);
            button_CompareRecipe.MinimumSize = new Size(1, 1);
            button_CompareRecipe.Name = "button_CompareRecipe";
            button_CompareRecipe.Size = new Size(108, 33);
            button_CompareRecipe.TabIndex = 5;
            button_CompareRecipe.Text = "比较参数";
            button_CompareRecipe.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_CompareRecipe.Click += button_CompareRecipe_Click;
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
            richTextBox1.Size = new Size(588, 258);
            richTextBox1.TabIndex = 8;
            richTextBox1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiButton_ScanToDownloadRecipe
            // 
            uiButton_ScanToDownloadRecipe.Enabled = false;
            uiButton_ScanToDownloadRecipe.Font = new Font("Microsoft YaHei UI", 11F);
            uiButton_ScanToDownloadRecipe.Location = new Point(490, 58);
            uiButton_ScanToDownloadRecipe.MinimumSize = new Size(1, 1);
            uiButton_ScanToDownloadRecipe.Name = "uiButton_ScanToDownloadRecipe";
            uiButton_ScanToDownloadRecipe.Size = new Size(108, 33);
            uiButton_ScanToDownloadRecipe.TabIndex = 5;
            uiButton_ScanToDownloadRecipe.Text = "扫码下载程式";
            uiButton_ScanToDownloadRecipe.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_ScanToDownloadRecipe.Click += uiButton_ScanToDownloadRecipe_Click;
            // 
            // uiButton_WaferOut
            // 
            uiButton_WaferOut.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_WaferOut.Location = new Point(490, 241);
            uiButton_WaferOut.MinimumSize = new Size(1, 1);
            uiButton_WaferOut.Name = "uiButton_WaferOut";
            uiButton_WaferOut.Size = new Size(108, 35);
            uiButton_WaferOut.TabIndex = 9;
            uiButton_WaferOut.Text = "Wafer Out";
            uiButton_WaferOut.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_WaferOut.Click += uiButton_WaferOut_Click;
            // 
            // uiButton_WaferIn
            // 
            uiButton_WaferIn.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_WaferIn.Location = new Point(490, 200);
            uiButton_WaferIn.MinimumSize = new Size(1, 1);
            uiButton_WaferIn.Name = "uiButton_WaferIn";
            uiButton_WaferIn.Size = new Size(108, 35);
            uiButton_WaferIn.TabIndex = 9;
            uiButton_WaferIn.Text = "Wafer In";
            uiButton_WaferIn.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_WaferIn.Click += uiButton_WaferIn_Click;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(614, 638);
            Controls.Add(uiButton_WaferIn);
            Controls.Add(uiButton_WaferOut);
            Controls.Add(uiButton_ScanToDownloadRecipe);
            Controls.Add(richTextBox1);
            Controls.Add(button_CompareRecipe);
            Controls.Add(groupBox_1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "EAP Client";
            ZoomScaleRect = new Rectangle(15, 15, 614, 638);
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            groupBox_1.ResumeLayout(false);
            groupBox_1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private UITextBox textBox_LotId;
        private UILabel label_1;
        private UILabel label1;
        private UITextBox textBox_modelname;
        private UITextBox textBox_machinerecipe;
        private UILabel label2;
        private UILabel label3;
        private UILabel label4;
        private UILabel label_updatetime_aoi;
        private UILabel label_conn_status;
        private UIButton button1;
        private UIButton button_getModelName;
        private UIGroupBox groupBox1;
        private UIGroupBox groupBox_1;
        private UILabel label_ProcessState;
        private UICheckBox checkBox_checkrecipe;
        private UIRichTextBox richTextBox1;
        private UITextBox textBox_waferId;
        private UILabel label5;
        private UIButton button_CompareRecipe;
        private UIButton uiButton_ScanToDownloadRecipe;
        private UIButton uiButton_WaferOut;
        private UIButton uiButton_WaferIn;
    }
}