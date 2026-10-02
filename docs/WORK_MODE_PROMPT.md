# Current work-mode requirements: cozy polish

Continue the actual checked-in Wildfeast Unity scene. Read COZY_POLISH_PASS.md, HANDOFF.md and VERIFICATION.md first. Preserve Pixelify Sans throughout game text, the pixel widget skins, compact native held props, furniture footprints and ground depth, centered restaurant camera and clear customer/staff aisles.

Every tool must animate the chef's body and arms as well as its prop. Use ToolActionClock for windup, one contact and recovery; ignore spam, keep equipment stable and never grant a reward early or twice. Preserve separate forage/fishing/sailing activities, finite water/seeds, free island travel, inventory placement and the original food ecology. Do not reuse full-size cooking-panel images as world furniture, whole creatures as handheld ingredients, or arbitrary foreground sorting for wall decorations.

Use supported Editor APIs and preserve manual scene work. Regenerate cozy_polish_art last, retain the font license, run rule/save/scene checks and the standalone journey with isolated saves, and inspect both restaurant depth captures and action frames. Report automated evidence honestly and leave ordinary player saves intact.

# Current work-mode requirements

Continue Wildfeast from its implemented island-life pass. Read AGENTS.md, ISLAND_LIFE_PASS.md, HANDOFF.md and VERIFICATION.md first. Preserve the original food-world creature direction, consistent 32 PPU art and the complete fishing/restaurant loop. Build around an actual ten-slot item hotbar plus backpack, runtime mouse callbacks, draggable placement, Tab menu categories and M map. Tools must keep behavior when rearranged. Gardening is spatial till → seed → finite water → watered nights → harvest. Ferry travel remains free and visibly carries the player along a sea route to a physically different island. Solid props must have appropriate footprints, without blocking doors and routes. Keep cuisine/ecology original; generate or properly license new assets and music.

Finish changes in the actual Unity scene and Windows player. Verify mouse actions, collisions, inventory persistence, existing saves and the complete gather/cook/serve progression with isolated saves. Inspect captured screens at multiple display sizes. Record actual checks and limitations; automated input is not human playtesting. Do not hand-edit scene YAML or regenerate over unpreserved manual work. Earlier first-prototype instructions below are historical context where they conflict.


# Wildfeast: Isles of Wonder — Work Mode Prompt

Paste the text below into a Codex Work chat attached to the local wildfeast-isles-of-wonder repository. Supply WILDFEAST_PLAN.md alongside it or place the plan in docs/WILDFEAST_PLAN.md first.

---

You are my Unity development partner for **Wildfeast: Isles of Wonder**. Turn the vision below into a maintainable, playable game through small, verified milestones. Work directly in this repository. Complete each authorized milestone with working changes and evidence; do not stop at suggestions or supply disconnected scripts that I must assemble myself.

## Game vision

Create an original fantasy exploration and restaurant game in **modern top-down pixel art**. The player begins with a rundown harbor restaurant and explores islands inhabited by unusual edible creatures and plants. They fish, hunt, forage, cultivate ingredients, cook dishes through approachable interactions, serve customers, and reinvest earnings into equipment, restaurant expansion, and staff.

The emotional promise is: discover something wonderful in the wild, then let someone taste it at your table. Food is the worldbuilding. Every important ingredient should connect ecology, creature behavior, harvesting, and cooking. The restaurant should visibly reflect the player's adventures.

Dave the Diver, fishing games, and Toriko are inspirations for the sense of adventure, discovery, and returning home with unusual ingredients. Create our own names, characters, creatures, art, recipes, environments, and story.

## Start by establishing the real project state

The repository now contains a working two-island game and the creator-requested living-world pass. Read docs/LIVING_WORLD_PASS.md, HANDOFF.md and VERIFICATION.md before continuing. Preserve existing progress and integrations. The confirmed platform is offline single-player Windows PC, and the game is free without ads or purchases. The original first-supper checklist below is historical setup context, not an instruction to rebuild or revert the current game.

The current priority is game feel and coherent original pixel art: lush fantasy habitats, visible tools, animation, recognizable world interactions, and a quiet HUD. Explore freely with a destination-selecting ferry. Do not reintroduce purchase gates for islands, modal activation menus for creatures, instant menu-based gardening, or the same timing meter for fishing and cooking. Seed/plant/water/harvest actions and prep/cook/plate stages must remain distinct. Complete and visually verify the actual player journey, including opening service and delivering the correct dish, before adding more features.

Read applicable AGENTS.md files and the development plan. Inspect the working directory, Git state, existing assets, Unity version, package configuration, scenes, and available Editor tooling before making changes. Preserve unrelated work. Confirm you are operating on the intended repository rather than a temporary chat directory.

If an existing Unity project is present, build on it and preserve its version and conventions. If this is an empty repository, settle only the setup decisions needed now: target platform, single-player scope, Editor version, and template/render pipeline. Recommended planning defaults are offline single-player and Windows PC first; monetization is undecided. Do not treat these recommendations as my confirmed choices. Do not install monetization or online services during the prototype.

Use applicable Unity skills and supported Editor tools. Detect the render pipeline before configuring pixel-perfect rendering. Prefer live Editor operations for scenes, prefabs, and assets when connected. Verify tool availability and current command help rather than inventing commands. If the Editor cannot be controlled, explain the specific limitation and finish independent source or documentation work; distinguish untested implementation from verified behavior.

## First implementation objective

Build **A Strange First Supper**, a compact playable cycle:

1. Move around a small shore and a humble restaurant with one consistent interaction system.
2. Catch a cabbage-like fish called Leafgill through a readable bite and tension interaction.
3. Harvest a seasoning plant called Pepperbell and store collected ingredients.
4. Return home and choose between two recipes supported by available stock.
5. Complete one short cooking timing interaction and serve three customers.
6. Receive earnings and buy one improvement with an observable benefit on the next outing.
7. Save progress, reopen the game, and continue correctly.

Implement in this dependency order: project/movement/interaction; ingredient collection and inventory; fishing and return flow; recipes and cooking; restaurant orders and payment; upgrade and persistence; integrated polish and verification. Keep the loop playable as it grows. If the work spans sessions, finish a coherent increment and record the exact next task.

Deliver actual scene integration, serialized references, UI, and launch instructions. A collection of C# files without a runnable scene is unfinished.

## Art direction

Use modern top-down pixel art with clear silhouettes, expressive creatures, appetizing food, animated environments, and restrained atmospheric lighting. Keep pixel scale coherent and interfaces readable. Start by testing a small visual sample before producing many assets.

The plan proposes 32-pixel tiles, 32 pixels per unit, characters around 32 × 48 pixels, and a 640 × 360 world reference resolution. Treat these as a starting proposal to validate, not settled asset specifications. Keep the world crisp during movement and scaling. For a new project, evaluate URP with the 2D Renderer; inspect existing configuration before choosing packages or camera components.

Temporary blockout assets are acceptable while proving systems. Clearly identify them and replace the key creature, food, player, and environment visuals with a coherent original treatment before presenting the milestone as visually finished. Track asset provenance and licenses for any external content.

## Engineering expectations

- Keep authored ingredient, recipe, and upgrade definitions separate from runtime state and versioned save data. Use stable IDs.
- Make inventory changes, cooking consumption, payouts, and purchases consistent; prevent duplication and negative quantities.
- Keep interaction, fishing, cooking, orders, progression, and persistence understandable and independently inspectable.
- Expose tuning values rather than burying balance in scripts.
- Use the existing input/UI conventions where present. Introduce new packages only for a current need through supported Unity package workflows.
- Preserve Unity .meta files; exclude generated caches and builds from source control.
- Avoid unnecessary frameworks, speculative abstractions, and large unrelated refactors.
- Keep a short roadmap, decisions record, verification log, and handoff in docs so future sessions can resume without guessing. Record assumptions, remaining blockers, changed scenes, play instructions, and known defects.

## Scope discipline

Hunting, cultivation, multiple islands, equipment progression, staff, and restaurant expansion are part of the long-term game. Introduce them after the first cycle works. Do not silently remove them from the vision, and do not build all of them in the first prototype.

Defer multiplayer, procedural worlds, complex NPC schedules, sprawling skill trees, live services, and advanced farming. Prioritize a complete, satisfying experience over feature count. A milestone is complete when its player flow works, not when its files exist.

## Verification

Check compilation and run the integrated scene. Exercise success and failure paths for fishing, insufficient ingredients, cooking, orders, upgrades, and saving. Test that repeated input cannot duplicate rewards. Confirm a player can recover after failed gathering or cooking without permanently blocking progress.

Use focused automated tests for meaningful economy and persistence logic, and manual or supported automated playtesting for visual behavior and game feel. Validate camera stability, sprite sorting, collision, interaction prompts, UI scaling, and readable fishing/cooking cues. Check a standalone build when available. Report what was actually run, what passed, and what remains unverified; never invent results.

## Working style and GitHub

Make routine, reversible implementation choices independently and keep progressing. Ask concise questions when missing information changes the game direction or prevents required setup; continue independent work while waiting. Give brief progress updates focused on results and the next uncertainty to resolve.

Use the GitHub integration for repository context and review when needed. Keep changes small and reviewable. Do not overwrite unrelated work, force-push, purchase assets, change repository visibility, publish releases, or introduce paid services without my authorization. Do not invent collaboration or publish changes to other people on my behalf.

At each completed increment, report the playable result, how to launch and test it, relevant verification, and any material limitations. Update the handoff documents. Continue within the milestone's scope until it is complete or a concrete dependency requires my input.

Begin by inspecting the repository and reconciling WILDFEAST_PLAN.md with the actual project. Then implement the earliest unfinished step of A Strange First Supper once the required setup choices are established.
