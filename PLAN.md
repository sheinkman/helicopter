# Skyjoust — Project Plan

A *Joust* clone built in **Unity 6 (2D, Rigidbody2D physics)**. Instead of a
bird-riding knight, the player pilots an **aircraft** that **joust-rams enemy
drones** and **collects power orbs** dropped by the wrecks.

## 1. Vision

- Arcade single-screen flyer: thrust to climb, momentum-driven movement,
  horizontal screen wrap.
- Win a joust by colliding with an enemy while **higher** than it.
- Defeated drones drop **power orbs** — the core twist on the original.
- Endless escalating waves; chase a high score.

## 2. Core mechanics

| System | Behaviour |
|--------|-----------|
| Flight | Tap thrust for a sharp climb, hold for gentle lift; gravity always pulls down. |
| Joust  | Higher craft wins; equal altitude = harmless bounce; lower craft is destroyed. |
| Orbs   | Gold = +250 score · Blue = 6s shield · Green = extra craft. Fade after ~8s. |
| Hazard | Lava floor destroys any craft (drones incinerate with no reward). |
| Waves  | Clear all drones to advance; later waves add more and tougher (tiered) drones. |
| Lives  | Start with 3 craft; brief invulnerability on respawn. |

## 3. Architecture (scripts — DONE)

All in `Assets/Scripts/`:

- `GameManager` — run state, score/lives, wave progression, player lifecycle.
- `AircraftController` — player flight input + joust resolution.
- `EnemyDrone` — AI flight, tiered difficulty, orb drops.
- `PowerOrb` — score/shield/extra-life pickups with timed fade.
- `WaveSpawner` — spawns escalating waves, reports wave-cleared.
- `ScreenWrap` — horizontal wrap-around.
- `LavaHazard` — destroys anything touching the lava trigger.
- `HUDController` — HUD + menu/pause/game-over panels.

## 4. Assets needed

Generate art with an image tool (e.g. Gemini Pro). The **drone** and **orb**
must be **white/light-grey** — the scripts tint them at runtime.

### Sprites (`Assets/Sprites/`)
- `aircraft.png` (~256×128) — player craft, full color, faces right.
- `drone.png` (~256×128) — enemy craft, white/grey only (tinted per tier).
- `orb.png` (~128×128) — power orb, white/near-white (tinted per kind).
- `shield.png` (~256×256) — translucent shield bubble.
- `platform.png` (~512×64), `ground.png` (~1024×128) — tileable.
- `lava.png` (~1024×128) — bright molten surface.
- `background.png` (1920×1080) — static sky/space backdrop.
- `spark.png` (~64×64) — white soft-circle, reused by all particle systems.

### UI
- `logo.png` ("SKYJOUST" title), `button.png`, `panel.png`.
- A free arcade font (e.g. "Press Start 2P") — image tools cannot make fonts.

### Audio (not image-generatable)
- Thrust loop, joust hit, orb pickup, drone explosion, player death,
  wave-start fanfare, background music loop.

> A detailed generation prompt for every asset lives in `ASSET_PROMPTS.md`.
> Full import specs (Pixels Per Unit, pivots, layers) live in `SETUP.md`.

## 5. Build phases

- [x] **Phase 0 — Code skeleton.** All gameplay scripts + project config.
- [ ] **Phase 1 — Assets.** Generate sprites, UI, audio per section 4.
- [ ] **Phase 2 — Scene & prefabs.** Build `Main.unity`, the Aircraft / Drone /
      PowerOrb prefabs, layers and the collision matrix (see `SETUP.md`).
- [ ] **Phase 3 — UI wiring.** Canvas, HUD text, menu/pause/game-over panels,
      button hookups.
- [ ] **Phase 4 — Playtest & tune.** Balance thrust/gravity forces, joust
      threshold, drone AI aggression, orb drop weights.
- [ ] **Phase 5 — Polish.** Particle effects, audio, score juice, build target.

## 6. Status

Phase 0 complete and pushed to `claude/joust-aircraft-powerups-X9mCs`.
Next up: generate assets (Phase 1), then assemble the scene in the Unity
Editor following `SETUP.md`.

## 7. Possible extensions

- Egg/hatch mechanic (Joust's wrecked drones leaving a "salvage" that
  re-hatches if not collected).
- Pterodactyl-style hazard enemy on long waves.
- Co-op second player.
- Persistent high-score table.
