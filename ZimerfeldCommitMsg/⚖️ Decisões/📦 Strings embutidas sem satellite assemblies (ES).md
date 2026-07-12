---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [decisao, adr, i18n, resx, deploy, dll-única]
status: aceita
---

# 📦 ADR — Strings de UI incrustadas (sin satellite assemblies)

> 🇧🇷 Portugués → [[📦 Strings embutidas sem satellite assemblies]] · 🇺🇸 English → [[📦 Strings embutidas sem satellite assemblies (EN)]]

## 🎯 Contexto
El plugin se distribuye como una **DLL única** (empaquetada en el `lib\` raíz del nupkg). Las strings de UI deben estar localizadas (pt-BR / inglés / español). El mecanismo estándar de .NET para `.resx` localizados genera **satellite assemblies** (`pt-BR\*.resources.dll`), lo que rompería el deploy de archivo único.

## ✅ Decisión
Incrustar los `.resx` (`Strings.resx` = inglés/neutro, `StringsPtBr.resx` = portugués, `StringsEsEs.resx` = español) en el **assembly principal** con un `LogicalName` fijo en el `.csproj`, para que MSBuild **no** los interprete como culturas y no genere satellite assemblies. En runtime, `Strings` usa tres `ResourceManager` y lee con **`InvariantCulture`** (evita el probing de satélites), seleccionando el idioma por el valor **resuelto** — no por la cultura global del hilo.

## 🔀 Alternativas consideradas
- **Satellite assemblies estándar** — idiomático, pero genera DLLs por cultura → incompatible con el deploy de DLL única.
- **Strings hard-coded en el código** — sin `.resx`, difícil de mantener y traducir.
- **`.resx` neutro incrustado + LogicalName (elegida)** — una única DLL, traducción mantenida en `.resx`, la selección honra el override manual de idioma.

## ⚖️ Consecuencias
**Positivas:**
- Deploy de **DLL única** preservado (encaja con el empaquetado en el `lib\` raíz).
- Override manual de idioma honrado (selección por idioma resuelto, no por el hilo).

**Negativas / trade-offs:**
- Requiere el `LogicalName` explícito y el uso de `InvariantCulture` — no es el camino idiomático de .NET.
- Los nombres de recurso deterministas deben coincidir exactamente con los `ResourceManager` en `Strings.cs`.

## 🔗 Relacionado
- [[🌐 Localization (ES)|Localization]]
- [[🏷️ Versionamento (ES)|Versionado]]
- [[🔗 Dependências (ES)|Dependencias]]
