using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JA_Pixelizacja_Obrazu
{
    public partial class Image_Processor_Form : Form
    {
        private PixelizeLibraryDelegate libDelegate;

        public Image_Processor_Form()
        {
            InitializeComponent();
        }


        private void processButton_Click(object sender, EventArgs e)
        {
            libDelegate = CPPLibrary.sum;

            MessageBox.Show(libDelegate(1, 2).ToString());

            //Stopwatch stopwatch = new Stopwatch();

            ////Check if a file path is provided
            //if (string.IsNullOrEmpty(filePathTextBox.Text))
            //{
            //    MessageBox.Show("Please select an image file");
            //    return;
            //}

            //try
            //{
            //    // Load the original image
            //    Bitmap originalImage = new Bitmap(filePathTextBox.Text);

            //    // Get the pixel size from the picker
            //    int pixelSize = int.Parse(pixelNumPicker.Text);

            //    // Start the timer
            //    stopwatch.Start();
            //    // Call the pixelization method
            //    Bitmap pixelizedImage = ImageProcessing.PixelizeImage(originalImage, pixelSize);
            //    // Stop the timer
            //    stopwatch.Stop();

            //    // Display the result in the PictureBox
            //    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            //    pictureBox1.Image = pixelizedImage;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error: {ex.Message}");
            //}
            //finally
            //{

            //    // Display the elapsed time
            //    MessageBox.Show($"Processing time: {stopwatch.ElapsedMilliseconds} ms");
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void browseFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.InitialDirectory = @"C:\"; // You can set initial directory
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathTextBox.Text = openFileDialog.FileName;
            }

        }

        private void pixelNumSpecifier_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void threadsPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void threadsTrackBar_Scroll(object sender, EventArgs e)
        {
            threadValueLabel.Text = threadsTrackBar.Value.ToString(); // Display the value selected on the trackbar
        }
    }
}
