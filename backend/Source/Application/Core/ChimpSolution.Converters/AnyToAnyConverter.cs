using ChimpSolution.Common.Enum;
using Converters.Helpers;
using SkiaSharp;

namespace Converters;

public class AnyToAnyConverter
{
   public SKBitmap Convert(SKBitmap image, ColorSpaceEnum initialScheme, ColorSpaceEnum targetScheme)
   {
      if (initialScheme == targetScheme)
         return image;
      
      if (initialScheme == ColorSpaceEnum.Rgb)
      {
         var converter = InferConverterForColorScheme(targetScheme);
         return converter.FromRgb(image);
      }

      if (targetScheme == ColorSpaceEnum.Rgb)
      {
         var converter = InferConverterForColorScheme(initialScheme);
         return converter.ToRgb(image);
      }

      var initialSchemeConverter = InferConverterForColorScheme(initialScheme);
      var targetSchemeConverter = InferConverterForColorScheme(targetScheme);

      var rgbBitmap = initialSchemeConverter.ToRgb(image);
      return targetSchemeConverter.FromRgb(rgbBitmap);
   }

   private IConverter InferConverterForColorScheme(ColorSpaceEnum scheme)
   {
      switch (scheme)
      {
        case ColorSpaceEnum.Cmy:
           return new CmyConverter();
        case ColorSpaceEnum.Hsl:
           return new HslConverter();
        case ColorSpaceEnum.Hsv:
           return new HsvConverter();
        case ColorSpaceEnum.YCbCr601:
           var yCbCr601Converter = new YCbCrConverter();
           yCbCr601Converter.SetConvention(YCbCrConvention.YCbCr601);
           return yCbCr601Converter;
        case ColorSpaceEnum.YCbCr709:
           var yCbCr709Converter = new YCbCrConverter();
           yCbCr709Converter.SetConvention(YCbCrConvention.YCbCr709);
           return yCbCr709Converter;
        case ColorSpaceEnum.YCoCg:
           return new YCoCgConverter();
      }

      throw new ArgumentOutOfRangeException();
   }
}