# AI Companion Builder

🌍 **English** · [Português](docs/README-pt.md) · [Español](docs/README-es.md) · [Français](docs/README-fr.md) · [Italiano](docs/README-it.md)

A **BepInEx 5 mod for Valheim 1.0** that gives your companion the job of **building what you draw**.

> **Status: foundation (0.1.0).** The plugin loads, has its own settings and its own DevBridge, and the build
> knowledge from the AI Companions project has been moved here — the piece catalog, the blueprint reader, the
> support and failure tables, the terrain rules. **The builder that places the pieces is the next step**; until it
> lands, this mod loads and does nothing in the world.

## What it is

| | |
|---|---|
| GUID | `julianu37.AICompanionBuilder` |
| DLL | `CompanionBuilder.dll` |
| settings | `BepInEx\config\AiCompanionBuilder.cfg` |
| plugin folder | `BepInEx\plugins\AI-Companion-Builder\` |
| DevBridge | `127.0.0.1:47111` (off by default; the AI Companions mod uses 47110) |

It installs **side by side** with [AI Companions](https://github.com/julianu37/AI-Companions) and neither depends on
the other: a different GUID, a different DLL, a different settings file and a different DevBridge port. Removing one
does not touch the other.

## Install

1. Install **BepInEx 5** in your Valheim folder.
2. Copy `CompanionBuilder.dll` into `BepInEx\plugins\AI-Companion-Builder\`.
3. Start the game once. The settings file appears at `BepInEx\config\AiCompanionBuilder.cfg`.
4. Check `BepInEx\LogOutput.log` for `Loading [AI Companion Builder <version>]` and `loaded successfully!`.

## Settings

| Section | Key | Default | What it does |
|---|---|---|---|
| `Builder` | `Enabled` | `true` | The builder works at all. Off = it loads and does nothing |
| `Builder` | `DebugLog` | `false` | Writes every decision and every retry, not only the summary |
| `Advanced` | `DevBridge` | `false` | Runs the development bridge (loopback only) |
| `Advanced` | `DevBridgePort` | `47111` | The port of that bridge |

## Building from source

`Environment.props` is **gitignored** — copy `Environment.props.example` to `Environment.props` and set `ValheimDir`
(and `DeployDir` to copy the DLL into `BepInEx\plugins\AI-Companion-Builder` after every build). There is **no NuGet
package for Valheim**: the game assemblies are read in place from `$(ValheimDir)\valheim_Data\Managed` and
`$(ValheimDir)\BepInEx\core`.

```powershell
# build (also DEPLOYS to the game folder)
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release

# build without touching the install
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release -p:SkipDeploy=true
```

## Documentation

- **[README (5 languages)](docs/README-pt.md)** — the same page in Portuguese, Spanish, French and Italian
- `docs/backlog-construcao.md` — the work queue, with the acceptance criteria of each item
- `docs/plan-construcao.md` — the design of the autonomous build
- `docs/re/building-notes.md` — the measured build runs, one section per run
- `docs/re/ciclo-de-run-devbridge.md` — the procedure to measure a build in game

## Credit

Derived from [Offline Companions Reborn](https://github.com/MaddhattrRocks/Offline-Companions-Reborn) by ProfMags
(MIT). See `NOTICE.md` and `THIRD_PARTY_NOTICES.md`.
