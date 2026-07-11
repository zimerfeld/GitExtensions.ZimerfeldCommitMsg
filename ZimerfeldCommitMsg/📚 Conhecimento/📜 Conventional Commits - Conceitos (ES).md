---
tipo: conhecimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [conhecimento, git, conventional-commits, mensagem-de-commit]
---

# 📜 Conventional Commits — Conceitos

> 🇧🇷 Portugués → [[📜 Conventional Commits - Conceitos]] · 🇺🇸 English → [[📜 Conventional Commits - Conceitos (EN)]]

## 📝 Resumen
**Conventional Commits** es una convención para mensajes de commit que clasifica cada cambio por un **tipo** semántico. El formato canónico es `<tipo>[ámbito opcional]: <descripción>`, con cuerpo y pie opcionales. Ver https://www.conventionalcommits.org/en/v1.0.0/.

## 🏷️ Tipos usados por el plugin

| Tipo | Significado | Verbo (pt / en) |
|---|---|---|
| `feat` | nueva funcionalidad | Adiciona / Add |
| `fix` | corrección de bug | Corrige / Fix |
| `docs` | documentación | Documenta / Document |
| `test` | tests | Testa / Test |
| `chore` | tareas de mantenimiento | Atualiza / Update |
| `build` | build/dependencias | Compila / Build |
| `refactor` | refactorización sin cambio de comportamiento | Refatora / Refactor |

## ⚙️ Cómo lo aplica el plugin

> [!important] Verbo, no prefijo literal
> El plugin usa el tipo para **elegir el verbo**, **no** para escribir `feat:`. El subject empieza por el verbo; los tipos involucrados aparecen solo entre paréntesis en la línea-resumen. Ver [[✍️ Subject iniciado por verbo sem prefixo de tipo (ES)|Subject iniciado por verbo]].

Clasificación en dos dimensiones:
- **Categoría por extensión** (`ExtCategory`): `source`, `web`, `docs`, `build`, `config` — usada para priorizar archivos e inferir el tipo.
- **Tipo Conventional Commits**: heurística sobre categoría + contenido del diff (ej.: archivos de test → `test`; `.md` → `docs`; `.csproj`/`.sln` → `build`).

## 🧾 Formato del mensaje generado

```
[<contexto>] - <verbo> N archivos (tipo1, tipo2, …)   ← subject consolidado
                                                       ← línea en blanco
- <frase resumen del archivo 1>                        ← cuerpo (hasta 5 viñetas)
- <frase resumen del archivo 2>
```

Ejemplo:
```
Overlay - Adiciona 10 arquivos (feat, fix, docs)

- Adiciona autenticação
- Corrige cálculo de saldo
- Documenta instalação
```

## 📍 Dónde aparece esto en el plugin
El [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]] hace la clasificación y el `LanguagePack` mapea tipo → verbo por idioma. Ver [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]].

## 🔗 Relacionado
- [[📦 GitExtensions.ZimerfeldCommitMsg (ES)|GitExtensions.ZimerfeldCommitMsg]]
- [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]]
- [[✍️ Subject iniciado por verbo sem prefixo de tipo (ES)|Subject iniciado por verbo]]
