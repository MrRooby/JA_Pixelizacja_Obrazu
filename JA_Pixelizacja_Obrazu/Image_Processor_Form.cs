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
    public partial class Image_Processor_Form : Form
    {
        public Image_Processor_Form()
        {
            InitializeComponent();
        }


        private void processButton_Click(object sender, EventArgs e)
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
                imageProcessing.chooseProcessingLibrary(libraryPicker.Text.ToString());
                
                Bitmap processedImage = imageProcessing.processImage(Int32.Parse(pixelNumPicker.Text));          

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
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // Display the elapsed time
                MessageBox.Show($"Processing time: {imageProcessing.elapsedMilliseconds} ms");
            }
        }

        private void browseFilesButton_Click(object sender, EventArgs e)
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

        private void threadsTrackBar_Scroll(object sender, EventArgs e)
        {
            threadValueLabel.Text = threadsTrackBar.Value.ToString(); // Display the value selected on the trackbar
        }
    }
}
