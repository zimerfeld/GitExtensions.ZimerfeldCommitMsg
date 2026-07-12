---
tipo: sistema
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [sistema, overview, plugin, gitextensions, commit-message, conventional-commits, i18n]
---

# 🔭 Visión General

> 🇧🇷 Portugués → [[🔭 Visão Geral]] · 🇺🇸 English → [[🔭 Visão Geral (EN)]]

## 🎯 Qué es

Plugin para **GitExtensions** (Windows) que **genera el mensaje de commit automáticamente** a partir del contenido real de los cambios en stage. Se integra con el **diálogo de commit** del host: al abrir el diálogo (y siempre que archivos entran/salen del stage), el plugin lee el `git diff --cached`, clasifica los cambios por **Conventional Commits** y materializa un **subject iniciado por verbo** + **cuerpo en bullets**, en el idioma elegido. Ver [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]] y [[✍️ Subject iniciado por verbo sem prefixo de tipo (ES)|Subject iniciado por verbo sin prefijo de tipo]].

## 🧱 Stack

| Item | Valor |
|---|---|
| Lenguaje | C# (.NET 9) |
| Target | `net9.0-windows` |
| UI Framework | Windows Forms (`UseWindowsForms`) — sin ventana propia; se integra al diálogo de commit |
| Tipo de salida | `Library` (DLL cargada por GitExtensions) |
| Assembly de salida | `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` |
| Namespace | `GitExtensions.ZimerfeldCommitMsg` |
| Versión actual | `1.0.97` |
| Idiomas | Portugués-BR / Inglés / Español (auto por el SO + override) |
| Licencia | CC BY-NC-ND 4.0 © 2026 Zimerfeld |
| Autor | Zimerfeld |

> El plugin muestra un **icono** (PNG embebido en `Resources/icon.png`) en el menú Plugins y en el dropdown de plantillas del diálogo de commit.

## 🔌 Los tres modos de integración

### 1️⃣ Plantilla en el dropdown del diálogo de commit
El plugin registra **cuatro elementos de plantilla** (uno por idioma) vía `CommitTemplateManager`. Elegir un elemento aplica el mensaje generado en ese idioma y fija el idioma para el auto-refresh de la sesión. Ver [[🌐 3 - Idioma e auto-refresh (ES)|3 - Idioma y auto-refresh]].

### 2️⃣ Menú Plugins → ZimerfeldCommitMsg
Dispara `Execute` y genera el mensaje para el repositorio actual, usando el idioma del **setting** (Automático/PT/EN/ES).

### 3️⃣ Auto-relleno (Application.Idle)
Como no hay evento de "diálogo de commit abierto", el plugin observa el **`Application.Idle`**, detecta el `FormCommit` recién abierto y **rellena la caja de mensaje** — repitiendo al stage/unstage. **Nunca sobrescribe** texto escrito manualmente. Ver [[🔍 1 - Detecção do diálogo de commit (ES)|1 - Detección del diálogo de commit]].

## ⚙️ Cómo se genera el mensaje

El [[⚙️ CommitMessageGenerator (ES)|CommitMessageGenerator]] lee el `git diff --cached` y produce:
- **Subject consolidado:** `[<contexto>] - <verbo> N arquivos (tipos)` — ej.: `Overlay - Adiciona 10 arquivos (feat, fix, docs)`.
- **Cuerpo en bullets:** hasta 5 frases, cada una resumiendo el cambio más significativo de un archivo, extraída de los **comentarios** del diff (estrategia principal) o derivada de los **nombres de archivo** (fallback).

Ver [[⚙️ Geração de mensagem a partir do diff (ES)|Generación de mensaje a partir del diff]].

## 🧰 Recursos transversales

| Recurso | Detalle |
|---|---|
| Dos estrategias | comentarios del diff (principal) + nombres de archivo (fallback) — [[🔀 Duas estratégias - comentários e nomes de arquivo (ES)|Dos estrategias - comentarios y nombres de archivo]] |
| Vocabulario por repo | `.zimerfeldcommitmsg.json` extiende vocabulario/conceptos — [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]] |
| Traducción EN→PT/ES | comentarios traducidos cuando la salida es pt-BR o es-ES; intactos en inglés |
| Saneamiento | descarta comentarios con delimitadores desbalanceados o conector suelto al final |
| Localización | strings de UI embebidas (`Strings.resx`/`StringsPtBr.resx`/`StringsEsEs.resx`) + idioma del mensaje |
| No destructivo | jamás sobrescribe lo que el usuario ya escribió |

## ✅ Requisitos de runtime

- **GitExtensions 6.x** (.NET 9)
- **`git`** en el `PATH` (el generador ejecuta `git diff --cached`)

## 🔗 Enlaces

- [[🏗️ Arquitetura (ES)|Arquitectura]]
- [[🏷️ Versionamento (ES)|Versionado]]
- [[🔗 Dependências (ES)|Dependencias]]
- [[📜 Conventional Commits - Conceitos (ES)|Conventional Commits - Conceptos]]
