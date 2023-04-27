using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using Converters;
using Converters.Helpers;
using SkiaSharp;

namespace ChannelSplitters.Splitters;

public static class YCbCrSplitter
{
    public static SKBitmap SplitTo(YCbCrChannel channel, SKBitmap picture, YCbCrConvention convention)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);
        
        var converter = new YCbCrConverter();
        converter.SetConvention(convention);
        converter.FromRgb(picture);
        var convertedPicture = converter.ConvertedPicture; 
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(convertedPicture, x, y);
                var yCbCr = new YCbCr(rgb.R, rgb.G, rgb.B);
                switch (channel)
                {
                    case YCbCrChannel.Y:
                        yCbCr.Cb = 0;
                        yCbCr.Cr = 0;
                        break;
                    case YCbCrChannel.Cb:
                        yCbCr.Y = 0;
                        yCbCr.Cr = 0;
                        break;
                    case YCbCrChannel.Cr:
                        yCbCr.Y = 0;
                        yCbCr.Cb = 0;
                        break;
                }
                
                var color = new SKColor((byte)(yCbCr.Y * 255), (byte) (yCbCr.Cb * 255), (byte) (yCbCr.Cr * 255));
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
                var yCbCr = new YCbCr(rgb.R, rgb.G, rgb.B);
                var mean = (yCbCr.Y + yCbCr.Cb + yCbCr.Cr) / 3;
                yCbCr.Y = mean;
                yCbCr.Cb = mean;
                yCbCr.Cr = mean;
                var color = new SKColor((byte)yCbCr.Y, (byte) yCbCr.Cb, (byte) yCbCr.Cr);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
}