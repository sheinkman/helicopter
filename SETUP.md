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

## 2. Layers & physics

Create these layers (Edit ▸ Project Settings ▸ Tags and Layers):
`Player`, `Enemy`, `Orb`, `Ground`, `Lava`.

In **Physics 2D ▸ Layer Collision Matrix**, leave defaults except:
- `Orb` collides with `Ground` only — uncheck `Orb`×`Player`, `Orb`×`Enemy`,
  `Orb`×`Orb`, `Orb`×`Lava`. (Orbs rest on platforms; the player picks them up
  by proximity, not by physical collision.)
- Set global **Gravity Y** to about `-18` (Edit ▸ Project Settings ▸ Physics 2D).

## 3. Scene layout (`Assets/Scenes/Main.unity`)

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
| `Canvas`      | UI — see step 6. |

## 4. Player prefab (`Assets/Prefabs/Aircraft.prefab`)

- Sprite for the aircraft hull, layer `Player`.
- `Rigidbody2D`: Gravity Scale `1`, Freeze Rotation Z **on**, Collision Detection `Continuous`.
- `BoxCollider2D` (or Polygon) roughly matching the hull.
- `AircraftController` script.
- `ScreenWrap` script.
- Optional children: a thrust `ParticleSystem` → `thrustVfx`; a `shieldVisual`
  child (a translucent circle, start disabled) → `shieldVisual`.
- Assign the prefab to `GameManager.playerPrefab`.

## 5. Enemy & orb prefabs

**`Drone.prefab`** — layer `Enemy`, `Rigidbody2D` (freeze rotation Z),
`Collider2D`, `EnemyDrone` + `ScreenWrap`. Assign a child `SpriteRenderer` to
`body` (it gets tinted per tier). Assign `orbPrefab` and a `destroyVfx`
`ParticleSystem`. Assign `Drone.prefab` to `WaveSpawner.dronePrefab`.

**`PowerOrb.prefab`** — layer `Orb`, `Rigidbody2D` (gravity scale ~0.5),
`CircleCollider2D` (solid, not trigger), `PowerOrb` + `ScreenWrap`. Assign its
`SpriteRenderer` to `body`. Assign `PowerOrb.prefab` to `EnemyDrone.orbPrefab`.

> Particle VFX prefabs: set the Particle System's **Stop Action = Destroy** so
> spawned effects clean themselves up.

## 6. UI Canvas

Add a `Canvas` (Screen Space - Overlay) with a child `HUDController`. Create:
- HUD `Text` objects: score, wave, lives → assign to `HUDController`.
- `menuPanel` — title + **LAUNCH** button → OnClick `HUDController.OnStartButton`.
- `pausePanel` — **RESUME** button → `HUDController.OnResumeButton`.
- `gameOverPanel` — `finalScoreText` + **FLY AGAIN** button → `OnRestartButton`.

## 7. Controls

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
