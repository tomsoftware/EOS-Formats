﻿namespace Demo
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lstProperty = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeKeys = new System.Windows.Forms.TreeView();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.lblLayerThickness = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lstPartNames = new System.Windows.Forms.CheckedListBox();
            this.labLayerIndex = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labLayerPos = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkFill = new System.Windows.Forms.CheckBox();
            this.txt_m33 = new System.Windows.Forms.TextBox();
            this.txt_m32 = new System.Windows.Forms.TextBox();
            this.txt_m31 = new System.Windows.Forms.TextBox();
            this.txt_m23 = new System.Windows.Forms.TextBox();
            this.txt_m22 = new System.Windows.Forms.TextBox();
            this.txt_m21 = new System.Windows.Forms.TextBox();
            this.txt_m13 = new System.Windows.Forms.TextBox();
            this.txt_m12 = new System.Windows.Forms.TextBox();
            this.txt_m11 = new System.Windows.Forms.TextBox();
            this.picOutput = new System.Windows.Forms.PictureBox();
            this.sliceLayer = new System.Windows.Forms.TrackBar();
            this.txtSliFileName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labServerTime = new System.Windows.Forms.Label();
            this.labLayerNr = new System.Windows.Forms.Label();
            this.labStatusText = new System.Windows.Forms.Label();
            this.labLayerzDelta = new System.Windows.Forms.Label();
            this.labLayer = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.listMachines = new System.Windows.Forms.ComboBox();
            this.txtPsw = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.lstJobs = new System.Windows.Forms.ListView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lstEvents = new System.Windows.Forms.ListView();
            this.lstLayers = new System.Windows.Forms.ListView();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lstParts = new System.Windows.Forms.ListView();
            this.lstJobInfo = new System.Windows.Forms.ListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkShowDebug = new System.Windows.Forms.CheckBox();
            this.btnErrorCLS = new System.Windows.Forms.Button();
            this.lstError = new System.Windows.Forms.ListBox();
            this.timer_refreshState = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliceLayer)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage7);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(829, 372);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lstProperty);
            this.tabPage1.Controls.Add(this.treeKeys);
            this.tabPage1.Controls.Add(this.btnSelectFile);
            this.tabPage1.Controls.Add(this.txtFileName);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btnOpen);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(821, 346);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Job File Viewer";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lstProperty
            // 
            this.lstProperty.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstProperty.Location = new System.Drawing.Point(325, 73);
            this.lstProperty.Name = "lstProperty";
            this.lstProperty.Size = new System.Drawing.Size(482, 267);
            this.lstProperty.TabIndex = 11;
            this.lstProperty.UseCompatibleStateImageBehavior = false;
            this.lstProperty.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Comment";
            this.columnHeader3.Width = 300;
            // 
            // treeKeys
            // 
            this.treeKeys.Location = new System.Drawing.Point(6, 73);
            this.treeKeys.Name = "treeKeys";
            this.treeKeys.Size = new System.Drawing.Size(313, 267);
            this.treeKeys.TabIndex = 10;
            this.treeKeys.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeKeys_AfterSelect);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(520, 28);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(33, 28);
            this.btnSelectFile.TabIndex = 9;
            this.btnSelectFile.Text = "...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(18, 33);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(496, 20);
            this.txtFileName.TabIndex = 8;
            this.txtFileName.Text = "box_job.job";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Filename (*.job, *.cft, *.hpr)";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(669, 23);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(67, 33);
            this.btnOpen.TabIndex = 6;
            this.btnOpen.Text = "open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.lblLayerThickness);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.lstPartNames);
            this.tabPage2.Controls.Add(this.labLayerIndex);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.labLayerPos);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.chkFill);
            this.tabPage2.Controls.Add(this.txt_m33);
            this.tabPage2.Controls.Add(this.txt_m32);
            this.tabPage2.Controls.Add(this.txt_m31);
            this.tabPage2.Controls.Add(this.txt_m23);
            this.tabPage2.Controls.Add(this.txt_m22);
            this.tabPage2.Controls.Add(this.txt_m21);
            this.tabPage2.Controls.Add(this.txt_m13);
            this.tabPage2.Controls.Add(this.txt_m12);
            this.tabPage2.Controls.Add(this.txt_m11);
            this.tabPage2.Controls.Add(this.picOutput);
            this.tabPage2.Controls.Add(this.sliceLayer);
            this.tabPage2.Controls.Add(this.txtSliFileName);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(821, 346);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "show slice";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Filename (*.job, *.sli)";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(447, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(33, 28);
            this.button2.TabIndex = 28;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblLayerThickness
            // 
            this.lblLayerThickness.AutoSize = true;
            this.lblLayerThickness.Location = new System.Drawing.Point(764, 122);
            this.lblLayerThickness.Name = "lblLayerThickness";
            this.lblLayerThickness.Size = new System.Drawing.Size(13, 13);
            this.lblLayerThickness.TabIndex = 27;
            this.lblLayerThickness.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(705, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Layer thickness:";
            // 
            // lstPartNames
            // 
            this.lstPartNames.CheckOnClick = true;
            this.lstPartNames.FormattingEnabled = true;
            this.lstPartNames.Location = new System.Drawing.Point(560, 149);
            this.lstPartNames.Name = "lstPartNames";
            this.lstPartNames.Size = new System.Drawing.Size(255, 184);
            this.lstPartNames.TabIndex = 25;
            this.lstPartNames.SelectedValueChanged += new System.EventHandler(this.lstPartNames_SelectedValueChanged);
            this.lstPartNames.Validated += new System.EventHandler(this.lstPartNames_Validated);
            // 
            // labLayerIndex
            // 
            this.labLayerIndex.AutoSize = true;
            this.labLayerIndex.Location = new System.Drawing.Point(680, 3);
            this.labLayerIndex.Name = "labLayerIndex";
            this.labLayerIndex.Size = new System.Drawing.Size(13, 13);
            this.labLayerIndex.TabIndex = 24;
            this.labLayerIndex.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(647, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Layer:";
            // 
            // labLayerPos
            // 
            this.labLayerPos.AutoSize = true;
            this.labLayerPos.Location = new System.Drawing.Point(741, 3);
            this.labLayerPos.Name = "labLayerPos";
            this.labLayerPos.Size = new System.Drawing.Size(23, 13);
            this.labLayerPos.TabIndex = 22;
            this.labLayerPos.Text = "mm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(561, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Transformation matrix";
            // 
            // chkFill
            // 
            this.chkFill.AutoSize = true;
            this.chkFill.Location = new System.Drawing.Point(718, 63);
            this.chkFill.Name = "chkFill";
            this.chkFill.Size = new System.Drawing.Size(59, 17);
            this.chkFill.TabIndex = 20;
            this.chkFill.Text = "fill slice";
            this.chkFill.UseVisualStyleBackColor = true;
            this.chkFill.CheckedChanged += new System.EventHandler(this.chkFill_CheckedChanged);
            // 
            // txt_m33
            // 
            this.txt_m33.BackColor = System.Drawing.SystemColors.Control;
            this.txt_m33.Enabled = false;
            this.txt_m33.Location = new System.Drawing.Point(651, 115);
            this.txt_m33.Name = "txt_m33";
            this.txt_m33.Size = new System.Drawing.Size(32, 20);
            this.txt_m33.TabIndex = 19;
            this.txt_m33.Text = "1";
            // 
            // txt_m32
            // 
            this.txt_m32.BackColor = System.Drawing.SystemColors.Control;
            this.txt_m32.Enabled = false;
            this.txt_m32.Location = new System.Drawing.Point(613, 115);
            this.txt_m32.Name = "txt_m32";
            this.txt_m32.Size = new System.Drawing.Size(32, 20);
            this.txt_m32.TabIndex = 18;
            this.txt_m32.Tag = "";
            this.txt_m32.Text = "0";
            // 
            // txt_m31
            // 
            this.txt_m31.BackColor = System.Drawing.SystemColors.Control;
            this.txt_m31.Enabled = false;
            this.txt_m31.Location = new System.Drawing.Point(575, 115);
            this.txt_m31.Name = "txt_m31";
            this.txt_m31.Size = new System.Drawing.Size(32, 20);
            this.txt_m31.TabIndex = 17;
            this.txt_m31.Tag = "";
            this.txt_m31.Text = "0";
            // 
            // txt_m23
            // 
            this.txt_m23.Location = new System.Drawing.Point(651, 89);
            this.txt_m23.Name = "txt_m23";
            this.txt_m23.Size = new System.Drawing.Size(32, 20);
            this.txt_m23.TabIndex = 16;
            this.txt_m23.Text = "0";
            // 
            // txt_m22
            // 
            this.txt_m22.Location = new System.Drawing.Point(613, 89);
            this.txt_m22.Name = "txt_m22";
            this.txt_m22.Size = new System.Drawing.Size(32, 20);
            this.txt_m22.TabIndex = 15;
            this.txt_m22.Text = "1";
            // 
            // txt_m21
            // 
            this.txt_m21.Location = new System.Drawing.Point(575, 89);
            this.txt_m21.Name = "txt_m21";
            this.txt_m21.Size = new System.Drawing.Size(32, 20);
            this.txt_m21.TabIndex = 14;
            this.txt_m21.Text = "0";
            // 
            // txt_m13
            // 
            this.txt_m13.Location = new System.Drawing.Point(651, 63);
            this.txt_m13.Name = "txt_m13";
            this.txt_m13.Size = new System.Drawing.Size(32, 20);
            this.txt_m13.TabIndex = 13;
            this.txt_m13.Text = "0";
            // 
            // txt_m12
            // 
            this.txt_m12.Location = new System.Drawing.Point(613, 63);
            this.txt_m12.Name = "txt_m12";
            this.txt_m12.Size = new System.Drawing.Size(32, 20);
            this.txt_m12.TabIndex = 12;
            this.txt_m12.Text = "0";
            // 
            // txt_m11
            // 
            this.txt_m11.Location = new System.Drawing.Point(575, 63);
            this.txt_m11.Name = "txt_m11";
            this.txt_m11.Size = new System.Drawing.Size(32, 20);
            this.txt_m11.TabIndex = 11;
            this.txt_m11.Text = "1";
            // 
            // picOutput
            // 
            this.picOutput.Location = new System.Drawing.Point(6, 42);
            this.picOutput.Name = "picOutput";
            this.picOutput.Size = new System.Drawing.Size(548, 291);
            this.picOutput.TabIndex = 0;
            this.picOutput.TabStop = false;
            this.picOutput.Click += new System.EventHandler(this.picOutput_Click);
            this.picOutput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picOutput_MouseDown);
            this.picOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picOutput_MouseMove);
            // 
            // sliceLayer
            // 
            this.sliceLayer.Location = new System.Drawing.Point(575, 15);
            this.sliceLayer.Name = "sliceLayer";
            this.sliceLayer.Size = new System.Drawing.Size(217, 45);
            this.sliceLayer.TabIndex = 10;
            this.sliceLayer.TickStyle = System.Windows.Forms.TickStyle.None;
            this.sliceLayer.Scroll += new System.EventHandler(this.sliceLayer_Scroll);
            // 
            // txtSliFileName
            // 
            this.txtSliFileName.Location = new System.Drawing.Point(16, 16);
            this.txtSliFileName.Name = "txtSliFileName";
            this.txtSliFileName.Size = new System.Drawing.Size(425, 20);
            this.txtSliFileName.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(486, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "open";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.listMachines);
            this.tabPage4.Controls.Add(this.txtPsw);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.txtUser);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.txtHost);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(821, 346);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "DB-Connect";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(49, 319);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(293, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "First, select a [Job], then you can use [Layer] and [Parts] tab.";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 319);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 13);
            this.label14.TabIndex = 11;
            this.label14.Text = "Info:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labServerTime);
            this.groupBox1.Controls.Add(this.labLayerNr);
            this.groupBox1.Controls.Add(this.labStatusText);
            this.groupBox1.Controls.Add(this.labLayerzDelta);
            this.groupBox1.Controls.Add(this.labLayer);
            this.groupBox1.Location = new System.Drawing.Point(427, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 222);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "State";
            // 
            // labServerTime
            // 
            this.labServerTime.AutoSize = true;
            this.labServerTime.Location = new System.Drawing.Point(18, 37);
            this.labServerTime.Name = "labServerTime";
            this.labServerTime.Size = new System.Drawing.Size(75, 13);
            this.labServerTime.TabIndex = 12;
            this.labServerTime.Text = "labServerTime";
            // 
            // labLayerNr
            // 
            this.labLayerNr.AutoSize = true;
            this.labLayerNr.Location = new System.Drawing.Point(19, 82);
            this.labLayerNr.Name = "labLayerNr";
            this.labLayerNr.Size = new System.Drawing.Size(58, 13);
            this.labLayerNr.TabIndex = 11;
            this.labLayerNr.Text = "labLayerNr";
            // 
            // labStatusText
            // 
            this.labStatusText.Location = new System.Drawing.Point(19, 128);
            this.labStatusText.Name = "labStatusText";
            this.labStatusText.Size = new System.Drawing.Size(299, 85);
            this.labStatusText.TabIndex = 10;
            this.labStatusText.Text = "labStatusText";
            // 
            // labLayerzDelta
            // 
            this.labLayerzDelta.AutoSize = true;
            this.labLayerzDelta.Location = new System.Drawing.Point(19, 100);
            this.labLayerzDelta.Name = "labLayerzDelta";
            this.labLayerzDelta.Size = new System.Drawing.Size(77, 13);
            this.labLayerzDelta.TabIndex = 9;
            this.labLayerzDelta.Text = "labLayerzDelta";
            // 
            // labLayer
            // 
            this.labLayer.AutoSize = true;
            this.labLayer.Location = new System.Drawing.Point(18, 16);
            this.labLayer.Name = "labLayer";
            this.labLayer.Size = new System.Drawing.Size(47, 13);
            this.labLayer.TabIndex = 8;
            this.labLayer.Text = "labLayer";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(214, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Machines:";
            // 
            // listMachines
            // 
            this.listMachines.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.listMachines.FormattingEnabled = true;
            this.listMachines.Location = new System.Drawing.Point(228, 39);
            this.listMachines.Name = "listMachines";
            this.listMachines.Size = new System.Drawing.Size(146, 21);
            this.listMachines.TabIndex = 8;
            this.listMachines.SelectedIndexChanged += new System.EventHandler(this.listMachines_SelectedIndexChanged);
            // 
            // txtPsw
            // 
            this.txtPsw.Location = new System.Drawing.Point(41, 137);
            this.txtPsw.Name = "txtPsw";
            this.txtPsw.PasswordChar = '*';
            this.txtPsw.Size = new System.Drawing.Size(115, 20);
            this.txtPsw.TabIndex = 6;
            this.txtPsw.Text = "%admin4L0gDB.#";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 121);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Password:";
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(41, 85);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(115, 20);
            this.txtUser.TabIndex = 4;
            this.txtUser.Text = "LogDBAdmin";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "User";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(41, 42);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(115, 20);
            this.txtHost.TabIndex = 2;
            this.txtHost.Text = "192.168.1.28";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Server / Host:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(41, 185);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(115, 37);
            this.button3.TabIndex = 0;
            this.button3.Text = "connect";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.lstJobs);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(821, 346);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "DB: Jobs";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // lstJobs
            // 
            this.lstJobs.FullRowSelect = true;
            this.lstJobs.Location = new System.Drawing.Point(3, 3);
            this.lstJobs.Name = "lstJobs";
            this.lstJobs.Size = new System.Drawing.Size(654, 333);
            this.lstJobs.TabIndex = 0;
            this.lstJobs.UseCompatibleStateImageBehavior = false;
            this.lstJobs.View = System.Windows.Forms.View.Details;
            this.lstJobs.SelectedIndexChanged += new System.EventHandler(this.lstJobs_SelectedIndexChanged);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.label13);
            this.tabPage6.Controls.Add(this.label12);
            this.tabPage6.Controls.Add(this.lstEvents);
            this.tabPage6.Controls.Add(this.lstLayers);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(821, 346);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "DB: Layer + Log";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 232);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Log:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 6);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 13);
            this.label12.TabIndex = 8;
            this.label12.Text = "Layers:";
            // 
            // lstEvents
            // 
            this.lstEvents.FullRowSelect = true;
            this.lstEvents.Location = new System.Drawing.Point(50, 232);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(768, 111);
            this.lstEvents.TabIndex = 5;
            this.lstEvents.UseCompatibleStateImageBehavior = false;
            this.lstEvents.View = System.Windows.Forms.View.Details;
            // 
            // lstLayers
            // 
            this.lstLayers.FullRowSelect = true;
            this.lstLayers.Location = new System.Drawing.Point(50, 6);
            this.lstLayers.Name = "lstLayers";
            this.lstLayers.Size = new System.Drawing.Size(680, 220);
            this.lstLayers.TabIndex = 1;
            this.lstLayers.UseCompatibleStateImageBehavior = false;
            this.lstLayers.View = System.Windows.Forms.View.Details;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.label11);
            this.tabPage7.Controls.Add(this.label10);
            this.tabPage7.Controls.Add(this.lstParts);
            this.tabPage7.Controls.Add(this.lstJobInfo);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(821, 346);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "DB: Parts + Config";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 164);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Parts:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Job Parameters:";
            // 
            // lstParts
            // 
            this.lstParts.FullRowSelect = true;
            this.lstParts.Location = new System.Drawing.Point(10, 180);
            this.lstParts.Name = "lstParts";
            this.lstParts.Size = new System.Drawing.Size(798, 163);
            this.lstParts.TabIndex = 6;
            this.lstParts.UseCompatibleStateImageBehavior = false;
            this.lstParts.View = System.Windows.Forms.View.Details;
            // 
            // lstJobInfo
            // 
            this.lstJobInfo.FullRowSelect = true;
            this.lstJobInfo.Location = new System.Drawing.Point(10, 25);
            this.lstJobInfo.Name = "lstJobInfo";
            this.lstJobInfo.Size = new System.Drawing.Size(798, 136);
            this.lstJobInfo.TabIndex = 5;
            this.lstJobInfo.UseCompatibleStateImageBehavior = false;
            this.lstJobInfo.View = System.Windows.Forms.View.Details;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkShowDebug);
            this.tabPage3.Controls.Add(this.btnErrorCLS);
            this.tabPage3.Controls.Add(this.lstError);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(821, 346);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Errors";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkShowDebug
            // 
            this.chkShowDebug.AutoSize = true;
            this.chkShowDebug.Checked = true;
            this.chkShowDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowDebug.Location = new System.Drawing.Point(14, 317);
            this.chkShowDebug.Name = "chkShowDebug";
            this.chkShowDebug.Size = new System.Drawing.Size(86, 17);
            this.chkShowDebug.TabIndex = 2;
            this.chkShowDebug.Text = "show Debug";
            this.chkShowDebug.UseVisualStyleBackColor = true;
            this.chkShowDebug.CheckedChanged += new System.EventHandler(this.chkShowDebug_CheckedChanged);
            // 
            // btnErrorCLS
            // 
            this.btnErrorCLS.Location = new System.Drawing.Point(748, 312);
            this.btnErrorCLS.Name = "btnErrorCLS";
            this.btnErrorCLS.Size = new System.Drawing.Size(59, 25);
            this.btnErrorCLS.TabIndex = 1;
            this.btnErrorCLS.Text = "cls";
            this.btnErrorCLS.UseVisualStyleBackColor = true;
            this.btnErrorCLS.Click += new System.EventHandler(this.btnErrorCLS_Click);
            // 
            // lstError
            // 
            this.lstError.FormattingEnabled = true;
            this.lstError.Location = new System.Drawing.Point(3, 3);
            this.lstError.Name = "lstError";
            this.lstError.Size = new System.Drawing.Size(815, 303);
            this.lstError.TabIndex = 0;
            // 
            // timer_refreshState
            // 
            this.timer_refreshState.Interval = 2000;
            this.timer_refreshState.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 400);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Demo: EOS - File Formats";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sliceLayer)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lstProperty;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TreeView treeKeys;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox picOutput;
        private System.Windows.Forms.TextBox txtSliFileName;
        private System.Windows.Forms.TrackBar sliceLayer;
        private System.Windows.Forms.TextBox txt_m33;
        private System.Windows.Forms.TextBox txt_m32;
        private System.Windows.Forms.TextBox txt_m31;
        private System.Windows.Forms.TextBox txt_m23;
        private System.Windows.Forms.TextBox txt_m22;
        private System.Windows.Forms.TextBox txt_m21;
        private System.Windows.Forms.TextBox txt_m13;
        private System.Windows.Forms.TextBox txt_m12;
        private System.Windows.Forms.TextBox txt_m11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkFill;
        private System.Windows.Forms.Label labLayerPos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labLayerIndex;
        private System.Windows.Forms.CheckedListBox lstPartNames;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox lstError;
        private System.Windows.Forms.CheckBox chkShowDebug;
        private System.Windows.Forms.Button btnErrorCLS;
        private System.Windows.Forms.Label lblLayerThickness;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox txtPsw;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox listMachines;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labServerTime;
        private System.Windows.Forms.Label labLayerNr;
        private System.Windows.Forms.Label labStatusText;
        private System.Windows.Forms.Label labLayerzDelta;
        private System.Windows.Forms.Label labLayer;
        private System.Windows.Forms.Timer timer_refreshState;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ListView lstJobs;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.ListView lstLayers;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.ListView lstJobInfo;
        private System.Windows.Forms.ListView lstEvents;
        private System.Windows.Forms.ListView lstParts;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;

    }
}

