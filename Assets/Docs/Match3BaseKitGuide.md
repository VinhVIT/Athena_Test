# Match3 Base Kit (offline, 3-5 ngày)

## 1) Mục tiêu
Bộ sườn này tách riêng phần **core gameplay offline** để bạn ráp logic Match-3 nhanh:
- Không ads / IAP / analytics / server.
- Chỉ giữ vòng lặp game: `Boot -> Menu -> StartLevel -> Play -> Win/Lose -> Next/Retry`.
- Dùng event đơn giản để UI bám theo gameplay.

## 2) Cấu trúc đề xuất
- `Core/`:
  - `GameBootstrap`: entry point, giữ flow state.
  - `GameStateMachine`: đổi state game.
  - `LevelSession`: runtime data của 1 màn.
- `Gameplay/`:
  - `Match3Board`: quản lý grid, swap, match, collapse, refill.
  - `Match3RuleSet`: tham số board (width/height/moves/objectives).
  - `PieceView`: mapping data -> visual.
- `UI/`:
  - `HUDPresenter`: bind moves/score/goal.
  - `ScreenRouter`: bật/tắt màn hình Menu/Game/Result.

## 3) Mapping từ source Watermelon sang base kit
Trong source hiện tại, phần có thể xem là "xương sống" thường nằm ở:
- `GameController` (điều phối flow game + page UI).
- `LevelController` (nạp level, turn, win/lose check).
- Các page UI (`UIGame`, `UIMainMenu`, `UIComplete`, `UIGameOver`).

Base kit này giữ tinh thần đó nhưng tối giản dependency để làm test Match-3 nhanh.

## 4) Quy trình ráp Match-3 trong vài ngày
1. Tạo prefab `GameBootstrap` + gán `ScreenRouter`, `HUDPresenter`, `Match3Board`.
2. Tạo `Match3RuleSet` ScriptableObject cho 1-3 level mẫu.
3. Implement logic trong `Match3Board` theo TODO:
   - `TrySwap`
   - `FindMatches`
   - `ResolveCascade`
4. UI chỉ nghe event từ `LevelSession`.
5. Khi ổn mới thêm polish: FX, combo text, audio, tutorial nhẹ.

## 5) Những thứ bỏ hẳn cho bài test
- AdsManager, IAP, lives online, remote config, firebase, attribution.
- Bất kỳ service nào cần network hoặc SDK ngoài gameplay.

