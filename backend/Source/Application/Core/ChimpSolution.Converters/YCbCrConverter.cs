using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using Converters.Helpers;
using SkiaSharp;

namespace Converters;

public class YCbCrConverter: IConverter
{

    private static YCbCrConvention Convention { get; set; }
    
    public SKBitmap ConvertedPicture { get; private set; }
    
    public void SetConvention(YCbCrConvention convention)
    {
        Convention = convention;
    }

    public SKBitmap ToRgb(SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
    
        var bitmap = new SKBitmap(width, height);
    
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(picture, x, y);
                var yCbCr = new YCbCr(rgb.R, rgb.G, rgb.B);
                var convertedByte = ConvertYCbCrToRgb(yCbCr);
                var color = new SKColor(convertedByte.RedByte, convertedByte.GreenByte, convertedByte.BlueByte);
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
                var convertedByte = ConvertRgbToYCbCr(pixelInRgb);
                var color = new SKColor((byte) (convertedByte.Y * 255), (byte) (convertedByte.Cb * 255), (byte) (convertedByte.Cr * 255));
                bitmap.SetPixel(x, y, color);
            }
        }

        ConvertedPicture = bitmap;
        return picture;
    }

    private static Rgb ConvertYCbCrToRgb(YCbCr ycbcr)
    {
        var rgb = new Rgb();
        
        float r;
        float g;
        float b;
        
        switch (Convention)
        {
            case YCbCrConvention.YCbCr709:
                r = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y + 0.0000 * ycbcr.Cb + 1.5748 * ycbcr.Cr)));
                g = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y - 0.1873 * ycbcr.Cb - 0.4681 * ycbcr.Cr)));
                b = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y + 1.8556 * ycbcr.Cb + 0.0000 * ycbcr.Cr)));
                break;
                
            case YCbCrConvention.YCbCr601:
                r = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y + 0.0000 * ycbcr.Cb + 1.4022 * ycbcr.Cr)));
                g = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y - 0.3456 * ycbcr.Cb - 0.7145 * ycbcr.Cr)));
                b = Math.Max(0.0f, Math.Min(1.0f, (float) (ycbcr.Y + 1.7710 * ycbcr.Cb + 0.0000 * ycbcr.Cr)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        rgb.R = r;
        rgb.G = g;
        rgb.B = b;
        return rgb;
    }
    
    private static YCbCr ConvertRgbToYCbCr(Rgb rgb)
    {
        var ycbcr = new YCbCr();
        
        var r = rgb.R;
        var g = rgb.G;
        var b = rgb.B;
      
        
        switch (Convention)
        {
            case YCbCrConvention.YCbCr709:
                ycbcr.Y = (float) (0.2126 * r + 0.7152 * g + 0.0722 * b);
                ycbcr.Cb = (float) (-0.1146 * r - 0.3854 * g + 0.5000 * b);
                ycbcr.Cr = (float) (0.5000 * r - 0.4542 * g - 0.0458 * b);
                break;

            case YCbCrConvention.YCbCr601:
                ycbcr.Y = (float) (0.2989 * r + 0.5866 * g + 0.1145 * b);
                ycbcr.Cb = (float) (-0.1687 * r - 0.3313 * g + 0.5000 * b);
                ycbcr.Cr = (float) (0.5000 * r - 0.4184 * g - 0.0816 * b);
                break;
                    
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return ycbcr;
    }
}
