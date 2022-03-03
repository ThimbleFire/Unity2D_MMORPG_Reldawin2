using System.Xml.Serialization;

[XmlRoot( "ItemYield" )]
public class Yield
{
    public int id { get; set; }
    public int rate { get; set; }
}