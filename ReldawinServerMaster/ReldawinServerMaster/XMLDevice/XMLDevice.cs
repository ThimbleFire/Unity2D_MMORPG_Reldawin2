using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace ReldawinServerMaster
{
    class XMLDevice
    {
        public static Dictionary<int, TETile> tileinfo = new Dictionary<int, TETile>();
        public static Dictionary<int, DEDoodad> doodadinfo = new Dictionary<int, DEDoodad>();
        public static Dictionary<int, IEItem> iteminfo = new Dictionary<int, IEItem>();

        private static TETileList tileList;
        private static IEItemList itemList;
        private static DEDoodadList doodadList;

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
            bool result = false;

            result = LoadTiles();

            if ( result )
            {
                Console.WriteLine( "[XMLDevice] Loading " + tileList.list.Count + " tiles." );
                foreach ( TETile tile in tileList.list )
                {
                    tileinfo.Add( tile.id, tile );

                    tile.Setup();
                }
            }

            result = LoadItems();

            if ( result )
            {
                Console.WriteLine( "[XMLDevice] Loading " + tileList.list.Count + " doodads." );
                foreach ( DEDoodad doodad in doodadList.list )
                {
                    doodadinfo.Add( doodad.id, doodad );

                    doodad.Setup();
                }
            }

            result = LoadDoodads();

            if ( result )
            {
                Console.WriteLine( "[XMLDevice] Loading " + tileList.list.Count + " doodads." );
                foreach ( DEDoodad doodad in doodadList.list )
                {
                    doodadinfo.Add( doodad.id, doodad );

                    doodad.Setup();
                }
            }
        }

        private static bool LoadTiles()
        {
            if ( File.Exists( "tiles.xml" ) )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( TETileList ) );
                FileStream stream = new FileStream( "tiles.xml", FileMode.Open );
                tileList = serializer.Deserialize( stream ) as TETileList;
                stream.Close();
                return true;
            }
            else
            {
                Debug.LogWarning( "[XMLDevice] tiles.xml could not be found." );
                return false;
            }
        }
        private static bool LoadItems()
        {
            if ( File.Exists( "items.xml" ) )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
                FileStream stream = new FileStream( "items.xml", FileMode.Open );
                itemList = serializer.Deserialize( stream ) as IEItemList;
                stream.Close();
                return true;
            }
            else
            {
                Debug.LogWarning( "[XMLDevice] items.xml could not be found." );
                return false;
            }
        }
        private static bool LoadDoodads()
        {
            if ( File.Exists( "doodads.xml" ) )
            {
                XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
                FileStream stream = new FileStream( "doodads.xml", FileMode.Open );
                doodadList = serializer.Deserialize( stream ) as DEDoodadList;
                stream.Close();
                return true;
            }
            else
            {
                Debug.LogWarning( "[XMLDevice] doodads.xml could not be found." );
                return false;
            }
        }
    }
}
