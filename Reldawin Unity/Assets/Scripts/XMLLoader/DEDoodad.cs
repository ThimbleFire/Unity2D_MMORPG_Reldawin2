using System.Xml.Serialization;

[XmlRoot( "Doodad" )]
public class DEDoodad
{
    public string name { get; set; }
    public int id { get; set; }

    [XmlArray( "ItemYields" )]
    public Yield[] _yieldProbabilities;
}