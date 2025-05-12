
namespace EAP.Client.Forms
{
    partial class SputterCathodeSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SputterCathodeSettingForm));
            this.uiTextBox_seq = new Sunny.UI.UITextBox();
            this.uiButton_confirm = new Sunny.UI.UIButton();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.uiTextBox_cathodeid = new Sunny.UI.UITextBox();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.SuspendLayout();
            // 
            // uiTextBox_seq
            // 
            this.uiTextBox_seq.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_seq.Font = new System.Drawing.Font("宋体", 15F);
            this.uiTextBox_seq.Location = new System.Drawing.Point(159, 52);
            this.uiTextBox_seq.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox_seq.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_seq.Name = "uiTextBox_seq";
            this.uiTextBox_seq.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox_seq.ShowText = false;
            this.uiTextBox_seq.Size = new System.Drawing.Size(281, 29);
            this.uiTextBox_seq.TabIndex = 1;
            this.uiTextBox_seq.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_seq.Watermark = "";
            // 
            // uiButton_confirm
            // 
            this.uiButton_confirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton_confirm.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton_confirm.Location = new System.Drawing.Point(356, 159);
            this.uiButton_confirm.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton_confirm.Name = "uiButton_confirm";
            this.uiButton_confirm.Size = new System.Drawing.Size(84, 32);
            this.uiButton_confirm.TabIndex = 2;
            this.uiButton_confirm.Text = "确认";
            this.uiButton_confirm.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton_confirm.Click += new System.EventHandler(this.uiButton_confirm_Click);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("宋体", 15F);
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel1.Location = new System.Drawing.Point(22, 58);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(61, 23);
            this.uiLabel1.TabIndex = 3;
            this.uiLabel1.Text = "Seq";
            // 
            // uiTextBox_cathodeid
            // 
            this.uiTextBox_cathodeid.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox_cathodeid.Font = new System.Drawing.Font("宋体", 15F);
            this.uiTextBox_cathodeid.Location = new System.Drawing.Point(159, 103);
            this.uiTextBox_cathodeid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox_cathodeid.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox_cathodeid.Name = "uiTextBox_cathodeid";
            this.uiTextBox_cathodeid.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox_cathodeid.ShowText = false;
            this.uiTextBox_cathodeid.Size = new System.Drawing.Size(281, 29);
            this.uiTextBox_cathodeid.TabIndex = 1;
            this.uiTextBox_cathodeid.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox_cathodeid.Watermark = "";
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("宋体", 15F);
            this.uiLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel2.Location = new System.Drawing.Point(22, 109);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(115, 23);
            this.uiLabel2.TabIndex = 3;
            this.uiLabel2.Text = "Cathode ID";
            // 
            // SputterCathodeSettingForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(473, 220);
            this.Controls.Add(this.uiLabel2);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.uiButton_confirm);
            this.Controls.Add(this.uiTextBox_cathodeid);
            this.Controls.Add(this.uiTextBox_seq);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SputterCathodeSettingForm";
            this.Text = "Cathode";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 629, 333);
            this.Shown += new System.EventHandler(this.SputterCathodeSettingForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion
        private Sunny.UI.UITextBox uiTextBox_seq;
        private Sunny.UI.UIButton uiButton_confirm;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UITextBox uiTextBox_cathodeid;
        private Sunny.UI.UILabel uiLabel2;
    }
}