# AI Companion Builder

🌍 [English](../README.md) · **Português** · [Español](README-es.md) · [Français](README-fr.md) · [Italiano](README-it.md)

Um **mod BepInEx 5 para o Valheim 1.0** que dá ao seu companheiro o trabalho de **construir o que você desenha**.

> **Estado: fundação (0.1.0).** O plugin carrega, tem as configurações dele e o DevBridge dele, e o conhecimento de
> construção do projeto AI Companions foi movido para cá — o catálogo de peças, o leitor de blueprint, as tabelas de
> apoio e de falha, as regras de terreno. **O construtor que coloca as peças é o próximo passo**; até ele chegar, este
> mod carrega e não faz nada no mundo.

## O que é

| | |
|---|---|
| GUID | `julianu37.AICompanionBuilder` |
| DLL | `CompanionBuilder.dll` |
| configurações | `BepInEx\config\AiCompanionBuilder.cfg` |
| pasta do plugin | `BepInEx\plugins\AI-Companion-Builder\` |
| DevBridge | `127.0.0.1:47111` (desligado por padrão; o AI Companions usa 47110) |

Ele instala **lado a lado** com o [AI Companions](https://github.com/julianu37/AI-Companions) e nenhum depende do
outro: GUID diferente, DLL diferente, arquivo de configuração diferente e porta de DevBridge diferente. Remover um não
mexe no outro.

## Instalação

1. Instale o **BepInEx 5** na pasta do Valheim.
2. Copie `CompanionBuilder.dll` para `BepInEx\plugins\AI-Companion-Builder\`.
3. Abra o jogo uma vez. O arquivo de configuração aparece em `BepInEx\config\AiCompanionBuilder.cfg`.
4. Confira `BepInEx\LogOutput.log` por `Loading [AI Companion Builder <versão>]` e `loaded successfully!`.

## Configurações

| Seção | Chave | Padrão | O que faz |
|---|---|---|---|
| `Builder` | `Enabled` | `true` | O construtor trabalha. Desligado = carrega e não faz nada |
| `Builder` | `DebugLog` | `false` | Escreve cada decisão e cada tentativa, não só o resumo |
| `Advanced` | `DevBridge` | `false` | Liga a ponte de desenvolvimento (só loopback) |
| `Advanced` | `DevBridgePort` | `47111` | A porta dessa ponte |

## Compilar do código

`Environment.props` é **ignorado pelo git** — copie `Environment.props.example` para `Environment.props` e defina
`ValheimDir` (e `DeployDir` para copiar a DLL para `BepInEx\plugins\AI-Companion-Builder` depois de cada build). **Não
existe pacote NuGet do Valheim**: as assemblies do jogo são lidas no lugar, de `$(ValheimDir)\valheim_Data\Managed` e
`$(ValheimDir)\BepInEx\core`.

```powershell
# build (também DEPLOYA na pasta do jogo)
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release

# build sem tocar na instalação
dotnet build src/CompanionBuilder/CompanionBuilder.csproj -c Release -p:SkipDeploy=true
```

## Documentação

- `docs/backlog-construcao.md` — a fila de trabalho, com o critério de aceite de cada item
- `docs/plan-construcao.md` — o desenho da construção autônoma
- `docs/re/building-notes.md` — as runs de construção medidas, uma seção por run
- `docs/re/ciclo-de-run-devbridge.md` — o procedimento para medir uma construção no jogo

## Crédito

Derivado do [Offline Companions Reborn](https://github.com/MaddhattrRocks/Offline-Companions-Reborn) de ProfMags
(MIT). Veja `NOTICE.md` e `THIRD_PARTY_NOTICES.md`.
