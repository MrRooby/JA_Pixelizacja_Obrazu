using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JA_Pixelizacja_Obrazu
{
    /// <summary>
    /// Form for the image processing application
    /// </summary>
    public partial class ImageProcessorForm : Form
    {
        public ImageProcessorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Browse for an image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseFiles_ButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathTextBox.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Change the value of the pixel number picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Threads_TrackBarScroll(object sender, EventArgs e)
        {
            threadValueLabel.Text = threadsTrackBar.Value.ToString(); // Display the value selected on the trackbar
        }


        /// <summary>
        /// Process the image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Process_ButtonClick(object sender, EventArgs e)
        {
            // Load the image
            ImageProcessing imageProcessing = new ImageProcessing();

            //Check if a file path is provided
            if (string.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show("Please select an image file");
                return;
            }

            try
            {
                imageProcessing.LoadImage(filePathTextBox.Text);

                // Choose the processing library
                imageProcessing.ChooseProcessingLibrary(libraryPicker.Text.ToString());

                Bitmap processedImage = imageProcessing.ProcessImage(Int32.Parse(pixelNumPicker.Text), threadsTrackBar.Value);

                // Display the processed image
                pictureBoxProcessed.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBoxProcessed.Image = processedImage;

                // Save the processed image to a file
                string saveFilePath = "C:/Users/barte/OneDrive/Pulpit/processed/processed_image.jpg";
                DialogResult result = MessageBox.Show("Do you want to save the processed image?", "Save Image", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    processedImage.Save(saveFilePath, ImageFormat.Jpeg);
                    MessageBox.Show($"Image saved to {saveFilePath}");
                }
                else
                {
                    MessageBox.Show("Image not saved");
                }
                pictureBoxProcessed.Image = processedImage;
            }
            catch (Exception ex)
            {
                if(checkBox1.Checked)
                {
                    MessageBox.Show($"Error: {ex.ToString()}");
                }
                else
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            finally
            {
                // Display the elapsed time
                MessageBox.Show($"Processing time: {imageProcessing.elapsedMilliseconds} ms");
            }
        }
    }
}
