# ReldawinUnity
Reldawin is a multiplayer 2d isometric RPG. The player character starts in a wilderness where they create tools, build homes and survive the elements with talents, skills and magics. There is no class system, anyone can be anything.

![Reldawin](https://i.imgur.com/38DS2Wp.png)

## Reldawin-0.3

https://i.imgur.com/C8AYzRa.gif

![image](https://github.com/ThimbleFire/Reldawin/assets/14812476/d34e4bc6-4a94-4d19-8afb-ec084a64c209)
![sow grow demo2](https://github.com/ThimbleFire/Reldawin/assets/14812476/1651338e-8964-45a3-8e7f-2a7eb7b19ead)
![image](https://github.com/ThimbleFire/Reldawin/assets/14812476/a5af8b0e-445d-4272-a300-cc4dca819bf9)
![sow grow demo2](https://github.com/ThimbleFire/Reldawin/assets/14812476/cd1e3840-ebfb-44a5-92d8-d30670bfe17b)

Enemy Item Drop `server -> OnEntitySlain :: Server.PlaceItem( RollGlobalLootTable(client.properties) );`

```c#
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
private List<byte> RollGlobalLootTable(Properties cProperties) {

    List<byte> outgoing = new List<byte>();
    
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
    
    Random rand = new Random();
    int type = rand.Next( 1, 10 ) * 16;
    int tier = rand.Next( 0, MathF.Ceiling( cProperties.CharacterLevel / 3 ) );
    byte identity = (byte)(type + tier);
    outgoing.Add(identity);
    
    if(true) {
        if( rand.Next(0, 255) < MagicFind )
                prefixes++;
    }
    if(cProperties.CharacterLevel >= 9) {
        if( rand.Next(0, 255) < MagicFind )
                suffixes++;    
    }
    if(cProperties.CharacterLevel >= 18) {
        if( rand.Next(0, 255) < MagicFind )
                prefixes++;
    }
    if(cProperties.CharacterLevel >= 27) {
        if( rand.Next(0, 255) < MagicFind )
                suffixes++;
    }
    if(cProperties.CharacterLevel >= 36) {
        if( rand.Next(0, 255) < MagicFind )
                prefixes++;
    }
    if(cProperties.CharacterLevel >= 45) {
        if( rand.Next(0, 255) < MagicFind )
                suffixes++;
    }
    
    // type range is set to only include items with either an attack or defence value meaning this is always true
    ItemPropertyTruthTable itemTruthTable = ItemPropertyTruthTable.HasDefAtk;
    
    //No data exists with predefined implicits for item types and tiers, this could be defined as 
    
    switch(prefixes) {
        case 1: itemTruthTable |= ItemPropertyTruthTable.HasPrefix1;
        case 2: itemTruthTable |= ItemPropertyTruthTable.HasPrefix2;
        case 3: itemTruthTable |= ItemPropertyTruthTable.HasPrefix3;
    }
    switch(suffixes) {
        case 1: itemTruthTable |= ItemPropertyTruthTable.HasSuffix1;
        case 2: itemTruthTable |= ItemPropertyTruthTable.HasSuffix2;
        case 3: itemTruthTable |= ItemPropertyTruthTable.HasSuffix3;
    }

    outgoing.Add((byte)itemTruthTable);
    
    if( itemTruthTable & ItemPropertyTruthTable.HasDefAtk != 0)
        outgoing.Add( (byte)rand.Next( 3 + tier * 11, 7 + tier * 11 );

    if (itemTruthTable & ItemPropertyTruthTable.HasImplicit != 0)
        outgoing.Add( 0 ); // Implicits aren't implemented

    for(int i = 0; i < prefixes; i++)
        outgoing.Add( BuildAffix() );

    for(int i = 0; i < suffixes; i++)
        outgoing.Add( BuildAffix() );

    return outgoing;
}
private byte BuildAffix()
{
    int affixPow = Random.Range( 1, 1 * MathF.Ceiling(cProperties.CharacterLevel / 20) ) * 64;
    int affixID = Random.Range( 0, 36 ); // In future, we want to split affixes into prefixes and suffixes
    return = (byte)( affixPow + affixID );
}
```

Resource Gather `server -> client -> OnTick :: Server.PlaceItem( CompileProbability( InteractingWithSceneObject.probabilities), client.WorldPosition );`

TODO
* [x] Chunk Loading
* [x] Pathfinding
* [x] Movement and Animation
* [x] Networking and Multiplayer
* [x] Loading scene objects on chunk change
* [x] Resource pool scene objects
* [ ] Inventory and Equipment
* [ ] Gathering
* [ ] Crafting
* [ ] Building

////////////////////////////////////// throwaway code

```c#
public static void Main() {
    decimal myValue = 0b0111_1111_1111_1111_1111_1111_000_0101_0000_0100_0000_0011_0000_0010_0000_0001;		
    byte[] r = ToBaseTen(3, 1, myValue);
}	
private static byte[] ToBaseTen(int start, int length, decimal myValue) {
    byte[] source;
    byte[] target = new byte[2] { byte.MaxValue, byte.MaxValue };
    MemoryStream m = new MemoryStream();
    BinaryWriter writer = new BinaryWriter(m);
    writer.Write(myValue);
    source = m.ToArray();
    Array.Copy(source, source.GetLowerBound(0)+start, target, target.GetLowerBound(0), length);
    m.Close();
    writer.Close();
    return target;
}
```
