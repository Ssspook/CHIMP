using ChimpSolution.Common;
using ChimpSolution.Common.Models;
using SkiaSharp;

namespace ChimpSolution.LineDrawing;

public class Line
{
    private double _x0, _y0, _x1, _y1;
    private readonly int _lineWidth;
    private readonly float _alpha;
    private readonly Rgb _lineColor;

    public Line(double x0, double y0, double x1, double y1, Rgb lineColor, int lineWidth, int transparency)
    {
        _x0 = x0;
        _y0 = y0;
        _y1 = y1;
        _x1 = x1;
        _lineColor = lineColor;
        _lineWidth = lineWidth;
        _alpha =  1 - (float)transparency / 100;
    }

    public void DrawLine(SKBitmap bitmap)
    {
        var steep = Math.Abs(_y1 -_y0) > Math.Abs(_x1 -_x0);
        double temp;
        if (steep)
        {
            temp =_x0; _x0 =_y0; _y0 = temp;
            temp =_x1; _x1 = _y1; _y1 = temp;
        }
        if (_x0 >_x1)
        {
            temp = _x0; _x0 =_x1; _x1 = temp;
            temp = _y0; _y0 =_y1; _y1 = temp;
        }
        
        var dx = _x1 -_x0;
        var dy = _y1 -_y0;
        var gradient = dy/dx;

        var yIntersection = _y0 + Rfpart(_x0) * gradient;
        var x = _x0;
        var y = _y0 + gradient;
        
        while (x <= _x1) {
            PutLinePoint(bitmap, steep,  x, y - Math.Round((double)_lineWidth / 2) + 1);
            DoAntiAliasing(bitmap, steep, x, y - Math.Round((double)_lineWidth / 2), Rfpart(yIntersection));
            DoAntiAliasing(bitmap, steep, x, y + Math.Round((double)_lineWidth / 2) - 1, Fpart(yIntersection));

            x++;
            y += gradient;
            yIntersection += gradient;
        }
    }
    
    private void PutLinePoint(SKBitmap bitmap, bool steep, double x, double y)
    {
        var color = new SKColor((byte)_lineColor.R, (byte)_lineColor.G, (byte)_lineColor.B);
        for (var i = 0; i < _lineWidth - 2; i++) {
            if (steep)
            {
                var oldColor = PixelReader.GetRgbFromPixel(bitmap, (int) y + i, (int) x);
                var newColor = NewColor(
                    new SKColor(oldColor.RedByte, oldColor.GreenByte, oldColor.BlueByte), 
                    color, 
                    _alpha
                );
                using var canvas = new SKCanvas(bitmap);
                canvas.DrawPoint((float)y + i, (float)x, newColor);
            } 
            else 
            {
                var oldColor = PixelReader.GetRgbFromPixel(bitmap, (int) x, (int) y + i);
                var newColor = NewColor(
                    new SKColor(oldColor.RedByte, oldColor.GreenByte, oldColor.BlueByte), 
                    color, 
                    _alpha
                 );
                using var canvas = new SKCanvas(bitmap);
                canvas.DrawPoint((float)x, (float)y + i, newColor);
            }
        }
    }
    
    private void DoAntiAliasing(SKBitmap bitmap, bool steep, double x, double y, double brightness)
    {
        var alpha = _alpha * brightness;
        var color = new SKColor((byte)_lineColor.R, (byte)_lineColor.G, (byte)_lineColor.B);
        var oldColor = PixelReader.GetRgbFromPixel(bitmap, steep ? (int)y : (int)x, steep ? (int)x : (int)y);

        var newColor = NewColor(new SKColor(oldColor.RedByte, oldColor.GreenByte, oldColor.BlueByte), color, (float)alpha);
        using var canvas = new SKCanvas(bitmap);
        canvas.DrawPoint(steep ? (float)y : (float)x, steep ? (float)x : (float)y, newColor);
    }

    private SKColor NewColor(SKColor oldColor, SKColor colorToSet, float alpha)
    {
        return new SKColor(
            (byte) ((1 - alpha) * oldColor.Red + alpha * colorToSet.Red),
            (byte) ((1 - alpha) * oldColor.Green + alpha * colorToSet.Green),
            (byte) ((1 - alpha) * oldColor.Blue + alpha * colorToSet.Blue)
        );
    }
    
    private static double Fpart(double x) 
    {
        if (x < 0) return 1 - (x - Math.Floor(x));
        return x - Math.Floor(x);
    }

    private static double Rfpart(double x) 
    {
        return 1 - Fpart(x);
    }
}