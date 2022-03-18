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
        public static List<AEAtlas> atlas = new List<AEAtlas>();

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
        public static string GetAtlas(int tileType, Tile[] neighbours)
        {
            //Create a copy of the tiles and their states (<short>s)
            List<AEAtlas> tempAtlas = new List<AEAtlas>( atlas );

            //Go through all the states
            for ( int i = 0; i < 8; i++ )
            {
                //Go through each atlas
                foreach ( AEAtlas a in atlas )
                {
                    //If the atlas' short is 'cannot connect'...
                    if ( a.state[i] == 2 )
                    {
                        //if the neighbour's tile type is equal to the tile being assessed.
                        //basically if the neighbour says it can't connect and the adjacent tile is of the same type..
                        //it should connect, but the neighbour's state says it doesn't, so remove it
                        if ( neighbours[i].TileType == tileType )
                        {
                            tempAtlas.Remove( a );
                            continue;
                        }
                    }
                }
            }

            //once all adjacent tiles that can't connect and are of the same type are removed, return the first atlas' name
            //this'll be something like _0, _1, _2 etc, which is the auto generated part of the tile name in the sprite texture 
            return tempAtlas[0].name;
        }

        public static void Setup()
        {
            LoadTiles();
            LoadDoodads();
            LoadItems();
            LoadAtlas();
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


        private static void LoadAtlas ()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( AEAtlasList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/atlas.xml", FileMode.Open );
            AEAtlasList aeatlaslist = serializer.Deserialize( stream ) as AEAtlasList;
            atlas = aeatlaslist.list;
            stream.Close();
        }
    }
}