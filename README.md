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
* [ ] Inventory occupation on server

The inventory is an 8x4 grid and is represented by an integer. 4 bytes, 1 bit each slot.
Equipment is represented by a 16-bit ushort.

These bytes are updated when an item is placed into the inventory, or equipment slot.

Item db element pos is represented by a byte.
- 2 bits represent the y-axis
- 4 bits represent the x-axis
- 2 bit represents whether the item is equipped, in the inventory, or on the ground.

Item db element size is represented by a byte.
- 2 bits represent width
- 2 bits represent height
4 bits unused.

* [ ] Item Editor

server generates item.
server sends item to client.
client has database containing item names, min and max values.
client looks up entry with matching identity.

* [ ] Gathering
* [ ] Crafting
* [ ] Building
