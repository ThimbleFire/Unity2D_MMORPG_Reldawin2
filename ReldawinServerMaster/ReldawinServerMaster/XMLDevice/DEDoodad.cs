using System;
using System.Xml.Serialization;

namespace ReldawinServerMaster
{
    [XmlRoot( "Doodad" )]
    public class DEDoodad
    {
        public string name { get; set; }
        public int id { get; set; }

        [XmlArray( "Droprate" )]
        public Droprate[] droprates;

        [NonSerialized]
        Probability probability = new Probability();

        public void Setup()
        {
            if ( droprates == null )
            {
                Console.WriteLine( "[TETile] Droprates belonging to DEDoodad with id " + id + " is NULL" );
                return;
            }

            foreach ( Droprate spawn in droprates )
            {
                probability.Add( spawn.id, spawn.percent );
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
}