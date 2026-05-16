# Skyjoust — Unity Setup Guide

A *Joust* clone where you pilot an **aircraft** that **joust-rams enemy drones**
and **collects power orbs**. This repo contains the C# gameplay scripts and the
project skeleton; you assemble the scene, prefabs and sprites in the Unity
Editor (scenes/prefabs are binary-ish YAML and are not generated here).

## 1. Open the project

- Built against **Unity 6 (6000.0 LTS)**. Open the repo folder with Unity Hub;
  let it import. `Library/` regenerates locally and is git-ignored.
- **Using Unity 2022.3 LTS instead?** The scripts call `Rigidbody2D.linearVelocity`
  (Unity 6 API). Find/replace `linearVelocity` → `velocity` across `Assets/Scripts/`.

## 2. Assets to create

Generate art with an image tool (e.g. Gemini Pro), then import each PNG as
**Texture Type = Sprite (2D and UI)** with a consistent **Pixels Per Unit**
(100 works) and **Center** pivot for the craft/orbs.

> The scripts tint the **drone** and **power orb** via `SpriteRenderer.color`
> (per-tier colors / per-orb-kind colors). Generate those two as **white /
> light-grey** art only, or the tint will not read. The aircraft, platforms,
> lava and background are used at full color.

### Sprites

| File (`Assets/Sprites/`) | Size | Notes |
|--------------------------|------|-------|
| `aircraft.png`     | ~256×128 | Player craft, full color, faces **right** (code flips X), transparent bg. |
| `drone.png`        | ~256×128 | Enemy craft, **white/light-grey only** (tinted per tier), faces right. |
| `orb.png`          | ~128×128 | Power orb, **white/near-white** (tinted per kind). |
| `shield.png`       | ~256×256 | Translucent bubble, child of the aircraft, starts disabled. |
| `platform.png`     | ~512×64  | Floating ledge, tileable horizontally. |
| `ground.png`       | ~1024×128| Bottom floor strip, tileable. |
| `lava.png`         | ~1024×128| Molten lava surface, bright. |
| `background.png`   | 1920×1080| Static sky/space backdrop. |
| `spark.png`        | ~64×64   | One white soft-circle texture, reused by all Particle Systems. |

### UI

| File | Notes |
|------|-------|
| `logo.png`   | "SKYJOUST" title, ~1024×512, transparent. |
| `button.png` | Rounded rectangle, ~256×96, 9-slice friendly. |
| `panel.png`  | Dark semi-transparent rounded panel for menu/pause/game-over. |
| Font         | Image tools cannot make fonts — grab a free arcade font (e.g. "Press Start 2P") and import it. |

### Audio (not image-generatable)

Source from a free library or audio AI: thrust loop, joust hit, orb pickup,
drone explosion, player death, wave-start fanfare, background music loop.

### Prompts

A detailed, copy-paste generation prompt for every asset above lives in
**`ASSET_PROMPTS.md`**. Quick reminders:

- Ask for the subject **isolated on a transparent (or plain contrasting)
  background**; remove leftover background before import.
- For `drone.png` / `orb.png` explicitly request **white and light-grey only**.
- Keep all sprites **side-view, flat/vector style** for a consistent look.

## 3. Layers & physics

Create these layers (Edit ▸ Project Settings ▸ Tags and Layers):
`Player`, `Enemy`, `Orb`, `Ground`, `Lava`.

In **Physics 2D ▸ Layer Collision Matrix**, leave defaults except:
- `Orb` collides with `Ground` only — uncheck `Orb`×`Player`, `Orb`×`Enemy`,
  `Orb`×`Orb`, `Orb`×`Lava`. (Orbs rest on platforms; the player picks them up
  by proximity, not by physical collision.)
- Set global **Gravity Y** to about `-18` (Edit ▸ Project Settings ▸ Physics 2D).

## 4. Scene layout (`Assets/Scenes/Main.unity`)

Create a new scene and add:

| Object        | Components / notes |
|---------------|--------------------|
| Main Camera   | Orthographic, size ~6, tagged `MainCamera`. |
| `GameManager` | `GameManager` script. |
| `WaveSpawner` | `WaveSpawner` script. |
| Spawn points  | 4–6 empty GameObjects over the platforms; assign to `WaveSpawner.spawnPoints`. |
| `PlayerSpawn` | Empty GameObject mid-air; assign to `GameManager.playerSpawn`. |
| Platforms     | Sprites with `BoxCollider2D`, layer `Ground` (one ground strip + a few floating ledges). |
| `Lava`        | Wide `BoxCollider2D` at the bottom, **Is Trigger** on, layer `Lava`, `LavaHazard` script. |
| `Canvas`      | UI — see step 7. |

## 5. Player prefab (`Assets/Prefabs/Aircraft.prefab`)

- Sprite for the aircraft hull, layer `Player`.
- `Rigidbody2D`: Gravity Scale `1`, Freeze Rotation Z **on**, Collision Detection `Continuous`.
- `BoxCollider2D` (or Polygon) roughly matching the hull.
- `AircraftController` script.
- `ScreenWrap` script.
- Optional children: a thrust `ParticleSystem` → `thrustVfx`; a `shieldVisual`
  child (a translucent circle, start disabled) → `shieldVisual`.
- Assign the prefab to `GameManager.playerPrefab`.

## 6. Enemy & orb prefabs

**`Drone.prefab`** — layer `Enemy`, `Rigidbody2D` (freeze rotation Z),
`Collider2D`, `EnemyDrone` + `ScreenWrap`. Assign a child `SpriteRenderer` to
`body` (it gets tinted per tier). Assign `orbPrefab` and a `destroyVfx`
`ParticleSystem`. Assign `Drone.prefab` to `WaveSpawner.dronePrefab`.

**`PowerOrb.prefab`** — layer `Orb`, `Rigidbody2D` (gravity scale ~0.5),
`CircleCollider2D` (solid, not trigger), `PowerOrb` + `ScreenWrap`. Assign its
`SpriteRenderer` to `body`. Assign `PowerOrb.prefab` to `EnemyDrone.orbPrefab`.

> Particle VFX prefabs: set the Particle System's **Stop Action = Destroy** so
> spawned effects clean themselves up.

## 7. UI Canvas

Add a `Canvas` (Screen Space - Overlay) with a child `HUDController`. Create:
- HUD `Text` objects: score, wave, lives → assign to `HUDController`.
- `menuPanel` — title + **LAUNCH** button → OnClick `HUDController.OnStartButton`.
- `pausePanel` — **RESUME** button → `HUDController.OnResumeButton`.
- `gameOverPanel` — `finalScoreText` + **FLY AGAIN** button → `OnRestartButton`.

## 8. Controls

- **← / →** (or A/D) — steer
- **Space / ↑ / W** — thrust (tap to climb, hold for gentle lift)
- **P** — pause

## Gameplay rules

- Ram a drone while you are **higher** than it → drone destroyed, drops an orb.
- A drone that hits you from **above** costs a life. Equal altitude → harmless bounce.
- Touching the **lava** costs a life (drones that fall in are incinerated).
- **Orbs**: gold = +250 score · blue = 6s shield (destroys drones on contact) ·
  green = extra craft. Orbs fade after ~8s — grab them fast.
- Clear every drone to advance the wave; later waves add more and tougher drones.
