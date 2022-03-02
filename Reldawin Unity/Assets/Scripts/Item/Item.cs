using System;
using UnityEngine;
using UnityEngine.UI;

namespace LowCloud.Reldawin
{
    public class Item : MonoBehaviour
    {
        public enum Type
        {
            Log, 
            Grass, 
            Stone, 
            Flint, 
            Knife
        }

        private UI_Slot uiSlot;
        string _itemName = string.Empty;
        string _itemSpriteFileName16x16 = string.Empty;
        string _itemSpriteFileName32x32 = string.Empty;
        string _itemSpriteOnFloorFileName = string.Empty;
        Sprite _sprite16;
        Sprite _sprite32;
        Sprite _spriteOnFloor;
        bool _stackable;
        bool _equipment;
        int _equipment_selected;
        bool _equipment_ranged;
        int _equipment_max_sockets;
        int _equipment_minDamDef, _equipment_maxDamDef;
        int _equipment_attack_speed;
        bool _craftable;
        string _flavourText;
        int _equipment_material_selected;
        int _ID;
        string _binary;

        public void Setup( UI_Slot ui_slot )
        {
            uiSlot = ui_slot;
        }

        public void Build( int itemID )
        {
            _ID =                                              itemID;
            _itemName =                     XMLLoader.iteminfo[itemID].name;
            _itemSpriteFileName16x16 =      XMLLoader.iteminfo[itemID].itemSpriteFileName16x16;
            _itemSpriteFileName32x32 =      XMLLoader.iteminfo[itemID].itemSpriteFileName32x32;
            _itemSpriteOnFloorFileName =    XMLLoader.iteminfo[itemID].itemSpriteOnFloorFileName;
            _flavourText =                  XMLLoader.iteminfo[itemID].flavourText;

            GetComponent<Image>().sprite = SpriteLoader.itemDictionary[_itemSpriteFileName32x32];
        }
    }
}