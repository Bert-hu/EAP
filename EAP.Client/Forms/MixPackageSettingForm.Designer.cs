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
            SuspendLayout();
            // 
            // textBox_icos
            // 
            textBox_icos.Font = new Font("Microsoft YaHei UI", 20F);
            textBox_icos.Location = new Point(131, 51);
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
            button_confirm.Location = new Point(374, 216);
            button_confirm.MinimumSize = new Size(1, 1);
            button_confirm.Name = "button_confirm";
            button_confirm.Size = new Size(118, 30);
            button_confirm.TabIndex = 1;
            button_confirm.Text = "OK";
            button_confirm.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_confirm.Click += button_confirm_Click;
            // 
            // uiTextBox_m
            // 
            uiTextBox_m.Font = new Font("Microsoft YaHei UI", 20F);
            uiTextBox_m.Location = new Point(131, 102);
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
            uiTextBox_oh.Location = new Point(131, 153);
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
            uiLabel3.Location = new Point(17, 153);
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
            uiLabel2.Location = new Point(17, 102);
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
            uiLabel1.Location = new Point(17, 51);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(107, 27);
            uiLabel1.TabIndex = 4;
            uiLabel1.Text = "MIX-ICOS";
            // 
            // MixPackageSettingForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(516, 269);
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
    }
}