# NicerTeleporters

A lethal company mod that makes teleporters a little bit nicer :)

## Features
- Teleporter and inverse teleporter no longer drop the following item ids when being used: `["Shovel", "WalkieTalkie", "KeyItem", "FlashlightItem", "BoomboxItem", "Radar-Booster"]`
- Reduced inverse teleporter cooldown from 210 to 45 seconds.
- Resets teleporter cooldowns at start of round.

## How to install

### Requirements
- [BepInEx 5.4.x](https://github.com/BepInEx/BepInEx)

### Manual
- Ensure you've downloaded BepInEx and have run the game atleast once.
- Download the `NicerTeleporters.dll` from [GitHub Releases](https://github.com/rhydiaan/NicerTeleporters/releases) page.
- Copy into the following directory: `\{GAME_LOCATION}\Lethal Company\BepInEx\plugins`

### Automatic
- Install via [r2modman](https://thunderstore.io/c/lethal-company/p/ebkr/r2modman/) (Recommended) or [Thunderstore Mod Manager](https://www.overwolf.com/oneapp/thunderstore-mod-manager)

## Change list
### v1.1.2
- Added Radar-Booster to kept items list

### v1.1.1
- Fixed a bug where player weight was not calculated properly after teleporting with items.

### v1.1.0
- Reduced inverse teleporter cooldown from 210 to 45 seconds.
- Resets teleporter cooldowns at start of round.

### v1.0.0
- Teleporter and inverse teleporter no longer drop the following item ids when being used: `["Shovel", "WalkieTalkie", "KeyItem", "FlashlightItem", "BoomboxItem"]`
