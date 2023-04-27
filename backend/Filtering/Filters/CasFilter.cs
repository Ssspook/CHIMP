using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace Filtering;

public class CasFilter
{
    public SKBitmap Filter(SKBitmap picture, float sharpening)
    {
        var height = picture.Height;
        var width = picture.Width;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var neighbourPixels = GetNeighbourPixelsFor(picture, x, y, width, height);
                var newRed = CalculateValueForChannel(RgbChannel.Red, neighbourPixels, sharpening, x, y, picture);
                var newGreen = CalculateValueForChannel(RgbChannel.Green, neighbourPixels, sharpening, x, y, picture);
                var newBlue = CalculateValueForChannel(RgbChannel.Blue, neighbourPixels, sharpening, x, y, picture);

                var newPixel = new Rgb(newRed, newGreen, newBlue);
                var color = new SKColor(newPixel.RedByte, newPixel.GreenByte, newPixel.BlueByte);
                
                picture.SetPixel(x, y, color);
            }
        }

        return picture;
    }

    private static Rgb[] GetNeighbourPixelsFor(SKBitmap picture, int x, int y, int width, int height)
    {
        var yMinusOne = y - 1;
        var yPlusOne = y + 1;

        var xPlusOne = x + 1;
        var xMinusOne = x - 1;

        if (y - 1 < 0)
            yMinusOne = y;
        if (y + 1 > height)
            yPlusOne = y;
        if (x - 1 < 0)
            xMinusOne = x;
        if (x + 1 > width)
            xPlusOne = x;
        
        var upPixel = PixelReader.GetRgbFromPixel(picture, x, yMinusOne);
        var downPixel = PixelReader.GetRgbFromPixel(picture, x, yPlusOne);
        
        var leftPixel = PixelReader.GetRgbFromPixel(picture, xPlusOne, y);
        var rightPixel = PixelReader.GetRgbFromPixel(picture, xMinusOne, y);

        return new[] { upPixel, leftPixel, downPixel, rightPixel };
    }

    private static float CalculateValueForChannel(RgbChannel channel, Rgb[] neighbourPixels, float sharpness, int currentPixelX, int currentPixelY, SKBitmap picture)
    {
        var (min, max) = CalculateMinMaxFromNeighbours(neighbourPixels, channel);
        var distanceMax = 1 - max;
        
        double w;
        if (distanceMax < min)
            w = Math.Sqrt(distanceMax / max) * (-0.075 * sharpness - 0.125);
        else
            w = Math.Sqrt(min / max) * (-0.075 * sharpness - 0.125);

        var outputColor = CalculateInitialOutputColor(channel, picture, currentPixelX, currentPixelY);
        outputColor = CalculateOutputColorFor(channel, outputColor, neighbourPixels[0], w);
        outputColor = CalculateOutputColorFor(channel, outputColor, neighbourPixels[1], w);
        outputColor = CalculateOutputColorFor(channel, outputColor, neighbourPixels[2], w);
        outputColor = CalculateOutputColorFor(channel, outputColor, neighbourPixels[3], w);
        
        outputColor /=  4 * w + 1;

        return outputColor switch
        {
            < 0 => 0,
            > 255 => 255,
            _ => (float)outputColor
        };
    }

    private static double CalculateOutputColorFor(RgbChannel channel, double outputColor, Rgb neighbourPixel, double w)
    {
        switch (channel)
        {
          case RgbChannel.Red:
              return outputColor + (float)(neighbourPixel.R * w);
          case RgbChannel.Green:
              return outputColor + (float)(neighbourPixel.G * w);
          case RgbChannel.Blue:
              return outputColor + (float)(neighbourPixel.B * w);
        }
        
        throw new ApplicationException("Invalid case");
    }

    private static double CalculateInitialOutputColor(RgbChannel channel, SKBitmap picture, int x, int y)
    {
        switch (channel)
        {
            case RgbChannel.Red:
                return PixelReader.GetRgbFromPixel(picture, x, y).R;
            case  RgbChannel.Green:
                return PixelReader.GetRgbFromPixel(picture, x, y).G;
            case RgbChannel.Blue:
                return PixelReader.GetRgbFromPixel(picture, x, y).B;
        }
        
        throw new ApplicationException("Invalid case"); 
    }

    private static (float, float) CalculateMinMaxFromNeighbours(Rgb[] neighbours, RgbChannel channel)
    {
        switch (channel)
        {
            case RgbChannel.Red:
                return (neighbours.ToList().Min(rgb => rgb.R), neighbours.ToList().Max(rgb => rgb.R));
            case RgbChannel.Green:
                return (neighbours.ToList().Min(rgb => rgb.G), neighbours.ToList().Max(rgb => rgb.G));
            case RgbChannel.Blue:
                return (neighbours.ToList().Min(rgb => rgb.B), neighbours.ToList().Max(rgb => rgb.B));
        }
        
        throw new ApplicationException("Invalid case"); 
    }
}