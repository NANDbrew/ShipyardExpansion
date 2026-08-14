# Shipyard Expansion
New parts and options for Sailwind's vanilla ships.

**⚠️Saving while this is installed will make the save dependent on it!⚠️**
There are two ways to safely uninstall this mod:
- Use ["Clean Save"](https://github.com/NANDbrew/ShipyardExpansion#fixers) to sanitize your saves **before** uninstalling the mod
- Install the [standalone save cleaner](https://github.com/NANDbrew/SaveCleaner) to fix missing parts **after** uninstalling the mod

## New Parts
- More masts
- More stays
- More shrouds
- Telltales for your shrouds
- More cabin options for Cog and Dhow
- Tillers for the small boats
- Deep keels for the small boats
  - Moves ballast down for added stability
- Sheathing
  - Reduces hull damage from wear and impacts by ⅔
- Crow's nests and ratlines for all boats
  - Ratlines are click-to-climb instead of vanilla's janky climbing
- Lug sails
- Raked masts
- Topmasts for Brig, Junk, Sanbuq
- Jib-booms for several boats (bowsprit extension, not the modern type)
- Bermuda masts (early historical style, only accepts staysails)
- Other misc. parts
- For a more complete list of added parts and ship adjustments see [PartsList.md](https://github.com/NANDbrew/ShipyardExpansion/blob/main/PartsList.md)

## General Features/Adjustments
- Topmast linking
  - Links square sail angles on topmasts to the ones on the mast below, making them behave as if it's all one mast
  - Can be turned off in [settings](https://github.com/NANDbrew/ShipyardExpansion#general-settings)
- Sail area: Fixes occasional issues with sail area calculations for resized sails
- Rotate lateen and fin sails to work better with raked masts
- Flip staysails along the stay
- Improved sail scaling and limits
  - Allows bigger and/or wider square sails
  - Brig Jibs can be width-adjusted
  - Can be turned off in [settings](https://github.com/NANDbrew/ShipyardExpansion#general-settings)
- Includes the "Junk gaff fix" from NANDFixes
- Fixes a visual issue with the rope routing on certain sails
- Furl/unfurl all sails while in the shipyard for ease of planning
- Change the cloth texture of your sails for better color matching
- Adds size percent to sail names
  - Can be turned off in [settings](https://github.com/NANDbrew/ShipyardExpansion#general-settings)
- Adjusts sail height limits on existing masts
- Increases sail capacity of a few vanilla masts
- Adds targeting colliders to a few key railings and things on the boats to make arranging tools easier
- Adds pages and next/previous page buttons to the shipyard parts pane to fit more options per tab
- Moves a few stays from the 'masts' tab to the 'stays' tab
### Technical adjustments for mod authors
- Resizes sail prefab array to 128
- Resizes mast/mast button array to 512

## Configuration
### General settings
- "Unfurl sails in shipyard": Unfurl existing sails when entering the shipyard. Sails can also be furled/unfurled with a button in the sail menu
- "Show percent scale in sail name"
- "Auto-fit sails": Automatically scale too-big new sails to fit the mast
- "Climb speed": Speed when climbing ratlines
- "Link topmasts": Link square sail angles on topmasts to the ones on the mast below (requires a restart)
- "Override scaling": Override sail size buttons with the mod's version
- "Combined scaling": scale square sails with a "size" button and "width" button (same as vanilla) **off by default**
  - Requires "Override scaling" to work
  - When disabled, height and width are scaled separately
- "Vertical lateens": add lateens upright on raked masts instead of tilted with the mast
- "Vertical fins": add fin sails upright on raked masts instead of tilted with the mast
### Fixers
- "Convert saves": Enable this before loading a save from previous versions of Shipyard Expansion **off by default**
- "Clean save": Enable this before saving if you want to uninstall this mod (will disable itself when done)
- "Clean load": Enable this before loading a broken save **on by default**
### Advanced settings
- "Add lug sails": Adds new sails in the 'Other' category. **on by default**
- "skip sail data": Ignore the mod's sail data (flipped jibs, tilted sails) when loading **off by default**
- "Starter set fix": keeps starting items from bugging out due to embarking adjustments **on by default**
### How to configure
- Method 1
  - download the BepInEx 5 version of [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases)
  - install the configuration manager like a mod
  - open the configuration menu in-game with f1
- Method 2
  - open `BepInEx/config/com.nandbrew.shipyardexpansion.cfg`
  - follow instructions in the file

