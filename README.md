# DragNWash.SpeedrunPractice

*Drag'n Wash* 的 speedrun 練習工具（BepInEx 插件）。
A speedrun practice plugin for *Drag'n Wash* (BepInEx). English section below.

## 功能

- **F1** 練習選單：目前關卡 / 龍的狀態 / 清潔百分比 / 每關計時
- **關卡選擇**：直接跳到 1–14 任一關（劇情旗標會自動設成該關應有的狀態）
- **練習存檔槽**（5 槽）：**F5** 存、**F9** 讀，PageUp / PageDown 切換槽位。不會覆蓋你的遊戲存檔，獨立存在 `BepInEx/config/DragNWash.SpeedrunPractice/`
- **F4** 從本關開頭重來
- **F6** 一鍵洗乾淨、**F7** 跳過本關（遊戲內建但被隱藏的 debug 功能）
- 天氣切換、慢動作（Time Scale）滑桿、劇情旗標即時勾選
- 遊戲原本藏起來的 *Instant Clean / Skip Level / Delete Save* 按鈕會顯示在遊戲 UI 裡

> 限制：遊戲存檔只記錄「第幾關 + 劇情旗標」，所以存讀槽的最小粒度是**關卡開頭**，無法保存關卡中途的清洗進度。

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
| F5 / F9 | 存入 / 讀取目前練習槽 |
| PageUp / PageDown | 切換練習槽 |
| F4 | 本關重來 |
| F6 | 一鍵洗乾淨 |
| F7 | 跳過本關 |

---

## English

### Features

- **F1** overlay: current level, dragon state, clean %, per-level timer
- **Level select**: jump straight to any of the 14 levels with story flags set as a real run would have them
- **5 practice state slots**: **F5** save / **F9** load, PageUp / PageDown to pick a slot. Stored separately in `BepInEx/config/DragNWash.SpeedrunPractice/`, your real save is untouched
- **F4** restart current level, **F6** instant clean, **F7** skip level
- Weather switch, time-scale slider, live story-flag toggles
- Re-enables the game's hidden *Instant Clean / Skip Level / Delete Save* debug buttons

> Limitation: the game only saves *level index + story flags*, so state slots restore the **start of a level**, not mid-level washing progress.

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
