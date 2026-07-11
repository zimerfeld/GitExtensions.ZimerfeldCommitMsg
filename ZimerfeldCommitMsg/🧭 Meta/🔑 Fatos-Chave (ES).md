---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [fatos-chave, referencia, meta]
---

# 🔑 Fatos-Chave

> 🇧🇷 Portugués → [[🔑 Fatos-Chave]] · 🇺🇸 English → [[🔑 Fatos-Chave (EN)]]

> [!tip] Lee primero
> Verdades estables y siempre útiles. Actualiza cuando algo cambie.

## 👤 Usuario
- Nombre: **Renato Zimerfeld**
- Email: renato.zimerfeld@gmail.com
- Idioma de trabajo: **Portugués (BR)**
- Ver [[👤 Renato (ES)|Renato]] para preferencias detalladas

## 📁 Rutas importantes
- Cofre de memoria (este): `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg\ZimerfeldCommitMsg`
- Raíz del proyecto: `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg`
- `C:\GitExtensions\GitExtensions.ZimerfeldCommitMsg` **es** el repositorio del proyecto [[📦 GitExtensions.ZimerfeldCommitMsg (ES)|GitExtensions.ZimerfeldCommitMsg]] (C#, plugin de GitExtensions que genera mensajes de commit)
- GitExtensions instalado en: `C:\Program Files\GitExtensions\`
- Plugins de GitExtensions: `C:\Program Files\GitExtensions\Plugins\`
- Settings de GitExtensions (fuente del working dir): `%APPDATA%\GitExtensions\GitExtensions\GitExtensions.settings`
- Config opcional por repositorio: `.zimerfeldcommitmsg.json` en la raíz del directorio de trabajo — ver [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]]
- Assemblies de referencia del host: versionados en `refs\` (build determinista, offline)
- Proyectos hermanos: `GitExtensions.ZimerfeldLFS` (Git LFS) y `GitExtensions.ZimerfeldTree` (árbol de branches), cada uno con su propio cofre

## 🛠️ Herramientas activas
- **RTK (Rust Token Killer)** — proxy CLI que ahorra tokens. Ver [[🦀 RTK (ES)|RTK]]
- **Obsidian** — este cofre de memoria
- **Claude Code** en Windows (shell: PowerShell / git bash)

## 📐 Convenciones
- Fechas: `AAAA-MM-DD`
- **Este proyecto** es un **plugin MEF de GitExtensions** que **genera mensajes de commit** a partir del contenido real del diff en stage, clasificando los cambios según **Conventional Commits** para elegir el **verbo** — el subject empieza por verbo, **sin** el prefijo literal `tipo:`. Ver [[📦 GitExtensions.ZimerfeldCommitMsg (ES)|GitExtensions.ZimerfeldCommitMsg]] y [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]]
- Salida **multilingüe** (pt-BR / inglés / español): automática por el SO + override manual (setting y dropdown de templates)
- Requisito de runtime: **GitExtensions 6.x (.NET 9)** — el plugin ejecuta `git diff --cached` para leer el stage
- Versionado `major.minor.BUILD`, build incrementado automáticamente por `build.ps1`
- Licencia: **CC BY-NC-ND 4.0 © 2026 Zimerfeld**
- Commits co-authored cuando se solicita push
- No hacer commit/push sin petición explícita

## 🔗 Relacionado
- [[🏠 Home (ES)|Home]]
- [[📌 Backlog (ES)|Backlog]]
- [[🧭 Como usar este cofre (ES)|Cómo usar este cofre]]
