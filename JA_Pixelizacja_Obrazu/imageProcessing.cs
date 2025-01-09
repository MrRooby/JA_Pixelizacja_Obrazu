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
    public class ImageProcessing
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern unsafe byte* memcpy(byte* destination, byte* source, int count);
        
        private String fileInputPath;
        private String fileOutputPath;

        public Bitmap imageBitmap;

        public static PixelizeLibraryDelegate processingLibrary;
        public long? elapsedMilliseconds = null;

        public void chooseProcessingLibrary(String choice)
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

        public void LoadImage(String path)
        {
            fileInputPath = path;
            imageBitmap = new Bitmap(fileInputPath);
        }

        public IntPtr ConvertImageToPointer(Bitmap bitmap)
        {
            IntPtr imagePtr;
            BitmapData imageData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            return imagePtr = imageData.Scan0;
        }

        Rectangle AreaForProcessing(Bitmap originalBitmap, int pixelSize)
        {
            int xOverflow = originalBitmap.Width % pixelSize;
            int yOverflow = originalBitmap.Height % pixelSize;

            int x = originalBitmap.Width - xOverflow;
            int y = originalBitmap.Height - yOverflow;

            return new Rectangle(0, 0, x, y);
        }

        private unsafe Bitmap CroppedBitmapForProcessing(Rectangle cropArea)
        {
            // Check if the image is already cropped
            if ((imageBitmap.Width == cropArea.Width) && (imageBitmap.Height == cropArea.Height))
                return imageBitmap;
            // Che if the crop area is larger than the image
            else if ((imageBitmap.Width < cropArea.Width) || (imageBitmap.Height < cropArea.Height))
                throw new Exception("Crop area is larger than the image");

            // Load the origianl image to the memory
            BitmapData sourceBitmapData = imageBitmap.LockBits(new Rectangle(0, 0, imageBitmap.Width, imageBitmap.Height), ImageLockMode.ReadOnly, imageBitmap.PixelFormat);
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

        public Bitmap processImage(int pixelSize)
        {
            Stopwatch stopwatch = new Stopwatch();
            Rectangle cropArea = AreaForProcessing(imageBitmap, pixelSize);
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
                    IntPtr imagePtr = bitmapData.Scan0;                    
                    //int stride = bitmapData.Stride;
                    //MessageBox.Show("Stride: " + stride);

                    // Validate pixelSize
                    if (pixelSize <= 0)
                    {
                        throw new ArgumentException("Pixel size must be a positive integer.");
                    }

                    stopwatch.Start();

                    processingLibrary(imagePtr, formattedImage.Width, formattedImage.Height, pixelSize);
                    stopwatch.Stop();
                    elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                }
                finally
                {
                    // Ensure that we unlock the bits even if an exception occurs
                    formattedImage.UnlockBits(bitmapData);
                }

                // Optionally, you can clone the formattedImage to return a new Bitmap
                return (Bitmap)formattedImage.Clone();
            }
        }
    }
}
