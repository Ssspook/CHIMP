using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace Converters;

public class YCoCgConverter : IConverter
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
                var aliasedYcocgPixel = new YCoCg(pixel.R, pixel.G, pixel.B);
                var tmp = aliasedYcocgPixel.Y - aliasedYcocgPixel.Cg;
                var r   = tmp + aliasedYcocgPixel.Co;
                var g   = aliasedYcocgPixel.Y + aliasedYcocgPixel.Cg;
                var b = tmp - aliasedYcocgPixel.Co;
                
                var convertedRgbPixel = new Rgb(r, g, b);

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
                var ycocg = new YCoCg();
                ycocg.Y =  (float)(0.25 * pixelInRgb.R + 0.50 * pixelInRgb.G + 0.25 * pixelInRgb.B);
                ycocg.Co = (float) (0.50 * pixelInRgb.R - 0 * pixelInRgb.G - 0.50 * pixelInRgb.B);
                ycocg.Cg = (float) (-0.25 * pixelInRgb.R - 0.50 * pixelInRgb.G - 0.25 * pixelInRgb.B);

                var color = new SKColor((byte) (ycocg.Y * 255), (byte) (ycocg.Co * 255), (byte) (ycocg.Cg * 255));
                bitmap.SetPixel(x, y, color);
            }
        }

        ConvertedPicture = bitmap;
        return picture;
    }
}