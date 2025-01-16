using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JA_Pixelizacja_Obrazu
{
    /// <summary>
    /// Class for calculating and displaying the histogram of an image
    /// </summary>
    internal class Histogram
    {
        // Timer for the loading animation
        private Timer loadingTimer;
        private int loadingDots = 0;

        // Label for displaying the loading message
        private Label loadingLabel;

        public Histogram()
        {
            // Initialize the loading timer
            loadingTimer = new Timer();
            loadingTimer.Interval = 500; // Update every 500 milliseconds (add dot evry interval)
            loadingTimer.Tick += LoadingTimer_Tick;
        }

        /// <summary>
        /// Timer event handler for the loading animation
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">The event data</param>
        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            loadingDots = (loadingDots + 1) % 4;
            if (loadingLabel != null)
            {
                loadingLabel.Text = "Loading histogram" + new string('.', loadingDots);
            }
        }

        /// <summary>
        /// Calculate the histogram of an image
        /// </summary>
        /// <param name="image">The image bitmap for which the histogram is to be calculated</param>
        /// <returns>Returns a 2D array representing the histogram of the image</returns>
        public int[][] CalculateHistogram(Bitmap image)
        {
            int[][] histogram = new int[3][]; // 0: Red, 1: Green, 2: Blue
            // Initialize the histogram arrays
            histogram[0] = new int[256];
            histogram[1] = new int[256];
            histogram[2] = new int[256];

            // Iterate over all pixels in the image
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // For every pixel in the image, increment the corresponding histogram value
                    Color pixelColor = image.GetPixel(x, y);
                    histogram[0][pixelColor.R]++;
                    histogram[1][pixelColor.G]++;
                    histogram[2][pixelColor.B]++;
                }
            }

            return histogram;
        }

        /// <summary>
        /// Create a bitmap image of the histogram
        /// </summary>
        /// <param name="histogram">The 2D array representing the histogram of the image</param>
        /// <returns>Returns a bitmap image representing the histogram</returns>
        public Bitmap HistogramBitmap(int[][] histogram)
        {
            int width = 256;
            int height = 100;
            Bitmap histogramImage = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(histogramImage))
            {
                g.Clear(Color.White);

                // Find the maximum value in the histogram for scaling
                int max = histogram.Max(channel => channel.Max());

                for (int i = 0; i < 256; i++)
                {
                    int redHeight = (int)((histogram[0][i] / (float)max) * height);
                    int greenHeight = (int)((histogram[1][i] / (float)max) * height);
                    int blueHeight = (int)((histogram[2][i] / (float)max) * height);

                    g.DrawLine(Pens.Red, i, height, i, height - redHeight);
                    g.DrawLine(Pens.Green, i, height, i, height - greenHeight);
                    g.DrawLine(Pens.Blue, i, height, i, height - blueHeight);
                }
            }

            // Display the histogram in a PictureBox
            return histogramImage;
        }

        /// <summary>
        /// Get the histogram of an image asynchronously.
        /// Display it in a PictureBox with a loading animation
        /// </summary>
        /// <param name="loadingLabel">The label to display the loading message</param>
        /// <param name="image">The image for which the histogram is to be calculated</param>
        /// <param name="control">The control to invoke UI updates</param>
        /// <param name="pictureBox">The PictureBox to display the histogram</param>
        async public void GetHistogram(Label loadingLabel, Image image, Control control, PictureBox pictureBox)
        {
            this.loadingLabel = loadingLabel;

            // Display loading message
            loadingLabel.Text = "Loading histogram...";
            loadingLabel.Visible = true;

            // Start the loading animation
            loadingDots = 0;
            loadingTimer.Start();

            // Calculate and display the histogram asynchronously
            await Task.Run(() =>
            {
                int[][] histogramValues = CalculateHistogram(new Bitmap(image));
                Bitmap histogramImage = HistogramBitmap(histogramValues);

                // Update the UI on the main thread
                control.Invoke(new Action(() =>
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Image = histogramImage;
                    loadingLabel.Visible = false;

                    // Stop the loading animation
                    loadingTimer.Stop();
                }));
            });
        }
    }
}