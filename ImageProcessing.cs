﻿using System;
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

        public Bitmap imageBitmap;

        public static PixelizeLibraryDelegate processingLibrary;
        public long? elapsedMilliseconds = null;

        /// <summary>
        /// Sets the processing library to be used for pixelizing the image.
        /// </summary>
        /// <param name="choice"> ASM, C++ or ASM SIMD </param>
        /// 
        public void ChooseProcessingLibrary(String choice)
        {
            if (choice == "C++")
            {
                processingLibrary = CPPLibrary.PixelizeImage;
            }
            else if (choice == "ASM SIMD")
            {
                processingLibrary = ASMLibrary.PixelizeImage;
            }
            else if (choice == "ASM")
            {
                processingLibrary = ASM_NonVectorLibrary.PixelizeImage;
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
        /// Calculates the area for the image so the pixel count
        /// is divisible by the pixel size and thread count.
        /// </summary>
        /// <param name="originalBitmap"> image for which to calculate the area</param>
        /// <param name="pixelSize"> how many pixel will be averaged</param>
        /// <param name="threadCount"> count of threads the operation will be performed</param>
        /// <param name="squarePixels"> if true, the image will be cropped to consist of only square pixels</param>
        /// <returns> Rectangle divisible by pixelSize and threadCount</returns>
        Rectangle AreaForProcessing(Bitmap originalBitmap, int pixelSize, bool squarePixels)
        {
            int width = originalBitmap.Width;
            int height = originalBitmap.Height;

            int newWidth;
            int newHeight;

            // Calculate the new width and height
            if (squarePixels)
            {
                newWidth = width - (width % pixelSize);
                newHeight = height - (height % pixelSize);
            }
            else
            {
                newWidth = width - (width % 4);
                newHeight = height - (height % 4);
            }

            return new Rectangle(0, 0, newWidth, newHeight);
        }

        /// <summary>
        /// Cropps the image according to the specified area.
        /// </summary>
        /// <param name="cropArea"> size of the returned image</param>
        /// <returns> Cropped bitmap image </returns>
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
        /// <param name="pixelSize"> size of area to average, must be at least 4</param>
        /// <param name="threadCount"> number of threads operation will be performed on, must be at least 1</param>
        /// <param name="squarePixels"> if true, the image will be cropped to consist of only square pixels</param>
        /// <returns> Processed image </returns>
        /// <exception cref="ArgumentException"> Thrown if <paramref name="pixelSize"/> is less than 4 or <paramref name="threadCount"/> is less than 1</exception>
        public Bitmap ProcessImage(int pixelSize, int threadCount, bool squarePixels)
        {
            if (threadCount <= 0)
                throw new ArgumentException("Number of threads must be greater than 0");
            if (pixelSize <= 1)
                throw new ArgumentException("Pixel size must be greater than 0");
            
            // Stopwatch for measuring the time of processing
            Stopwatch stopwatch = new Stopwatch();

            // Transform image size for processing (must be divisible by 4 because asm library takes in 4 pixels at a time)
            Rectangle cropArea = AreaForProcessing(imageBitmap, pixelSize, squarePixels);

            // Create a copy of the loaded image. MUST BE 32bit RGBA format
            Bitmap bitmap = new Bitmap(CroppedBitmapForProcessing(cropArea));
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb); 

            int width = bitmap.Width;
            int height = bitmap.Height;
            int stride = bmpData.Stride;

            // Calculate the number of bytes required and copy pixel data
            int bytes = Math.Abs(stride) * height;
            // Allocate arrays for the image data
            byte[] data = new byte[bytes];
            Marshal.Copy(bmpData.Scan0, data, 0, bytes);

            // Create threads for processing
            Thread[] threads = new Thread[threadCount];

            // Calculate the number of blocks and the remaining pixels
            int numOfBlocks = height / pixelSize;
            float leftOver = height % pixelSize;

            // Calculate the number of rows per thread
            int rowsPerThread = (height / threadCount / pixelSize) * pixelSize;
            if (rowsPerThread <= 0)
                throw new ArgumentException("Number of threads is too high for the image size");
            int remainingRows = height - (rowsPerThread * (threadCount - 1));

            // Stopwatch started before the entire process consisting of both the image processing and the thread creation
            stopwatch.Start();

            for (int t = 0; t < threadCount; t++)
            {
                // Calculate the start and end row for the thread
                int startRow = t * rowsPerThread;
                int endRow = (t == threadCount - 1) ? height : startRow + rowsPerThread;

                threads[t] = new Thread(() =>
                {
                    int heightForThread = endRow - startRow;

                    // Overlap for neighboring rows
                    int localStart = startRow;
                    int localEnd = endRow;
                    int localHeight = localEnd - localStart;

                    // Create a local copy of the image data
                    byte[] image = new byte[localHeight * stride];
                    Buffer.BlockCopy(data, localStart * stride, image, 0, localHeight * stride);

                    // Process the part of the image
                    processingLibrary(image, width, localHeight, pixelSize);

                    // Copy the processed image back to the data array
                    Buffer.BlockCopy(image, 0, data, localStart * stride, localHeight * stride);
                });

                // Start the thread
                threads[t].Start();
            }

            // Wait for all threads to finish
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Stopwatch stopped after the entire process consisting of both the image processing and the thread creation
            stopwatch.Stop();
            elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Copy the processed data back to the bitmap
            Marshal.Copy(data, 0, bmpData.Scan0, bytes);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
    }
}
