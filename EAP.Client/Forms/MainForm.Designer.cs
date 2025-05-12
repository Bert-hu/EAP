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
            DataGridViewCellStyle dataGridViewCellStyle11 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle12 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle13 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle14 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle15 = new DataGridViewCellStyle();
            label1 = new UILabel();
            textBox_modelname = new UITextBox();
            textBox_machinerecipe = new UITextBox();
            label2 = new UILabel();
            label_updatetime_aoi = new UILabel();
            label_conn_status = new UILabel();
            groupBox_1 = new UIGroupBox();
            checkBox_checkrecipe = new UICheckBox();
            textBox_projectname = new UITextBox();
            label5 = new UILabel();
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
            groupBox_1.SuspendLayout();
            uiGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_Material).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 11F);
            label1.ForeColor = Color.FromArgb(48, 48, 48);
            label1.Location = new Point(10, 70);
            label1.Name = "label1";
            label1.Size = new Size(104, 20);
            label1.TabIndex = 1;
            label1.Text = "Model Name";
            // 
            // textBox_modelname
            // 
            textBox_modelname.Font = new Font("Microsoft YaHei UI", 11F);
            textBox_modelname.Location = new Point(163, 70);
            textBox_modelname.Margin = new Padding(4, 5, 4, 5);
            textBox_modelname.MinimumSize = new Size(1, 16);
            textBox_modelname.Name = "textBox_modelname";
            textBox_modelname.Padding = new Padding(5);
            textBox_modelname.ShowText = false;
            textBox_modelname.Size = new Size(202, 29);
            textBox_modelname.TabIndex = 0;
            textBox_modelname.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_modelname.Watermark = "";
            // 
            // textBox_machinerecipe
            // 
            textBox_machinerecipe.Font = new Font("Microsoft YaHei UI", 11F);
            textBox_machinerecipe.Location = new Point(163, 29);
            textBox_machinerecipe.Margin = new Padding(4, 5, 4, 5);
            textBox_machinerecipe.MinimumSize = new Size(1, 16);
            textBox_machinerecipe.Name = "textBox_machinerecipe";
            textBox_machinerecipe.Padding = new Padding(5);
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
            label2.Size = new Size(125, 20);
            label2.TabIndex = 1;
            label2.Text = "Machine Recipe";
            // 
            // label_updatetime_aoi
            // 
            label_updatetime_aoi.AutoSize = true;
            label_updatetime_aoi.Font = new Font("宋体", 12F);
            label_updatetime_aoi.ForeColor = Color.FromArgb(48, 48, 48);
            label_updatetime_aoi.Location = new Point(11, 237);
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
            label_conn_status.Location = new Point(22, 47);
            label_conn_status.Name = "label_conn_status";
            label_conn_status.Size = new Size(120, 27);
            label_conn_status.TabIndex = 3;
            label_conn_status.Text = "Connecting";
            // 
            // groupBox_1
            // 
            groupBox_1.Controls.Add(textBox_projectname);
            groupBox_1.Controls.Add(checkBox_checkrecipe);
            groupBox_1.Controls.Add(textBox_modelname);
            groupBox_1.Controls.Add(label_updatetime_aoi);
            groupBox_1.Controls.Add(textBox_machinerecipe);
            groupBox_1.Controls.Add(label2);
            groupBox_1.Controls.Add(label5);
            groupBox_1.Controls.Add(label1);
            groupBox_1.Font = new Font("宋体", 12F);
            groupBox_1.Location = new Point(12, 110);
            groupBox_1.Margin = new Padding(4, 5, 4, 5);
            groupBox_1.MinimumSize = new Size(1, 1);
            groupBox_1.Name = "groupBox_1";
            groupBox_1.Padding = new Padding(0, 32, 0, 0);
            groupBox_1.Size = new Size(471, 321);
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
            checkBox_checkrecipe.Location = new Point(10, 277);
            checkBox_checkrecipe.MinimumSize = new Size(1, 1);
            checkBox_checkrecipe.Name = "checkBox_checkrecipe";
            checkBox_checkrecipe.Size = new Size(178, 32);
            checkBox_checkrecipe.TabIndex = 4;
            checkBox_checkrecipe.Text = "自动检查Recipe";
            checkBox_checkrecipe.CheckedChanged += checkBox_checkrecipe_CheckedChanged;
            // 
            // textBox_projectname
            // 
            textBox_projectname.Font = new Font("Microsoft YaHei UI", 11F);
            textBox_projectname.Location = new Point(163, 109);
            textBox_projectname.Margin = new Padding(4, 5, 4, 5);
            textBox_projectname.MinimumSize = new Size(1, 16);
            textBox_projectname.Name = "textBox_projectname";
            textBox_projectname.Padding = new Padding(5);
            textBox_projectname.ShowText = false;
            textBox_projectname.Size = new Size(202, 29);
            textBox_projectname.TabIndex = 0;
            textBox_projectname.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_projectname.Watermark = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft YaHei UI", 11F);
            label5.ForeColor = Color.FromArgb(48, 48, 48);
            label5.Location = new Point(10, 109);
            label5.Name = "label5";
            label5.Size = new Size(108, 20);
            label5.TabIndex = 1;
            label5.Text = "Project Name";
            // 
            // label_ProcessState
            // 
            label_ProcessState.AutoSize = true;
            label_ProcessState.BackColor = Color.Gray;
            label_ProcessState.Font = new Font("Microsoft YaHei UI", 15F);
            label_ProcessState.ForeColor = Color.White;
            label_ProcessState.Location = new Point(175, 47);
            label_ProcessState.Name = "label_ProcessState";
            label_ProcessState.Size = new Size(103, 27);
            label_ProcessState.TabIndex = 3;
            label_ProcessState.Text = "Unknown";
            label_ProcessState.Visible = false;
            // 
            // button_CompareRecipe
            // 
            button_CompareRecipe.Font = new Font("Microsoft YaHei UI", 11F);
            button_CompareRecipe.Location = new Point(22, 77);
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
            richTextBox1.Location = new Point(14, 441);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.MinimumSize = new Size(1, 1);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Padding = new Padding(2);
            richTextBox1.ReadOnly = true;
            richTextBox1.ShowText = false;
            richTextBox1.Size = new Size(896, 150);
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
            uiGroupBox1.Location = new Point(491, 110);
            uiGroupBox1.Margin = new Padding(4, 5, 4, 5);
            uiGroupBox1.MinimumSize = new Size(1, 1);
            uiGroupBox1.Name = "uiGroupBox1";
            uiGroupBox1.Padding = new Padding(0, 32, 0, 0);
            uiGroupBox1.Size = new Size(419, 321);
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
            dataGridViewCellStyle11.BackColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_Material.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle11;
            uiDataGridView_Material.AutoGenerateColumns = false;
            uiDataGridView_Material.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            uiDataGridView_Material.BackgroundColor = Color.White;
            uiDataGridView_Material.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle12.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle12.ForeColor = Color.White;
            dataGridViewCellStyle12.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = DataGridViewTriState.True;
            uiDataGridView_Material.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle12;
            uiDataGridView_Material.ColumnHeadersHeight = 32;
            uiDataGridView_Material.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            uiDataGridView_Material.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            uiDataGridView_Material.DataSource = bindingSource;
            dataGridViewCellStyle13.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = SystemColors.Window;
            dataGridViewCellStyle13.Font = new Font("宋体", 15F);
            dataGridViewCellStyle13.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle13.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = DataGridViewTriState.False;
            uiDataGridView_Material.DefaultCellStyle = dataGridViewCellStyle13;
            uiDataGridView_Material.EnableHeadersVisualStyles = false;
            uiDataGridView_Material.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_Material.GridColor = Color.FromArgb(80, 160, 255);
            uiDataGridView_Material.Location = new Point(14, 91);
            uiDataGridView_Material.Name = "uiDataGridView_Material";
            uiDataGridView_Material.ReadOnly = true;
            dataGridViewCellStyle14.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle14.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle14.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle14.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle14.SelectionForeColor = Color.White;
            dataGridViewCellStyle14.WrapMode = DataGridViewTriState.True;
            uiDataGridView_Material.RowHeadersDefaultCellStyle = dataGridViewCellStyle14;
            uiDataGridView_Material.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle15.BackColor = Color.White;
            dataGridViewCellStyle15.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiDataGridView_Material.RowsDefaultCellStyle = dataGridViewCellStyle15;
            uiDataGridView_Material.RowTemplate.Height = 23;
            uiDataGridView_Material.SelectedIndex = -1;
            uiDataGridView_Material.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            uiDataGridView_Material.Size = new Size(392, 218);
            uiDataGridView_Material.StripeOddColor = Color.FromArgb(235, 243, 255);
            uiDataGridView_Material.TabIndex = 10;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.DataPropertyName = "Seq";
            dataGridViewTextBoxColumn1.HeaderText = "Seq";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.DataPropertyName = "CathodeId";
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
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(924, 603);
            Controls.Add(uiButton_login);
            Controls.Add(uiLabel_admin);
            Controls.Add(uiGroupBox1);
            Controls.Add(richTextBox1);
            Controls.Add(label_ProcessState);
            Controls.Add(button_CompareRecipe);
            Controls.Add(label_conn_status);
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
            uiGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)uiDataGridView_Material).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
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
        private UITextBox textBox_projectname;
        private UILabel label5;
        private UIButton button_CompareRecipe;
        private NotifyIcon notifyIcon;
        private UIGroupBox uiGroupBox1;
        private UIDataGridView uiDataGridView_Material;
        private System.Windows.Forms.DataGridViewTextBoxColumn seqDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cathodeIdDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private BindingSource bindingSource;
        private UILabel uiLabel_admin;
        private UIButton uiButton_login;
        private UIButton uiButton_del;
        private UIButton uiButton_add;
        private UIButton uiButton_check;
    }
}