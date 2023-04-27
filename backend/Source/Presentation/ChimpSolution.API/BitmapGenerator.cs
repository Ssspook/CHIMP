using ChimpSolution.Abstractions.Services;
using ChimpSolution.Extensions;
using ChimpSolution.Sdk.Services;
using PNMReader;
using SkiaSharp;

namespace ChimpSolution.API;

public static class BitmapGenerator
{
    private static readonly IFileManager FileManager = new FileSystemFileManager(AppDomain.CurrentDomain.BaseDirectory);
    private const string FolderForImages = "temp";
    private static readonly PnmReader PnmReader = new PnmReader();
    
    public static async Task<SKBitmap> GetBitmapFromImage(IFormFile image)
    {
        var bytes = await image.GetBytes();
        if (!image.FileName.Contains(".pnm"))
        {
            return SKBitmap.Decode(bytes);
        }
        
        FileManager.SaveFile(FolderForImages, bytes, image.FileName);

        var bitmap = PnmReader.ReadImage(Path.Combine(FileManager.BasePath, FolderForImages, image.FileName));
        FileManager.RemoveFile(FolderForImages, image.FileName);
        return bitmap;
    }
}