using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace ChimpSolution.GammaCorrection;

public class GammaCorrector
{
    private float _invertedGamma;
    private static bool _isDefaultGamma;
    public GammaCorrector(float gamma)
    {
        if (gamma == 0)
            _isDefaultGamma = true;
        else
            _invertedGamma = 1 / gamma;
    }

    public SKBitmap RecalculateGamma(SKBitmap picture)
    {
        var bitmap = InterpretAs(_invertedGamma, picture);
        return InterpretAs(1 / _invertedGamma, bitmap);
    }

    public SKBitmap AssignGamma(SKBitmap picture)
    {
        return InterpretAs(_invertedGamma, picture);
    }

    public void SetGamma(float gamma)
    {
        if (gamma == 0)
            _isDefaultGamma = true;
        else
            _invertedGamma = 1 / gamma;
    }
    
    private static SKBitmap InterpretAs(float power, SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;
        
        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = PixelReader.GetRgbFromPixel(picture, x, y);
                
                Rgb correctedPixel;
                float correctedRed;
                float correctedGreen;
                float correctedBlue;
                
                if (_isDefaultGamma)
                {
                    if ((pixel.R + pixel.G + pixel.B) / 3 > 0.0031308)
                    {
                        correctedRed = (float) (1.055 * Math.Pow(pixel.R, 1 / 2.4) - 0.055);
                        correctedGreen = (float) (1.055 * Math.Pow(pixel.G, 1 / 2.4) - 0.055);
                        correctedBlue = (float) (1.055 * Math.Pow(pixel.B, 1 / 2.4) - 0.055);
                        
                        correctedPixel = new Rgb(correctedRed, correctedGreen, correctedBlue);
                    }
                    else
                    {
                        correctedRed = (float) (12.92 * pixel.R);
                        correctedGreen = (float) (12.92 * pixel.G);;
                        correctedBlue = (float) (12.92 * pixel.B);
                        
                        correctedPixel = new Rgb(correctedRed, correctedGreen, correctedBlue);
                    }
                }
                else
                {
                    correctedRed = (float) Math.Pow(pixel.R, power);
                    correctedGreen = (float) Math.Pow(pixel.G, power);
                    correctedBlue = (float) Math.Pow(pixel.B, power);
                
                    correctedPixel = new Rgb(correctedRed, correctedGreen, correctedBlue);
                }
              
               
                var color = new SKColor(correctedPixel.RedByte, correctedPixel.GreenByte,  correctedPixel.BlueByte);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }
}