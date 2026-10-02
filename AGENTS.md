# Wildfeast project guidance

Read `docs/HANDOFF.md`, `docs/DECISIONS.md`, and `docs/VERIFICATION.md` before extending the game. The original vision is in `docs/WILDFEAST_PLAN.md`. Work in the Unity project at `Game/`; the repository root is not the Unity project root.

## Direction

Modern top-down pixel art. Offline single-player Windows game, free without ads or purchases. Exploration and restaurant growth must remain connected through original food ecology. Keep the six-ingredient/two-island slice coherent before adding content.

## Implementation

- Unity 6000.3.7f1, URP 2D, Input System, uGUI/TMP.
- Play scene: `Game/Assets/Wildfeast/Scenes/Wildfeast.unity`.
- Preserve .meta files. Never commit Library, Temp, logs, generated projects, or build output.
- Every serialized MonoBehaviour belongs in a file with its class name.
- Domain state and transactions live in `GameData.cs` and `ItemInventory.cs`; external tuning definitions live in `Resources/Content.json`. Scene and UI integration are real serialized references.
- Keep quantity changes, crop resets, payouts, and purchases atomic with respect to save checkpoints.
- Use supported Editor tooling for scene/prefab/asset edits. Detect the pipeline before changing pixel rendering. Do not hand-edit scene YAML or the package manifest.
- Ordinary scene edits should preserve manual work. `ProjectBuilder.Assemble` intentionally reconstructs the scene; do not invoke it on top of someone else's manual edits without first preserving them.
- Regenerate original art/audio in order: `tools/make_art.py`, `tools/polish_art.py`, `tools/creature_art.py`, then `tools/island_life_art.py`, requiring Python, Pillow and numpy. Intentional scene assembly rebakes illustrated maps. Keep bundled TMP font license files.
- Read docs/ISLAND_LIFE_PASS.md first. Preserve item hotbar/backpack placement, runtime pointer callbacks, Tab categories, M maps, spatial farming, finite water, resources, original music and continuous ship travel.
- Read docs/LIVING_WORLD_PASS.md. Preserve the creator's free ferry travel, visible tools, seed/can actions, quiet HUD, proximity creature behavior and distinct prep/cook/plate cooking. Do not revert to the initial prototype's modal activation interactions or travel purchase gates.
- Do not present blockouts, automated test inputs, or offscreen performance samples as finished art, human playtesting, or actual display FPS.

## Checks

Run relevant Unity tests. The economy/save suite is in `Assets/Wildfeast/Tests/Editor/`. Run the standalone integrated journey when changing the end-to-end flow, using `--smoke-test --save-path <isolated-file> --test-output <directory>`; diagnostic mode uses a virtual Input System keyboard and does not alter an ordinary save. Check its result, runtime-error log, and captured frames, not just the executable's presence.

`Wildfeast.Editor.ProjectBuilder.Build` builds the authored scene. `-wildfeastBuild <absolute-executable-path>` selects an output folder. For a fresh bootstrap only, `ImportFontsAndExit` must run without `-quit` and confirm completion before scene assembly. Required font resources are already checked in.

Keep verification and handoff documents current. State what actually passed and what remains untested, especially physical controller support and fresh-player comprehension. Keep changes small and preserve unrelated edits.
