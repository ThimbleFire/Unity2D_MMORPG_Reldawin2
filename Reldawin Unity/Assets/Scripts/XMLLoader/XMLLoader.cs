using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class XMLLoader
    {
        public static Dictionary<int, TETile> tileinfo = new Dictionary<int, TETile>();
        public static Dictionary<int, DEDoodad> doodadinfo = new Dictionary<int, DEDoodad>();
        public static Dictionary<int, IEItem> iteminfo = new Dictionary<int, IEItem>();

        public static TETile GetTile( int key )
        {
            if ( tileinfo.ContainsKey( key ) )
            {
                return tileinfo[key];
            }
            else
            {
                Debug.LogError( key + " isn't in the XMLLoader tileinfo dictionary" );
                return tileinfo[0];
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
                Debug.LogError( key + " isn't in the XMLLoader doodadinfo dictionary" );
                return doodadinfo[0];
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
                Debug.LogError( key + " isn't in the XMLLoader iteminfo dictionary" );
                return iteminfo[0];
            }
        }

        public static void Setup()
        {
            LoadTiles();
            LoadDoodads();
            LoadItems();
        }

        private static void LoadTiles()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( TETileList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/tiles.xml", FileMode.Open );
            var tileList = serializer.Deserialize( stream ) as TETileList;
            stream.Close();

            foreach ( TETile tile in tileList.list ) {
                tileinfo.Add( tile.id, tile );
            }
        }

        private static void LoadItems()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Open );
            var itemList = serializer.Deserialize( stream ) as IEItemList;
            stream.Close();

            foreach ( IEItem item in itemList.list ) {
                iteminfo.Add( item.id, item );
            }
        }

        private static void LoadDoodads()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/doodads.xml", FileMode.Open );
            var doodadList = serializer.Deserialize( stream ) as DEDoodadList;
            stream.Close();

            foreach ( DEDoodad doodad in doodadList.list ) {
                doodadinfo.Add( doodad.id, doodad );
            }
        }
    }
}