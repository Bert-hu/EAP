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
            DataGridViewCellStyle dataGridViewCellStyle11 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle12 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle13 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle14 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle15 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DataGridViewCellStyle dataGridViewCellStyle16 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle17 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle18 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle19 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle20 = new DataGridViewCellStyle();
            textBox_machinerecipe = new UITextBox();
            label2 = new UILabel();
            label_updatetime_aoi = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            uiButton_ScanSn = new UIButton();
            uiDataGridView1 = new UIDataGridView();
            sNDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            carrierIdDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            snInfoBindingSource = new BindingSource(components);
            checkBox_checkrecipe = new UICheckBox();
            uiTextBox1 = new UITextBox();
            uiLabel1 = new UILabel();
            label_ProcessState = new UILabel();
            button_CompareRecipe = new UIButton();
            richTextBox1 = new UIRichTextBox();
            notifyIcon = new NotifyIcon(components);
            uiGroupBox1 = new UIGroupBox();
            uiButton_check = new UIButton();
            uiButton_del = new UIButton();
            uiButton_add = new UIButton();
            uiDataGridView_Material = new UIDataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            bindingSource = new BindingSource(components);
            uiLabel_admin = new UILabel();
            uiButton_login = new UIButton();
            uiTabControl1 = new UITabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            uiLabel2 = new UILabel();
            uiTextBox_empNo = new UITextBox();
            groupBox_1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)snInfoBindingSource).BeginInit();
            uiGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_Material).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
            uiTabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // textBox_machinerecipe
            // 
            textBox_machinerecipe.Font = new Font("Microsoft YaHei UI", 11F);
            textBox_machinerecipe.Location = new Point(76, 29);
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
            // label_updatetime_aoi
            // 
            label_updatetime_aoi.AutoSize = true;
            label_updatetime_aoi.Font = new Font("宋体", 10F);
            label_updatetime_aoi.ForeColor = Color.FromArgb(48, 48, 48);
            label_updatetime_aoi.Location = new Point(163, 289);
            label_updatetime_aoi.Name = "label_updatetime_aoi";
            label_updatetime_aoi.Size = new Size(91, 14);
            label_updatetime_aoi.TabIndex = 2;
            label_updatetime_aoi.Text = "Update Time:";
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
            groupBox_1.Controls.Add(uiButton_ScanSn);
            groupBox_1.Controls.Add(uiDataGridView1);
            groupBox_1.Controls.Add(checkBox_checkrecipe);
            groupBox_1.Controls.Add(label_updatetime_aoi);
            groupBox_1.Controls.Add(uiTextBox1);
            groupBox_1.Controls.Add(textBox_machinerecipe);
            groupBox_1.Controls.Add(uiLabel1);
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
            // uiButton_ScanSn
            // 
            uiButton_ScanSn.Font = new Font("宋体", 10F);
            uiButton_ScanSn.Location = new Point(317, 29);
            uiButton_ScanSn.MinimumSize = new Size(1, 1);
            uiButton_ScanSn.Name = "uiButton_ScanSn";
            uiButton_ScanSn.Size = new Size(96, 29);
            uiButton_ScanSn.TabIndex = 6;
            uiButton_ScanSn.Text = "扫码";
            uiButton_ScanSn.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_ScanSn.Click += uiButton_ScanSn_Click;
            // 
            // uiDataGridView1
            // 
            uiDataGridView1.AllowUserToAddRows = false;
            uiDataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle11.BackColor = Color.FromArgb(235, 243, 255);
            uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle11;
            uiDataGridView1.AutoGenerateColumns = false;
            uiDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            uiDataGridView1.BackgroundColor = Color.White;
            uiDataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle12.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle12.ForeColor = Color.White;
            dataGridViewCellStyle12.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle12;
            uiDataGridView1.ColumnHeadersHeight = 32;
            uiDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            uiDataGridView1.Columns.AddRange(new DataGridViewColumn[] { sNDataGridViewTextBoxColumn, carrierIdDataGridViewTextBoxColumn });
            uiDataGridView1.DataSource = snInfoBindingSource;
            dataGridViewCellStyle13.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = SystemColors.Window;
            dataGridViewCellStyle13.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle13.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle13.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = DataGridViewTriState.False;
            uiDataGridView1.DefaultCellStyle = dataGridViewCellStyle13;
            uiDataGridView1.EnableHeadersVisualStyles = false;
            uiDataGridView1.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView1.GridColor = Color.FromArgb(80, 160, 255);
            uiDataGridView1.Location = new Point(12, 79);
            uiDataGridView1.Name = "uiDataGridView1";
            uiDataGridView1.ReadOnly = true;
            dataGridViewCellStyle14.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle14.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle14.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle14.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle14.SelectionForeColor = Color.White;
            dataGridViewCellStyle14.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle14;
            dataGridViewCellStyle15.BackColor = Color.White;
            dataGridViewCellStyle15.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle15;
            uiDataGridView1.SelectedIndex = -1;
            uiDataGridView1.Size = new Size(510, 159);
            uiDataGridView1.StripeOddColor = Color.FromArgb(235, 243, 255);
            uiDataGridView1.TabIndex = 5;
            // 
            // sNDataGridViewTextBoxColumn
            // 
            sNDataGridViewTextBoxColumn.DataPropertyName = "SN";
            sNDataGridViewTextBoxColumn.HeaderText = "SN";
            sNDataGridViewTextBoxColumn.Name = "sNDataGridViewTextBoxColumn";
            sNDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // carrierIdDataGridViewTextBoxColumn
            // 
            carrierIdDataGridViewTextBoxColumn.DataPropertyName = "CarrierId";
            carrierIdDataGridViewTextBoxColumn.HeaderText = "CarrierId";
            carrierIdDataGridViewTextBoxColumn.Name = "carrierIdDataGridViewTextBoxColumn";
            carrierIdDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // snInfoBindingSource
            // 
            snInfoBindingSource.DataSource = typeof(Models.SnInfo);
            // 
            // checkBox_checkrecipe
            // 
            checkBox_checkrecipe.AutoSize = true;
            checkBox_checkrecipe.Checked = true;
            checkBox_checkrecipe.Font = new Font("Microsoft YaHei UI", 10F);
            checkBox_checkrecipe.ForeColor = Color.FromArgb(48, 48, 48);
            checkBox_checkrecipe.Location = new Point(10, 283);
            checkBox_checkrecipe.MinimumSize = new Size(1, 1);
            checkBox_checkrecipe.Name = "checkBox_checkrecipe";
            checkBox_checkrecipe.Size = new Size(133, 25);
            checkBox_checkrecipe.TabIndex = 4;
            checkBox_checkrecipe.Text = "自动检查Recipe";
            checkBox_checkrecipe.CheckedChanged += checkBox_checkrecipe_CheckedChanged;
            // 
            // uiTextBox1
            // 
            uiTextBox1.Font = new Font("Microsoft YaHei UI", 11F);
            uiTextBox1.Location = new Point(76, 246);
            uiTextBox1.Margin = new Padding(4, 5, 4, 5);
            uiTextBox1.MinimumSize = new Size(1, 16);
            uiTextBox1.Name = "uiTextBox1";
            uiTextBox1.Padding = new Padding(5);
            uiTextBox1.ShowText = false;
            uiTextBox1.Size = new Size(202, 29);
            uiTextBox1.TabIndex = 0;
            uiTextBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox1.Watermark = "";
            // 
            // uiLabel1
            // 
            uiLabel1.AutoSize = true;
            uiLabel1.Font = new Font("Microsoft YaHei UI", 11F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(10, 250);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(59, 20);
            uiLabel1.TabIndex = 1;
            uiLabel1.Text = "Tray ID";
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
            button_CompareRecipe.Location = new Point(553, 21);
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
            uiGroupBox1.Controls.Add(uiButton_check);
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
            // 
            // uiButton_check
            // 
            uiButton_check.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_check.Location = new Point(245, 35);
            uiButton_check.MinimumSize = new Size(1, 1);
            uiButton_check.Name = "uiButton_check";
            uiButton_check.Size = new Size(100, 35);
            uiButton_check.TabIndex = 11;
            uiButton_check.Text = "检查";
            uiButton_check.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
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
            dataGridViewCellStyle16.BackColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_Material.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle16;
            uiDataGridView_Material.AutoGenerateColumns = false;
            uiDataGridView_Material.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            uiDataGridView_Material.BackgroundColor = Color.White;
            uiDataGridView_Material.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle17.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle17.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle17.ForeColor = Color.White;
            dataGridViewCellStyle17.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = DataGridViewTriState.True;
            uiDataGridView_Material.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            uiDataGridView_Material.ColumnHeadersHeight = 32;
            uiDataGridView_Material.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            uiDataGridView_Material.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            uiDataGridView_Material.DataSource = bindingSource;
            dataGridViewCellStyle18.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = SystemColors.Window;
            dataGridViewCellStyle18.Font = new Font("宋体", 15F);
            dataGridViewCellStyle18.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle18.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle18.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle18.WrapMode = DataGridViewTriState.False;
            uiDataGridView_Material.DefaultCellStyle = dataGridViewCellStyle18;
            uiDataGridView_Material.EnableHeadersVisualStyles = false;
            uiDataGridView_Material.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_Material.GridColor = Color.FromArgb(80, 160, 255);
            uiDataGridView_Material.Location = new Point(14, 91);
            uiDataGridView_Material.Name = "uiDataGridView_Material";
            uiDataGridView_Material.ReadOnly = true;
            dataGridViewCellStyle19.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle19.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle19.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle19.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle19.SelectionForeColor = Color.White;
            dataGridViewCellStyle19.WrapMode = DataGridViewTriState.True;
            uiDataGridView_Material.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
            uiDataGridView_Material.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle20.BackColor = Color.White;
            dataGridViewCellStyle20.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_Material.RowsDefaultCellStyle = dataGridViewCellStyle20;
            uiDataGridView_Material.RowTemplate.Height = 23;
            uiDataGridView_Material.SelectedIndex = -1;
            uiDataGridView_Material.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            uiDataGridView_Material.Size = new Size(632, 218);
            uiDataGridView_Material.StripeOddColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_Material.TabIndex = 10;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.DataPropertyName = "Seq";
            dataGridViewTextBoxColumn1.FillWeight = 101.522842F;
            dataGridViewTextBoxColumn1.HeaderText = "Seq";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.DataPropertyName = "CathodeId";
            dataGridViewTextBoxColumn2.FillWeight = 98.47716F;
            dataGridViewTextBoxColumn2.HeaderText = "CathodeId";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // bindingSource
            // 
            bindingSource.DataSource = typeof(Models.CathodeSetting);
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
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox_1);
            tabPage1.Controls.Add(button_CompareRecipe);
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
            tabPage2.Location = new Point(0, 23);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new Size(674, 333);
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
            uiTextBox_empNo.Location = new Point(98, 83);
            uiTextBox_empNo.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_empNo.MinimumSize = new Size(1, 16);
            uiTextBox_empNo.Name = "uiTextBox_empNo";
            uiTextBox_empNo.Padding = new Padding(5);
            uiTextBox_empNo.ShowText = false;
            uiTextBox_empNo.Size = new Size(202, 29);
            uiTextBox_empNo.TabIndex = 0;
            uiTextBox_empNo.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_empNo.Watermark = "";
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
            Controls.Add(uiTextBox_empNo);
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
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)snInfoBindingSource).EndInit();
            uiGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_Material).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
            uiTabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
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
        private UIButton button_CompareRecipe;
        private NotifyIcon notifyIcon;
        private UIGroupBox uiGroupBox1;
        private UIDataGridView uiDataGridView_Material;
        private System.Windows.Forms.DataGridViewTextBoxColumn seqDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cathodeIdDataGridViewTextBoxColumn;
        private BindingSource bindingSource;
        private UILabel uiLabel_admin;
        private UIButton uiButton_login;
        private UIButton uiButton_del;
        private UIButton uiButton_add;
        private UIButton uiButton_check;
        private UITabControl uiTabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private UIDataGridView uiDataGridView1;
        private UITextBox uiTextBox1;
        private UILabel uiLabel1;
        private DataGridViewTextBoxColumn sNDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn carrierIdDataGridViewTextBoxColumn;
        private BindingSource snInfoBindingSource;
        private UIButton uiButton_ScanSn;
        private UILabel uiLabel2;
        private UITextBox uiTextBox_empNo;
    }
}