# Modo Fácil — Plano de Implementação (Entrega A)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tornar as 348 blueprints do corpus construíveis por escolha explícita do jogador, com o painel dizendo **antes** o que vai entrar, o que vai faltar e por quê.

**Architecture:** O modo fácil é **uma combinação de três gates** lidos em três pontos distintos do código (receita em `BuildBlueprintSkip`, material em `MHR_Build.cs:2570`, estação em `MHR_Build.cs:4756`) mais um controle no painel. Cada gate ganha o "OU o modo fácil" no **próprio ponto de leitura** — não existe um estado de config que reescreve outro. O selo conta com `easy: false` de propósito, para não mentir zero.

**Tech Stack:** C# net472 / BepInEx 5 / Unity uGUI (ver skill `ui-ugui`) / harness `net472` em `tools/ai-intents-check`.

**Spec:** `docs/superpowers/specs/2026-11-18-modo-facil-e-painel-design.md` (as entregas B e C da spec estão **fora deste ciclo** — ver `docs/backlog-construcao.md` §0b)

## Global Constraints

- **`Bind` do `ModConfig` fica numa linha só.** `gen_config_doc.py:19` e `audit_translations.py:203` extraem com
  `^\t\t\w+ = Bind\(cfg, "([^"]+)", "([^"]+)", (.*)\);$` e `re.M` — `.` não atravessa linha. Um `Bind` quebrado em duas
  linhas **some** das duas ferramentas e o gate fica cego (verde sem a chave nos JSONs).
- **`ConfigEntry` novo exige chave `hc_cfg_<secao>_<chave>` nos 4 JSONs** no mesmo ciclo.
- **`package/CONFIGURATION.md` é gerado** — nunca editar à mão.
- **Comentários de código em inglês; log em PT-BR.**
- **Harness só adiciona asserção.** Seção nova exige `new SectionDef(...)` registrada, senão `--only=` sai **exit 2**.
- **Arquivo puro novo em `src/Companions/AI/` exige `<Compile Include>` em `tools/ai-intents-check/ai-intents-check.csproj`.**
- **Build e harness em série**; `-p:SkipDeploy=true` fora do jogo.

## Review Focus

1. **Modo fácil com prefab que não existe** — tem de continuar pulando e nomeando. O modo libera **receita**, nunca a existência do prefab.
2. **`BlueprintFree=true` e `EasyBuild=false`** (e o inverso) — os dois são independentes; o painel e o relatório não podem confundir um com o outro.
3. **Selo mostrando zero** — acontece se ele ler `report.Skipped` com o modo ligado. O número vem de uma contagem com `easy: false`.
4. **Aviso misturando as duas razões** — `Skipped` soma `NotAPiece` + `RecipeLocked`; o aviso tem de distinguir, senão o jogador caça receita para um item que não é peça.
5. **Painel aberto sem blueprint selecionada** — selo e aviso não podem lançar nem mostrar "0 peças" mentiroso.

---

### Task A1: `BuildBlueprintSkip` aprende o modo fácil

**Files:**
- Modify: `src/Companions/AI/BuildBlueprintSkip.cs`
- Test: `tools/ai-intents-check/Program.cs` (seção `ac`, helpers em ~3505)

**Interfaces:**
- Consumes: nada (arquivo puro, sem dependências)
- Produces: `internal static string Reason(bool hasPiece, bool recipeKnown, bool easy)`. Hoje existe **um** método; ele é mantido e passa a delegar com `easy: false`, então os 3 chamadores (`MHR_Build.cs:2353`, `BuildBlueprintReport.cs:139`, `Program.cs:3509`) não mudam nesta tarefa.

- [ ] **Step 1: Write the failing test**

A seção `ac` tem **4** chamadas de `SkipCase` (mais 2 `CheckText` e 1 `Check`). **Não estenda o `SkipCase`** — os 4
casos antigos deixariam de provar o caminho normal. Adicione um helper novo ao lado:

```csharp
private static void SkipCaseEasy(bool hasPiece, bool recipeKnown, bool easy, string expected, string why)
{
    CheckText("Reason(hasPiece: " + (hasPiece ? "true" : "false") + ", recipeKnown: "
        + (recipeKnown ? "true" : "false") + ", easy: " + (easy ? "true" : "false") + ") - " + why,
        expected, BuildBlueprintSkip.Reason(hasPiece, recipeKnown, easy));
}
```

E os 5 casos:

```csharp
SkipCaseEasy(false, false, true, BuildBlueprintSkip.NotAPiece, "modo facil nao torna um item uma peca");
SkipCaseEasy(false, true, true, BuildBlueprintSkip.NotAPiece, "nem com a receita conhecida");
SkipCaseEasy(true, false, true, null, "modo facil com a receita travada: entra no plano");
SkipCaseEasy(true, true, true, null, "modo facil com receita conhecida: entra igual");
SkipCaseEasy(true, false, false, BuildBlueprintSkip.RecipeLocked, "modo normal continua pulando");
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet build Companions.sln -c Release -p:SkipDeploy=true && dotnet run --project tools/ai-intents-check -c Release -- --only=(ac)`
Expected: **erro de compilação** do harness (`Reason` não tem sobrecarga de 3 argumentos). Não é FAIL de asserção —
o harness nem sobe.

- [ ] **Step 3: Implement the third parameter**

```csharp
internal static string Reason(bool hasPiece, bool recipeKnown)
{
    return Reason(hasPiece, recipeKnown, false);
}

internal static string Reason(bool hasPiece, bool recipeKnown, bool easy)
{
    if (!hasPiece) return NotAPiece;
    if (!recipeKnown && !easy) return RecipeLocked;
    return null;
}
```

- [ ] **Step 4: Run the whole harness**

Run: `dotnet build Companions.sln -c Release -p:SkipDeploy=true && dotnet run --project tools/ai-intents-check -c Release`
Expected: **1256 PASS / 0 FAIL** (1251 + 5), `TODOS OS TESTES PASSARAM`.

- [ ] **Step 5: Commit**

```bash
git add src/Companions/AI/BuildBlueprintSkip.cs tools/ai-intents-check/Program.cs
git commit -m "feat(build): o modo facil e um terceiro parametro da regra de pulo, nao uma regra nova"
```

---

### Task A2: config `[Building] EasyBuild`

**Files:**
- Modify: `src/Companions/Core/ModConfig.cs` (campo ~205, `Bind` ~999)
- Modify: `Translations/Portuguese_Brazilian.json`, `Spanish.json`, `French.json`, `Italian.json`
- Modify: `package/CONFIGURATION.md` (regenerado)

**Interfaces:**
- Consumes: nada
- Produces: `internal static ConfigEntry<bool> BuildEasyBuild;` default `false`.

- [ ] **Step 1: Add the field**

Junto de `BuildBlueprintFree`:

```csharp
internal static ConfigEntry<bool> BuildEasyBuild;
```

- [ ] **Step 2: Add the Bind — ONE line, no wrapping**

Logo após `BuildBlueprintFree`, numa **única linha** (o regex das duas ferramentas não atravessa linha):

```csharp
BuildEasyBuild = Bind(cfg, "Building", "EasyBuild", false, "Easy build: a blueprint ignores the recipe gate, the station and the material cost (the same three the panel's Easy mode switches together). A prefab that does not exist in the game is still skipped and named. Default off.");
```

- [ ] **Step 3: Add the label to the four JSONs**

Chave `hc_cfg_building_easybuild` no bloco `Building`, onde estão as `hc_cfg_building_blueprintfree`.

- [ ] **Step 4: Verify the two gates see the option**

Run: `python tools/audit_translations.py --check-config`
Expected: `lacunas: 0`

Run: `python tools/gen_config_doc.py src/Companions/Core/ModConfig.cs package/CONFIGURATION.md && python tools/gen_config_doc.py --check src/Companions/Core/ModConfig.cs package/CONFIGURATION.md`
Expected: contagem **369** (368 + 1) e `is up to date`.

**Se o `--check-config` passar com a chave faltando, o `Bind` foi quebrado em duas linhas** — é o modo de falha do
Global Constraint. Confira com `grep -c "EasyBuild" package/CONFIGURATION.md` (tem de achar a opção).

- [ ] **Step 5: Commit**

```bash
git add src/Companions/Core/ModConfig.cs package/CONFIGURATION.md Translations/
git commit -m "feat(config): EasyBuild (default off) com o rotulo nos 4 idiomas"
```

---

### Task A3: os dois outros gates leem o modo fácil

**Files:**
- Modify: `src/Companions/Jobs/MHR_Build.cs:2570` (`free`) e `:4756` (`waived`)
- Modify: `src/Companions/Jobs/MHR_Build.cs:2353` e `src/Companions/AI/BuildBlueprintReport.cs:139` (o `Reason`)

**Interfaces:**
- Consumes: `BuildEasyBuild.Value` (A2), `Reason(bool,bool,bool)` (A1)
- Produces: nada novo. **Esta é a tarefa que faz o modo fácil valer os três gates** — sem ela o interruptor só libera receita.

- [ ] **Step 1: Read the two gate lines first**

```csharp
// MHR_Build.cs:2570
bool free = ModConfig.BuildBlueprintFree != null && ModConfig.BuildBlueprintFree.Value;
// MHR_Build.cs:4756
bool waived = ModConfig.BuildBlueprintStations == null ? BuildStationCover.Default : ModConfig.BuildBlueprintStations.Value;
```

- [ ] **Step 2: OR the mode into both**

```csharp
bool free = (ModConfig.BuildBlueprintFree != null && ModConfig.BuildBlueprintFree.Value)
    || (ModConfig.BuildEasyBuild != null && ModConfig.BuildEasyBuild.Value);
```

```csharp
bool waived = (ModConfig.BuildBlueprintStations == null ? BuildStationCover.Default : ModConfig.BuildBlueprintStations.Value)
    || (ModConfig.BuildEasyBuild != null && ModConfig.BuildEasyBuild.Value);
```

- [ ] **Step 3: Pass the mode to `Reason` in both call sites**

```csharp
    local == null || (resolved != null && local.IsRecipeKnown(resolved.m_name)),
    ModConfig.BuildEasyBuild != null && ModConfig.BuildEasyBuild.Value);
```

- [ ] **Step 4: Build + harness**

Run: `dotnet build Companions.sln -c Release -p:SkipDeploy=true && dotnet run --project tools/ai-intents-check -c Release`
Expected: 0 avisos, 0 FAIL.

- [ ] **Step 5: Commit**

```bash
git add src/Companions/Jobs/MHR_Build.cs src/Companions/AI/BuildBlueprintReport.cs
git commit -m "feat(build): o modo facil vale os tres gates - receita, material e estacao"
```

---

### Task A4: contagem honesta das duas razões de pulo

**Files:**
- Modify: `src/Companions/AI/BuildBlueprintReport.cs`
- Test: `tools/ai-intents-check/Program.cs` (seção `ac`)

**Interfaces:**
- Consumes: `BuildBlueprintSkip.Reason(...)` (A1), `SkipReason { Prefab, Reason, Count }` (`BuildBlueprintReport.cs:26-31`)
- Produces: `internal static void CountByReason(List<SkipReason> reasons, string reason)` — a soma por razão, para o painel contar **sem receita** sem recarregar o arquivo.

**Por que existe:** `report.Skipped` soma `NotAPiece` **e** `RecipeLocked` no mesmo contador
(`BuildBlueprintReport.cs:44-46`). O selo ("N peças sem receita") e o aviso do modo normal precisam separar as duas —
"24 peças sem receita" pode ser 24 rugs, que nenhuma receita desbloqueia.

- [ ] **Step 1: Write the failing test**

Na seção `ac`:

```csharp
// a mesma lista que o report monta: 2 prefabs por receita + 1 que nao e peca
List<BuildBlueprintReport.SkipReason> reasons = new List<BuildBlueprintReport.SkipReason>();
BuildBlueprintReport.SkipReason r1 = new BuildBlueprintReport.SkipReason { Prefab = "darkwood_roof", Reason = BuildBlueprintSkip.RecipeLocked, Count = 16 };
BuildBlueprintReport.SkipReason r2 = new BuildBlueprintReport.SkipReason { Prefab = "sign", Reason = BuildBlueprintSkip.RecipeLocked, Count = 8 };
BuildBlueprintReport.SkipReason r3 = new BuildBlueprintReport.SkipReason { Prefab = "rug", Reason = BuildBlueprintSkip.NotAPiece, Count = 3 };
reasons.Add(r1); reasons.Add(r2); reasons.Add(r3);
Check("CountByReason(RecipeLocked) soma 24", BuildBlueprintReport.CountByReason(reasons, BuildBlueprintSkip.RecipeLocked) == 24);
Check("CountByReason(NotAPiece) soma 3", BuildBlueprintReport.CountByReason(reasons, BuildBlueprintSkip.NotAPiece) == 3);
Check("CountByReason de razao ausente e 0", BuildBlueprintReport.CountByReason(reasons, "nada") == 0);
Check("CountByReason de lista nula e 0", BuildBlueprintReport.CountByReason(null, BuildBlueprintSkip.RecipeLocked) == 0);
```

- [ ] **Step 2: Run it to see it fail**

Run: `dotnet build Companions.sln -c Release -p:SkipDeploy=true && dotnet run --project tools/ai-intents-check -c Release -- --only=(ac)`
Expected: erro de compilação (`CountByReason` não existe).

- [ ] **Step 3: Implement it**

```csharp
internal static int CountByReason(List<SkipReason> reasons, string reason)
{
    if (reasons == null || reason == null) return 0;
    int total = 0;
    foreach (SkipReason item in reasons)
    {
        if (item != null && item.Reason == reason) total += item.Count;
    }
    return total;
}
```

**Nota:** `SkipReason` é `internal sealed class` com campos `internal` — o harness já acessa tipos `internal` do mod
(seção `h`), então não é preciso mudar visibilidade.

- [ ] **Step 4: Run the harness**

Run: `dotnet run --project tools/ai-intents-check -c Release -- --only=(ac)`
Expected: PASS; total **1260 PASS** (1256 + 4).

- [ ] **Step 5: Commit**

```bash
git add src/Companions/AI/BuildBlueprintReport.cs tools/ai-intents-check/Program.cs
git commit -m "feat(build): a contagem por razao de pulo, para o painel nao somar receita com prefab inexistente"
```

---

### Task A5: controle do modo no painel

**Files:**
- Modify: `src/Companions/UI/CompanionBuildPanel.cs` (`BuildBlueprintsBlock` 647, `RefreshBlueprintDetail`)

**Interfaces:**
- Consumes: `ModConfig.BuildEasyBuild` (A2), `BuildBlueprintReport.CountByReason` (A4), `_bpReport` (campo já existente)
- Produces: nenhuma API nova.

- [ ] **Step 1: Add the toggle**

No bloco das blueprints, um **botão de alternância** (não um rótulo): `Modo fácil: ligado|desligado`. O clique grava
direto em `ModConfig.BuildEasyBuild.Value` — é o padrão dos painéis (`CompanionAiPanel.cs:1889` e seguintes fazem
`ModConfig.X.Value = v`) e **já persiste o arquivo**: o `SaveOnConfigSet` desligado em `ModConfig.Init` volta ao valor
original ao fim do boot (`ModConfig.cs:779-795`, comentário medido: *"the flag goes back to what it was, so the panels
(which write entry.Value) keep saving the file on the spot"*). Não chame `Save()` à mão.

Chaves de tradução novas: `hc_ui_build_easy_mode`, `hc_ui_build_easy_on`, `hc_ui_build_easy_off` nos 4 JSONs — o
painel já localiza os outros botões por `ModLocalization` (`hc_ui_build_btn_join` / `hc_ui_build_btn_leave`).

- [ ] **Step 2: The seal — count WITHOUT the mode**

Com o modo ligado e um arquivo selecionado, um rótulo permanente:

```
modo fácil: 24 peça(s) sem receita nesta blueprint
```

O número vem de `BuildBlueprintReport.CountByReason(_bpReport.SkippedReasons, BuildBlueprintSkip.RecipeLocked)`.
**Não use `_bpReport.Skipped`** — com o modo ligado ele já respeita o fácil e a receita não entra na conta, então o
selo diria zero (Review Focus 3).

Sem arquivo selecionado, o rótulo diz só `modo fácil: ligado`.

- [ ] **Step 3: The warning — NORMAL mode, both reasons named**

Com o modo **desligado** e `_bpReport.Skipped > 0`:

```
faltam 24 peça(s) sem receita: 16 darkwood_roof_45, 8 sign
e 3 linha(s) que não são peça neste jogo: rug
```

As duas linhas só aparecem se o total daquela razão for > 0 (Review Focus 4). Os nomes vêm de `SkippedReasons`
agrupados, com `Count`.

- [ ] **Step 4: Build**

Run: `dotnet build Companions.sln -c Release -p:SkipDeploy=true`
Expected: 0 erros, 0 avisos.

- [ ] **Step 5: Commit**

```bash
git add src/Companions/UI/CompanionBuildPanel.cs Translations/
git commit -m "feat(panel): modo facil com interruptor, selo contado sem o modo, e aviso com as duas razoes"
```

---

### Task A6: a marca do modo no relatório do run

**Files:**
- Modify: `src/Companions/Jobs/MHR_Build.cs` (a linha de log de `:2612`)
- Modify: `tools/build_run_report.py`
- Test: `tools/test_build_run_report.py`

**Interfaces:**
- Consumes: `ModConfig.BuildEasyBuild` (A2)
- Produces: a linha de log `- BlueprintFree is on (no material) - EasyBuild is on` e o campo `modo facil: on|off|?` no
  relatório.

**Por que existe:** hoje só `BlueprintFree` é logado. O relatório **não pode** inferir modo fácil dela — são configs
independentes (Review Focus 2).

- [ ] **Step 1: Write the failing test**

Em `tools/test_build_run_report.py`, um caso com log sintético contendo a linha de `:2612` com e sem o sufixo do modo:

```python
def test_easy_mode_line(self):
    log = "...: blueprint X: 4 pieces near (...) - BlueprintFree is on (no material) - EasyBuild is on\n"
    self.assertEqual(report.easy_mode(log), "on")

    log = "...: blueprint X: 4 pieces near (...) - BlueprintFree is on (no material)\n"
    self.assertEqual(report.easy_mode(log), "off")

    self.assertEqual(report.easy_mode("nada"), "?")
```

- [ ] **Step 2: Run it to see it fail**

Run: `python tools/test_build_run_report.py`
Expected: FAIL — `easy_mode` não existe.

- [ ] **Step 3: Extend the log line, then the parser**

Na linha de `MHR_Build.cs:2612`, acrescentar o sufixo quando o modo estiver ligado:

```csharp
+ (free ? " - BlueprintFree is on (no material)" : string.Empty)
+ ((ModConfig.BuildEasyBuild != null && ModConfig.BuildEasyBuild.Value) ? " - EasyBuild is on" : string.Empty)
```

Em `build_run_report.py`, `easy_mode(text)` devolve `"on"` com o sufixo, `"off"` com a linha do `BlueprintFree` sem o
sufixo, e `"?"` quando não há nem a linha.

- [ ] **Step 4: Run the tests**

Run: `python tools/test_build_run_report.py`
Expected: `Ran 5 tests ... OK`

- [ ] **Step 5: Commit**

```bash
git add src/Companions/Jobs/MHR_Build.cs tools/build_run_report.py tools/test_build_run_report.py
git commit -m "feat(report): o run diz em que modo foi feito, sem inferir de outra config"
```

---

## Fechamento do ciclo

- [ ] `dotnet build Companions.sln -c Release` → 0/0
- [ ] `dotnet run --project tools/ai-intents-check -c Release` → **1260 PASS / 0 FAIL**
- [ ] `python tools/audit_translations.py --check-config` → 0 lacunas
- [ ] `python tools/gen_config_doc.py --check ...` → 369 settings, up to date
- [ ] `python tools/test_build_run_report.py` → 5 OK
- [ ] `powershell -File tools/release-check.ps1` → 9/9
- [ ] **Run em jogo, modo fácil DESLIGADO**, `CommHomeBasicBP`: o baseline não pode mudar (985/985)
- [ ] **Run em jogo, modo fácil LIGADO**, `hs_ashlands_madzobuild_coffeetable` (antes `refused`, 4 peças de receita travada): tem de subir
- [ ] Conferir o painel: interruptor liga/desliga, selo com número > 0, aviso nomeando as razões
- [ ] `AGENTS.md`: números medidos + a opção `EasyBuild` documentada
- [ ] Merge na `main`, apagar a branch
