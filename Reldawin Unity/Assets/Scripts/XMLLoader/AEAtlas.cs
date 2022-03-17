using UnityEngine;
using System.Xml.Serialization;

[XmlRoot( "Tile" )]
public class AEAtlas
{
    public string name;
    public short[] state;

    public AEAtlas()
    {
        state = new short[8]
        {
            0, 0, 0, 0, 0, 0, 0, 0
        };
    }
}
