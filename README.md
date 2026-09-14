# DragNWash.SpeedrunPractice

*Drag'n Wash* 的 speedrun 練習工具（BepInEx 插件）。
A speedrun practice plugin for *Drag'n Wash* (BepInEx). English section below.

## 功能

- **F1** 練習選單：目前關卡 / 龍的狀態 / 清潔百分比 / 每關計時
- **關卡選擇**：直接跳到 1–14 任一關（劇情旗標會自動設成該關應有的狀態）
- **龍的狀態單步推進**（**F8**）：`WaitingToAppear → WalkingToWindow → WaitingToBeLetIn → … → Exited`，一次前進一格
- **F4** 從本關開頭重來
- **F6** 一鍵洗乾淨、**F7** 跳過本關（呼叫遊戲內建的 debug 功能）
- 天氣切換、慢動作（Time Scale）滑桿
- 劇情旗標即時勾選，附上每個旗標的說明

> 注意：跳關與重來會覆寫**目前存檔槽**的 `savegame.dgn`（遊戲重載關卡的唯一途徑）。建議用專門的存檔槽練習。

## 安裝

1. **安裝 BepInEx 5**（若已裝過可跳過）
   - 下載 [BepInEx 5.4.x **x64**](https://github.com/BepInEx/BepInEx/releases)（檔名類似 `BepInEx_win_x64_5.4.23.x.zip`）
   - 解壓到遊戲資料夾（含 `DragNWash.exe` 的那層；Steam：右鍵遊戲 → 管理 → 瀏覽本機檔案）
   - 啟動一次遊戲再關閉，讓 BepInEx 產生 `BepInEx/plugins` 等資料夾
2. 從 [Releases](../../releases) 下載 `DragNWash.SpeedrunPractice-vX.Y.Z.zip`
3. 解壓到遊戲資料夾，讓路徑成為  
   `Drag'n Wash/BepInEx/plugins/DragNWash.SpeedrunPractice/DragNWash.SpeedrunPractice.dll`
4. 啟動遊戲，載入存檔後按 **F1**

### 改鍵

第一次啟動後會產生 `BepInEx/config/dragnwash.speedrunpractice.cfg`，用文字編輯器改 `[Hotkeys]` 區段即可（鍵名為 Unity Input System 的 `Key` 列舉，例如 `F1`、`Backquote`、`Numpad0`）。

### 移除

刪除 `BepInEx/plugins/DragNWash.SpeedrunPractice/` 資料夾即可。

## 快捷鍵一覽

| 鍵 | 功能 |
|---|---|
| F1 | 開關練習選單 |
| F4 | 本關重來 |
| F6 | 一鍵洗乾淨 |
| F7 | 跳過本關 |
| F8 | 龍的狀態前進一格 |

---

## English

### Features

- **F1** overlay: current level, dragon state, clean %, per-level timer
- **Level select**: jump straight to any of the 14 levels with story flags set as a real run would have them
- **Dragon state stepper** (**F8**): advance `WaitingToAppear → WalkingToWindow → … → Exited` one step at a time
- **F4** restart current level, **F6** instant clean, **F7** skip level
- Weather switch, time-scale slider
- Live story-flag toggles with a short description of each flag

> Note: level jump / reload overwrite the **current save slot's** `savegame.dgn` (it is the only way the game reloads a level). Use a dedicated slot for practice.

### Install

1. Install [BepInEx 5.4.x x64](https://github.com/BepInEx/BepInEx/releases) into the game folder (next to `DragNWash.exe`), run the game once, quit.
2. Download `DragNWash.SpeedrunPractice-vX.Y.Z.zip` from [Releases](../../releases).
3. Extract into the game folder so you get `BepInEx/plugins/DragNWash.SpeedrunPractice/DragNWash.SpeedrunPractice.dll`.
4. Launch, load a save, press **F1**.

Hotkeys are configurable in `BepInEx/config/dragnwash.speedrunpractice.cfg`. To uninstall, delete the plugin folder.

### Building from source

Requires the .NET SDK (7+) and an installed copy of the game (the project references the game's assemblies directly; they are not included in this repo).

```powershell
.\pack.ps1 -GameDir "D:\SteamLibrary\steamapps\common\Drag'n Wash"
# → dist\DragNWash.SpeedrunPractice-vX.Y.Z.zip
```

## License

MIT
