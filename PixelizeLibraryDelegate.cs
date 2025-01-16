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
    /// <param name="imageData">The image data to be pixelized. Must be in a 32-bit RGBA format</param>
    /// <param name="width">The width of the image. Value must be divisible by 4</param>
    /// <param name="height">The height of the image. Value must be divisible by 4</param>
    /// <param name="pixelSize">The size of the pixel blocks. Value must be divisible by 4</param>
    public delegate void PixelizeLibraryDelegate(
            byte[] imageData,
            int width,
            int height,
            int pixelSize);

    /// <summary>
    /// Class containing the C++ library functions for pixelization.
    /// </summary>
    public static class CPPLibrary
    {
        [DllImport("Lib/ImageProcessingCPP.dll",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void PixelizeImage(
            byte[] imageData,
            int width,
            int height,
            int pixelSize);
    }

    /// <summary>
    /// Class containing the ASM library functions for pixelization. Contains SIMD operations
    /// </summary>
    public static class ASMLibrary
    {
        [DllImport("Lib/ImageProcessingASM.dll",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void PixelizeImage(
            byte[] imageData,
            int width,
            int height,
            int pixelSize);
    }

    /// <summary>
    /// Class containing the ASM library functions for pixelization.
    /// </summary>
    public static class ASM_NonVectorLibrary
    {
        [DllImport("Lib/ImageProcessingASM_NonVector.dll",
            CallingConvention = CallingConvention.StdCall)]
        public static extern void PixelizeImage(
            byte[] imageData,
            int width,
            int height,
            int pixelSize);
    }
}
