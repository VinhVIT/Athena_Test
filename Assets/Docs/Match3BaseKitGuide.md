# MATCH-3 TECHNICAL TEST
# Cách tiếp cận bài toán

Khi bắt đầu project, mình ưu tiên giải quyết theo thứ tự:

1. Gameplay flow cơ bản
2. Board generation
3. Dữ liệu level
4. Session/game state
5. Tách gameplay logic khỏi UI

mình muốn xây nền architecture đủ rõ ràng trước và add các feature như:
- swapping,
- matching,
- cascading,
- refill,
- animation,
- special tiles

có thể được thêm vào sau mà không cần rewrite nhiều hệ thống cũ.

---

# Cấu trúc project

## 1. GameBootstrap

`GameBootstrap` đóng vai trò entry point chính của game.

Class này chịu trách nhiệm:
- khởi tạo các system,
- bind dependencies,
- start/restart level,
- điều phối gameplay flow.

Mình dùng class này để tránh việc logic gameplay bị phân tán ở nhiều nơi.

---

## 2. GameStateMachine

`GameStateMachine` được dùng để quản lý flow trạng thái của game.

Các state hiện tại:
- Boot
- Menu
- Playing
- Result

Mình tách state machine riêng thay vì dùng boolean flags vì:
- dễ mở rộng,
- dễ debug,
- tránh logic bị chồng chéo khi project lớn hơn.

---

## 3. LevelSession

`LevelSession` dùng để quản lý runtime data của một màn chơi:
- moves,
- score,
- trạng thái hoàn thành level.

Class này hoạt động như một lightweight session model.

UI không tự quản lý dữ liệu mà chỉ subscribe event từ session:
- `OnMovesChanged`
- `OnScoreChanged`
- `OnLevelChanged`

Điều này giúp gameplay logic và UI không phụ thuộc trực tiếp vào nhau.

---

## 4. Match3Board

`Match3Board` là nơi xử lý board gameplay.

Chức năng hiện tại:
- tạo board,
- spawn tile,
- lưu tile bằng mảng 2 chiều,
- đảm bảo board khởi tạo không có sẵn match.

Mình sử dụng `Tile[,]` vì:
- match-3 là bài toán grid-based tự nhiên,
- truy cập tile lân cận đơn giản,
- dễ implement match detection/cascade sau này.

Ví dụ:
- trái: `[x - 1, y]`
- dưới: `[x, y - 1]`

---

# Logic tạo board ban đầu

Một yêu cầu quan trọng của bài test là:
- board khởi tạo không được có match sẵn.

Để xử lý điều này:
- mỗi lần spawn tile,
- hệ thống sẽ kiểm tra nếu tile mới tạo ra horizontal hoặc vertical match thì tile đó sẽ bị reject.

Logic này nằm trong:
- `SetupRandomTileWithoutMatch()`
- `CreatesMatchAt()`

Thay vì generate toàn bộ board rồi reroll lại toàn bộ nếu invalid,
mình chọn validate ngay trong lúc spawn vì:
- đơn giản hơn,
- dễ kiểm soát,
- ít phát sinh xử lý dư thừa.

---

# Dữ liệu và khả năng mở rộng

Mình dùng `ScriptableObject` cho:
- `LevelData`
- `TileData`

Điều này giúp:
- dễ config level,
- dễ thêm loại tile mới,
- tách data khỏi gameplay code.

Ví dụ:
- board size,
- move limit,
- tile types

đều có thể chỉnh trực tiếp trong Inspector.

---

# UI Structure

UI hiện tại được chia thành:
- `HUDPresenter`
- `ScreenRouter`

## HUDPresenter
Chỉ chịu trách nhiệm hiển thị:
- score,
- moves,
- level.

Không chứa gameplay logic.

## ScreenRouter
Quản lý:
- menu screen,
- gameplay screen,
- result screen.

Việc tách riêng UI routing giúp code gameplay không cần biết chi tiết về UI implementation.

---

# Những điểm mình ưu tiên

Trong project này mình ưu tiên:
- readability,
- separation of responsibility,
- extensibility,
- predictable data flow.

Mình cố tránh:
- singleton không cần thiết,
- coupling mạnh giữa gameplay và UI,
- over-engineering quá sớm.

---

# Những phần chưa hoàn thành

Hiện tại project vẫn đang trong quá trình phát triển thêm các feature:
- swap input,
- match detection hoàn chỉnh,
- cascading,
- refill system,
- valid move detection,
- reshuffle,
- animation.

Tuy nhiên phần nền architecture đã được chuẩn bị để các feature này có thể bổ sung dần mà không cần thay đổi cấu trúc chính.

---

# Nếu có thêm thời gian

Mình muốn cải thiện thêm:
- object pooling cho tile,
- command/event system rõ ràng hơn,
- animation pipeline,
- special tiles,
- combo system,
- board simulation riêng cho AI/testing,
- automated tests cho match validation.

---

# AI Usage

Mình có sử dụng AI tools để:
- tham khảo hướng tổ chức code,
- review naming,
- thảo luận architecture,
- và kiểm tra một số edge cases.

Tuy nhiên toàn bộ logic gameplay chính đều được mình tự điều chỉnh và hiểu rõ trước khi sử dụng.

Một ví dụ AI đưa ra giải pháp chưa phù hợp:
- đề xuất generate toàn bộ board rồi reroll lại nếu có match.
- mình không chọn cách này vì dễ gây xử lý dư thừa khi board lớn hơn.

Thay vào đó mình kiểm tra match ngay trong quá trình spawn từng tile để tối giản logic hơn.