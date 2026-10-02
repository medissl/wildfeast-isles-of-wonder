# Verification — 2 October 2026

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
