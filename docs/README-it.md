# AI Companion Builder

🌍 [English](../README.md) · [Português](README-pt.md) · [Español](README-es.md) · [Français](README-fr.md) · **Italiano**

Un **mod BepInEx 5 per Valheim 1.0** che affida al tuo compagno il compito di **costruire ciò che disegni**.

> **Stato: fondamenta (0.1.0).** Il plugin si carica, ha le proprie impostazioni e il proprio DevBridge, e la
> conoscenza di costruzione del progetto AI Companions è stata spostata qui — il catalogo dei pezzi, il lettore di
> blueprint, le tabelle di supporto e di errore, le regole del terreno. **Il costruttore che piazza i pezzi è il passo
> successivo**; fino ad allora, questo mod si carica e non fa nulla nel mondo.

## Cos'è

| | |
|---|---|
| GUID | `julianu37.AICompanionBuilder` |
| DLL | `CompanionBuilder.dll` |
| impostazioni | `BepInEx\config\AiCompanionBuilder.cfg` |
| cartella del plugin | `BepInEx\plugins\AI-Companion-Builder\` |
| DevBridge | `127.0.0.1:47111` (disattivato per impostazione predefinita; AI Companions usa 47110) |

Si installa **accanto** ad [AI Companions](https://github.com/julianu37/AI-Companions) e nessuno dei due dipende
dall'altro: GUID diverso, DLL diversa, file di configurazione diverso e porta DevBridge diversa. Rimuoverne uno non
tocca l'altro.

## Installazione

1. Installa **BepInEx 5** nella cartella di Valheim.
2. Copia `CompanionBuilder.dll` in `BepInEx\plugins\AI-Companion-Builder\`.
3. Avvia il gioco una volta. Il file di configurazione appare in `BepInEx\config\AiCompanionBuilder.cfg`.
4. Controlla `BepInEx\LogOutput.log` per `Loading [AI Companion Builder <versione>]` e `loaded successfully!`.

## Impostazioni

| Sezione | Chiave | Predefinito | Cosa fa |
|---|---|---|---|
| `Builder` | `Enabled` | `true` | Il costruttore lavora. Spento = si carica e non fa nulla |
| `Builder` | `DebugLog` | `false` | Scrive ogni decisione e ogni tentativo, non solo il riepilogo |
| `Advanced` | `DevBridge` | `false` | Avvia il ponte di sviluppo (solo loopback) |
| `Advanced` | `DevBridgePort` | `47111` | La porta di quel ponte |

## Compilare dai sorgenti

`Environment.props` è **ignorato da git** — copia `Environment.props.example` in `Environment.props` e imposta
`ValheimDir` (e `DeployDir` per copiare la DLL in `BepInEx\plugins\AI-Companion-Builder` dopo ogni build). **Non esiste
alcun pacchetto NuGet di Valheim**: le assembly del gioco vengono lette sul posto, da
`$(ValheimDir)\valheim_Data\Managed` e `$(ValheimDir)\BepInEx\core`.

```powershell
# build (DISPEGA anche nella cartella del gioco)
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release

# build senza toccare l'installazione
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release -p:SkipDeploy=true
```

## Documentazione

- `docs/backlog-construcao.md` — la coda di lavoro, con il criterio di accettazione di ogni voce
- `docs/plan-construcao.md` — il progetto della costruzione autonoma
- `docs/re/building-notes.md` — le esecuzioni di costruzione misurate, una sezione per esecuzione
- `docs/re/ciclo-de-run-devbridge.md` — la procedura per misurare una costruzione nel gioco

## Crediti

Derivato da [Offline Companions Reborn](https://github.com/MaddhattrRocks/Offline-Companions-Reborn) di ProfMags (MIT).
Vedi `NOTICE.md` e `THIRD_PARTY_NOTICES.md`.
