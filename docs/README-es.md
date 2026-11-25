# AI Companion Builder

🌍 [English](../README.md) · [Português](README-pt.md) · **Español** · [Français](README-fr.md) · [Italiano](README-it.md)

Un **mod de BepInEx 5 para Valheim 1.0** que le da a tu compañero el trabajo de **construir lo que dibujas**.

> **Estado: cimientos (0.1.0).** El plugin carga, tiene su propia configuración y su propio DevBridge, y el
> conocimiento de construcción del proyecto AI Companions se trasladó aquí — el catálogo de piezas, el lector de
> blueprint, las tablas de soporte y de fallo, las reglas de terreno. **El constructor que coloca las piezas es el
> siguiente paso**; hasta que llegue, este mod carga y no hace nada en el mundo.

## Qué es

| | |
|---|---|
| GUID | `julianu37.AICompanionBuilder` |
| DLL | `CompanionBuilder.dll` |
| configuración | `BepInEx\config\AiCompanionBuilder.cfg` |
| carpeta del plugin | `BepInEx\plugins\AI-Companion-Builder\` |
| DevBridge | `127.0.0.1:47111` (desactivado por defecto; AI Companions usa 47110) |

Se instala **junto a** [AI Companions](https://github.com/julianu37/AI-Companions) y ninguno depende del otro: GUID
distinto, DLL distinta, archivo de configuración distinto y puerto de DevBridge distinto. Quitar uno no toca al otro.

## Instalación

1. Instala **BepInEx 5** en tu carpeta de Valheim.
2. Copia `CompanionBuilder.dll` a `BepInEx\plugins\AI-Companion-Builder\`.
3. Abre el juego una vez. El archivo de configuración aparece en `BepInEx\config\AiCompanionBuilder.cfg`.
4. Revisa `BepInEx\LogOutput.log` en busca de `Loading [AI Companion Builder <versión>]` y `loaded successfully!`.

## Configuración

| Sección | Clave | Por defecto | Qué hace |
|---|---|---|---|
| `Builder` | `Enabled` | `true` | El constructor trabaja. Apagado = carga y no hace nada |
| `Builder` | `DebugLog` | `false` | Escribe cada decisión y cada intento, no solo el resumen |
| `Advanced` | `DevBridge` | `false` | Enciende el puente de desarrollo (solo loopback) |
| `Advanced` | `DevBridgePort` | `47111` | El puerto de ese puente |

## Compilar desde el código

`Environment.props` está **ignorado por git** — copia `Environment.props.example` a `Environment.props` y define
`ValheimDir` (y `DeployDir` para copiar la DLL a `BepInEx\plugins\AI-Companion-Builder` después de cada build). **No
hay paquete NuGet de Valheim**: las assemblies del juego se leen en su sitio, desde
`$(ValheimDir)\valheim_Data\Managed` y `$(ValheimDir)\BepInEx\core`.

```powershell
# build (también DEPLOYA en la carpeta del juego)
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release

# build sin tocar la instalación
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release -p:SkipDeploy=true
```

## Documentación

- `docs/backlog-construcao.md` — la cola de trabajo, con el criterio de aceptación de cada elemento
- `docs/plan-construcao.md` — el diseño de la construcción autónoma
- `docs/re/building-notes.md` — las ejecuciones de construcción medidas, una sección por ejecución
- `docs/re/ciclo-de-run-devbridge.md` — el procedimiento para medir una construcción en el juego

## Crédito

Derivado de [Offline Companions Reborn](https://github.com/MaddhattrRocks/Offline-Companions-Reborn) de ProfMags
(MIT). Ver `NOTICE.md` y `THIRD_PARTY_NOTICES.md`.
