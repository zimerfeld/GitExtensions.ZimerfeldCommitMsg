---
tipo: fluxo
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [fluxo, i18n, dropdown, auto-refresh, etapa3]
---

# 🌐 Fluxo: Idioma e auto-refresh

Como o usuário escolhe o idioma e como a mensagem se mantém atualizada enquanto o diálogo está aberto.

## 🗣️ Duas formas de escolher o idioma

**1. Dropdown de templates da tela de commit** — três itens (escolha rápida por commit):
```text
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
```
**2. Configurações → Plugins → ZimerfeldCommitMsg** — o seletor **"Idioma da mensagem / Message language"** define o **padrão** (usado pelo menu Plugins e pelo auto-refresh quando nenhum item do dropdown foi escolhido).

| Opção | Comportamento |
|---|---|
| `Automático/Automatic` | **Padrão.** Detecta pelo SO (`pt-*` → português; outro → inglês) |
| `Português/Portuguese` | Força pt-BR |
| `Inglês/English` | Força inglês |

## 🔎 Como a escolha do dropdown é detectada

```
Host abre o dropdown de templates
        │  invoca o Func de geração de TODOS os itens, em sequência
        │  (NÃO há callback de clique — o host materializa o texto de cada item)
        ▼
GenerateForTemplate(forced)  → gera no idioma do item + RememberTemplateMessage(msg → idioma)
        │
        ▼
Usuário clica um item  → host aplica o texto materializado na caixa (ReplaceMessage)
        │
        ▼
TextChanged da caixa  → DetectTemplateSelection: a caixa virou exatamente um texto conhecido?
        │  se sim → fixa _sessionLanguage no idioma daquele item
        ▼
Auto-refresh passa a usar EffectiveLanguage() = _sessionLanguage (prioridade sobre setting/SO)
```

> [!note] Por que a detecção é indireta
> A API de template do host **não** dá callback de clique: ele chama o `Func` de **todos** os itens ao **abrir** o dropdown e, no clique, só aplica o texto já materializado. Se o plugin fixasse o idioma dentro do `Func`, ele rodaria para todos os itens e fixaria sempre o último. Por isso o plugin **registra cada texto gerado** (msg → idioma) e reconhece a escolha observando a caixa (`TextChanged`). Ver [[🔌 Integração via template dropdown e Application.Idle]].

## 🔄 Auto-refresh ao stage/unstage

Com o diálogo aberto, quando arquivos entram/saem do stage, o próximo `Application.Idle` regenera a mensagem no idioma efetivo (`EffectiveLanguage()`), desde que a caixa ainda seja "nossa" (não editada manualmente). Ver [[🔍 1 - Detecção do diálogo de commit]].

## 🗣️ Idioma efetivo

```
EffectiveLanguage() = _sessionLanguage ?? CurrentLanguage()
CurrentLanguage()   = setting (Automático/PT/EN) → MessageLanguageResolver.Resolve
                      "Automático" → FromCulture(CultureInfo.CurrentUICulture)
```

## 🔗 Ligações

- [[🔍 1 - Detecção do diálogo de commit]]
- [[⚙️ 2 - Geração da mensagem]]
- [[🌐 Localization]]
- [[🔌 ZimerfeldCommitMsgPlugin]]
