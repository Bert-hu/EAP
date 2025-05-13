using Sunny.UI;

namespace EAP.Client.Forms
{
    partial class MixPackageSettingForm
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
            textBox_icos = new UITextBox();
            button_confirm = new UIButton();
            uiTextBox_m = new UITextBox();
            uiTextBox_oh = new UITextBox();
            uiLabel3 = new UILabel();
            uiLabel2 = new UILabel();
            uiLabel1 = new UILabel();
            uiComboBox_packageName = new UIComboBox();
            uiButton_add = new UIButton();
            SuspendLayout();
            // 
            // textBox_icos
            // 
            textBox_icos.Font = new Font("Microsoft YaHei UI", 20F);
            textBox_icos.Location = new Point(139, 131);
            textBox_icos.Margin = new Padding(4, 5, 4, 5);
            textBox_icos.MinimumSize = new Size(1, 16);
            textBox_icos.Name = "textBox_icos";
            textBox_icos.Padding = new Padding(5);
            textBox_icos.ShowText = false;
            textBox_icos.Size = new Size(361, 41);
            textBox_icos.TabIndex = 0;
            textBox_icos.TextAlignment = ContentAlignment.MiddleLeft;
            textBox_icos.Watermark = "";
            // 
            // button_confirm
            // 
            button_confirm.Font = new Font("宋体", 12F);
            button_confirm.Location = new Point(382, 296);
            button_confirm.MinimumSize = new Size(1, 1);
            button_confirm.Name = "button_confirm";
            button_confirm.Size = new Size(118, 30);
            button_confirm.TabIndex = 1;
            button_confirm.Text = "保存并关闭";
            button_confirm.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_confirm.Click += button_confirm_Click;
            // 
            // uiTextBox_m
            // 
            uiTextBox_m.Font = new Font("Microsoft YaHei UI", 20F);
            uiTextBox_m.Location = new Point(139, 182);
            uiTextBox_m.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_m.MinimumSize = new Size(1, 16);
            uiTextBox_m.Name = "uiTextBox_m";
            uiTextBox_m.Padding = new Padding(5);
            uiTextBox_m.ShowText = false;
            uiTextBox_m.Size = new Size(361, 41);
            uiTextBox_m.TabIndex = 0;
            uiTextBox_m.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_m.Watermark = "";
            // 
            // uiTextBox_oh
            // 
            uiTextBox_oh.Font = new Font("Microsoft YaHei UI", 20F);
            uiTextBox_oh.Location = new Point(139, 233);
            uiTextBox_oh.Margin = new Padding(4, 5, 4, 5);
            uiTextBox_oh.MinimumSize = new Size(1, 16);
            uiTextBox_oh.Name = "uiTextBox_oh";
            uiTextBox_oh.Padding = new Padding(5);
            uiTextBox_oh.ShowText = false;
            uiTextBox_oh.Size = new Size(361, 41);
            uiTextBox_oh.TabIndex = 0;
            uiTextBox_oh.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox_oh.Watermark = "";
            // 
            // uiLabel3
            // 
            uiLabel3.AutoSize = true;
            uiLabel3.Font = new Font("Microsoft YaHei UI", 15F);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(25, 233);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(91, 27);
            uiLabel3.TabIndex = 2;
            uiLabel3.Text = "MIX-OH";
            // 
            // uiLabel2
            // 
            uiLabel2.AutoSize = true;
            uiLabel2.Font = new Font("Microsoft YaHei UI", 15F);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(25, 182);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(80, 27);
            uiLabel2.TabIndex = 3;
            uiLabel2.Text = "MIX-M";
            // 
            // uiLabel1
            // 
            uiLabel1.AutoSize = true;
            uiLabel1.Font = new Font("Microsoft YaHei UI", 15F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(25, 131);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(107, 27);
            uiLabel1.TabIndex = 4;
            uiLabel1.Text = "MIX-ICOS";
            // 
            // uiComboBox_packageName
            // 
            uiComboBox_packageName.DataSource = null;
            uiComboBox_packageName.FillColor = Color.White;
            uiComboBox_packageName.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiComboBox_packageName.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox_packageName.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox_packageName.Location = new Point(25, 63);
            uiComboBox_packageName.Margin = new Padding(4, 5, 4, 5);
            uiComboBox_packageName.MinimumSize = new Size(63, 0);
            uiComboBox_packageName.Name = "uiComboBox_packageName";
            uiComboBox_packageName.Padding = new Padding(0, 0, 30, 2);
            uiComboBox_packageName.Size = new Size(258, 29);
            uiComboBox_packageName.SymbolSize = 24;
            uiComboBox_packageName.TabIndex = 5;
            uiComboBox_packageName.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox_packageName.Watermark = "";
            uiComboBox_packageName.SelectedIndexChanged += uiComboBox_packageName_SelectedIndexChanged;
            // 
            // uiButton_add
            // 
            uiButton_add.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            uiButton_add.Location = new Point(409, 63);
            uiButton_add.MinimumSize = new Size(1, 1);
            uiButton_add.Name = "uiButton_add";
            uiButton_add.Size = new Size(91, 29);
            uiButton_add.TabIndex = 6;
            uiButton_add.Text = "新增";
            uiButton_add.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            // 
            // MixPackageSettingForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(527, 349);
            Controls.Add(uiButton_add);
            Controls.Add(uiComboBox_packageName);
            Controls.Add(uiLabel3);
            Controls.Add(uiLabel2);
            Controls.Add(uiLabel1);
            Controls.Add(button_confirm);
            Controls.Add(uiTextBox_oh);
            Controls.Add(uiTextBox_m);
            Controls.Add(textBox_icos);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MixPackageSettingForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "修改设定";
            TopMost = true;
            ZoomScaleRect = new Rectangle(15, 15, 448, 109);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private UITextBox textBox_icos;
        private UIButton button_confirm;
        private UITextBox uiTextBox_m;
        private UITextBox uiTextBox_oh;
        private UILabel uiLabel3;
        private UILabel uiLabel2;
        private UILabel uiLabel1;
        private UIComboBox uiComboBox_packageName;
        private UIButton uiButton_add;
    }
}