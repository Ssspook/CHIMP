namespace PNGReader;

public class Program
{
    public static void Main(string[] args)
    {

        // add path to the input image to be compressed
        const string sourceImagePath = "/Users/sasha/Documents/Study/ITMO/3 course/5 sem/Компьютерная графика/cg22-project-CHIMP/backend/Source/Presentation/ChimpSolution.API/bin/Debug/net6.0/temp/6u4pxh82dhc31.png";

        var frame = new Frame( 16, sourceImagePath);
        frame.ImportPngFrame( sourceImagePath);
        var png = new PngReader( frame, frame.Width, frame.Height);
        var output = png.ReadPNG();
        Console.ReadLine();

    } 
}