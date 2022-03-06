using System.Xml.Serialization;

[XmlRoot( "Droprate" )]
public class Droprate
{
    public int id { get; set; }
    public int percent { get; set; }
}
