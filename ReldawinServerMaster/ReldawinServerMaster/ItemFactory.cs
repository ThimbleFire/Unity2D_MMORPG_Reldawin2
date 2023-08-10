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
        const byte Primary                 = 0b01010000; //80 
        const byte Secondary               = 0b01100000; //96
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

        const byte maxLevel = 60;

        public static void Generate(int characterLevel, out byte[] data)
        {
            Random rand = new Random();
            byte itemType = (byte)(rand.Next(1, 10) * 16);
            byte itemTier = (byte)rand.Next(0, (byte)MathF.Floor(characterLevel / 3));
            byte value1   = (byte)rand.Next(3 + itemTier * 11, 7 + itemTier * 11); // min damage / defense
            byte value2   = (byte)(itemType == Secondary ? 0 : value1 + 4);                            ;
            byte durability = (byte)rand.Next(10 + itemTier * 12, 7 + itemTier * 11);

            data = new byte[] { (byte)(itemType + itemTier), value1, value2, dura };
        }

    }
}
