using UnityEngine;
using System.Xml.Serialization;

public class AEAtlas
{
    public string name;
    public short[] state;

    public AEAtlas()
    {
        state = new short[9]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0
        };
    }
}
