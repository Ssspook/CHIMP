using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace Converters;


public class HslConverter: IConverter
{
    private struct Constants
    {
        public const double Tolerance = 10e-15;
        public const float HsvParametersAssignmentPeriodicityInTrigonometricAngle = 60;
    }

    public SKBitmap ConvertedPicture { get; private set; }
    public SKBitmap FromRgb(SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(picture, x, y);
                var convertedByte = ConvertRgbToHsl(rgb);
                var color = new SKColor((byte)(convertedByte.H * 255), (byte)convertedByte.SaturationInPercentage, (byte)convertedByte.LightnessInPercentage);
                bitmap.SetPixel(x, y, color);
            }
        }

        ConvertedPicture = bitmap;
        return picture;
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
                var hsl = new Hsl(rgb.R, rgb.G, rgb.B);
                var convertedByte = RevertToRgbFromHsl(hsl);
                var color = new SKColor(convertedByte.RedByte, convertedByte.GreenByte, convertedByte.BlueByte);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
    
    private static Hsl ConvertRgbToHsl(Rgb rgb)
    {
        float h, s;

        var cMax = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);
        var cMin = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
        
        var delta = cMax - cMin;
        var l = (cMax + cMin) / 2;
        if (delta == 0)
            h = 0;
        else if (Math.Abs(cMax - rgb.R) < Constants.Tolerance)
            h = (rgb.G - rgb.B) / delta % 6 * Constants.HsvParametersAssignmentPeriodicityInTrigonometricAngle;
        else if (Math.Abs(cMax - rgb.B) < Constants.Tolerance)
            h = ((rgb.B - rgb.R) / delta + 2) * Constants.HsvParametersAssignmentPeriodicityInTrigonometricAngle;
        else
            h = ((rgb.R - rgb.G) / delta + 4) * Constants.HsvParametersAssignmentPeriodicityInTrigonometricAngle;

        if (delta == 0)
            s = 0;
        else
            s = delta / (1 - Math.Abs(2 * l - 1));
        
        return new Hsl(h, s, l);
    }

    private static Rgb RevertToRgbFromHsl(Hsl hsl)
    {
        float r, g, b;
        hsl.H *= 255;
        var c = (1 - Math.Abs(2 * hsl.L - 1)) * hsl.S;
        var x = c * (1 - Math.Abs(hsl.H / Constants.HsvParametersAssignmentPeriodicityInTrigonometricAngle % 2 - 1));
        var m = hsl.L - c / 2;

        switch (hsl.H)
        {
            case >= 0 and < 60:
                r = c;
                g = x;
                b = 0;
                break;
            case >= 60 and < 120:
                r = x;
                g = c;
                b = 0;
                break;
            case >= 120 and < 180:
                r = 0;
                g = c;
                b = x;
                break;
            case >= 180 and < 240:
                r = 0;
                g = x;
                b = c;
                break;
            case >= 240 and <= 300:
                r = x;
                g = 0;
                b = c;
                break;
            default:
                r = c;
                g = 0;
                b = x;
                break;
        }
        return new Rgb(r + m, g + m, b + m);
    }
}