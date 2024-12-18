#pragma once

#ifdef IMAGEPROCESSINGCPP_EXPORTS
#define IMAGEPROCESSINGCPP_API __declspec(dllexport)
#else
#define IMAGEPROCESSINGCPP_API __declspec(dllimport)
#endif

//IMAGEPROCESSINGCPP_API void PixelizeRegion(Gdiplus::Bitmap* src, Gdiplus::Bitmap* dst, int pixelSize);

extern "C" 
{
    IMAGEPROCESSINGCPP_API void PixelizeImage(
        unsigned char* imageData, // Pointer to the image pixel data
        int width,                // Width of the image in pixels
        int height,               // Height of the image in pixels
        int pixelSize             // Size of the pixelization block (e.g., 10 for 10x10)
    );
}