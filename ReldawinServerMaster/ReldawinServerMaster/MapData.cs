using SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace ReldawinServerMaster
{
    internal class MapData
    {
        private static Dictionary<Color, byte> colorDictionary = new Dictionary<Color, byte>();
        private static Bitmap map;

        public static void Setup() {
            map = new("map2.png");
            colorDictionary.Add( Color.FromArgb( 255, 93, 118, 203 ), 0 ); //water
            colorDictionary.Add( Color.FromArgb( 255, 113, 170, 52 ), 1 ); //grass
            colorDictionary.Add( Color.FromArgb( 255, 227, 207, 87 ), 2 ); //sand
            colorDictionary.Add( Color.FromArgb( 255, 129, 77, 48 ), 3 ); //dirt
            colorDictionary.Add( Color.FromArgb( 255, 33, 94, 33 ), 4 ); //forest
            colorDictionary.Add( Color.FromArgb( 255, 0, 0, 0 ), 5 ); //cliff
        }

        public static string GetChunk( int chunkX, int chunkY ) {
            string data = string.Empty;

            // Unity treats zero, zero as the bottom-left
            // System.Drawing.Bitmap treats zero, zero as the top left
            // Therefore we have to offset this

            int xStart = chunkX * 20;
            int yStart = (map.Height-1) -(chunkY * 20);
            int xLim = xStart + 20;
            int yLim = yStart - 20;

            // There's no limit to stop us from trying to load chunks out of bounds.

            for( int y = yStart; y > yLim; y-- ) {
                for( int x = xStart; x < xLim; x++ ) {
                    data += colorDictionary[map.GetPixel( x, y )];
                }
            }
            return data;
        }
    }
}
