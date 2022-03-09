using System.Xml.Serialization;

namespace ReldawinServerMaster
{
    [XmlRoot( "Droprate" )]
    public class Droprate
    {
        public int id { get; set; }
        public int percent { get; set; }
    }
}