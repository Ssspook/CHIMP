using System.Net.Mime;
using ChimpSolution.Abstractions.Services;
using ChimpSolution.Extensions;
using ChimpSolution.Sdk.Services;
using Microsoft.AspNetCore.Mvc;
using PNGReader;

namespace ChimpSolution.API.Controllers;

[ApiController]
[Route("api/png")]
public class PngController : ControllerBase
{
    private readonly IFileManager _fileManager;
    
    private const string FolderForImages = "temp";

    public PngController()
    {
        _fileManager = new FileSystemFileManager(AppDomain.CurrentDomain.BaseDirectory);
    }
    
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [Produces("image/png")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<byte[]>> GetInitialImage(IFormFile image)
    {
        try
        {
            var bytes = await image.GetBytes();
            _fileManager.SaveFile(FolderForImages, bytes, image.FileName);
            var path = $"{_fileManager.BasePath}{FolderForImages}/{image.FileName}";
            var frame = new Frame(16, path);
            frame.ImportPngFrame(path);
            var pngReader = new PngReader(frame, frame.Width, frame.Height);
            var bytesResult = await pngReader.ReadPNG();

            var cd = new ContentDisposition
            {
                FileName = image.FileName,
                DispositionType = "attachment"
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return new FileContentResult(bytesResult, "image/png");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(e.Message);
        }
    }
}