using System.Runtime.InteropServices;
using SkiaSharp;

public class GaussianFilter
{
    public SKBitmap Filter(SKBitmap picture, int theta)
    {
        var kernel = GaussianBlur(3 * theta, 3 * theta);
        var width = picture.Width;
        var height = picture.Height;
        
        var bytes = picture.RowBytes * height;
        var buffer = new byte[bytes];

        Marshal.Copy(picture.GetPixels(), buffer, 0, bytes);
        
        var rgb = new double[3];
        var foff = (kernel.GetLength(0) - 1) / 2;
        int kcenter;
        int kpixel;
        
        for (var y = foff; y < height - foff; y++)
        {
            for (var x = foff; x < width - foff; x++)
            {
                rgb[0] = 0.0;
                rgb[1] = 0.0;
                rgb[2] = 0.0;

                kcenter = y * picture.RowBytes + x * 4;
                for (var fy = -foff; fy <= foff; fy++)
                {
                    for (var fx = -foff; fx <= foff; fx++)
                    {
                        kpixel = kcenter + fy * picture.RowBytes + fx * 4;
                        for (var c = 0; c < 3; c++)
                            rgb[c] += buffer[kpixel + c] * kernel[fy + foff, fx + foff];
                    }
                }

                for (var c = 0; c < 3; c++)
                {
                    rgb[c] = rgb[c] switch
                    {
                        > 255 => 255,
                        < 0 => 0,
                        _ => rgb[c]
                    };
                }

                picture.SetPixel(x, y, new SKColor((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]));
            }
        }
        
        return picture;
    }
    
    private double[,] GaussianBlur(int lenght, double weight)
    {
        var kernel = new double[lenght, lenght];
        double kernelSum = 0;
        var foff = (lenght - 1) / 2;
        double distance;
        var constant = 1d / (2 * Math.PI * weight * weight);
        for (var y = -foff; y <= foff; y++)
        {
            for (var x = -foff; x <= foff; x++)
            {
                distance = (y * y + x * x) / (2 * weight * weight);
                kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                kernelSum += kernel[y + foff, x + foff];
            }
        }

        for (var y = 0; y < lenght; y++)
        {
            for (var x = 0; x < lenght; x++)
            {
                kernel[y, x] = kernel[y, x] * 1d / kernelSum;
            }
        }

        return kernel;
    }
}