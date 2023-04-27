using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using Converters;
using SkiaSharp;

namespace ChannelSplitters.Splitters;

public class CmySplitter
{
    public static SKBitmap SplitTo(CmyChannel channel, SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;

        var bitmap = new SKBitmap(width, height);
        
        var converter = new CmyConverter();
        converter.FromRgb(picture);
        var convertedPicture = converter.ConvertedPicture; 
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(convertedPicture, x, y);
                var cmy = new Cmy(rgb.R, rgb.G, rgb.B);
                switch (channel)
                {
                    case CmyChannel.Cian:
                        cmy.M = 0;
                        cmy.Y = 0;
                        break;
                    case CmyChannel.Magenta:
                        cmy.C = 0;
                        cmy.Y = 0;
                        break;
                    case CmyChannel.Yellow:
                        cmy.C = 0;
                        cmy.M = 0;
                        break;
                }

                var color = new SKColor((byte) (cmy.C * 255), (byte) (cmy.M * 255), (byte) (cmy.Y * 255));
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
                var cmy = new Cmy(rgb.R, rgb.G, rgb.B);
                var mean = (cmy.C + cmy.M + cmy.Y) / 3;
                cmy.C = mean;
                cmy.M = mean;
                cmy.Y = mean;
                var color = new SKColor((byte)cmy.C, (byte) cmy.M, (byte)cmy.Y);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
}