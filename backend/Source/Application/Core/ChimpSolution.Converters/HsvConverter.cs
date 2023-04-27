using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace Converters;

public class HsvConverter: IConverter
{
    private struct Constants
    {
        public const double Tolerance = 10e-15;
        public const int HsvParametersAssignmentPeriodicityInTrigonometricAngle = 60;
    }
    
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
                var rgb = PixelReader.GetRgbFromPixel(picture, x, y);
                var hsv = new Hsv(rgb.RedByte, rgb.G, rgb.B);
                var convertedByte = ConvertByteFromHsvToRgb(hsv);
                
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
                var rgb = PixelReader.GetRgbFromPixel(picture, x, y);
                var hsv = ConvertByteFromRgbToHsv(rgb);
                var color = new SKColor((byte) hsv.H, (byte)hsv.SaturationInPercentage, (byte )hsv.ValueInPercentage);
                bitmap.SetPixel(x, y, color);
            }
        }

        ConvertedPicture = bitmap;
        return picture;
    }

    private static Hsv ConvertByteFromRgbToHsv(Rgb rgb)
    {
        float h, s;

        var cMax = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);
        var cMin = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
        
        var delta = cMax - cMin;

        if (delta == 0)
            h = 0;
        else if (Math.Abs(cMax - rgb.R) < Constants.Tolerance)
            h = (rgb.G - rgb.B) / delta % 6 * Constants.HsvParametersAssignmentPeriodicityInTrigonometricAngle;
        else if (Math.Abs(cMax - rgb.B) < Constants.Tolerance)
            h = ((rgb.B - rgb.R) / delta + 2) * Constants.HsvParametersAssignmentPeriodicityInTrigonometricAngle;
        else
            h = ((rgb.R - rgb.G) / delta + 4) * Constants.HsvParametersAssignmentPeriodicityInTrigonometricAngle;

        if (cMax == 0)
            s = 0;
        else
            s = delta / cMax;
        return new Hsv(h, s, cMax);
    }

    private Rgb ConvertByteFromHsvToRgb(Hsv hsv)
    {
        double r = 0, g = 0, b = 0;
        
        if (hsv.S == 0)
        {
            r = hsv.V;
            g = hsv.V;
            b = hsv.V;
        }
        else
        {
            int i;
            double f, p, q, t;

            if (Math.Abs(hsv.H - 360) < Constants.Tolerance)
                hsv.H = 0;
            else
                hsv.H /= 60;

            i = (int)Math.Truncate(hsv.H);
            f = hsv.H - i;

            p = hsv.V * (1.0 - hsv.S);
            q = hsv.V * (1.0 - hsv.S * f);
            t = hsv.V * (1.0 - hsv.S * (1.0 - f));

            switch (i)
            {
                case 0:
                    r = hsv.V;
                    g = t;
                    b = p;
                    break;

                case 1:
                    r = q;
                    g = hsv.V;
                    b = p;
                    break;

                case 2:
                    r = p;
                    g = hsv.V;
                    b = t;
                    break;

                case 3:
                    r = p;
                    g = q;
                    b = hsv.V;
                    break;

                case 4:
                    r = t;
                    g = p;
                    b = hsv.V;
                    break;

                default:
                    r = hsv.V;
                    g = p;
                    b = q;
                    break;
            }

        }

        return new Rgb((float) r, (float) g, (float) b);
    }
}