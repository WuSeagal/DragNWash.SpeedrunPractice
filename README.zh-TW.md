# DragNWash.SpeedrunPractice

[English](README.md) | **繁體中文**

*Drag'n Wash* 的 speedrun 練習工具（BepInEx 插件）。

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

## Story flags 說明

遊戲劇情用 **Yarn Spinner** 對話腳本驅動，每個旗標就是腳本裡的一個布林變數。存檔裡除了 `levelIndex` 之外只有這些旗標，所以旗標 = 劇情進度的全部。F1 選單的「Show story flags」可即時勾選。

### 1. 關卡進度標記（由 LevelFlow 自動設）
| 旗標 | 意思 |
|---|---|
| `level_1`, `level_5`, `level_6_started` | 該關**開始**時設 true |
| `level_1_complete`, `level_5_complete` | 該關**完成**時設 true |

跳關功能會依目標關卡自動把前面的設 true、後面的設 false。

### 2. 當前這隻龍的互動狀態（每關重置）
| 旗標 | 意思 |
|---|---|
| `DragonFinishedFromHandJob` | 這隻龍是用手完成的 |
| `DragonOnaholeSexInProgress` / `DragonOnaholeSexCompleted` | 用坐騎（mount）進行中 / 已完成 |
| `DragonDenyMount` | 這隻龍拒絕用坐騎 |

關卡完成判定（`LevelEvaluator`）會看這些決定「洗完了沒 + 額外目標達成沒」，直接影響關卡何時算 DONE。

### 3. 支線道具／目標
| 旗標 | 意思 |
|---|---|
| `HasMountFrame → DeliveredMountFrame → BuiltMount → PlacedMount → PlacedMountForUse` | 坐騎支線五階段：拿到骨架 → 交付 → 組裝 → 放置 → 放好可用 |
| `MedkitCompleted` | 醫藥箱／繃帶目標完成 |
| `PicnicPlaced` / `PicnicCompleted` | 野餐擺好 / 完成 |

### 4. 對話與感情線
| 旗標 | 意思 |
|---|---|
| `has_talked_to_ryan`, `has_asked_question`, `suggested_ryan_to_conrad` | 對話分支門檻（講過就不再重複） |
| `conrad_used_mount_3`, `conrad_jerked_off_3` | Conrad 第 3 次來訪用了哪種方式，影響後續對話 |
| `*_romanced` | 該角色感情線解鎖 |
| `*_sex_scene` | 過場動畫**已排入佇列** |
| `finished_watching_*_sex_scene` | 該過場**已看過** |

最後兩個最重要：每次載入關卡時，若 `X_sex_scene` 為 true 但 `finished_watching_X` 為 false，遊戲會**先播過場**而不是開始關卡。跳關功能會自動把已觸發的過場標成已看過；若想練習過場後的段落，取消勾選 `finished_watching_*` 再 Reload 即可從過場開始。

`Yarn.Internal.Once.line:xxxx` 是 Yarn 的「這句台詞只講一次」標記，對進度無影響，工具內不顯示。

## 從原始碼建置

需要 .NET SDK 7+ 與已安裝的遊戲（專案直接參考遊戲的 DLL，repo 不含這些檔案）。

```powershell
.\pack.ps1 -GameDir "D:\SteamLibrary\steamapps\common\Drag'n Wash"
# → dist\DragNWash.SpeedrunPractice-vX.Y.Z.zip
```

## 授權

MIT
