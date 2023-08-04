using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ItemFactory
{
	//Types
	const byte Any                     = 0b00000000;
	const byte Head                    = 0b00010000;
	const byte Chest                   = 0b00100000;
	const byte Gloves                  = 0b00110000;
	const byte Legs                    = 0b01000000;
	const byte Feet                    = 0b01010000;
	const byte Primary                 = 0b01100000;
	const byte Secondary               = 0b01110000;
	const byte Ring                    = 0b10000000;
	const byte Neck                    = 0b10010000;
	const byte Artifact                = 0b10100000;
	const byte Misc                    = 0b10110000;
	const byte Consumable              = 0b11000000;
	const byte Quest                   = 0b11010000;
	const byte Belt                    = 0b11100000;
	//Sprites
	const byte amulet1                 = 0b00000000;
	const byte amulet2                 = 0b00000001;
	//
	const byte ring1                   = 0b00000000;
	const byte ring2                   = 0b00000001;
	//
	const byte chainboots              = 0b00000000;
	const byte heavyboots              = 0b00000001;
	//
	const byte quiltedarmor            = 0b00000000;
	const byte studdedleather          = 0b00000001;
	//
	const byte cap                     = 0b00000000;
	const byte skullcap                = 0b00000001;
	//
	const byte axe                     = 0b00000000;
	const byte broadaxe                = 0b00000001;
	//
	const byte buckler                 = 0b00000000;
	const byte smallshield             = 0b00000001;
	//
	const byte chain                   = 0b00000000;
	const byte heavygloves             = 0b00000001;
	//
	const byte sash                    = 0b00000000;
	//Affixes
	const byte Nothing                 = 0b00000000; //0
	const byte Plus_Accuracy           = 0b00000001; //1
	const byte Dmg_Phys_Min            = 0b00000010; //2
	const byte Dmg_Phys_Max            = 0b00000011; //3
	const byte Dmg_Phys_Percent        = 0b00000100; //4
	const byte Dmg_Ele_Fire            = 0b00000101; //5
	const byte Dmg_Ele_Cold            = 0b00000110; //6
	const byte Dmg_Ele_Lightning       = 0b00000111; //7
	const byte Dmg_Ele_Poison          = 0b00001000; //8
	const byte Def_Phys_Flat           = 0b00001001; //9
	const byte Def_Phys_Percent        = 0b00001010; //10
	const byte Def_Dmg_Reduction_Phys  = 0b00001011; //11
	const byte Def_Dmg_Reduction_Magic = 0b00001100; //12
	const byte Def_Dmg_Reduction_All   = 0b00001101; //13
	const byte Def_Ele_Res_Fire        = 0b00001110; //14
	const byte Def_Ele_Res_Cold        = 0b00001111; //15
	const byte Def_Ele_Res_Lightning   = 0b00010000; //16
	const byte Def_Ele_Res_Poison      = 0b00010001; //17
	const byte Def_Ele_Res_All         = 0b00010010; //18
	const byte On_Hit_Life             = 0b00010011; //19
	const byte On_Kill_Life            = 0b00010100; //20
	const byte On_Hit_Mana             = 0b00010101; //21
	const byte On_Kill_Mana            = 0b00010110; //22
	const byte Plus_Life               = 0b00010111; //23
	const byte Plus_Mana               = 0b00011000; //24
	const byte Plus_Regen_Life         = 0b00011001; //25
	const byte Plus_Regen_Mana         = 0b00011010; //26
	const byte Plus_Str                = 0b00011011; //27
	const byte Plus_Dex                = 0b00011100; //28
	const byte Plus_Con                = 0b00011101; //29
	const byte Plus_Int                = 0b00011110; //30
	const byte Plus_Speed_Phys         = 0b00011111; //31
	const byte Plus_Speed_Magic        = 0b00100000; //32
	const byte Plus_Speed_Movement     = 0b00100001; //33
	const byte Plus_Block_Recovery     = 0b00100010; //34
	const byte Plus_Stagger_Recovery   = 0b00100011; //35
	const byte Plus_Magic_Find         = 0b00100100; //36
	// attributes
	const byte Strength       		   = 0b00000000;
	const byte Dexterity        	   = 0b01000000;
	const byte Constitution     	   = 0b10000000;
	const byte Intelligence     	   = 0b11000000;

    public static void Build() {
	    byte value1 = 0;
	    byte value2 = 0;
	    byte durability = 0;
		
		byte[] data = { Head + cap, value1, value2, durability, 0b00000000, Strength + 3, Strength + 0, Strength + 0 };
		ulong itemBinaries = BitConverter.ToUInt64(data, 0);
		Console.WriteLine(itemBinaries);
		List<byte> byteList = new List<byte>(BitConverter.GetBytes(itemBinaries));
		for(int i = 0; i < byteList.Count; i++) {
			Console.WriteLine(Convert.ToString(byteList[i], 2));
		}

	// We can combine item type and item sprite. Have the right 4 bits determine item type and left 4 bits determine item sprite.
	// This means 16 total item types and 16 sprite variations for each item type
	// You can reduce casts by removing enums and having each be constant bytes. Enums elements are essentially constants anyway.
	// This could make things complicated however

	//Write a formula for armour stats (value1, value2, durability, 'cap' tier) based on the player character level and the slain NPC level
	//Have attribute requirements be determined by prefixes and suffixes. If a suffix boosts mana regen by 60% for example, have it subsequently require 40 intelligence.

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
