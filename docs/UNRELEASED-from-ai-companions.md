## Unreleased

**The building moved out to its own mod: AI Companion Builder.** This mod no longer builds anything - not a single
line of building code, not a `[Building]` setting, not a blueprint command on the DevBridge. Everything that builds
(blueprints, templates, single pieces, the build panel, the support and retry tables) lives in the new mod, which
installs side by side and has its own GUID, DLL, config file and DevBridge port.

- **What leaves:** 41 `AI/Build*` tables, `MHR_Build`, `MHR_BuildGroup`, `MHR_Level`, the build panel and its map pins,
  25 `[Building]` settings (369 -> 344 options), the `build.*` / `blueprints.*` / `terrain.*` / `companions.build*`
  DevBridge commands, and 110 translation keys.
- **What stays, and why:** the story of the ground (`MHR_Level`'s tolerance) is now inline in the mover, the prefab and
  layer lookups the bridge needs are `GameProbe`, and the plain placement `world.chest` uses is `GamePlacement`. None of
  them builds a plan.
- **The DevBridge is still here, on the same port (47110).** The builder's bridge is 47111.

