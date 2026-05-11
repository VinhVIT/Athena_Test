# MATCH-3 TECHNICAL TEST

# Problem-Solving Approach

When starting this project, I prioritized solving the problems in the following order:

1. Core gameplay flow
2. Board generation
3. Level data
4. Session/game state
5. Separating gameplay logic from UI

I wanted to establish a clear enough architecture first so that features such as:

* swapping,
* matching,
* cascading,
* refill,
* animation,
* special tiles

could be added later without requiring major rewrites of existing systems.

---

Since this project is intended as a technical test prototype, I intentionally avoided excessive refactoring or splitting classes too aggressively early on.

# Project Structure

## 1. GameBootstrap

`GameBootstrap` acts as the main entry point of the game.

This class is responsible for:

* initializing systems,
* binding dependencies,
* starting/restarting levels,
* coordinating gameplay flow.

I used this class to avoid gameplay logic being scattered across multiple places.

---

## 2. GameStateMachine

`GameStateMachine` is used to manage the game's flow states.

Current states:

* Boot
* Menu
* Playing
* Result

I separated the state machine instead of relying on boolean flags because it is:

* easier to extend,
* easier to debug,
* safer for avoiding overlapping logic as the project grows.

---

## 3. LevelSession

`LevelSession` is responsible for managing runtime data for a level:

* moves,
* score,
* level completion state.

This class acts as a lightweight session model.

The UI does not directly manage gameplay data and instead subscribes to session events:

* `OnMovesChanged`
* `OnScoreChanged`
* `OnLevelChanged`

This helps keep gameplay logic and UI loosely coupled.

---

## 4. Match3Board

`Match3Board` handles the board gameplay logic.

Current responsibilities:

* generating the board,
* spawning tiles,
* storing tiles in a 2D array,
* ensuring the initial board contains no pre-existing matches.

I used `Tile[,]` because:

* match-3 games are naturally grid-based,
* neighbor tile access becomes straightforward,
* it simplifies match detection and cascading logic.

Examples:

* left: `[x - 1, y]`
* below: `[x, y - 1]`

---

# Initial Board Generation Logic

One important requirement of this test is:

* the initial board must not contain pre-existing matches.

To solve this:

* each time a tile is spawned,
* the system checks whether the newly created tile forms a horizontal or vertical match. If it does, that tile is rejected.

This logic is handled inside:

* `SetupRandomTileWithoutMatch()`
* `CreatesMatchAt()`

---

# Core Gameplay Logic

Instead of generating the entire board and rerolling everything if invalid,
I chose to validate tiles during spawning because it is:

* simpler,
* easier to control,
* less wasteful.

The board is generated from bottom-left upward, which also simplifies match checking because only the left and bottom directions need to be validated during generation.

The match detection logic uses the selected tile as the center point and checks whether there are 3 or more connected tiles of the same type.

Additional gameplay systems such as:

* refill,
* shuffle,
* match finding

are separated into helper classes such as:

* `BoardRefiller`
* `BoardShuffler`
* `BoardFinder`

---

# Data and Extensibility

I used `ScriptableObject` for:

* `LevelData`
* `TileData`

This helps:

* simplify level configuration,
* make adding new tile types easier,
* separate data from gameplay code.

Examples such as:

* board size,
* move limit,
* tile types

can all be configured directly in the Inspector.

---

# UI Structure

The current UI is separated into:

* `HUDPresenter`
* `ScreenRouter`
* `BoardView`
* `FloatingText`
* `ScreenTransition`

## HUDPresenter

Responsible only for displaying:

* score,
* moves,
* level information.

It does not contain gameplay logic.

## ScreenRouter

Handles:

* menu screen,
* gameplay screen,
* result screen.

## BoardView

Responsible only for:

* board-related visuals.

Separating UI routing from gameplay logic helps keep gameplay systems independent from UI implementation details.

## SoundManager

`SoundManager` is responsible for managing:

* audio playback,
* sound effects,
* game-related audio events.

---

# Development Priorities

In this project, I prioritized:

* readability,
* separation of responsibilities,
* extensibility,
* predictable data flow.

I tried to avoid:

* unnecessary singletons,
* strong coupling between gameplay and UI,
* premature over-engineering.

---

# Level Editor

To add, edit, or remove levels, the custom Level Editor can be accessed through:

`Tools -> Match3 -> LevelEditor`

This allows easier level configuration directly inside the Unity Editor.

---

# Completed Features

* Core gameplay
* UI
* Basic animations
* Audio
* Basic save system using PlayerPrefs
* Level Editor

---

# Areas for Future Improvement

If given more time, I would improve:

* Animation quality and polish
* Better responsive support for the board layout
  (currently some uncommon mobile aspect ratios such as foldable devices may cause overflow issues)
* Custom board shapes
  (currently the board only supports rectangular layouts)
* Map progression system
  to allow players to track progress and replay older levels
* Additional meta systems such as:

  * shop,
  * battle pass,
  * missions,
  * achievements

However, the current architecture was designed so these features can be added incrementally without major structural changes.

---

# Technical Improvements

From a technical perspective, I would further improve:

* the Level Editor,
* automated level testing,
* code refactoring and cleanup.

---

# AI Usage

I used AI tools for:

* debugging,
* translation,
* reviewing code quality,
* comparing possible solutions and approaches.

However, all core gameplay logic was adjusted, implemented, and fully understood by myself before being integrated into the project.

One example where the AI suggested a suboptimal solution:

* generating the entire board first and rerolling the whole board if matches existed.

I decided not to use this approach because it could create unnecessary processing overhead as the board size increases.

Instead, I validated matches immediately during individual tile spawning to keep the logic simpler and more efficient.
