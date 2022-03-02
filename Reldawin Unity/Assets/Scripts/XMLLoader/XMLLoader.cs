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

        private static TETileList tileList;
        private static IEItemList itemList;
        private static DEDoodadList doodadList;

        public static void Setup()
        {
            LoadTiles();
            LoadItems();
            LoadDoodads();

            foreach ( TETile tile in tileList.list )
            {
                tileinfo.Add( tile.id, tile );
            }

            foreach ( DEDoodad doodad in doodadList.list )
            {
                doodadinfo.Add( doodad.id, doodad );
            }

            foreach ( IEItem item in itemList.list )
            {
                iteminfo.Add( item.id , item );
            }
        }

        private static void LoadTiles()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( TETileList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/tiles.xml", FileMode.Open );
            tileList = serializer.Deserialize( stream ) as TETileList;
            stream.Close();
        }

        private static void LoadItems()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Open );
            itemList = serializer.Deserialize( stream ) as IEItemList;
            stream.Close();
        }

        private static void LoadDoodads()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/doodads.xml", FileMode.Open );
            doodadList = serializer.Deserialize( stream ) as DEDoodadList;
            stream.Close();
        }
    }

    [XmlRoot( "DoodadSpawns" )]
    public class Spawns
    {
        public int id { get; set; }
        public double rate { get; set; }
    }

    [XmlRoot( "Tile" )]
    public class TETile
    {
        public string name { get; set; }
        public int id { get; set; }
        public float minHeight { get; set; }
        public float maxHeight { get; set; }
        public int layerIndex { get; set; }

        [XmlArray( "DoodadSpawnRatesOnTile" )]
        public Spawns[] _doodadSpawnsAndProbabilities;
    }

    [XmlRoot( "Tiles" )]
    public class TETileList
    {
        public List<TETile> list = new List<TETile>();
    }

    [Serializable, XmlRoot( "Item" )]
    public class IEItem
    {
        public string name { get; set; }
        public int id { get; set; }
        public string itemSpriteFileName16x16 { get; set; }
        public string itemSpriteFileName32x32 { get; set; }
        public string itemSpriteOnFloorFileName { get; set; }
        public string flavourText { get; set; }
    }

    [Serializable, XmlRoot( "Items" )]
    public class IEItemList
    {
        public List<IEItem> list = new List<IEItem>();
    }

    [XmlRoot( "ItemYield" )]
    public class Yield
    {
        public string id { get; set; }
        public float rate { get; set; }
    }

    [XmlRoot( "Doodad" )]
    public class DEDoodad
    {
        public string name { get; set; }
        public int id { get; set; }

        [XmlArray( "ItemYields" )]
        public Yield[] _yieldProbabilities;
    }

    [XmlRoot( "Doodads" )]
    public class DEDoodadList
    {
        public List<DEDoodad> list = new List<DEDoodad>();
    }
}