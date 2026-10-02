# Latest decisions: cozy polish

The creator explicitly prioritizes consistent cozy/pixel typography, furniture that belongs in the restaurant, correct collision/depth, compact distinct tools, actual chef action frames and animation cooldowns. Pixelify Sans Regular replaces Liberation Sans in visible game text. Its static atlas and matching HUD material avoid mixed fonts. Original native world furniture replaces reuse of large cooking-screen artwork. Pixel UI borders, 32 PPU props and body/arm action poses maintain the art direction.

Ordinary tool actions last 0.65–0.8 seconds and apply once at contact; input during windup/recovery is ignored rather than queued. The feet and equipment are locked during that action. Cooking cuts/stirs follow the same timing. Original foraging, fishing and sailing retain their separate activity durations. Ground furniture sorts at its footprint; wall hangings stay behind actors. Customers and staff use aisle routes. No save or economy redesign was introduced.

# Latest decisions: island life

The creator asked for Stardew-like item/tool organization and reference-inspired UI, while retaining original food ecology. The hotbar now has ten numeric slots, with thirty backpack slots, and no journal/bag commands. Tools act by identity even after rearrangement. Food retains the existing portion-capacity upgrade economy; tools, seeds and resource stacks remain separate.

Both islands permit spatial gardening on unobstructed land. Water is finite. Resource tools harvest original Cinnamonwood, Saltstone and Noodlegrass; materials can be traded at the workshop. Free ferry access now uses continuous ship movement between distinct island scenery, not immediate area switching. Original synthesized music replaces the ambient drone as the musical layer. All original world sprites retain 32 PPU; native image dimensions vary by object size.

Earlier decisions below remain applicable except where explicitly superseded here.


# Decisions

## Confirmed by the creator

- Wildfeast: Isles of Wonder; repository medissl/wildfeast-isles-of-wonder.
- Unity and modern top-down pixel art.
- Offline single-player, Windows PC.
- Free game without advertising or in-game purchases.
- Complete the development plan's small-world milestones, including hunting, crops, staff, and a second island.

## Implementation choices

- Unity 6000.3.7f1, the installed editor, with its actual Universal 2D template; URP 17.3.0.
- A 640 × 360 world reference, 32 pixels per unit, 32 × 48 chef sprite; pixel snapping and a full camera viewport. Borderless fullscreen is the default. The living-world pass deliberately replaces windowboxing, following the creator's screen-framing feedback.
- UI scales from a 1280 × 720 reference separately from the world to keep text readable.
- Input System with keyboard/mouse as the tested target; optional gamepad mappings included.
- uGUI with TextMesh Pro. Liberation Sans is from Unity's TMP Essential Resources, whose included license is retained.
- One integrated scene with serialized Saltleaf, Mistwake, and restaurant roots. Switching areas activates the relevant root. No procedural terrain or streaming framework.
- A small domain model owns inventory and economy transactions. Definition data is in `Resources/Content.json`; quantities and progression live in a versioned save. Controllers handle interaction; world and UI views handle presentation.
- Discovered ingredients unlock authored recipes. Forage provides finite seed quantities, and planting, watering and harvest are separate actions.
- Service orders are generated against temporary reserved stock so every offered order can be fulfilled. Cooking consumes ingredients once; serving pays once. Runtime saves preserve all order states.
- Cooking quality follows chopping completion, stove heat and stirring, then plating. Cooking errors reduce tips rather than producing unusable meals. Fishing stays in the world with a held rod, line, float, and catch animation.
- Gathered ingredients enter the bag, then transfer to pantry on entering the restaurant. No spoilage in this slice.
- Every morning the community pantry restores at least three grain portions. The free porridge recipe prevents a depleted kitchen from blocking progress.
- Daily phase transitions are explicit. There is no continuous day countdown and no customer patience penalty in this slice.
- Brothback hunting collects shed stock after a telegraphed charge. No lethal combat or health grind is needed for the first hunting interaction.
- Plants grow after two watered nights. Untended plants pause growth; they do not die. Harvesting restarts growth without consuming the plant.
- Nori walks cooked dishes to guests; the player remains responsible for cooking. The terrace adds two guest tables. Guests walk into and out of service and show order icons beside numbered tables.
- Both islands and floating fruit are freely accessible through the ferry's destination menu. Legacy upgrade IDs `boat` and `reach` now mean a provision locker (+4 capacity) and botanical gloves (three fruit per source).
- Custom visuals/audio are original deterministic assets generated by `tools/make_art.py`, then `tools/polish_art.py`, then `tools/creature_art.py`. No asset purchases, third-party art packs, sampled reference pixels, or copied franchise content.
- No service accounts, monetization SDKs, cloud save, or online multiplayer features were added. The chosen Unity template includes some unused tooling packages; they have not been treated as game features.

## Remaining design decisions

Validate appetite for the loop, preferred sprite detail, service pacing, exact upgrade prices, and how much hunting challenge the audience wants through human playtests. Choose a broader release content budget only after measuring production time for a fully polished habitat.
