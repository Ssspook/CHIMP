using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace Filtering;

public class ThresholdFilter
{
    
    public SKBitmap Filter(SKBitmap picture, int threshold)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = PixelReader.GetRgbFromPixel(picture, x, y);
                var newRgb = new Rgb(
                    (float)GetNewValueForChannel(pixel.RedByte, threshold) / 255,
                    (float)GetNewValueForChannel(pixel.GreenByte, threshold) / 255,
                    (float)GetNewValueForChannel(pixel.BlueByte, threshold) / 255
                );

                var color = new SKColor(newRgb.RedByte, newRgb.GreenByte, newRgb.BlueByte);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }

    private static byte GetNewValueForChannel(float channel, int threshold)
        => channel >= threshold ? (byte) 255 : (byte)0;
}