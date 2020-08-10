namespace JdbWrapper
{
    partial class DebuggerGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebuggerGUI));
            this.stepInButton = new System.Windows.Forms.Button();
            this.stepOverButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.suspendButton = new System.Windows.Forms.Button();
            this.resumeButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.outputTextbox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outputTextbox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stepInButton
            // 
            this.stepInButton.BackColor = System.Drawing.Color.White;
            this.stepInButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stepInButton.Location = new System.Drawing.Point(68, 3);
            this.stepInButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stepInButton.Name = "stepInButton";
            this.stepInButton.Size = new System.Drawing.Size(75, 32);
            this.stepInButton.TabIndex = 2;
            this.stepInButton.Text = "Step In";
            this.stepInButton.UseVisualStyleBackColor = false;
            this.stepInButton.Click += new System.EventHandler(this.Step_Click);
            // 
            // stepOverButton
            // 
            this.stepOverButton.BackColor = System.Drawing.Color.White;
            this.stepOverButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stepOverButton.Location = new System.Drawing.Point(63, 41);
            this.stepOverButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stepOverButton.Name = "stepOverButton";
            this.stepOverButton.Size = new System.Drawing.Size(102, 31);
            this.stepOverButton.TabIndex = 3;
            this.stepOverButton.Text = "Step Over";
            this.stepOverButton.UseVisualStyleBackColor = false;
            this.stepOverButton.Click += new System.EventHandler(this.Next_Click);
            // 
            // continueButton
            // 
            this.continueButton.BackColor = System.Drawing.Color.White;
            this.continueButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.continueButton.Location = new System.Drawing.Point(149, 3);
            this.continueButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(96, 32);
            this.continueButton.TabIndex = 4;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = false;
            this.continueButton.Click += new System.EventHandler(this.Continue_Click);
            // 
            // suspendButton
            // 
            this.suspendButton.BackColor = System.Drawing.Color.White;
            this.suspendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.suspendButton.Location = new System.Drawing.Point(253, 3);
            this.suspendButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.suspendButton.Name = "suspendButton";
            this.suspendButton.Size = new System.Drawing.Size(83, 31);
            this.suspendButton.TabIndex = 5;
            this.suspendButton.Text = "Suspend";
            this.suspendButton.UseVisualStyleBackColor = false;
            this.suspendButton.Click += new System.EventHandler(this.Suspend_Click);
            // 
            // resumeButton
            // 
            this.resumeButton.BackColor = System.Drawing.Color.White;
            this.resumeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resumeButton.Location = new System.Drawing.Point(253, 40);
            this.resumeButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.resumeButton.Name = "resumeButton";
            this.resumeButton.Size = new System.Drawing.Size(83, 32);
            this.resumeButton.TabIndex = 6;
            this.resumeButton.Text = "Resume";
            this.resumeButton.UseVisualStyleBackColor = false;
            this.resumeButton.Click += new System.EventHandler(this.Resume_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Location = new System.Drawing.Point(6, 87);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1084, 242);
            this.tabControl1.TabIndex = 7;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl2.Location = new System.Drawing.Point(0, 335);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1092, 397);
            this.tabControl2.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.outputTextbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabPage1.Size = new System.Drawing.Size(1084, 367);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Output";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // outputTextbox
            // 
            this.outputTextbox.AcceptsReturn = false;
            this.outputTextbox.AcceptsTab = false;
            this.outputTextbox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.outputTextbox.AutoIndent = false;
            this.outputTextbox.AutoIndentChars = false;
            this.outputTextbox.AutoScrollMinSize = new System.Drawing.Size(2, 18);
            this.outputTextbox.BackBrush = null;
            this.outputTextbox.CharHeight = 18;
            this.outputTextbox.CharWidth = 10;
            this.outputTextbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.outputTextbox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.outputTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTextbox.IsReplaceMode = false;
            this.outputTextbox.Location = new System.Drawing.Point(4, 3);
            this.outputTextbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.outputTextbox.Name = "outputTextbox";
            this.outputTextbox.Paddings = new System.Windows.Forms.Padding(0);
            this.outputTextbox.ReadOnly = true;
            this.outputTextbox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.outputTextbox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("outputTextbox.ServiceColors")));
            this.outputTextbox.ShowLineNumbers = false;
            this.outputTextbox.Size = new System.Drawing.Size(1076, 361);
            this.outputTextbox.TabIndex = 0;
            this.outputTextbox.Zoom = 100;
            // 
            // openButton
            // 
            this.openButton.BackColor = System.Drawing.Color.White;
            this.openButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openButton.Location = new System.Drawing.Point(4, 3);
            this.openButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(56, 32);
            this.openButton.TabIndex = 10;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = false;
            this.openButton.Click += new System.EventHandler(this.Open_Click);
            // 
            // runButton
            // 
            this.runButton.BackColor = System.Drawing.Color.White;
            this.runButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.runButton.Location = new System.Drawing.Point(4, 41);
            this.runButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(51, 31);
            this.runButton.TabIndex = 11;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = false;
            this.runButton.Click += new System.EventHandler(this.Run_Click);
            // 
            // nextButton
            // 
            this.nextButton.BackColor = System.Drawing.Color.White;
            this.nextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextButton.Location = new System.Drawing.Point(173, 41);
            this.nextButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(72, 31);
            this.nextButton.TabIndex = 12;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = false;
            this.nextButton.Click += new System.EventHandler(this.StepOver_Click);
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.Color.White;
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Location = new System.Drawing.Point(668, 41);
            this.button12.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(152, 34);
            this.button12.TabIndex = 17;
            this.button12.Text = "List Classes";
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.Classes_Click);
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.Color.White;
            this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button13.Location = new System.Drawing.Point(806, 4);
            this.button13.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(120, 31);
            this.button13.TabIndex = 18;
            this.button13.Text = "List Fields";
            this.button13.UseVisualStyleBackColor = false;
            this.button13.Click += new System.EventHandler(this.Fields_Click);
            // 
            // button14
            // 
            this.button14.BackColor = System.Drawing.Color.White;
            this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button14.Location = new System.Drawing.Point(668, 4);
            this.button14.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(130, 31);
            this.button14.TabIndex = 19;
            this.button14.Text = "List Methods";
            this.button14.UseVisualStyleBackColor = false;
            this.button14.Click += new System.EventHandler(this.Methods_Click);
            // 
            // button15
            // 
            this.button15.BackColor = System.Drawing.Color.White;
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button15.Location = new System.Drawing.Point(824, 41);
            this.button15.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(144, 34);
            this.button15.TabIndex = 20;
            this.button15.Text = "List Threads";
            this.button15.UseVisualStyleBackColor = false;
            this.button15.Click += new System.EventHandler(this.Threads_Click);
            // 
            // helpButton
            // 
            this.helpButton.BackColor = System.Drawing.Color.White;
            this.helpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.helpButton.Location = new System.Drawing.Point(455, 3);
            this.helpButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(72, 32);
            this.helpButton.TabIndex = 21;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = false;
            this.helpButton.Click += new System.EventHandler(this.Help_Click);
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.White;
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Location = new System.Drawing.Point(934, 3);
            this.button11.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(86, 32);
            this.button11.TabIndex = 22;
            this.button11.Text = "Locals";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.Locals_Click);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.White;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Location = new System.Drawing.Point(623, 41);
            this.button10.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(36, 31);
            this.button10.TabIndex = 24;
            this.button10.Text = "Go";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.DirectCommand_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Bodoni MT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(341, 41);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(276, 31);
            this.textBox1.TabIndex = 23;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.button10);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.button11);
            this.panel1.Controls.Add(this.helpButton);
            this.panel1.Controls.Add(this.button15);
            this.panel1.Controls.Add(this.button14);
            this.panel1.Controls.Add(this.button13);
            this.panel1.Controls.Add(this.button12);
            this.panel1.Controls.Add(this.nextButton);
            this.panel1.Controls.Add(this.runButton);
            this.panel1.Controls.Add(this.openButton);
            this.panel1.Controls.Add(this.resumeButton);
            this.panel1.Controls.Add(this.suspendButton);
            this.panel1.Controls.Add(this.continueButton);
            this.panel1.Controls.Add(this.stepOverButton);
            this.panel1.Controls.Add(this.stepInButton);
            this.panel1.Location = new System.Drawing.Point(33, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 77);
            this.panel1.TabIndex = 25;
            // 
            // DebuggerGUI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1092, 732);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Courier New", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DebuggerGUI";
            this.Text = "JDB GUI";
            this.Load += new System.EventHandler(this.DebuggerGUI_Load);
            this.SizeChanged += new System.EventHandler(this.DebuggerGUI_SizeChanged);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outputTextbox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button stepInButton;
        private System.Windows.Forms.Button stepOverButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Button suspendButton;
        private System.Windows.Forms.Button resumeButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.TabPage tabPage1;
        private FastColoredTextBoxNS.FastColoredTextBox outputTextbox;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel1;
    }
}