---
tipo: fluxo
tags: [fluxo, dropdown, template, commit, auto-resumo, ui]
atualizado: 2026-06-01
---

# Fluxo: Dropdown "Zimerfeld Commit Msg — Auto/PT/EN"

Passos executados quando o usuário seleciona um dos **três itens** (um por idioma) **"Zimerfeld Commit Msg — Automático/Português/Inglês"** no dropdown de templates do diálogo de commit (`FormCommit`) do GitExtensions.

É **uma das três portas de entrada** do plugin — ver também [[Stage Trigger]] (evento automático) e o `Execute()` do menu Plugins ([[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]]).

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
- `GenerateForTemplate` fixa `_sessionLanguage = forced` e instancia `CommitMessageGenerator(workingDir, forced ?? CurrentLanguage())` — o auto-refresh subsequente mantém o idioma escolhido.
- A `factory` é um **delegate preguiçoso**: não roda no registro, só quando o usuário **seleciona** a opção.
- `PluginIcon` é o ícone (PNG embutido) exibido ao lado do rótulo no dropdown e também no menu Plugins. Ver [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]].

## Sequência ao clicar

```
Usuário abre o diálogo de commit (FormCommit)
        │
        ▼
Abre o dropdown de templates de mensagem
        │   (3 itens "Zimerfeld Commit Msg — Auto/PT/EN" com ícone)
        ▼
Usuário seleciona um item "Zimerfeld Commit Msg — …"
        │
        ▼
GitExtensions invoca a factory registrada do item
        │
        ▼
GenerateForTemplate(WorkingDir, forced)
        │   fixa _sessionLanguage = forced (idioma do item)
        ▼
new CommitMessageGenerator(WorkingDir, idioma).Generate()   ← pipeline completo
        │   git diff --cached --name-status
        │   git diff --cached --no-color
        │   → "<Verbo> <descrição>" + corpo em bullets   (SEM prefixo "tipo:")
        ▼
Retorna string da mensagem
        │
        ▼
GitExtensions preenche a caixa de mensagem do FormCommit
```

## Detalhe dos passos

1. **Seleção** — o usuário escolhe a entrada no dropdown. O GitExtensions é quem dispara a factory; o plugin não intercepta o clique diretamente.
2. **Instanciação sob demanda** — cada seleção cria um `CommitMessageGenerator` **novo**, vinculado ao `WorkingDir` lido no momento do clique (sem estado entre chamadas).
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
| Quem preenche a caixa | GitExtensions | `StartCommitDialog(commitMessage:)` | O plugin (`tb.Text = msg`) |
| Validação de repo | — (host gerencia) | `IsValidGitWorkingDir()` + `MessageBox` | Ignora se `WorkingDir` vazio |
| Sem mudanças staged | Caixa recebe string vazia | `MessageBox` "Nenhuma mudança staged" | Não atualiza (`Generate()` vazio) |
| Protege texto manual | N/A (usuário pediu) | N/A (abre diálogo novo) | **Sim** — só sobrescreve vazio ou `_lastGeneratedMessage` |
| Thread | UI thread (host) | UI thread (host) | Marshalling via `_syncContext.Post()` |

## Observações

- Este caminho **não** exibe `MessageBox`: se não houver staged changes, a factory retorna `string.Empty` e o GitExtensions simplesmente não preenche nada.
- Como a factory é avaliada a cada seleção, **reselecionar** o template regenera a mensagem com o estado atual do stage.
- Os 3 templates são removidos no `Unregister()` via `RemoveCommitTemplate(label)` para cada item.

## Relacionado

- [[Geração da Mensagem]] — o que `Generate()` faz internamente
- [[Stage Trigger]] — atualização automática ao (un)stage
- [[../Arquivos-Chave/ZimerfeldCommitMsgPlugin]] — `Register` / `Execute` / ícone
- [[../Sistema/Arquitetura]]
