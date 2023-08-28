using System.Collections.Generic;
using System;
using Bindings;

namespace ReldawinServerMaster
{
    public class ItemFactory
    {
        public static List<byte> RollGlobalLootTable( ClientProperties cProperties ) {

            List<byte> outgoing = new List<byte>();

            Random rand = new Random();
            int type = rand.Next( 1, 10 ) * 16;
            int tier = rand.Next( 0, (int)MathF.Ceiling( cProperties.CharacterLevel / 3 ) );
            byte identity = (byte)(type + tier);
            outgoing.Add( identity );

            byte prefixes = 0;
            byte suffixes = 0;

            if( true ) {
                if( rand.Next( 0, 255 ) < cProperties.MagicFind )
                    prefixes++;
            }
            if( cProperties.CharacterLevel >= 9 ) {
                if( rand.Next( 0, 255 ) < cProperties.MagicFind )
                    suffixes++;
            }
            if( cProperties.CharacterLevel >= 18 ) {
                if( rand.Next( 0, 255 ) < cProperties.MagicFind )
                    prefixes++;
            }
            if( cProperties.CharacterLevel >= 27 ) {
                if( rand.Next( 0, 255 ) < cProperties.MagicFind )
                    suffixes++;
            }
            if( cProperties.CharacterLevel >= 36 ) {
                if( rand.Next( 0, 255 ) < cProperties.MagicFind )
                    prefixes++;
            }
            if( cProperties.CharacterLevel >= 45 ) {
                if( rand.Next( 0, 255 ) < cProperties.MagicFind )
                    suffixes++;
            }

            // type range is set to only include items with either an attack or defence value meaning this is always true
            ItemPropertyTruthTable itemTruthTable = ItemPropertyTruthTable.HasDefAtk;

            //No data exists with predefined implicits for item types and tiers, this could be defined as 

            switch( prefixes ) {
                case 1: itemTruthTable |= ItemPropertyTruthTable.HasPrefix1; break;
                case 2: itemTruthTable |= ItemPropertyTruthTable.HasPrefix2; break;
                case 3: itemTruthTable |= ItemPropertyTruthTable.HasPrefix3; break;
            }
            switch( suffixes ) {
                case 1: itemTruthTable |= ItemPropertyTruthTable.HasSuffix1; break;
                case 2: itemTruthTable |= ItemPropertyTruthTable.HasSuffix2; break;
                case 3: itemTruthTable |= ItemPropertyTruthTable.HasSuffix3; break;
            }

            outgoing.Add( (byte)itemTruthTable );

            if( itemTruthTable.HasFlag(ItemPropertyTruthTable.HasDefAtk) )
                outgoing.Add( (byte)rand.Next( 3 + tier * 11, 7 + tier * 11 ));

            if( itemTruthTable.HasFlag( ItemPropertyTruthTable.HasImplicit) )
                outgoing.Add( 0 ); // Implicits aren't implemented

            int cLvlDiv20 = (int)MathF.Ceiling( cProperties.CharacterLevel / 20 );

            for( int i = 0; i < prefixes; i++ )
                outgoing.Add( BuildAffix( rand, cLvlDiv20 ) );

            for( int i = 0; i < suffixes; i++ )
                outgoing.Add( BuildAffix( rand, cLvlDiv20 ) );

            return outgoing;
        }

        private static byte BuildAffix(Random rand, int cLvlDiv20) {
            int affixPow = rand.Next( 1, 1 * cLvlDiv20 ) * 64;
            int affixID = rand.Next( 0, 36 ); // In future, we want to split affixes into prefixes and suffixes
            return (byte)( affixPow + affixID );
        }

        [Flags]
        enum ItemPropertyTruthTable : byte
        {
            HasDefAtk =    0b00000001,
            HasImplicit =  0b00000010,
            HasPrefix1 =   0b00000100,
            HasPrefix2 =   0b00001000,
            HasPrefix3 =   0b00001100,
            HasSuffix1 =   0b00010000,
            HasSuffix2 =   0b00100000,
            HasSuffix3 =   0b00110000,
        }

        //Types
        const byte Material                = 0b00000000; //0
        const byte Head                    = 0b00010000; //16
        const byte Chest                   = 0b00100000; //32
        const byte Gloves                  = 0b00110000; //48
        const byte Feet                    = 0b01000000; //64
        const byte Primary                 = 0b01010000; //80 
        const byte Secondary               = 0b01100000; //96
        const byte Ring                    = 0b01110000; //112
        const byte Neck                    = 0b10000000; //128
        const byte Belt                    = 0b10010000; //144
        const byte Consumable              = 0b10100000; //160
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

        const byte Strength                = 0b00000000;
        const byte Dexterity               = 0b01000000;
        const byte Constitution            = 0b10000000;
        const byte Intelligence            = 0b11000000;
    }
}