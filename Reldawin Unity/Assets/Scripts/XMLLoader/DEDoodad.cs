using System.Xml.Serialization;

[XmlRoot( "Doodad" )]
public class DEDoodad
{
    public enum Interact
    {
        NONE,
        WOODCUTTING,
        MINING,
        HANDS
    };

    public string name { get; set; }
    public int id { get; set; }
    public Interact interact { get; set; }

    [XmlArray( "Droprate" )]
    public Droprate[] droprates;
}
