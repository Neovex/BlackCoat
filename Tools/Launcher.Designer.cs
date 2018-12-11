namespace BlackCoat
{
    partial class Launcher
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this._TapHost = new System.Windows.Forms.TabControl();
            this._EngineTabPage = new System.Windows.Forms.TabPage();
            this._FPSComboBox = new System.Windows.Forms.ComboBox();
            this._VsyncCheckBox = new System.Windows.Forms.CheckBox();
            this._BorderCheckBox = new System.Windows.Forms.CheckBox();
            this._AAComboBox = new System.Windows.Forms.ComboBox();
            this._WindowedCheckBox = new System.Windows.Forms.CheckBox();
            this._ResolutionComboBox = new System.Windows.Forms.ComboBox();
            this._ExitButton = new System.Windows.Forms.Button();
            this._StartButton = new System.Windows.Forms.Button();
            this._Banner = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this._TapHost.SuspendLayout();
            this._EngineTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._Banner)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(98, 54);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(57, 13);
            label1.TabIndex = 1;
            label1.Text = "Resolution";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(104, 105);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(51, 13);
            label2.TabIndex = 3;
            label2.Text = "FPS Limit";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(91, 78);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(64, 13);
            label3.TabIndex = 4;
            label3.Text = "Anti Aliasing";
            // 
            // _TapHost
            // 
            this._TapHost.Controls.Add(this._EngineTabPage);
            this._TapHost.Location = new System.Drawing.Point(12, 118);
            this._TapHost.Name = "_TapHost";
            this._TapHost.SelectedIndex = 0;
            this._TapHost.Size = new System.Drawing.Size(510, 203);
            this._TapHost.TabIndex = 1;
            // 
            // _EngineTabPage
            // 
            this._EngineTabPage.Controls.Add(this._FPSComboBox);
            this._EngineTabPage.Controls.Add(this._VsyncCheckBox);
            this._EngineTabPage.Controls.Add(this._BorderCheckBox);
            this._EngineTabPage.Controls.Add(this._AAComboBox);
            this._EngineTabPage.Controls.Add(label3);
            this._EngineTabPage.Controls.Add(label2);
            this._EngineTabPage.Controls.Add(this._WindowedCheckBox);
            this._EngineTabPage.Controls.Add(label1);
            this._EngineTabPage.Controls.Add(this._ResolutionComboBox);
            this._EngineTabPage.Location = new System.Drawing.Point(4, 22);
            this._EngineTabPage.Name = "_EngineTabPage";
            this._EngineTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._EngineTabPage.Size = new System.Drawing.Size(502, 177);
            this._EngineTabPage.TabIndex = 0;
            this._EngineTabPage.Text = "Graphics";
            // 
            // _FPSComboBox
            // 
            this._FPSComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._FPSComboBox.FormattingEnabled = true;
            this._FPSComboBox.Items.AddRange(new object[] {
            "50",
            "60",
            "120",
            "Unlimited"});
            this._FPSComboBox.Location = new System.Drawing.Point(171, 102);
            this._FPSComboBox.Name = "_FPSComboBox";
            this._FPSComboBox.Size = new System.Drawing.Size(142, 21);
            this._FPSComboBox.TabIndex = 7;
            // 
            // _VsyncCheckBox
            // 
            this._VsyncCheckBox.AutoSize = true;
            this._VsyncCheckBox.Location = new System.Drawing.Point(330, 104);
            this._VsyncCheckBox.Name = "_VsyncCheckBox";
            this._VsyncCheckBox.Size = new System.Drawing.Size(88, 17);
            this._VsyncCheckBox.TabIndex = 6;
            this._VsyncCheckBox.Text = "Vertical Sync";
            this._VsyncCheckBox.UseVisualStyleBackColor = true;
            this._VsyncCheckBox.CheckedChanged += new System.EventHandler(this.VsyncCheckBox_CheckedChanged);
            // 
            // _BorderCheckBox
            // 
            this._BorderCheckBox.AutoSize = true;
            this._BorderCheckBox.Location = new System.Drawing.Point(330, 77);
            this._BorderCheckBox.Name = "_BorderCheckBox";
            this._BorderCheckBox.Size = new System.Drawing.Size(75, 17);
            this._BorderCheckBox.TabIndex = 6;
            this._BorderCheckBox.Text = "Borderless";
            this._BorderCheckBox.UseVisualStyleBackColor = true;
            // 
            // _AAComboBox
            // 
            this._AAComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._AAComboBox.FormattingEnabled = true;
            this._AAComboBox.Items.AddRange(new object[] {
            "Off",
            "1",
            "2",
            "4",
            "8",
            "16",
            "32"});
            this._AAComboBox.Location = new System.Drawing.Point(171, 75);
            this._AAComboBox.Name = "_AAComboBox";
            this._AAComboBox.Size = new System.Drawing.Size(142, 21);
            this._AAComboBox.TabIndex = 5;
            // 
            // _WindowedCheckBox
            // 
            this._WindowedCheckBox.AutoSize = true;
            this._WindowedCheckBox.Location = new System.Drawing.Point(330, 50);
            this._WindowedCheckBox.Name = "_WindowedCheckBox";
            this._WindowedCheckBox.Size = new System.Drawing.Size(77, 17);
            this._WindowedCheckBox.TabIndex = 2;
            this._WindowedCheckBox.Text = "Windowed";
            this._WindowedCheckBox.UseVisualStyleBackColor = true;
            this._WindowedCheckBox.CheckedChanged += new System.EventHandler(this.WindowedCheckBox_CheckedChanged);
            // 
            // _ResolutionComboBox
            // 
            this._ResolutionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._ResolutionComboBox.FormattingEnabled = true;
            this._ResolutionComboBox.Location = new System.Drawing.Point(171, 48);
            this._ResolutionComboBox.Name = "_ResolutionComboBox";
            this._ResolutionComboBox.Size = new System.Drawing.Size(142, 21);
            this._ResolutionComboBox.TabIndex = 0;
            // 
            // _ExitButton
            // 
            this._ExitButton.Location = new System.Drawing.Point(447, 327);
            this._ExitButton.Name = "_ExitButton";
            this._ExitButton.Size = new System.Drawing.Size(75, 23);
            this._ExitButton.TabIndex = 2;
            this._ExitButton.Text = "Exit";
            this._ExitButton.UseVisualStyleBackColor = true;
            this._ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // _StartButton
            // 
            this._StartButton.Location = new System.Drawing.Point(366, 327);
            this._StartButton.Name = "_StartButton";
            this._StartButton.Size = new System.Drawing.Size(75, 23);
            this._StartButton.TabIndex = 2;
            this._StartButton.Text = "Start";
            this._StartButton.UseVisualStyleBackColor = true;
            this._StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // _Banner
            // 
            this._Banner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this._Banner.Image = global::BlackCoat.Properties.Resources.Banner;
            this._Banner.Location = new System.Drawing.Point(12, 12);
            this._Banner.Name = "_Banner";
            this._Banner.Size = new System.Drawing.Size(510, 100);
            this._Banner.TabIndex = 0;
            this._Banner.TabStop = false;
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 362);
            this.Controls.Add(this._StartButton);
            this.Controls.Add(this._ExitButton);
            this.Controls.Add(this._TapHost);
            this.Controls.Add(this._Banner);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Launcher";
            this.ShowIcon = false;
            this.Text = "Black Coat Game";
            this._TapHost.ResumeLayout(false);
            this._EngineTabPage.ResumeLayout(false);
            this._EngineTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._Banner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _Banner;
        private System.Windows.Forms.TabControl _TapHost;
        private System.Windows.Forms.TabPage _EngineTabPage;
        private System.Windows.Forms.ComboBox _ResolutionComboBox;
        private System.Windows.Forms.Button _ExitButton;
        private System.Windows.Forms.Button _StartButton;
        private System.Windows.Forms.CheckBox _WindowedCheckBox;
        private System.Windows.Forms.ComboBox _AAComboBox;
        private System.Windows.Forms.ComboBox _FPSComboBox;
        private System.Windows.Forms.CheckBox _BorderCheckBox;
        private System.Windows.Forms.CheckBox _VsyncCheckBox;
    }
}