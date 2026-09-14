# DragNWash.SpeedrunPractice

**English** | [繁體中文](README.zh-TW.md)

A speedrun practice plugin for *Drag'n Wash* (BepInEx).

> ### Quick install
> 1. Steam → right-click *Drag'n Wash* → **Manage** → **Browse local files**
> 2. Download a zip from **[Releases](../../releases/latest)**:
>    - **`-full.zip`** if you don't have BepInEx (most players)
>    - **`-plugin-only.zip`** if you already have BepInEx 5
> 3. Drag everything inside the zip into that game folder
> 4. Launch the game, load a save, press **F1**
>
> Details: [Install](#install)

## Features

- **F1** overlay: current level, dragon state, clean %, per-level timer
- **Level select**: jump straight to any of the 14 levels with story flags set as a real run would have them
- **Dragon state stepper** (**F8**): advance `WaitingToAppear → WalkingToWindow → … → Exited` one step at a time
- **F4** restart current level, **F6** instant clean, **F7** skip level
- Weather switch, time-scale slider
- Live story-flag toggles with a short description of each flag

> Note: level jump / reload overwrite the **current save slot's** `savegame.dgn` (it is the only way the game reloads a level). Use a dedicated slot for practice.

## Install

Open the game folder first: in Steam, right-click *Drag'n Wash* → **Manage** → **Browse local files**. That folder contains `DragNWash.exe`.

### A. I don't have BepInEx (most players)

1. Download **`DragNWash.SpeedrunPractice-vX.Y.Z-full.zip`** from [Releases](../../releases/latest).
2. Open the zip and drag **everything** inside it into the game folder (next to `DragNWash.exe`). If asked, allow overwriting.
3. Launch the game, load a save, press **F1**.

After step 2 the folder should contain `winhttp.dll`, `doorstop_config.ini` and a `BepInEx` folder. The first launch takes a few seconds longer while BepInEx sets itself up.

### B. I already have BepInEx 5

1. Download **`DragNWash.SpeedrunPractice-vX.Y.Z-plugin-only.zip`** from [Releases](../../releases/latest).
2. Open the zip and drag its `BepInEx` folder into the game folder (merges with the existing one).
3. Launch the game, load a save, press **F1**.

Either way you end up with `BepInEx/plugins/DragNWash.SpeedrunPractice/DragNWash.SpeedrunPractice.dll`.

### Uninstall

Delete `BepInEx/plugins/DragNWash.SpeedrunPractice/`. To remove BepInEx entirely, also delete `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `changelog.txt` and the `BepInEx` folder.

### Hotkeys

Configurable in `BepInEx/config/dragnwash.speedrunpractice.cfg` (key names are Unity Input System `Key` enum values, e.g. `F1`, `Backquote`, `Numpad0`).

| Key | Action |
|---|---|
| F1 | Toggle practice overlay |
| F4 | Restart current level |
| F6 | Instant clean |
| F7 | Skip level |
| F8 | Advance dragon state |

## Game version compatibility

The plugin is compiled against the game's own assemblies, so a game update can break it. Match the plugin to your game build (find the build ID on [SteamDB](https://steamdb.info/app/4739660/patchnotes/), or in `steamapps/appmanifest_4739660.acf` under `buildid`).

| Plugin | Game build (Steam build ID) | Notes |
|---|---|---|
| **v1.2.0+** | **25286774** (2026-09-14) and later | Saves via the game's own routine; expected to also work on older builds |
| v1.0.0 – v1.1.1 | builds before 25286774 | Level jump is broken on 25286774+ (the update moved saves to a new folder/format) |

If a new game update breaks something, open an issue with the build ID and the `BepInEx/LogOutput.log` lines containing `Speedrun`.

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
# → dist\DragNWash.SpeedrunPractice-vX.Y.Z-full.zip        (BepInEx 5 + plugin)
# → dist\DragNWash.SpeedrunPractice-vX.Y.Z-plugin-only.zip (plugin only)
```

The full zip bundles [BepInEx](https://github.com/BepInEx/BepInEx) (LGPL-2.1, license included as `BepInEx/LICENSE.BepInEx.txt`).

## License

MIT
