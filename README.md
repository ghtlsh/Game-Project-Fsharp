# Legend of Nobody — CLI Turn-Based Battle Game

A command-line turn-based battle game built with **F# / .NET 10**.

You play as a **🧙 Player** who faces a fearsome **👹 Enemy**. Each turn you choose to **Attack** or **Defend**. Defeat the enemy before your HP hits zero!

---

## Getting Started

**Prerequisites**

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — verify with:
  ```
  dotnet --version
  ```
  The output should begin with `10.`. No other dependencies or global tools are required.

**Running the Game**

Clone or download the repository, then from the **project root directory** (where `Project.fsproj` lives):

```
dotnet run
```

**Build only (no run)**

```
dotnet build
```

**Publish a self-contained executable**

```
dotnet publish -c Release -r win-x64 --self-contained
```

Replace `win-x64` with `linux-x64` or `osx-x64` for other platforms.

---

## How to Play

When it is your turn, a status screen is displayed showing both characters' HP bars and the action menu:

```
╔════════════════════════════════════╗
║          TURN 3                    ║
╚════════════════════════════════════╝

🧙 Player: HP 14/30 [█████████░░░░░░░░░░░] | ATK 5
👹 Enemy : HP 5/20  [█████░░░░░░░░░░░░░░░] | ATK 4

================================================

Choose your action:
  1. ⚔️  Attack
  2. 🛡️  Defend
>
```

Type `1` or `2` and press Enter.

### Character Stats

| Character | HP | Attack |
|-----------|----|--------|
| Player 🧙 | 30 | 5      |
| Enemy  👹 | 20 | 4      |

### Combat Rules

- **Attack** — deals your base ATK damage to the enemy. Has a **20% chance** of landing a **Critical Hit** (2× damage).
- **Defend** — braces for the enemy's strike. The player forgoes their attack this turn and instead reduces **incoming damage by 50%**.
- After your action resolves, the **enemy always attacks** (unless it is already dead).
- The enemy also has a 20% critical hit chance on its attacks.

### Win / Lose Conditions

| Condition | Result |
|-----------|--------|
| Enemy HP reaches 0 | **You Win** 🎉 |
| Your HP reaches 0 | **Game Over** 💀 |

After the battle ends you are prompted to **play again** (`y`) or **quit** (`n`).

---

## Requirements Document

The game satisfies the following testable requirements:

1. The game runs from the command line with `dotnet run` and requires no additional setup beyond .NET 10.
2. The player character has 30 HP and 5 ATK. The enemy has 20 HP and 4 ATK.
3. Each turn the player is presented with exactly two actions: Attack and Defend.
4. Choosing Attack reduces the enemy's HP by the player's ATK value (5).
5. Attack has a 20% probability of dealing a Critical Hit (2× ATK = 10 damage).
6. Choosing Defend reduces incoming enemy damage this turn by 50%.
7. After the player acts, the enemy attacks the player (unless already dead).
8. The enemy has a 20% probability of landing a Critical Hit (2× ATK = 8 damage).
9. The HP bar changes colour (green → yellow → red) as HP decreases.
10. The game ends in victory when the enemy's HP reaches 0 or below.
11. The game ends in defeat when the player's HP reaches 0 or below.
12. After the game ends, the player is offered a play-again prompt (`y`/`n`).
13. Invalid input (anything other than `1` or `2`) re-prompts without crashing.

---

## Project Structure

```
Project/
├── Program.fs       # Entry point. Main game loop and turn execution
├── Domain.fs        # Core type definitions (Character, Action, GameState, GameResult, TurnResult)
├── GameLogic.fs     # Combat logic: damage calculation, defence reduction, turn processing
├── Display.fs       # Console rendering: HP bars, turn headers, win/lose screens, messages
├── Input.fs         # User input handling: action parsing, yes/no prompts
└── Project.fsproj   # .NET project file
```

### Module Responsibilities

**`Domain.fs`**  
Defines all shared types. `Character` holds HP and ATK. `GameState` tracks both combatants and whether the player is defending. `TurnResult` carries the new state and messages produced during a turn.

**`GameLogic.fs`**  
Game-rule functions. `processTurn` orchestrates a full turn: applies the player's chosen action, checks if the enemy survived, then applies the enemy's counter-attack. Critical-hit rolls (using `System.Random`) and defence reduction are also computed here.

**`Display.fs`**  
All `printfn`-based output. Generates a visual HP bar (`█░`) whose colour changes from green → yellow → red as HP falls. Handles the victory and game-over screens with coloured ASCII boxes.

**`Input.fs`**  
Reads raw console input and converts it into typed `Action` values. Uses tail-recursive loops to re-prompt on invalid input without throwing exceptions.

**`Program.fs`**  
Wires everything together. `gameLoop` is the recursive engine that calls `executeTurn` and checks `checkGameResult` after each turn. `mainLoop` wraps a full game run and handles the play-again prompt.

---

## Use of Large Language Models

This project was developed with the assistance of **Claude**.

### What the LLM was used for

Claude was used to generate the initial F# module structure (`Domain.fs`, `GameLogic.fs`, `Display.fs`, `Input.fs`, `Program.fs`) and to generate the first working draft of the combat logic and console rendering code. 

### What required manual changes or re-prompting

Two issues required re-prompting after reviewing the generated output.

- **Turn-order edge-case handling**: Claude's initial code always executed the enemy's counter-attack regardless of whether the enemy had already been reduced to 0 HP. The guard check (`if not (isAlive stateAfterPlayer.Enemy) then …`) had to be inserted in `GameLogic.fs`.

- **Box-drawing characters in the turn header**: The `printTurnHeader` function in `Display.fs` renders a bordered box around the turn number. Claude generated the bottom border using `╔` (top-left corner character) instead of `╚` (bottom-left corner character), producing a visually broken box. The incorrect character had to be identified and corrected.