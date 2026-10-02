# Current implemented content budget

The creator authorized five designed island maps, more original ecology/cuisine, grounded vegetation and expanded Options. The delivered archipelago contains five maps, 19 ingredients, 20 recipes, five food creatures, six crops and six original music themes. It retains the Windows/offline/free target and the existing service, inventory and action animation systems. ARCHIPELAGO_EXPANSION.md records the current scope. Earlier smaller budgets below are historical constraints, superseded by this request.

# Production planning

## Current playable scope

Two authored outdoor areas and one restaurant; six ingredient definitions, six recipes, five purchasable improvements, three crop plots, three to five guest orders per service, one employee, four sequential harbor requests, and a short island letter. Primary interactions: movement, fishing, foraging, telegraphed hunting, cultivation, cooking, delivery, investment, and travel.

This satisfies the plan's small-world implementation scope. It is a prototype for evaluating production, not evidence that a large finished game is ready to ship.

## Suggested next public demo budget

Keep the present two-island footprint. Aim for a polished 30–60 minute journey without requiring endless repetition. Retain six recipes and five upgrades while refining art, onboarding, music, service pacing, and ecology. Add content only when it fixes a gap in the journey. Treat the suggested duration as a design target until measured with players.

Before setting a full game's size, measure the labor needed for one habitat, one animated creature, one fishing variation, one original dish, one customer, and one complete story request. Build a content spreadsheet from those observed costs. Do not extrapolate release dates from code completion alone.

## Prioritized production backlog

1. Observe at least five fresh-player sessions. Measure first-catch comprehension, time to first served dish, mistaken interactions, route confusion, and whether players want another expedition.
2. Refine the creature and food art with the creator; add four-direction movement and animated ecology.
3. Adjust recipes and upgrade costs using service revenue and player route data; reduce chores that do not support discovery.
4. Improve ingredient journal presentation and recipe discovery celebrations.
5. Add richer ambient audio and a small music set; tune levels with sound off and headphones.
6. Test physical controllers and navigation; add rebinding, text-size options, and supported aspect-ratio checks.
7. Harden saves through upgrade migrations, error-injection tests, and long-session regression checks.
8. Prepare an accessible public demo package, credits, a feedback channel, and a clear statement of supported platforms.

## Performance budget

Proposed target: a stable 60 FPS at 1280 × 720 on an agreed reference Windows PC, with 1080p and non-integer window-size checks. Keep per-frame work limited to active interactions, camera/player animation, and small UI updates. Keep menus from allocating every frame; rebuild only on player actions. Avoid hundreds of active physics bodies, unnecessary real-time lights, and texture growth before profiling indicates a need.

Runtime smoke verification records actual frame timing and allocated memory in its result folder. Those observations apply only to the test machine, build, and sample window; they are not a minimum hardware specification. Establish minimum requirements by testing representative hardware later.

## Accessibility and feedback

- No sound-only required cues: fishing, hunting, and cooking expose visual instruction.
- No color-only required cues: tension and cooking also use labels and text.
- Relaxed timing enabled by default; standard timing available.
- Failed gathering can be retried; mistimed cooking still produces a serveable meal.
- Exploration has no continuous countdown; menus can be read without a daily clock running.
- No crop death or ingredient spoilage in the current scope.
- Mute option and independent readable UI scaling.
- No camera shake. Further work: font size, key rebinding, reduced motion, contrast review, and physical controller testing.

## Onboarding and narrative

The first-launch panel teaches the complete daily loop. A persistent next-discovery prompt follows progression. Physical interaction prompts name actions and guest orders; the journal explains habitats, food properties, and creature behavior. Free harbor grain is documented in the journal and README. The tidekeeper's letter connects the second island to the restaurant's purpose.

Human testing must still confirm that the UI explains these ideas at the right time. Automated checks establish correctness, not delight or comprehension.

## Release gate

Compile and build cleanly; pass economy/save checks; repeat a full journey in the standalone build; inspect screenshots at intended display sizes; complete fresh-player playtests; resolve critical usability issues; verify all credits/licenses; test save recovery and supported hardware. Publish only after the creator authorizes release. No ads, purchases, accounts, or recurring costs are part of the confirmed business model.
