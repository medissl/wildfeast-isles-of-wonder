# Implemented expansion: five food ecosystems

The creator has expanded the initial slice. The current playable scope is Saltleaf Shore, Mistwake Isle, Emberfold Cay, Moonfen Hollow and Pearltide Atoll, connected by free animated ferry travel. Outdoor map layout/artwork, terrain planting rules, grounded foliage, resource placement, food ecology, cuisine, paged books and sound/camera Options are implemented. The polished restaurant and original loop remain.

The expansion contains 19 ingredients, 20 dishes, five food-creature species and six crops, with coherent paths and habitats. See ARCHIPELAGO_EXPANSION.md for island design, controls and the manual verification route; see VERIFICATION.md for actual automated evidence. The original proposal below describes earlier scope and remains historical where it differs.

# Current implemented refinement: cozy polish

Typography, restaurant furniture/collision/depth, pixel widgets, compact held props, directional chef action poses and cooldowns supersede the earlier presentation. See COZY_POLISH_PASS.md and VERIFICATION.md. Preserve the two-island inventory/gardening/service loop. The historical plans below remain context where not superseded.

# Current implemented refinement: island-life pass

The creator's latest feedback prioritizes a clickable item hotbar/backpack, Tab categories, an M map with player/landmarks, keyboard/mouse actions, free-ground till/seed/water flow, finite refillable can, axe/scythe/pickaxe resources, forage pull-out animation, continuous sailing to a distinct island, prop collision, a livelier restaurant and original music. See ISLAND_LIFE_PASS.md for the actual scope and VERIFICATION.md for evidence. Preserve these refinements over the historical fixed-tool and instant-travel designs below.


# Wildfeast: Isles of Wonder — Development Plan

Planning draft · 2 October 2026

Creator playtest feedback now drives the living-world pass documented in LIVING_WORLD_PASS.md. Its full-screen presentation, eight-slot tools, separate seed/water actions, natural creature behavior, prep/cook/plate cooking and free destination-selecting ferry supersede the initial prototype's interaction and travel assumptions below. Follow those accepted changes in future development.

## Confirmed direction

- Title: Wildfeast: Isles of Wonder.
- Engine: Unity.
- Visual direction: modern top-down pixel art.
- Repository: https://github.com/medissl/wildfeast-isles-of-wonder.
- Core fantasy: explore strange ecosystems, fish, hunt, and grow impossible ingredients; cook them and build a fantasy restaurant from humble beginnings.
- Inspirations provide emotional and mechanical reference only. Create original creatures, names, environments, characters, recipes, art, and story.

## Product brief

You arrive at a neglected harbor restaurant in an archipelago where the wildlife produces extraordinary food. Each expedition uncovers ingredients and the behaviors needed to obtain them. Back home, you turn discoveries into dishes, serve curious customers, and invest the proceeds in equipment, cultivation, staff, and restaurant improvements. Your growing restaurant becomes a record of your adventures.

The defining promise: **discover something wonderful in the wild, then let someone taste it at your table.**

## Design pillars

1. Food explains the world. Creature behavior, habitat, harvesting, and cooking properties belong together.
2. Discovery changes play. A new ingredient should introduce a recipe, technique, expedition choice, or customer opportunity.
3. Home rewards adventure. Restaurant improvements are visible and make returning satisfying.
4. Skills stay approachable. Clear signals and adjustable challenge matter more than button spam or punishing failure.
5. Progress opens possibilities. Equipment and knowledge unlock new interactions rather than only increasing numbers.

## Confirmed setup

Single-player, offline, Windows PC; keyboard/mouse first. The creator confirmed a free game with no ads or purchases. Basic gamepad mappings are included but physical controller validation remains outstanding. Multiplayer, consoles, mobile, and online services are outside this prototype.

Unity 6000.3.7f1 was used with the installed Universal 2D template. The project is in Game/ with URP 17.3.0, Input System, uGUI/TMP, and an integrated serialized scene. No other Editor version was installed.

## Daily loop

Prepare → explore → fish/hunt/forage → return → discover or select recipes → cook and serve → receive earnings → upgrade → begin the next day.

- Morning: choose an expedition goal, pack tools, and inspect customer requests.
- Expedition: move through compact authored habitats; read creature behavior; decide which ingredients fit in the bag.
- Home: store ingredients, tend crops once cultivation exists, and plan a menu from available stock.
- Service: customers choose from the offered menu; fulfill orders through a short cooking interaction and delivery.
- Closing: show revenue and expenses, buy improvements, save progress, and preview the next opportunity.

Begin with explicit phase transitions. A continuous clock can be evaluated later; do not introduce time pressure across every activity by default.

## First playable milestone: A Strange First Supper

Target experience: roughly 10–15 minutes for a first complete cycle, subject to playtesting.

One small harbor restaurant and one nearby shore. The player walks to a fishing spot, catches a Leafgill, harvests seasoning, returns to cook, serves three customers, and buys a useful improvement.

### Included

- Top-down movement, collision, camera, and one consistent interaction prompt.
- A small bag and ingredient storage with visible quantities.
- One fishing interaction with a readable bite cue and short tension challenge.
- One fish species, one forage source, and a small supporting pantry supply.
- Two authored recipes and one cooking timing interaction.
- Menu selection, a three-customer service, order display, delivery, and earnings.
- One upgrade, such as bag capacity, that changes the next expedition.
- Phase transitions, a closing summary, and local save/load.
- A way to reset the prototype for repeatable testing.

### Completion criteria

- A new player can complete the cycle using in-game prompts.
- Fishing success adds the right quantity; failure grants no duplicate reward.
- Cooking consumes ingredients once and creates the expected dish.
- A completed order pays once; insufficient ingredients cannot create free dishes.
- Money and ingredient counts never become negative.
- Buying the upgrade charges once and visibly applies its effect.
- Closing and reopening preserves inventory, money, learned recipes, purchased upgrades, and day progression.
- An interrupted service resumes from a documented safe checkpoint without duplicated payouts.
- Unity compiles without errors, the relevant scene runs, and a standalone build is checked when tooling permits.
- The player can recover after unsuccessful fishing or cooking; the loop cannot become permanently blocked by running out of ingredients or funds.

## Starter content concepts

These names and mechanics are proposals.

| Discovery | Habitat and behavior | Ingredient and cooking role |
|---|---|---|
| Leafgill | Shallow water; folds leafy fins around itself under tension | Layered flesh for a seared dish or leaf wrap |
| Pepperbell | Shore plant; ripe pods rattle in the wind | Aromatic seasoning; later cultivatable |
| Brothback | Later hot-spring creature; vents steam before a charge | Rich stock that unlocks soups |
| Lanternroot | Later cave plant; glows near underground water | Sweet root used in luminous desserts |

Prototype recipes: Seared Leafgill and Pepperbell Leaf Wrap. Begin with explicit recipes; later experimentation can use ingredient tags and hints. Avoid a huge hidden combination system before feedback and economics work.

Each eventual species needs a clear silhouette, habitat, behavior, harvest method, edible property, and recipe purpose. A food-shaped sprite alone is insufficient.

## Modern pixel-art direction

Top-down with a slight view of object fronts, readable paths, and layered depth. Combine deliberate pixel shapes with lively water, foliage, food steam, expressive animation, and controlled lighting. Prioritize creature recognition and appetizing dishes.

Provisional art scale to test before asset production: 32-pixel tiles, 32 pixels per Unity unit, characters approximately 32 × 48 pixels, and a 640 × 360 world reference resolution. Validate this with one room, one shoreline, and a character before locking it. Larger creatures may occupy multiple tiles.

Use consistent pixel density, point filtering, deliberate pivots, and crisp integer scaling where practical. Test camera movement for jitter and tiles for seams. Keep UI text readable at supported screen sizes. Detect the actual render pipeline before selecting pixel-perfect camera components. Proposed pipeline for a new project: URP with the 2D Renderer; adopt only after inspecting the project and confirming setup.

Create an art reference sheet covering palette, scale, silhouettes, shadows, outlines, water, foliage, and UI. Review that sheet before producing a large asset set. Early blockouts prove interaction; the milestone should receive coherent original art before it is treated as a visual showcase.

## Development milestones

| Milestone | Result | Gate before expansion |
|---|---|---|
| 0 — Foundation | Inspect repo, settle platform and art scale, prepare Unity project and conventions | Project opens and a movement/art test is readable |
| 1 — First supper | Complete fishing-to-service loop described above | Full cycle works, saves, and invites another expedition |
| 2 — Ecosystem slice | Add one huntable creature, two more habitats, recipe discovery, and a small crop plot | Each gathering method feels distinct; cultivation supports expeditions |
| 3 — Restaurant identity | Visible expansion, one recruitable employee, customer preferences, and requests | Staff removes repetitive work and creates meaningful choices |
| 4 — Second island | Travel, new ecology, equipment interaction, and a local story | A second island adds behavior and discovery beyond reskinned items |
| 5 — Production planning | Content budget, performance targets, accessibility, audio, onboarding, and release scope | Scope follows playtest evidence and actual production speed |

Do not assign release dates until the first milestone reveals the time needed for systems and original assets. Avoid building the full archipelago, advanced farming, staff simulation, bosses, procedural terrain, or multiplayer in parallel with the first loop.

## Technical approach

Keep systems small and connected through explicit responsibilities: interaction, inventory, fishing, recipes/cooking, service/orders, economy/upgrades, day flow, and persistence. Add hunting, cultivation, travel, and staff only at their milestones.

Use stable IDs for ingredient, recipe, and upgrade definitions. Keep authored definitions separate from runtime quantities and save data. Save schema should have a version and a recoverable failure path. Handle ingredient consumption, rewards, and purchases as operations that either succeed completely or make no change.

Expose balancing values in editable definitions. Avoid one giant manager or a general-purpose framework. Prefer direct, inspectable solutions until complexity demonstrates a need for abstraction.

Unity scenes, prefabs, and asset settings should be authored through the Editor and supported project tooling. Preserve .meta files and existing project conventions. Install packages through supported Unity package workflows. Add dependencies only for a current milestone need.

## Verification and playtesting

Automated tests should focus on inventory arithmetic, recipe consumption, duplicate payout prevention, purchases, save round trips, and phase transitions. Use Editor playtesting for movement, fishing feel, cooking readability, sorting, and service pacing. Check rendering at common display sizes and a non-integer scale.

Ask after each playtest: Did the ingredient feel surprising? Was harvesting understandable? Did the dish connect to the discovery? Did the reward make the next outing more appealing? Change the loop if those answers are weak before multiplying content.

## Current handoff

The small-world implementations for milestones 0–5 are in the Unity project. See HANDOFF.md for the feature map, DECISIONS.md for settled choices, VERIFICATION.md for executed checks, and PRODUCTION_PLAN.md for the production review. Automated correctness and rendered-frame review do not replace fresh-player testing of comprehension, pacing, or enjoyment. The current next step is the creator's playtest, followed by refinement before broader content production.
