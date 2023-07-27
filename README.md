# ReldawinUnity
Reldawin is a multiplayer 2d isometric RPG. The player character starts in a wilderness where they create tools, build homes and survive the elements with talents, skills and magics. There is no class system, anyone can be anything.

![Reldawin](https://i.imgur.com/38DS2Wp.png)

## Reldawin-0.3

https://i.imgur.com/C8AYzRa.gif

![image](https://github.com/ThimbleFire/Reldawin/assets/14812476/d34e4bc6-4a94-4d19-8afb-ec084a64c209)


This is a separate build of the game that uses Unity Tilemaps and SQLite-Net.

The server hosts the map. players connect then login. the server returns the players position. the client places the character at that position which causes chunks to spawn. chunks request map data from the server then begin building the world.

characters start off naked. they right click tiles and forage.

crude iron + stone shard = crude knife.
branch + crude knife = shaft & wood scrap.
wood scrap + shaft = fire.
fire + crude iron = molten crude iron.
crude knife + shaft = mallet head, wooden handle
mallet head + molten crude iron = crude hammer head, crude stone chisel, crude pickaxe head, crude axe head, crude saw, crude nails, crude anvil, crude shovel head.
crude tool heads + shaft = crude tool.
mine iron and smelt it to produce regular iron.
axe + tree = logs + saw = planks + nails = wall, door, window.
anvil + Molten iron = ribbon + planks = water barrel.


TODO
* [x] Chunk Loading
* [x] Pathfinding
* [x] Movement and Animation
* [ ] Networking and Multiplayer
* [ ] Scene Objects
* [ ] Inventory and Equipment
* [ ] Gathering
* [ ] Crafting
* [ ] Building
