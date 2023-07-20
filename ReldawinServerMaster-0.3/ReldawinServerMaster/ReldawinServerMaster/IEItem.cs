using System;
using System.Xml.Serialization;

namespace ReldawinServerMaster
{
    [Serializable, XmlRoot( "Item" )]
    public class IEItem
    {
        public string name { get; set; }
        public int id { get; set; }
        public string itemSpriteFileName16x16 { get; set; }
        public string itemSpriteFileName32x32 { get; set; }
        public string itemSpriteOnFloorFileName { get; set; }
        public string flavourText { get; set; }
    }
}