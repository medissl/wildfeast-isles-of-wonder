# Cozy polish: typography, restaurant and tool actions

The creator requested typography first, followed by restaurant collision/layering, distinct compact tools and actual chef animations with cooldowns. This pass preserves the two-island food world and the existing inventory, gardening, fishing, cooking, service, music and save progression.

## What changed

- Every authored and dynamically created game label uses Pixelify Sans Regular. Titles, body text and small labels share the same font. Its glyphs are baked into one static 1024 atlas; the HUD uses a matching outline material. Font sources and the author's SIL Open Font License are included in `Assets/Wildfeast/Fonts/`.
- Inventory slots, hotbar slots, buttons and menu tabs use original pixel borders and the same parchment, wood and burgundy palette. Existing screen hierarchies and pointer callbacks are preserved.
- The kitchen is one original 160×64 world worktop with a small integrated chopping board, soup pot, pan and oven. Large 96×72 cooking-screen images are retained for the minigame and removed from the room.
- The worktop, dining tables, bed, planters and Saltstone have appropriate ground footprints. Wall hangings stay behind actors. Ground furniture sorts from its base rather than a fixed foreground number. Table labels follow their tables' depth. The restaurant camera centers immediately on entry.
- Customers and Nori follow clear aisles around furniture. Nori waits beside the counter rather than inside it. Guests enter in a staggered queue.
- Scythe and pickaxe have different silhouettes. Held tools, dishes, ingredients and materials have separate compact native sprites; 32 PPU and point filtering are retained. Brothback stock is held as a flask, rather than the entire animal sprite.
- Pickup feedback uses one compact ingredient image, rather than five full-size creatures or plants. Transient pickup/catch effects are cleared when changing areas, so an outdoor effect cannot float through the restaurant.
- 120 new 32×48 chef frames cover four directions and five poses for swinging, pouring, planting, pulling, casting and stirring. The body and arms animate alongside the held tool.
- Ordinary tool actions have windup, one contact, and recovery lasting 0.65–0.8 seconds. Rewards and resource hits occur at contact. Movement and equipment changes wait for recovery; rapid clicks are ignored and never queued. Chopping and stirring use the same action clock. Foraging and fishing retain their own longer activity locks and animations.

## Test this build

1. Open Tab and browse Inventory, Map, Recipes, Journal, Requests and Options. Check font readability, slot outlines and text clipping at your normal display size.
2. Enter the restaurant. Walk along the front and sides of the counter, around the tables and beside the bed. Solid footprints should stop your feet; overhead artwork should sort naturally as you pass in front or behind.
3. Open service and watch guests take the aisles to their tables. After hiring Nori, cook a dish and watch its delivery route.
4. Select the scythe and pickaxe. Compare their icons and held shapes. Quickly click an outcrop or tree: one swing should complete before another starts. The chef should raise an arm, make contact and recover; the selected tool should remain stable during that animation.
5. Till, plant and water a plot. Watch the chef bend or pour before the tile changes. Refill the can at water. Check that its quantity changes once per completed action.
6. Pull a wild ingredient, then immediately enter the restaurant after collection. Large duplicate plant/animal images should not follow you inside.
7. Cook an order. Alternate A/D with the chopping rhythm, adjust heat, stir with Space, then plate. Check that extra clicks during a chop or stir cannot accelerate it.
8. Catch a fish, serve three guests, sleep, sail to Mistwake and return. Quit/reopen to check that the existing progress is retained.

Automated checks exercise these flows with virtual keyboard/mouse input and scripted positioning. They do not establish human game feel, accessibility or physical controller support. Record any awkward animation, blocked aisle or unreadable label with the action/location and screen size.

## Development notes

`ToolActionClock.cs` owns transient action timing; model transactions and save schema remain unchanged. `PropDepth.cs` uses furniture footprints. `CozyPolish.Typography` and `CozyPolish.ArtAndRoom` update the existing authored scene through Editor APIs. `ProjectBuilder.Build` builds that scene. Back up manual scene edits before intentional reconstruction with Assemble.

Run art generation in order: make_art, polish_art, creature_art, island_life_art, **cozy_polish_art last**. The final script creates original native assets; it does not rescale the reference pictures or import a commercial art pack. Reapply supported import settings after generation. The maps remain illustrated island maps; room furniture is not represented on the island map.

Pixelify Sans source: https://github.com/eifetx/Pixelify-Sans/tree/main/fonts/ttf. The unmodified Regular TTF and the repository's OFL.txt are retained. The font is the third-party addition in this pass; game sprites and synthesized music remain original.
