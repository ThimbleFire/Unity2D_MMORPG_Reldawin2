using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ReldawinServerMaster
{
    /// <summary>
    /// A class containing commonly used SQL queries
    /// </summary>
    internal class CommonSQL
    {
        /// Set the cell position of entity on the server.
        public static void SetEntityCoodinates( int x, int y, int ID )
        {
            SQLReader.RunQuery( string.Format( @"UPDATE entities 
                                                    SET CellPosX    = {0}, 
                                                        CellPosY    = {1} 
                                                  WHERE entities.ID = {2};"
                                                 , x
                                                 , y
                                                 , ID ), out bool executed );
        }

        /// Get the default values responsible for building the world prior to modification.
        public static object[] GetMapData()
        {
            List<object> result = SQLReader.RunQuery( @"SELECT * 
                                                          FROM MapDefaults ;"
                                                     
                                                   , out bool executed
                                                   , SQLReader.Stream.OUTPUT
                                                   , null
                                                   , "Seed"
                                                   , "Width"
                                                   , "Height"
                                                   , "Scale"
                                                   , "RNGSeed"
                                                   );
            return result.ToArray();
        }

        /// Get the coordinates of the entity with this ID.
        public static object[] GetEntityCoordinates( int ID )
        {
            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter( "?playerID", ID )
            };
            
            List<object> result = SQLReader.RunQuery( @"SELECT * 
                                                          FROM Entities 
                                                         WHERE OwnerID = ?playerID ;"
                                                     
                                                   , out bool executed
                                                   , SQLReader.Stream.OUTPUT
                                                   , parameters
                                                   , "CellPosX"
                                                   , "CellPosY"
                                                   );

            return result.ToArray();
        }

        /// Gets the Entity ID from the username
        public static object[] GetEntityID( string username )
        {
            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter( "?username", username )
            };
            
            List<object> result = SQLReader.RunQuery( @"SELECT *
                                                          FROM Players 
                                                         WHERE Username = ?username ;"
                                                     
                                                   , out bool executed
                                                   , SQLReader.Stream.OUTPUT
                                                   , parameters
                                                   , "ID"
                                                   );

            if ( result == null )
            {
                return null;
            }

            return result.ToArray();
        }

        /// Gets the Player Password and ID from the username
        public static object[] GetPlayerIDAndPassword( string username )
        {
            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter( "?username", username )
            };

            List<object> result = SQLReader.RunQuery( @"SELECT * 
                                                          FROM Players 
                                                         WHERE Username = ?username ;"
                                                     
                                                   , out bool executed
                                                   , SQLReader.Stream.OUTPUT
                                                   , parameters
                                                   , "Password"
                                                   , "ID"
                                                   );

            return result.ToArray();
        }

        /// Create the account under the parameters provided
        public static void CreateAccount( string username, string password )
        {
            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter( "?username", username ),
                new SQLReader.Parameter( "?password", password )
            };
            
            SQLReader.RunQuery( "INSERT INTO players ( username, password ) VALUES( ?username, ?password )"
                               
                               , out bool executed
                               , SQLReader.Stream.INPUT
                               , parameters );
        }

        /// An entity is the base component of all physical objects in the game world
        public static void CreateEntity( int x, int y, int ID )
        {
            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter( "?cellposx", x ),
                new SQLReader.Parameter( "?cellposy", y ),
                new SQLReader.Parameter( "?ownerid", ID )
            };
            
            SQLReader.RunQuery( "INSERT INTO entities ( cellposx, cellposy, ownerid ) VALUES( ?cellposx, ?cellposy, ?ownerid )"
                               
                               , out bool executed
                               , SQLReader.Stream.INPUT
                               , parameters );
        }

        public static void InsertDoodad(Doodad doodad)
        {
            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter("?TileX", doodad.tileX),
                new SQLReader.Parameter("?TileY", doodad.tileY),
                new SQLReader.Parameter("?Type", doodad.type)
            };

            SQLReader.RunQuery( "INSERT INTO doodads ( TileX, TileY, Type ) VALUES ( ?TileX, ?TileY, ?Type )"

                        , out bool executed
                        , SQLReader.Stream.INPUT
                        , parameters );
        }

        /// Returns all tile types in the database between the coordinates of x and xLim and y and yLim
        public static List<object> GetChunkTiles( int chunkX, int chunkY )
        {
            int x = chunkX * 15;
            int y = chunkY * 15;
            int xLim = x + 17;
            int yLim = y + 17;

            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter("?x", x),
                new SQLReader.Parameter( "?xLim", xLim ),
                new SQLReader.Parameter( "?y", y ),
                new SQLReader.Parameter( "?yLim", yLim )
            };

            List<object> result = SQLReader.RunQuery( @"SELECT * 
                                                          FROM terrain 
                                                         WHERE TileX >= ?x AND TileX < ?xLim 
                                                           AND TileY >= ?y AND TileY < ?yLim
                                                      
                                                      ;"
                                                     
                                                    , out bool executed
                                                    , SQLReader.Stream.OUTPUT
                                                    , parameters
                                                    , "Type"
                                                    );
           
            return result;
        }

        public static List<object> GetDoodad( int tileX, int tileY )
        {
            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter("?x", tileX),
                new SQLReader.Parameter( "?y", tileY )
            };

            List<object> result = SQLReader.RunQuery( @"SELECT * 
                                                          FROM doodads 
                                                         WHERE TileX = ?x
                                                           AND TileY = ?y
                                                      
                                                      ;"

                                                    , out bool executed
                                                    , SQLReader.Stream.OUTPUT
                                                    , parameters
                                                    , "Type"
                                                    );

            return result;
        }

        public static List<object> GetChunkDoodads(int chunkX, int chunkY)
        {
            int x = chunkX * 15;
            int y = chunkY * 15;
            int xLim = x + 15;
            int yLim = y + 15;

            SQLReader.Parameter[] parameters = new SQLReader.Parameter[] {
                new SQLReader.Parameter("?x", x),
                new SQLReader.Parameter( "?xLim", xLim ),
                new SQLReader.Parameter( "?y", y ),
                new SQLReader.Parameter( "?yLim", yLim )
            };

            List<object> result = SQLReader.RunQuery( @"SELECT * 
                                                          FROM doodads 
                                                         WHERE TileX >= ?x AND TileX < ?xLim 
                                                           AND TileY >= ?y AND TileY < ?yLim
                                                      
                                                      ;"

                                                    , out bool executed
                                                    , SQLReader.Stream.OUTPUT
                                                    , parameters
                                                    , "Type"
                                                    , "TileX"
                                                    , "TileY"
                                                    );

            return result;
        }

        public static void EmptyTable( string tableName )
        {
            SQLReader.RunQuery( "DELETE FROM " + tableName
                               
                               , out bool executed
                               , SQLReader.Stream.OUTPUT );
        }

        public static int CountRows(string tableName)
        {
            int count = int.Parse(SQLReader.RunQueryScalar( "SELECT COUNT(*) FROM " + tableName ).ToString());

            return count;
        }
        
        ///<summary>
        /// This might cause errors since we're not specifying search parameters / columns.
        ///</summary>
        public static bool HasRows(string tableName)
        {
            List<object> result = SQLReader.RunQuery("SELECT * FROM " + tableName + " LIMIT 1", out bool executed, SQLReader.Stream.OUTPUT);
        
            return result != null;
        }
    }
}
