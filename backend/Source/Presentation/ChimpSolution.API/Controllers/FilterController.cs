using System.Net.Mime;
using Filtering;
using Microsoft.AspNetCore.Mvc;
using PNMReader;

namespace ChimpSolution.API.Controllers;

[ApiController]
[Route("api/filters")]
public class FilterController : ControllerBase
{
    private readonly PnmReader _pnmReader;
    private readonly AnyFilter _filter;
    
    public FilterController()
    {
        _pnmReader = new PnmReader();
        _filter = new AnyFilter();
    }
    
    [HttpPost("threshold")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    public async Task<IActionResult> ThresholdFilter(
        IFormFile image,
        [FromQuery] int threshold)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var filteredBitmap = _filter.Filter(bitmap, Filter.Threshold, threshold);
            var buffer = _pnmReader.ConvertToPng(filteredBitmap);

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
    
    [HttpPost("otsu")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    public async Task<IActionResult> OtsuThreshold(
        IFormFile image)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var filteredBitmap = _filter.Filter(bitmap, Filter.OtsuThreshold, null);
            var buffer = _pnmReader.ConvertToPng(filteredBitmap);

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
    
    [HttpPost("median")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    public async Task<IActionResult> MedianFilter(
        IFormFile image,
        [FromQuery] int radius)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var filteredBitmap = _filter.Filter(bitmap, Filter.Median, radius);
            var buffer = _pnmReader.ConvertToPng(filteredBitmap);

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
    
    [HttpPost("gaussian")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    public async Task<IActionResult> GaussianFilter(
        IFormFile image,
        [FromQuery] int sigma)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var filteredBitmap = _filter.Filter(bitmap, Filter.Gaussian, sigma);
            var buffer = _pnmReader.ConvertToPng(filteredBitmap);

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
    
    [HttpPost("linear")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    public async Task<IActionResult> LinearFilter(
        IFormFile image,
        [FromQuery] int radius)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var filteredBitmap = _filter.Filter(bitmap, Filter.BoxBlur, radius);
            var buffer = _pnmReader.ConvertToPng(filteredBitmap);

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
    
    [HttpPost("sobel")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    public async Task<IActionResult> SobelFilter(
        IFormFile image)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var filteredBitmap = _filter.Filter(bitmap, Filter.Sobel, null);
            var buffer = _pnmReader.ConvertToPng(filteredBitmap);

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
    
    [HttpPost("contrast")]
    [Consumes("multipart/form-data")]
    [Produces("image/x-portable-anymap")]
    public async Task<IActionResult> ContrastFilter(
        IFormFile image,
        [FromQuery] float sharpening)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            
            var filteredBitmap = _filter.Filter(bitmap, Filter.ContrastAdaptiveSharpening, sharpening);
            var buffer = _pnmReader.ConvertToPng(filteredBitmap);

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
}