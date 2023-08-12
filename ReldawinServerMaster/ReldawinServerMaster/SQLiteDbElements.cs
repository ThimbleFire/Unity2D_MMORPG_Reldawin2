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

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public byte identifier { get; set; }
        public byte value { get; set; }
        public byte dura { get; set; }
        public byte affix { get; set; }
        public byte prefix1 { get; set; }
        public byte prefix2 { get; set; }
        public byte prefix3 { get; set; }
        public byte suffix1 { get; set; }
        public byte suffix2 { get; set; }
        public byte suffix3 { get; set; }

        public int OwnerID { get; set; }
        public byte inventoryPos { get; set; }
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

    public class SceneObjects
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int Type { get; set; }
    }
}