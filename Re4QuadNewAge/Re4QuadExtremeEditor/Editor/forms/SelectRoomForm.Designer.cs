
namespace Re4QuadExtremeEditor.Editor.Forms
{
    partial class SelectRoomForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectRoomForm));
            this.labelText1 = new System.Windows.Forms.Label();
            this.buttonLoadComplete = new PowerLib.Winform.Controls.XButton();
            this.dummyItem = new PowerLib.Winform.Controls.DropDownButtonItem();
            this.buttonLoad = new PowerLib.Winform.Controls.XButton();
            this.buttonCancel = new PowerLib.Winform.Controls.XButton();
            this.advancedLoadContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.includeITA = new System.Windows.Forms.ToolStripMenuItem();
            this.includeAEV = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.includeLIT = new System.Windows.Forms.ToolStripMenuItem();
            this.includeETS = new System.Windows.Forms.ToolStripMenuItem();
            this.includeESE = new System.Windows.Forms.ToolStripMenuItem();
            this.includeEMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.includeDSE = new System.Windows.Forms.ToolStripMenuItem();
            this.includeFSE = new System.Windows.Forms.ToolStripMenuItem();
            this.includeSAR = new System.Windows.Forms.ToolStripMenuItem();
            this.includeEAR = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.room_searchBar = new System.Windows.Forms.Panel();
            this.room_searchButton = new System.Windows.Forms.Button();
            this.room_searchField = new System.Windows.Forms.TextBox();
            this.room_listBox = new System.Windows.Forms.ListBox();
            this.comboBoxMainList = new ReaLTaiizor.Controls.SkyComboBox();
            this.advancedLoadContextMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.room_searchBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelText1
            // 
            this.labelText1.AutoSize = true;
            this.labelText1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelText1.Location = new System.Drawing.Point(3, 5);
            this.labelText1.Name = "labelText1";
            this.labelText1.Size = new System.Drawing.Size(73, 19);
            this.labelText1.TabIndex = 4;
            this.labelText1.Text = "Room List:";
            // 
            // buttonLoadComplete
            // 
            this.buttonLoadComplete.AllowDrop = true;
            this.buttonLoadComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoadComplete.CheckedEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(128)))), ((int)(((byte)(252)))));
            this.buttonLoadComplete.CheckedForeColor = System.Drawing.Color.White;
            this.buttonLoadComplete.CheckedStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(169)))), ((int)(((byte)(255)))));
            this.buttonLoadComplete.DefaultButtonBorderWidth = 2;
            this.buttonLoadComplete.DropDownItems.Add(this.dummyItem);
            this.buttonLoadComplete.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonLoadComplete.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoadComplete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.buttonLoadComplete.HoldingEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(128)))), ((int)(((byte)(252)))));
            this.buttonLoadComplete.HoldingForeColor = System.Drawing.Color.White;
            this.buttonLoadComplete.HoldingImage = null;
            this.buttonLoadComplete.HoldingStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(169)))), ((int)(((byte)(255)))));
            this.buttonLoadComplete.Image = null;
            this.buttonLoadComplete.Location = new System.Drawing.Point(404, 271);
            this.buttonLoadComplete.MaximumSize = new System.Drawing.Size(200, 500);
            this.buttonLoadComplete.MinimumSize = new System.Drawing.Size(140, 30);
            this.buttonLoadComplete.Name = "buttonLoadComplete";
            this.buttonLoadComplete.Size = new System.Drawing.Size(162, 30);
            this.buttonLoadComplete.StartColor = System.Drawing.Color.White;
            this.buttonLoadComplete.TabIndex = 8;
            this.buttonLoadComplete.Text = "Load With Objects";
            this.buttonLoadComplete.Click += new System.EventHandler(this.buttonLoadComplete_Click);
            this.buttonLoadComplete.DropDown += new System.EventHandler(this.buttonLoadComplete_Dropdown);
            // 
            // dummyItem
            // 
            this.dummyItem.Caption = "";
            this.dummyItem.Location = new System.Drawing.Point(0, 0);
            this.dummyItem.Name = "dummyItem";
            this.dummyItem.ParentBtn = this.buttonLoadComplete;
            this.dummyItem.Size = new System.Drawing.Size(162, 30);
            this.dummyItem.TabIndex = 0;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoad.CheckedEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(128)))), ((int)(((byte)(252)))));
            this.buttonLoad.CheckedForeColor = System.Drawing.Color.White;
            this.buttonLoad.CheckedStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(169)))), ((int)(((byte)(255)))));
            this.buttonLoad.DefaultButtonBorderWidth = 2;
            this.buttonLoad.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonLoad.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.buttonLoad.HoldingEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(128)))), ((int)(((byte)(252)))));
            this.buttonLoad.HoldingForeColor = System.Drawing.Color.White;
            this.buttonLoad.HoldingImage = null;
            this.buttonLoad.HoldingStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(169)))), ((int)(((byte)(255)))));
            this.buttonLoad.Image = null;
            this.buttonLoad.Location = new System.Drawing.Point(572, 271);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(85, 30);
            this.buttonLoad.StartColor = System.Drawing.Color.White;
            this.buttonLoad.TabIndex = 7;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.CheckedEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(128)))), ((int)(((byte)(252)))));
            this.buttonCancel.CheckedForeColor = System.Drawing.Color.White;
            this.buttonCancel.CheckedStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(169)))), ((int)(((byte)(255)))));
            this.buttonCancel.DefaultButtonBorderWidth = 2;
            this.buttonCancel.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.buttonCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.buttonCancel.HoldingEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(128)))), ((int)(((byte)(252)))));
            this.buttonCancel.HoldingForeColor = System.Drawing.Color.White;
            this.buttonCancel.HoldingImage = null;
            this.buttonCancel.HoldingStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(169)))), ((int)(((byte)(255)))));
            this.buttonCancel.Image = null;
            this.buttonCancel.Location = new System.Drawing.Point(663, 271);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(85, 30);
            this.buttonCancel.StartColor = System.Drawing.Color.White;
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // advancedLoadContextMenu
            // 
            this.advancedLoadContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeITA,
            this.includeAEV,
            this.toolStripSeparator1,
            this.includeLIT,
            this.includeETS,
            this.includeESE,
            this.includeEMI,
            this.toolStripSeparator2,
            this.includeDSE,
            this.includeFSE,
            this.includeSAR,
            this.includeEAR});
            this.advancedLoadContextMenu.Name = "advancedLoadContextMenu";
            this.advancedLoadContextMenu.Size = new System.Drawing.Size(251, 236);
            this.advancedLoadContextMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.advancedLoadContextMenu_Closing);
            // 
            // includeITA
            // 
            this.includeITA.Checked = true;
            this.includeITA.CheckOnClick = true;
            this.includeITA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeITA.Name = "includeITA";
            this.includeITA.Size = new System.Drawing.Size(250, 22);
            this.includeITA.Text = "Include ITA (Itens)";
            this.includeITA.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // includeAEV
            // 
            this.includeAEV.Checked = true;
            this.includeAEV.CheckOnClick = true;
            this.includeAEV.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeAEV.Name = "includeAEV";
            this.includeAEV.Size = new System.Drawing.Size(250, 22);
            this.includeAEV.Text = "Include AEV (Event)";
            this.includeAEV.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(247, 6);
            // 
            // includeLIT
            // 
            this.includeLIT.Checked = true;
            this.includeLIT.CheckOnClick = true;
            this.includeLIT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeLIT.Name = "includeLIT";
            this.includeLIT.Size = new System.Drawing.Size(250, 22);
            this.includeLIT.Text = "Include LIT (Light)";
            this.includeLIT.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // includeETS
            // 
            this.includeETS.Checked = true;
            this.includeETS.CheckOnClick = true;
            this.includeETS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeETS.Name = "includeETS";
            this.includeETS.Size = new System.Drawing.Size(250, 22);
            this.includeETS.Text = "Include ETS (EtcModel)";
            this.includeETS.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // includeESE
            // 
            this.includeESE.Checked = true;
            this.includeESE.CheckOnClick = true;
            this.includeESE.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeESE.Name = "includeESE";
            this.includeESE.Size = new System.Drawing.Size(250, 22);
            this.includeESE.Text = "Include ESE (Environment Sound)";
            this.includeESE.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // includeEMI
            // 
            this.includeEMI.Checked = true;
            this.includeEMI.CheckOnClick = true;
            this.includeEMI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeEMI.Name = "includeEMI";
            this.includeEMI.Size = new System.Drawing.Size(250, 22);
            this.includeEMI.Text = "Include EMI (Interaction Point)";
            this.includeEMI.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(247, 6);
            // 
            // includeDSE
            // 
            this.includeDSE.Checked = true;
            this.includeDSE.CheckOnClick = true;
            this.includeDSE.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeDSE.Name = "includeDSE";
            this.includeDSE.Size = new System.Drawing.Size(250, 22);
            this.includeDSE.Text = "Include DSE (Door Sound)";
            this.includeDSE.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // includeFSE
            // 
            this.includeFSE.Checked = true;
            this.includeFSE.CheckOnClick = true;
            this.includeFSE.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeFSE.Name = "includeFSE";
            this.includeFSE.Size = new System.Drawing.Size(250, 22);
            this.includeFSE.Text = "Include FSE (Floor Sound)";
            this.includeFSE.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // includeSAR
            // 
            this.includeSAR.Checked = true;
            this.includeSAR.CheckOnClick = true;
            this.includeSAR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeSAR.Name = "includeSAR";
            this.includeSAR.Size = new System.Drawing.Size(250, 22);
            this.includeSAR.Text = "Include SAR (Ctrl Light Group)";
            this.includeSAR.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // includeEAR
            // 
            this.includeEAR.Checked = true;
            this.includeEAR.CheckOnClick = true;
            this.includeEAR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.includeEAR.Name = "includeEAR";
            this.includeEAR.Size = new System.Drawing.Size(250, 22);
            this.includeEAR.Text = "Include EAR (Ctrl Effect Group)";
            this.includeEAR.Click += new System.EventHandler(this.buttonLoadCompleteItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.room_searchBar, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.room_listBox, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 30);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(749, 235);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // room_searchBar
            // 
            this.room_searchBar.BackColor = System.Drawing.Color.White;
            this.room_searchBar.Controls.Add(this.room_searchButton);
            this.room_searchBar.Controls.Add(this.room_searchField);
            this.room_searchBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.room_searchBar.Location = new System.Drawing.Point(3, 2);
            this.room_searchBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 1);
            this.room_searchBar.Name = "room_searchBar";
            this.room_searchBar.Size = new System.Drawing.Size(743, 21);
            this.room_searchBar.TabIndex = 2;
            // 
            // room_searchButton
            // 
            this.room_searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.room_searchButton.BackColor = System.Drawing.Color.Transparent;
            this.room_searchButton.FlatAppearance.BorderSize = 0;
            this.room_searchButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DimGray;
            this.room_searchButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.room_searchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.room_searchButton.Image = ((System.Drawing.Image)(resources.GetObject("room_searchButton.Image")));
            this.room_searchButton.Location = new System.Drawing.Point(720, -2);
            this.room_searchButton.Margin = new System.Windows.Forms.Padding(0);
            this.room_searchButton.Name = "room_searchButton";
            this.room_searchButton.Size = new System.Drawing.Size(22, 25);
            this.room_searchButton.TabIndex = 1;
            this.room_searchButton.UseVisualStyleBackColor = false;
            // 
            // room_searchField
            // 
            this.room_searchField.BackColor = System.Drawing.Color.White;
            this.room_searchField.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.room_searchField.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.room_searchField.HideSelection = false;
            this.room_searchField.Location = new System.Drawing.Point(3, 3);
            this.room_searchField.Margin = new System.Windows.Forms.Padding(1);
            this.room_searchField.MaximumSize = new System.Drawing.Size(1000, 21);
            this.room_searchField.MinimumSize = new System.Drawing.Size(100, 21);
            this.room_searchField.Name = "room_searchField";
            this.room_searchField.Size = new System.Drawing.Size(718, 16);
            this.room_searchField.TabIndex = 0;
            this.room_searchField.TabStop = false;
            this.room_searchField.TextChanged += new System.EventHandler(this.room_searchField_TextChanged);
            // 
            // room_listBox
            // 
            this.room_listBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.room_listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.room_listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.room_listBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.room_listBox.FormattingEnabled = true;
            this.room_listBox.ItemHeight = 17;
            this.room_listBox.Location = new System.Drawing.Point(3, 27);
            this.room_listBox.Name = "room_listBox";
            this.room_listBox.Size = new System.Drawing.Size(743, 205);
            this.room_listBox.TabIndex = 3;
            this.room_listBox.SelectedIndexChanged += new System.EventHandler(this.room_listBox_SelectedIndexChanged);
            // 
            // comboBoxMainList
            // 
            this.comboBoxMainList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxMainList.BackColor = System.Drawing.Color.Transparent;
            this.comboBoxMainList.BGColorA = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.comboBoxMainList.BGColorB = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.comboBoxMainList.BorderColorA = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(252)))), ((int)(((byte)(252)))));
            this.comboBoxMainList.BorderColorB = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
            this.comboBoxMainList.BorderColorC = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.comboBoxMainList.BorderColorD = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.comboBoxMainList.Cursor = System.Windows.Forms.Cursors.Default;
            this.comboBoxMainList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxMainList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMainList.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.comboBoxMainList.ForeColor = System.Drawing.Color.Black;
            this.comboBoxMainList.FormattingEnabled = true;
            this.comboBoxMainList.ItemHeight = 16;
            this.comboBoxMainList.ItemHighlightColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.comboBoxMainList.LineColorA = System.Drawing.Color.White;
            this.comboBoxMainList.LineColorB = System.Drawing.Color.FromArgb(((int)(((byte)(189)))), ((int)(((byte)(189)))), ((int)(((byte)(189)))));
            this.comboBoxMainList.LineColorC = System.Drawing.Color.White;
            this.comboBoxMainList.ListBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.comboBoxMainList.ListBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.comboBoxMainList.ListDashType = System.Drawing.Drawing2D.DashStyle.Dot;
            this.comboBoxMainList.ListForeColor = System.Drawing.Color.Black;
            this.comboBoxMainList.ListSelectedBackColorA = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.comboBoxMainList.ListSelectedBackColorB = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.comboBoxMainList.Location = new System.Drawing.Point(81, 5);
            this.comboBoxMainList.Name = "comboBoxMainList";
            this.comboBoxMainList.Size = new System.Drawing.Size(670, 22);
            this.comboBoxMainList.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.comboBoxMainList.StartIndex = 0;
            this.comboBoxMainList.TabIndex = 10;
            this.comboBoxMainList.TriangleColorA = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboBoxMainList.TriangleColorB = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboBoxMainList.SelectedIndexChanged += new System.EventHandler(this.comboBoxMainList_SelectedIndexChanged);
            // 
            // SelectRoomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 308);
            this.Controls.Add(this.comboBoxMainList);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.buttonLoadComplete);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelText1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(615, 347);
            this.Name = "SelectRoomForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Room";
            this.Load += new System.EventHandler(this.SelectRoomForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectRoomForm_KeyDown);
            this.advancedLoadContextMenu.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.room_searchBar.ResumeLayout(false);
            this.room_searchBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelText1;
        private PowerLib.Winform.Controls.XButton buttonLoadComplete;
        private PowerLib.Winform.Controls.XButton buttonLoad;
        private PowerLib.Winform.Controls.XButton buttonCancel;
        private System.Windows.Forms.ContextMenuStrip advancedLoadContextMenu;
        private System.Windows.Forms.ToolStripMenuItem includeETS;
        private System.Windows.Forms.ToolStripMenuItem includeITA;
        private System.Windows.Forms.ToolStripMenuItem includeAEV;
        private System.Windows.Forms.ToolStripMenuItem includeDSE;
        private System.Windows.Forms.ToolStripMenuItem includeFSE;
        private System.Windows.Forms.ToolStripMenuItem includeSAR;
        private System.Windows.Forms.ToolStripMenuItem includeEAR;
        private System.Windows.Forms.ToolStripMenuItem includeEMI;
        private System.Windows.Forms.ToolStripMenuItem includeESE;
        private System.Windows.Forms.ToolStripMenuItem includeLIT;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel room_searchBar;
        private System.Windows.Forms.Button room_searchButton;
        private System.Windows.Forms.TextBox room_searchField;
        private System.Windows.Forms.ListBox room_listBox;
        private ReaLTaiizor.Controls.SkyComboBox comboBoxMainList;
        private PowerLib.Winform.Controls.DropDownButtonItem dummyItem;
    }
}