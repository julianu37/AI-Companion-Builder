# AI Companion Builder

🌍 [English](../README.md) · [Português](README-pt.md) · [Español](README-es.md) · **Français** · [Italiano](README-it.md)

Un **mod BepInEx 5 pour Valheim 1.0** qui confie à votre compagnon le travail de **construire ce que vous dessinez**.

> **État : fondations (0.1.0).** Le plugin se charge, a sa propre configuration et son propre DevBridge, et le savoir
> de construction du projet AI Companions a été déplacé ici — le catalogue de pièces, le lecteur de blueprint, les
> tables de support et d'échec, les règles de terrain. **Le constructeur qui pose les pièces est l'étape suivante** ;
> d'ici là, ce mod se charge et ne fait rien dans le monde.

## Ce que c'est

| | |
|---|---|
| GUID | `julianu37.AICompanionBuilder` |
| DLL | `CompanionBuilder.dll` |
| configuration | `BepInEx\config\AiCompanionBuilder.cfg` |
| dossier du plugin | `BepInEx\plugins\AI-Companion-Builder\` |
| DevBridge | `127.0.0.1:47111` (désactivé par défaut ; AI Companions utilise 47110) |

Il s'installe **à côté** de [AI Companions](https://github.com/julianu37/AI-Companions) et aucun ne dépend de l'autre :
GUID différent, DLL différente, fichier de configuration différent et port DevBridge différent. Retirer l'un ne touche
pas l'autre.

## Installation

1. Installez **BepInEx 5** dans votre dossier Valheim.
2. Copiez `CompanionBuilder.dll` dans `BepInEx\plugins\AI-Companion-Builder\`.
3. Lancez le jeu une fois. Le fichier de configuration apparaît dans `BepInEx\config\AiCompanionBuilder.cfg`.
4. Vérifiez `BepInEx\LogOutput.log` pour `Loading [AI Companion Builder <version>]` et `loaded successfully!`.

## Configuration

| Section | Clé | Défaut | Ce qu'elle fait |
|---|---|---|---|
| `Builder` | `Enabled` | `true` | Le constructeur travaille. Désactivé = il se charge et ne fait rien |
| `Builder` | `DebugLog` | `false` | Écrit chaque décision et chaque tentative, pas seulement le résumé |
| `Advanced` | `DevBridge` | `false` | Lance le pont de développement (loopback uniquement) |
| `Advanced` | `DevBridgePort` | `47111` | Le port de ce pont |

## Compiler depuis les sources

`Environment.props` est **ignoré par git** — copiez `Environment.props.example` vers `Environment.props` et définissez
`ValheimDir` (et `DeployDir` pour copier la DLL dans `BepInEx\plugins\AI-Companion-Builder` après chaque build). Il
n'existe **aucun paquet NuGet pour Valheim** : les assemblies du jeu sont lues sur place, depuis
`$(ValheimDir)\valheim_Data\Managed` et `$(ValheimDir)\BepInEx\core`.

```powershell
# build (DÉPLOIE aussi dans le dossier du jeu)
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release

# build sans toucher à l'installation
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release -p:SkipDeploy=true
```

## Documentation

- `docs/backlog-construcao.md` — la file de travail, avec le critère d'acceptation de chaque élément
- `docs/plan-construcao.md` — la conception de la construction autonome
- `docs/re/building-notes.md` — les exécutions de construction mesurées, une section par exécution
- `docs/re/ciclo-de-run-devbridge.md` — la procédure pour mesurer une construction en jeu

## Crédit

Dérivé de [Offline Companions Reborn](https://github.com/MaddhattrRocks/Offline-Companions-Reborn) par ProfMags (MIT).
Voir `NOTICE.md` et `THIRD_PARTY_NOTICES.md`.
