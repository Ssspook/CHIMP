using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using Converters;
using SkiaSharp;

namespace ChannelSplitters.Splitters;

public class HsvSplitter
{
    public static SKBitmap SplitTo(HsvChannel channel, SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        var converter = new HsvConverter();
        converter.FromRgb(picture);
        var convertedPicture = converter.ConvertedPicture; 
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(convertedPicture, x, y);
                var hsv = new Hsv(rgb.R, rgb.G, rgb.B);
                switch (channel)
                {
                    case HsvChannel.Hue:
                        hsv.S = 0;
                        hsv.V = 0;
                        break;
                    case HsvChannel.Saturation:
                        hsv.H = 0;
                        hsv.V = 0;
                        break;
                    case HsvChannel.Value:
                        hsv.H = 0;
                        hsv.S = 0;
                        break;
                }
                
                var color = new SKColor((byte) (hsv.H * 255), (byte) hsv.SaturationInPercentage, (byte) hsv.ValueInPercentage);
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
                var hsv = new Hsv(rgb.R, rgb.G, rgb.B);
                var mean = (hsv.H + hsv.S + hsv.V) / 3;
                hsv.H = mean;
                hsv.S = mean;
                hsv.V = mean;
                var color = new SKColor((byte)hsv.H, (byte) hsv.SaturationInPercentage, (byte) hsv.ValueInPercentage);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
}