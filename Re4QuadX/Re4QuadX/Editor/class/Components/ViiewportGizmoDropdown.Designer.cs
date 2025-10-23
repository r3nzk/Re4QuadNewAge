namespace Re4QuadX.Editor.Controls
{
    partial class ViewportGizmoDropdown
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.gridSizeTextbox = new PowerLib.Winform.Controls.XTextBox();
            this.gridSizeLabel = new System.Windows.Forms.Label();
            this.drawGridCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.metroTrackBar1 = new ReaLTaiizor.Controls.MetroTrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.drawBoundingBoxCheckbox = new System.Windows.Forms.CheckBox();
            this.fovValueLabel = new System.Windows.Forms.Label();
            this.camFovSlider = new ReaLTaiizor.Controls.MetroTrackBar();
            this.fovLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.drawGizmoCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.metroTrackBar2 = new ReaLTaiizor.Controls.MetroTrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.camMoveTypeCombox = new ReaLTaiizor.Controls.SkyComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.metroTrackBar3 = new ReaLTaiizor.Controls.MetroTrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.panel2);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.panel3);
            this.flowLayoutPanel1.Controls.Add(this.panel4);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(241, 213);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gridSizeTextbox);
            this.panel2.Controls.Add(this.gridSizeLabel);
            this.panel2.Controls.Add(this.drawGridCheckbox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.metroTrackBar1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(1, 1);
            this.panel2.Margin = new System.Windows.Forms.Padding(1);
            this.panel2.MinimumSize = new System.Drawing.Size(238, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(238, 52);
            this.panel2.TabIndex = 2;
            // 
            // gridSizeTextbox
            // 
            this.gridSizeTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gridSizeTextbox.BackColor = System.Drawing.Color.White;
            this.gridSizeTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridSizeTextbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.gridSizeTextbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridSizeTextbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.gridSizeTextbox.Location = new System.Drawing.Point(66, 25);
            this.gridSizeTextbox.Name = "gridSizeTextbox";
            this.gridSizeTextbox.PlaceHolder = null;
            this.gridSizeTextbox.Size = new System.Drawing.Size(169, 22);
            this.gridSizeTextbox.TabIndex = 6;
            this.gridSizeTextbox.Text = "100";
            this.gridSizeTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridSizeTextbox_KeyDown);
            this.gridSizeTextbox.Validated += new System.EventHandler(this.gridSizeTextbox_Validated);
            // 
            // gridSizeLabel
            // 
            this.gridSizeLabel.AutoSize = true;
            this.gridSizeLabel.Location = new System.Drawing.Point(8, 27);
            this.gridSizeLabel.Name = "gridSizeLabel";
            this.gridSizeLabel.Size = new System.Drawing.Size(52, 13);
            this.gridSizeLabel.TabIndex = 5;
            this.gridSizeLabel.Text = "Grid Size:";
            // 
            // drawGridCheckbox
            // 
            this.drawGridCheckbox.Location = new System.Drawing.Point(8, 3);
            this.drawGridCheckbox.Name = "drawGridCheckbox";
            this.drawGridCheckbox.Size = new System.Drawing.Size(227, 17);
            this.drawGridCheckbox.TabIndex = 4;
            this.drawGridCheckbox.Text = "Show Grid";
            this.drawGridCheckbox.UseVisualStyleBackColor = true;
            this.drawGridCheckbox.CheckedChanged += new System.EventHandler(this.drawGridCheckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            // 
            // metroTrackBar1
            // 
            this.metroTrackBar1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.metroTrackBar1.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.metroTrackBar1.DisabledBorderColor = System.Drawing.Color.Empty;
            this.metroTrackBar1.DisabledHandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.metroTrackBar1.DisabledValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.metroTrackBar1.HandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.metroTrackBar1.IsDerivedStyle = true;
            this.metroTrackBar1.Location = new System.Drawing.Point(0, 0);
            this.metroTrackBar1.Maximum = 100;
            this.metroTrackBar1.Minimum = 0;
            this.metroTrackBar1.Name = "metroTrackBar1";
            this.metroTrackBar1.Size = new System.Drawing.Size(0, 0);
            this.metroTrackBar1.Style = ReaLTaiizor.Enum.Metro.Style.Light;
            this.metroTrackBar1.StyleManager = null;
            this.metroTrackBar1.TabIndex = 1;
            this.metroTrackBar1.ThemeAuthor = "Taiizor";
            this.metroTrackBar1.ThemeName = "MetroLight";
            this.metroTrackBar1.Value = 0;
            this.metroTrackBar1.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.drawBoundingBoxCheckbox);
            this.panel1.Controls.Add(this.fovValueLabel);
            this.panel1.Controls.Add(this.camFovSlider);
            this.panel1.Controls.Add(this.fovLabel);
            this.panel1.Location = new System.Drawing.Point(1, 55);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.MinimumSize = new System.Drawing.Size(238, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(238, 22);
            this.panel1.TabIndex = 1;
            // 
            // drawBoundingBoxCheckbox
            // 
            this.drawBoundingBoxCheckbox.Location = new System.Drawing.Point(8, 3);
            this.drawBoundingBoxCheckbox.Name = "drawBoundingBoxCheckbox";
            this.drawBoundingBoxCheckbox.Size = new System.Drawing.Size(227, 17);
            this.drawBoundingBoxCheckbox.TabIndex = 4;
            this.drawBoundingBoxCheckbox.Text = "Show Bounding Box";
            this.drawBoundingBoxCheckbox.UseVisualStyleBackColor = true;
            // 
            // fovValueLabel
            // 
            this.fovValueLabel.Location = new System.Drawing.Point(0, 0);
            this.fovValueLabel.Name = "fovValueLabel";
            this.fovValueLabel.Size = new System.Drawing.Size(100, 23);
            this.fovValueLabel.TabIndex = 0;
            // 
            // camFovSlider
            // 
            this.camFovSlider.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.camFovSlider.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.camFovSlider.DisabledBorderColor = System.Drawing.Color.Empty;
            this.camFovSlider.DisabledHandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.camFovSlider.DisabledValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.camFovSlider.HandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.camFovSlider.IsDerivedStyle = true;
            this.camFovSlider.Location = new System.Drawing.Point(0, 0);
            this.camFovSlider.Maximum = 100;
            this.camFovSlider.Minimum = 0;
            this.camFovSlider.Name = "camFovSlider";
            this.camFovSlider.Size = new System.Drawing.Size(0, 0);
            this.camFovSlider.Style = ReaLTaiizor.Enum.Metro.Style.Light;
            this.camFovSlider.StyleManager = null;
            this.camFovSlider.TabIndex = 1;
            this.camFovSlider.ThemeAuthor = "Taiizor";
            this.camFovSlider.ThemeName = "MetroLight";
            this.camFovSlider.Value = 0;
            this.camFovSlider.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            // 
            // fovLabel
            // 
            this.fovLabel.Location = new System.Drawing.Point(0, 0);
            this.fovLabel.Name = "fovLabel";
            this.fovLabel.Size = new System.Drawing.Size(100, 23);
            this.fovLabel.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.drawGizmoCheckbox);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.metroTrackBar2);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Location = new System.Drawing.Point(1, 79);
            this.panel3.Margin = new System.Windows.Forms.Padding(1);
            this.panel3.MinimumSize = new System.Drawing.Size(238, 22);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(238, 22);
            this.panel3.TabIndex = 3;
            // 
            // drawGizmoCheckbox
            // 
            this.drawGizmoCheckbox.Location = new System.Drawing.Point(8, 3);
            this.drawGizmoCheckbox.Name = "drawGizmoCheckbox";
            this.drawGizmoCheckbox.Size = new System.Drawing.Size(227, 17);
            this.drawGizmoCheckbox.TabIndex = 4;
            this.drawGizmoCheckbox.Text = "Show Gizmo Handles";
            this.drawGizmoCheckbox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 23);
            this.label5.TabIndex = 0;
            // 
            // metroTrackBar2
            // 
            this.metroTrackBar2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.metroTrackBar2.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.metroTrackBar2.DisabledBorderColor = System.Drawing.Color.Empty;
            this.metroTrackBar2.DisabledHandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.metroTrackBar2.DisabledValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.metroTrackBar2.HandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.metroTrackBar2.IsDerivedStyle = true;
            this.metroTrackBar2.Location = new System.Drawing.Point(0, 0);
            this.metroTrackBar2.Maximum = 100;
            this.metroTrackBar2.Minimum = 0;
            this.metroTrackBar2.Name = "metroTrackBar2";
            this.metroTrackBar2.Size = new System.Drawing.Size(0, 0);
            this.metroTrackBar2.Style = ReaLTaiizor.Enum.Metro.Style.Light;
            this.metroTrackBar2.StyleManager = null;
            this.metroTrackBar2.TabIndex = 1;
            this.metroTrackBar2.ThemeAuthor = "Taiizor";
            this.metroTrackBar2.ThemeName = "MetroLight";
            this.metroTrackBar2.Value = 0;
            this.metroTrackBar2.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 23);
            this.label6.TabIndex = 2;
            // 
            // camMoveTypeCombox
            // 
            this.camMoveTypeCombox.BackColor = System.Drawing.Color.Transparent;
            this.camMoveTypeCombox.BGColorA = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.camMoveTypeCombox.BGColorB = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.camMoveTypeCombox.BorderColorA = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.camMoveTypeCombox.BorderColorB = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.camMoveTypeCombox.BorderColorC = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(189)))), ((int)(((byte)(189)))));
            this.camMoveTypeCombox.BorderColorD = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(168)))), ((int)(((byte)(168)))), ((int)(((byte)(168)))));
            this.camMoveTypeCombox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.camMoveTypeCombox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.camMoveTypeCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.camMoveTypeCombox.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold);
            this.camMoveTypeCombox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(94)))), ((int)(((byte)(137)))));
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
            this.camMoveTypeCombox.Location = new System.Drawing.Point(0, 0);
            this.camMoveTypeCombox.Name = "camMoveTypeCombox";
            this.camMoveTypeCombox.Size = new System.Drawing.Size(75, 22);
            this.camMoveTypeCombox.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.camMoveTypeCombox.StartIndex = 0;
            this.camMoveTypeCombox.TabIndex = 0;
            this.camMoveTypeCombox.TriangleColorA = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(176)))), ((int)(((byte)(214)))));
            this.camMoveTypeCombox.TriangleColorB = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(94)))), ((int)(((byte)(137)))));
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.checkBox1);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.metroTrackBar3);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Location = new System.Drawing.Point(1, 103);
            this.panel4.Margin = new System.Windows.Forms.Padding(1);
            this.panel4.MinimumSize = new System.Drawing.Size(238, 22);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(238, 22);
            this.panel4.TabIndex = 4;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(8, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(227, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Show Bounding Box";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 0;
            // 
            // metroTrackBar3
            // 
            this.metroTrackBar3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.metroTrackBar3.DisabledBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.metroTrackBar3.DisabledBorderColor = System.Drawing.Color.Empty;
            this.metroTrackBar3.DisabledHandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
            this.metroTrackBar3.DisabledValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(205)))), ((int)(((byte)(205)))));
            this.metroTrackBar3.HandlerColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.metroTrackBar3.IsDerivedStyle = true;
            this.metroTrackBar3.Location = new System.Drawing.Point(0, 0);
            this.metroTrackBar3.Maximum = 100;
            this.metroTrackBar3.Minimum = 0;
            this.metroTrackBar3.Name = "metroTrackBar3";
            this.metroTrackBar3.Size = new System.Drawing.Size(0, 0);
            this.metroTrackBar3.Style = ReaLTaiizor.Enum.Metro.Style.Light;
            this.metroTrackBar3.StyleManager = null;
            this.metroTrackBar3.TabIndex = 1;
            this.metroTrackBar3.ThemeAuthor = "Taiizor";
            this.metroTrackBar3.ThemeName = "MetroLight";
            this.metroTrackBar3.Value = 0;
            this.metroTrackBar3.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(177)))), ((int)(((byte)(225)))));
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 23);
            this.label7.TabIndex = 2;
            // 
            // ViewportGizmoDropdown
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ViewportGizmoDropdown";
            this.Size = new System.Drawing.Size(241, 213);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private ReaLTaiizor.Controls.MetroTrackBar camFovSlider;
        private ReaLTaiizor.Controls.SkyComboBox camMoveTypeCombox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label fovValueLabel;
        private System.Windows.Forms.CheckBox drawBoundingBoxCheckbox;
        private System.Windows.Forms.Panel panel2;
        private PowerLib.Winform.Controls.XTextBox gridSizeTextbox;
        private System.Windows.Forms.Label gridSizeLabel;
        private System.Windows.Forms.CheckBox drawGridCheckbox;
        private System.Windows.Forms.Label label1;
        private ReaLTaiizor.Controls.MetroTrackBar metroTrackBar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox drawGizmoCheckbox;
        private System.Windows.Forms.Label label5;
        private ReaLTaiizor.Controls.MetroTrackBar metroTrackBar2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
        private ReaLTaiizor.Controls.MetroTrackBar metroTrackBar3;
        private System.Windows.Forms.Label label7;
    }
}
