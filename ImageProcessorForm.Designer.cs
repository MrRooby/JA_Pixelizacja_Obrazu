namespace JA_Pixelizacja_Obrazu
{
    partial class ImageProcessorForm
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
            this.processButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.browseFilesButton = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.pixelNumPicker = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.libraryPicker = new System.Windows.Forms.ComboBox();
            this.PictureBoxProcessed = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.threadsTrackBar = new System.Windows.Forms.TrackBar();
            this.threadValueLabel = new System.Windows.Forms.Label();
            this.CropImageCheckbox = new System.Windows.Forms.CheckBox();
            this.PictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.PictureBoxHistogramOriginal = new System.Windows.Forms.PictureBox();
            this.PictureBoxHistogramProcessed = new System.Windows.Forms.PictureBox();
            this.LoadingLabelOriginal = new System.Windows.Forms.Label();
            this.LoadingLabelProcessed = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxProcessed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadsTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHistogramOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHistogramProcessed)).BeginInit();
            this.SuspendLayout();
            // 
            // processButton
            // 
            this.processButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.processButton.Location = new System.Drawing.Point(400, 318);
            this.processButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(95, 43);
            this.processButton.TabIndex = 0;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += new System.EventHandler(this.Process_ButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(14, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(276, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose image you want to process:";
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Location = new System.Drawing.Point(258, 21);
            this.filePathTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.Size = new System.Drawing.Size(506, 20);
            this.filePathTextBox.TabIndex = 2;
            // 
            // browseFilesButton
            // 
            this.browseFilesButton.Location = new System.Drawing.Point(781, 15);
            this.browseFilesButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.browseFilesButton.Name = "browseFilesButton";
            this.browseFilesButton.Size = new System.Drawing.Size(86, 30);
            this.browseFilesButton.TabIndex = 3;
            this.browseFilesButton.Text = "Browse";
            this.browseFilesButton.UseVisualStyleBackColor = true;
            this.browseFilesButton.Click += new System.EventHandler(this.BrowseFiles_ButtonClick);
            // 
            // pixelNumPicker
            // 
            this.pixelNumPicker.FormattingEnabled = true;
            this.pixelNumPicker.Items.AddRange(new object[] {
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"});
            this.pixelNumPicker.Location = new System.Drawing.Point(258, 74);
            this.pixelNumPicker.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pixelNumPicker.Name = "pixelNumPicker";
            this.pixelNumPicker.Size = new System.Drawing.Size(52, 21);
            this.pixelNumPicker.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(14, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(280, 40);
            this.label2.TabIndex = 5;
            this.label2.Text = "Choose pixelization strength \r\n(higher number -> more pixelization)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(371, 74);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Library";
            // 
            // libraryPicker
            // 
            this.libraryPicker.FormattingEnabled = true;
            this.libraryPicker.Items.AddRange(new object[] {
            "ASM",
            "ASM SIMD",
            "C++"});
            this.libraryPicker.Location = new System.Drawing.Point(430, 74);
            this.libraryPicker.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.libraryPicker.Name = "libraryPicker";
            this.libraryPicker.Size = new System.Drawing.Size(94, 21);
            this.libraryPicker.TabIndex = 7;
            // 
            // PictureBoxProcessed
            // 
            this.PictureBoxProcessed.Location = new System.Drawing.Point(520, 160);
            this.PictureBoxProcessed.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PictureBoxProcessed.Name = "PictureBoxProcessed";
            this.PictureBoxProcessed.Size = new System.Drawing.Size(347, 350);
            this.PictureBoxProcessed.TabIndex = 8;
            this.PictureBoxProcessed.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label4.Location = new System.Drawing.Point(14, 117);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Threads";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 19);
            this.label5.TabIndex = 0;
            // 
            // threadsTrackBar
            // 
            this.threadsTrackBar.LargeChange = 8;
            this.threadsTrackBar.Location = new System.Drawing.Point(258, 110);
            this.threadsTrackBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.threadsTrackBar.Maximum = 64;
            this.threadsTrackBar.Minimum = 1;
            this.threadsTrackBar.Name = "threadsTrackBar";
            this.threadsTrackBar.Size = new System.Drawing.Size(471, 56);
            this.threadsTrackBar.TabIndex = 11;
            this.threadsTrackBar.Value = 1;
            this.threadsTrackBar.Scroll += new System.EventHandler(this.Threads_TrackBarScroll);
            // 
            // threadValueLabel
            // 
            this.threadValueLabel.AutoSize = true;
            this.threadValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.threadValueLabel.Location = new System.Drawing.Point(741, 110);
            this.threadValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.threadValueLabel.Name = "threadValueLabel";
            this.threadValueLabel.Size = new System.Drawing.Size(18, 20);
            this.threadValueLabel.TabIndex = 12;
            this.threadValueLabel.Text = "1";
            // 
            // CropImageCheckbox
            // 
            this.CropImageCheckbox.AutoSize = true;
            this.CropImageCheckbox.Checked = true;
            this.CropImageCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CropImageCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.CropImageCheckbox.Location = new System.Drawing.Point(595, 69);
            this.CropImageCheckbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CropImageCheckbox.Name = "CropImageCheckbox";
            this.CropImageCheckbox.Size = new System.Drawing.Size(334, 30);
            this.CropImageCheckbox.TabIndex = 14;
            this.CropImageCheckbox.Text = "Square Pixels (will crop image)";
            this.CropImageCheckbox.UseVisualStyleBackColor = true;
            // 
            // PictureBoxOriginal
            // 
            this.PictureBoxOriginal.Location = new System.Drawing.Point(30, 160);
            this.PictureBoxOriginal.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PictureBoxOriginal.Name = "PictureBoxOriginal";
            this.PictureBoxOriginal.Size = new System.Drawing.Size(347, 350);
            this.PictureBoxOriginal.TabIndex = 15;
            this.PictureBoxOriginal.TabStop = false;
            // 
            // PictureBoxHistogramOriginal
            // 
            this.PictureBoxHistogramOriginal.Location = new System.Drawing.Point(30, 548);
            this.PictureBoxHistogramOriginal.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PictureBoxHistogramOriginal.Name = "PictureBoxHistogramOriginal";
            this.PictureBoxHistogramOriginal.Size = new System.Drawing.Size(347, 151);
            this.PictureBoxHistogramOriginal.TabIndex = 16;
            this.PictureBoxHistogramOriginal.TabStop = false;
            // 
            // PictureBoxHistogramProcessed
            // 
            this.PictureBoxHistogramProcessed.Location = new System.Drawing.Point(520, 548);
            this.PictureBoxHistogramProcessed.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PictureBoxHistogramProcessed.Name = "PictureBoxHistogramProcessed";
            this.PictureBoxHistogramProcessed.Size = new System.Drawing.Size(347, 151);
            this.PictureBoxHistogramProcessed.TabIndex = 17;
            this.PictureBoxHistogramProcessed.TabStop = false;
            // 
            // LoadingLabelOriginal
            // 
            this.LoadingLabelOriginal.AutoSize = true;
            this.LoadingLabelOriginal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.LoadingLabelOriginal.Location = new System.Drawing.Point(107, 614);
            this.LoadingLabelOriginal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LoadingLabelOriginal.Name = "LoadingLabelOriginal";
            this.LoadingLabelOriginal.Size = new System.Drawing.Size(0, 29);
            this.LoadingLabelOriginal.TabIndex = 18;
            // 
            // LoadingLabelProcessed
            // 
            this.LoadingLabelProcessed.AutoSize = true;
            this.LoadingLabelProcessed.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.LoadingLabelProcessed.Location = new System.Drawing.Point(608, 614);
            this.LoadingLabelProcessed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LoadingLabelProcessed.Name = "LoadingLabelProcessed";
            this.LoadingLabelProcessed.Size = new System.Drawing.Size(0, 29);
            this.LoadingLabelProcessed.TabIndex = 19;
            // 
            // ImageProcessorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(904, 736);
            this.Controls.Add(this.LoadingLabelProcessed);
            this.Controls.Add(this.LoadingLabelOriginal);
            this.Controls.Add(this.PictureBoxOriginal);
            this.Controls.Add(this.PictureBoxHistogramOriginal);
            this.Controls.Add(this.PictureBoxHistogramProcessed);
            this.Controls.Add(this.CropImageCheckbox);
            this.Controls.Add(this.threadValueLabel);
            this.Controls.Add(this.threadsTrackBar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PictureBoxProcessed);
            this.Controls.Add(this.libraryPicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pixelNumPicker);
            this.Controls.Add(this.browseFilesButton);
            this.Controls.Add(this.filePathTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.processButton);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ImageProcessorForm";
            this.Text = "Image Processor";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxProcessed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadsTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHistogramOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxHistogramProcessed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.Button browseFilesButton;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ComboBox pixelNumPicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox libraryPicker;
        private System.Windows.Forms.PictureBox PictureBoxProcessed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar threadsTrackBar;
        private System.Windows.Forms.Label threadValueLabel;
        private System.Windows.Forms.CheckBox CropImageCheckbox;
        private System.Windows.Forms.PictureBox PictureBoxOriginal;
        private System.Windows.Forms.PictureBox PictureBoxHistogramOriginal;
        private System.Windows.Forms.PictureBox PictureBoxHistogramProcessed;
        private System.Windows.Forms.Label LoadingLabelOriginal;
        private System.Windows.Forms.Label LoadingLabelProcessed;
    }
}

