# Unity project instructions

This repository is a Unity project. Follow these rules when assisting:

- Prefer code changes under `Assets/Scripts` unless an existing pattern clearly places code elsewhere.
- Do not edit generated Unity files (`Library/`, `Temp/`, `Obj/`, `Logs/`, `Build/`).
- Preserve `.meta` file integrity: keep GUID relationships intact and avoid unnecessary asset moves/renames.
- Reuse existing assembly definitions (`.asmdef`) and project structure instead of creating new assemblies by default.
- Keep changes small and targeted; avoid broad refactors unless explicitly requested.
- Use existing naming/style conventions in this repo for C# scripts, MonoBehaviours, and ScriptableObjects.
- When touching serialized data structures, preserve backward compatibility where possible.
- Prefer deterministic, editor-safe changes and avoid introducing runtime-only assumptions into editor tooling.

Validation guidance:

- Use the project’s existing Unity batchmode commands when available.
- Typical examples (adjust paths/targets to this repo):
  - EditMode tests: `Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -quit -logFile -`
  - PlayMode tests: `Unity.exe -batchmode -projectPath . -runTests -testPlatform PlayMode -quit -logFile -`
- If there is an established build script or CI entrypoint, use that command instead of inventing a new one.
