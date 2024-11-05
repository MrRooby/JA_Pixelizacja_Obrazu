namespace JA_Pixelizacja_Obrazu
{
    partial class Form1
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
            this.pixelNumSpecifier = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.libraryPicker = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // processButton
            // 
            this.processButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.processButton.Location = new System.Drawing.Point(995, 523);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(127, 53);
            this.processButton.TabIndex = 0;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += new System.EventHandler(this.processButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label1.Location = new System.Drawing.Point(18, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(276, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose image you want to process:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Location = new System.Drawing.Point(329, 19);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.Size = new System.Drawing.Size(558, 22);
            this.filePathTextBox.TabIndex = 2;
            // 
            // browseFilesButton
            // 
            this.browseFilesButton.Location = new System.Drawing.Point(893, 12);
            this.browseFilesButton.Name = "browseFilesButton";
            this.browseFilesButton.Size = new System.Drawing.Size(115, 37);
            this.browseFilesButton.TabIndex = 3;
            this.browseFilesButton.Text = "Browse";
            this.browseFilesButton.UseVisualStyleBackColor = true;
            this.browseFilesButton.Click += new System.EventHandler(this.browseFilesButton_Click);
            // 
            // pixelNumSpecifier
            // 
            this.pixelNumSpecifier.FormattingEnabled = true;
            this.pixelNumSpecifier.Items.AddRange(new object[] {
            "2",
            "4",
            "8",
            "16",
            "32",
            "64"});
            this.pixelNumSpecifier.Location = new System.Drawing.Point(338, 95);
            this.pixelNumSpecifier.Name = "pixelNumSpecifier";
            this.pixelNumSpecifier.Size = new System.Drawing.Size(68, 24);
            this.pixelNumSpecifier.TabIndex = 4;
            this.pixelNumSpecifier.SelectedIndexChanged += new System.EventHandler(this.pixelNumSpecifier_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(19, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(280, 40);
            this.label2.TabIndex = 5;
            this.label2.Text = "Choose pixelization strength \r\n(higher number -> more pixelization)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(432, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Choose library";
            // 
            // libraryPicker
            // 
            this.libraryPicker.FormattingEnabled = true;
            this.libraryPicker.Items.AddRange(new object[] {
            "ASM",
            "C#"});
            this.libraryPicker.Location = new System.Drawing.Point(578, 95);
            this.libraryPicker.Name = "libraryPicker";
            this.libraryPicker.Size = new System.Drawing.Size(63, 24);
            this.libraryPicker.TabIndex = 7;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(22, 145);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(957, 431);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(1134, 588);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.libraryPicker);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pixelNumSpecifier);
            this.Controls.Add(this.browseFilesButton);
            this.Controls.Add(this.filePathTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.processButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.Button browseFilesButton;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ComboBox pixelNumSpecifier;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox libraryPicker;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

