# DragNWash.SpeedrunPractice

**English** | [繁體中文](README.zh-TW.md)

A speedrun practice plugin for *Drag'n Wash* (BepInEx).

## Features

- **F1** overlay: current level, dragon state, clean %, per-level timer
- **Level select**: jump straight to any of the 14 levels with story flags set as a real run would have them
- **Dragon state stepper** (**F8**): advance `WaitingToAppear → WalkingToWindow → … → Exited` one step at a time
- **F4** restart current level, **F6** instant clean, **F7** skip level
- Weather switch, time-scale slider
- Live story-flag toggles with a short description of each flag

> Note: level jump / reload overwrite the **current save slot's** `savegame.dgn` (it is the only way the game reloads a level). Use a dedicated slot for practice.

## Install

1. Install [BepInEx 5.4.x x64](https://github.com/BepInEx/BepInEx/releases) into the game folder (next to `DragNWash.exe`), run the game once, quit.
2. Download `DragNWash.SpeedrunPractice-vX.Y.Z.zip` from [Releases](../../releases).
3. Extract into the game folder so you get `BepInEx/plugins/DragNWash.SpeedrunPractice/DragNWash.SpeedrunPractice.dll`.
4. Launch, load a save, press **F1**.

Hotkeys are configurable in `BepInEx/config/dragnwash.speedrunpractice.cfg` (key names are Unity Input System `Key` enum values, e.g. `F1`, `Backquote`, `Numpad0`). To uninstall, delete the plugin folder.

## Hotkeys

| Key | Action |
|---|---|
| F1 | Toggle practice overlay |
| F4 | Restart current level |
| F6 | Instant clean |
| F7 | Skip level |
| F8 | Advance dragon state |

## Story flags reference

The story is driven by **Yarn Spinner** dialogue scripts; every flag is a boolean variable in those scripts. Apart from `levelIndex`, the save file contains nothing but these flags, so they *are* the story progress. Toggle them live under "Show story flags" in the F1 overlay.

**1. Level milestones** (set automatically by LevelFlow)

| Flag | Meaning |
|---|---|
| `level_1`, `level_5`, `level_6_started` | Set true when that level **starts** |
| `level_1_complete`, `level_5_complete` | Set true when that level is **finished** |

Level jump sets earlier levels' flags true and later ones false for you.

**2. Current dragon interaction** (reset every level)

| Flag | Meaning |
|---|---|
| `DragonFinishedFromHandJob` | This dragon was finished by hand |
| `DragonOnaholeSexInProgress` / `DragonOnaholeSexCompleted` | Mount session in progress / finished |
| `DragonDenyMount` | This dragon refused the mount |

The level evaluator reads these to decide when the level counts as DONE.

**3. Side objectives**

| Flag | Meaning |
|---|---|
| `HasMountFrame → DeliveredMountFrame → BuiltMount → PlacedMount → PlacedMountForUse` | Mount side-quest stages: frame obtained → delivered → built → placed → ready for use |
| `MedkitCompleted` | Medkit / bandage objective done |
| `PicnicPlaced` / `PicnicCompleted` | Picnic set up / done |

**4. Dialogue and romance**

| Flag | Meaning |
|---|---|
| `has_talked_to_ryan`, `has_asked_question`, `suggested_ryan_to_conrad` | Dialogue gates (a branch plays once) |
| `conrad_used_mount_3`, `conrad_jerked_off_3` | Which option was used on Conrad's 3rd visit; affects later dialogue |
| `*_romanced` | That character's route unlocked |
| `*_sex_scene` | Cutscene **queued** |
| `finished_watching_*_sex_scene` | Cutscene **already watched** |

The last two matter most: on every level load, if `X_sex_scene` is true but `finished_watching_X` is false, the game **plays the cutscene first** instead of starting the level. Level jump marks triggered cutscenes as watched automatically; to practice the section right after a cutscene, untick `finished_watching_*` and Reload.

`Yarn.Internal.Once.line:xxxx` flags are Yarn's "say this line once" markers. They don't affect progress and are hidden in the overlay.

## Building from source

Requires the .NET SDK (7+) and an installed copy of the game (the project references the game's assemblies directly; they are not included in this repo).

```powershell
.\pack.ps1 -GameDir "D:\SteamLibrary\steamapps\common\Drag'n Wash"
# → dist\DragNWash.SpeedrunPractice-vX.Y.Z.zip
```

## License

MIT
