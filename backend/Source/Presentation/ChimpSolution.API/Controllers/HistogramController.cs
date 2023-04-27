using ChimpSolution.Common;
using Histogram;
using Microsoft.AspNetCore.Mvc;

namespace ChimpSolution.API.Controllers;

[ApiController]
[Route("api/histogram")]
public class HistogramController : ControllerBase
{
    private readonly HistogramEqualizer _histogramEqualizer;

    public HistogramController()
    {
        _histogramEqualizer = new HistogramEqualizer();
    }
    
    [HttpPost("channel")]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<int[]>> GetInitialImage(IFormFile image, [FromQuery] RgbChannel channel)
    {
        try
        {
            var bitmap = await BitmapGenerator.GetBitmapFromImage(image);
            var buffer = _histogramEqualizer.GetChannelHistogram(bitmap, channel);
            return buffer;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}