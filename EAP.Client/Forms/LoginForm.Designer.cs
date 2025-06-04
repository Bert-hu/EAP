namespace EAP.Client.Forms
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Text = "SMD";
            // 
            // lblSubText
            // 
            lblSubText.Location = new Point(376, 421);
            lblSubText.Text = "USI SMD";
            // 
            // LoginForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(750, 450);
            LoginImage = UILoginImage.Login4;
            Name = "LoginForm";
            SubText = "USI SMD";
            Text = "LoginForm";
            Title = "SMD";


            ResumeLayout(false);
        }

        #endregion
    }
}