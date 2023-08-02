# ReldawinUnity
Reldawin is a multiplayer 2d isometric RPG. The player character starts in a wilderness where they create tools, build homes and survive the elements with talents, skills and magics. There is no class system, anyone can be anything.

![Reldawin](https://i.imgur.com/38DS2Wp.png)

## Reldawin-0.3

https://i.imgur.com/C8AYzRa.gif

![image](https://github.com/ThimbleFire/Reldawin/assets/14812476/d34e4bc6-4a94-4d19-8afb-ec084a64c209)
![sow grow demo2](https://github.com/ThimbleFire/Reldawin/assets/14812476/1651338e-8964-45a3-8e7f-2a7eb7b19ead)
![image](https://github.com/ThimbleFire/Reldawin/assets/14812476/a5af8b0e-445d-4272-a300-cc4dca819bf9)
![sow grow demo2](https://github.com/ThimbleFire/Reldawin/assets/14812476/cd1e3840-ebfb-44a5-92d8-d30670bfe17b)



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
