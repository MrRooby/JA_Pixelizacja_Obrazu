using System;
using System.Collections.Generic;
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
    /// Class for processing images
    /// </summary>
    public class ImageProcessing
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern unsafe byte* memcpy(byte* destination, byte* source, int count);
        
        private String fileInputPath;
        private String fileOutputPath;

        public Bitmap imageBitmap;

        public static PixelizeLibraryDelegate processingLibrary;
        public long? elapsedMilliseconds = null;

        /// <summary>
        /// Sets the processing library to be used for pixelizing the image.
        /// </summary>
        /// <param name="choice"> ASM or C++  </param>
        /// 
        public void ChooseProcessingLibrary(String choice)
        {
            if (choice == "C++")
            {
                processingLibrary = CPPLibrary.PixelizeImage;
            }
            else if (choice == "ASM")
            {
                processingLibrary = ASMLibrary.PixelizeImage;
            }
        }

        /// <summary>
        /// Loads an image from the specified path for further processing.
        /// </summary>
        /// <param name="path"> file location </param>
        public void LoadImage(String path)
        {
            fileInputPath = path;
            imageBitmap = new Bitmap(fileInputPath);
        }

        /// <summary>
        /// Converts bitmap image to pointer.
        /// </summary>
        /// <param name="bitmap"> input image </param>
        /// <returns> Pointer to an image </returns>
        public IntPtr ConvertImageToPointer(Bitmap bitmap)
        {
            IntPtr imagePtr;
            BitmapData imageData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            return imagePtr = imageData.Scan0;
        }

        /// <summary>
        /// Calculates the greatest common divisor of two numbers.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns> GCD of two numbers </returns>
        private int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        /// <summary>
        /// Calculates the least common multiple of two numbers.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns> LCM of two numbers </returns>
        private int LCM(int a, int b)
        {
            return a * b / GCD(a, b);
        }

        /// <summary>
        /// Calculates the area for the image so the pixel count
        /// is divisible by the pixel size and thread count.
        /// </summary>
        /// <param name="originalBitmap"> image for which to calculate the area</param>
        /// <param name="pixelSize"> how many pixel will be averaged</param>
        /// <param name="threadCount"> count of threads the operation will be performed</param>
        /// <returns> Rectangle divisible by pixelSize and threadCount</returns>
        Rectangle AreaForProcessing(Bitmap originalBitmap, int pixelSize, int threadCount)
        {
            int width = originalBitmap.Width;
            int height = originalBitmap.Height;

            int lcm = LCM(pixelSize, threadCount);

            // Calculate the new width and height
            int newWidth = (width / lcm) * lcm; 
            int newHeight = (height / lcm) * lcm;

            return new Rectangle(0, 0, newWidth, newHeight);
        }

        /// <summary>
        /// Cropps the image according to the specified area.
        /// </summary>
        /// <param name="cropArea"> size of the returned image</param>
        /// <returns> cropped image </returns>
        /// <exception cref="Exception"> Thrown if <paramref name="cropArea"/> exceeds the current image size</exception>
        private unsafe Bitmap CroppedBitmapForProcessing(Rectangle cropArea)
        {
            // Check if the image is already cropped
            if ((imageBitmap.Width == cropArea.Width) && (imageBitmap.Height == cropArea.Height))
                return imageBitmap;

            // Check if the crop area is larger than the image
            else if ((imageBitmap.Width < cropArea.Width) || (imageBitmap.Height < cropArea.Height))
                throw new Exception("Crop area is larger than the image");

            // Load the origianl image to the memory
            BitmapData sourceBitmapData = imageBitmap.LockBits(
                new Rectangle(0, 0, imageBitmap.Width, imageBitmap.Height), 
                ImageLockMode.ReadOnly, 
                imageBitmap.PixelFormat);
            int bpp = sourceBitmapData.Stride / sourceBitmapData.Width; // 3 or 4
            var srcPtr = (byte*)sourceBitmapData.Scan0.ToPointer() + cropArea.Y * sourceBitmapData.Stride + cropArea.X * bpp;
            int srcStride = sourceBitmapData.Stride;

            // Create a new bitmap for the cropped image
            Bitmap croppedImage = new Bitmap(cropArea.Width, cropArea.Height, imageBitmap.PixelFormat);
            BitmapData destinationBitmapData = croppedImage.LockBits(
                new Rectangle(0, 0, croppedImage.Width, croppedImage.Height), 
                ImageLockMode.WriteOnly, 
                croppedImage.PixelFormat);
            var dstPtr = (byte*)destinationBitmapData.Scan0.ToPointer();
            int dstStride = destinationBitmapData.Stride;

            // Copy the image data to the new bitmap
            for (int y = 0; y < cropArea.Height; y++)
            {
                memcpy(dstPtr, srcPtr, dstStride);
                srcPtr += srcStride;
                dstPtr += dstStride;
            }

            imageBitmap.UnlockBits(sourceBitmapData);
            croppedImage.UnlockBits(destinationBitmapData);

            return croppedImage;
        }

        /// <summary>
        /// Processes the image using the specified pixel size and thread count.
        /// </summary>
        /// <param name="pixelSize"> size of area to average</param>
        /// <param name="threadCount"> number of threads operation will be performed on</param>
        /// <returns> Processed image </returns>
        /// <exception cref="ArgumentException"> 
        /// Throw if <paramref name="pixelSize"/> 
        /// or <paramref name="threadCount"/> 
        /// is less than 1</exception>
        public Bitmap ProcessImage(int pixelSize, int threadCount)
        {
            Stopwatch stopwatch = new Stopwatch();

            int totalPixels = imageBitmap.Width * imageBitmap.Height;

            if( threadCount > ( totalPixels / (pixelSize*pixelSize) ) )
            {
                throw new ArgumentException("Image to small for selected threads and pixelization");
            }

            Rectangle cropArea = AreaForProcessing(imageBitmap, pixelSize, threadCount);

            Bitmap croppedImage = CroppedBitmapForProcessing(cropArea);

            // Ensure the cropped image is in a compatible pixel format
            using (Bitmap formattedImage = croppedImage.Clone(
                new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
                PixelFormat.Format32bppArgb))
            {
                // Lock the bitmap's bits for read/write access
                Rectangle rect = new Rectangle(0, 0, formattedImage.Width, formattedImage.Height);
                BitmapData bitmapData = formattedImage.LockBits(
                    rect,
                    ImageLockMode.ReadWrite,
                    formattedImage.PixelFormat
                );

                try
                {
                    IntPtr basePtr = bitmapData.Scan0;
                    int stride = bitmapData.Stride;

                    int rowsPerThread = formattedImage.Height / threadCount;
                    List<Task> tasks = new List<Task>();
                    
                    stopwatch.Start();

                    for (int i = 0; i < threadCount; i++)
                    {
                        int startY = i * rowsPerThread;
                        int endY = (i == threadCount - 1) ? formattedImage.Height : (i + 1) * rowsPerThread;

                        tasks.Add(Task.Run(() =>
                        {
                            // Process sub-rectangle: (0, startY) to (width, endY)
                            IntPtr subPtr = basePtr + (startY * stride);
                            processingLibrary(subPtr, formattedImage.Width, endY - startY, pixelSize);
                        }));
                    }

                    Task.WaitAll(tasks.ToArray());
                }
                finally
                {
                    stopwatch.Stop();
                    elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    // Ensure that we unlock the bits even if an exception occurs
                    formattedImage.UnlockBits(bitmapData);
                }

                // Optionally, you can clone the formattedImage to return a new Bitmap
                return (Bitmap)formattedImage.Clone();
            }
        }
    }
}
