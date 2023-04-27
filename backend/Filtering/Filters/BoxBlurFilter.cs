using SkiaSharp;

namespace Filtering.Filters;

public class BoxBlurFilter
{
    public SKBitmap Filter(SKBitmap picture, int radius)
    {
        var width = picture.Width;
        var height = picture.Height;
        
        SKBitmap bitmap = new SKBitmap(width, height);
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                if (i <= radius || j <= radius || i + radius == width || j + radius == height) continue;
                var newRValue = picture.GetPixel(i, j + radius).Red +
                                picture.GetPixel(i + radius, j + radius).Red +
                                picture.GetPixel(i - radius, j).Red +
                                picture.GetPixel(i, j).Red +
                                picture.GetPixel(i + radius, j).Red +
                                picture.GetPixel(i - radius, j - radius).Red +
                                picture.GetPixel(i, j - radius).Red +
                                picture.GetPixel(i + radius, j - radius).Red;
                newRValue = Convert.ToInt32(newRValue / 9);

                var newGValue = picture.GetPixel(i, j + radius).Green +
                                picture.GetPixel(i + radius, j + radius).Green +
                                picture.GetPixel(i - radius, j).Green +
                                picture.GetPixel(i, j).Green +
                                picture.GetPixel(i + radius, j).Green +
                                picture.GetPixel(i - radius, j - radius).Green +
                                picture.GetPixel(i, j - radius).Green +
                                picture.GetPixel(i + radius, j - radius).Green;
                newGValue = Convert.ToInt32(newGValue / 9);

                var newBValue = picture.GetPixel(i, j + radius).Blue +
                                picture.GetPixel(i + radius, j + radius).Blue +
                                picture.GetPixel(i - radius, j).Blue +
                                picture.GetPixel(i, j).Blue +
                                picture.GetPixel(i + radius, j).Blue +
                                picture.GetPixel(i - radius, j - radius).Blue +
                                picture.GetPixel(i, j - radius).Blue +
                                picture.GetPixel(i + radius, j - radius).Blue;
                newBValue = Convert.ToInt32(newBValue / 9);

                bitmap.SetPixel(i, j, new SKColor((byte)newRValue, (byte)newGValue, (byte)newBValue));
            }
        }

        return bitmap;
    }
}