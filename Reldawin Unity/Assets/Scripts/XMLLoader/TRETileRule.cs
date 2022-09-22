using UnityEngine;
using System.Xml.Serialization;

[XmlRoot( "Tile" )]
public class TRETileRule
{
    public string name;
    public short[] state;

    public TRETileRule()
    {
        state = new short[8]
        {
            0, 0, 0, 0, 0, 0, 0, 0
        };
    }
}