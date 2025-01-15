using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JA_Pixelizacja_Obrazu
{
    /// <summary>
    /// Delegate for the pixelization library functions. 
    /// The delegate is used to call the pixelization functions from the C++ and ASM libraries.
    /// </summary>
    /// <param name="imageData"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="pixelSize"></param>
    public delegate void PixelizeLibraryDelegate(
            byte[] imageData,         // Pointer to the image pixel data
            int width,                // Width of the image in pixels
            int height,               // Height of the image in pixels
            int pixelSize);           // Size of the pixelization block (e.g., 10 for 10x10)

    /// <summary>
    /// Class containing the C++ library functions for pixelization.
    /// </summary>
    public static class CPPLibrary
    {
        [DllImport("ImageProcessingCPP.dll",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void PixelizeImage(
            byte[] imageData,
            int width,
            int height,
            int pixelSize);
    }

    /// <summary>
    /// Class containing the ASM library functions for pixelization.
    /// </summary>
    public static class ASMLibrary
    {
        [DllImport("ImageProcessingASM.dll",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void PixelizeImage(
            byte[] imageData,
            int width,
            int height,
            int pixelSize);
    }

    public static class ASM_NonVectorLibrary
    {
        [DllImport("ImageProcessingASM_NonVector.dll",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void PixelizeImage(
            byte[] imageData,
            int width,
            int height,
            int pixelSize);
    }
}
