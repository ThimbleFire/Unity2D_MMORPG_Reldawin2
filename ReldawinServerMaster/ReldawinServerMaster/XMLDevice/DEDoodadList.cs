using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReldawinServerMaster
{
    [XmlRoot( "Doodads" )]
    public class DEDoodadList
    {
        public List<DEDoodad> list = new List<DEDoodad>();

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
}