using System.Net;
using System.Net.Mime;
using ChannelSplitters;
using ChimpSolution.Abstractions.Services;
using ChimpSolution.Common.Enum;
using ChimpSolution.Dithering;
using ChimpSolution.GammaCorrection;
using ChimpSolution.LineDrawing;
using ChimpSolution.Sdk.Services;
using Converters;
using Histogram;
using Microsoft.AspNetCore.Mvc;
using PNMReader;

namespace ChimpSolution.API.Controllers;

[ApiController]
[Route("api/images")]
public class PnmController : ControllerBase
{
    private readonly PnmReader _pnmReader;
    private readonly IFileManager _fileManager;
    private readonly AnyToAnyConverter _colorSpaceConverter;
    private readonly AnySplitter _splitter;
    private readonly GammaCorrector _gammaCorrector;
    private readonly LineDrawer _lineDrawer;
    private readonly AnyDithering _ditherer;
    private readonly GradientGenerator _gradientGenerator;
    private readonly HistogramEqualizer _histogramEqualizer;

    private const string FolderForImages = "temp";

    public PnmController()
    {
        _pnmReader = new PnmReader();
        _fileManager = new FileSystemFileManager(AppDomain.CurrentDomain.BaseDirectory);
        _colorSpaceConverter = new AnyToAnyConverter();
        _splitter = new AnySplitter();
        _gammaCorrector = new GammaCorrector(0);
        _lineDrawer = new LineDrawer();
        _ditherer = new AnyDithering();
        _gradientGenerator = new GradientGenerator();
        _histogramEqualizer = new HistogramEqualizer();
    }

    [HttpPost("initial")]
    public async Task<IActionResult> GetInitialImage(IFormFile image)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var buffer = _pnmReader.ConvertToPng(bitmap);
            var cd = new ContentDisposition
            {
                FileName = image.FileName,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return new FileContentResult(buffer, "image/x-portable-anymap");
        }
        catch (Exception e)
        {
            _fileManager.RemoveBaseDirectory(FolderForImages);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("read/space")]
    [HttpPost("upload/space")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.NotAcceptable)]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> ReadImageWithColorSpace(
        IFormFile image,
        [FromQuery] int bitrate,
        [FromQuery] DitheringAlgorithm ditheringAlgorithm,
        [FromQuery] string? fileName,
        [FromQuery] ColorSpaceEnum inputConvertStrategy,
        [FromQuery] ColorSpaceEnum outputConvertStrategy,
        [FromQuery] int? channel,
        [FromQuery] bool hasAutoCorrection,
        [FromQuery] float correctionPercentage)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);

            var gradientBitmap = _ditherer.DoDithering(bitmap, ditheringAlgorithm, bitrate);
            var fixedBitmap = _colorSpaceConverter.Convert(gradientBitmap, inputConvertStrategy, outputConvertStrategy);
            if (hasAutoCorrection)
            {
                fixedBitmap = _histogramEqualizer.Equalize(fixedBitmap, correctionPercentage);
            }
            
            byte[] buffer;
            if (channel != null)
            {
                var parsedChannel = (int)channel;
                _splitter.SetBitmap(fixedBitmap);
                var bitmapWithChannel = _splitter.SplitPictureTo(parsedChannel, outputConvertStrategy);
                buffer = _pnmReader.ConvertToPng(bitmapWithChannel);
            }
            else
            {
                buffer = _pnmReader.ConvertToPng(fixedBitmap);
            }

            var cd = new ContentDisposition
            {
                FileName = fileName ?? image.FileName,
                Inline = true,
                DispositionType = "attachment"
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return new FileContentResult(buffer, "image/png");
        }
        catch (Exception e)
        {
            _fileManager.RemoveBaseDirectory(FolderForImages);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("apply_gradient")]
    [Produces("image/x-portable-anymap")]
    public Task<IActionResult> ApplyGradient()
    {
        try
        {
            var bitmap = _gradientGenerator.Generate(1000, 500);
            var buffer = _pnmReader.ConvertToPng(bitmap);

            var cd = new ContentDisposition
            {
                FileName = "gradient",
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return Task.FromResult<IActionResult>(new FileContentResult(buffer, "image/x-portable-anymap"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Task.FromResult<IActionResult>(BadRequest(e.Message));
        }
    }
    
    [HttpPost("read_gradient")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.NotAcceptable)]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> ReadGradientImage(
        IFormFile image,
        [FromQuery] int bitrate,
        [FromQuery] DitheringAlgorithm ditheringAlgorithm)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            _fileManager.RemoveFile(FolderForImages, image.FileName);

            var gradientBitmap = _ditherer.DoDithering(bitmap, ditheringAlgorithm, bitrate);
            var buffer = _pnmReader.ConvertToPng(gradientBitmap);

            var cd = new ContentDisposition
            {
                FileName = image.FileName,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return new FileContentResult(buffer, "image/x-portable-anymap");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("assign_gamma")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.NotAcceptable)]
    public async Task<IActionResult> SetGamma(IFormFile image, [FromQuery] float gamma)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            _gammaCorrector.SetGamma(gamma);
            var gammaBitmap = _gammaCorrector.AssignGamma(bitmap);
            var buffer = _pnmReader.ConvertToPng(gammaBitmap);
            var cd = new ContentDisposition
            {
                FileName = image.FileName,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return new FileContentResult(buffer, "image/x-portable-anymap");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("recalculate_gamma")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.NotAcceptable)]
    public async Task<IActionResult> RecalculateGamma(IFormFile image, [FromQuery] int gamma)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            _fileManager.RemoveFile(FolderForImages, image.FileName);
            _gammaCorrector.SetGamma(gamma);
            var gammaBitmap = _gammaCorrector.RecalculateGamma(bitmap);
            var buffer = _pnmReader.ConvertToPng(gammaBitmap);
            var cd = new ContentDisposition
            {
                FileName = image.FileName,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return new FileContentResult(buffer, "image/x-portable-anymap");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("draw_line")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.NotAcceptable)]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> GetImageWithLine(
        IFormFile image,
        [FromQuery] int x1,
        [FromQuery] int y1,
        [FromQuery] int x2,
        [FromQuery] int y2,
        [FromQuery] string color,
        [FromQuery] int lineWidth,
        [FromQuery] int transparency)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);

            var rgbColor = HexToRgbConverter.ConvertHex(color);

            var fixedBitmap = _lineDrawer.DrawLine(
                bitmap,
                new Point(x1, y1),
                new Point(x2, y2),
                rgbColor,
                lineWidth,
                transparency);
            var buffer = _pnmReader.ConvertToPng(fixedBitmap);

            var cd = new ContentDisposition
            {
                FileName = image.FileName,
                Inline = true,
                DispositionType = "attachment"
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return new FileContentResult(buffer, "image/x-portable-anymap");
        }
        catch (Exception e)
        {
            _fileManager.RemoveBaseDirectory(FolderForImages);
            return BadRequest(e.Message);
        }
    }
}