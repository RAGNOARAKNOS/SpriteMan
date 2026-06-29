# SpriteMan

SpriteMan is a 2D platformer prototype built with **Unity 6** and **URP**. The project includes player movement, camera follow, collectible tracking, enemy and pit hazards, respawn handling, and a simple on-screen session HUD.

## Project Snapshot

| Item | Value |
| --- | --- |
| Engine | Unity `6000.5.0f1` |
| Render Pipeline | Universal Render Pipeline (`com.unity.render-pipelines.universal` 17.5.0) |
| Primary Scene | `Assets/Scenes/SampleScene.unity` |
| Input | Unity Input System package installed, with legacy input fallback in player controller |
| Genre | 2D Platformer |

## Features

- Horizontal movement and jumping with grounded checks
- Camera follow with smoothing and optional level-bound clamping
- Enemy patrol behavior with edge and wall detection
- Hazard and enemy-triggered player respawn
- Collectibles and goal trigger with HUD status (collectibles, deaths, completion)
- Placeholder art pipeline for rapid gameplay iteration

## Controls

- **Move:** `A / D` or `Left / Right Arrow`
- **Jump:** `Space`, `W`, or `Up Arrow`

## Getting Started

1. Install **Unity Hub** and Unity Editor version **6000.5.0f1**.
2. Clone this repository.
3. Open the repository folder in Unity Hub.
4. Let Unity resolve packages from `Packages/manifest.json`.
5. Open `Assets/Scenes/SampleScene.unity`.
6. Press **Play** in the Editor.

## Project Structure

```text
Assets/
  Art/                  Placeholder sprites and art assets
  Prefabs/              Gameplay prefabs
  Scenes/               Unity scenes (SampleScene.unity)
  Scripts/              Core gameplay scripts
  Settings/             URP and rendering settings assets
Packages/               Unity package dependencies
ProjectSettings/        Unity project configuration
```

## Gameplay Scripts

| Script | Responsibility |
| --- | --- |
| `PlatformerPlayerController2D` | Reads input, applies horizontal movement/jump, and checks grounded state |
| `PlatformerCameraFollow2D` | Follows player with smoothing and optional camera bounds |
| `PlatformerEnemyPatrol2D` | Patrols platforms, flips at edges/walls, respawns player on contact |
| `PlatformerPitHazard` | Respawns player on trigger |
| `PlatformerRespawnController` | Handles spawn point and fall-threshold respawns |
| `PlatformerCollectible` | Registers and consumes collectibles |
| `PlatformerGoal` | Marks level completion when reached |
| `PlatformerGameSession` | Tracks HUD state (instructions, collectibles, deaths, completion) |

## Development Notes

- Keep gameplay scripts under `Assets/Scripts`.
- Avoid committing generated Unity folders (`Library/`, `Temp/`, `Logs/`, `Build/`).
- Preserve `.meta` files to keep Unity asset GUID links stable.

## License

This project is licensed under the **MIT License**. See [`LICENSE`](LICENSE) for details.
