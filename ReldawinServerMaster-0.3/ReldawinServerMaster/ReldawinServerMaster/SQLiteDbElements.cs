using SQLite;

namespace ReldawinServerMaster
{
    public class EntityCoordinates
    {
        public int CoordinateX { get; set; }

        public int CoordinateY { get; set; }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }

    public class Item
    {
        public string Binary { get; set; }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int OwnerID { get; set; }
    }

    public class MapOreVeins
    {
        public int CoordinateX { get; set; }

        public int CoordinateY { get; set; }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public byte Type { get; set; }
    }

    public class MapReservoirs
    {
        public int CoordinateX { get; set; }

        public int CoordinateY { get; set; }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }

    public class MapTiles
    {
        public int CoordinateX { get; set; }

        public int CoordinateY { get; set; }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public byte Type { get; set; }
    }

    public class UserCredentials
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Password { get; set; }
        public string Username { get; set; }
    }
}