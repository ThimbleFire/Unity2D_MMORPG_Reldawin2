
using System;
using System.Xml.Serialization;

namespace ReldawinServerMaster
{
    [XmlRoot( "Tile" )]
    public class TETile
    {
        public string name { get; set; }
        public int id { get; set; }
        public float minHeight { get; set; }
        public float maxHeight { get; set; }
        public int layerIndex { get; set; }

        [XmlArray( "Droprates" )]
        public Droprate[] droprates;

        [NonSerialized]
        Probability probability = new Probability();

        public void Setup()
        {
            if(droprates == null)
            {
                Console.WriteLine( "[TETile] Droprates belonging to TETile with id " + id + " is NULL" );
                return;
            }    

            foreach ( Droprate spawn in droprates )
            {
                probability.Add( spawn.id, spawn.percent );
            }
        }

        public object GetDoodad
        {
            get
            {
                object doodadID = probability.Roll();

                if ( doodadID == null )
                    return null;
                else
                    return new Doodad( Convert.ToInt32( doodadID ) );
            }
        }
    }
}