using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA_Pixelizacja_Obrazu
{
    public class ImageProcessing
    {
        public static Bitmap PixelizeImage(Bitmap originalImage, int pixelSize)
        {
            Bitmap pixelizedImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y += pixelSize)
            {
                for (int x = 0; x < originalImage.Width; x += pixelSize)
                {
                    int offsetX = Math.Min(pixelSize, originalImage.Width - x);
                    int offsetY = Math.Min(pixelSize, originalImage.Height - y);

                    int r = 0, g = 0, b = 0;
                    int pixelCount = 0;

                    for (int dy = 0; dy < offsetY; dy++)
                    {
                        for (int dx = 0; dx < offsetX; dx++)
                        {
                            Color pixelColor = originalImage.GetPixel(x + dx, y + dy);
                            r += pixelColor.R;
                            g += pixelColor.G;
                            b += pixelColor.B;
                            pixelCount++;
                        }
                    }

                    r /= pixelCount;
                    g /= pixelCount;
                    b /= pixelCount;

                    Color averageColor = Color.FromArgb(r, g, b);

                    for (int dy = 0; dy < offsetY; dy++)
                    {
                        for (int dx = 0; dx < offsetX; dx++)
                        {
                            pixelizedImage.SetPixel(x + dx, y + dy, averageColor);
                        }
                    }
                }
            }

            return pixelizedImage;
        }
    }
}
