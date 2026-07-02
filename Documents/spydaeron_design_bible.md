# Spydaeron Game Project — Living Design Bible
*Paste this at the start of each new session to restore full context.*

---

## The Developer

- Experienced C# developer with 20+ years of game development
- Built **Ecalpon** (an Ultima clone) — fully realized with combat engine, party system,
  THAC0 resolution, spell casting, ranged weapons, loot, experience
- Currently learning MonoGame via **TileResearch** repo (tile engine + camera + player movement working)
- Has 1978 AD&D books and SSI Gold Box games as design references
- Amateur musician — the world concept has a song: "Sutekh the Destroyer"
- Prefers to understand *why* code is written, not just copy-paste solutions
- Also has an ongoing 6502 assembly learning chat (keep separate)

---

## The Game Concept

A single-player roguelike RPG in the tradition of **Ultima III/IV** with
**SSI Gold Box tactical combat**. Small codebase, elegant systems, emergent complexity.

### The Elevator Pitch
A present-day scientist accidentally opens a dimensional gateway (*The Way*) and
is transported to antiquity — the era when Rome and Egypt were the great powers of
the western world. They discover a necklace they cannot remove, containing a trapped
entity named **Spydaeron**, whose ancient knowledge manifests as **neoscience** —
part psionics, part science, indistinguishable from magic to everyone else.
The goal: get home. The obstacle: Spydaeron engineered this reunion across millennia
and has its own agenda. The moral question: whose will are you actually serving?

---

## The World — Six Points of the Hexagram

The unicursal hexagram (30/60/120 degree angles, drawn in one continuous line)
is the game's central symbol — cosmologically, mechanically, and visually.

The six points are six world locations, each a node the player must visit:

| Node | Location | Thematic Role |
|------|-----------|---------------|
| 1 | **Egypt** | Source — where Spydaeron was first imprisoned |
| 2 | **Rome** | Apollonian extreme — order, hierarchy, sterility |
| 3 | **Carthage** | Shadow empire — Ba'al Hammon / Tanit, closest to synthesis |
| 4 | **Babylon** | First astronomers — science and wonder coexisting |
| 5 | **Avalon** | Liminal world — Druidic mysteries, the mists, seed of renaissance |
| 6 | **Ireland/Tír na nÓg** | Tuatha Dé Danann — Nuada's people, transformation's cost |

The **seventh point** — the center of the hexagram where all lines converge —
is where The Way was opened. The final destination.

### The Philosophical Spine
The game is NOT an elegy about magic fading. It is about a coming renaissance —
the synthesis of **Dionysus and Apollo**: ecstasy and reason, mystery and geometry,
the organic and the crafted. Neoscience is the technology of that synthesis.
The villain is not Spydaeron — the villain is **the will to keep people ignorant**.

---

## The Necklace (NAME TBD)

- **Material:** Electrum (the divine alloy of gold and silver, called *asem* in Egypt)
- **Pendant:** The unicursal hexagram in electrum
- **Center:** A flawless diamond — where Spydaeron is imprisoned
- **Key property:** Once put on, it **cannot be removed**
- **Visual:** The diamond changes appearance as power grows — dark at rest,
  cold inner light when drawing power, something *moving* inside when Spydaeron
  is actively present
- **The irremovability** is not mechanical — Spydaeron will not let go.
  It has been alone for millennia and has finally found a mind that can hear it clearly.

**The necklace needs a name** — one of the first tasks for next session.

---

## Spydaeron

- Trapped inside the diamond at the center of the necklace
- Name constructed from Greek *aer* (air/breath) — "the spider of the air"
- **Predates the gods.** Not a god — something older.
- The Egyptian priests didn't recognize it as a trapped entity — they experienced
  its influence as divine inspiration
- The pyramids' geometry (30/60/120 degrees) is Spydaeron's signature,
  encoded in stone by people who thought they were serving gods
- Has been whispering through the diamond to sensitive minds for millennia —
  but your scientist is the first person it can speak to *clearly*
- **Is not simply evil.** Was imprisoned because its knowledge threatened
  the power of priests and kings who derived authority from mystery
- Its ultimate purpose is the Dionysian/Apollonian synthesis —
  the renaissance of love and reason
- Inspired by: Spydron from the 1970s ITV series *The Tomorrow People*

---

## Sutekh

- Egyptian god of chaos, war, and the desert
- Son of Osiris, grandson of Ra, father of Anubis
- Fought Horus, lost, escaped his prison, took vengeance, was ultimately
  blessed by Ra and ruled Egypt for a thousand years
- In the game: not simply evil but the embodiment of **power that fears synthesis**
- He rules through chaos and terror because a world of love and reason
  has no need of a god of destruction
- He imprisoned Spydaeron because Spydaeron's knowledge would have made
  his divine authority obsolete
- The lyric "Your words – my hands / Sweet Sutekh! Take your fill" describes
  his relationship to the player character — he wants to use you as his instrument

---

## The Song — "Sutekh the Destroyer"
*(by the developer — full lyrics on file)*
Key design material extracted from the lyrics:
- "Neo-science has opened the Way" — the scientist's discovery is the inciting event
- "Your words – my hands" — Sutekh needs a vessel; recognizes the scientist
- "The Will of Love / As you will / Do what thou wilt" — Thelemic Law woven in
- "Strike down the God of the Sky" — Horus is the ultimate target of Sutekh's revenge

---

## Thelema & The Unicursal Hexagram

- Aleister Crowley's Law: "Do what thou wilt shall be the whole of the Law"
- True Will vs. ego's desires — the game's moral engine
- The unicursal hexagram: drawn in ONE continuous line without lifting the pen
  (unlike the Star of David which is two separate triangles)
- Angles: 30°, 60°, 120° — the complete harmonic set, all dividing cleanly into 360°
- The hexagram appears: in UI design, as map structure, as the magic system's
  progression path, as the moral spine
- Crowley was recovering (in his own mind) ancient Egyptian knowledge —
  historically accurate connection to your setting

---

## The Power System — Neoscience

- **Part psionics, part psionic** — the scientist doesn't see magic, they see geometry
- Source: Spydaeron's ancient accumulated knowledge channeled through the necklace
- Mechanical basis: **3rd Edition Psionics Handbook** (Bruce Cordell)
  adapted toward **1st Edition AD&D** sensibility
- Core engine: **Power Points** (mental fuel pool) — not Vancian memorize/fire/gone
- Six disciplines mapping to hexagram points and AD&D ability scores:

| Discipline | Ability | Role in your world |
|------------|---------|-------------------|
| Clairsentience | Wisdom | Seeing hidden truth / pattern recognition |
| Psychoportation | Dexterity | Interplanetary/interplanar travel |
| Psychokinesis | Constitution | Sutekh's destructive power |
| Telepathy | Charisma | Recruiting and influencing companions |
| Psychometabolism | Strength | Survival on alien worlds |
| Metacreativity | Intelligence | Ancient artifacts, hexagram geometry made physical |

- **The necklace as growing item:** Like the Golden Torc in Julian May's
  *Saga of the Pliocene Exile* — it doesn't give powers that aren't yours,
  it *unlocks* what was always there. Grows with experience.
- Psionic combat modes (attack/defense) from the 3e handbook are retained
  as a parallel combat layer

---

## The Arthurian / Celtic Thread

**Key influences:**
- Marion Zimmer Bradley's *Mists of Avalon* — Avalon as living spiritual technology,
  the goddess religion, the priestesses holding knowledge in trust
- Lloyd Alexander's *Prydain Chronicles* — the cost of transformation,
  the Summer Country, True Will vs. destiny imposed from outside
- John Boorman's *Excalibur* — Merlin's line: *"For it is the doom of men that they forget"*
  This is the game's thesis. Spydaeron's imprisonment succeeded because humanity keeps forgetting.

**Nuada of the Silver Hand:**
- First king of the Tuatha Dé Danann, lost his hand, received a silver prosthetic
  that worked better than the original
- Represents: the organic and artificial unified, the body enhanced beyond its origins
- **The scientist IS Nuada.** The necklace IS the silver hand.
- The four treasures of the Tuatha Dé Danann (Spear of Lugh, Sword of Nuada,
  Cauldron of the Dagda, Stone of Fál) are items with impossible properties —
  Spydaeron has been busy in Ireland

**Avalon specifically:**
- NOT the last refuge of dying magic — the *seed* of what's coming
- The mists as gameplay mechanic — requires the right kind of perception to navigate
- The priestesses have been holding knowledge in trust, waiting for conditions to be right
- The scientist arriving with Spydaeron is those conditions being right

---

## The Tomorrow People Connection

- 1970s ITV series: homo superior with telepathy, telekinesis, jaunting (teleportation)
- Key constraint: Tomorrow People **cannot kill** — psychically impossible
- Consider borrowing this as a neoscience constraint — creates moral gameplay tension
- Spydron in the show: malevolent intelligence, ancient and patient, trapped and waiting

---

## Combat System

**Model:** SSI Gold Box series (Pool of Radiance 1988 onwards)
**Rules:** AD&D 1st Edition (developer has original 1978 books)

### The Gold Box Anatomy
- Separate combat screen (clean transition from exploration)
- Top-down tile grid ~16x16
- Turn-based with visible initiative order
- Action menu: Attack / Move / Use Power / Use Item / Turn / Quit
- Text feedback panel showing exactly what happened and why
- THAC0 resolution (already proven in Ecalpon)

### What's New vs. Ecalpon
- **No database dependency** — attack matrices in memory as arrays/dictionaries
- **Proper state machine** — `CombatState` class instead of scattered global flags
- **Procedural formation placement** — context-sensitive, not hardcoded positions
- **Psionic layer** — power point pool running parallel to physical combat
- **Emergent behavior** — simple enemy rules combining unexpectedly

### Enemy Design Philosophy
Not hundreds of enemies — twenty enemies with **behaviors that combine**.
Example trio: Roman Legionary (shield wall when adjacent) +
Cult Priest of Sutekh (chaos blessing on adjacent enemies) +
Desert Jackal (targets lowest HP, ignores armor) = unique tactical puzzle every encounter.

### Companions
Recruitable NPCs with distinct **tactical identities** (different verbs, not just stats):
- Nubian mercenary — breaks formations
- Druidic priestess from Avalon — terrain manipulation
- Babylonian astronomer — predictive ability (sees enemy intentions)
- Roman deserter — formation exploitation from inside

---

## Space / Planar Exploration

- Draws from **Gamma World** (mutation, environmental strangeness, wrong-feeling planets)
- Draws from **Traveller** (space travel as procedural system with real costs)
- Six hexagram nodes include off-world destinations
- Every location connects back to the hexagram — not arbitrary like Ultima II's time travel
- Psychoportation discipline is the mechanical basis for travel

---

## Technical Foundation

**Engine:** MonoGame (C#)
**Current state:** TileResearch repo has working tile engine, camera, player movement
**Reference codebase:** Ecalpon (kept as design reference, not ported directly)

### Key Technical Decisions Made
- Overlay pattern: FightBase (terrain) + FightOverlay (entities) — proven in Ecalpon
- Numeric encoding for entity types in overlay — extend ranges for psionic layer
- Scale matrix for pixel-perfect 2x rendering — already in TileResearch
- PointClamp sampling — already in TileResearch

### Development Phases Planned
1. Combat foundation — grid, sprites, turn order, action menu, THAC0, text panel
2. Neoscience layer — power point pool, 3-4 psionic abilities
3. Companion system — multiple player-controlled characters
4. Enemy variety — behavioral rules and emergent combinations
5. World integration — exploration map, encounter triggers, persistence

---

## Open Questions (Next Session Priorities)

1. **Name the necklace** — needs a proper artifact name
2. **Name the protagonist** — player-defined or fixed?
3. **Name the game** — should encode something like "Ecalpon" (= Noplace) did
4. **Spydaeron's ultimate goal** — precisely what does it want to be free to do?
5. **The sixth companion** — who represents the Babylonian astronomical tradition?
6. **Start coding** — combat engine or exploration system first?

---

## Easter Eggs & Hidden Layers

- **Ecalpon** = "Noplace" backwards — the protagonist has no fixed place
- The unicursal hexagram hidden in UI elements throughout
- The 30/60/120 degree geometry encoded in map structures
- Game title should work on multiple levels when reversed or decoded
- Thelemic references visible to those who know, invisible to those who don't

---

*Last updated: Session 1 — World building and design bible established*
*Next session: Name the necklace, name the game, begin coding*
