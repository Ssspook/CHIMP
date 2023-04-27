using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace ChimpSolution.Dithering;

public class AtkinsonDithering
{
     public SKBitmap Dither(SKBitmap picture, int bitrate)
    {
        var height = picture.Height;
        var width = picture.Width;
        var multiplier = (int)(256 / Math.Pow(2, bitrate));

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixelColor = PixelReader.GetRgbFromPixelBytes(picture, x, y);

                var xPlusOne = x + 1;
                var xPlusTwo = x + 2;
                
                var yPlusOne = y + 1;
                var yPlusTwo = y + 2;
                
                var xMinusOne = x - 1;

                var newColor = new SKColor(
                    CheckBorder(pixelColor.R / multiplier), 
                    CheckBorder(pixelColor.G / multiplier), 
                    CheckBorder(pixelColor.B / multiplier)
                );
                newColor = newColor.WithRed(CheckBorder(newColor.Red * (int)(256 / Math.Pow(2, bitrate - 1))));
                newColor = newColor.WithGreen(CheckBorder(newColor.Green * (int)(256 / Math.Pow(2, bitrate - 1))));
                newColor = newColor.WithBlue(CheckBorder(newColor.Blue * (int)(256 / Math.Pow(2, bitrate - 1))));

                var redChannelError = pixelColor.R - newColor.Red;
                var greenChannelError =  pixelColor.G - newColor.Green;
                var blueChannelError =  pixelColor.B - newColor.Blue;

                Rgb pixel;
                if (CoordinatesAreValid(width, height, xPlusOne, y))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, xPlusOne, y);
                    picture.SetPixel(xPlusOne, y, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError));
                }
                
                if (CoordinatesAreValid(width, height, xPlusTwo, y))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, xPlusTwo, y);
                    picture.SetPixel(xPlusTwo, y, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError));
                }
                
                if (CoordinatesAreValid(width, height, xPlusOne, yPlusOne))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, xPlusOne, yPlusOne);
                    picture.SetPixel(xPlusOne, yPlusOne, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError));
                }
                
                if (CoordinatesAreValid(width, height, x, yPlusOne))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, x, yPlusOne);
                    picture.SetPixel(x, yPlusOne, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError));
                }
                
                if (CoordinatesAreValid(width, height, x, yPlusTwo))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, x, yPlusTwo);
                    picture.SetPixel(x, yPlusTwo, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError));
                }
                
                if (CoordinatesAreValid(width, height, xMinusOne, yPlusOne))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, xMinusOne, yPlusOne);
                    picture.SetPixel(xMinusOne, yPlusOne, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError));
                }
                
                picture.SetPixel(x, y, newColor);
            }
        }

        return picture;
    }
     
     private SKColor GetNewColor(Rgb pixel, double redError, double greenError, double blueError)
     {
         return new SKColor(
             CheckBorder(pixel.R + (float)redError / 8),
             CheckBorder(pixel.G + (float)greenError / 8), 
             CheckBorder(pixel.B + (float)blueError / 8)
         );
     }
     
     private byte CheckBorder(float channel)
     {
         return channel switch
         {
             < 0 => 0,
             > 255 => 255,
             _ => (byte) channel
         };
     }
     private static bool CoordinatesAreValid(int width, int height, int x, int y)
         =>  x <= width - 1 && x >= 0 && y <= height - 1 && y >= 0;
}