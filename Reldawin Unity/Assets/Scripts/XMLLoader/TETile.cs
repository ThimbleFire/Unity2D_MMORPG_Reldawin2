
using System.Xml.Serialization;

[XmlRoot( "Tile" )]
public class TETile
{
    public string name { get; set; }
    public byte id { get; set; }
    public float minHeight { get; set; }
    public float maxHeight { get; set; }
    public int layerIndex { get; set; }

    [XmlArray( "Droprates" )]
    public Droprate[] droprates;
}
