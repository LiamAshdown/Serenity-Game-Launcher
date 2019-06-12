namespace SteerStone.WebBrowser
{
    partial class WebBrowser
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
            this.WebBrowser_Browser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // WebBrowser_Browser
            // 
            this.WebBrowser_Browser.AllowWebBrowserDrop = false;
            this.WebBrowser_Browser.Location = new System.Drawing.Point(1, 0);
            this.WebBrowser_Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser_Browser.Name = "WebBrowser_Browser";
            this.WebBrowser_Browser.Size = new System.Drawing.Size(1326, 542);
            this.WebBrowser_Browser.TabIndex = 0;
            this.WebBrowser_Browser.Url = new System.Uri("http://www.serenity-wow.com", System.UriKind.Absolute);
            // 
            // WebBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(1329, 542);
            this.Controls.Add(this.WebBrowser_Browser);
            this.DoubleBuffered = true;
            this.Name = "WebBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serenity-WoW";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.WebBrowser_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebBrowser_FormClosing);
            this.Resize += new System.EventHandler(this.WebBrowser_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.WebBrowser WebBrowser_Browser;
    }
}