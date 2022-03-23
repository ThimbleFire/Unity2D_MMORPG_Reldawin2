using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class XMLLoader
    {
        public static Dictionary<int, TETile> Tile = new Dictionary<int, TETile>();
        public static Dictionary<int, DEDoodad> Doodad = new Dictionary<int, DEDoodad>();
        public static Dictionary<int, IEItem> Item = new Dictionary<int, IEItem>();

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
                Tile.Add( tile.id, tile );
            }
        }

        private static void LoadItems()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Open );
            var itemList = serializer.Deserialize( stream ) as IEItemList;
            stream.Close();

            foreach ( IEItem item in itemList.list ) {
                Item.Add( item.id, item );
            }
        }

        private static void LoadDoodads()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/doodads.xml", FileMode.Open );
            var doodadList = serializer.Deserialize( stream ) as DEDoodadList;
            stream.Close();

            foreach ( DEDoodad doodad in doodadList.list ) {
                Doodad.Add( doodad.id, doodad );
            }
        }
    }
}
