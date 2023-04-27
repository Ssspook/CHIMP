using System.Buffers;
using System.Drawing;
using System.Runtime.InteropServices;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace Filtering.Filters;

public class SobelFilter
{
    private static readonly IReadOnlyList<List<double>> XSobel;
    private static readonly IReadOnlyList<List<double>> YSobel;
    private static readonly ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount - 1,
    };

    static SobelFilter()
    {
        XSobel = new List<List<double>>
        {
            new() { -1, 0, 1 },
            new() { -2, 0, 2 },
            new() { -1, 0, 1 },
        };

        YSobel = new List<List<double>>
        {
            new() { -1, -2, -1 },
            new() { 0, 0, 0 },
            new() { 1, 2, 1 },
        };
    }

    private readonly bool _greyscale;

    public SobelFilter(bool greyscale)
    {
        _greyscale = greyscale;
    }

    public SKBitmap Filter(SKBitmap picture)
    {
        var height = picture.Height;
        var width = picture.Width;

        var initialPixels = picture.Bytes;
        
        var pixels = ArrayPool<byte>.Shared.Rent(initialPixels.Length);
        initialPixels.CopyTo(pixels, 0);

        int filterOffset = 1;

        void SobelAlgo(int offsetY)
        {
            var xChannels = ArrayPool<double>.Shared.Rent(picture.BytesPerPixel);
            var yChannels = ArrayPool<double>.Shared.Rent(picture.BytesPerPixel);

            for (var offsetX = filterOffset; offsetX < width - filterOffset; offsetX++)
            {
                Array.Fill(xChannels, 0);
                Array.Fill(yChannels, 0);

                var byteOffset = offsetY * picture.RowBytes + offsetX * picture.BytesPerPixel;

                for (var filterY = -filterOffset; filterY <= filterOffset; filterY++)
                for (var filterX = -filterOffset; filterX <= filterOffset; filterX++)
                {
                    var calcOffset = byteOffset + filterX * picture.BytesPerPixel + filterY * picture.RowBytes;
                    xChannels[0] += pixels[calcOffset] * XSobel[filterY + filterOffset][filterX + filterOffset];
                    xChannels[1] += pixels[calcOffset + 1] * XSobel[filterY + filterOffset][filterX + filterOffset];
                    xChannels[2] += pixels[calcOffset + 2] * XSobel[filterY + filterOffset][filterX + filterOffset];

                    xChannels[0] += pixels[calcOffset] * YSobel[filterY + filterOffset][filterX + filterOffset];
                    xChannels[1] += pixels[calcOffset + 1] * YSobel[filterY + filterOffset][filterX + filterOffset];
                    xChannels[2] += pixels[calcOffset + 2] * YSobel[filterY + filterOffset][filterX + filterOffset];
                }
                
                var pixel = new Rgb
                {
                    R = (float)Math.Sqrt(Math.Pow(xChannels[0], 2) + Math.Pow(yChannels[0], 2)),
                    G = (float) Math.Sqrt(Math.Pow(xChannels[1], 2) + Math.Pow(yChannels[1], 2)),
                    B = (float)Math.Sqrt(Math.Pow(xChannels[2], 2) + Math.Pow(yChannels[2], 2))
                };

                picture.SetPixel(offsetX, offsetY, new SKColor(CheckBorder(pixel.R), CheckBorder(pixel.G), CheckBorder(pixel.B)));
            }
            
            ArrayPool<double>.Shared.Return(xChannels);
            ArrayPool<double>.Shared.Return(yChannels);
        }

        Parallel.For(filterOffset, height - filterOffset, ParallelOptions, SobelAlgo);
        
        ArrayPool<byte>.Shared.Return(pixels);
        
        return picture;
    }

    private byte CheckBorder(double channel)
    {
        return channel switch
        {
            < 0 => 0,
            > 255 => 255,
            _ => (byte) channel
        };
    }
}