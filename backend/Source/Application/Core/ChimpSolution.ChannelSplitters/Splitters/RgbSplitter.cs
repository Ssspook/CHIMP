using ChimpSolution.Common;
using SkiaSharp;

namespace ChannelSplitters.Splitters;

public static class RgbSplitter
{
    public static SKBitmap SplitTo(RgbChannel channel, SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(picture, x, y);
                switch (channel)
                {
                    case RgbChannel.Red:
                        rgb.G = 0;
                        rgb.B = 0;
                        break;
                    case RgbChannel.Green:
                        rgb.R = 0;
                        rgb.B = 0;
                        break;
                    case RgbChannel.Blue:
                        rgb.R = 0;
                        rgb.G = 0;
                        break;
                }

                var color = new SKColor(rgb.RedByte, rgb.GreenByte, rgb.BlueByte);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }

    public static SKBitmap GetRgbRepresentationForDownloading(SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(picture, x, y);
                var mean = (rgb.R + rgb.G + rgb.B) / 3;
                rgb.R = mean;
                rgb.G = mean;
                rgb.B = mean;
                var color = new SKColor(rgb.RedByte, rgb.GreenByte, rgb.BlueByte);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
}