namespace ChimpSolution.Common.Models;

public struct Hsv
{
    public float H;
    public float S;
    public float V;

    public Hsv(float h, float s, float v)
    {
        H = h;
        S = s;
        V = v;
    }
    
    public float SaturationInPercentage => S * 100;
    public float ValueInPercentage => V * 100;
}