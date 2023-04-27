﻿using ChimpSolution.Common;
using SkiaSharp;

namespace ChimpSolution.Dithering;

public class RandomDithering
{
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
                var pixelColor = PixelReader.GetRgbFromPixelBytes(picture, x, y);
                var random = new Random();
                var rnd = random.Next(-multiplier, multiplier + 1);
                
                pixelColor.R += rnd;
                pixelColor.G += rnd;
                pixelColor.B += rnd;
                
                var newColor = new SKColor(
                    CheckBorder(pixelColor.R / multiplier), 
                    CheckBorder(pixelColor.G / multiplier), 
                    CheckBorder(pixelColor.B / multiplier)
                );
                
                newColor = newColor.WithRed(CheckBorder((float)(newColor.Red * (256 / Math.Pow(2, bitrate -1 )))));
                newColor = newColor.WithGreen(CheckBorder((float)(newColor.Green * (256 / Math.Pow(2, bitrate -1)))));
                newColor = newColor.WithBlue(CheckBorder((float)(newColor.Blue * (256 / Math.Pow(2, bitrate - 1)))));

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