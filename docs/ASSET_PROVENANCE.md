# Asset provenance and art direction

## Custom assets

The PNGs in `Game/Assets/Wildfeast/Resources/Art/` and four WAV clips in `Resources/Audio/` were created specifically for Wildfeast by deterministic project sources. Regenerate in this order: `tools/make_art.py`, `tools/polish_art.py`, `tools/creature_art.py`. Later scripts intentionally replace initial prototype assets. No image-generation service, paid asset provider, external texture, sampled reference pixels, or sampled recording was used.

The source draws deliberate pixel shapes, layered color clusters, and deterministic background detail with Pillow. Audio is original synthesized PCM. The custom assets can be regenerated with Python and Pillow; the Unity game itself does not need Python. Imported textures use point filtering, no mipmaps, no texture compression, and 32 PPU. The project creator may use and edit these project-specific assets.

The sprite study was visually inspected during development. Creature silhouettes connect to their edible properties: folded cabbage fins, rattling seed pods, a kettle shell, luminous tubers, and buoyant fruit. The study is an initial direction, not a claim of final art polish.

## Palette and proportions

| Role | Color |
|---|---|
| Deep outlines / UI | #17383D |
| Ocean | #24566B |
| Water highlights | #36788A |
| Pale surf | #B3D7BF |
| Grass | #6D995D |
| Tree foliage | #347050 |
| Leafgill highlights | #B4D884 |
| Sand | #DDC28C |
| Warm wood | #8A5741 |
| Food and UI cream | #F7E8B3 |
| Pepperbell orange | #E8A15B |

Base unit: 32 pixels. Chef: 32 × 48. Larger silhouettes use multiple units. Sprite pivots sit at the ground contact; top-down depth follows ground Y. UI font renders independently for readability.

## Bundled font and engine packages

TextMesh Pro Essential Resources supply Liberation Sans and its SDF font. The font's included license under `Game/Assets/TextMesh Pro/Fonts/` is retained. Unity packages remain governed by their own package licenses. No claim is made that the project owns Unity package code or the bundled font.

## Production art work

The living-world pass adds four-direction chef frames, four-frame visitors and Brothback, textured multicolor foliage, new architecture, water ripples, seed/sprout states, held tools, and cooking art. Future work should add bespoke portraits, expressive action animation, more hand-authored terrain/foliage clusters, rich lighting/audio, and recipe-specific presentation. The creator's reference guides density, color, and garden composition; these original procedural drawings do not reproduce its finished detail.
