# AI Companion Builder

A **BepInEx 5 mod for Valheim 1.0** (C#, net472, Harmony): the **build half** of the AI Companions mod as a mod of its
own — blueprint and template construction, with its own builder and its own identity.

It installs **side by side** with [AI Companions](https://github.com/julianu37/AI-Companions) and neither depends on
the other: different GUID, different DLL, different config file. Removing one does not touch the other.

| | |
|---|---|
| GUID | `julianu37.AICompanionBuilder` |
| DLL | `CompanionBuilder.dll` |
| config | `BepInEx\config\AiCompanionBuilder.cfg` |
| plugin folder | `BepInEx\plugins\AI-Companion-Builder\` |

> **Status: foundation (0.1.0).** The plugin boots, loads its config and reports itself; the build pipeline arrives
> file by file, each one proven before it is wired to anything.

## Setup

`Environment.props` is **gitignored** — copy `Environment.props.example` to `Environment.props` and set `ValheimDir`
(and `DeployDir` to copy the DLL into `BepInEx\plugins\AI-Companion-Builder` after every build). There is **no NuGet
package for Valheim**: the game assemblies are referenced in place from `$(ValheimDir)\valheim_Data\Managed` and
`$(ValheimDir)\BepInEx\core`.

## Commands

```powershell
# build (also DEPLOYS to the game folder — see the pitfall below)
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release

# build without touching the install (worktree, agent, test)
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release -p:SkipDeploy=true
```

## Pitfalls

- **The build deploys.** Plain `dotnet build` copies the DLL into the game folder. Use `-p:SkipDeploy=true` when you
  must not touch the install.
- **BepInEx loads the DLL at boot.** A build does not change a running game: close and reopen it, or you are measuring
  the previous DLL.
- **Two mods, two identities.** Never reuse the companion mod's GUID, its `HC_*` ZDO keys or its prefabs here — a
  shared network identity across two DLLs is how "the same mod loaded twice" bugs start.
- **Load check:** `BepInEx\LogOutput.log` must contain `Loading [AI Companion Builder <version>]` and
  `loaded successfully!`.

## Where this came from

The build code being ported lives in the AI Companions repository (`src/Companions/Jobs/MHR_Build.cs` and the
`AI/Build*.cs` tables). The port keeps the parts that were measured there (blueprint parsing, the support/failure
tables, the material maths) and writes a builder of its own — the companion's movement engine is not copied, because
it belongs to the companion.
