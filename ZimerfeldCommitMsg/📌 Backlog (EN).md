---
tipo: backlog
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-07
---

# 📌 Backlog

> 🇧🇷 Português → [[📌 Backlog]] · 🇪🇸 Español → [[📌 Backlog (ES)]]

> [!tip] Start here
> Project resumption point. When coming back in another session, read the [[🏠 Home (EN)|Home]] and this backlog.

## 📍 Current state
- **Version:** `1.0.94` (`major.minor.BUILD`, build auto-incremented by `build.ps1`).
- **Tests:** **xUnit** suite in `tests\GitExtensions.ZimerfeldCommitMsg.Tests\` — comment extraction, concept derivation, `.zimerfeldcommitmsg.json`, EN→PT translation.
- **Vault:** restructured to the **"Cofre de Neurônios v2"** standard (emoji + §3 frontmatter + EN pairs + priority `sortspec`).

## ✅ Next steps

### 🔧 Build maintenance (derived from [[🛠️ build.ps1 (EN)|build.ps1]])
- [ ] **Update section 2b of `build.ps1`** to the new v2 vault paths — the 4 stamped notes changed folder/name:
  - `💼 Negócio\📦 GitExtensions.ZimerfeldCommitMsg.md`
  - `📚 Conhecimento\📖 README — Instalação, Uso e Build.md`
  - `🧩 Sistemas\🏷️ Versionamento.md`
  - `🧩 Sistemas\🔭 Visão Geral.md`
- [ ] Consider also stamping the **`(EN)` pairs** of those notes in section 2b.

### 🧪 Quality
- [ ] Keep the xUnit suite green on every change; add cases as new comment/translation syntaxes appear.

## 🧊 Ideas / later
- [ ] (nothing pending recorded — capture in [[📥 Inbox (EN)|Inbox]] when it comes up)

## ✅ Recently done
- [x] **Landing-page fix — PT title/subtitle line break** — 2026-07-07: the landing page (`index.html`, served at **commitmsg.zimerfeld.com** via GitHub Pages) shares an i18n template with the rule `html[data-lang="pt"] .lang-pt{display:inline}`, which forced **every** Portuguese element to `inline` — including `h2`/`h3` — making the title/subtitle collapse into the following text when the site opens in PT (EN was fine, since `h2`/`h3` are `block` by default). **1-line CSS fix:** `html[data-lang="pt"] h2.lang-pt,html[data-lang="pt"] h3.lang-pt{display:block}` — restores the break only on PT titles/subtitles, with no effect on EN. Shipped via GitFlow as a **hotfix** (`hotfix/pt-heading-break` → `main`, with a back-merge into `develop`, which was behind `main`) + tag **`202607071915pt-heading-break`**; `CNAME` preserved; deploy verified live.

## 🔗 Links
- [[🏠 Home (EN)|Home]]
- [[🔑 Fatos-Chave (EN)|Key Facts]]
- [[🛠️ build.ps1 (EN)|build.ps1]]
