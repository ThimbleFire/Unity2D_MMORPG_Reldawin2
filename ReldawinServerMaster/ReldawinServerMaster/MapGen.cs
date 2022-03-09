using System;
using System.IO;
using System.Drawing.Imaging;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace ReldawinServerMaster
{
    public class MapGen
    {
		public static byte[,] GenerateTiles(int width, int height, double scale, int seed)
        {
			var noiseMap = Generate( width, height, scale, true, seed );

			byte[,] tiles = new byte[width, height];

            for ( int y = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++ )
                {
					tiles[x, y] = Tile.GetTypeByHeight( noiseMap[x, y] );
                }
            }

			return tiles;
        }

		public static NoiseMap Generate( int width, int height, double scale, bool getNoiseMap, int seed )
		{
			var noiseSource = CreateDefinition(seed);

			var noiseMap = new NoiseMap { };
			var noiseMapBuilder = new PlaneNoiseMapBuilder
			{
				DestNoiseMap = noiseMap,
				SourceModule = noiseSource,
				EnableSeamless = true,
			};

			noiseMapBuilder.SetDestSize( width, height );
			noiseMapBuilder.SetBounds( -scale, scale, -scale, scale );
			noiseMapBuilder.Build();

			if ( getNoiseMap )
				return noiseMap;
			
			// Create a new image and image renderer
			var image = new Image();
			var renderer = new ImageRenderer
			{
				SourceNoiseMap = noiseMap,
				DestinationImage = image
			};

			renderer.AddGradientPoint( 0.025d, new Color( 57, 89, 179, 255 ) );  //water
			renderer.AddGradientPoint( 0.075d, new Color( 188, 188, 128, 255 ) );//sand
			renderer.AddGradientPoint( 0.15d, new Color( 168, 120, 47, 255 ) );	//dirt
			renderer.AddGradientPoint( 0.20d, new Color( 21, 192, 0, 255 ) );	//grass

			renderer.Render();
			using ( var fs = File.OpenWrite( string.Format( "{1}x{2}_{0}.png", seed, width, height ) ) )
			{
				image.SaveGdiBitmap( fs, ImageFormat.Png );
			}

			//renderer.BuildGrayscaleGradient();
			//renderer.Render();
			//using ( var fs = File.OpenWrite( string.Format( "{1}x{2}_{0}_grayscale.png", noiseSource.Seed, width, height ) ) )
			//{
			//	image.SaveGdiBitmap( fs, ImageFormat.Png );
			//}
			
			return null;
		}

		private static Module CreateDefinition(int seed)
        {
			var cac = new Cache
			{
				Source0 = new Clamp
				{
					LowerBound = 0,
					UpperBound = 1,

					Source0 = new Perlin
					{
						Seed = seed
					},
				}
			};

			return cac;
        }
	}
}
