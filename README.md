# Expand World Factions

Allows adding new factions and changing existing ones.

Install on all clients and on the server (modding [guide](https://youtu.be/L9ljm2eKLrk)).

## How to use

[<img width="80px" style="margin-bottom: -4" src="https://cdn.prod.website-files.com/6257adef93867e50d84d30e2/636e0b5493894cf60b300587_full_logo_white_RGB.svg">](https://discord.gg/VFRJcPwUdm) for help and examples.

This mod itself can only be used to configure factions.

Other mods have to be used to inject the faction id to creatures.

- World Edit Commands: `tweak_character faction=` command can be used to set the faction name of an existing creature.
- Spawn That: Faction name can be set in some of the configs.
- RRR: Faction name can be set in some of the configs.
- Expand World Spawns: Faction name can be set in the `expand_spawns.yaml`.
- Expand World Events: Faction name can be set in the `expand_events.yaml`.
- Expand World Prefabs: Faction data can be added to `expand_data.yaml` and used in `expand_prefabs.yaml`.
  - Possible data keys are string `faction` and int `faction`.
  - String `faction` can be used to specify the faction name.
  - Int `faction` can be used to specify the faction id.

## Configuration

The file `expand_world/expand_factions.yaml` is created when loading a world.

### expand_factions.yaml

- faction: Name of the faction.
  - This is mostly used by other mods to specify the faction.
  - Recommended to not change name of original factions.
- id: Internal number used by the game.
  - This is how the base game specifies the faction.
  - Recommended to not change id of original factions.
  - For new factions, just add +1 to the last id (unless you have a good reason not to).
- friendly: List of friendly faction names (separated by `,`).
  - `All` can be used to make the faction friendly to all except `Players`.
  - If own faction name is not included, the faction will fight with each other.
  - Faction can be friendly with a faction that is not friendly with them.
- alertedFriendly: List of friendly faction names when aggroed/alerted.
  - By default, the `friendly` list is used.
  - Creatures become alerted when hit or when they see a hostile target.
  - Creatures calm down after a while (resetting back to `friendly` list).
- aggravatedFriendly: List of friendly faction names when aggravated.
  - By default, the `friendly` list is used.
  - If used, the creatures are automatically set aggravatable.
  - Creatures become aggravated when attacked by the player (or some other hostile action).
  - Aggravated status is never lost. However when alerted, the `alertedFriendly` list is used.
  - Players can always attack aggravatable creatures, regardless of faction.
- attackSame: If true, same creatures in different factions can attack each other.
  - This can be used to make pets fight the wild version of the same creature.
- tamedFaction: Name of the faction that is used when tamed.
  - By default, the `Players` faction is used.
  - Tamed creatures are always hostile to anything that is hostile to them.

## Credits

Thanks for Azumatt for creating the mod icon!

Sources: [GitHub](https://github.com/JereKuusela/valheim-expand_world_factions)
Donations: [Buy me a computer](https://www.buymeacoffee.com/jerekuusela)
