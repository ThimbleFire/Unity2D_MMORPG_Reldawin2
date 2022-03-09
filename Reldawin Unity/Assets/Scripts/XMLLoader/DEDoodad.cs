using System.Xml.Serialization;

[XmlRoot( "Doodad" )]
public class DEDoodad
{
    public string name { get; set; }
    public int id { get; set; }

    [XmlArray( "Droprate" )]
    public Droprate[] droprates;
}
