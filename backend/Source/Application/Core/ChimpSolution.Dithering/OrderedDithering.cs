using ChimpSolution.Common;
using SkiaSharp;

namespace ChimpSolution.Dithering;

public class OrderedDithering
{
    private struct Constants
    {
        public const int MatrixShape = 8;
    }

    private readonly List<List<float>> _thresholdMatrix = new()
    {
        new List<float> {0, 32, 8, 40, 2, 34, 10, 42},
        new List<float> {48, 16, 56, 24, 50, 18, 58, 26},
        new List<float> {12, 44, 4, 36, 14, 46, 6, 38},
        new List<float> {60, 28, 52, 20, 62, 30, 54, 22},
        new List<float> {3, 35, 11, 43, 1, 33, 9, 41},
        new List<float> {51, 19, 59, 27, 49, 17, 57, 25},
        new List<float> {15, 47, 7, 39, 13, 45, 5, 37},
        new List<float> {61, 31, 55, 23, 61, 29, 53, 21}
    };

    public SKBitmap Dither(SKBitmap picture, int bitrate)
    {
        var height = picture.Height;
        var width = picture.Width;
        var multiplier = (int)(256 / Math.Pow(2, bitrate));
        
        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixelColor = PixelReader.GetRgbFromPixel(picture, x, y);
                pixelColor.R += (_thresholdMatrix[x % Constants.MatrixShape][y % Constants.MatrixShape] / 64 - 0.5f);
                pixelColor.G += (_thresholdMatrix[x % Constants.MatrixShape][y % Constants.MatrixShape] / 64 - 0.5f);
                pixelColor.B +=(_thresholdMatrix[x % Constants.MatrixShape][y % Constants.MatrixShape] / 64 - 0.5f);

                var newColor = new SKColor(
                    CheckBorder(pixelColor.R * 255 / multiplier), 
                    CheckBorder(pixelColor.G * 255/ multiplier), 
                    CheckBorder(pixelColor.B * 255/ multiplier)
                );

                newColor = newColor.WithRed(CheckBorder((float)(newColor.Red * (256 / Math.Pow(2, bitrate -1)))));
                newColor = newColor.WithGreen(CheckBorder((float)(newColor.Green * (256 / Math.Pow(2, bitrate-1)))));
                newColor = newColor.WithBlue(CheckBorder((float)(newColor.Blue * (256 / Math.Pow(2, bitrate-1)))));
                
                bitmap.SetPixel(x, y, newColor);
            }
        }

        return bitmap;
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