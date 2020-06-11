namespace EDDLite
{
    partial class EDDLiteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EDDLiteForm));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenShotCaptureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdrViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extPanelDataGridViewScrollCmdrs = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.dataGridViewCommanders = new System.Windows.Forms.DataGridView();
            this.ColumnCommander = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EdsmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JournalDirCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NotesCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripCmdr = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extScrollBar1 = new ExtendedControls.ExtScrollBar();
            this.extStatusStrip = new ExtendedControls.ExtStatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.extPanelScrollStatus = new ExtendedControls.ExtPanelScroll();
            this.extButtonEDSY = new ExtendedControls.ExtButton();
            this.extButtonCoriolis = new ExtendedControls.ExtButton();
            this.extButtonInaraStation = new ExtendedControls.ExtButton();
            this.extButtonInaraSystem = new ExtendedControls.ExtButton();
            this.extButtonEDSM = new ExtendedControls.ExtButton();
            this.labelSystem = new System.Windows.Forms.Label();
            this.labelCmdr = new System.Windows.Forms.Label();
            this.labelShip = new System.Windows.Forms.Label();
            this.labelShipH = new System.Windows.Forms.Label();
            this.labelLocation = new System.Windows.Forms.Label();
            this.labelLocationH = new System.Windows.Forms.Label();
            this.labelSysH = new System.Windows.Forms.Label();
            this.labelCmdrH = new System.Windows.Forms.Label();
            this.extScrollBar2 = new ExtendedControls.ExtScrollBar();
            this.flowLayoutPanelTop = new System.Windows.Forms.FlowLayoutPanel();
            this.label_version = new System.Windows.Forms.Label();
            this.labelInfoBoxTop = new System.Windows.Forms.Label();
            this.labelGameDateTime = new System.Windows.Forms.Label();
            this.panel_close = new ExtendedControls.ExtButtonDrawn();
            this.panel_minimize = new ExtendedControls.ExtButtonDrawn();
            this.panel_eddiscovery = new System.Windows.Forms.Panel();
            this.splitContainerDataLogs = new System.Windows.Forms.SplitContainer();
            this.extRichTextBoxLog = new ExtendedControls.ExtRichTextBox();
            this.panelCmdrs = new System.Windows.Forms.Panel();
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanelRight = new System.Windows.Forms.FlowLayoutPanel();
            this.menuMain.SuspendLayout();
            this.extPanelDataGridViewScrollCmdrs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCommanders)).BeginInit();
            this.contextMenuStripCmdr.SuspendLayout();
            this.extStatusStrip.SuspendLayout();
            this.extPanelScrollStatus.SuspendLayout();
            this.flowLayoutPanelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDataLogs)).BeginInit();
            this.splitContainerDataLogs.Panel1.SuspendLayout();
            this.splitContainerDataLogs.Panel2.SuspendLayout();
            this.splitContainerDataLogs.SuspendLayout();
            this.panelCmdrs.SuspendLayout();
            this.tableLayoutPanelTop.SuspendLayout();
            this.flowLayoutPanelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(107, 24);
            this.menuMain.TabIndex = 0;
            this.menuMain.Text = "menuStrip1";
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenShotCaptureToolStripMenuItem});
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.configToolStripMenuItem.Text = "Config";
            // 
            // screenShotCaptureToolStripMenuItem
            // 
            this.screenShotCaptureToolStripMenuItem.Name = "screenShotCaptureToolStripMenuItem";
            this.screenShotCaptureToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.screenShotCaptureToolStripMenuItem.Text = "Screen Shot Capture";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdrViewToolStripMenuItem,
            this.timeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // cmdrViewToolStripMenuItem
            // 
            this.cmdrViewToolStripMenuItem.Checked = true;
            this.cmdrViewToolStripMenuItem.CheckOnClick = true;
            this.cmdrViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cmdrViewToolStripMenuItem.Name = "cmdrViewToolStripMenuItem";
            this.cmdrViewToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cmdrViewToolStripMenuItem.Text = "Commanders";
            // 
            // timeToolStripMenuItem
            // 
            this.timeToolStripMenuItem.Checked = true;
            this.timeToolStripMenuItem.CheckOnClick = true;
            this.timeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
            this.timeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.timeToolStripMenuItem.Text = "Time";
            // 
            // extPanelDataGridViewScrollCmdrs
            // 
            this.extPanelDataGridViewScrollCmdrs.Controls.Add(this.dataGridViewCommanders);
            this.extPanelDataGridViewScrollCmdrs.Controls.Add(this.extScrollBar1);
            this.extPanelDataGridViewScrollCmdrs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelDataGridViewScrollCmdrs.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollCmdrs.LimitLargeChange = 2147483647;
            this.extPanelDataGridViewScrollCmdrs.Location = new System.Drawing.Point(0, 0);
            this.extPanelDataGridViewScrollCmdrs.Name = "extPanelDataGridViewScrollCmdrs";
            this.extPanelDataGridViewScrollCmdrs.Size = new System.Drawing.Size(794, 92);
            this.extPanelDataGridViewScrollCmdrs.TabIndex = 1;
            this.extPanelDataGridViewScrollCmdrs.VerticalScrollBarDockRight = true;
            // 
            // dataGridViewCommanders
            // 
            this.dataGridViewCommanders.AllowUserToAddRows = false;
            this.dataGridViewCommanders.AllowUserToDeleteRows = false;
            this.dataGridViewCommanders.AllowUserToResizeRows = false;
            this.dataGridViewCommanders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCommanders.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewCommanders.CausesValidation = false;
            this.dataGridViewCommanders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCommanders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCommander,
            this.EdsmName,
            this.JournalDirCol,
            this.NotesCol});
            this.dataGridViewCommanders.ContextMenuStrip = this.contextMenuStripCmdr;
            this.dataGridViewCommanders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCommanders.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewCommanders.MultiSelect = false;
            this.dataGridViewCommanders.Name = "dataGridViewCommanders";
            this.dataGridViewCommanders.ReadOnly = true;
            this.dataGridViewCommanders.RowHeadersVisible = false;
            this.dataGridViewCommanders.RowHeadersWidth = 20;
            this.dataGridViewCommanders.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewCommanders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCommanders.Size = new System.Drawing.Size(778, 92);
            this.dataGridViewCommanders.TabIndex = 3;
            this.dataGridViewCommanders.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCommanders_CellDoubleClick);
            this.dataGridViewCommanders.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridViewCommanders_MouseDown);
            // 
            // ColumnCommander
            // 
            this.ColumnCommander.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnCommander.DataPropertyName = "Name";
            this.ColumnCommander.FillWeight = 120F;
            this.ColumnCommander.HeaderText = "Commander";
            this.ColumnCommander.MinimumWidth = 50;
            this.ColumnCommander.Name = "ColumnCommander";
            this.ColumnCommander.ReadOnly = true;
            // 
            // EdsmName
            // 
            this.EdsmName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.EdsmName.DataPropertyName = "EdsmName";
            this.EdsmName.HeaderText = "EDSM Name";
            this.EdsmName.MinimumWidth = 50;
            this.EdsmName.Name = "EdsmName";
            this.EdsmName.ReadOnly = true;
            // 
            // JournalDirCol
            // 
            this.JournalDirCol.DataPropertyName = "JournalDir";
            this.JournalDirCol.FillWeight = 120F;
            this.JournalDirCol.HeaderText = "Journal Folder";
            this.JournalDirCol.MinimumWidth = 50;
            this.JournalDirCol.Name = "JournalDirCol";
            this.JournalDirCol.ReadOnly = true;
            // 
            // NotesCol
            // 
            this.NotesCol.DataPropertyName = "Info";
            this.NotesCol.FillWeight = 180F;
            this.NotesCol.HeaderText = "Notes";
            this.NotesCol.MinimumWidth = 50;
            this.NotesCol.Name = "NotesCol";
            this.NotesCol.ReadOnly = true;
            // 
            // contextMenuStripCmdr
            // 
            this.contextMenuStripCmdr.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.addToolStripMenuItem});
            this.contextMenuStripCmdr.Name = "contextMenuStripCmdr";
            this.contextMenuStripCmdr.Size = new System.Drawing.Size(108, 70);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // extScrollBar1
            // 
            this.extScrollBar1.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBar1.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBar1.ArrowColorScaling = 0.5F;
            this.extScrollBar1.ArrowDownDrawAngle = 270F;
            this.extScrollBar1.ArrowUpDrawAngle = 90F;
            this.extScrollBar1.BorderColor = System.Drawing.Color.White;
            this.extScrollBar1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBar1.HideScrollBar = true;
            this.extScrollBar1.LargeChange = 0;
            this.extScrollBar1.Location = new System.Drawing.Point(778, 0);
            this.extScrollBar1.Maximum = -1;
            this.extScrollBar1.Minimum = 0;
            this.extScrollBar1.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBar1.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBar1.Name = "extScrollBar1";
            this.extScrollBar1.Size = new System.Drawing.Size(16, 92);
            this.extScrollBar1.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBar1.SmallChange = 1;
            this.extScrollBar1.TabIndex = 0;
            this.extScrollBar1.Text = "extScrollBar1";
            this.extScrollBar1.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBar1.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBar1.ThumbColorScaling = 0.5F;
            this.extScrollBar1.ThumbDrawAngle = 0F;
            this.extScrollBar1.Value = -1;
            this.extScrollBar1.ValueLimited = -1;
            // 
            // extStatusStrip
            // 
            this.extStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripStatus});
            this.extStatusStrip.Location = new System.Drawing.Point(3, 425);
            this.extStatusStrip.Name = "extStatusStrip";
            this.extStatusStrip.Size = new System.Drawing.Size(794, 22);
            this.extStatusStrip.TabIndex = 4;
            this.extStatusStrip.Text = "extStatusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Enabled = false;
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // extPanelScrollStatus
            // 
            this.extPanelScrollStatus.Controls.Add(this.extButtonEDSY);
            this.extPanelScrollStatus.Controls.Add(this.extButtonCoriolis);
            this.extPanelScrollStatus.Controls.Add(this.extButtonInaraStation);
            this.extPanelScrollStatus.Controls.Add(this.extButtonInaraSystem);
            this.extPanelScrollStatus.Controls.Add(this.extButtonEDSM);
            this.extPanelScrollStatus.Controls.Add(this.labelSystem);
            this.extPanelScrollStatus.Controls.Add(this.labelCmdr);
            this.extPanelScrollStatus.Controls.Add(this.labelShip);
            this.extPanelScrollStatus.Controls.Add(this.labelShipH);
            this.extPanelScrollStatus.Controls.Add(this.labelLocation);
            this.extPanelScrollStatus.Controls.Add(this.labelLocationH);
            this.extPanelScrollStatus.Controls.Add(this.labelSysH);
            this.extPanelScrollStatus.Controls.Add(this.labelCmdrH);
            this.extPanelScrollStatus.Controls.Add(this.extScrollBar2);
            this.extPanelScrollStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelScrollStatus.FlowControlsLeftToRight = false;
            this.extPanelScrollStatus.Location = new System.Drawing.Point(0, 0);
            this.extPanelScrollStatus.Name = "extPanelScrollStatus";
            this.extPanelScrollStatus.Size = new System.Drawing.Size(794, 212);
            this.extPanelScrollStatus.TabIndex = 5;
            this.extPanelScrollStatus.VerticalScrollBarDockRight = true;
            // 
            // extButtonEDSY
            // 
            this.extButtonEDSY.Image = global::EDDLite.Properties.Resources.EDShipYard;
            this.extButtonEDSY.Location = new System.Drawing.Point(262, 112);
            this.extButtonEDSY.Name = "extButtonEDSY";
            this.extButtonEDSY.Size = new System.Drawing.Size(42, 33);
            this.extButtonEDSY.TabIndex = 6;
            this.extButtonEDSY.UseVisualStyleBackColor = true;
            this.extButtonEDSY.Click += new System.EventHandler(this.extButtonEDSY_Click);
            // 
            // extButtonCoriolis
            // 
            this.extButtonCoriolis.Image = global::EDDLite.Properties.Resources.ShowOnCoriolis;
            this.extButtonCoriolis.Location = new System.Drawing.Point(214, 112);
            this.extButtonCoriolis.Name = "extButtonCoriolis";
            this.extButtonCoriolis.Size = new System.Drawing.Size(42, 33);
            this.extButtonCoriolis.TabIndex = 6;
            this.extButtonCoriolis.UseVisualStyleBackColor = true;
            this.extButtonCoriolis.Click += new System.EventHandler(this.extButtonCoriolis_Click);
            // 
            // extButtonInaraStation
            // 
            this.extButtonInaraStation.Image = global::EDDLite.Properties.Resources.Inara;
            this.extButtonInaraStation.Location = new System.Drawing.Point(215, 75);
            this.extButtonInaraStation.Name = "extButtonInaraStation";
            this.extButtonInaraStation.Size = new System.Drawing.Size(42, 33);
            this.extButtonInaraStation.TabIndex = 6;
            this.extButtonInaraStation.UseVisualStyleBackColor = true;
            this.extButtonInaraStation.Click += new System.EventHandler(this.extButtonInaraStation_Click);
            // 
            // extButtonInaraSystem
            // 
            this.extButtonInaraSystem.Image = global::EDDLite.Properties.Resources.Inara;
            this.extButtonInaraSystem.Location = new System.Drawing.Point(263, 37);
            this.extButtonInaraSystem.Name = "extButtonInaraSystem";
            this.extButtonInaraSystem.Size = new System.Drawing.Size(42, 33);
            this.extButtonInaraSystem.TabIndex = 6;
            this.extButtonInaraSystem.UseVisualStyleBackColor = true;
            this.extButtonInaraSystem.Click += new System.EventHandler(this.extButtonInaraSystem_Click);
            // 
            // extButtonEDSM
            // 
            this.extButtonEDSM.Image = global::EDDLite.Properties.Resources.EDSM;
            this.extButtonEDSM.Location = new System.Drawing.Point(215, 37);
            this.extButtonEDSM.Name = "extButtonEDSM";
            this.extButtonEDSM.Size = new System.Drawing.Size(42, 33);
            this.extButtonEDSM.TabIndex = 6;
            this.extButtonEDSM.UseVisualStyleBackColor = true;
            this.extButtonEDSM.Click += new System.EventHandler(this.extButtonEDSM_Click);
            // 
            // labelSystem
            // 
            this.labelSystem.AutoSize = true;
            this.labelSystem.Location = new System.Drawing.Point(60, 47);
            this.labelSystem.Name = "labelSystem";
            this.labelSystem.Size = new System.Drawing.Size(13, 13);
            this.labelSystem.TabIndex = 5;
            this.labelSystem.Text = "?";
            // 
            // labelCmdr
            // 
            this.labelCmdr.AutoSize = true;
            this.labelCmdr.Location = new System.Drawing.Point(60, 16);
            this.labelCmdr.Name = "labelCmdr";
            this.labelCmdr.Size = new System.Drawing.Size(13, 13);
            this.labelCmdr.TabIndex = 4;
            this.labelCmdr.Text = "?";
            // 
            // labelShip
            // 
            this.labelShip.AutoSize = true;
            this.labelShip.Location = new System.Drawing.Point(59, 122);
            this.labelShip.Name = "labelShip";
            this.labelShip.Size = new System.Drawing.Size(13, 13);
            this.labelShip.TabIndex = 3;
            this.labelShip.Text = "?";
            // 
            // labelShipH
            // 
            this.labelShipH.AutoSize = true;
            this.labelShipH.Location = new System.Drawing.Point(3, 122);
            this.labelShipH.Name = "labelShipH";
            this.labelShipH.Size = new System.Drawing.Size(28, 13);
            this.labelShipH.TabIndex = 3;
            this.labelShipH.Text = "Ship";
            // 
            // labelLocation
            // 
            this.labelLocation.AutoSize = true;
            this.labelLocation.Location = new System.Drawing.Point(60, 85);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(13, 13);
            this.labelLocation.TabIndex = 2;
            this.labelLocation.Text = "?";
            // 
            // labelLocationH
            // 
            this.labelLocationH.AutoSize = true;
            this.labelLocationH.Location = new System.Drawing.Point(4, 85);
            this.labelLocationH.Name = "labelLocationH";
            this.labelLocationH.Size = new System.Drawing.Size(48, 13);
            this.labelLocationH.TabIndex = 2;
            this.labelLocationH.Text = "Location";
            // 
            // labelSysH
            // 
            this.labelSysH.AutoSize = true;
            this.labelSysH.Location = new System.Drawing.Point(4, 47);
            this.labelSysH.Name = "labelSysH";
            this.labelSysH.Size = new System.Drawing.Size(41, 13);
            this.labelSysH.TabIndex = 2;
            this.labelSysH.Text = "System";
            // 
            // labelCmdrH
            // 
            this.labelCmdrH.AutoSize = true;
            this.labelCmdrH.Location = new System.Drawing.Point(4, 16);
            this.labelCmdrH.Name = "labelCmdrH";
            this.labelCmdrH.Size = new System.Drawing.Size(31, 13);
            this.labelCmdrH.TabIndex = 1;
            this.labelCmdrH.Text = "Cmdr";
            // 
            // extScrollBar2
            // 
            this.extScrollBar2.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBar2.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBar2.ArrowColorScaling = 0.5F;
            this.extScrollBar2.ArrowDownDrawAngle = 270F;
            this.extScrollBar2.ArrowUpDrawAngle = 90F;
            this.extScrollBar2.BorderColor = System.Drawing.Color.White;
            this.extScrollBar2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBar2.HideScrollBar = true;
            this.extScrollBar2.LargeChange = 10;
            this.extScrollBar2.Location = new System.Drawing.Point(778, 0);
            this.extScrollBar2.Maximum = -58;
            this.extScrollBar2.Minimum = 0;
            this.extScrollBar2.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBar2.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBar2.Name = "extScrollBar2";
            this.extScrollBar2.Size = new System.Drawing.Size(16, 212);
            this.extScrollBar2.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBar2.SmallChange = 1;
            this.extScrollBar2.TabIndex = 0;
            this.extScrollBar2.Text = "extScrollBar2";
            this.extScrollBar2.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBar2.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBar2.ThumbColorScaling = 0.5F;
            this.extScrollBar2.ThumbDrawAngle = 0F;
            this.extScrollBar2.Value = -58;
            this.extScrollBar2.ValueLimited = -58;
            // 
            // flowLayoutPanelTop
            // 
            this.flowLayoutPanelTop.AutoSize = true;
            this.flowLayoutPanelTop.Controls.Add(this.menuMain);
            this.flowLayoutPanelTop.Controls.Add(this.label_version);
            this.flowLayoutPanelTop.Controls.Add(this.labelInfoBoxTop);
            this.flowLayoutPanelTop.Controls.Add(this.labelGameDateTime);
            this.flowLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelTop.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanelTop.Name = "flowLayoutPanelTop";
            this.flowLayoutPanelTop.Size = new System.Drawing.Size(696, 24);
            this.flowLayoutPanelTop.TabIndex = 6;
            this.flowLayoutPanelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.flowLayoutPanel1_MouseDown);
            this.flowLayoutPanelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.flowLayoutPanel1_MouseUp);
            // 
            // label_version
            // 
            this.label_version.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label_version.AutoSize = true;
            this.label_version.Location = new System.Drawing.Point(115, 6);
            this.label_version.Margin = new System.Windows.Forms.Padding(8, 1, 3, 0);
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(43, 13);
            this.label_version.TabIndex = 24;
            this.label_version.Text = "<code>";
            this.label_version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelInfoBoxTop
            // 
            this.labelInfoBoxTop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelInfoBoxTop.AutoSize = true;
            this.labelInfoBoxTop.Location = new System.Drawing.Point(169, 6);
            this.labelInfoBoxTop.Margin = new System.Windows.Forms.Padding(8, 1, 3, 0);
            this.labelInfoBoxTop.Name = "labelInfoBoxTop";
            this.labelInfoBoxTop.Size = new System.Drawing.Size(43, 13);
            this.labelInfoBoxTop.TabIndex = 22;
            this.labelInfoBoxTop.Text = "<code>";
            // 
            // labelGameDateTime
            // 
            this.labelGameDateTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelGameDateTime.AutoSize = true;
            this.labelGameDateTime.Location = new System.Drawing.Point(223, 6);
            this.labelGameDateTime.Margin = new System.Windows.Forms.Padding(8, 1, 3, 0);
            this.labelGameDateTime.Name = "labelGameDateTime";
            this.labelGameDateTime.Size = new System.Drawing.Size(43, 13);
            this.labelGameDateTime.TabIndex = 23;
            this.labelGameDateTime.Text = "<code>";
            // 
            // panel_close
            // 
            this.panel_close.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.panel_close.AutoEllipsis = false;
            this.panel_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel_close.Image = null;
            this.panel_close.ImageSelected = ExtendedControls.ExtButtonDrawn.ImageType.Close;
            this.panel_close.Location = new System.Drawing.Point(63, 2);
            this.panel_close.Margin = new System.Windows.Forms.Padding(0);
            this.panel_close.MouseOverColor = System.Drawing.Color.White;
            this.panel_close.MouseSelectedColor = System.Drawing.Color.Green;
            this.panel_close.MouseSelectedColorEnable = true;
            this.panel_close.Name = "panel_close";
            this.panel_close.Padding = new System.Windows.Forms.Padding(3);
            this.panel_close.PanelDisabledScaling = 0.25F;
            this.panel_close.Selectable = false;
            this.panel_close.Size = new System.Drawing.Size(16, 16);
            this.panel_close.TabIndex = 23;
            this.panel_close.TabStop = false;
            this.panel_close.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panel_close.UseMnemonic = true;
            this.panel_close.Click += new System.EventHandler(this.panel_close_Click);
            // 
            // panel_minimize
            // 
            this.panel_minimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_minimize.AutoEllipsis = false;
            this.panel_minimize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel_minimize.Image = null;
            this.panel_minimize.ImageSelected = ExtendedControls.ExtButtonDrawn.ImageType.Minimize;
            this.panel_minimize.Location = new System.Drawing.Point(47, 0);
            this.panel_minimize.Margin = new System.Windows.Forms.Padding(0);
            this.panel_minimize.MouseOverColor = System.Drawing.Color.White;
            this.panel_minimize.MouseSelectedColor = System.Drawing.Color.Green;
            this.panel_minimize.MouseSelectedColorEnable = true;
            this.panel_minimize.Name = "panel_minimize";
            this.panel_minimize.Padding = new System.Windows.Forms.Padding(3);
            this.panel_minimize.PanelDisabledScaling = 0.25F;
            this.panel_minimize.Selectable = false;
            this.panel_minimize.Size = new System.Drawing.Size(16, 16);
            this.panel_minimize.TabIndex = 22;
            this.panel_minimize.TabStop = false;
            this.panel_minimize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.panel_minimize.UseMnemonic = true;
            this.panel_minimize.Click += new System.EventHandler(this.panel_minimize_Click);
            // 
            // panel_eddiscovery
            // 
            this.panel_eddiscovery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_eddiscovery.BackColor = System.Drawing.SystemColors.Control;
            this.panel_eddiscovery.BackgroundImage = global::EDDLite.Properties.Resources.Logo;
            this.panel_eddiscovery.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel_eddiscovery.Location = new System.Drawing.Point(0, 0);
            this.panel_eddiscovery.Margin = new System.Windows.Forms.Padding(0);
            this.panel_eddiscovery.Name = "panel_eddiscovery";
            this.panel_eddiscovery.Size = new System.Drawing.Size(47, 20);
            this.panel_eddiscovery.TabIndex = 21;
            this.panel_eddiscovery.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel_eddiscovery_MouseClick);
            // 
            // splitContainerDataLogs
            // 
            this.splitContainerDataLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDataLogs.Location = new System.Drawing.Point(3, 125);
            this.splitContainerDataLogs.Name = "splitContainerDataLogs";
            this.splitContainerDataLogs.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerDataLogs.Panel1
            // 
            this.splitContainerDataLogs.Panel1.Controls.Add(this.extPanelScrollStatus);
            // 
            // splitContainerDataLogs.Panel2
            // 
            this.splitContainerDataLogs.Panel2.Controls.Add(this.extRichTextBoxLog);
            this.splitContainerDataLogs.Size = new System.Drawing.Size(794, 300);
            this.splitContainerDataLogs.SplitterDistance = 212;
            this.splitContainerDataLogs.TabIndex = 1;
            // 
            // extRichTextBoxLog
            // 
            this.extRichTextBoxLog.BorderColor = System.Drawing.Color.Transparent;
            this.extRichTextBoxLog.BorderColorScaling = 0.5F;
            this.extRichTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extRichTextBoxLog.HideScrollBar = true;
            this.extRichTextBoxLog.Location = new System.Drawing.Point(0, 0);
            this.extRichTextBoxLog.Name = "extRichTextBoxLog";
            this.extRichTextBoxLog.ReadOnly = false;
            this.extRichTextBoxLog.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft S" +
    "ans Serif;}}\r\n\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
            this.extRichTextBoxLog.ScrollBarArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extRichTextBoxLog.ScrollBarArrowButtonColor = System.Drawing.Color.LightGray;
            this.extRichTextBoxLog.ScrollBarBackColor = System.Drawing.SystemColors.Control;
            this.extRichTextBoxLog.ScrollBarBorderColor = System.Drawing.Color.White;
            this.extRichTextBoxLog.ScrollBarFlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extRichTextBoxLog.ScrollBarForeColor = System.Drawing.SystemColors.ControlText;
            this.extRichTextBoxLog.ScrollBarMouseOverButtonColor = System.Drawing.Color.Green;
            this.extRichTextBoxLog.ScrollBarMousePressedButtonColor = System.Drawing.Color.Red;
            this.extRichTextBoxLog.ScrollBarSliderColor = System.Drawing.Color.DarkGray;
            this.extRichTextBoxLog.ScrollBarThumbBorderColor = System.Drawing.Color.Yellow;
            this.extRichTextBoxLog.ScrollBarThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extRichTextBoxLog.ShowLineCount = false;
            this.extRichTextBoxLog.Size = new System.Drawing.Size(794, 84);
            this.extRichTextBoxLog.TabIndex = 0;
            this.extRichTextBoxLog.TextBoxBackColor = System.Drawing.SystemColors.Control;
            this.extRichTextBoxLog.TextBoxForeColor = System.Drawing.SystemColors.ControlText;
            // 
            // panelCmdrs
            // 
            this.panelCmdrs.Controls.Add(this.extPanelDataGridViewScrollCmdrs);
            this.panelCmdrs.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCmdrs.Location = new System.Drawing.Point(3, 33);
            this.panelCmdrs.Name = "panelCmdrs";
            this.panelCmdrs.Size = new System.Drawing.Size(794, 92);
            this.panelCmdrs.TabIndex = 1;
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.AutoSize = true;
            this.tableLayoutPanelTop.ColumnCount = 2;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTop.Controls.Add(this.flowLayoutPanelTop, 0, 0);
            this.tableLayoutPanelTop.Controls.Add(this.flowLayoutPanelRight, 1, 0);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.RowCount = 1;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(794, 30);
            this.tableLayoutPanelTop.TabIndex = 24;
            // 
            // flowLayoutPanelRight
            // 
            this.flowLayoutPanelRight.Controls.Add(this.panel_eddiscovery);
            this.flowLayoutPanelRight.Controls.Add(this.panel_minimize);
            this.flowLayoutPanelRight.Controls.Add(this.panel_close);
            this.flowLayoutPanelRight.Location = new System.Drawing.Point(705, 3);
            this.flowLayoutPanelRight.Name = "flowLayoutPanelRight";
            this.flowLayoutPanelRight.Size = new System.Drawing.Size(86, 24);
            this.flowLayoutPanelRight.TabIndex = 7;
            // 
            // EDDLiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainerDataLogs);
            this.Controls.Add(this.panelCmdrs);
            this.Controls.Add(this.tableLayoutPanelTop);
            this.Controls.Add(this.extStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Name = "EDDLiteForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "EDD Lite";
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.extPanelDataGridViewScrollCmdrs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCommanders)).EndInit();
            this.contextMenuStripCmdr.ResumeLayout(false);
            this.extStatusStrip.ResumeLayout(false);
            this.extStatusStrip.PerformLayout();
            this.extPanelScrollStatus.ResumeLayout(false);
            this.extPanelScrollStatus.PerformLayout();
            this.flowLayoutPanelTop.ResumeLayout(false);
            this.flowLayoutPanelTop.PerformLayout();
            this.splitContainerDataLogs.Panel1.ResumeLayout(false);
            this.splitContainerDataLogs.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDataLogs)).EndInit();
            this.splitContainerDataLogs.ResumeLayout(false);
            this.panelCmdrs.ResumeLayout(false);
            this.tableLayoutPanelTop.ResumeLayout(false);
            this.tableLayoutPanelTop.PerformLayout();
            this.flowLayoutPanelRight.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private ExtendedControls.ExtPanelDataGridViewScroll extPanelDataGridViewScrollCmdrs;
        private ExtendedControls.ExtScrollBar extScrollBar1;
        private ExtendedControls.ExtStatusStrip extStatusStrip;
        private ExtendedControls.ExtPanelScroll extPanelScrollStatus;
        private ExtendedControls.ExtScrollBar extScrollBar2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTop;
        private System.Windows.Forms.SplitContainer splitContainerDataLogs;
        private ExtendedControls.ExtRichTextBox extRichTextBoxLog;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.Panel panelCmdrs;
        private System.Windows.Forms.DataGridView dataGridViewCommanders;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCommander;
        private System.Windows.Forms.DataGridViewTextBoxColumn EdsmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn JournalDirCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn NotesCol;
        private ExtendedControls.ExtButtonDrawn panel_close;
        private ExtendedControls.ExtButtonDrawn panel_minimize;
        private System.Windows.Forms.Panel panel_eddiscovery;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelRight;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.Label labelInfoBoxTop;
        private System.Windows.Forms.Label labelGameDateTime;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCmdr;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.Label labelSystem;
        private System.Windows.Forms.Label labelCmdr;
        private System.Windows.Forms.Label labelShipH;
        private System.Windows.Forms.Label labelSysH;
        private System.Windows.Forms.Label labelCmdrH;
        private ExtendedControls.ExtButton extButtonCoriolis;
        private ExtendedControls.ExtButton extButtonEDSM;
        private System.Windows.Forms.Label labelShip;
        private ExtendedControls.ExtButton extButtonEDSY;
        private System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.Label labelLocationH;
        private ExtendedControls.ExtButton extButtonInaraStation;
        private ExtendedControls.ExtButton extButtonInaraSystem;
        private System.Windows.Forms.ToolStripMenuItem screenShotCaptureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmdrViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
    }
}

