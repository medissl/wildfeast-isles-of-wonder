# Verification — 2 October 2026

## Living-world pass: current evidence

- Unity 6000.3.7f1 compiled and built the revised Windows x64 player. Final build report: 113,360,335 bytes.
- **29 EditMode tests passed, none failed or skipped.** New checks cover free travel, finite seed consumption, dry planting, separate watering/harvest checkpoints, forage seed uniqueness, seed/tool persistence, legacy saves and restarting visitors across consecutive services.
- **47 integrated checks passed at 1920×1080 and 1366×768**, exit 0, no recorded runtime errors/exceptions. The current journey adds number-key equipment, mouse-wheel selection, full camera viewport, actual outdoor sign/OPEN-button service flow, separate seed and watering-can input, chopping/heat/plating, pointer dragging, proximity-triggered Brothback, walking delivery, free travel and upgraded fruit yield.
- The opt-in journey creates virtual keyboard and mouse devices. It exercises the actual Input System/UI events, while setup positions and repeated earning cycles remain scripted. It does not constitute a human playthrough.
- Offscreen Unity frames of both resolutions were inspected: village, shoreline casting, garden watering, restaurant, stove and plating. The ordinary window and welcome screen were also inspected through Windows Computer Use. Physical keyboard usability and an uninterrupted manual loop are not claimed from that inspection.
- Original artwork now contains 133 PNGs, including chef/visitor/creature animation, held tools, richer foliage/architecture, distinct interaction props, water effects and cooking visuals. Sprite imports remain point-filtered, uncompressed, without mipmaps, at 32 PPU.
- The native borderless fullscreen launch was inspected through Windows Computer Use, with no window titlebar or boxed viewport. An isolated save was used for this display check.
- The pre-pass scene was backed up before supported Editor authoring reconstructed the clean tracked scene. Scene references and font resources were reopened and checked. Source/document diffs have no whitespace errors; Unity-generated scene whitespace is retained as emitted by its serializer.

## Bugs found and corrected in this pass

- A reel input could be reused as a cast on the same frame that a catch resolved. Busy-frame input is now consumed once.
- The initial diagnostic mouse could retain an unrelated physical click. Verification now uses its own virtual mouse.
- Garden beds initially competed with the doorway prompt. They were moved clear of the entrance.
- Pixel frame borders were initially oversized in Canvas units; their border scale was corrected.
- Consecutive services now restart guests who had not finished walking out the previous night.
- Legacy tool/seed fields, separate watering, and changed upgrade effects preserve the existing save/economy.
- Replaced an unsupported checkmark glyph and corrected the final food icons to show cooked dishes rather than the raw Leafgill.

## Current performance observation and remaining checks

Concurrent hidden-window diagnostic samples on Intel Iris Xe, 180 stationary Mistwake frames with forced offscreen rendering: 1080p mean 2.14ms / P95 2.78ms; 1366×768 mean 2.72ms / P95 4.29ms; about 98.5 MiB Unity allocated. These are diagnostic intervals, **not display FPS**, and concurrent execution makes them unsuitable for a hardware claim.

Still needs creator review: whether the original visual treatment is close enough to the desired direction, manual navigation and comprehension, cooking enjoyment after repeated services, fishing feel, guest pacing, long sessions, audio, physical controller completeness, minimum hardware and another-machine import. The knife is visual only, NPC routes are simple, cooking remains order-based, and the world remains two authored islands.

## Historical first-prototype evidence

The evidence below concerns the preceding build and its superseded interactions/travel gates.

## Final evidence

- Unity 6000.3.7f1 compiled the game and built a Windows x64 Mono player successfully. Final build report: 111,868,995 bytes.
- The Editor assembled, saved, reopened, and checked the scene's gameplay references, MonoScript bindings, and font references.
- **21 EditMode tests passed, zero failed or skipped**, including the crop checkpoint regression and serialized-scene/font checks. The repository's `tools/test.ps1` was also executed successfully.
- **35 integrated checks passed in the final Windows player at 1920 × 1080 and 1366 × 768**, with exit code 0 and no logged runtime errors/exceptions. The earlier 1280 × 720 journey passed 34 checks before the final visible-five-guest check was added.
- Rendered shore, restaurant, cooking, expanded-restaurant, and Mistwake frames were captured through Unity URP render requests. Shore/cooking/restaurant frames and non-integer scaling were visually inspected. These are engine-rendered offscreen frames, not desktop screenshots.

## Integrated check coverage

Movement from virtual keyboard events through Unity Input System; three catches with exactly one reward each; daily forage uniqueness; transfer to pantry; stocked service; cooking resolution and delivery; closing; useful satchel effect; one-time request reward; two watered nights and crop yield; Brothback telegraph, charge, one bounded knockback, and cooling-phase harvest through interaction input; recipe discovery; earnings-funded upgrades; five guests and an active employee; Nori's timed delivery; boat travel; island story; reach-gated Cloudfruit/recipe; disk save round trip; runtime error detection.

The scripted journey uses test setup positions and domain calls for repeated service earnings and some progression. It is not a human walking every route, and does not establish physical keyboard/gamepad usability or player enjoyment. Ordinary gameplay does not activate the diagnostic keyboard or offscreen captures.

## Meaningful fixes found during verification

- Changed save replacement to the API available in Unity's runtime.
- Moved WorldPoint to its own correctly named MonoBehaviour source file; the initial player had failed during scene loading.
- Completed asynchronous TMP Essential Resources import before building and added a required-resource guard.
- Isolated diagnostic input from hidden-window focus and used the game's own positioning setup.
- Fixed repeated frame-based charge knockback by allowing one bounded push per charge.
- Made crop harvesting save its reward and growth reset as one checkpoint.
- Corrected title text bounds, restaurant framing, customer silhouettes, shoreline fishing placement, and offscreen UI sorting.

## Performance observation

Test renderer: Intel Iris Xe Graphics. The 180-frame stationary Mistwake sample forces URP offscreen rendering and includes UI setup; it is **not display FPS** or a minimum hardware qualification.

| Resolution | Mean interval | P95 interval | Unity allocated memory |
|---|---:|---:|---:|
| 1920 × 1080 | 1.11 ms | 1.30 ms | 95.9 MiB |
| 1366 × 768 | 0.95 ms | 1.18 ms | 95.6 MiB |

The production target remains stable 60 FPS on an agreed reference PC. Actual foreground frame pacing, extended sessions, and lower-end hardware still need profiling.

## Unverified / production gates

Fresh-player comprehension and enjoyment, a fully manual first-day journey, long-session regression, actual audio listening/mixing, physical controller navigation, key rebinding, text-size preferences, minimum hardware, and a clean import on another machine. No claim of release readiness or final art polish is made.

Machine-readable test reports and result summaries are retained with the local deliverables. Git source excludes generated logs, caches, and build files.
