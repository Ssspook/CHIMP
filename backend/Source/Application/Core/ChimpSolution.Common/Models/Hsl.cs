namespace ChimpSolution.Common.Models;

public struct Hsl
{
    public float H;
    public float S;
    public float L;

    public Hsl(float h, float s, float l)
    {
        H = h;
        S = s;
        L = l;
    }
    
    public float SaturationInPercentage => S * 100;
    public float LightnessInPercentage => L * 100;
}