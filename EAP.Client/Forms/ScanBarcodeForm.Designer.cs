using Sunny.UI;

namespace EAP.Client.Forms
{
    partial class ScanBarcodeForm
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
            textBox1 = new UITextBox();
            button_confirm = new UIButton();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Microsoft YaHei UI", 20F);
            textBox1.Location = new Point(14, 51);
            textBox1.Margin = new Padding(4, 5, 4, 5);
            textBox1.MinimumSize = new Size(1, 16);
            textBox1.Name = "textBox1";
            textBox1.Padding = new Padding(5);
            textBox1.ShowText = false;
            textBox1.Size = new Size(478, 41);
            textBox1.TabIndex = 0;
            textBox1.TextAlignment = ContentAlignment.MiddleLeft;
            textBox1.Watermark = "";
            // 
            // button_confirm
            // 
            button_confirm.Font = new Font("宋体", 12F);
            button_confirm.Location = new Point(374, 109);
            button_confirm.MinimumSize = new Size(1, 1);
            button_confirm.Name = "button_confirm";
            button_confirm.Size = new Size(118, 30);
            button_confirm.TabIndex = 1;
            button_confirm.Text = "OK";
            button_confirm.TipsFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            button_confirm.Click += button_confirm_Click;
            // 
            // ScanBarcodeForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(509, 152);
            Controls.Add(button_confirm);
            Controls.Add(textBox1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ScanBarcodeForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "扫码";
            TopMost = true;
            ZoomScaleRect = new Rectangle(15, 15, 448, 109);
            Shown += ScanBarcodeForm_Shown;
            ResumeLayout(false);
        }

        #endregion

        private UITextBox textBox1;
        private UIButton button_confirm;
    }
}