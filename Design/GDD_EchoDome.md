# Echo Dome — Game Design Document (Working Title)

## One-line concept
A narrative-driven 2D point-and-click exploration game inside a sealed biosphere after a failed sustainability experiment — players manage systems and uncover a directed story through logs, NPCs, and environmental cues.

## High-level goals
- Deliver environmental storytelling focused on discovery, not exposition.
- Create a moody exploration experience with emotional agency but a single authored narrative path.
- Present system-management tension (oxygen, water, power) that affects pacing and revelations, not the final outcome.

## Game pillars
- Environmental storytelling: story told through objects, plant states, and logs.
- Exploration & atmosphere: minimal spaces, strong mood, tight player movement.
- Emotional agency, authored narrative: player choices change pacing/amount of revealed story but not ending.
- Human vs system tension: ongoing life-support management creates stakes and emotional context.

## Core gameplay loop
1. Walk around a room.
2. Discover interactables.
3. Click/interact to trigger messages, logs, or small puzzles.
4. Read/inspect and learn environmental state or lore.
5. Manage systems to delay collapse, unlocking more content.
6. Repeat; story advances through time and key interactions.

## Controls
- Arrow keys / WASD: move player block
- Mouse click: interact with objects
- UI button / keyboard: close panels, advance text

## Prototype status (snapshot)
- Block-based movement: Complete
- Click-to-interact: Complete
- Message popup UI: Complete
- Multiple interactables: In progress
- Oxygen/system logic: Implemented (see `Assets/Scripts/Systems/LifeSupportManager.cs`)

## Setting & visual style
- Location: Echo Dome, sealed biosphere with control rooms, greenhouses, crew quarters, and airlocks.
- Prototype visuals: graybox / colored blocks.
- Target visuals: pixel art or minimalist 2D with retro-sci-fi vibes.

## Characters
- The Player: a biologist tasked with maintaining ecosystems.
- Dr. Maya Chen: botanist, pragmatic, skeptical of risky interventions.
- Alex Ramirez: systems engineer, trusts data and diagnostics.
- Mission Commander: authority figure, appears through logs/directives.
- Logs/Voices: disconnected crew fragments that reveal past dynamics.
- The System (optional): automated warnings/announcements that respond to system states.

## Biosphere systems (design overview)
- Atmosphere Control: O2/CO2 balance influenced by plants and scrubbers.
- Water Recycling: filters and condensers; failing units cause contamination/clues.
- Power Management: distributes electricity to subsystems; affects terminal availability.
- Greenhouses: primary O2 producers; plant states change visually and narratively.
- Waste Processing: affects contamination and possible narrative clues.
- Airlocks: later area with pressure/containment risks; higher stakes.

Design notes: Systems exist to create tension and gate content. They are simulation-lite: simple numerical states (percent, timers) that drive visuals, audio cues, and unlock rules.

## Narrative summary
You’re part of Experiment 3 — the last attempt at a sealed biosphere. Systems are failing in ways that echo prior attempts. The player must stabilize systems enough to uncover the crew’s fractured past. The ending is fixed; the journey determines how much and which emotional details are revealed.

## Narrative design approach
- Single authored ending.
- Player actions change pace and depth of revelation, not the outcome.
- Story gating: some logs or events require system thresholds (example: oxygen > 60% to access Log B).
- NPC dialogue and plant states alter tone and perceived agency.

## Player-facing progression & gating examples
- Time + interactions unlock new logs.
- System thresholds gate optional logs / terminals.
- Fixing systems delays the next scripted narrative beat, allowing more exploration.

## Room: Crew Quarters (Starting hub)
- Purpose: teach controls, tone, and basic puzzles; emotional anchor.
- Interactables:
  - Bed + Shelf: hidden journal, personal effects.
  - Wall Notes: one fluff, one clue.
  - Left Terminal: system logs & crew assignments.
  - Right Terminal: password-locked (clue-based puzzle).
  - NPC: early dialogue scaffolding.
  - Floor Hatch (future): unlock-area mechanic.

## Greenhouse scene (design)
- Central Plant Beds: inspect health; one mutated bed triggers a log.
- Left Terminal: shows environment stats; partially broken.
- Sample Storage Cabinet: locked drawer with notes.
- Ceiling Irrigation: inspectable; fixing affects plant health.
- NPC (Alex): gives mechanical insight / access after trust.
- Wall Logs & Compost Bin: small lore pieces and clues.

## Dynamic plant states
- Thriving: bright, lush visuals; positive NPC tone.
- Wilting: droop/yellow; NPCs show concern.
- Sick: brown spots; system warnings escalate.
- Dead: gray/brittle; heightened tension and narrative weight.

Plants serve as visible feedback loops that reflect system states and nudge player decisions.

## Design contract (minimal)
- Inputs: player position, clicks, item states, system actions (toggle power, repair).
- Outputs: UI messages, terminal logs, plant visuals, NPC dialogue, system state changes.
- Success criteria: player can read key logs before the final scripted beat if they manage systems; no branching endings; player feels agency via pacing and access.

## Edge cases & mitigations
- Player ignores all systems: ensure scripted beats still expose essential story (fallback reveals).
- Rapid toggling of systems: debounce interactions and show clear feedback.
- Accessibility: include text speed settings and clear visual cues.

## Technical notes (prototype)
- Engine: Unity (2D); existing workspace indicates Unity project structure.
- Movement: tile/block-based system already implemented.
- UI: existing message popup UI to be used for terminals and logs.
- Systems: a `LifeSupportManager` is present at `Assets/Scripts/Systems/LifeSupportManager.cs` implementing oxygen and power drain mechanics, UI text bindings, and repair methods. Oxygen drain rate increases when power drops below a threshold.
- Data: store logs and plant states as ScriptableObjects or JSON for easy iteration.

## Next dev goals (scoped)
- Add 2–3 more interactables with unique messages (short term).
- Hook plant visuals and NPC reactions to the existing `LifeSupportManager` oxygen thresholds (Thriving >70, Wilting 50–70, Sick 25–50, Dead <25).
- Add a second room and transitions (door/scene link).
- Introduce NPC dialogues and unlock a few logs.
- Add a single decision point: temporary choice to fix a system now vs unlock narrative content.

## Metrics & testing
- Verify oxygen system changes plant visuals at thresholds (70/50/25%).
- Test that at least one log is gated by a threshold and becomes available when met.
- Include a quick automated smoke test for UI panels and movement.

## Appendices
- Asset ideas: retro UI fonts, low-res plant sprites in 4 states, ambient soundtrack loops, UI SFX for clicks/alarms.
- Potential expansions: branching DLC, multiplayer observation mode (design note only).

---

Created for iteration in-repo. If you want, I can: 1) convert logs into JSON/ScriptableObjects, 2) wire a simple oxygen system script in `Scripts/Systems/`, or 3) add placeholder pixel art assets under `Assets/Sprites/`.
