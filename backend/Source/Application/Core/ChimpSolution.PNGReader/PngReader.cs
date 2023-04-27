using System.IO.Compression;
using SkiaSharp;

namespace PNGReader;

public class PngReader
{
	private readonly Frame _frame;
	private byte[] _bitDepth;

	private readonly byte[] _ihdrChunk;
	private readonly byte[] _pHYsChunk;
	private readonly byte[] _idatChunk;
	private readonly byte[] _plteChunk;

	public PngReader(Frame inputFrame, int frameWidth, int frameHeight)
	{
		_ihdrChunk = WriteIhdrChunk(frameWidth, frameHeight, 8, 2 );
		_pHYsChunk = WritepHYsChunk(2835 );
		_idatChunk = WriteIdatChunk(inputFrame);
		_plteChunk = WritePlteChunk(inputFrame);

		_frame = inputFrame;

		/*var directory = Directory.GetCurrentDirectory();
		var file = Path.Combine(directory, filename);

		try
		{
			var filestream = new FileStream(file, FileMode.CreateNew, FileAccess.Write);
			filestream.Write( ihdrChunk, 0, ihdrChunk.Length );
			filestream.Write( pHYsChunk, 0, pHYsChunk.Length );
			filestream.Write( idatChunk, 0, idatChunk.Length );
			filestream.Close();

			//Console.WriteLine( "{0} was successfully exported.", filename );

		}
		catch (IOException e)
		{

			Console.WriteLine( e );
			Console.WriteLine("EXPORT ERROR: file was not created.");

		}*/

	}

	public async Task<byte[]> ReadPNG()
	{
		var rv = new byte[_ihdrChunk.Length + _pHYsChunk.Length + _idatChunk.Length];
		Buffer.BlockCopy(_ihdrChunk, 0, rv, 0, _ihdrChunk.Length);
		Buffer.BlockCopy(_pHYsChunk, 0, rv, _ihdrChunk.Length, _pHYsChunk.Length);
		Buffer.BlockCopy(_idatChunk, 0, rv, _ihdrChunk.Length + _pHYsChunk.Length, _idatChunk.Length);
		Buffer.BlockCopy(_plteChunk, 0, rv, _plteChunk.Length + _idatChunk.Length, _plteChunk.Length);
		
		var readBytes = await File.ReadAllBytesAsync(_frame.Path);
		return readBytes;
	}

	private byte[] WriteIhdrChunk( int width, int height, byte bitdepth, byte colortype )
	{    // this function writes the IDAT chunk and file signature of the PNG file
		// and is called first when writing a PNG file

		// pre-defined byte sequences
		byte[] signature = { 137, 80, 78, 71, 13, 10, 26, 10 };
		byte[] ihdrChunk = { 73, 72, 68, 82 };   //73 72 68 82 = IHDR
		byte[] compressionMethod = { 0 };
		byte[] filterMethod = { 0 };
		byte[] interlaceMethod = { 0 };

		// computed byte sequences
		byte[] ihdrLength;      // number of bytes in IHDR chunk

		var imageWidth = new byte[ width ];
		imageWidth = DecimalToByteSequence( width, 4, true );

		var imageHeight = new byte[ height ];
		imageHeight = DecimalToByteSequence( height, 4, true );

		_bitDepth = new byte[1];
		SetBitDepth( bitdepth );

		var colorType = new byte[ 1 ];
		colorType[0] = colortype;

		// compile ihdr data
		var ihdrData = new List<byte>();
		ihdrData.AddRange(imageWidth);
		ihdrData.AddRange(imageHeight);
		ihdrData.AddRange(_bitDepth);
		ihdrData.AddRange(colorType);
		ihdrData.AddRange(compressionMethod);
		ihdrData.AddRange(filterMethod);
		ihdrData.AddRange(interlaceMethod);
		var ihdrDataChunk = ihdrData.ToArray();

		// get length of ihdr data
		ihdrLength = DecimalToByteSequence( ihdrDataChunk.Length, 4, true);

		// add ihdr chunk type code
		var ihdrAddTypeCode = new List<byte>();
		ihdrAddTypeCode.AddRange( ihdrChunk );
		ihdrAddTypeCode.AddRange( ihdrDataChunk );
		var ihdrDataAndTypeCode = ihdrAddTypeCode.ToArray();

		// combine png file signature, ihdrlength, data, type code, and checksum
		var ihdrComplete = new List<byte>();
		ihdrComplete.AddRange(signature);
		ihdrComplete.AddRange(ihdrLength);
		ihdrComplete.AddRange(ihdrDataAndTypeCode);
		//ihdrComplete.AddRange(checkSum);
		var outputIhdr = ihdrComplete.ToArray();

		return outputIhdr;

	}  // end writeIHDRChunk()

	private byte[] WriteIdatChunk( Frame inputFrame )
	{

		byte[] idatchunkTypeCode = { 73, 68, 65, 84 };  // IDAT
		byte[] iendchunkTypeCode = { 73, 69, 78, 68 };  // IEND
		byte[] iendLength = { 0, 0, 0, 0 };
		byte[] idatChecksum = new byte[4];
		byte[] iendChecksum = new byte[4];
		//	long decimalCheckSum;

		// compile ihdr data and get length
		byte[] idatData = Deflate(inputFrame);
		byte[] idatLength = DecimalToByteSequence( idatData.Length, 4, true );

		// add idat chunk type code
		var idatAddChunkType = new List<byte>();
		idatAddChunkType.AddRange( idatchunkTypeCode );
		idatAddChunkType.AddRange( idatData );
		byte[] idatDataPlusChunkType = idatAddChunkType.ToArray();
			
		// combine all elements of idat chunk and add iend chunk
		var idatComplete = new List<byte>();
		idatComplete.AddRange( idatLength );
		idatComplete.AddRange( idatDataPlusChunkType );
		idatComplete.AddRange( idatChecksum );

		idatComplete.AddRange( iendLength );
		idatComplete.AddRange( iendchunkTypeCode );
		idatComplete.AddRange( iendChecksum );
		byte[] idatChunk = idatComplete.ToArray();
		return idatChunk;

	}  // end writeIDATChunk()
	

	public byte[] DecimalToByteSequence( long i, int maxBytes, bool endianness )
	{    // this function solves the equation i = (a * 256^maxBytes) + (b * 256^(maxBytes - 1)) + ... + (x * 256^2) + (y * 256^1) + (z * 256^0)

		var outputByteSequence = new byte[ maxBytes ];
		var byteNumber = maxBytes - 1;
		var counter = 0;

		// check if variable, i, can be represented with the number of bytes, maxBytes, specified by the user.
		var bytesNeeded = (int)Math.Ceiling( Math.Log( i, 256 ) );

		// add functionality to change the endianness of the output byte sequence
		if (bytesNeeded <= maxBytes)
		{
			// build outputByteSequence
			while (counter < maxBytes)
			{
				if (Math.Pow(256, byteNumber) > i)
				{
					outputByteSequence[counter] = 0;
				}
				else if (Math.Pow(256, byteNumber) <= i)
				{
					outputByteSequence[counter] = (byte)(i / Math.Pow(256, byteNumber));
					i -= (long)(Math.Pow(256, byteNumber) * outputByteSequence[counter]);
				}
				byteNumber--;
				counter++;
			}
		}
		else
		{
			throw new Exception("EXCEPTION: {0} can not be represented.");
		}
		return outputByteSequence;
	}  

	private void SetBitDepth( byte bitDepthValue )
	{

		if (bitDepthValue == 8)
		{

			_bitDepth[0] = 8;

		}
		else if (bitDepthValue == 16)
		{

			_bitDepth[0] = 16;

		}
		else
		{
			Console.Write("EXCEPTION: invalid value for PNG bit depth.");

		}

	}  

	private byte[] Deflate( Frame inputFrame )
	{

		var yCounter = 0;
		var xCounter = 0;
		var masterCounter = 0;
		var width = inputFrame.GetFrameWidth();
		var height = inputFrame.GetFrameHeight();

		// pre-defined and computed byte sequences
		byte[] checkSum = { 0, 0, 0, 0 };
		byte[] methodCode = { 0x78 };   // 0x78 = 120 sub 10 = 01111000 sub 2
		byte[] windowSize = { 0xda };   // leftmost three bits of window size
		byte[] filterTypeByte = { 0 };

		byte[] toAdlerChecksum = { methodCode[0], windowSize[0] };

		// convert 3, 2d arrays to 1, 1d array
		var arraySize = ( 3 * width * height ) + height;  // calculates arraySize for three values per pixel and adds height at the end
		// to include the filterTypeByte at the beginning of each line.
		var idatData = new byte[ arraySize ];

		while (yCounter < height)
		{

			idatData[masterCounter] = filterTypeByte[0];
			masterCounter++;

			while (xCounter < width )
			{

				idatData[masterCounter] = (byte)inputFrame.GetRedChannelPixel(xCounter, yCounter);
				masterCounter++;

				idatData[masterCounter] = (byte)inputFrame.GetGreenChannelPixel(xCounter, yCounter);
				masterCounter++;

				idatData[masterCounter] = (byte)inputFrame.GetBlueChannelPixel(xCounter, yCounter);
				masterCounter++;

				xCounter++;

			}

			xCounter = 0;
			yCounter++;

		}

		// run DEFLATE
		using MemoryStream outputStream = new MemoryStream();
		using DeflateStream dfStream = new DeflateStream( outputStream, CompressionLevel.Optimal );
		dfStream.Write( idatData, 0, idatData.Length );
		dfStream.Close();

		var data = outputStream.ToArray();


		var idatList = new List<byte>();
		idatList.AddRange( methodCode );  // "For PNG compression method 0, the zlib compression method/flags code must specify method code 8 ("deflate" compression)" http://libpng.org/pub/png/spec/1.2/PNG-Compression.html
		idatList.AddRange( windowSize );
		idatList.AddRange( data );   // idatData
		idatList.AddRange( checkSum );
		var outputByteSequence = idatList.ToArray();

		return outputByteSequence;
	}  

	private byte[] WritepHYsChunk( int ppm )
	{

		byte[] chunkType = { 112, 72, 89, 115 };
		byte[] length;
		byte[] pixelsPerUnitWidthX = DecimalToByteSequence( ppm, 4, true );
		byte[] pixelsPerUnitHeightY = DecimalToByteSequence( ppm, 4, true );
		byte[] unitSpecifier = { 0 };
		//byte[] pHYsChecksum;
		//	long decimalCheckSum;

		// build data chunk
		var pHYsList = new List<byte>();
		pHYsList.AddRange( pixelsPerUnitWidthX );
		pHYsList.AddRange( pixelsPerUnitHeightY );
		pHYsList.AddRange( unitSpecifier );
		byte[] pHYsListDataChunk = pHYsList.ToArray();

		// get length of data chunk
		length = DecimalToByteSequence(pHYsListDataChunk.Length, 4, true );

		// add chunkType and data chunk
		var pHYsTypeAndDataList = new List<byte>();
		pHYsTypeAndDataList.AddRange( chunkType );
		pHYsTypeAndDataList.AddRange( pHYsListDataChunk );
		byte[] pHYsChunkTypeAndDataChunk = pHYsTypeAndDataList.ToArray();
			

		// assemble final pHYs chunk and outputByteSequence
		var pHYsChunkList = new List<byte>();
		pHYsChunkList.AddRange( length );
		pHYsChunkList.AddRange( pHYsChunkTypeAndDataChunk );
		//pHYsChunkList.AddRange( pHYsChecksum );
		byte[] pHYsChunk = pHYsChunkList.ToArray();

		return pHYsChunk;

	}

	private byte[] WritePlteChunk(Frame input)
	{
		MemoryStream ms = new MemoryStream();
		byte[] data = ms.ToArray();

		ms = new MemoryStream();
		ms.Write(data, 0, 8);

		int offset = 8;
		byte[] chkLenBytes = new byte[4];
		int chkLength = 0;
		string chkType = string.Empty;

		while (offset < data.Length - 12)
		{
			chkLenBytes[0] = data[offset];
			chkLenBytes[1] = data[offset + 1];
			chkLenBytes[2] = data[offset + 2];
			chkLenBytes[3] = data[offset + 3];
			if (System.BitConverter.IsLittleEndian)
				System.Array.Reverse(chkLenBytes);

			chkLength = System.BitConverter.ToInt32(chkLenBytes, 0);

			chkType = System.Text.Encoding.ASCII.GetString(data, offset + 4, 4);

			if (chkType != "gAMA")
			{
				if (chkType == "IDAT" || chkType == "PLTE")
				{
					ms.Write(data, offset, data.Length - offset);
					break;
				}
				else
				{
					ms.Write(data, offset, 12 + chkLength);
					offset += 12 + chkLength;
				}
			}
			else
			{
				offset += 12 + chkLength;
				ms.Write(data, offset, data.Length - offset);
				break;
			}

			return data;
		}

		return data;
	}
}