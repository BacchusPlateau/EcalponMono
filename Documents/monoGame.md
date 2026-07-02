# MonoGame Development Context
*Companion to spydaeron_design_bible.md — covers coding decisions, lessons learned, and current state.*

---

## Developer Profile

- 30+ years of game development experience
- Built Dune Adventure (1997) in C++ with DirectDraw/DirectSound — used enumerations to control game phases, hand-rolled adjacency matrix pathfinding
- Built Ecalpon — a fully realized Ultima clone in C# with complete combat engine, THAC0 resolution, party system, spell casting, ranged weapons, loot, experience
- Strong preference for readable, explicit code over terse syntactic sugar
- Wants to understand WHY code is written, not just copy-paste solutions
- Prefers explicit if/else over ternary operators
- Uses => only for genuinely trivial single-expression properties
- LINQ chains are acceptable where they read naturally as data transformations

---

## Project Setup

- **Engine:** MonoGame (C#)
- **Project name:** Ecalpon (renamed from TileResearch)
- **Main game class:** `TheGame.cs` (renamed from `Game1.cs`)
- **Namespace:** `Ecalpon`
- **Combat namespace:** `Ecalpon.Combat`
- **Window size:** 1080x800
- **Scale:** 2x pixel scaling via `Matrix.CreateScale`
- **Sampling:** `SamplerState.PointClamp` for crisp pixel art
- **Content root:** `Content/`
- **Content folder structure:**
  - `Content/fonts/` — SpriteFont files
  - `Content/sheets/` — tilesets (t1)
  - `Content/sprites/` — character sprites (rogueSword)

---

## Working Systems (Pre-Combat)

The exploration map in `TheGame.cs` has:
- Tile-based map rendering from a hardcoded `int[,] map` array
- Tile IDs mapped to source rectangles on a spritesheet using `%` and `/`
- WASD grid-snapped movement (key-up detection prevents holding)
- Camera centered on player with visible tile range calculated dynamically
- Map bounds clamping via `MathHelper.Clamp`
- 2x scale matrix rendering with PointClamp sampling
- Player sprite (`rogueSword`) drawn on top of tile layer

---

## Combat System Architecture

### The Core Principle
Turn-based means the game waits for input. There is no timing. Nothing happens until a key is pressed. Enemy turns resolve immediately without delays.

### Mode Switching
A single flag in `TheGame.cs` controls which mode is active:
```csharp
private bool _inCombat = true; // true = combat screen, false = exploration
```
`Update` and `Draw` both branch on this flag. Eventually a real encounter trigger will set it — for now it's manual scaffolding.

### File Structure
```
Ecalpon/
    Combat/
        CombatState.cs
        Combatant.cs
        CombatManager.cs
        CombatScreen.cs
```

---

## CombatState.cs

The enum controlling what mode combat is in at any given moment. Only one state is ever active at a time — this prevents illegal combinations of boolean flags.

```csharp
public enum CombatState
{
    Initializing,
    PlayerTurn,
    SelectingMove,
    SelectingTarget,
    UsingPower,
    EnemyTurn,
    Resolving,
    Victory,
    Defeat
}
```

**Why its own file:** Every other combat class references this enum. Keeping it separate avoids verbose cross-class references and unnecessary dependencies.

---

## Combatant.cs

Represents anything that participates in combat — player character, companion, or enemy. Same class, different data. This avoids the parallel-hierarchy problem from Ecalpon where `CCharacter` and `CMonster` required duplicate logic everywhere.

### Key Design Decisions

**`IsAlive` as computed property** — one place to change if "alive" ever means something more nuanced than HP > 0.

**`IsPlayerControlled` as computed property** — the turn system never needs to ask "is this a character or a companion," just "is this player controlled." Companions and the scientist are handled identically.

**`HasActedThisTurn` as computed property** — derived from `AttacksRemainingThisTurn <= 0`. The `<= 0` rather than `== 0` means it can never be wrong regardless of how the count reached zero. Nothing sets this directly.

**`StartTurn()` method** — the combatant resets its own turn state. `CombatManager` calls `StartTurn()` without knowing what that involves internally. Moving MaxMoves and MaxAttacks reset inside the combatant follows black-box / encapsulation principles — the manager shouldn't know the implementation details of what resetting a turn means for a given combatant.

**`AttacksPerRound` vs `MaxAttacks`** — settled on `MaxAttacks` as the single source of truth. Having both was cruft, not a bug, but worth removing to avoid future confusion.

**Power points alongside hit points from the start** — every combatant has a power point pool even if it's zero for a Roman Legionary. Costs nothing, means the neoscience system slots in without restructuring later.

**`ThacO` stored on the combatant** — pre-calculated at creation, not looked up from a database during combat. Combat resolution becomes one subtraction and one comparison.

### Current Shape
```csharp
public enum CombatantType
{
    PlayerCharacter,
    Companion,
    Enemy
}

public class Combatant
{
    // Identity
    public string Name { get; set; }
    public CombatantType Type { get; set; }

    // Grid position
    public int GridRow { get; set; }
    public int GridCol { get; set; }

    // AD&D stats
    public int MaxAttacks { get; set; }
    public int AttacksRemainingThisTurn { get; set; }
    public int HitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int ArmorClass { get; set; }
    public int Level { get; set; }
    public int ThacO { get; set; }
    public int DamageMin { get; set; }
    public int DamageMax { get; set; }
    public int DamageBonus { get; set; }

    // Neoscience
    public int PowerPoints { get; set; }
    public int MaxPowerPoints { get; set; }

    // Turn management
    public int Initiative { get; set; }
    public bool HasActedThisTurn
    {
        get { return AttacksRemainingThisTurn <= 0; }
    }
    public int MovesRemaining { get; set; }
    public int MaxMoves { get; set; }

    // Computed state
    public bool IsAlive => HitPoints > 0;
    public bool IsPlayerControlled =>
        Type == CombatantType.PlayerCharacter ||
        Type == CombatantType.Companion;

    public int ExperienceValue { get; set; }

    public Combatant()
    {
        MaxAttacks = 1;
    }

    public void StartTurn()
    {
        AttacksRemainingThisTurn = MaxAttacks;
        MovesRemaining = MaxMoves;
    }
}
```

---

## CombatManager.cs

Owns all combat rules and data. Knows nothing about MonoGame, the screen, or keyboard input. If MonoGame were removed from the project, `CombatManager` should still compile.

### Responsibility Boundary
- Owns the combatant list and turn order
- Owns the current state
- Owns the current target (for SelectingTarget state)
- Resolves THAC0, damage, and death
- Maintains the message log
- Decides victory and defeat conditions

### Key Design Decisions

**`private set` on `CurrentState`** — outside world can read state, only the manager can change it.

**`TransitionTo` as the single state-change method** — all state transitions go through one place. The switch statement there calls appropriate setup logic for each state. This is where the Legionary bug lived — `EnemyTurn` case had a placeholder comment and never called `PrepareCurrentCombatantTurn`.

**`PrepareCurrentCombatantTurn` calls `current.StartTurn()`** — the manager triggers the moment, the combatant handles its own reset. The manager also adds the appropriate log message here since logging is the manager's concern, not the combatant's.

**`AdvanceToNextCombatant` with attempts guard** — the do/while loop skips dead combatants. The `attempts <= _combatants.Count` guard prevents infinite loop if somehow all combatants are dead simultaneously.

**Message log as `List<string>`** — not a Queue. Messages accumulate visibly, oldest at top, newest at bottom, capped at 8 entries by removing from index 0. A List was chosen over Queue because Queue's Dequeue pattern was swallowing messages before they could be rendered.

**Three separate attack resolution methods** — `RollToHit`, `RollDamage`, `ApplyDamage` called in sequence. Kept separate so future abilities can insert between them — a shield bash between hit and damage, a psionic deflection before ApplyDamage, etc.

**THAC0 math:**
```csharp
int needed = attacker.ThacO - defender.ArmorClass;
bool hit = roll >= needed;
```
Lower THAC0 is better. A fighter with THAC0 12 attacking AC 5 needs to roll 7+. A novice with THAC0 20 needs to roll 15+.

**`GetAliveCombatants()`** — public method returning only living combatants for the renderer to iterate.

### Target Selection (In Progress)
`CombatManager` needs to own:
- A field tracking the currently targeted `Combatant`
- A method to initialize targeting when entering `SelectingTarget` state
- Methods to cycle to next/previous valid target (alive enemies only, with wraparound)
- This keeps `CombatScreen` from ever touching the enemy list directly

---

## CombatScreen.cs

Translates MonoGame input into `CombatManager` instructions. Renders whatever state `CombatManager` is in. Dependency flows one direction only — `CombatScreen` depends on `CombatManager`, never the reverse.

### Grid Constants
```csharp
private const int GRID_COLS = 16;
private const int GRID_ROWS = 16;
private const int TILE_SIZE = 32;
private const int GRID_ORIGIN_X = 16;
private const int GRID_ORIGIN_Y = 16;
private const int PANEL_X = GRID_ORIGIN_X + (GRID_COLS * TILE_SIZE) + 16;
private const int PANEL_Y = 16;
```

### The 1x1 Pixel Texture Pattern
```csharp
_pixel = new Texture2D(graphicsDevice, 1, 1);
_pixel.SetData(new Color[] { Color.White });

private void DrawFilledRect(Rectangle rect, Color color)
{
    _spriteBatch.Draw(_pixel, rect, color);
}
```
One white pixel stretched to any rectangle, tinted to any color. Every filled rectangle in the entire UI comes from this.

### Input Dispatch Pattern
`Update` branches by `CurrentState` at the top level. Each state has its own dedicated handler method. They never run simultaneously, so each method only knows about keys valid for that specific state:

```csharp
if (_manager.CurrentState == CombatState.PlayerTurn)
    HandlePlayerInput();

if (_manager.CurrentState == CombatState.EnemyTurn)
    ResolveEnemyTurn();

if (_manager.CurrentState == CombatState.SelectingTarget)
    HandlePlayerTargeting();
```

### Key Bindings (PlayerTurn)
- A = Attack (transitions to SelectingTarget)
- M = Move (transitions to SelectingMove)
- P = Use Power (transitions to UsingPower)
- Space = End Turn

### HandlePlayerTargeting (In Progress)
```csharp
private void HandlePlayerTargeting()
{
    if (WasKeyJustPressed(Keys.Up) || WasKeyJustPressed(Keys.Right))
    {
        // cycle to next enemy — calls CombatManager method
        return;
    }

    if (WasKeyJustPressed(Keys.Down) || WasKeyJustPressed(Keys.Left))
    {
        // cycle to previous enemy — calls CombatManager method
        return;
    }

    // Enter = confirm target and attack
    // Escape = cancel, return to PlayerTurn
}
```

### Rendering
- **Grid:** Alternating colored rectangles as placeholder for tile art
- **Combatants:** Colored squares (blue = player, green = companion, red = enemy) with first initial drawn
- **Active combatant:** Yellow highlight border
- **Combat Log panel:** Dark background, gold title, messages in light gray, 8 lines max
- **Action Menu panel:** Shows available keys during PlayerTurn, shows current state name otherwise

---

## Combat Rules Established

### Initiative
```csharp
combatant.Initiative = _rng.Next(1, 20) + combatant.Level;
```
Sorted descending — highest goes first. Veterans react faster.

### THAC0 Resolution
```csharp
int roll = _rng.Next(1, 21);        // d20
int needed = attacker.ThacO - defender.ArmorClass;
bool hit = roll >= needed;
```

### Damage
```csharp
int damage = _rng.Next(attacker.DamageMin, attacker.DamageMax + 1)
             + attacker.DamageBonus;
```

### Multiple Attacks (AD&D)
- High level fighters get multiple attacks per round
- Resolved separately and consecutively, not simultaneously
- Each attack can target a different enemy if the first target dies
- `MaxAttacks` on `Combatant` defaults to 1, overridden for high-level characters
- `AttacksRemainingThisTurn` decrements per attack, turn ends when it hits 0

---

## Test Combat Setup (in LoadContent)

```csharp
// Party
Name = "Scientist", PlayerCharacter, HP 20, AC 6, ThacO 20, Dmg 1-6+1, GridRow 12 Col 7

// Enemies
Name = "Legionary", Enemy, HP 12, AC 4, ThacO 20, Dmg 1-8, GridRow 4 Col 7
Name = "Jackal", Enemy, HP 6, AC 7, ThacO 19, Dmg 1-4, GridRow 3 Col 9
```

Proven turn order: Jackal (index 1) → Scientist (index 2) → Legionary (index 0)
All three cycle correctly — confirmed via debug output.

---

## Bugs Found and Fixed

**The Legionary placeholder bug** — `TransitionTo` had a comment-only placeholder for `CombatState.EnemyTurn`, so `PrepareCurrentCombatantTurn` was never called for any enemy. Fixed by adding `PrepareCurrentCombatantTurn()` call to the `EnemyTurn` case. Found by placing a breakpoint on the log message that was never being hit.

---

## Lessons Learned This Session

- Write one concept at a time, understood before moving on
- Never replace working code wholesale — surgical ADD/REPLACE/NEW FILE edits only
- Placeholder comments in switch cases are dangerous — they compile and silently do nothing
- Use the debugger to find bugs before writing more code to fix symptoms
- Turn-based means the game waits for input — no timing delays needed or wanted
- Black-box / encapsulation principle: `CombatManager` calls `StartTurn()` without knowing what it does internally
- Derived/computed properties (`HasActedThisTurn`, `IsAlive`) eliminate entire categories of bugs where flags and counts could disagree

---

## What's Next

Target selection needs to be completed:
1. `CombatManager` needs a current target field and cycle methods
2. `HandlePlayerTargeting` calls those methods for arrow keys
3. Enter confirms and calls `RollToHit` → `RollDamage` → `ApplyDamage`
4. Escape cancels back to `PlayerTurn`
5. After attack resolves, decrement `AttacksRemainingThisTurn`
6. If `HasActedThisTurn` is true, end the turn automatically

After Attack works end to end:
- Combat grid visual theme (pseudo-isometric Gold Box style)
- Terrain features as static obstacles (columns, statues)
- Interactive world objects in combat — noted as important design requirement, not just Gold Box scenery
- Enemy AI (move toward nearest player combatant, attack if adjacent)
- Movement system
- Neoscience / power point system

---

## Important Design Notes for Future Sessions

**Object interaction** is a first-class citizen, not an afterthought. The biggest complaint about Gold Box games was very linear gameplay with almost no interaction with the world outside of combat. The goal is to balance tactical combat with object examination and manipulation similar to King's Quest or Ultima IV.

**Combat map should reflect explorer map context** — if you're in a temple, the combat grid should have columns and statues in positions matching the explorer map. Special encounters can have unique interactive features: activating a device, interacting with an object mid-combat.

**Interactive terrain objects and explorer-mode interactive objects** should share underlying logic — write interaction once, works in both contexts.

**Multiple attacks per round** are resolved separately and consecutively, potentially against different targets.

*Last updated: Session 2 — Combat skeleton built, target selection in progress*
