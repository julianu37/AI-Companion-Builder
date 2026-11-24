# Modo Fácil e painel de construção v2 — desenho

**Data:** 18/11/2026 · **Branch:** `feat/build-easy-mode-and-panel` · **Decidido com o jogador nesta sessão**

## Por que

Hoje o mod constrói **76 de 348** blueprints do corpus. Medi os 348 pelo próprio `blueprints.inspect`: as
**381.110 linhas** puladas são **todas** `recipe not unlocked` — **zero** linhas têm prefab inexistente. O gate de
receita é o único bloqueador de "conseguir construir", e ele é removível.

O segundo bloqueador é a altura: **129 de 348** (37%) têm peça com `y` negativo, desenhada para ser enterrada. O
`DeckLift` atual sobe no máximo 1 m e não resolve.

Medição completa em `docs/backlog-construcao.md` §0 e nesta sessão (18/11/2026).

## Objetivos

1. **Todas as blueprints construíveis** num modo explícito, escolhido pelo jogador.
2. **O jogador vê antes de começar** o que vai entrar, o que vai faltar e por quê.
3. **O painel mostra onde as obras estão** sem precisar procurar.

## Não-objetivos

- Pré-visualização 3D renderizada (decisão: gizmo de contorno — ver Abordagens).
- Resolver a altura automaticamente nesta rodada (fica registrado, não entra).
- Navio/`m_waterPiece`: **1 blueprint** de 348 e a resposta certa é recusar com mensagem, não "resolver".

## Abordagens consideradas

### Modo fácil: onde mora o interruptor

| abordagem | o que faz | veredito |
|---|---|---|
| **A. Terceira flag em `BuildBlueprintSkip`** | `Reason(hasPiece, recipeKnown, easy)` devolve `null` quando `easy` | **escolhida** — é uma linha no ponto exato onde a decisão já mora, e o painel, a ordem e o `blueprints.inspect` continuam concordando por construção |
| B. Liberar receitas no `Player` | mexer em `IsRecipeKnown` / desbloquear receitas no save | rejeitada: muda o save do jogador, efeito colateral permanente para um switch de sessão |
| C. `BlueprintFree` existente estendido | sobrecarregar a flag de material | rejeitada: mistura duas decisões e quebra quem já usa `BlueprintFree` para testar só o custo |

### Painel: pré-visualização

| abordagem | custo | veredito |
|---|---|---|
| **A. Gizmo de contorno** | desenhar AABB das peças com `GL`/linhas; zero instanciação | **escolhida (decisão do jogador)** |
| B. RenderTexture com câmera isolada | 985 `GameObject`s com `ZNetView`/`WearNTear`; risco de vazar objetos no mundo | rejeitada nesta rodada: o risco aparece só depois de várias aberturas |

### Painel: várias obras

Hoje `MHR_Build.CurrentSite` devolve **um site por jogador** (`MHR_BuildGroup.SiteBusy`). O jogador decidiu
**mudar para vários**. Isso é a maior parte do trabalho desta spec.

## O desenho

### 1. Modo Fácil (três gates juntos)

- `ModConfig`: nova opção `[Building] EasyBuild` (bool, default **false**), e o bloco do painel que a liga.
- `BuildBlueprintSkip.Reason(hasPiece, recipeKnown, easy)`: com `easy`, receita não bloqueia. Prefab inexistente
  **continua** bloqueando (não há o que colocar).
- `EasyBuild` liga, de uma vez: **receita** (novo), **material** (`BlueprintFree`), **estação** (`BlueprintStations`).
  Os três continuam existindo individualmente no `.cfg` como válvula de escape; o painel expõe o **modo**, não os
  três checkboxes soltos.
- **Selo no painel** (decisão do jogador): com o modo ligado, um selo permanente
  `modo fácil: N peça(s) sem receita`. Não bloqueia nada — serve para o relatório do run ficar honesto.
- **Modo Normal** passa a avisar **antes** de começar: a contagem de receitas faltando aparece no painel quando uma
  blueprint é selecionada.

### 2. Simulação antes de construir

Na seleção de uma blueprint, o painel mostra (reaproveitando `BuildBlueprintReport`, que o `blueprints.inspect` já usa):
peças do plano, peças que entram, peças puladas **nomeadas por motivo**, materiais totais, e **o tamanho de terreno
necessário** (do `bounds`). O mod já nivela sozinho (`LevelGround`), então o aviso é informativo: *"precisa de
30,6 × 29,5 m; o terreno atual varia 1,67 m — o companheiro vai nivelar"*.

> **Estado em 18/11/2026 — escopo reduzido.** Três revisores independentes (fatos, cobertura, netcode) mediram o
> código depois da aprovação inicial. Resultado: **só as seções 1 e 2 estão neste ciclo** (Entrega A). As seções 3 a 6
> estão **adiadas**: a 3 (várias obras) não é segura sem 5 guardas que não existem — o líder da obra nova é arrancado
> da antiga e o canteiro pode desaparecer em silêncio; os detalhes medidos e o critério de reabertura estão em
> `docs/backlog-construcao.md` §0b. Este documento **não** é mais a descrição do que será construído agora; é o
> desenho-alvo. O ciclo em execução é `docs/superpowers/plans/2026-11-18-modo-facil-e-painel.md`.

### 3. Várias obras ao mesmo tempo *(ADIADO — ver backlog §0b)*

- `CurrentSite(owner, out SiteInfo)` → **`CurrentSites(owner)`** devolvendo a lista. Mantém um overload de um site
  para os chamadores que só querem o primeiro (o painel antigo, o DevBridge).
- `MHR_BuildGroup.SiteBusy` deixa de bloquear a segunda obra **do mesmo jogador**; continua bloqueando dois
  companheiros no mesmo canteiro.
- **Cancelar/desistir por obra**: o painel ganha o botão em cada aba, ligado ao `MHR_Build.Cancel`/`CancelSite` que
  já existe.
- **Materiais compartilhados**: cada obra continua pagando dos baús **próximos da sua própria âncora**
  (`SiteMaterials` já é por âncora). Duas obras perto uma da outra competem pelos mesmos baús — isso é aceito e
  mostrado no painel como aviso, não é corrigido nesta rodada.

### 4. Abas por obra no painel *(ADIADO — proximo ciclo)*

- Um seletor no topo (padrão do submenu de spawn de companions que o jogador citou) com uma aba por obra ativa:
  rótulo da blueprint, fase e progresso `placed/plan`.
- A aba selecionada desenha os quatro blocos que já existem (site, companheiros, materiais, blueprints).
- **Seletor de companheiro por obra**: o bloco de companheiros ganha um dropdown dos companheiros disponíveis
  (`CompanionOrders.CanWorkSite`, a mesma regra do painel hoje) com "entrar nesta obra".

### 5. Pins no mapa *(ADIADO — proximo ciclo)*

- `Minimap.AddPin(pos, PinType.Icon1|Icon2|Icon3, name, save:false, ...)` na âncora de cada obra:
  **em andamento** (Icon1), **completa** (Icon2), **cancelada** (Icon3).
- `save: false` — pin de sessão, não entra no save do mundo (não polui o mapa do jogador).
- O tooltip leva o progresso (`"CommHome — 857/985"`).
- Removidos por `RemovePin` quando a obra sai da lista ativa.

### 6. Gizmo de contorno *(ADIADO — proximo ciclo)*

- Com a blueprint selecionada no painel, desenhar o AABB de cada peça como linhas no mundo, na âncora escolhida.
- Sem `GameObject` por peça: só posições do plano (que o `BuildBlueprintParser` já entrega).
- Some quando o painel fecha ou a ordem começa.

## Consequências

- `AGENTS.md` e `package/CONFIGURATION.md` (gerado) ganham a opção nova — o gate `gen_config_doc --check` exige.
- Novas chaves `hc_cfg_building_easybuild` nos **4 JSONs** de tradução (gate `audit_translations.py --check-config`).
- Harness: `BuildBlueprintSkip.Reason` ganha asserções para o parâmetro `easy` (só adições, regra do repo).
- O `BlueprintFree`/`BlueprintStations` atuais **não mudam** de nome nem de default.

## O que fica para depois

- Altura automática (129 blueprints com `y` negativo) — desenho próprio, precisa decidir o que "chão" significa.
- Pré-visualização 3D renderizada.
- Materiais compartilhados entre obras vizinhas.
- `m_waterPiece`: detectar navio e recusar com mensagem nomeada.
