using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace Converters;

public class CmyConverter: IConverter
{
    public SKBitmap ConvertedPicture { get; private set; }
    public SKBitmap ToRgb(SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = PixelReader.GetRgbFromPixel(picture, x, y);
                var aliasedCmyPixel = new Cmy(pixel.R, pixel.G, pixel.B);
                var convertedRgbPixel = new Rgb(1 - aliasedCmyPixel.C, 1 - aliasedCmyPixel.M, 1 - aliasedCmyPixel.Y);

                var color = new SKColor(convertedRgbPixel.RedByte, convertedRgbPixel.GreenByte,  convertedRgbPixel.BlueByte);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }

    public SKBitmap FromRgb(SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixelInRgb = PixelReader.GetRgbFromPixel(picture, x, y);
                var cmy = new Cmy(255 - pixelInRgb.RedByte, 255 - pixelInRgb.GreenByte, 255 - pixelInRgb.BlueByte);

                var color = new SKColor((byte) cmy.C, (byte) cmy.M, (byte) cmy.Y);
                bitmap.SetPixel(x, y, color);
            }
        }

        ConvertedPicture = bitmap;
        return picture;
    }
}