using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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

            Rectangle cropArea = AreaForProcessing(imageBitmap, pixelSize, threadCount);

            // Create a copy of the loaded image
            Bitmap bitmap = new Bitmap(CroppedBitmapForProcessing(cropArea));
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            int width = bitmap.Width;
            int height = bitmap.Height;
            int stride = bmpData.Stride;

            // Calculate the number of bytes required and copy pixel data
            int bytes = Math.Abs(stride) * height;
            // Allocate arrays for the image data
            byte[] data = new byte[bytes];
            Marshal.Copy(bmpData.Scan0, data, 0, bytes);

            Thread[] threads = new Thread[threadCount];

            int numOfBlocks = height / pixelSize;
            int rowPerThread = numOfBlocks / threadCount;

            stopwatch.Start();

            int currentRow = 0;

            for (int t = 0; t < threadCount; t++)
            {
                int startRow = currentRow;

                int endRow = (t == threadCount - 1) ? height : (startRow + rowPerThread);
                currentRow = endRow;

                threads[t] = new Thread(() =>
                {
                    int heightForThread = endRow - startRow;

                    // Overlap for neighboring rows
                    int localStart = Math.Max(0, startRow - 1);
                    int localEnd = Math.Min(height, endRow + 1);
                    int localHeight = localEnd - localStart;

                    byte[] image = new byte[localHeight * stride];
                    Buffer.BlockCopy(data, localStart * stride, image, 0, localHeight * stride);

                    processingLibrary(image, width, localHeight, pixelSize);

                    int outCopyOffset = (startRow - localStart) * stride;
                    int outCopySize = heightForThread * stride;

                    // Copy processed data from local output buffer back to main data array
                    Buffer.BlockCopy(image, outCopyOffset, data, startRow * stride, outCopySize);

                });
                // Start the thread
                threads[t].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();
            elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Marshal.Copy(data, 0, bmpData.Scan0, bytes);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
    }
}
