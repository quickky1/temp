namespace PS4_BO3_GSC
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.styleManager = new MetroFramework.Components.MetroStyleManager(this.components);
            this.connectionGroupBox = new System.Windows.Forms.GroupBox();
            this.detectFirmwareButton = new System.Windows.Forms.Button();
            this.firmwareComboBox = new MetroFramework.Controls.MetroComboBox();
            this.firmwareLabel = new MetroFramework.Controls.MetroLabel();
            this.attachBo3Button = new System.Windows.Forms.Button();
            this.connectPS4Button = new System.Windows.Forms.Button();
            this.ps4PortTextBox = new MetroFramework.Controls.MetroTextBox();
            this.portLabel = new MetroFramework.Controls.MetroLabel();
            this.ps4IpTextBox = new MetroFramework.Controls.MetroTextBox();
            this.ipLabel = new MetroFramework.Controls.MetroLabel();
            this.statusGroupBox = new System.Windows.Forms.GroupBox();
            this.connectionStatusLabel = new MetroFramework.Controls.MetroLabel();
            this.staticStatusLabel = new MetroFramework.Controls.MetroLabel();
            this.dumpGroupBox = new System.Windows.Forms.GroupBox();
            this.dumpMemoryButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.styleManager)).BeginInit();
            this.connectionGroupBox.SuspendLayout();
            this.statusGroupBox.SuspendLayout();
            this.dumpGroupBox.SuspendLayout();
            this.SuspendLayout();
            //
            // styleManager
            //
            this.styleManager.Owner = this;
            this.styleManager.Style = MetroFramework.MetroColorStyle.Purple;
            this.styleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            //
            // connectionGroupBox
            //
            this.connectionGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.connectionGroupBox.Controls.Add(this.detectFirmwareButton);
            this.connectionGroupBox.Controls.Add(this.firmwareComboBox);
            this.connectionGroupBox.Controls.Add(this.firmwareLabel);
            this.connectionGroupBox.Controls.Add(this.attachBo3Button);
            this.connectionGroupBox.Controls.Add(this.connectPS4Button);
            this.connectionGroupBox.Controls.Add(this.ps4PortTextBox);
            this.connectionGroupBox.Controls.Add(this.portLabel);
            this.connectionGroupBox.Controls.Add(this.ps4IpTextBox);
            this.connectionGroupBox.Controls.Add(this.ipLabel);
            this.connectionGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(65)))), ((int)(((byte)(153)))));
            this.connectionGroupBox.Location = new System.Drawing.Point(24, 64);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new System.Drawing.Size(412, 150);
            this.connectionGroupBox.TabIndex = 0;
            this.connectionGroupBox.TabStop = false;
            this.connectionGroupBox.Text = "Connection";
            //
            // detectFirmwareButton
            //
            this.detectFirmwareButton.BackColor = System.Drawing.Color.Black;
            this.detectFirmwareButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.detectFirmwareButton.Location = new System.Drawing.Point(216, 96);
            this.detectFirmwareButton.Name = "detectFirmwareButton";
            this.detectFirmwareButton.Size = new System.Drawing.Size(120, 30);
            this.detectFirmwareButton.TabIndex = 7;
            this.detectFirmwareButton.Text = "Detect Firmware";
            this.detectFirmwareButton.UseVisualStyleBackColor = false;
            this.detectFirmwareButton.Click += new System.EventHandler(this.detectFirmwareButton_Click);
            //
            // firmwareComboBox
            //
            this.firmwareComboBox.FormattingEnabled = true;
            this.firmwareComboBox.ItemHeight = 23;
            this.firmwareComboBox.Items.AddRange(new object[] {
            "9.00",
            "9.05",
            "9.20",
            "9.40",
            "9.60",
            "10.00",
            "10.01",
            "10.20",
            "10.40",
            "10.60",
            "11.00",
            "11.20",
            "11.40",
            "11.60",
            "12.00",
            "12.02",
            "12.20",
            "12.40",
            "12.60",
            "12.70",
            "13.00",
            "13.20",
            "13.40",
            "13.42",
            "13.60"});
            this.firmwareComboBox.Location = new System.Drawing.Point(76, 98);
            this.firmwareComboBox.Name = "firmwareComboBox";
            this.firmwareComboBox.Size = new System.Drawing.Size(120, 29);
            this.firmwareComboBox.TabIndex = 6;
            this.firmwareComboBox.UseSelectable = true;
            //
            // firmwareLabel
            //
            this.firmwareLabel.AutoSize = true;
            this.firmwareLabel.Location = new System.Drawing.Point(6, 100);
            this.firmwareLabel.Name = "firmwareLabel";
            this.firmwareLabel.Size = new System.Drawing.Size(68, 19);
            this.firmwareLabel.TabIndex = 8;
            this.firmwareLabel.Text = "Firmware:";
            this.firmwareLabel.UseStyleColors = true;
            //
            // attachBo3Button
            //
            this.attachBo3Button.BackColor = System.Drawing.Color.Black;
            this.attachBo3Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.attachBo3Button.Location = new System.Drawing.Point(216, 60);
            this.attachBo3Button.Name = "attachBo3Button";
            this.attachBo3Button.Size = new System.Drawing.Size(120, 30);
            this.attachBo3Button.TabIndex = 5;
            this.attachBo3Button.Text = "Attach BO3";
            this.attachBo3Button.UseVisualStyleBackColor = false;
            this.attachBo3Button.Click += new System.EventHandler(this.attachBo3Button_Click);
            //
            // connectPS4Button
            //
            this.connectPS4Button.BackColor = System.Drawing.Color.Black;
            this.connectPS4Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectPS4Button.Location = new System.Drawing.Point(76, 60);
            this.connectPS4Button.Name = "connectPS4Button";
            this.connectPS4Button.Size = new System.Drawing.Size(120, 30);
            this.connectPS4Button.TabIndex = 4;
            this.connectPS4Button.Text = "Send Payload";
            this.connectPS4Button.UseVisualStyleBackColor = false;
            this.connectPS4Button.EnabledChanged += new System.EventHandler(this.connectPS4Button_EnabledChanged);
            this.connectPS4Button.Click += new System.EventHandler(this.connectPS4Button_Click);
            //
            // ps4PortTextBox
            //
            //
            //
            //
            this.ps4PortTextBox.CustomButton.Image = null;
            this.ps4PortTextBox.CustomButton.Location = new System.Drawing.Point(40, 1);
            this.ps4PortTextBox.CustomButton.Name = "";
            this.ps4PortTextBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.ps4PortTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.ps4PortTextBox.CustomButton.TabIndex = 1;
            this.ps4PortTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.ps4PortTextBox.CustomButton.UseSelectable = true;
            this.ps4PortTextBox.CustomButton.Visible = false;
            this.ps4PortTextBox.Lines = new string[0];
            this.ps4PortTextBox.Location = new System.Drawing.Point(344, 19);
            this.ps4PortTextBox.MaxLength = 32767;
            this.ps4PortTextBox.Name = "ps4PortTextBox";
            this.ps4PortTextBox.PasswordChar = '\0';
            this.ps4PortTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ps4PortTextBox.SelectedText = "";
            this.ps4PortTextBox.SelectionLength = 0;
            this.ps4PortTextBox.SelectionStart = 0;
            this.ps4PortTextBox.ShortcutsEnabled = true;
            this.ps4PortTextBox.Size = new System.Drawing.Size(62, 23);
            this.ps4PortTextBox.TabIndex = 3;
            this.ps4PortTextBox.UseSelectable = true;
            this.ps4PortTextBox.UseStyleColors = true;
            this.ps4PortTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.ps4PortTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            //
            // portLabel
            //
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(301, 21);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(37, 19);
            this.portLabel.TabIndex = 2;
            this.portLabel.Text = "Port:";
            this.portLabel.UseStyleColors = true;
            //
            // ps4IpTextBox
            //
            //
            //
            //
            this.ps4IpTextBox.CustomButton.Image = null;
            this.ps4IpTextBox.CustomButton.Location = new System.Drawing.Point(238, 1);
            this.ps4IpTextBox.CustomButton.Name = "";
            this.ps4IpTextBox.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.ps4IpTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.ps4IpTextBox.CustomButton.TabIndex = 1;
            this.ps4IpTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.ps4IpTextBox.CustomButton.UseSelectable = true;
            this.ps4IpTextBox.CustomButton.Visible = false;
            this.ps4IpTextBox.Lines = new string[0];
            this.ps4IpTextBox.Location = new System.Drawing.Point(35, 19);
            this.ps4IpTextBox.MaxLength = 32767;
            this.ps4IpTextBox.Name = "ps4IpTextBox";
            this.ps4IpTextBox.PasswordChar = '\0';
            this.ps4IpTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ps4IpTextBox.SelectedText = "";
            this.ps4IpTextBox.SelectionLength = 0;
            this.ps4IpTextBox.SelectionStart = 0;
            this.ps4IpTextBox.ShortcutsEnabled = true;
            this.ps4IpTextBox.Size = new System.Drawing.Size(260, 23);
            this.ps4IpTextBox.TabIndex = 1;
            this.ps4IpTextBox.UseSelectable = true;
            this.ps4IpTextBox.UseStyleColors = true;
            this.ps4IpTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.ps4IpTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            //
            // ipLabel
            //
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(6, 21);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(23, 19);
            this.ipLabel.TabIndex = 0;
            this.ipLabel.Text = "IP:";
            this.ipLabel.UseStyleColors = true;
            //
            // statusGroupBox
            //
            this.statusGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.statusGroupBox.Controls.Add(this.connectionStatusLabel);
            this.statusGroupBox.Controls.Add(this.staticStatusLabel);
            this.statusGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(65)))), ((int)(((byte)(153)))));
            this.statusGroupBox.Location = new System.Drawing.Point(24, 223);
            this.statusGroupBox.Name = "statusGroupBox";
            this.statusGroupBox.Size = new System.Drawing.Size(412, 56);
            this.statusGroupBox.TabIndex = 1;
            this.statusGroupBox.TabStop = false;
            this.statusGroupBox.Text = "Status";
            //
            // connectionStatusLabel
            //
            this.connectionStatusLabel.AutoSize = true;
            this.connectionStatusLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.connectionStatusLabel.ForeColor = System.Drawing.Color.Red;
            this.connectionStatusLabel.Location = new System.Drawing.Point(70, 19);
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(127, 25);
            this.connectionStatusLabel.TabIndex = 1;
            this.connectionStatusLabel.Text = "Not Connected";
            this.connectionStatusLabel.UseCustomForeColor = true;
            this.connectionStatusLabel.UseStyleColors = true;
            //
            // staticStatusLabel
            //
            this.staticStatusLabel.AutoSize = true;
            this.staticStatusLabel.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.staticStatusLabel.Location = new System.Drawing.Point(6, 19);
            this.staticStatusLabel.Name = "staticStatusLabel";
            this.staticStatusLabel.Size = new System.Drawing.Size(61, 25);
            this.staticStatusLabel.TabIndex = 0;
            this.staticStatusLabel.Text = "Status:";
            this.staticStatusLabel.UseStyleColors = true;
            //
            // dumpGroupBox
            //
            this.dumpGroupBox.BackColor = System.Drawing.Color.Transparent;
            this.dumpGroupBox.Controls.Add(this.dumpMemoryButton);
            this.dumpGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(65)))), ((int)(((byte)(153)))));
            this.dumpGroupBox.Location = new System.Drawing.Point(24, 288);
            this.dumpGroupBox.Name = "dumpGroupBox";
            this.dumpGroupBox.Size = new System.Drawing.Size(412, 90);
            this.dumpGroupBox.TabIndex = 2;
            this.dumpGroupBox.TabStop = false;
            this.dumpGroupBox.Text = "Memory Dumper";
            //
            // dumpMemoryButton
            //
            this.dumpMemoryButton.BackColor = System.Drawing.Color.Black;
            this.dumpMemoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpMemoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dumpMemoryButton.ForeColor = System.Drawing.Color.White;
            this.dumpMemoryButton.Location = new System.Drawing.Point(118, 25);
            this.dumpMemoryButton.Name = "dumpMemoryButton";
            this.dumpMemoryButton.Size = new System.Drawing.Size(175, 38);
            this.dumpMemoryButton.TabIndex = 0;
            this.dumpMemoryButton.Text = "Dump BO3 Memory";
            this.dumpMemoryButton.UseVisualStyleBackColor = false;
            this.dumpMemoryButton.Click += new System.EventHandler(this.DumpMemoryButton_Click);
            //
            // MainWindow
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 398);
            this.Controls.Add(this.dumpGroupBox);
            this.Controls.Add(this.statusGroupBox);
            this.Controls.Add(this.connectionGroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Purple;
            this.Text = "PS4 BO3 Memory Dumper";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.styleManager)).EndInit();
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            this.statusGroupBox.ResumeLayout(false);
            this.statusGroupBox.PerformLayout();
            this.dumpGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MetroFramework.Components.MetroStyleManager styleManager;
        private System.Windows.Forms.GroupBox connectionGroupBox;
        private System.Windows.Forms.Button detectFirmwareButton;
        private MetroFramework.Controls.MetroComboBox firmwareComboBox;
        private MetroFramework.Controls.MetroLabel firmwareLabel;
        private System.Windows.Forms.Button attachBo3Button;
        private System.Windows.Forms.Button connectPS4Button;
        private MetroFramework.Controls.MetroTextBox ps4PortTextBox;
        private MetroFramework.Controls.MetroLabel portLabel;
        private MetroFramework.Controls.MetroTextBox ps4IpTextBox;
        private MetroFramework.Controls.MetroLabel ipLabel;
        private System.Windows.Forms.GroupBox statusGroupBox;
        private MetroFramework.Controls.MetroLabel connectionStatusLabel;
        private MetroFramework.Controls.MetroLabel staticStatusLabel;
        private System.Windows.Forms.GroupBox dumpGroupBox;
        private System.Windows.Forms.Button dumpMemoryButton;
    }
}
