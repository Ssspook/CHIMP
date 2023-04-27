using ChimpSolution.Common.Models;

namespace Converters;

public static class HexToRgbConverter
{
    public static Rgb ConvertHex(string hex)
    {
        var hexWithoutHash = hex.Replace("#", string.Empty);
        var rgb = new Rgb
        {
            R = Convert.ToInt32(hexWithoutHash[..2], 16),
            G = Convert.ToInt32(hexWithoutHash.Substring(2, 2), 16),
            B = Convert.ToInt32(hexWithoutHash.Substring(4, 2), 16)
        };
        
        return rgb;
    }
}