using System.Xml.Serialization;

[XmlRoot( "DoodadSpawns" )]
public class Spawns
{
    public int id { get; set; }
    public int rate { get; set; }
}
