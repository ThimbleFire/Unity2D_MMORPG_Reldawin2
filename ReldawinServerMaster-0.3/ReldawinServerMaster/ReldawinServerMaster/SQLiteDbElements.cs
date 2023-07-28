using Org.BouncyCastle.Asn1;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReldawinServerMaster
{
    public class UserCredentials
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class EntityCoordinates
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
    }

    public class MapTiles
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public byte Type { get; set; }
    }
    public class MapReservoirs
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
    }
    public class MapOreVeins
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public byte Type { get; set; }
    }
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int OwnerID { get; set; }
        public string Binary { get; set; }
    }
}
