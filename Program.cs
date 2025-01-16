using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
    @Author: Bartosz Faruga
    @Title: Pixelization of an image
    @version: 1.0

    @description: This solution contains a Windows Forms application for loading and processing images.
    It includes multiple libraries (C++, ASM) for pixelization, with SIMD optimizations, and displays
    histograms of the processed images, showing the distribution of pixel values. 
    The application allows the user to select the pixelization method and the number of threads to use for processing.
    Calculations are performed in parallel using the selected number of threads.
    Program tracks time taken to process the image and displays the time taken to process the image.
 */

namespace JA_Pixelizacja_Obrazu
{
    /// <summary>
    /// Main class for the image processing application
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ImageProcessorForm());
        }
    }
}
