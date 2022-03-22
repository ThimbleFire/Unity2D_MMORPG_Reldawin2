using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot( "Atlas" )]
public class TRETileRuleList
{
    [XmlArray( "List" )]
    public List<TRETileRule> list = new List<TRETileRule>();

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