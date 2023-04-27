using ChimpSolution.Common;
using SkiaSharp;

namespace Histogram;

public class HistogramEqualizer
{
    public SKBitmap Equalize(SKBitmap picture, float percentage)
    {
        var subtraction = GetSubtraction(picture, percentage);
        var addition = GetAddition(picture, percentage);
        
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = PixelReader.GetRgbFromPixelBytes(picture, x, y);
                var color = new SKColor(
                    CheckBorder((pixel.R - subtraction) * 255 / (addition - subtraction)),
                    CheckBorder((pixel.G - subtraction) * 255 / (addition - subtraction)),
                    CheckBorder((pixel.B - subtraction) * 255 / (addition - subtraction))
                );
                
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }

    public int[] GetChannelHistogram(SKBitmap picture, RgbChannel channel)
    {
        var histogram = new int[256];
        for (var i = 0; i <= 255; i++)
            histogram[i] = 0;
        
        var height = picture.Height;
        var width = picture.Width;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = PixelReader.GetRgbFromPixelBytes(picture, x, y);
                switch (channel)
                {
                    case RgbChannel.Red:
                        histogram[(int)pixel.R] += 1;
                        break;
                    case RgbChannel.Green:
                        histogram[(int)pixel.G] += 1;
                        break;
                    case RgbChannel.Blue:
                        histogram[(int)pixel.B] += 1;
                        break;
                }
            }
        }

        return histogram;
    }
    
    private byte GetSubtraction(SKBitmap picture, float percentageOfIgnore)
    {
        var redValues = new List<float>();
        var greenValues = new List<float>();
        var blueValues = new List<float>();
        
        var height = picture.Height;
        var width = picture.Width;
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = PixelReader.GetRgbFromPixelBytes(picture, x, y);
                redValues.Add(pixel.R);
                greenValues.Add(pixel.G);
                blueValues.Add(pixel.B);
            }
        }

        redValues.Sort();
        greenValues.Sort();
        blueValues.Sort();

        var ignoredAmountOfPixels = (int) Math.Round(width * height * percentageOfIgnore / 100);
        
        redValues.RemoveRange(0, ignoredAmountOfPixels);
        greenValues.RemoveRange(0, ignoredAmountOfPixels);
        blueValues.RemoveRange(0, ignoredAmountOfPixels);
        
        var minRed = redValues.First();
        var minGreen = greenValues.First();
        var minBlue = blueValues.First();
        
        return (byte) Math.Min(minRed, Math.Min(minGreen, minBlue));
    }
    
    private byte GetAddition(SKBitmap picture, float percentageOfIgnore)
    {
        var redValues = new List<float>();
        var greenValues = new List<float>();
        var blueValues = new List<float>();
        
        var height = picture.Height;
        var width = picture.Width;
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = PixelReader.GetRgbFromPixelBytes(picture, x, y);
                redValues.Add(pixel.R);
                greenValues.Add(pixel.G);
                blueValues.Add(pixel.B);
            }
        }

        var ignoredAmountOfPixels = (int) Math.Round(width * height * percentageOfIgnore / 100);
        
        redValues.Sort();
        redValues.Reverse();
        
        greenValues.Sort();
        greenValues.Reverse();
        
        blueValues.Sort();
        blueValues.Reverse();
        
        redValues.RemoveRange(0, ignoredAmountOfPixels);
        greenValues.RemoveRange(0, ignoredAmountOfPixels);
        blueValues.RemoveRange(0, ignoredAmountOfPixels);
        
        var maxRed = redValues.First();
        var maxGreen = greenValues.First();
        var maxBlue = blueValues.First();
        
        return (byte) Math.Max(maxRed, Math.Max(maxGreen, maxBlue));
    }

    private byte CheckBorder(float channel)
    {
        return channel switch
        {
            < 0 => 0,
            > 255 => 255,
            _ => (byte) channel
        };
    }
}