using ChannelSplitters.Splitters;
using ChimpSolution.Common;
using ChimpSolution.Common.Enum;
using Converters.Helpers;
using SkiaSharp;

namespace ChannelSplitters;

public class AnySplitter
{
    private SKBitmap Picture { get; set; }

    public void SetBitmap(SKBitmap picture)
    {
        Picture = picture;
    }

    public SKBitmap SplitPictureTo(int channel, ColorSpaceEnum colorSpace)
    {
        return colorSpace switch
        {
            ColorSpaceEnum.Rgb => RgbSplitter.SplitTo((RgbChannel)channel, Picture),
            ColorSpaceEnum.Cmy => CmySplitter.SplitTo((CmyChannel)channel, Picture),
            ColorSpaceEnum.Hsl => HslSplitter.SplitTo((HslChannel)channel, Picture),
            ColorSpaceEnum.Hsv => HsvSplitter.SplitTo((HsvChannel)channel, Picture),
            ColorSpaceEnum.YCbCr601 => YCbCrSplitter.SplitTo((YCbCrChannel)channel, Picture, YCbCrConvention.YCbCr601),
            ColorSpaceEnum.YCbCr709 => YCbCrSplitter.SplitTo((YCbCrChannel)channel, Picture, YCbCrConvention.YCbCr709),
            ColorSpaceEnum.YCoCg => YCoCgSplitter.SplitTo((YCoCgChannel)channel, Picture),
            _ => throw new ArgumentException()
        };
    }
}