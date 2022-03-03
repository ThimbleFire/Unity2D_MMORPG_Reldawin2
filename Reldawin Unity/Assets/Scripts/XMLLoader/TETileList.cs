using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot( "Tiles" )]
public class TETileList
{
    public List<TETile> list = new List<TETile>();

    public string[] GetNames
    {
        get
        {
            if ( list == null )
                return new string[0];

            string[] names = new string[list.Count];

            for ( int i = 0; i < list.Count; i++ )
            {
                names[i] = list[i].name;
            }

            return names;
        }
    }
}