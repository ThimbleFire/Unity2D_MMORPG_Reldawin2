# ReldawinUnity
Reldawin is a multiplayer 2d isometric RPG. The player character starts in a wilderness where they create tools, build homes and survive the elements with talents, skills and magics. There is no class system, anyone can be anything.

![Reldawin](https://i.imgur.com/38DS2Wp.png)

## Reldawin-0.3

https://i.imgur.com/C8AYzRa.gif

This is a separate build of the game that uses Unity Tilemaps and XMLite-Net. Map and LPC (local player character) data is stored on the local player's client. Equipment and animation status is sent to the server on login and is distributed to nearby OPCs (other player characters) when they're within 2 chunks of one another.

LPC coordinates and destination are still stored and distrubted by the server on connection.

Although much of the code is reused from the Reldawin Unity project, the majority of it is built from the ground up.
