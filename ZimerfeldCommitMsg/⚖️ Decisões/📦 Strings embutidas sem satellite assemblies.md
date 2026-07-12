---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [decisao, adr, i18n, resx, deploy, dll-única]
status: aceita
---

# 📦 ADR — Strings de UI embutidas (sem satellite assemblies)

> 🇺🇸 English → [[📦 Strings embutidas sem satellite assemblies (EN)]] · 🇪🇸 Español → [[📦 Strings embutidas sem satellite assemblies (ES)]]

## 🎯 Contexto
O plugin é distribuído como uma **DLL única** (empacotada em `lib\` raiz do nupkg). As strings de UI precisam ser localizadas (pt-BR / inglês). O mecanismo padrão do .NET para `.resx` localizados gera **satellite assemblies** (`pt-BR\*.resources.dll`), o que quebraria o deploy de arquivo único.

## ✅ Decisão
Embutir os `.resx` (`Strings.resx` = inglês/neutro, `StringsPtBr.resx` = português) no **assembly principal** com `LogicalName` fixo no `.csproj`, para o MSBuild **não** interpretá-los como culturas e não gerar satellite assemblies. Em runtime, `Strings` usa dois `ResourceManager` e lê com **`InvariantCulture`** (evita probing de satélites), selecionando o idioma pelo valor **resolvido** — não pela cultura global da thread.

## 🔀 Alternativas consideradas
- **Satellite assemblies padrão** — idiomático, mas gera DLLs por cultura → incompatível com o deploy de DLL única.
- **Strings hard-coded no código** — sem `.resx`, difícil de manter e traduzir.
- **`.resx` neutro embutido + LogicalName (escolhida)** — uma única DLL, tradução mantida em `.resx`, seleção honra o override manual de idioma.

## ⚖️ Consequências
**Positivas:**
- Deploy de **DLL única** preservado (casa com o empacotamento em `lib\` raiz).
- Override manual de idioma honrado (seleção por idioma resolvido, não pela thread).

**Negativas / trade-offs:**
- Requer o `LogicalName` explícito e o uso de `InvariantCulture` — não é o caminho idiomático do .NET.
- Nomes de recurso determinísticos precisam casar exatamente com os `ResourceManager` em `Strings.cs`.

## 🔗 Relacionado
- [[🌐 Localization]]
- [[🏷️ Versionamento]]
- [[🔗 Dependências]]
