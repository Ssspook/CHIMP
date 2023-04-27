using System.ComponentModel;
using Filtering.Filters;
using SkiaSharp;

namespace Filtering;

public class AnyFilter
{
    public SKBitmap Filter(SKBitmap picture, Filter filter, object? param)
    {
        switch (filter)
        {
          case Filtering.Filter.Threshold:
              var thresholdFilter = new ThresholdFilter();
              if (param is int threshold)
                  return thresholdFilter.Filter(picture, threshold);
              throw new InvalidEnumArgumentException("Parameter is not castable to int");
          
          case Filtering.Filter.OtsuThreshold:
              var otsuThresholdFilter = new OtsuThresholdFilter();
              return otsuThresholdFilter.Filter(picture);
          
          case Filtering.Filter.Gaussian:
              var gaussianFilter = new GaussianFilter();
              if (param is int kernel)
                return gaussianFilter.Filter(picture, kernel);
              throw new InvalidEnumArgumentException("Parameter is not castable to float[,]");
              
          case Filtering.Filter.Median:
              var medianFilter = new MedianFilter();
              return medianFilter.Filter(picture);
          
          case Filtering.Filter.Sobel:
              var sobelFilter = new SobelFilter(false);
              return sobelFilter.Filter(picture);
          
          case Filtering.Filter.BoxBlur: 
              var bbFilter = new BoxBlurFilter();
              if (param is int radius)
                  return bbFilter.Filter(picture, radius);
              throw new InvalidEnumArgumentException("Parameter is not castable to int");
          
          case Filtering.Filter.ContrastAdaptiveSharpening:
              var casFilter = new CasFilter();
              if (param is float sharpness)
                  return casFilter.Filter(picture, sharpness);
              throw new InvalidEnumArgumentException("Parameter is not castable to float");
        }

        throw new ApplicationException("Invalid case");
    }
}