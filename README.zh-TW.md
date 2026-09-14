# DragNWash.SpeedrunPractice

[English](README.md) | **繁體中文**

*Drag'n Wash* 的 speedrun 練習工具（BepInEx 插件）。

> ### 快速安裝
> 1. Steam → 對 *Drag'n Wash* 按右鍵 → **管理** → **瀏覽本機檔案**
> 2. 到 **[Releases](../../releases/latest)** 下載 zip：
>    - 沒裝過 BepInEx（大多數玩家）→ **`-full.zip`**
>    - 已經有 BepInEx 5 → **`-plugin-only.zip`**
> 3. 把 zip 裡的全部內容拖進剛才打開的遊戲資料夾
> 4. 啟動遊戲，載入存檔後按 **F1**
>
> 詳細說明：[安裝](#安裝)

## 功能

- **F1** 練習選單：目前關卡 / 龍的狀態 / 清潔百分比 / 每關計時
- **關卡選擇**：直接跳到任一關。劇情旗標會**從空白重建**成正常 run 打到該關時的狀態（關卡里程碑、坐騎、野餐）；感情線一律不開，需要時在旗標清單勾選
- **龍的狀態單步推進**（**F8**）：`WaitingToAppear → WalkingToWindow → WaitingToBeLetIn → … → Exited`，一次前進一格
- **F4** 從本關開頭重來
- **F6** 一鍵洗乾淨、**F7** 跳過本關（呼叫遊戲內建的 debug 功能）
- **過場動畫**：四個過場（Ryan+Conrad / Ryan / Conrad / Alexander）任選一個直接載入。播完後遊戲會標記已看過、存檔並回到關卡（Alexander 的會接到片尾）。若想走正式流程觸發，跳到對應關卡（見下表）、勾上對應的 `*_romanced` 旗標、打完該關按出口即可
- 天氣切換、慢動作（Time Scale）滑桿
- 劇情旗標即時勾選，附上每個旗標的說明

> 注意：跳關與重來會覆寫**目前存檔槽**的 `savegame.dgn`（遊戲重載關卡的唯一途徑）。建議用專門的存檔槽練習。遊戲的「新遊戲」選到已有存檔的槽**不會清空它**，要重新開始請在存檔畫面用「刪除」。

## 友站連結：本地化補丁

**[Drag'n Wash 非官方本地化補丁](https://github.com/nekodakohaku-dev/Drag-n-Wash_Localization)** — 提供**繁體中文、簡體中文、日文**，翻譯遊戲內的對話、選項與介面文字（BepInEx + XUnity AutoTranslator）。想用中文玩這款遊戲的話，強烈推薦先裝這個。

兩個專案都基於 BepInEx 5，可以同時使用。若你已經裝了本地化補丁，代表 BepInEx 已經在了，直接抓下面的 **`-plugin-only.zip`** 即可。

## 安裝

先打開遊戲資料夾：Steam 收藏庫對 *Drag'n Wash* 按右鍵 → **管理** → **瀏覽本機檔案**，裡面有 `DragNWash.exe`。

### A. 我沒裝過 BepInEx（大多數玩家）

1. 從 [Releases](../../releases/latest) 下載 **`DragNWash.SpeedrunPractice-vX.Y.Z-full.zip`**
2. 打開 zip，把裡面**全部內容**拖進遊戲資料夾（和 `DragNWash.exe` 同一層），詢問是否覆蓋就選是
3. 啟動遊戲，載入存檔後按 **F1**

拖完後遊戲資料夾應該會多出 `winhttp.dll`、`doorstop_config.ini` 和一個 `BepInEx` 資料夾。第一次啟動會多花幾秒讓 BepInEx 初始化。

### B. 我已經有 BepInEx 5

1. 從 [Releases](../../releases/latest) 下載 **`DragNWash.SpeedrunPractice-vX.Y.Z-plugin-only.zip`**
2. 打開 zip，把裡面的 `BepInEx` 資料夾拖進遊戲資料夾（會和原本的合併）
3. 啟動遊戲，載入存檔後按 **F1**

兩種方式最後都會得到 `BepInEx/plugins/DragNWash.SpeedrunPractice/DragNWash.SpeedrunPractice.dll`。

### 移除

刪除 `BepInEx/plugins/DragNWash.SpeedrunPractice/` 即可。若要連 BepInEx 一起移除，再刪 `winhttp.dll`、`doorstop_config.ini`、`.doorstop_version`、`changelog.txt` 和 `BepInEx` 資料夾。

### 改鍵

第一次啟動後會產生 `BepInEx/config/dragnwash.speedrunpractice.cfg`，用文字編輯器改 `[Hotkeys]` 區段即可（鍵名為 Unity Input System 的 `Key` 列舉，例如 `F1`、`Backquote`、`Numpad0`）。

## 快捷鍵一覽

| 鍵 | 功能 |
|---|---|
| F1 | 開關練習選單 |
| F4 | 本關重來 |
| F6 | 一鍵洗乾淨 |
| F7 | 跳過本關 |
| F8 | 龍的狀態前進一格 |

## 疑難排解：龍開始走路時遊戲崩潰

如果遊戲在關卡（重新）載入後、龍剛開始走的瞬間崩潰，且 `Player.log` 裡出現 `UnityPlayer.dll` / `D3D12ScratchAllocator::DestroyScratch`，這是 Unity 6 的 Direct3D 12 問題，場景重載會提高發生機率（有沒有裝插件都可能發生）。請改用 Direct3D 11 執行：

1. Steam → 對 *Drag'n Wash* 按右鍵 → **內容** → **一般** → **啟動選項**
2. 填入 `-force-d3d11`

之後 `Player.log` 會顯示 `Forcing GfxDevice: Direct3D 11`。實測：在 D3D12 下每次必崩的存檔，改 D3D11 後正常。

## 遊戲版本對應

插件是對遊戲本身的組件編譯的，遊戲更新可能讓它失效。請依你的遊戲 build 選擇插件版本（build ID 可在 [SteamDB](https://steamdb.info/app/4739660/patchnotes/) 查，或看 `steamapps/appmanifest_4739660.acf` 裡的 `buildid`）。

| 插件版本 | 遊戲 build（Steam build ID） | 備註 |
|---|---|---|
| **v1.2.0 – v1.5.0** | **25286774**（2026-09-14）及之後 | 透過遊戲自己的存檔函式寫入；預期舊 build 也能用 |
| v1.0.0 – v1.1.1 | 25286774 之前的 build | 在 25286774 以上跳關會失效（該次更新把存檔換了目錄與格式） |

若遊戲更新後某功能失效，請開 issue 附上 build ID 以及 `BepInEx/LogOutput.log` 裡含 `Speedrun` 的行。

## 關卡順序

實際有 15 關（遊戲存檔畫面寫 `/ 14`，但存檔會到 15）。粗體為會觸發過場的關卡。

| # | 龍 | # | 龍 | # | 龍 |
|---|---|---|---|---|---|
| 1 | Ryan 1（教學） | 6 | Alexander 2 | 11 | **Conrad 4** → Ryan+Conrad 過場 |
| 2 | Conrad 1 | 7 | Ryan 3 | 12 | Alexander 4 |
| 3 | Alexander 1 | 8 | Conrad 3 | 13 | **Ryan 5** → Ryan 過場 |
| 4 | Ryan 2 | 9 | Alexander 3 | 14 | **Conrad 5** → Conrad 過場 |
| 5 | Conrad 2 | 10 | Ryan 4 | 15 | **Alexander 5** → Alexander 過場，接片尾 |

過場是由該關的 outro 對話排入的，條件是對應的 `*_romanced` 旗標為 true（outro 會依固定順序檢查感情線旗標，先中的先贏），之後在下一次載入關卡時播放。

## Story flags 說明

遊戲劇情用 **Yarn Spinner** 對話腳本驅動，每個旗標就是腳本裡的一個布林變數。存檔裡除了 `levelIndex` 之外只有這些旗標，所以旗標 = 劇情進度的全部。F1 選單的「Show story flags」可即時勾選。

### 1. 關卡進度標記（由 LevelFlow 自動設）
| 旗標 | 意思 |
|---|---|
| `level_1`, `level_5`, `level_6_started` | 該關**開始**時設 true |
| `level_1_complete`, `level_5_complete` | 該關**完成**時設 true |

跳關會把整組旗標從空白重建：前面關卡的里程碑、第 9 關起的坐騎流程、第 11 關起的野餐。這是盡力而為的推估——若某關看起來缺了什麼，在旗標清單勾上即可。

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

最後兩個最重要：每次載入關卡時，若 `X_sex_scene` 為 true 但 `finished_watching_X` 為 false，遊戲會**先播過場**而不是開始關卡。跳關會把所有感情線與過場旗標留在關閉；若想練習過場後的段落，勾上 `X_sex_scene`、取消 `finished_watching_X` 再 Reload 即可從過場開始。

`Yarn.Internal.Once.line:xxxx` 是 Yarn 的「這句台詞只講一次」標記，對進度無影響，工具內不顯示。

## 公平性

這是**練習**工具：會改寫存檔、略過遊戲邏輯，請勿在準備投稿的正式 run 中使用——移除或停用插件（刪除 `BepInEx/plugins/DragNWash.SpeedrunPractice/`），並確認你所屬社群對修改遊戲檔案的規定。

## 開發者備註

用於自動重現問題的環境變數（未設定時完全無作用）：`SRP_AUTOCONTINUE=1` 在主選單自動按 Continue、`SRP_AUTOJUMP=<關卡>` 進關後自動跳關、`SRP_AUTOMENU=gui|cursor|both` 自動開啟選單、`SRP_AUTORELOAD=<秒>` 延遲後重載。

## 從原始碼建置

需要 .NET SDK 7+ 與已安裝的遊戲（專案直接參考遊戲的 DLL，repo 不含這些檔案）。

```powershell
.\pack.ps1 -GameDir "D:\SteamLibrary\steamapps\common\Drag'n Wash"
# → dist\DragNWash.SpeedrunPractice-vX.Y.Z-full.zip        (BepInEx 5 + 插件)
# → dist\DragNWash.SpeedrunPractice-vX.Y.Z-plugin-only.zip (只有插件)
```

full 版內含 [BepInEx](https://github.com/BepInEx/BepInEx)（LGPL-2.1，授權文附在 `BepInEx/LICENSE.BepInEx.txt`）。

## 授權

MIT
