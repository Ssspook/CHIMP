using ChimpSolution.Common.Models;
using SkiaSharp;

namespace ChimpSolution.LineDrawing;

public class LineDrawer
{
    public SKBitmap DrawLine(SKBitmap picture, Point startPoint, Point endPoint, Rgb lineColor, int lineWidth, int transparency)
    {
        var line = new Line(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, lineColor, lineWidth, transparency);
        line.DrawLine(picture);
        
        return picture;
    }
}