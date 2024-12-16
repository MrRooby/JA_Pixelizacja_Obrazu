#include "pch.h"
#include "ImageProcessingCPP.h"


extern "C"
{
    // Exported function for pixelizing an image
    __declspec(dllexport) void pixelizeImage(
        unsigned char* imageData, // Pointer to the image pixel data
        int width,                // Width of the image in pixels
        int height,               // Height of the image in pixels
        int pixelSize)            // Size of the pixelization block (e.g., 10 for 10x10)
    {
        if (pixelSize < 1) return; // Invalid pixel size

        // Iterate over the image in blocks of pixelSize x pixelSize
        for (int y = 0; y < height; y += pixelSize)
        {
            for (int x = 0; x < width; x += pixelSize)
            {
                // Variables to accumulate color components
                long sumR = 0, sumG = 0, sumB = 0, sumA = 0;
                int count = 0;

                // Iterate over each pixel within the current block
                for (int dy = 0; dy < pixelSize; dy++)
                {
                    for (int dx = 0; dx < pixelSize; dx++)
                    {
                        int currentX = x + dx;
                        int currentY = y + dy;

                        // Check boundaries to avoid accessing out-of-bounds memory
                        if (currentX >= width || currentY >= height)
                            continue;

                        int index = (currentY * width + currentX) * 4; // Calculate index for RGBA

                        // Accumulate the color components
                        sumB += imageData[index + 0];
                        sumG += imageData[index + 1];
                        sumR += imageData[index + 2];
                        sumA += imageData[index + 3];
                        count++;
                    }
                }

                if (count == 0) continue; // Avoid division by zero

                // Calculate average color components
                unsigned char avgB = static_cast<unsigned char>(sumB / count);
                unsigned char avgG = static_cast<unsigned char>(sumG / count);
                unsigned char avgR = static_cast<unsigned char>(sumR / count);
                unsigned char avgA = static_cast<unsigned char>(sumA / count);

                // Set all pixels within the block to the average color
                for (int dy = 0; dy < pixelSize; dy++)
                {
                    for (int dx = 0; dx < pixelSize; dx++)
                    {
                        int currentX = x + dx;
                        int currentY = y + dy;

                        // Check boundaries again
                        if (currentX >= width || currentY >= height)
                            continue;

                        int index = (currentY * width + currentX) * 4; // Calculate index for RGBA

                        // Assign the average color to the current pixel
                        imageData[index + 0] = avgB;
                        imageData[index + 1] = avgG;
                        imageData[index + 2] = avgR;
                        imageData[index + 3] = avgA;
                    }
                }
            }
        }
    }
}