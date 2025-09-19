
namespace Re4QuadExtremeEditor.Editor.Forms
{
    partial class SplashScreenForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreenForm));
            this.splashLoadingLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splashLoadingBar = new System.Windows.Forms.ProgressBar();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splashLoadingLabel
            // 
            this.splashLoadingLabel.AutoEllipsis = true;
            this.splashLoadingLabel.AutoSize = true;
            this.splashLoadingLabel.BackColor = System.Drawing.Color.Transparent;
            this.splashLoadingLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.splashLoadingLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splashLoadingLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.splashLoadingLabel.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.splashLoadingLabel.Location = new System.Drawing.Point(178, 0);
            this.splashLoadingLabel.Name = "splashLoadingLabel";
            this.splashLoadingLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splashLoadingLabel.Size = new System.Drawing.Size(141, 21);
            this.splashLoadingLabel.TabIndex = 1;
            this.splashLoadingLabel.Text = "loadingStatusLabel";
            this.splashLoadingLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.splashLoadingLabel);
            this.panel1.Location = new System.Drawing.Point(349, 449);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(319, 24);
            this.panel1.TabIndex = 2;
            // 
            // splashLoadingBar
            // 
            this.splashLoadingBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splashLoadingBar.ForeColor = System.Drawing.Color.Lime;
            this.splashLoadingBar.Location = new System.Drawing.Point(0, 476);
            this.splashLoadingBar.Maximum = 110;
            this.splashLoadingBar.Name = "splashLoadingBar";
            this.splashLoadingBar.Size = new System.Drawing.Size(671, 10);
            this.splashLoadingBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.splashLoadingBar.TabIndex = 0;
            // 
            // SplashScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackgroundImage = global::Re4QuadExtremeEditor.Properties.Resources.quad_splash;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(671, 486);
            this.Controls.Add(this.splashLoadingBar);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SplashScreenForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RE4 QuadX";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SplashScreenForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SplashScreenForm_FormClosed);
            this.Load += new System.EventHandler(this.SplashScreenForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label splashLoadingLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar splashLoadingBar;
    }
}