using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReldawinServerMaster
{
    public class ItemFactory
    {
        //Types
        const byte Material                = 0b00000000; //0
        const byte Head                    = 0b00010000; //16
        const byte Chest                   = 0b00100000; //32
        const byte Gloves                  = 0b00110000; //48
        const byte Feet                    = 0b01000000; //64
        const byte Secondary               = 0b01010000; //80
        const byte Primary                 = 0b01100000; //96 
        const byte Ring                    = 0b01110000; //112
        const byte Neck                    = 0b10000000; //128
        const byte Belt                    = 0b10010000; //144
        const byte Consumable              = 0b10100000; //160
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
        const byte Strength                = 0b00000000;
        const byte Dexterity               = 0b01000000;
        const byte Constitution            = 0b10000000;
        const byte Intelligence            = 0b11000000;

        [Flags]
        enum ItemPropertyTT : uint
        {
            HasV1 =        0b00000001,
            HasV2 =        0b00000010,
            HasDura =      0b00000100,
            HasImplicit =  0b00001000,
            HasPrefix1 =   0b00010000,
            HasPrefix2 =   0b00100000,
            HasPrefix3 =   0b00110000,
            HasSuffix1 =   0b01000000,
            HasSuffix2 =   0b10000000,
            HasSuffix3 =   0b11000000,
        }

        const byte maxLevel = 60;

        public static void Generate(int characterLevel, out List<byte> d)
        {
            d = new List<byte>();
            Random rand = new Random();

            // Item identity

            int itemType = rand.Next(1, 10) * 16;
            int itemTier = rand.Next(0, (int)MathF.Ceiling(characterLevel / 3));
            byte itemIdentity = (byte)(itemType + itemTier);
            d.Add( itemIdentity );

            // Item truth table

            ItemPropertyTT truthTable = 0b00000000;

            bool itemHasAttackMinOrDefense = itemType >= 16 && itemType <=  96;
            bool itemHasAttackMax = itemType == Primary;

            if( itemHasAttackMinOrDefense )
                truthTable |= ItemPropertyTT.HasV1;
            if( itemHasAttackMax )
                truthTable |= ItemPropertyTT.HasV2;
            if( itemHasAttackMinOrDefense || itemHasAttackMax )
                truthTable |= ItemPropertyTT.HasDura;

            // we have no item data and no way to determine whether an item should have an implicit value

            if( false )
                truthTable |= ItemPropertyTT.HasImplicit;

            int[] rarity =
            rand.Next( 0, 99 ) <= 01 ? new int[] { 3, 3 }:
            rand.Next( 0, 99 ) <= 03 ? new int[] { 3, 2 }:
            rand.Next( 0, 99 ) <= 05 ? new int[] { 2, 2 }:
            rand.Next( 0, 99 ) <= 07 ? new int[] { 2, 1 }:
            rand.Next( 0, 99 ) <= 09 ? new int[] { 1, 1 }:
            rand.Next( 0, 99 ) <= 11 ? new int[] { 1, 0 }:
                                       new int[] { 0, 0 };

            switch( rarity[0] ) {
                case 1: truthTable |= ItemPropertyTT.HasPrefix1; break;
                case 2: truthTable |= ItemPropertyTT.HasPrefix2; break;
                case 3: truthTable |= ItemPropertyTT.HasPrefix3; break;
            }
            switch( rarity[1] ) {
                case 1: truthTable |= ItemPropertyTT.HasSuffix1; break;
                case 2: truthTable |= ItemPropertyTT.HasSuffix2; break;
                case 3: truthTable |= ItemPropertyTT.HasSuffix3; break;
            }

            d.Add( (byte)truthTable );

            //item properties

            if( truthTable.HasFlag( ItemPropertyTT.HasV1 ) )
                d.Add( byte.MaxValue );
            if( truthTable.HasFlag( ItemPropertyTT.HasV2 ) )
                d.Add( byte.MaxValue );
            if( truthTable.HasFlag( ItemPropertyTT.HasDura ) )
                d.Add( byte.MaxValue / 2 );
            if( truthTable.HasFlag( ItemPropertyTT.HasDura ) )
                d.Add( byte.MaxValue );

            for( int i = 0; i < rarity[0]; i++ ) {
                d.Add( byte.MaxValue );
            }
            for( int i = 0; i < rarity[1]; i++ ) {
                d.Add( byte.MaxValue );
            }

        }
    }
}
