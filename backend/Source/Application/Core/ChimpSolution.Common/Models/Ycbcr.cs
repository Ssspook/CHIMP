namespace ChimpSolution.Common.Models;

public struct YCbCr
{
    public float Y;
    public float Cb;
    public float Cr;

    public YCbCr(float y, float cb, float cr)
    {
        Y = y;
        Cb = cb;
        Cr = cr;
    }
}