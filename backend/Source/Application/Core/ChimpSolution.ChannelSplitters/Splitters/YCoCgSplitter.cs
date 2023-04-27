using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using Converters;
using SkiaSharp;

namespace ChannelSplitters.Splitters;

public class YCoCgSplitter
{
    public static SKBitmap SplitTo(YCoCgChannel channel, SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        var converter = new YCoCgConverter();
        converter.FromRgb(picture);
        var convertedPicture = converter.ConvertedPicture; 
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(convertedPicture, x, y);
                var yCoCg = new YCoCg(rgb.R, rgb.G, rgb.B);
                switch (channel)
                {
                    case YCoCgChannel.Y:
                        yCoCg.Co = 0;
                        yCoCg.Cg = 0;
                        break;
                    case YCoCgChannel.Co:
                        yCoCg.Y = 0;
                        yCoCg.Cg = 0;
                        break;
                    case YCoCgChannel.Cg:
                        yCoCg.Y = 0;
                        yCoCg.Co = 0;
                        break;
                }

                var color = new SKColor((byte)(yCoCg.Y * 255), (byte) (yCoCg.Co * 255), (byte) (yCoCg.Cg * 255));
                bitmap.SetPixel(x, y, color);
            }
        }

        return converter.ToRgb(bitmap);
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
                var yCoCg = new YCoCg(rgb.R, rgb.G, rgb.B);
                var mean = (yCoCg.Y + yCoCg.Co + yCoCg.Cg) / 3;
                yCoCg.Y = mean;
                yCoCg.Co = mean;
                yCoCg.Cg = mean;
                var color = new SKColor((byte)yCoCg.Y, (byte) yCoCg.Co, (byte) yCoCg.Cg);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
}