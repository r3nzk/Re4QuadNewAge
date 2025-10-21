namespace Re4QuadExtremeEditor.Editor.Controls
{
    partial class ViewportCameraDropdown
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel item1;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label fovLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.camMoveTypeCombox = new ReaLTaiizor.Controls.SkyComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.item1 = new System.Windows.Forms.Panel();
            this.camSpeedValueLabel = new System.Windows.Forms.Label();
            this.speedLabel = new System.Windows.Forms.Label();
            this.camSpeedSlider = new ReaLTaiizor.Controls.MetroTrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fovValueLabel = new System.Windows.Forms.Label();
            this.camFovSlider = new ReaLTaiizor.Controls.MetroTrackBar();
            this.fovLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.item1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.panel3);
            this.flowLayoutPanel1.Controls.Add(this.item1);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(261, 86);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.camMoveTypeCombox);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(1, 1);
            this.panel3.Margin = new System.Windows.Forms.Padding(1);
            this.panel3.MinimumSize = new System.Drawing.Size(255, 26);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(260, 26);
            this.panel3.TabIndex = 3;
            // 
            // camMoveTypeCombox
            // 
            this.camMoveTypeCombox.BackColor = System.Drawing.SystemColors.Control;
            this.camMoveTypeCombox.BGColorA = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.camMoveTypeCombox.BGColorB = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.camMoveTypeCombox.BorderColorA = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.camMoveTypeCombox.BorderColorB = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.camMoveTypeCombox.BorderColorC = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(189)))), ((int)(((byte)(189)))));
            this.camMoveTypeCombox.BorderColorD = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(168)))), ((int)(((byte)(168)))), ((int)(((byte)(168)))));
            this.camMoveTypeCombox.Cursor = System.Windows.Forms.Cursors.Default;
            this.camMoveTypeCombox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.camMoveTypeCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.camMoveTypeCombox.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.camMoveTypeCombox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(94)))), ((int)(((byte)(137)))));
            this.camMoveTypeCombox.FormattingEnabled = true;
            this.camMoveTypeCombox.ItemHeight = 16;
            this.camMoveTypeCombox.ItemHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(176)))), ((int)(((byte)(214)))));
            this.camMoveTypeCombox.LineColorA = System.Drawing.Color.White;
            this.camMoveTypeCombox.LineColorB = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(189)))), ((int)(((byte)(189)))));
            this.camMoveTypeCombox.LineColorC = System.Drawing.Color.White;
            this.camMoveTypeCombox.ListBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.camMoveTypeCombox.ListBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.camMoveTypeCombox.ListDashType = System.Drawing.Drawing2D.DashStyle.Dot;
            this.camMoveTypeCombox.ListForeColor = System.Drawing.Color.Black;
            this.camMoveTypeCombox.ListSelectedBackColorA = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.camMoveTypeCombox.ListSelectedBackColorB = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.camMoveTypeCombox.Location = new System.Drawing.Point(86, 2);
            this.camMoveTypeCombox.Name = "camMoveTypeCombox";
            this.camMoveTypeCombox.Size = new System.Drawing.Size(171, 22);
            this.camMoveTypeCombox.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.camMoveTypeCombox.StartIndex = 0;
            this.camMoveTypeCombox.TabIndex = 0;
            this.camMoveTypeCombox.TriangleColorA = System.Drawing.Color.Black;
            this.camMoveTypeCombox.TriangleColorB = System.Drawing.Color.Black;
            this.camMoveTypeCombox.SelectedIndexChanged += new System.EventHandler(this.camMoveTypeCombox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(6, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type:";
            // 
            // item1
            // 
            this.item1.Controls.Add(this.camSpeedValueLabel);
            this.item1.Controls.Add(this.speedLabel);
            this.item1.Controls.Add(this.camSpeedSlider);
            this.item1.Location = new System.Drawing.Point(1, 29);
            this.item1.Margin = new System.Windows.Forms.Padding(1);
            this.item1.MinimumSize = new System.Drawing.Size(255, 26);
            this.item1.Name = "item1";
            this.item1.Size = new System.Drawing.Size(260, 26);
            this.item1.TabIndex = 0;
            // 
            // camSpeedValueLabel
            // 
            this.camSpeedValueLabel.AutoSize = true;
            this.camSpeedValueLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.camSpeedValueLabel.Location = new System.Drawing.Point(229, 6);
            this.camSpeedValueLabel.Name = "camSpeedValueLabel";
            this.camSpeedValueLabel.Size = new System.Drawing.Size(25, 15);
            this.camSpeedValueLabel.TabIndex = 5;
            this.camSpeedValueLabel.Text = "100";
            // 
            // speedLabel
            // 
            this.speedLabel.AutoSize = true;
            this.speedLabel.BackColor = System.Drawing.Color.Transparent;
            this.speedLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.speedLabel.Location = new System.Drawing.Point(5, 5);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Size = new System.Drawing.Size(70, 15);
            this.speedLabel.TabIndex = 0;
            this.speedLabel.Text = "Cam Speed:";
            // 
            // camSpeedSlider
            // 
            this.camSpeedSlider.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.camSpeedSlider.Cursor = System.Windows.Forms.Cursors.Hand;
            this.camSpeedSlider.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.camSpeedSlider.DisabledBorderColor = System.Drawing.Color.Empty;
            this.camSpeedSlider.DisabledHandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.camSpeedSlider.DisabledValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.camSpeedSlider.HandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.camSpeedSlider.IsDerivedStyle = true;
            this.camSpeedSlider.Location = new System.Drawing.Point(86, 5);
            this.camSpeedSlider.Maximum = 200;
            this.camSpeedSlider.Minimum = 10;
            this.camSpeedSlider.Name = "camSpeedSlider";
            this.camSpeedSlider.Size = new System.Drawing.Size(141, 16);
            this.camSpeedSlider.Style = ReaLTaiizor.Enum.Metro.Style.Light;
            this.camSpeedSlider.StyleManager = null;
            this.camSpeedSlider.TabIndex = 2;
            this.camSpeedSlider.Text = "metroTrackBar1";
            this.camSpeedSlider.ThemeAuthor = "Taiizor";
            this.camSpeedSlider.ThemeName = "MetroLight";
            this.camSpeedSlider.Value = 100;
            this.camSpeedSlider.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            this.camSpeedSlider.Scroll += new ReaLTaiizor.Controls.MetroTrackBar.ScrollEventHandler(this.camSpeedSlider_Scroll);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fovValueLabel);
            this.panel1.Controls.Add(this.camFovSlider);
            this.panel1.Controls.Add(this.fovLabel);
            this.panel1.Location = new System.Drawing.Point(1, 57);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.MinimumSize = new System.Drawing.Size(255, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 26);
            this.panel1.TabIndex = 1;
            // 
            // fovValueLabel
            // 
            this.fovValueLabel.AutoSize = true;
            this.fovValueLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fovValueLabel.Location = new System.Drawing.Point(229, 6);
            this.fovValueLabel.Name = "fovValueLabel";
            this.fovValueLabel.Size = new System.Drawing.Size(25, 15);
            this.fovValueLabel.TabIndex = 4;
            this.fovValueLabel.Text = "100";
            // 
            // camFovSlider
            // 
            this.camFovSlider.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.camFovSlider.Cursor = System.Windows.Forms.Cursors.Hand;
            this.camFovSlider.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.camFovSlider.DisabledBorderColor = System.Drawing.Color.Empty;
            this.camFovSlider.DisabledHandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.camFovSlider.DisabledValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.camFovSlider.HandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.camFovSlider.IsDerivedStyle = true;
            this.camFovSlider.Location = new System.Drawing.Point(86, 5);
            this.camFovSlider.Maximum = 150;
            this.camFovSlider.Minimum = 10;
            this.camFovSlider.Name = "camFovSlider";
            this.camFovSlider.Size = new System.Drawing.Size(141, 16);
            this.camFovSlider.Style = ReaLTaiizor.Enum.Metro.Style.Light;
            this.camFovSlider.StyleManager = null;
            this.camFovSlider.TabIndex = 3;
            this.camFovSlider.Text = "metroTrackBar2";
            this.camFovSlider.ThemeAuthor = "Taiizor";
            this.camFovSlider.ThemeName = "MetroLight";
            this.camFovSlider.Value = 100;
            this.camFovSlider.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            this.camFovSlider.Scroll += new ReaLTaiizor.Controls.MetroTrackBar.ScrollEventHandler(this.camFovSlider_Scroll);
            // 
            // fovLabel
            // 
            this.fovLabel.AutoSize = true;
            this.fovLabel.BackColor = System.Drawing.Color.Transparent;
            this.fovLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.fovLabel.Location = new System.Drawing.Point(5, 5);
            this.fovLabel.Name = "fovLabel";
            this.fovLabel.Size = new System.Drawing.Size(77, 15);
            this.fovLabel.TabIndex = 0;
            this.fovLabel.Text = "Field of View:";
            // 
            // ViewportCameraDropdown
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ViewportCameraDropdown";
            this.Size = new System.Drawing.Size(261, 86);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.item1.ResumeLayout(false);
            this.item1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private ReaLTaiizor.Controls.MetroTrackBar camSpeedSlider;
        private ReaLTaiizor.Controls.MetroTrackBar camFovSlider;
        private System.Windows.Forms.Panel panel3;
        private ReaLTaiizor.Controls.SkyComboBox camMoveTypeCombox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label camSpeedValueLabel;
        private System.Windows.Forms.Label fovValueLabel;
    }
}
