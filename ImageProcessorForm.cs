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

            // Disable the maximize button and disable resizing
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            threadsTrackBar.Value = Environment.ProcessorCount;         // Set the default value of the threads trackbar to the number of logical processors
            threadValueLabel.Text = threadsTrackBar.Value.ToString();   // Display the value selected on the trackbar
            
            // Set the form icon
            this.Icon = new Icon("Resources/icon.ico");
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
                PictureBoxOriginal.SizeMode = PictureBoxSizeMode.Zoom;
                PictureBoxOriginal.Image = Image.FromFile(openFileDialog.FileName);

                Histogram histogram = new Histogram();
                histogram.GetHistogram(LoadingLabelOriginal, Image.FromFile(openFileDialog.FileName), this, PictureBoxHistogramOriginal);

                // Clear previous processed image
                PictureBoxProcessed.Image = null;
                PictureBoxHistogramProcessed.Image = null;
            }
        }

        /// <summary>
        /// Change the value of the pixel number picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Threads_TrackBarScroll(object sender, EventArgs e)
        {
            threadValueLabel.Text = threadsTrackBar.Value.ToString(); // Update the label with the value selected on the trackbar
        }

        /// <summary>
        /// Start the image processing with the selected parameters (pixel number, threads, library, crop)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Process_ButtonClick(object sender, EventArgs e)
        {
            // Load the image
            ImageProcessing imageProcessing = new ImageProcessing();
            Bitmap processedImage = null;

            //Check if a file path is provided
            if (string.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show("Please select an image file");
                return;
            }

            // Check if a processing library is selected
            if (string.IsNullOrEmpty(libraryPicker.Text))
            {
                MessageBox.Show("Please select a processing library");
                return;
            }

            try
            {
                // Process the image
                imageProcessing.LoadImage(filePathTextBox.Text);
                imageProcessing.ChooseProcessingLibrary(libraryPicker.Text.ToString());
                processedImage = imageProcessing.ProcessImage(Int32.Parse(pixelNumPicker.Text), threadsTrackBar.Value, CropImageCheckbox.Checked);

                // Display the processed image in the PictureBox
                PictureBoxProcessed.SizeMode = PictureBoxSizeMode.Zoom;
                PictureBoxProcessed.Image = processedImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                if (processedImage != null)
                {
                    // Display the histogram for the processed image
                    // This operation is done first because it takes the longest (time depends on the image size)
                    Histogram histogram = new Histogram();
                    histogram.GetHistogram(LoadingLabelProcessed, processedImage, this, PictureBoxHistogramProcessed);

                    // Display the elapsed time
                    MessageBox.Show($"Processing time: {imageProcessing.elapsedMilliseconds} ms");

                    // Display a message box asking if the user wants to save the processed image
                    DialogResult result = MessageBox.Show("Do you want to save the processed image?", "Save Image", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Title = "Save processed image";
                        saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                processedImage.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                                MessageBox.Show("Image saved successfully");
                            }
                            catch
                            {
                                MessageBox.Show($"Did not save! Cannot override the original image!");
                            }
                        }
                    }
                }
            }
        }
    }
}
