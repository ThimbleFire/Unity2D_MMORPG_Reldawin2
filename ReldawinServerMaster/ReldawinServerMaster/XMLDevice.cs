using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace ReldawinServerMaster
{
    class XMLDevice
    {
        public static Dictionary<byte, TETile> tileinfo = new Dictionary<byte, TETile>();
        public static Dictionary<int, DEDoodad> doodadinfo = new Dictionary<int, DEDoodad>();

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

                tile.Setup();
            }

            foreach ( DEDoodad doodad in doodadList.list )
            {
                doodadinfo.Add( doodad.id, doodad );

                doodad.Setup();
            }
        }

        private static void LoadTiles()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( TETileList ) );
            FileStream stream = new FileStream( "tiles.xml", FileMode.Open );
            tileList = serializer.Deserialize( stream ) as TETileList;
            stream.Close();
        }

        private static void LoadItems()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
            FileStream stream = new FileStream( "items.xml", FileMode.Open );
            itemList = serializer.Deserialize( stream ) as IEItemList;
            stream.Close();
        }

        private static void LoadDoodads()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
            FileStream stream = new FileStream( "doodads.xml", FileMode.Open );
            doodadList = serializer.Deserialize( stream ) as DEDoodadList;
            stream.Close();
        }
    }

    [XmlRoot( "DoodadSpawns" )]
    public class Spawns
    {
        public int id { get; set; }
        public int rate { get; set; }
    }

    [XmlRoot( "Tile" )]
    public class TETile
    {
        public string name { get; set; }
        public byte id { get; set; }
        public float minHeight { get; set; }
        public float maxHeight { get; set; }
        public int layerIndex { get; set; }

        [XmlArray( "DoodadSpawnRatesOnTile" )]
        public Spawns[] _doodadSpawnsAndProbabilities;

        [NonSerialized]
        Probability doodadRoller = new Probability();

        public void Setup()
        {
            foreach ( Spawns spawn in _doodadSpawnsAndProbabilities )
            {
                doodadRoller.Add( spawn.id, spawn.rate );
            }
        }

        public object GetDoodad
        {
            get
            {
                object doodadID = doodadRoller.Roll();

                if ( doodadID == null )
                    return null;
                else
                    return new Doodad( Convert.ToInt32( doodadID ) );
            }
        }
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
        public int rate { get; set; }
    }

    [XmlRoot( "Doodad" )]
    public class DEDoodad
    {
        public string name { get; set; }
        public int id { get; set; }

        [XmlArray( "ItemYields" )]
        public Yield[] _doodadSpawnsAndProbabilities;

        [NonSerialized]
        Probability probability = new Probability();

        public void Setup()
        {
            foreach ( Yield spawn in _doodadSpawnsAndProbabilities )
            {
                probability.Add( spawn.id, spawn.rate );
            }
        }

        public int GetYieldRoll
        {
            get
            {
                object result = probability.Roll();

                if ( result == null )
                    return -1;
                else
                    return Convert.ToInt32( result );
            }
        }
    }

    [XmlRoot( "Doodads" )]
    public class DEDoodadList
    {
        public List<DEDoodad> list = new List<DEDoodad>();
    }
}
