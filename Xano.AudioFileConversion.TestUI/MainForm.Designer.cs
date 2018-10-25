namespace Xano.AudioFileConversion.TestUI
{
    partial class MainForm
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
            this.txtInputDirectory = new System.Windows.Forms.TextBox();
            this.txtOutputDirectory = new System.Windows.Forms.TextBox();
            this.btnInputDirectory = new System.Windows.Forms.Button();
            this.btnOutputDirectory = new System.Windows.Forms.Button();
            this.btnStartConversion = new System.Windows.Forms.Button();
            this.txtSampleRate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReadSampleRate = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // txtInputDirectory
            // 
            this.txtInputDirectory.Location = new System.Drawing.Point(22, 27);
            this.txtInputDirectory.Name = "txtInputDirectory";
            this.txtInputDirectory.Size = new System.Drawing.Size(441, 20);
            this.txtInputDirectory.TabIndex = 0;
            // 
            // txtOutputDirectory
            // 
            this.txtOutputDirectory.Location = new System.Drawing.Point(22, 64);
            this.txtOutputDirectory.Name = "txtOutputDirectory";
            this.txtOutputDirectory.Size = new System.Drawing.Size(441, 20);
            this.txtOutputDirectory.TabIndex = 1;
            // 
            // btnInputDirectory
            // 
            this.btnInputDirectory.Location = new System.Drawing.Point(469, 27);
            this.btnInputDirectory.Name = "btnInputDirectory";
            this.btnInputDirectory.Size = new System.Drawing.Size(96, 23);
            this.btnInputDirectory.TabIndex = 2;
            this.btnInputDirectory.Text = "Input Directory";
            this.btnInputDirectory.UseVisualStyleBackColor = true;
            this.btnInputDirectory.Click += new System.EventHandler(this.btnInputDirectory_Click);
            // 
            // btnOutputDirectory
            // 
            this.btnOutputDirectory.Location = new System.Drawing.Point(469, 64);
            this.btnOutputDirectory.Name = "btnOutputDirectory";
            this.btnOutputDirectory.Size = new System.Drawing.Size(96, 23);
            this.btnOutputDirectory.TabIndex = 3;
            this.btnOutputDirectory.Text = "Output Directory";
            this.btnOutputDirectory.UseVisualStyleBackColor = true;
            this.btnOutputDirectory.Click += new System.EventHandler(this.btnOutputDirectory_Click);
            // 
            // btnStartConversion
            // 
            this.btnStartConversion.BackColor = System.Drawing.Color.Lime;
            this.btnStartConversion.Location = new System.Drawing.Point(213, 106);
            this.btnStartConversion.Name = "btnStartConversion";
            this.btnStartConversion.Size = new System.Drawing.Size(117, 35);
            this.btnStartConversion.TabIndex = 4;
            this.btnStartConversion.Text = "Start Conversion";
            this.btnStartConversion.UseVisualStyleBackColor = false;
            this.btnStartConversion.Click += new System.EventHandler(this.btnStartConversion_Click);
            // 
            // txtSampleRate
            // 
            this.txtSampleRate.Location = new System.Drawing.Point(103, 114);
            this.txtSampleRate.Name = "txtSampleRate";
            this.txtSampleRate.Size = new System.Drawing.Size(73, 20);
            this.txtSampleRate.TabIndex = 5;
            this.txtSampleRate.Text = "10000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Sample Rate";
            // 
            // btnReadSampleRate
            // 
            this.btnReadSampleRate.Location = new System.Drawing.Point(456, 111);
            this.btnReadSampleRate.Name = "btnReadSampleRate";
            this.btnReadSampleRate.Size = new System.Drawing.Size(109, 23);
            this.btnReadSampleRate.TabIndex = 7;
            this.btnReadSampleRate.Text = "Read Sample Rate";
            this.btnReadSampleRate.UseVisualStyleBackColor = true;
            this.btnReadSampleRate.Click += new System.EventHandler(this.btnReadSampleRate_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 271);
            this.Controls.Add(this.btnReadSampleRate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSampleRate);
            this.Controls.Add(this.btnStartConversion);
            this.Controls.Add(this.btnOutputDirectory);
            this.Controls.Add(this.btnInputDirectory);
            this.Controls.Add(this.txtOutputDirectory);
            this.Controls.Add(this.txtInputDirectory);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Audio File Conversion";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInputDirectory;
        private System.Windows.Forms.TextBox txtOutputDirectory;
        private System.Windows.Forms.Button btnInputDirectory;
        private System.Windows.Forms.Button btnOutputDirectory;
        private System.Windows.Forms.Button btnStartConversion;
        private System.Windows.Forms.TextBox txtSampleRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReadSampleRate;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

