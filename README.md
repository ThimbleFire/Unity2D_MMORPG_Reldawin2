# ReldawinUnity
Reldawin is a multiplayer 2d isometric RPG. The player character starts in a wilderness where they create tools, build homes and survive the elements with talents, skills and magics. There is no class system, anyone can be anything.

![Reldawin](https://i.imgur.com/38DS2Wp.png)

## TODO List

- [Code cleanup]

_Code cleanup_: Trying to massively speed up the time it takes to load chunks. We're currently reworking how we get UVs. 
                Through testing we've discovered it is 4 to 5 times faster to get information directly through a static dictionary than through a Get method. 

- [Fix] Interact animations
- [Add] Crafting

You activate an item in the inventory and use it with another item. The two items are used to query existing recipes featuring those two items. Returned recipes are presented as buttons that can be clicked in order to craft these items. XMLDevice.GetRecipesContaining(itemA, itemB);

- [Add] Doodad destruction  
- [Add] Mining animation 
- [Add] Item database
- [Add] Chat window
- [Add] Drop item from inventory
- [Add] Pile of items model for multiple dropped items
- [Add] Pile of item container
- [Add] Take from Pile of item container
- [Add] Doodad decay (yes, including trees which will grow, die and spread naturally over time)

## Bugs to squash
- Sometimes when you move away from doodads while interacting the interaction animation persists.
