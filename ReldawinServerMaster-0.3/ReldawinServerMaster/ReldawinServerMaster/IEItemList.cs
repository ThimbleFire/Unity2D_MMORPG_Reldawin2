using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReldawinServerMaster
{
    [Serializable, XmlRoot( "Items" )]
    public class IEItemList
    {
        public List<IEItem> list = new List<IEItem>();

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