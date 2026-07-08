---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [decisao, adr, i18n, resx, deploy, dll-única]
status: aceita
---

# 📦 ADR — Embedded UI strings (no satellite assemblies)

## 🎯 Context
The plugin ships as a **single DLL** (packed into the nupkg's root `lib\`). The UI strings must be localized (pt-BR / English). The standard .NET mechanism for localized `.resx` files generates **satellite assemblies** (`pt-BR\*.resources.dll`), which would break the single-file deploy.

## ✅ Decision
Embed the `.resx` files (`Strings.resx` = English/neutral, `StringsPtBr.resx` = Portuguese) into the **main assembly** with a fixed `LogicalName` in the `.csproj`, so MSBuild does **not** interpret them as cultures and does not generate satellite assemblies. At runtime, `Strings` uses two `ResourceManager`s and reads with **`InvariantCulture`** (avoids satellite probing), selecting the language by the **resolved** value — not by the thread's global culture.

## 🔀 Alternatives considered
- **Standard satellite assemblies** — idiomatic, but generates per-culture DLLs → incompatible with the single-DLL deploy.
- **Hard-coded strings in code** — no `.resx`, hard to maintain and translate.
- **Embedded neutral `.resx` + LogicalName (chosen)** — a single DLL, translations kept in `.resx`, and selection honors the manual language override.

## ⚖️ Consequences
**Positive:**
- **Single-DLL** deploy preserved (matches the root `lib\` packaging).
- Manual language override honored (selection by resolved language, not by thread).

**Negative / trade-offs:**
- Requires the explicit `LogicalName` and the use of `InvariantCulture` — not the idiomatic .NET path.
- Deterministic resource names must exactly match the `ResourceManager`s in `Strings.cs`.

## 🔗 Related
- [[🌐 Localization (EN)|Localization]]
- [[🏷️ Versionamento (EN)|Versioning]]
- [[🔗 Dependências (EN)|Dependencies]]
