using ChimpSolution.Common.Models;
using SkiaSharp;

namespace ChimpSolution.Common;

public static class PixelReader
{
    public static Rgb GetRgbFromPixel(SKBitmap bitmap, int x, int y)
    {
        var red = (float)bitmap.GetPixel(x, y).Red;
        var green = (float)bitmap.GetPixel(x, y).Green;
        var blue = (float)bitmap.GetPixel(x, y).Blue;

        return new Rgb(red / 255, green / 255, blue / 255);
    }
    
    public static Rgb GetRgbFromPixelBytes(SKBitmap bitmap, int x, int y)
    {
        var red = (float)bitmap.GetPixel(x, y).Red;
        var green = (float)bitmap.GetPixel(x, y).Green;
        var blue = (float)bitmap.GetPixel(x, y).Blue;

        return new Rgb(red, green, blue);
    }
}