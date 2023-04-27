using ChimpSolution.Common;
using Histogram;
using SkiaSharp;

namespace Filtering;

public class OtsuThresholdFilter
{
    public SKBitmap Filter(SKBitmap picture)
    {
        var thresholdFilter = new ThresholdFilter();
        var threshold = CalculateOtsuThreshold(picture);
        return thresholdFilter.Filter(picture, threshold);
    }
    

    private int CalculateOtsuThreshold(SKBitmap picture)
    {
        var redHist = new HistogramEqualizer().GetChannelHistogram(picture, RgbChannel.Red);
        var greenHist = new HistogramEqualizer().GetChannelHistogram(picture, RgbChannel.Green);
        var blueHist = new HistogramEqualizer().GetChannelHistogram(picture, RgbChannel.Blue);
        
        var pixelCount = picture.Width * picture.Height;
        
        var (redIntensity, greenIntensity, blueIntensity) = CalculateIntensitySumForChannels(picture);

        var bestThreshold = 0;
        double bestSigma = 0;

        var firstClassPixelCountForRed = 0;
        var firstClassIntensitySumRed = 0;
        
        var firstClassPixelCountForGreen = 0;
        var firstClassIntensitySumGreen = 0;
        
        var firstClassPixelCountForBlue = 0;
        var firstClassIntensitySumBlue = 0;
        
        for (var thresh = 0; thresh < 255; ++thresh)
        {
            var redSigma = CalculateSigma(
                ref firstClassPixelCountForRed,
                ref firstClassIntensitySumRed,
                redHist,
                thresh,
                pixelCount, 
                redIntensity
            );

            var greenSigma = CalculateSigma(
                ref firstClassPixelCountForGreen,
                ref firstClassIntensitySumGreen,
                greenHist,
                thresh,
                pixelCount, 
                greenIntensity
            );
            
            var blueSigma = CalculateSigma(
                ref firstClassPixelCountForBlue,
                ref firstClassIntensitySumBlue,
                blueHist,
                thresh,
                pixelCount, 
                blueIntensity
            );

            var maxSigma = Math.Max(Math.Max(redSigma, greenSigma), blueSigma);
            if (!(maxSigma > bestSigma)) continue;
            
            bestSigma = maxSigma;
            bestThreshold = thresh;
        }

        return bestThreshold;
    }

    private static (int, int, int) CalculateIntensitySumForChannels(SKBitmap picture)
    {
        var sumRedIntensities = 0;
        var sumGreenIntensities = 0;
        var sumBlueIntensities = 0;
        
        for (var y = 0; y < picture.Height; y++)
        {
            for (var x = 0; x < picture.Width; x++)
            {
                var rgb = PixelReader.GetRgbFromPixel(picture, x, y);
                sumRedIntensities += rgb.RedByte;
                sumGreenIntensities += rgb.GreenByte;
                sumBlueIntensities += rgb.BlueByte;
            }
        }

        return (sumRedIntensities, sumGreenIntensities, sumBlueIntensities);
    }

    private double CalculateSigma(
        ref int firstClassPixelCount,
        ref int fistClassIntensitySum, 
        IReadOnlyList<int> hist, 
        int threshold, 
        int pixelCount, 
        int intensitySum
    )
    {
        firstClassPixelCount += hist[threshold];
        fistClassIntensitySum += threshold * hist[threshold];

        var firstClassProbability = firstClassPixelCount / (double) pixelCount;
        var secondClassProbability = 1.0 - firstClassProbability;

        var firstClassMean = fistClassIntensitySum / (double) firstClassPixelCount;
        var secondClassMean = (intensitySum - fistClassIntensitySum) 
                              / (double) (pixelCount - firstClassPixelCount);

        var meanDelta = firstClassMean - secondClassMean;

        return firstClassProbability * secondClassProbability * Math.Pow(meanDelta, 2);
    }
}