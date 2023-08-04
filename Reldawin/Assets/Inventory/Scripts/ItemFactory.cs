using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ItemFactory
{
    public static byte value1;
    public static byte value2;
    public static byte durability;

    public enum Type : byte
    { 
        Any                     = 0b00000000,
        Head                    = 0b00000001,
        Chest                   = 0b00000010,
        Gloves                  = 0b00000011,
        Legs                    = 0b00000100,
        Feet                    = 0b00000101,
        Primary                 = 0b00000110,
        Secondary               = 0b00000111,
        Ring                    = 0b00001000,
        Neck                    = 0b00001001,
        Artifact                = 0b00001010,
        Misc                    = 0b00001011,
        Consumable              = 0b00001100,
        Quest                   = 0b00001101,
        Belt                    = 0b00001110
    }; 
    public enum Item_Sprite_Filename : byte {
        amulet1                 = 0b00000000,
        amulet2                 = 0b00000001,
        axe                     = 0b00000010,
        broadaxe                = 0b00000011,
        buckler                 = 0b00000100,
        cap                     = 0b00000101,
        chain                   = 0b00000110,
        chainboots              = 0b00000111,
        heavyboots              = 0b00001000,
        heavygloves             = 0b00001001,
        quiltedarmor            = 0b00001010,
        ring1                   = 0b00001011,
        ring2                   = 0b00001100,
        sash                    = 0b00001101,
        skullcap                = 0b00001110,
        smallshield             = 0b00001111,
        studdedleather          = 0b00010000
    };
    public enum Item_Prefix : byte
    {
        Nothing                 = 0b00000000, //0
        //Plus_Accuracy           = 0b00000001, //1
        Dmg_Phys_Min            = 0b00000010, //2
        Dmg_Phys_Max            = 0b00000011, //3
        //Dmg_Phys_Percent        = 0b00000100, //4
        Dmg_Ele_Fire            = 0b00000101, //5
        Dmg_Ele_Cold            = 0b00000110, //6
        Dmg_Ele_Lightning       = 0b00000111, //7
        Dmg_Ele_Poison          = 0b00001000, //8
        Def_Phys_Flat           = 0b00001001, //9
        //Def_Phys_Percent        = 0b00001010, //10
        Def_Dmg_Reduction_Phys  = 0b00001011, //11
        Def_Dmg_Reduction_Magic = 0b00001100, //12
        //Def_Dmg_Reduction_All   = 0b00001101, //13
        Def_Ele_Res_Fire        = 0b00001110, //14
        Def_Ele_Res_Cold        = 0b00001111, //15
        Def_Ele_Res_Lightning   = 0b00010000, //16
        Def_Ele_Res_Poison      = 0b00010001, //17
        Def_Ele_Res_All         = 0b00010010, //18
        //On_Hit_Life             = 0b00010011, //19
        //On_Kill_Life            = 0b00010100, //20
        //On_Hit_Mana             = 0b00010101, //21
        //On_Kill_Mana            = 0b00010110, //22
        Plus_Life               = 0b00010111, //23
        Plus_Mana               = 0b00011000, //24
        //Plus_Regen_Life         = 0b00011001, //25
        //Plus_Regen_Mana         = 0b00011010, //26
        Plus_Str                = 0b00011011, //27
        Plus_Dex                = 0b00011100, //28
        Plus_Con                = 0b00011101, //29
        Plus_Int                = 0b00011110, //30
        //Plus_Speed_Phys         = 0b00011111, //31
        //Plus_Speed_Magic        = 0b00100000, //32
        //Plus_Speed_Movement     = 0b00100001, //33
        //Plus_Block_Recovery     = 0b00100010, //34
        //Plus_Stagger_Recovery   = 0b00100011, //35
        Plus_Magic_Find         = 0b00100100, //36
    };
    public enum Item_Suffix : short
    {
        Nothing                 = 0b00000000, //0
        Plus_Accuracy           = 0b00000001, //1
        //Dmg_Phys_Min            = 0b00000010, //2
        //Dmg_Phys_Max            = 0b00000011, //3
        Dmg_Phys_Percent        = 0b00000100, //4
        //Dmg_Ele_Fire            = 0b00000101, //5
        //Dmg_Ele_Cold            = 0b00000110, //6
        //Dmg_Ele_Lightning       = 0b00000111, //7
        //Dmg_Ele_Poison          = 0b00001000, //8
        //Def_Phys_Flat           = 0b00001001, //9
        Def_Phys_Percent        = 0b00001010, //10
        //Def_Dmg_Reduction_Phys  = 0b00001011, //11
        //Def_Dmg_Reduction_Magic = 0b00001100, //12
        Def_Dmg_Reduction_All   = 0b00001101, //13
        //Def_Ele_Res_Fire        = 0b00001110, //14
        //Def_Ele_Res_Cold        = 0b00001111, //15
        //Def_Ele_Res_Lightning   = 0b00010000, //16
        //Def_Ele_Res_Poison      = 0b00010001, //17
        //Def_Ele_Res_All         = 0b00010010, //18
        On_Hit_Life             = 0b00010011, //19
        On_Kill_Life            = 0b00010100, //20
        On_Hit_Mana             = 0b00010101, //21
        On_Kill_Mana            = 0b00010110, //22
        //Plus_Life               = 0b00010111, //23
        //Plus_Mana               = 0b00011000, //24
        Plus_Regen_Life         = 0b00011001, //25
        Plus_Regen_Mana         = 0b00011010, //26
        //Plus_Str                = 0b00011011, //27
        //Plus_Dex                = 0b00011100, //28
        //Plus_Con                = 0b00011101, //29
        //Plus_Int                = 0b00011110, //30
        Plus_Speed_Phys         = 0b00011111, //31
        Plus_Speed_Magic        = 0b00100000, //32
        Plus_Speed_Movement     = 0b00100001, //33
        Plus_Block_Recovery     = 0b00100010, //34
        Plus_Stagger_Recovery   = 0b00100011, //35
        Plus_Magic_Find         = 0b00100100, //36
    };
    public enum Item_Implicit : byte
    {
        Nothing                 = 0b00000000, //0
        //Plus_Accuracy           = 0b00000001, //1
        Dmg_Phys_Min            = 0b00000010, //2
        Dmg_Phys_Max            = 0b00000011, //3
        //Dmg_Phys_Percent        = 0b00000100, //4
        Dmg_Ele_Fire            = 0b00000101, //5
        Dmg_Ele_Cold            = 0b00000110, //6
        Dmg_Ele_Lightning       = 0b00000111, //7
        Dmg_Ele_Poison          = 0b00001000, //8
        Def_Phys_Flat           = 0b00001001, //9
        //Def_Phys_Percent        = 0b00001010, //10
        Def_Dmg_Reduction_Phys  = 0b00001011, //11
        Def_Dmg_Reduction_Magic = 0b00001100, //12
        Def_Dmg_Reduction_All   = 0b00001101, //13
        //Def_Ele_Res_Fire        = 0b00001110, //14
        //Def_Ele_Res_Cold        = 0b00001111, //15
        //Def_Ele_Res_Lightning   = 0b00010000, //16
        //Def_Ele_Res_Poison      = 0b00010001, //17
        Def_Ele_Res_All         = 0b00010010, //18
        On_Hit_Life             = 0b00010011, //19
        //On_Kill_Life            = 0b00010100, //20
        On_Hit_Mana             = 0b00010101, //21
        //On_Kill_Mana            = 0b00010110, //22
        Plus_Life               = 0b00010111, //23
        Plus_Mana               = 0b00011000, //24
        Plus_Regen_Life         = 0b00011001, //25
        Plus_Regen_Mana         = 0b00011010, //26
        Plus_Str                = 0b00011011, //27
        Plus_Dex                = 0b00011100, //28
        Plus_Con                = 0b00011101, //29
        Plus_Int                = 0b00011110, //30
        Plus_Speed_Phys         = 0b00011111, //31
        Plus_Speed_Magic        = 0b00100000, //32
        Plus_Speed_Movement     = 0b00100001, //33
        Plus_Block_Recovery     = 0b00100010, //34
        Plus_Stagger_Recovery   = 0b00100011, //35
        Plus_Magic_Find         = 0b00100100, //36
    };
    //Requirements value cannot exceed 63.
    //Requirements value of zero will not appear in the item tooltip.
    [Flags] public enum Requirement : byte
    {
        Strength    = 0b00000000,
        Dex         = 0b01000000,
        Const       = 0b10000000,
        Int         = 0b11000000
    }; 

    public static void Build() {

        long itemBinaries1 = 0;
        itemBinaries1 += (long)  Type.Head                   << 0;
        itemBinaries1 += (long)  value1                      << 8;
        itemBinaries1 += (long)  value2                      << 16;
        itemBinaries1 += (long)  durability                  << 24;
        itemBinaries1 += (long)  Item_Sprite_Filename.cap    << 32;
        itemBinaries1 += (long)( Requirement.Strength + 11 ) << 40;
        itemBinaries1 += (long)( Requirement.Strength + 0 )  << 48;
        itemBinaries1 += (long)( Requirement.Strength + 0 )  << 56;

        Debug.Log( itemBinaries1.ToBinaryString() );
        List<byte> byteList1 = new List<byte>(BitConverter.GetBytes(itemBinaries1));
        for( int i = 0; i < byteList1.Count; i++ )
            Debug.Log( ( (int)byteList1[i] ).ToBinaryString() );

        itemBinaries2 += (long)  Item_Implicit.Nothing      << 0;
        itemBinaries2 += (long)  Item_Prefix.Plus_Life      << 8;
        itemBinaries2 += (long)  Item_Prefix.Nothing        << 16;
        itemBinaries2 += (long)  Item_Prefix.Nothing        << 24;
        itemBinaries2 += (long)  Item_Suffix.Nothing        << 32;
        itemBinaries2 += (long)  Item_Suffix.Nothing        << 40;
        itemBinaries2 += (long)  Item_Suffix.Nothing        << 48;

        Debug.Log( itemBinaries2.ToBinaryString() );
        List<byte> byteList2 = new List<byte>(BitConverter.GetBytes(itemBinaries2));
        for( int i = 0; i < byteList2.Count; i++ )
            Debug.Log( ( (int)byteList2[i] ).ToBinaryString() );

        // We can combine item type and item sprite. Have the right 4 bits determine item type and left 4 bits determine item sprite.
        // This means 16 total item types and 16 sprite variations for each item type
        // You can reduce casts by removing enums and having each be constant bytes. Enums elements are essentially constants anyway.
        // This could make things complicated however
        
		//byte[] data = { Head + cap, value1, value2, durability, 0b00000000, Str + 3, Str + 0, Str + 0 };
		//ulong itemBinaries = BitConverter.ToUInt64(data, 0);
		//Console.WriteLine(itemBinaries);
		//List<byte> byteList = new List<byte>(BitConverter.GetBytes(itemBinaries));
		//for(int i = 0; i < byteList.Count; i++) {
		//	Console.WriteLine(Convert.ToString(byteList[i], 2));
		//}
        
        //reading

        //List<Item_Prefix> prefixes = new List<Item_Prefix>();
        //List<Item_Prefix> suffixes = new List<Item_Prefix>();
        //List<Item_Prefix> implicits = new List<Item_Prefix>();
        //byteList.ForEach( v => list.Add( (Item_Affix)v ) );

        //for( int i = 0; i < byteList.Count; i++ ) {
        //    System.Collections.BitArray bits = new BitArray(byteList[i]);
        //    if( bits[6] && bits[7] ) implicits.Add( (Item_Prefix)byteList[i] );
        //    else if( bits[6] )       suffixes.Add( (Item_Prefix)byteList[i] );
        //    else if( bits[7] )       prefixes.Add( (Item_Prefix)byteList[i] );
        //}
        //foreach( Item_Prefix item in prefixes ) { Debug.Log( item ); }
        //foreach( Item_Prefix item in suffixes ) { Debug.Log( item ); }
        //foreach( Item_Prefix item in implicits ) { Debug.Log( item ); }
    }
}
