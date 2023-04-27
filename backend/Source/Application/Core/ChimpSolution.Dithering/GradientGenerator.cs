using SkiaSharp;

namespace ChimpSolution.Dithering;

public class GradientGenerator
{
    public SKBitmap Generate(int width, int height)
    {
        var bitmap = new SKBitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var avg = 255 * x / width;

                bitmap.SetPixel(x, y, new SKColor((byte)avg, (byte)avg, (byte)avg));
            }
        }

        return bitmap;
    }
}