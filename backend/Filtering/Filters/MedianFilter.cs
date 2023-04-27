using SkiaSharp;

namespace Filtering.Filters;

public class MedianFilter
{
    public SKBitmap Filter(SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        var termsList = new List<byte>();

        var image = new byte[width, height];
        
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var c = picture.GetPixel(i, j);
                var gray = (byte)(.333 * c.Red + .333 * c.Green + .333 * c.Blue);
                image[i, j] = gray;
            }
        }
        
        for (var i = 0; i <= width - 3; i++)
        for (var j = 0; j <= height - 3; j++)
        {
            for (var x = i; x <= i + 2; x++)
            for (var y = j; y <= j + 2; y++)
            {
                termsList.Add(image[x, y]);
            }
            var terms = termsList.ToArray();
            termsList.Clear();
            Array.Sort<byte>(terms);
            Array.Reverse(terms);
            var color = terms[4];
            bitmap.SetPixel(i + 1, j + 1, new SKColor(color, color, color));
        }

        return bitmap;
    }
}