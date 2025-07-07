using Sunny.UI;
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
            textBox_machinerecipe = new UITextBox();
            label2 = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            uiSymbolButton_changeReel = new UISymbolButton();
            uiTextBox_reelId = new UITextBox();
            uiTextBox_modelName = new UITextBox();
            uiLabel1 = new UILabel();
            uiLabel4 = new UILabel();
            snInfoBindingSource = new BindingSource(components);
            checkBox_checkrecipe = new UICheckBox();
            label_ProcessState = new UILabel();
            button_CompareRecipe = new UIButton();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            uiLabel_admin = new UILabel();
            uiButton_login = new UIButton();
            uiLabel2 = new UILabel();
            uiTextBox_empNo = new UITextBox();
            uiLabel3 = new UILabel();
            uiTextBox_line = new UITextBox();
            uiButton_downloadRecipe = new UIButton();
            uiLabel6 = new UILabel();
            uiTextBox_groupName = new UITextBox();
            uiCheckBox_checkRecipeBody = new UICheckBox();
            groupBox_1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)snInfoBindingSource).BeginInit();
            SuspendLayout();
            // 
            // textBox_machinerecipe
            // 
            textBox_machinerecipe.Font = new Font("Microsoft YaHei UI", 11F);
            textBox_machinerecipe.Location = new Point(147, 33);
            textBox_machinerecipe.Margin = new Padding(4, 5, 4, 5);
            textBox_machinerecipe.MinimumSize = new Size(1, 16);
            textBox_machinerecipe.Name = "textBox_machinerecipe";
            textBox_machinerecipe.Padding = new Padding(5);
            textBox_machinerecipe.ReadOnly = true;
            textBox_machinerecipe.ShowText = false;
            textBox_machinerecipe.Size = new Size(534, 29);
            textBox_machinerecipe.TabIndex = 0;
            textBox_machinerecipe.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_machinerecipe.Watermark = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 11F);
            label2.ForeColor = Color.FromArgb(48, 48, 48);
            label2.Location = new Point(10, 33);
            label2.Name = "label2";
            label2.Size = new Size(59, 20);
            label2.TabIndex = 1;
            label2.Text = "Recipe";
            // 
            // label_conn_status
            // 
            label_conn_status.AutoSize = true;
            label_conn_status.BackColor = Color.Gray;
            label_conn_status.Font = new Font("Microsoft YaHei UI", 12F);
            label_conn_status.ForeColor = Color.White;
            label_conn_status.Location = new Point(22, 47);
            label_conn_status.Name = "label_conn_status";
            label_conn_status.Size = new Size(98, 21);
            label_conn_status.TabIndex = 3;
            label_conn_status.Text = "Connecting";
            // 
            // groupBox_1
            // 
            groupBox_1.Controls.Add(uiSymbolButton_changeReel);
            groupBox_1.Controls.Add(uiTextBox_reelId);
            groupBox_1.Controls.Add(uiTextBox_modelName);
            groupBox_1.Controls.Add(textBox_machinerecipe);
            groupBox_1.Controls.Add(uiLabel1);
            groupBox_1.Controls.Add(uiLabel4);
            groupBox_1.Controls.Add(label2);
            groupBox_1.Font = new Font("宋体", 12F);
            groupBox_1.Location = new Point(14, 155);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(699, 143);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "Info";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiSymbolButton_changeReel
            // 
            uiSymbolButton_changeReel.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_changeReel.Location = new Point(365, 106);
            uiSymbolButton_changeReel.MinimumSize = new Size(1, 1);
            uiSymbolButton_changeReel.Name = "uiSymbolButton_changeReel";
            uiSymbolButton_changeReel.Size = new Size(53, 29);
            uiSymbolButton_changeReel.Style = UIStyle.Custom;
            uiSymbolButton_changeReel.Symbol = 61561;
            uiSymbolButton_changeReel.TabIndex = 12;
            uiSymbolButton_changeReel.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiSymbolButton_changeReel.Click += uiSymbolButton_changeReel_Click;
            // 
            // uiTextBox_reelId
            // 
            uiTextBox_reelId.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_reelId.Location = new Point(147, 106);
            uiTextBox_reelId.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_reelId.MinimumSize = new Size(1, 16);
            uiTextBox_reelId.Name = "uiTextBox_reelId";
            uiTextBox_reelId.Padding = new Padding(5);
            uiTextBox_reelId.ReadOnly = true;
            uiTextBox_reelId.ShowText = false;
            uiTextBox_reelId.Size = new Size(200, 29);
            uiTextBox_reelId.TabIndex = 0;
            uiTextBox_reelId.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_reelId.Watermark = "";
            uiTextBox_reelId.TextChanged += uiTextBox_reelId_TextChanged;
            // 
            // uiTextBox_modelName
            // 
            uiTextBox_modelName.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_modelName.Location = new Point(147, 67);
            uiTextBox_modelName.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_modelName.MinimumSize = new Size(1, 16);
            uiTextBox_modelName.Name = "uiTextBox_modelName";
            uiTextBox_modelName.Padding = new Padding(5);
            uiTextBox_modelName.ReadOnly = true;
            uiTextBox_modelName.ShowText = false;
            uiTextBox_modelName.Size = new Size(534, 29);
            uiTextBox_modelName.TabIndex = 0;
            uiTextBox_modelName.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_modelName.Watermark = "";
            uiTextBox_modelName.TextChanged += uiTextBox_modelName_TextChanged;
            // 
            // uiLabel1
            // 
            uiLabel1.AutoSize = true;
            uiLabel1.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(10, 106);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(56, 20);
            uiLabel1.TabIndex = 1;
            uiLabel1.Text = "ReelID";
            // 
            // uiLabel4
            // 
            uiLabel4.AutoSize = true;
            uiLabel4.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel4.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel4.Location = new Point(10, 67);
            uiLabel4.Name = "uiLabel4";
            uiLabel4.Size = new Size(104, 20);
            uiLabel4.TabIndex = 1;
            uiLabel4.Text = "Model Name";
            // 
            // checkBox_checkrecipe
            // 
            checkBox_checkrecipe.AutoSize = true;
            checkBox_checkrecipe.Checked = true;
            checkBox_checkrecipe.Font = new Font("Microsoft YaHei UI", 10F);
            checkBox_checkrecipe.ForeColor = Color.FromArgb(48, 48, 48);
            checkBox_checkrecipe.Location = new Point(14, 306);
            checkBox_checkrecipe.MinimumSize = new Size(1, 1);
            checkBox_checkrecipe.Name = "checkBox_checkrecipe";
            checkBox_checkrecipe.ReadOnly = true;
            checkBox_checkrecipe.Size = new Size(203, 25);
            checkBox_checkrecipe.TabIndex = 4;
            checkBox_checkrecipe.Text = "Auto Check Recipe Name";
            checkBox_checkrecipe.CheckedChanged += checkBox_checkrecipe_CheckedChanged;
            // 
            // label_ProcessState
            // 
            label_ProcessState.AutoSize = true;
            label_ProcessState.BackColor = Color.Gray;
            label_ProcessState.Font = new Font("Microsoft YaHei UI", 12F);
            label_ProcessState.ForeColor = Color.White;
            label_ProcessState.Location = new Point(175, 47);
            label_ProcessState.Name = "label_ProcessState";
            label_ProcessState.Size = new Size(84, 21);
            label_ProcessState.TabIndex = 3;
            label_ProcessState.Text = "Unknown";
            label_ProcessState.Visible = false;
            // 
            // button_CompareRecipe
            // 
            button_CompareRecipe.Font = new Font("Microsoft YaHei UI", 11F);
            button_CompareRecipe.Location = new Point(14, 118);
            button_CompareRecipe.MinimumSize = new Size(1, 1);
            button_CompareRecipe.Name = "button_CompareRecipe";
            button_CompareRecipe.Size = new Size(135, 29);
            button_CompareRecipe.TabIndex = 5;
            button_CompareRecipe.Text = "Compare Recipe";
            button_CompareRecipe.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_CompareRecipe.Click += button_CompareRecipe_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.FillColor = Color.White;
            richTextBox1.Font = new Font("宋体", 10F);
            richTextBox1.Location = new Point(14, 339);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(2);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(699, 345);
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
            // uiLabel_admin
            // 
            uiLabel_admin.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLabel_admin.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel_admin.Location = new Point(324, 47);
            uiLabel_admin.Name = "uiLabel_admin";
            uiLabel_admin.Size = new Size(150, 23);
            uiLabel_admin.TabIndex = 10;
            uiLabel_admin.Text = "普通用户";
            // 
            // uiButton_login
            // 
            uiButton_login.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_login.Location = new Point(406, 41);
            uiButton_login.MinimumSize = new Size(1, 1);
            uiButton_login.Name = "uiButton_login";
            uiButton_login.Size = new Size(100, 29);
            uiButton_login.TabIndex = 11;
            uiButton_login.Text = "Login";
            uiButton_login.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_login.Click += uiButton_login_Click;
            // 
            // uiLabel2
            // 
            uiLabel2.AutoSize = true;
            uiLabel2.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(22, 85);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(69, 20);
            uiLabel2.TabIndex = 1;
            uiLabel2.Text = "EMP NO";
            // 
            // uiTextBox_empNo
            // 
            uiTextBox_empNo.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_empNo.Location = new Point(90, 81);
            uiTextBox_empNo.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_empNo.MinimumSize = new Size(1, 16);
            uiTextBox_empNo.Name = "uiTextBox_empNo";
            uiTextBox_empNo.Padding = new Padding(5);
            uiTextBox_empNo.ShowText = false;
            uiTextBox_empNo.Size = new Size(134, 29);
            uiTextBox_empNo.TabIndex = 0;
            uiTextBox_empNo.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_empNo.Watermark = "";
            uiTextBox_empNo.TextChanged += uiTextBox_empNo_TextChanged;
            // 
            // uiLabel3
            // 
            uiLabel3.AutoSize = true;
            uiLabel3.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(243, 83);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(39, 20);
            uiLabel3.TabIndex = 1;
            uiLabel3.Text = "Line";
            // 
            // uiTextBox_line
            // 
            uiTextBox_line.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_line.Location = new Point(289, 81);
            uiTextBox_line.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_line.MinimumSize = new Size(1, 16);
            uiTextBox_line.Name = "uiTextBox_line";
            uiTextBox_line.Padding = new Padding(5);
            uiTextBox_line.ReadOnly = true;
            uiTextBox_line.ShowText = false;
            uiTextBox_line.Size = new Size(134, 29);
            uiTextBox_line.TabIndex = 0;
            uiTextBox_line.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_line.Watermark = "";
            uiTextBox_line.TextChanged += uiTextBox_line_TextChanged;
            // 
            // uiButton_downloadRecipe
            // 
            uiButton_downloadRecipe.FillColor = Color.FromArgb(102, 58, 183);
            uiButton_downloadRecipe.FillColor2 = Color.FromArgb(102, 58, 183);
            uiButton_downloadRecipe.FillHoverColor = Color.FromArgb(133, 97, 198);
            uiButton_downloadRecipe.FillPressColor = Color.FromArgb(82, 46, 147);
            uiButton_downloadRecipe.FillSelectedColor = Color.FromArgb(82, 46, 147);
            uiButton_downloadRecipe.Font = new Font("Microsoft YaHei UI", 11F);
            uiButton_downloadRecipe.LightColor = Color.FromArgb(244, 242, 251);
            uiButton_downloadRecipe.Location = new Point(175, 118);
            uiButton_downloadRecipe.MinimumSize = new Size(1, 1);
            uiButton_downloadRecipe.Name = "uiButton_downloadRecipe";
            uiButton_downloadRecipe.RectColor = Color.FromArgb(102, 58, 183);
            uiButton_downloadRecipe.RectHoverColor = Color.FromArgb(133, 97, 198);
            uiButton_downloadRecipe.RectPressColor = Color.FromArgb(82, 46, 147);
            uiButton_downloadRecipe.RectSelectedColor = Color.FromArgb(82, 46, 147);
            uiButton_downloadRecipe.Size = new Size(135, 29);
            uiButton_downloadRecipe.Style = UIStyle.Custom;
            uiButton_downloadRecipe.TabIndex = 5;
            uiButton_downloadRecipe.Text = "Download Recipe";
            uiButton_downloadRecipe.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_downloadRecipe.Click += uiButton_downloadRecipe_Click;
            // 
            // uiLabel6
            // 
            uiLabel6.AutoSize = true;
            uiLabel6.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel6.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel6.Location = new Point(458, 81);
            uiLabel6.Name = "uiLabel6";
            uiLabel6.Size = new Size(55, 20);
            uiLabel6.TabIndex = 1;
            uiLabel6.Text = "Group";
            // 
            // uiTextBox_groupName
            // 
            uiTextBox_groupName.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_groupName.Location = new Point(538, 81);
            uiTextBox_groupName.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_groupName.MinimumSize = new Size(1, 16);
            uiTextBox_groupName.Name = "uiTextBox_groupName";
            uiTextBox_groupName.Padding = new Padding(5);
            uiTextBox_groupName.ReadOnly = true;
            uiTextBox_groupName.ShowText = false;
            uiTextBox_groupName.Size = new Size(134, 29);
            uiTextBox_groupName.TabIndex = 0;
            uiTextBox_groupName.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_groupName.Watermark = "";
            uiTextBox_groupName.TextChanged += uiTextBox_groupName_TextChanged;
            // 
            // uiCheckBox_checkRecipeBody
            // 
            uiCheckBox_checkRecipeBody.AutoSize = true;
            uiCheckBox_checkRecipeBody.Checked = true;
            uiCheckBox_checkRecipeBody.Font = new Font("Microsoft YaHei UI", 10F);
            uiCheckBox_checkRecipeBody.ForeColor = Color.FromArgb(48, 48, 48);
            uiCheckBox_checkRecipeBody.Location = new Point(232, 306);
            uiCheckBox_checkRecipeBody.MinimumSize = new Size(1, 1);
            uiCheckBox_checkRecipeBody.Name = "uiCheckBox_checkRecipeBody";
            uiCheckBox_checkRecipeBody.ReadOnly = true;
            uiCheckBox_checkRecipeBody.Size = new Size(197, 25);
            uiCheckBox_checkRecipeBody.TabIndex = 4;
            uiCheckBox_checkRecipeBody.Text = "Auto Check Recipe Body";
            uiCheckBox_checkRecipeBody.CheckedChanged += uiCheckBox_checkRecipeBody_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(728, 703);
            Controls.Add(groupBox_1);
            Controls.Add(uiButton_downloadRecipe);
            Controls.Add(button_CompareRecipe);
            Controls.Add(uiCheckBox_checkRecipeBody);
            Controls.Add(checkBox_checkrecipe);
            Controls.Add(uiButton_login);
            Controls.Add(uiLabel_admin);
            Controls.Add(richTextBox1);
            Controls.Add(label_ProcessState);
            Controls.Add(uiTextBox_groupName);
            Controls.Add(uiTextBox_line);
            Controls.Add(uiTextBox_empNo);
            Controls.Add(uiLabel6);
            Controls.Add(uiLabel3);
            Controls.Add(label_conn_status);
            Controls.Add(uiLabel2);
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
            ((System.ComponentModel.ISupportInitialize)snInfoBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public UITextBox textBox_machinerecipe;
        private UILabel label2;
        private UILabel label3;
        private UILabel label4;
        private UILabel label_conn_status;
        private UIButton button1;
        private UIButton button_getModelName;
        private UIGroupBox groupBox1;
        private UIGroupBox groupBox_1;
        private UILabel label_ProcessState;
        public UICheckBox checkBox_checkrecipe;
        private UIRichTextBox richTextBox1;
        private UIButton button_CompareRecipe;
        private NotifyIcon notifyIcon;
        private UILabel uiLabel_admin;
        private UIButton uiButton_login;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn sNDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn carrierIdDataGridViewTextBoxColumn;
        private UILabel uiLabel2;
        public UITextBox uiTextBox_empNo;
        private BindingSource snInfoBindingSource;
        private UILabel uiLabel3;
        public UITextBox uiTextBox_line;
        public UITextBox uiTextBox_modelName;
        private UILabel uiLabel4;
        private UIButton uiButton_downloadRecipe;
        public UITextBox uiTextBox_reelId;
        private UILabel uiLabel1;
        private UISymbolButton uiSymbolButton_changeReel;
        private UILabel uiLabel6;
        public UITextBox uiTextBox_groupName;
        public UICheckBox uiCheckBox_checkRecipeBody;
    }
}