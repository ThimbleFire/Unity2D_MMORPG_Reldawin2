using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace ReldawinServerMaster
{
    class DBIntegrityCheck
    {
        public static void MapDefaults()
        {
            //SQLReader.RunQuery( @"CREATE TABLE IF NOT EXISTS mapdefaults(
            //                        Seed        int,
            //                        Width       int,
            //                        Height      int,
            //                        Scale       double,
            //                        RNGSeed     int);", out bool executed );

            //if ( CommonSQL.CountRows( "mapdefaults" ) == 0 )
            //{
            //    Random rand = new Random();

            //    SQLReader.RunQuery( @"INSERT INTO mapdefaults (seed, width, height, scale, rngseed ) VALUES ( 1596423667, 64, 64, 0.5, " + rand.Next( int.MinValue, int.MaxValue ) + " )", out executed );
            //}
        }

        public static void Entities()
        {
            //SQLReader.RunQuery( @"CREATE TABLE IF NOT EXISTS entities(
            //                        ID            int AUTO_INCREMENT PRIMARY KEY,
            //                        OwnerID       int,
            //                        CellPosX      int,
            //                        CellPosY      int);", out bool executed );
        }

        public static void Players()
        {
            //SQLReader.RunQuery( @"CREATE TABLE IF NOT EXISTS players(
            //                        ID            int AUTO_INCREMENT PRIMARY KEY,
            //                        Username      text,
            //                        Password      text);", out bool executed );
        }

        public static void Terrain()
        {
            //SQLReader.RunQuery( @"CREATE TABLE IF NOT EXISTS terrain(
            //                        TileX            int,
            //                        TileY            int,
            //                        Type             int);", out bool executed );
        }

        public static void Doodads()
        {
            //SQLReader.RunQuery( @"CREATE TABLE IF NOT EXISTS doodads(
            //                        TileX            int,
            //                        TileY            int,
            //                        Type             int);", out bool executed );
        }
    }
}