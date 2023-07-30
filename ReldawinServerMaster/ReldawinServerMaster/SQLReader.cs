using SQLite;
using System.Drawing;
using System.Net.Sockets;

namespace ReldawinServerMaster
{
    internal class SQLReader
    {
        private static SQLiteConnection _connection;

        public static void CreateAccount( string username, string password ) {
            UserCredentials userCredentials = new UserCredentials() {
                Username = username,
                Password = password
            };
            _connection.Insert( userCredentials );
        }

        public static void CreateEntity( int x, int y, int ID ) {
            EntityCoordinates entityCoordinates = new EntityCoordinates() {
                ID = ID,
                CoordinateX = x,
                CoordinateY = y
            };
            _connection.Insert( entityCoordinates );
        }

        public static Vector2Int GetEntityCoordinates( int ID ) {
            EntityCoordinates entityCoordinates = _connection.Table<EntityCoordinates>().Where(v=>v.ID == ID).FirstOrDefault();
            return new Vector2Int( entityCoordinates.CoordinateX, entityCoordinates.CoordinateY );
        }

        public static object GetEntityId( string username ) {
            return _connection.Table<UserCredentials>().Where( v => v.Username == username ).FirstOrDefault()?.ID;
        }

        public static UserCredentials GetPlayerIDAndPassword( string username, string password ) {
            return _connection.Table<UserCredentials>().Where( v => v.Username == username && v.Password == password ).FirstOrDefault();
        }

        public static SceneObjects[] GetSceneObjectsInChunk(int chunkX, int chunkY) {

            int xMin = chunkX * 20;
            int xMax = xMin + 20;
            int yMin = chunkY * 20;
            int yMax = yMin + 20;

            return _connection.Table<SceneObjects>().Where(v=> v.CoordinateX >= xMin && v.CoordinateX < xMax &&
                                                               v.CoordinateY >= yMin && v.CoordinateY < yMax
                                                          ).ToArray();
        }
        public static void PopulateServerObjects( int chunkX, int chunkY ) {
            Console.WriteLine( $"Populating scene assets chunk {chunkX}, {chunkY}" );
            string data = MapData.GetChunk( chunkX, chunkY );
            int iteration = 0;
            Random rand = new Random();
            for( int y = 0; y < 20; y++ )
            for( int x = 0; x < 20; x++ ) {
                switch( data[iteration] ) {
                    case '1': //grass
                            if( rand.Next( 400 ) < 1 ) {
                                SceneObjects sceneObjects = new SceneObjects()
                                {
                                    CoordinateX = x + 20 * chunkX,
                                    CoordinateY = y + 20 * chunkY,
                                    Type = rand.Next(6)
                                };
                                _connection.Insert( sceneObjects );
                            } 
                            else if( rand.Next( 800 ) < 1 ) {
                                SceneObjects sceneObjects = new SceneObjects()
                                {
                                    CoordinateX = x + 20 * chunkX,
                                    CoordinateY = y + 20 * chunkY,
                                    Type = rand.Next(6)
                                };
                                _connection.Insert( sceneObjects );
                            }
                            break;
                        case '3': //dirt
                        if( rand.Next( 400 ) < 1 ) {
                            SceneObjects sceneObjects = new SceneObjects()
                                {
                                CoordinateX = x + 20 * chunkX,
                                CoordinateY = y + 20 * chunkY,
                                Type = rand.Next(6)
                            };
                            _connection.Insert( sceneObjects );
                        }
                        break;
                    case '4': //forest
                        if( rand.Next( 400 ) < 3 ) {
                            SceneObjects sceneObjects = new SceneObjects()
                                {
                                CoordinateX = x + 20 * chunkX,
                                CoordinateY = y + 20 * chunkY,
                                Type = rand.Next(6)
                            };
                            _connection.Insert( sceneObjects );
                        }
                        break;
                    default:
                        break;
                }
                iteration++;

            }
        }

        public static void SetEntityCoordinates( int x, int y, int ID ) {
            EntityCoordinates entityCoordinates = _connection.Table<EntityCoordinates>().Where(v=>v.ID == ID).FirstOrDefault();
            entityCoordinates.CoordinateX = x;
            entityCoordinates.CoordinateY = y;
            _connection.Update( entityCoordinates );
        }

        internal static void Setup() {
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");

            _connection = new SQLiteConnection( databasePath );

            _connection.CreateTable<UserCredentials>();
            _connection.CreateTable<EntityCoordinates>();
            _connection.CreateTable<MapTiles>();
            _connection.CreateTable<MapReservoirs>();
            _connection.CreateTable<MapOreVeins>();
            _connection.CreateTable<Item>();
            CreateTableResult result = _connection.CreateTable<SceneObjects>();

            if( result == CreateTableResult.Created ) {
                for( int y = 0; y < 50; y++ ) {
                    for( int x = 0; x < 50; x++ ) {
                        SQLReader.PopulateServerObjects( x, y );
                    }
                }
            }

        }

        internal static void Shutdown() {
            _connection.Close();
        }
    }
}