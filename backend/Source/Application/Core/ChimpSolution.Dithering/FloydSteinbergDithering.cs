using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace ChimpSolution.Dithering;

public class FloydSteinbergDithering
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
                var yPlusOne = y + 1;
                
                var xMinusOne = x - 1;

                var newColor = new SKColor(
                    CheckBorder(pixelColor.R / multiplier), 
                    CheckBorder(pixelColor.G / multiplier), 
                    CheckBorder(pixelColor.B / multiplier)
                );
                newColor = newColor.WithRed(CheckBorder((float)(newColor.Red * (int)(256 / Math.Pow(2, bitrate - 1)))));
                newColor = newColor.WithGreen(CheckBorder((float)(newColor.Green * (int)(256 / Math.Pow(2, bitrate - 1)))));
                newColor = newColor.WithBlue(CheckBorder((float)(newColor.Blue * (int)(256 / Math.Pow(2, bitrate - 1)))));

                var redChannelError = pixelColor.R - newColor.Red;
                var greenChannelError =  pixelColor.G - newColor.Green;
                var blueChannelError =  pixelColor.B - newColor.Blue;
                
                Rgb pixel;
                if (CoordinatesAreValid(width, height, xPlusOne, y))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, xPlusOne, y);
                    picture.SetPixel(xPlusOne, y, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError, 7));
                }
                
                if (CoordinatesAreValid(width, height, xPlusOne, yPlusOne))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, xPlusOne, yPlusOne);
                    picture.SetPixel(xPlusOne, yPlusOne, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError, 1));
                }
                
                if (CoordinatesAreValid(width, height, x, yPlusOne))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, x, yPlusOne);
                    picture.SetPixel(x, yPlusOne, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError, 5));
                }
                
                if (CoordinatesAreValid(width, height, xMinusOne, yPlusOne))
                {
                    pixel = PixelReader.GetRgbFromPixelBytes(picture, xMinusOne, yPlusOne);
                    picture.SetPixel(xMinusOne, yPlusOne, GetNewColor(pixel, redChannelError, greenChannelError, blueChannelError, 3));
                }
                
                // Console.WriteLine($"{newColor.Red} {newColor.Green} {newColor.Blue}");
                picture.SetPixel(x, y, newColor);
            }
        }

        return picture;
    }

    private SKColor GetNewColor(Rgb pixel, double redError, double greenError, double blueError, int part)
    {
        return new SKColor(
            CheckBorder(pixel.R + (float) redError * part / 16),
            CheckBorder(pixel.G + (float) greenError * part / 16), 
            CheckBorder(pixel.B + (float) blueError * part / 16)
        );
    }
    
    private byte CheckBorder(double channel)
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