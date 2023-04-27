namespace ChimpSolution.Common.Models;

public struct Rgb
{
    public float R;
    public float G;
    public float B;

    public Rgb(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public byte RedByte => (byte) (R * 255);
    public byte GreenByte => (byte) (G * 255);
    public byte BlueByte => (byte) (B * 255);

    public void Normalize()
    {
        R /= 255;
        G /= 255;
    }

}