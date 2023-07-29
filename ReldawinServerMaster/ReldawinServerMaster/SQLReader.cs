using SQLite;
using System.Drawing;

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

        public static void GetPlayerIDAndPassword( string username, out string password, out int id ) {
            UserCredentials userCredentials = _connection.Table<UserCredentials>().Where( v => v.Username == username ).FirstOrDefault();
            password = userCredentials.Password;
            id = userCredentials.ID;
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
        }

        internal static void Shutdown() {
            _connection.Close();
        }
    }
}