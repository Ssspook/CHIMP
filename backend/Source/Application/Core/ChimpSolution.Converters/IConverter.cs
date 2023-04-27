using SkiaSharp;

namespace Converters;

public interface IConverter
{ 
    SKBitmap ToRgb(SKBitmap picture);
    SKBitmap  FromRgb(SKBitmap picture);
}