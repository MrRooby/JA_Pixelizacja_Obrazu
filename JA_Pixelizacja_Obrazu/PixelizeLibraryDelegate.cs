using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JA_Pixelizacja_Obrazu
{
    public delegate void PixelizeLibraryDelegate(
            IntPtr imageData,         // Pointer to the image pixel data
            int width,                // Width of the image in pixels
            int height,               // Height of the image in pixels
            int pixelSize);           // Size of the pixelization block (e.g., 10 for 10x10)

    public static class CPPLibrary
    {
        [DllImport(@"D:\SUT\JA\Projekt SEM5\JA_Pixelizacja_Obrazu\x64\Debug\ImageProcessingCPP.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern void PixelizeRegion()
        public static extern void pixelizeImage(
            IntPtr imageData,
            int width,
            int height,
            int pixelSize);
    }

    public static class ASMLibrary
    {
        [DllImport(@"D:\SUT\JA\Projekt SEM5\JA_Pixelizacja_Obrazu\x64\Debug\ImageProcessingASM.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void pixelizeImage(
            IntPtr imageData,
            int width,
            int height,
            int pixelSize);
    }
}
