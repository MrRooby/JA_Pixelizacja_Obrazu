#pragma once

#ifdef IMAGEPROCESSINGCPP_EXPORTS
#define IMAGEPROCESSINGCPP_API __declspec(dllexport)
#else
#define IMAGEPROCESSINGCPP_API __declspec(dllimport)
#endif


extern "C" 
{
    /**
     * @brief Applies a pixelation effect to an image by averaging the colors in blocks of pixels.
     * 
     * This function processes the input image data and modifies it to create a pixelated effect.
     * It divides the image into blocks of size `pixelSize` x `pixelSize` and replaces each block
     * with the average color of the pixels within that block.
     * 
     * @param imageData Pointer to the image data in RGBA format.
     * @param width The width of the image in pixels.
     * @param height The height of the image in pixels.
     * @param pixelSize The size of the blocks to average. Must be greater than 0.
     * @return IMAGEPROCESSINGCPP_API
     */
    IMAGEPROCESSINGCPP_API void PixelizeImage(
        unsigned char* imageData,
        int width,
        int height,
        int pixelSize
    );
}