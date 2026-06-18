---
tipo: fluxo
tags: [fluxo, dropdown, template, commit, auto-resumo, ui]
atualizado: 2026-06-01
---

# Fluxo: Dropdown "Zimerfeld Commit Msg — Auto/PT/EN"

Passos executados quando o usuário seleciona um dos **três itens** (um por idioma) **"Zimerfeld Commit Msg — Automático/Português/Inglês"** no dropdown de templates do diálogo de commit (`FormCommit`) do GitExtensions.

É **uma das três portas de entrada** do plugin — ver também [[Stage Trigger]] (evento automático) e o `Execute()` do menu Plugins ([[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]]).

![[ScreenshotDropDown.png]]

## Registro (acontece uma vez, no `Register()`)

```csharp
private const string TemplatePrefix = "Zimerfeld Commit Msg";

// 3 itens: rótulo + idioma forçado (null = Automático)
foreach (var (label, lang) in _templateItems)   // Auto/null, PT, EN
{
    var forced = lang;
    gitUiCommands.AddCommitTemplate(
        label,                                                    // "… — Automático/Português/Inglês"
        () => GenerateForTemplate(gitUiCommands.Module.WorkingDir, forced),  // factory LAZY
        icon: PluginIcon);                                        // ícone ao lado do rótulo
}
```

- `AddCommitTemplate(label, factory, icon)` registra uma entrada **nomeada** por item; a API é **plana** (sem submenu), por isso são 3 itens, não um submenu de idioma.
- `GenerateForTemplate` instancia `CommitMessageGenerator(workingDir, forced ?? CurrentLanguage())`, registra o texto gerado (`msg → idioma`, via `RememberTemplateMessage`) e retorna a string. **Não** fixa idioma aqui — ver abaixo.
- ⚠️ A `factory` **não roda no clique**: o host (`CommitTemplateManager.RegisteredTemplates`) a invoca ao **ABRIR** o dropdown, uma vez para **cada** item (Auto/PT/EN). O clique só aplica via `ReplaceMessage` o texto já materializado, **sem** rechamar a factory nem dar callback de clique.
- **Fixação de idioma (pinning):** como a factory roda para todos os itens na abertura (fixar ali fixaria sempre o último, EN), a escolha é detectada de outro jeito: assinamos o `TextChanged` da caixa (`EnsureTextChangedHook`); quando ela vira exatamente uma das mensagens registradas (`DetectTemplateSelection`), fixamos `_sessionLanguage` no idioma daquele item. O auto-refresh então regenera fresco do stage nesse idioma (`EffectiveLanguage = _sessionLanguage ?? CurrentLanguage`). Vale enquanto o diálogo vive.
- `PluginIcon` é o ícone (PNG embutido) exibido ao lado do rótulo no dropdown e também no menu Plugins. Ver [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]].

## Sequência ao clicar

```
Usuário abre o diálogo de commit (FormCommit)
        │
        ▼
Usuário ABRE o dropdown de templates de mensagem
        │   host enumera RegisteredTemplates → invoca a factory dos 3 itens
        ▼
GenerateForTemplate(WorkingDir, forced)  ×3  (Auto, PT, EN)
        │   new CommitMessageGenerator(WorkingDir, idioma).Generate()  ← pipeline completo
        │   RememberTemplateMessage(msg → idioma)
        ▼
host materializa o texto de cada item no menu
        │
        ▼
Usuário CLICA num item "Zimerfeld Commit Msg — …"
        │   host: ReplaceMessage(textoMaterializado)  → caixa = mensagem do idioma
        ▼
TextChanged dispara → DetectTemplateSelection(caixa)
        │   caixa == mensagem registrada? → fixa _sessionLanguage = idioma do item
        ▼
auto-refresh (stage/unstage) regenera fresco do stage em EffectiveLanguage()
```

## Detalhe dos passos

1. **Abertura do menu** — o host dispara a factory dos **3 itens** ao abrir o dropdown (não no clique). O plugin não intercepta o clique diretamente; reconhece a escolha depois, pelo `TextChanged` da caixa.
2. **Instanciação sob demanda** — cada item cria um `CommitMessageGenerator` **novo**, vinculado ao `WorkingDir` lido na abertura do menu (sem estado entre chamadas).
3. **Geração** — roda o pipeline completo de [[Geração da Mensagem]]:
   - `ParseChanges` → tipos via `DetermineAllTypes`
   - comentários do diff (ranqueados + traduzidos en→pt-BR, preservando branches e tipos CC — ver [[../Decisoes/Preservação de Branches e Tipos CC]])
   - título do README staged (se houver)
   - montagem `title\n\nbody` → `<Verbo> <descrição>` + bullets (sem prefixo `tipo:`)
4. **Preenchimento** — a string retornada é colocada na caixa de mensagem **pelo próprio GitExtensions** (o plugin não manipula o `TextBox` neste caminho).

## Diferenças entre as três portas de entrada

| Aspecto | Dropdown (este fluxo) | Menu Plugins (`Execute`) | Evento (`PostRepositoryChanged`) |
|---|---|---|---|
| Disparo | Usuário seleciona o template | Usuário clica em Plugins → ZimerfeldCommitMsg | Stage/unstage de arquivos |
| Quem preenche a caixa | GitExtensions (plugin detecta a escolha via `TextChanged` e fixa o idioma) | `StartCommitDialog(commitMessage:)` | O plugin (`tb.Text = msg`) |
| Validação de repo | — (host gerencia) | `IsValidGitWorkingDir()` + `MessageBox` | Ignora se `WorkingDir` vazio |
| Sem mudanças staged | Caixa recebe string vazia | `MessageBox` "Nenhuma mudança staged" | Não atualiza (`Generate()` vazio) |
| Protege texto manual | N/A (usuário pediu) | N/A (abre diálogo novo) | **Sim** — só sobrescreve vazio ou `_lastGeneratedMessage` |
| Thread | UI thread (host) | UI thread (host) | Marshalling via `_syncContext.Post()` |

## Observações

- Este caminho **não** exibe `MessageBox`: se não houver staged changes, a factory retorna `string.Empty` e o GitExtensions simplesmente não preenche nada.
- Como a factory é avaliada a cada **abertura** do dropdown, reabrir o menu regenera as 3 mensagens com o estado atual do stage (sempre frescas).
- Os 3 templates são removidos no `Unregister()` via `RemoveCommitTemplate(label)` para cada item.

## Relacionado

- [[Geração da Mensagem]] — o que `Generate()` faz internamente
- [[Stage Trigger]] — atualização automática ao (un)stage
- [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]] — `Register` / `Execute` / ícone
- [[../Sistema/Arquitetura]]
