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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            textBox_machinerecipe = new UITextBox();
            label2 = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            uiDataGridView_snInfo = new UIDataGridView();
            sNDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            carrierIdDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            snInfoBindingSource = new BindingSource(components);
            uiLabel_inputStatus = new UILabel();
            uiButton_ScanSn = new UIButton();
            uiTextBox_trayId = new UITextBox();
            uiTextBox_modelName = new UITextBox();
            uiLabel1 = new UILabel();
            uiLabel4 = new UILabel();
            checkBox_checkrecipe = new UICheckBox();
            label_ProcessState = new UILabel();
            button_CompareRecipe = new UIButton();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            uiGroupBox1 = new UIGroupBox();
            uiButton_update = new UIButton();
            uiButton_del = new UIButton();
            uiButton_add = new UIButton();
            uiDataGridView_Material = new UIDataGridView();
            uiLabel_admin = new UILabel();
            uiButton_login = new UIButton();
            uiTabControl1 = new UITabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            uiLabel2 = new UILabel();
            uiTextBox_empNo = new UITextBox();
            uiLabel3 = new UILabel();
            uiTextBox_line = new UITextBox();
            groupBox_1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_snInfo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)snInfoBindingSource).BeginInit();
            uiGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_Material).BeginInit();
            uiTabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_machinerecipe
            // 
            textBox_machinerecipe.Font = new Font("Microsoft YaHei UI", 11F);
            textBox_machinerecipe.Location = new Point(117, 29);
            textBox_machinerecipe.Margin = new Padding(4, 5, 4, 5);
            textBox_machinerecipe.MinimumSize = new Size(1, 16);
            textBox_machinerecipe.Name = "textBox_machinerecipe";
            textBox_machinerecipe.Padding = new Padding(5);
            textBox_machinerecipe.ReadOnly = true;
            textBox_machinerecipe.ShowText = false;
            textBox_machinerecipe.Size = new Size(202, 29);
            textBox_machinerecipe.TabIndex = 0;
            textBox_machinerecipe.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_machinerecipe.Watermark = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft YaHei UI", 11F);
            label2.ForeColor = Color.FromArgb(48, 48, 48);
            label2.Location = new Point(10, 29);
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
            groupBox_1.Controls.Add(uiDataGridView_snInfo);
            groupBox_1.Controls.Add(uiLabel_inputStatus);
            groupBox_1.Controls.Add(uiButton_ScanSn);
            groupBox_1.Controls.Add(uiTextBox_trayId);
            groupBox_1.Controls.Add(uiTextBox_modelName);
            groupBox_1.Controls.Add(textBox_machinerecipe);
            groupBox_1.Controls.Add(uiLabel1);
            groupBox_1.Controls.Add(uiLabel4);
            groupBox_1.Controls.Add(label2);
            groupBox_1.Font = new Font("宋体", 12F);
            groupBox_1.Location = new Point(8, 5);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(538, 313);
            groupBox_1.TabIndex = 7;
            groupBox_1.TabStop = false;
            groupBox_1.Text = "Info";
            groupBox_1.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // uiDataGridView_snInfo
            // 
            dataGridViewCellStyle1.BackColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_snInfo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            uiDataGridView_snInfo.AutoGenerateColumns = false;
            uiDataGridView_snInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            uiDataGridView_snInfo.BackgroundColor = Color.White;
            uiDataGridView_snInfo.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            uiDataGridView_snInfo.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            uiDataGridView_snInfo.ColumnHeadersHeight = 32;
            uiDataGridView_snInfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            uiDataGridView_snInfo.Columns.AddRange(new DataGridViewColumn[] { sNDataGridViewTextBoxColumn, carrierIdDataGridViewTextBoxColumn });
            uiDataGridView_snInfo.DataSource = snInfoBindingSource;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            uiDataGridView_snInfo.DefaultCellStyle = dataGridViewCellStyle3;
            uiDataGridView_snInfo.EnableHeadersVisualStyles = false;
            uiDataGridView_snInfo.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_snInfo.GridColor = Color.FromArgb(80, 160, 255);
            uiDataGridView_snInfo.Location = new Point(12, 115);
            uiDataGridView_snInfo.Name = "uiDataGridView_snInfo";
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle4.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle4.SelectionForeColor = Color.White;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            uiDataGridView_snInfo.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_snInfo.RowsDefaultCellStyle = dataGridViewCellStyle5;
            uiDataGridView_snInfo.SelectedIndex = -1;
            uiDataGridView_snInfo.Size = new Size(510, 150);
            uiDataGridView_snInfo.StripeOddColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_snInfo.TabIndex = 9;
            // 
            // sNDataGridViewTextBoxColumn
            // 
            sNDataGridViewTextBoxColumn.DataPropertyName = "SN";
            sNDataGridViewTextBoxColumn.HeaderText = "SN";
            sNDataGridViewTextBoxColumn.Name = "sNDataGridViewTextBoxColumn";
            // 
            // carrierIdDataGridViewTextBoxColumn
            // 
            carrierIdDataGridViewTextBoxColumn.DataPropertyName = "CarrierId";
            carrierIdDataGridViewTextBoxColumn.HeaderText = "CarrierId";
            carrierIdDataGridViewTextBoxColumn.Name = "carrierIdDataGridViewTextBoxColumn";
            // 
            // snInfoBindingSource
            // 
            snInfoBindingSource.DataSource = typeof(Models.SnInfo);
            // 
            // uiLabel_inputStatus
            // 
            uiLabel_inputStatus.BackColor = Color.Orange;
            uiLabel_inputStatus.Font = new Font("黑体", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 134);
            uiLabel_inputStatus.ForeColor = Color.White;
            uiLabel_inputStatus.Location = new Point(347, 273);
            uiLabel_inputStatus.Name = "uiLabel_inputStatus";
            uiLabel_inputStatus.Size = new Size(175, 29);
            uiLabel_inputStatus.TabIndex = 7;
            uiLabel_inputStatus.Text = "等待中";
            uiLabel_inputStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // uiButton_ScanSn
            // 
            uiButton_ScanSn.Font = new Font("宋体", 10F);
            uiButton_ScanSn.Location = new Point(386, 29);
            uiButton_ScanSn.MinimumSize = new Size(1, 1);
            uiButton_ScanSn.Name = "uiButton_ScanSn";
            uiButton_ScanSn.Size = new Size(96, 29);
            uiButton_ScanSn.TabIndex = 6;
            uiButton_ScanSn.Text = "扫码";
            uiButton_ScanSn.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_ScanSn.Click += uiButton_ScanSn_Click;
            // 
            // uiTextBox_trayId
            // 
            uiTextBox_trayId.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_trayId.Location = new Point(76, 273);
            uiTextBox_trayId.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_trayId.MinimumSize = new Size(1, 16);
            uiTextBox_trayId.Name = "uiTextBox_trayId";
            uiTextBox_trayId.Padding = new Padding(5);
            uiTextBox_trayId.ShowText = false;
            uiTextBox_trayId.Size = new Size(202, 29);
            uiTextBox_trayId.TabIndex = 0;
            uiTextBox_trayId.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_trayId.Watermark = "";
            // 
            // uiTextBox_modelName
            // 
            uiTextBox_modelName.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_modelName.Location = new Point(117, 70);
            uiTextBox_modelName.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_modelName.MinimumSize = new Size(1, 16);
            uiTextBox_modelName.Name = "uiTextBox_modelName";
            uiTextBox_modelName.Padding = new Padding(5);
            uiTextBox_modelName.ReadOnly = true;
            uiTextBox_modelName.ShowText = false;
            uiTextBox_modelName.Size = new Size(202, 29);
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
            uiLabel1.Location = new Point(10, 277);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(59, 20);
            uiLabel1.TabIndex = 1;
            uiLabel1.Text = "Tray ID";
            // 
            // uiLabel4
            // 
            uiLabel4.AutoSize = true;
            uiLabel4.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel4.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel4.Location = new Point(10, 70);
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
            checkBox_checkrecipe.Location = new Point(553, 20);
            checkBox_checkrecipe.MinimumSize = new Size(1, 1);
            checkBox_checkrecipe.Name = "checkBox_checkrecipe";
            checkBox_checkrecipe.Size = new Size(105, 25);
            checkBox_checkrecipe.TabIndex = 4;
            checkBox_checkrecipe.Text = "检查Recipe";
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
            button_CompareRecipe.Location = new Point(553, 51);
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
            richTextBox1.Location = new Point(14, 477);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(2);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(674, 114);
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
            // uiGroupBox1
            // 
            uiGroupBox1.Controls.Add(uiButton_update);
            uiGroupBox1.Controls.Add(uiButton_del);
            uiGroupBox1.Controls.Add(uiButton_add);
            uiGroupBox1.Controls.Add(uiDataGridView_Material);
            uiGroupBox1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiGroupBox1.Location = new Point(8, 7);
            uiGroupBox1.Margin = new Padding(4, 5, 4, 5);
            uiGroupBox1.MinimumSize = new Size(1, 1);
            uiGroupBox1.Name = "uiGroupBox1";
            uiGroupBox1.Padding = new Padding(0, 32, 0, 0);
            uiGroupBox1.Size = new Size(662, 321);
            uiGroupBox1.TabIndex = 9;
            uiGroupBox1.Text = "Cathode";
            uiGroupBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiGroupBox1.Click += uiGroupBox1_Click;
            // 
            // uiButton_update
            // 
            uiButton_update.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_update.Location = new Point(245, 35);
            uiButton_update.MinimumSize = new Size(1, 1);
            uiButton_update.Name = "uiButton_update";
            uiButton_update.Size = new Size(100, 35);
            uiButton_update.TabIndex = 11;
            uiButton_update.Text = "更新";
            uiButton_update.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // uiButton_del
            // 
            uiButton_del.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_del.Location = new Point(131, 35);
            uiButton_del.MinimumSize = new Size(1, 1);
            uiButton_del.Name = "uiButton_del";
            uiButton_del.Size = new Size(100, 35);
            uiButton_del.TabIndex = 11;
            uiButton_del.Text = "删除";
            uiButton_del.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_del.Click += uiButton_del_Click;
            // 
            // uiButton_add
            // 
            uiButton_add.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_add.Location = new Point(14, 35);
            uiButton_add.MinimumSize = new Size(1, 1);
            uiButton_add.Name = "uiButton_add";
            uiButton_add.Size = new Size(100, 35);
            uiButton_add.TabIndex = 11;
            uiButton_add.Text = "添加";
            uiButton_add.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_add.Click += uiButton_add_Click;
            // 
            // uiDataGridView_Material
            // 
            uiDataGridView_Material.AllowUserToAddRows = false;
            uiDataGridView_Material.AllowUserToDeleteRows = false;
            uiDataGridView_Material.AllowUserToResizeColumns = false;
            uiDataGridView_Material.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.BackColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_Material.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            uiDataGridView_Material.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            uiDataGridView_Material.BackgroundColor = Color.White;
            uiDataGridView_Material.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle7.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle7.ForeColor = Color.White;
            dataGridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            uiDataGridView_Material.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            uiDataGridView_Material.ColumnHeadersHeight = 32;
            uiDataGridView_Material.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = SystemColors.Window;
            dataGridViewCellStyle8.Font = new Font("宋体", 15F);
            dataGridViewCellStyle8.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.False;
            uiDataGridView_Material.DefaultCellStyle = dataGridViewCellStyle8;
            uiDataGridView_Material.EnableHeadersVisualStyles = false;
            uiDataGridView_Material.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_Material.GridColor = Color.FromArgb(80, 160, 255);
            uiDataGridView_Material.Location = new Point(14, 91);
            uiDataGridView_Material.Name = "uiDataGridView_Material";
            uiDataGridView_Material.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle9.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle9.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle9.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle9.SelectionForeColor = Color.White;
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.True;
            uiDataGridView_Material.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            uiDataGridView_Material.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle10.BackColor = Color.White;
            dataGridViewCellStyle10.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_Material.RowsDefaultCellStyle = dataGridViewCellStyle10;
            uiDataGridView_Material.RowTemplate.Height = 23;
            uiDataGridView_Material.SelectedIndex = -1;
            uiDataGridView_Material.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            uiDataGridView_Material.Size = new Size(632, 218);
            uiDataGridView_Material.StripeOddColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_Material.TabIndex = 10;
            // 
            // uiLabel_admin
            // 
            uiLabel_admin.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiLabel_admin.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel_admin.Location = new Point(491, 51);
            uiLabel_admin.Name = "uiLabel_admin";
            uiLabel_admin.Size = new Size(150, 23);
            uiLabel_admin.TabIndex = 10;
            uiLabel_admin.Text = "普通用户";
            // 
            // uiButton_login
            // 
            uiButton_login.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_login.Location = new Point(588, 47);
            uiButton_login.MinimumSize = new Size(1, 1);
            uiButton_login.Name = "uiButton_login";
            uiButton_login.Size = new Size(100, 35);
            uiButton_login.TabIndex = 11;
            uiButton_login.Text = "登录";
            uiButton_login.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_login.Click += uiButton_login_Click;
            // 
            // uiTabControl1
            // 
            uiTabControl1.Controls.Add(tabPage1);
            uiTabControl1.Controls.Add(tabPage2);
            uiTabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            uiTabControl1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiTabControl1.ItemSize = new Size(150, 23);
            uiTabControl1.Location = new Point(14, 118);
            uiTabControl1.MainPage = "";
            uiTabControl1.Name = "uiTabControl1";
            uiTabControl1.SelectedIndex = 0;
            uiTabControl1.Size = new Size(674, 351);
            uiTabControl1.SizeMode = TabSizeMode.Fixed;
            uiTabControl1.TabIndex = 12;
            uiTabControl1.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            uiTabControl1.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiTabControl1.SelectedIndexChanged += uiTabControl1_SelectedIndexChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox_1);
            tabPage1.Controls.Add(button_CompareRecipe);
            tabPage1.Controls.Add(checkBox_checkrecipe);
            tabPage1.Location = new Point(0, 23);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new Size(674, 328);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "过站";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(uiGroupBox1);
            tabPage2.Location = new Point(0, 40);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(200, 60);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Cathode";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // uiLabel2
            // 
            uiLabel2.AutoSize = true;
            uiLabel2.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(34, 83);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(39, 20);
            uiLabel2.TabIndex = 1;
            uiLabel2.Text = "工号";
            // 
            // uiTextBox_empNo
            // 
            uiTextBox_empNo.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_empNo.Location = new Point(80, 81);
            uiTextBox_empNo.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_empNo.MinimumSize = new Size(1, 16);
            uiTextBox_empNo.Name = "uiTextBox_empNo";
            uiTextBox_empNo.Padding = new Padding(5);
            uiTextBox_empNo.ShowText = false;
            uiTextBox_empNo.Size = new Size(202, 29);
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
            uiLabel3.Location = new Point(312, 83);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(39, 20);
            uiLabel3.TabIndex = 1;
            uiLabel3.Text = "线体";
            // 
            // uiTextBox_line
            // 
            uiTextBox_line.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox_line.Location = new Point(358, 81);
            uiTextBox_line.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_line.MinimumSize = new Size(1, 16);
            uiTextBox_line.Name = "uiTextBox_line";
            uiTextBox_line.Padding = new Padding(5);
            uiTextBox_line.ShowText = false;
            uiTextBox_line.Size = new Size(202, 29);
            uiTextBox_line.TabIndex = 0;
            uiTextBox_line.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_line.Watermark = "";
            uiTextBox_line.TextChanged += uiTextBox_line_TextChanged;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(703, 603);
            Controls.Add(uiTabControl1);
            Controls.Add(uiButton_login);
            Controls.Add(uiLabel_admin);
            Controls.Add(richTextBox1);
            Controls.Add(label_ProcessState);
            Controls.Add(uiTextBox_line);
            Controls.Add(uiTextBox_empNo);
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
            SizeChanged += MainForm_SizeChanged;
            groupBox_1.ResumeLayout(false);
            groupBox_1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_snInfo).EndInit();
            ((System.ComponentModel.ISupportInitialize)snInfoBindingSource).EndInit();
            uiGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_Material).EndInit();
            uiTabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private UITextBox textBox_machinerecipe;
        private UILabel label2;
        private UILabel label3;
        private UILabel label4;
        private UILabel label_conn_status;
        private UIButton button1;
        private UIButton button_getModelName;
        private UIGroupBox groupBox1;
        private UIGroupBox groupBox_1;
        private UILabel label_ProcessState;
        private UICheckBox checkBox_checkrecipe;
        private UIRichTextBox richTextBox1;
        private UIButton button_CompareRecipe;
        private NotifyIcon notifyIcon;
        private UIGroupBox uiGroupBox1;
        private UIDataGridView uiDataGridView_Material;
        private UILabel uiLabel_admin;
        private UIButton uiButton_login;
        private UIButton uiButton_del;
        private UIButton uiButton_add;
        private UIButton uiButton_update;
        private UITabControl uiTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        public UITextBox uiTextBox_trayId;
        private UILabel uiLabel1;
        private DataGridViewTextBoxColumn sNDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn carrierIdDataGridViewTextBoxColumn;
        private UIButton uiButton_ScanSn;
        private UILabel uiLabel2;
        public UITextBox uiTextBox_empNo;
        private UILabel uiLabel_inputStatus;
        private UIDataGridView uiDataGridView_snInfo;
        private BindingSource snInfoBindingSource;
        private UILabel uiLabel3;
        public UITextBox uiTextBox_line;
        public UITextBox uiTextBox_modelName;
        private UILabel uiLabel4;
    }
}