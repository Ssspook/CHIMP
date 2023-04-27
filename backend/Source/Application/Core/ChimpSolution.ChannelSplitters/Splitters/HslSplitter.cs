using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using Converters;
using SkiaSharp;

namespace ChannelSplitters.Splitters;

public class HslSplitter
{
    public static SKBitmap SplitTo(HslChannel channel, SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        var converter = new HslConverter();
        converter.FromRgb(picture);
        var convertedPicture = converter.ConvertedPicture; 
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(convertedPicture, x, y);
                var hsl = new Hsl(rgb.R, rgb.G, rgb.B);
                switch (channel)
                {
                    case HslChannel.Hue:
                        hsl.S = 0;
                        hsl.L = 0;
                        break;
                    case HslChannel.Saturation:
                        hsl.H = 0;
                        hsl.L = 0;
                        break;
                    case HslChannel.Lightness:
                        hsl.H = 0;
                        hsl.S = 0;
                        break;
                }

                var color = new SKColor((byte) (hsl.H * 255), (byte) hsl.SaturationInPercentage, (byte) hsl.LightnessInPercentage);
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
                var hsl = new Hsl(rgb.R, rgb.G, rgb.B);
                var mean = (hsl.H + hsl.S + hsl.L) / 3;
                hsl.H = mean;
                hsl.S = mean;
                hsl.L = mean;
                var color = new SKColor((byte)(hsl.H * 255), (byte) hsl.SaturationInPercentage, (byte) hsl.LightnessInPercentage);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
}