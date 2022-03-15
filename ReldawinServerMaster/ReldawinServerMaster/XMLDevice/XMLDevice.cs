using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace ReldawinServerMaster
{
    class XMLDevice
    {
        private static Dictionary<int, TETile> tileinfo = new Dictionary<int, TETile>();
        private static Dictionary<int, DEDoodad> doodadinfo = new Dictionary<int, DEDoodad>();
        private static Dictionary<int, IEItem> iteminfo = new Dictionary<int, IEItem>();

        public static int GetTileCount { get { return tileinfo.Count; } }
        public static int GetDoodadCount { get { return doodadinfo.Count; } }
        public static int GetItemCount { get { return iteminfo.Count; } }

        public static TETile GetTile( int key )
        {
            if ( tileinfo.ContainsKey( key ) )
            {
                return tileinfo[key];
            }
            else
            {
                Console.WriteLine( key + " isn't in the XMLLoader tileinfo dictionary" );
                return null;
            }
        }
        public static DEDoodad GetDoodad( int key )
        {
            if ( doodadinfo.ContainsKey( key ) )
            {
                return doodadinfo[key];
            }
            else
            {
                Console.WriteLine( key + " isn't in the XMLLoader doodadinfo dictionary" );
                return null;
            }
        }
        public static IEItem GetItem( int key )
        {
            if ( iteminfo.ContainsKey( key ) )
            {
                return iteminfo[key];
            }
            else
            {
                Console.WriteLine( key + " isn't in the XMLLoader iteminfo dictionary" );
                return null;
            }
        }

        public static void Setup()
        {
            TETileList tileList = Load<TETileList>( "tiles.xml" );
            foreach ( TETile tile in tileList.list )
            {
                tileinfo.Add( tile.id, tile );
                tile.Setup();
            }

            IEItemList itemList = Load<IEItemList>( "items.xml" );
            foreach ( IEItem item in itemList.list )
            {
                iteminfo.Add( item.id, item );
            }

            DEDoodadList doodadList = Load<DEDoodadList>( "doodads.xml" );
            foreach ( DEDoodad doodad in doodadList.list )
            {
                doodadinfo.Add( doodad.id, doodad );
                doodad.Setup();
            }
        }

        /// <summary>Called by Load in inheriting class</summary>
        private static T Load<T>( string filename )
        {
            XmlSerializer xmlSerializer = new XmlSerializer( typeof( T ) );
            TextReader reader = new StreamReader( filename );
            return (T)xmlSerializer.Deserialize( reader );
        }
    }
}
