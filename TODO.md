# TODO / Roadmap

Lista de melhorias priorizadas. Marque `[x]` conforme for concluindo.

Cada item traz **Onde** (arquivos e trechos que provavelmente precisam mudar) e **Como** (abordagem sugerida). As linhas citadas referem-se ao código atual e podem mudar.

> **Convenção do projeto:** cada aba é um `UserControl` que implementa `IGameControl` ([Utils/IGameControl.cs](CP2077SaveEditor/Utils/IGameControl.cs)), é registrado em `MainForm()` via `RegisterControl(...)` e escuta `PropertyChanged` de `ActiveSaveFile` para se inicializar. O acesso ao save passa por [SaveFileHelper.cs](CP2077SaveEditor/Utils/SaveFileHelper.cs). Os dados estáticos do jogo ficam em `CP2077SaveEditor/Resources/*.json`, gerados pelo [ResourceGenerator](ResourceGenerator/Program.cs).

---

## ✅ Concluído

- [x] Mover itens entre inventário e cofre (`MoveItemsToStash` / `MoveItemsToInventory` em `InventoryControl.cs`)
- [x] Backups com data e hora a cada save (`sav.yyyyMMdd-HHmmss.bak`, mantém os 5 últimos — `MaxBackups` em `MainForm.cs`) + `sav.old` original preservado
- [x] Gravação segura: grava em `sav.dat.tmp`, relê com `CyberpunkSaveReader` (`VerifySaveFile`) e só então substitui o `sav.dat`
- [x] Aviso de alterações não salvas ao fechar ou abrir outro save (`ConfirmDiscardChanges`, compara o SHA-256 da serialização com `_savedStateHash`)
- [x] Botão **Save** + atalho `Ctrl+S` (`QuickSave`); o antigo botão virou **Save As...**
- [x] Copiar → Colar item (`CopyItem` / `PasteItem` / `CloneItemData` / `CloneItemStats` em `InventoryControl.cs` + fix em `CreateStatData`)
- [x] "Clear All Item Flags" com confirmação e opção de limpar apenas itens selecionados (`clearQuestFlagsButton_Click` em `InventoryControl.cs`)
- [x] Remover código morto do Debloat (`InventoryControl.cs` e `InventoryControl.Designer.cs`)
- [x] Corrigir falsos avisos de "unsaved changes" ao abrir abas ou `ItemDetails` (`_isLoading` em `PlayerStatsControl.cs`, `ItemDetails.cs`, `InventoryControl.cs` e `ModNodeDetails.cs`)
- [x] Suporte a High DPI (`Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)` em `Program.cs`)
- [x] Corrigir build quebrada (`using WolvenKit.RED4.Save;` faltando em `InventoryControl.cs`, usado por `CloneItemStats`)
- [x] Saves de versão mais nova que a suportada abrem **somente leitura** (`_isReadOnly` / `MaxSupportedGameVersion` em `MainForm.cs`). Evita gravar um save 2.5 com o WolvenKit antigo, que apaga os stats
- [x] Veículos: lista regenerada do TweakDB 2.5 (+ Muramasa), aba com busca, **Unlock all** / **Lock all** (atuam sobre o resultado da busca), seleção múltipla, código limpo (`VehiclesControl.cs`)
- [x] **Tema escuro** ([Theme.cs](CP2077SaveEditor/Utils/Theme.cs)): segue o Windows (`config.json` `"Theme"`: 1 claro, 2 escuro). Usa o modo escuro nativo do WinForms (.NET 10) e corrige o que ele não cobre: cores fixas dos designers, `ModernButton`, cabeçalho e seleção das ListViews (owner-draw só nessas partes), linhas de grade; diálogos são tematizados ao abrir (`Application.Idle`). Validado por print em Inventory, Perks, Cyberware e Vehicles; o modo claro continua igual ao original. `config.json` lido por `AppConfig`
- [x] `ResourceGenerator` aceita `[gameDir] [vehicles|items|mods|modifiergroups ...]`, compila com o WolvenKit atual (`new HashService()` registra os nomes do TweakDB), não quebra com records sem nome
- [x] Mensagens de erro ao carregar: versão do jogo no erro e no `error.txt`, dicas para "save mais novo que o editor" e "dados de mod desconhecidos", e o editor não fica habilitado com save carregado pela metade (`GetLoadErrorHint` em `MainForm.cs`)
- [x] "Add item" não quebra com itens sem nome conhecido (`AddItem.RefreshItems`)
- [x] **Suporte ao patch 2.5:** WolvenKit atualizado para o `main` (`65ea74b01`, com `update_2.5`), projetos em .NET 10 (`WFO1000` suprimido no `.csproj`). O round-trip do save 2.5 preserva o `StatsSystem` e o `ScriptableSystemsContainer`. O `PersistencySystem2` muda de tamanho, mas as 156 mil entradas ficam idênticas
- [x] **Stats no formato 2.5** ([OptimizedStatsHelper.cs](CP2077SaveEditor/Utils/OptimizedStatsHelper.cs)): expõe `gameSavedStatsDataOptimized` como `gameSavedStatsData`. O `FlushStats()` é chamado em `SerializeSave` e faz copy-on-write das fatias compartilhadas do `sharedDataBuffer`. Testado: materializar as 1.746 entradas e gravar dá um save idêntico; editar um item não afeta os que dividiam a fatia; clonar stats funciona; o flush é idempotente
- [x] `ItemClasses.json` regenerado do 2.5 (+191 itens, −27 loot tables obsoletas)
- [x] **Tier 5++**: botão "Make Tier 5++" no `ItemDetails` e no menu de contexto do inventário (vários itens de uma vez). `SetMaxTier` em `SaveFileHelper` usa o teto dos stats no TweakDB (`BaseStats.Quality` max 4, `BaseStats.IsItemPlus` max 2): soma modificadores aditivos até o total ser ≥ 4 / ≥ 2, então funciona para armas saqueadas e icônicas sem conhecer a qualidade base. Botão morto "Infuse Legendary Components" removido
- [x] **Add item** com busca (vários termos), filtro por tipo, lista, contador, duplo clique/Enter para adicionar e janela redimensionável
- [x] Busca do inventário: prefixos `quantity:`/`desc:` filtravam a coluna errada (desde que entrou a coluna Slot); adicionado `slot:` e os prefixos aparecem no placeholder. A ordenação por coluna já existia (clique no cabeçalho)
- [x] Barra de progresso (marquee) durante carregar/salvar/verificar e nome do save no título da janela
- [x] **Nomes do jogo** no inventário e no "Add item" (PT-BR se o Windows estiver em português, senão inglês). `ResourceGenerator ... names` gera `Resources/DisplayNames.json` a partir do `DisplayName` (LocKey) dos records de itens/veículos + `onscreens*.json` de `lang_en_text`/`lang_pt_text` (base e ep1). A coluna ID continua com o identificador técnico; a busca do "Add item" acha pelos dois
- [x] Aba de veículos com coluna "Name" (nome do jogo) e busca pelo nome
- [x] Removidos `items.bin` (30 MB, sem uso desde 2021) e o recurso `Mods.json` (não era usado; `ResourceGenerator ... mods` ainda gera o arquivo se precisar)
- [x] CI: `.github/workflows/build.yml` compila a cada push/PR (publish x64, zip como artefato) e anexa o zip à release em tags `v*`. README menciona o .NET 10 Desktop Runtime
- [x] Botão "Crafting components..." (Quick Actions do inventário): os 9 componentes de item/quickhack com nome do jogo, "Set all to 99999"; zero remove o item, valor > 0 cria se não existir ([CraftingComponentsDialog.cs](CP2077SaveEditor/Views/CraftingComponentsDialog.cs))
- [x] Logs: erros de carregar/salvar e exceções não tratadas vão para `logs\yyyy-MM-dd.log` ao lado do executável ([Logger.cs](CP2077SaveEditor/Utils/Logger.cs)); a mensagem mostra o caminho. Tratador global em `Program.cs` evita fechar o app sem registro
- [x] `Form2` renomeado para `MainForm` (classe, arquivos e handlers)
- [x] `ResourceGenerator "<pasta do jogo>" all --out CP2077SaveEditor/Resources` regenera todos os JSONs usados pelo editor (testado: sai idêntico ao commitado). README reescrito com as funcionalidades atuais e instruções
- [x] Limpeza: removidos `AppearanceHelper.cs` (~1900 linhas sem uso; `AppearanceValueLists`/`AppearanceGender` foram para `Utils/AppearanceValueLists.cs`) e `AppearancePreset.cs`; opção morta `EnablePSData`; `config.json` agora só tem `EnableWIPFeatures` e é procurado na pasta atual ou ao lado do .exe
- [x] Aba **Cyberware** ([CyberwareControl.cs](CP2077SaveEditor/Views/Controls/CyberwareControl.cs) + [CyberwareHelper.cs](CP2077SaveEditor/Utils/CyberwareHelper.cs)): slots por área com o implante equipado; equipar qualquer cyberware da área (do inventário ou criando o item), desequipar; **capacidade extra** = modificador `Humanity` aditivo salvo nos stats do V, somado à capacidade que o jogo calcula (21 + 3×nível + skill Tech + perk + shards, ver `Character.PlayerCyberwareSystem` no TweakDB). `ResourceGenerator ... cyberware` gera `Resources/Cyberware.json` (1582 itens → área). "Equip in Slot" do inventário agora grava o ItemID completo (seed + contador), como o jogo
- [x] Aba **Perks** ([PerksControl.cs](CP2077SaveEditor/Views/Controls/PerksControl.cs) + [PerkHelper.cs](CP2077SaveEditor/Utils/PerkHelper.cs)): árvores 2.0 por atributo + Relic, nomes no idioma do Windows, tier, nível atual/máximo; duplo clique alterna, "Set level", "Max all", "Remove all"; opção de cobrar/devolver perk points (Relic points para Espionage). `ResourceGenerator ... perks` gera `Resources/Perks.json` (194 perks). Requisitos entre perks não são checados
- [x] Tela inicial "Saves" ([SaveBrowserControl.cs](CP2077SaveEditor/Views/Controls/SaveBrowserControl.cs)): lista as pastas de save com miniatura, nível, life path, gênero, tempo de jogo, patch, data e "modded" (do `metadata.9.json`), busca e filtro de autosaves; duplo clique abre (com aviso de alterações não salvas) e leva para a aba Player Stats
- [x] Testes de round-trip (`CP2077SaveEditor.Tests`, xUnit v3): save idêntico ao regravar, ler todos os stats não altera nada, edição não vaza para itens que dividem a fatia, Tier 5++ idempotente, clone independente, stats do jogador. Usam `CP2077_TEST_SAVE`; sem ela são pulados (CI). `SaveFileHelper.Serialize()` e `CloneStats()` agora são compartilhados pela UI e pelos testes
- [x] Correções após os testes de 28/10: crash da lista virtual ao encolher com itens selecionados (`TypeExtensions.RetrieveDefaultVirtualItem`); duplo clique no inventário abre a linha clicada (antes, o primeiro item selecionado); "Make Tier 5++" avisa quando o item já está no máximo; tooltips do `ModernButton` (label filho engolia o hover); CI sem zip duplo e com actions atualizadas
- [x] Correções após os testes de 29/10 (tudo in-game passou: capacidade extra, implantes acima do limite, perks, clone 5++, persistência após salvar pelo jogo): tooltips do `ModernButton` de novo (o label só era criado em `OnCreateControl`, depois do construtor da tela; agora no construtor do botão); `ModernValuePicker` desenhava em preto fixo (gênero/voz ilegíveis no escuro), agora segue `ForeColor`; `TabPage` com `UseVisualStyleBackColor` ficava branca no escuro; lista virtual rolada que encolhia (filtro) deixava linhas em branco no topo (`SetVirtualItems` volta ao topo antes de encolher)
- [x] Aba Extras: "Unlock Fast Travel" só adiciona os pontos que faltam (antes apagava todos e recriava a partir do JSON, perdendo pontos que o JSON não conhecesse); confirmação/feedback e tooltips nas três ações
- [x] Ações em massa no menu do inventário: "Set Quantity..." (itens empilháveis) e "Make Tier 5++". Para mover tudo de um tipo: buscar `type:Weapon` e usar "Move All Listed Items to Stash"
- [x] Teste automatizado pelo `MainForm` real num save 2.5: carrega editável, sem aviso falso de alterações, Tier 5++ + salvar + verificar OK
- [x] Destaque visual no botão ativo da barra lateral (`ModernButton.IsSelected`, `ScrollMenuControl.SelectButton` e `SwitchPanel` em `MainForm.cs`) + rolagem com o mouse sobre os botões do menu
- [x] Versão no `.csproj` (`<Version>0.29.0</Version>`, mostrada na barra de status) e checagem de atualização ao abrir ([UpdateChecker.cs](CP2077SaveEditor/Utils/UpdateChecker.cs)): consulta a última release do fork no GitHub e, se houver uma mais nova, a versão vira um link para a página de download. Desligável com `"CheckForUpdates": 0` no `config.json`
- [x] Indicador de alterações não salvas: "*" após o nome do save (título e barra lateral). As ações de edição chamam `ChangeTracker.MarkChanged()` ([ChangeTracker.cs](CP2077SaveEditor/Utils/ChangeTracker.cs)); é zerado ao carregar/salvar e quando a checagem por hash conclui que nada mudou. **Ao criar uma ação nova que altera o save, chamar `ChangeTracker.MarkChanged()`**
- [x] **GUI moderna (taste-skill `data-terminal`):** paleta escura neutra (`#111113`–`#2B2B33`), destaque primário amarelo Cyberpunk (`#FCEE0A`) e secundário ciano (`#00F0FF`) em `Theme.cs` / `ModernButton` / `ModernValuePicker` / `GroupBox` / cabeçalhos de `ListView`; tipografia `Bahnschrift` + `Cascadia Mono` / `Consolas` tabular para valores numéricos; `PlayerStatsControl` reorganizado em `TableLayoutPanel` + `GroupBox` redimensionáveis; barra lateral alargada (176 px) com altura compacta (44 px); botões e barras de busca redimensionados para High DPI em `QuestFactsControl` e `AppearanceControl`; tooltips explicativos (`Theme.SetToolTip`) em todas as abas

---

## 🔴 Prioridade alta — correções

### Testar o patch 2.5 no jogo — feito (29/10/2026)
- [x] Editar um save 2.5 (cópia!) no app: mudar a Quality de uma arma, clonar um item, mudar dinheiro/atributos. Carregar no jogo e conferir que o item editado mudou, que **outras armas do mesmo modelo não mudaram** e que o save carrega sem erros.
- [x] "Make Tier 5++" numa arma saqueada (ex.: Pulsar) e numa icônica (ex.: Warden): conferir no jogo que ficam Tier 5++ e que o dano subiu. As icônicas usam curvas próprias e são as mais incertas.
- [x] Repetir com um save antigo (testado com um 2.2), para garantir que o caminho antigo (`Keys`/`Values`) continua funcionando.
- **Riscos conhecidos:**
  - O significado das flags de `gameBufferSlice.Combined` (bits 30–31) foi deduzido do save: 0 = modificadores de item, 1 = modificadores de entidade (o jogador), 2 = modifier groups.
  - Quando o `sharedDataBuffer` enche, ele cresce para a próxima potência de 2 (o jogo usou 32768). Isso ainda não foi testado no jogo: o save atual tem ~10 KB livres.
  - Fatias que deixam de ser usadas continuam no buffer, e cada edição salva acrescenta uma fatia nova (sem compactação).

### Recursos estáticos — o que sobrou
- `Vehicles.json` ✅ e `ModifierGroups.json` (regenerado, sem mudanças) ✅.
- `ItemClasses.json` ✅ (regenerado com o WolvenKit novo; todos os nomes resolvem).
- **`FastTravel.json`:** conferido contra o TweakDB do 2.5 (29/10/2026): cobre 232 dos 245 `gamedataFastTravelPoint_Record`, incluindo Dogtown. Os 13 restantes são mapas de teste (`gym_tech_*`), nomes antigos sem uso (`WatsonMain`, `WatsonSubway`...) e `wat_lch_dataterm_08`. Um gerador não ajudaria: o `MarkerRef` vem do mundo, não do TweakDB. Para conferir de novo, comparar os pontos de um save 100% com o JSON. **`Facts.json`** provavelmente veio de um dump externo; documentar a origem.
- **`AppearanceValues.json`:** `GenerateAppearanceValues` lê `basegame_1_engine.archive`; checar se ainda funciona.

### Testar na interface — feito
- [x] Aba de veículos: busca, Unlock all/Lock all com e sem filtro, double-click com vários selecionados.
- [x] Editor de stats (`StatsControl`) em item de save 2.5: listar, adicionar e remover modificadores. O botão de criar "forced modifiers" fica desabilitado (o formato 2.5 não tem esse campo).
- [x] A trava de somente leitura (conferida no código; falta um save de patch > 2.5 para testar de verdade) só dispara para saves de versão acima do 2.5 (`gameGameVersion.Current` = 2500).

---

## 🟠 Prioridade média — usabilidade

### Inventário

#### Tiers intermediários (1–5) e "+" exatos — inviável por enquanto
- O tier das armas/roupas aleatórias vem de curvas sobre `RandomCurveInput`, sorteado pelo jogo a partir da seed; não dá para calcular o tier base fora do jogo (ver "Cor por raridade").
- O jogo só limita o valor final (Quality 0–4, IsItemPlus 0–2). Sem saber a base, um aditivo só garante os extremos: +muito = Tier 5++ (já existe), −muito = Tier 1. Um tier do meio exigiria conhecer a base.


#### Cor por raridade — falta nas armas e roupas
- Feito para os itens de raridade fixa (mods, programas, cyberware, consumíveis...): `ItemClasses.json` tem `Quality` e o nome é pintado em `RefreshInventory` (`Theme.GetQualityColor`).
- **Falta:** armas e roupas cujo record modifica `BaseStats.Quality` (ficam sem `Quality` no JSON). O tier delas vem de curvas sobre `RandomCurveInput`, que o jogo sorteia a partir da seed, então não dá para calcular fora do jogo. Única saída plausível: ler um valor que o próprio jogo já calculou e salvou, se existir (procurar no save algo como cache de stats / `MaxQualityWhenLooted`).

### Personagem

#### Respec — não fazer (o jogo já tem)
- Desde o 2.0 o jogo devolve perks de graça no próprio menu e oferece um reset de atributos; a aba Player Stats já edita atributos e pontos (`DevPoints`) diretamente. Só vale fazer se alguém pedir.
- **Onde, se for fazer:** `PlayerDevelopmentData.AttributesData[].UnlockedPerks` (perks 2.0) e `DevPoints` (`Attribute`, `Primary`, `Espionage`) via `GetPlayerDevelopmentData()`.

#### Aba de aparência — avaliado, não vale agora
- Hoje: gênero, voz, presets e o editor avançado (grupos brutos). Os campos `HairStyle`/`HairColor`/`EyeColor` de `AppearanceHelper2` estão escondidos porque `SetValue` nunca foi implementado.
- No 2.x o penteado é o caminho de um `.app` (`hairs` → `hh_025_pwa__hairs_081.app`, com uma entrada `FPP_hairs` correspondente), a cor do cabelo é o `Definition` (`07_blonde_golden`) e a cor dos olhos usa nomes como `he_000_pwa__basehead__15_gradient_light_blue`, diferentes de `AppearanceValueLists` (lista da era 1.x).
- Implementar exigiria mapear os caminhos e testar no jogo (risco de V sem cabelo). O espelho do jogo (2.0+) já muda cabelo e olhos, então só vale se alguém pedir.

### Mundo
- [x] **Gigs / NCPD — inviável só com facts:** não existe um fact de "gig concluída". Os ~690 `sts_*` são etapas internas de cada gig (`..._combat_start`, `..._chase_spawned`) e o nível de reputação do fixer (`sts_<fixer>_tier_1..4`); os `ma_*` são as atividades da NCPD. A conclusão fica no diário de quests (journal), que o editor não edita. Mudar só os facts deixaria gigs pela metade.
- [x] **Fast travel:** "Unlock all" em `ExtrasControl.btn_UnlockAll_Click`; `FastTravel.json` conferido para o 2.5 (ver "Recursos estáticos").
- [x] **Guarda-roupa — avaliado, não vale agora:** o inventário `0x7901DE03D136A5AF` ("V's Wardrobe") já aparece e pode ser editado em `InventoryControl`, e `ModSupport/ExtraWardrobeSlots` já tem suporte à limpeza de blacklist em `ExtrasControl`. Os conjuntos do guarda-roupa nativo (`WardrobeSystem_ClothingSets`) guardam 6 blocos binários de `0x8E` bytes com campos `Unknown1..Unknown6` não mapeados no parser do WolvenKit, e o `WardrobeSystem` exige o `AppearanceName` interno de cada `.ent` (ausente em `ItemClasses.json`). O próprio jogo já permite montar e trocar os conjuntos gratuitamente.

---

## 🟡 GUI moderna

#### Visual Cyberpunk (amarelo/ciano) — feito
- Direção estética `data-terminal` (`Leonxlnx/taste-skill`, seed `1440651410`): superfícies escuras neutras (`#111113` / `#17171A` / `#1F1F24`), destaque primário amarelo (`Theme.Accent` `#FCEE0A`), destaque secundário ciano (`Theme.CyanAccent` `#00F0FF` no tema escuro / `#007A8C` no claro) nos títulos de `GroupBox`, cabeçalhos de colunas, `ModernValuePicker` e hover de `ModernButton`.
- Tipografia: `Bahnschrift` (fallback `Segoe UI`) para interface e `Cascadia Mono` (fallback `Consolas`) para campos numéricos (`NumericUpDown`).

#### DPI alto e redimensionamento — feito
- `Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)` em [Program.cs](CP2077SaveEditor/Program.cs); `MinimumSize = 960x580` em `MainForm`.
- `PlayerStatsControl` refeito com `TableLayoutPanel` e `GroupBox` responsivos (substituindo as coordenadas fixas herdadas da antiga imagem de fundo).
- Botões de topo redimensionados para `28px` em `AppearanceControl` e `QuestFactsControl`; barra lateral alargada para `176px` com `Anchor` horizontal.

#### Barra lateral com ícones — feito
- Destaque da página ativa com barra amarela + ícone colorido (`IGameControl.Icon`, glifo de Segoe Fluent Icons / Segoe MDL2 Assets em `ModernButton.IconGlyph`), alinhamento à esquerda sem corte de texto e rolagem automática ao selecionar aba (`ScrollMenuControl.SelectButton`).

#### Desfazer/refazer — feito
- `Ctrl+Z` / `Ctrl+Y` ([UndoHistory.cs](CP2077SaveEditor/Utils/UndoHistory.cs) + `MainForm.CaptureUndoPoint`/`RestoreState`). Um ponto é o save inteiro serializado (~4 MB), gravado 1,5 s depois que as edições param (`ChangeTracker.Edited`), na thread da UI (~1,3 s) e nunca com diálogo aberto. Desfazer relê o estado e as páginas reinicializam como num carregamento (~6 s). `"UndoSteps"` no `config.json`.
- **Melhoria possível:** desfazer é lento porque relê o save inteiro. Guardar e restaurar só os nós alterados (inventário, stats...) seria bem mais rápido, mas exige saber quais nós cada ação muda.

#### Tooltips e tradução — tooltips feitos
- **Feito (Tooltips):** `Theme.SetToolTip` aplicado nas ações principais e destrutivas (`MainForm`, `PlayerStatsControl`, `AppearanceControl`, `InventoryControl`, `CyberwareControl`, `PerksControl`, `VehiclesControl`, `QuestFactsControl`, `ExtrasControl`).
- **Falta (Tradução):** mover os textos para `Properties/Resources.resx` + `Resources.pt-BR.resx` e trocar as strings literais por `Resources.Xxx`.

#### WPF/Avalonia?
- **Recomendação:** só depois dos itens acima. A lógica está acoplada aos controles WinForms (por exemplo, o `InventoryControl` mexe direto no save). Um primeiro passo seria extrair a lógica para serviços (`InventoryService` etc.), o que já ajuda nos testes.

---

## 🔵 Infraestrutura e manutenção

- [x] **Refatorar arquivos grandes:** `InventoryControl.cs` (~975 linhas). Extraídos, com testes: cofre ([StashHelper.cs](CP2077SaveEditor/Utils/StashHelper.cs)), clone de itens (`SaveFileHelper.CloneItem`) e menu de contexto ([InventoryContextMenuBuilder.cs](CP2077SaveEditor/Views/InventoryContextMenuBuilder.cs)).
- [x] **Warnings:** o código do editor compila sem warnings; os ~850 restantes são todos do submódulo WolvenKit (não mexer lá).
