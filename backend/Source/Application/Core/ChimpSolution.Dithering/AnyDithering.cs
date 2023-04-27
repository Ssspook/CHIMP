using SkiaSharp;

namespace ChimpSolution.Dithering;

public class AnyDithering
{
    public SKBitmap DoDithering(SKBitmap picture, DitheringAlgorithm algorithm, int bitrate)
    {
        switch (algorithm)
        {
            case DitheringAlgorithm.Ordered:
                var orderedDitherer = new OrderedDithering();
                return orderedDitherer.Dither(picture, bitrate);
            case DitheringAlgorithm.Random:
                var randomDithering = new RandomDithering();
                return randomDithering.Dither(picture, bitrate);
            case DitheringAlgorithm.Atkinson:
                var atkinsonDithering = new AtkinsonDithering();
                return atkinsonDithering.Dither(picture, bitrate);
            case DitheringAlgorithm.FloydSteinberg:
                var floydSteinberg = new FloydSteinbergDithering();
                return floydSteinberg.Dither(picture, bitrate);
            case DitheringAlgorithm.None:
                var noneDithering = new NoneDithering();
                return noneDithering.Dither(picture, bitrate);
        }

        throw new NotImplementedException();
    }
}