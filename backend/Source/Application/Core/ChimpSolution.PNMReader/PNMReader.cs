using SkiaSharp;

namespace PNMReader;

public class PnmReader
{
    public SKBitmap ReadImage(string filePath)
    {
        using var br = new BinaryReader(new FileStream(filePath, FileMode.Open));
        if (br.ReadChar() == 'P')
        {
            return br.ReadChar() switch
            {
                '5' => ReadGreyscalePicture(br),
                '6' => ReadColoredPicture(br),
                _ => throw new Exception("Unsupported PNM format")
            };
        }

        throw new Exception("Unsupported image format");
    }

    public byte[] ConvertToPng(SKBitmap bitmap)
    {
        var image = SKImage.FromBitmap(bitmap);
        var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private SKBitmap ReadColoredPicture(BinaryReader reader)
    {
        var width = GetNextHeaderValue(reader);
        var height = GetNextHeaderValue(reader);
        var scale = GetNextHeaderValue(reader);

        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var red = Convert.ToByte(reader.ReadByte() * 255 / scale);
                var green = Convert.ToByte(reader.ReadByte() * 255 / scale);
                var blue = Convert.ToByte(reader.ReadByte() * 255 / scale);

                bitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return bitmap;
    }

    private SKBitmap ReadGreyscalePicture(BinaryReader reader)
    {
        var width = GetNextHeaderValue(reader);
        var height = GetNextHeaderValue(reader);
        var scale = GetNextHeaderValue(reader);

        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var grey = reader.ReadByte() * 255 / scale;
                var greyByte = Convert.ToByte(grey);

                bitmap.SetPixel(x, y, new SKColor(greyByte, greyByte, greyByte));
            }
        }

        return bitmap;
    }

    private int GetNextHeaderValue(BinaryReader reader)
    {
        var hasValue = false;
        var value = string.Empty;

        while (!hasValue)
        {
            var c = reader.ReadChar();

            switch (c)
            {
                case '\n' or ' ' or '\t' when value.Length != 0:
                    hasValue = true;
                    break;
                case >= '0' and <= '9':
                    value += c;
                    break;
            }
        }

        return int.Parse(value);
    }
}