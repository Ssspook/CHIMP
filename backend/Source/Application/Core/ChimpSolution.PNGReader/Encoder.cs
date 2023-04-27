using SkiaSharp;

namespace PNGReader;

public class Frame
{
     public int Width;
     public int Height;
     public int BlockSize;
     private double[,] _redChannel;
     private double[,] _blueChannel;
     private double[,] _greenChannel;

     public Frame(int blockSizeInput, string sourceImagePath)
     {
          Width = 0; //importPNGFrame() will initialize width and height when image is imported
          Height = 0;
          BlockSize = blockSizeInput;
          _redChannel = new double[Width, Height];
          _greenChannel = new double[Width, Height];
          _blueChannel = new double[Width, Height];

          Path = sourceImagePath;
     }

     public void ImportPngFrame( string filename)
     {    // this function creates and returns a matrix of size: [width, height]

          var image = SKBitmap.Decode(filename);
          Height = image.Height;
          Width = image.Width;

          Console.WriteLine("{0} is an {1}x{2} image.", filename, Height, Width);

          //var sourceFrame = new double[width, height];
          _redChannel = new double[Width, Height];
          _greenChannel = new double[Width, Height];
          _blueChannel = new double[Width, Height];

          // build new array: sourceImage
          var i = 0;    // counter variables: i and j.
          var j = 0;

          // timer variables
          var resolution = Width * Height;

          Console.WriteLine("width = {0}", Width);
          Console.WriteLine("height = {0}", Height);

          while ( j < Height )
          {

               while ( i < Width )
               {

                    var pixelColor = image.GetPixel(i, j);
                    _redChannel[i, j] = pixelColor.Red;
                    _greenChannel[i, j] = pixelColor.Green;
                    _blueChannel[i, j] = pixelColor.Blue;

                    i++;

               }
               i = 0;
               j++;

          }

     }  // end importPNGFrame()
     
     public string Path { get; }

     public int GetFrameWidth()
     {

          return Width;

     }

     public int GetFrameHeight()
     {

          return Height;

     }

     public void SetGreenChannelPixel( int x, int y, double pixelValue )
     {

          if ((pixelValue <= 255) & (pixelValue >= 0))
          {

               _greenChannel[x, y] = pixelValue;

          }
          else
          {

               Console.WriteLine("EXCEPTION: Green pixel ({0},{1}) = {2} and it must be between 0 and 255.", x, y, pixelValue);

               // temporary fix; some samples are over 255
               _greenChannel[x, y] = pixelValue;

          }

     }  // end setGreenChannelPixel()

     public double GetGreenChannelPixel( int x, int y )
     {

          return _greenChannel[x, y];

     }

     public void SetRedChannelPixel(int x, int y, double pixelValue)
     {

          if ((pixelValue <= 255) & (pixelValue >= 0))
          {

               _redChannel[x, y] = pixelValue;

          }
          else
          {

               Console.WriteLine( "EXCEPTION: Red pixel ({0},{1}) = {2} and it must be between 0 and 255.", x, y, pixelValue );

               // temporary fix; some samples are over 255
               _redChannel[x, y] = 255;

          }

     }  // end setRedChannelPixel()

     public double GetRedChannelPixel(int x, int y)
     {

          return _redChannel[x, y];

     }

     public void SetBlueChannelPixel(int x, int y, double pixelValue)
     {

          if ((pixelValue <= 255) & (pixelValue >= 0))
          {

               _blueChannel[x, y] = pixelValue;

          }
          else if( pixelValue > 255 )
          {

               Console.WriteLine("EXCEPTION: Blue pixel ({0},{1}) = {2} and it must be between 0 and 255.", x, y, pixelValue);

               // temporary fix; some samples are over 255
               _blueChannel[x, y] = 255;

          }

     }  // end setBlueChannelPixel()

     public double GetBlueChannelPixel(int x, int y)
     {

          return _blueChannel[x, y];

     }

}