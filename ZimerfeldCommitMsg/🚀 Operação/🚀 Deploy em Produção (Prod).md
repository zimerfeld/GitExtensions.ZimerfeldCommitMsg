---
tipo: procedimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [operação, deploy, release, nupkg, nuget, github]
---

# 🚀 Deploy em Produção (Prod)

Como publicar uma nova versão do plugin: gerar o `.nupkg` versionado e distribuí-lo (NuGet + release no GitHub). A distribuição para o usuário final é via **Plugin Manager do GitExtensions** e **NuGet**.

## ⚡ TL;DR — o comando único

```powershell
# na raiz do repo (Admin), gera o .nupkg de release com a versão bumpada
.\build.ps1
```

O `.nupkg` de produção fica na **raiz do repo** (`GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`). Publique-o no NuGet e anexe-o à release do GitHub.

## ⚙️ O que o script faz (em ordem)

`build.ps1` (fonte da verdade da versão: `.nuspec`/`.csproj`):
1. Lê a versão atual e calcula `newVersion` (build +1).
2. Detecta mudanças; sem `-Force` e sem mudança → não gera novo pacote.
3. Fecha o GitExtensions.
4. Carimba versão + data nos **READMEs** e nas notas do cofre que espelham a versão atual; dá o bump no `.nuspec`/`.csproj`.
5. `dotnet build -c Release`.
6. Copia a DLL para `tools\net9.0-windows\` (fonte do nupkg).
7. `nuget pack` → `.nupkg` na raiz; remove `.nupkg` de versões anteriores. A DLL vai em `lib\` **raiz** (grupo "any") — daí o aviso **NU5101**, filtrado de propósito.

## 📤 Publicação (após o build)

```powershell
# 1) Publicar no NuGet.org (requer API key)
nuget push .\GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey <API_KEY>

# 2) Criar a release no GitHub com a tag da versão e anexar o .nupkg
gh release create vX.Y.Z .\GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg --title "vX.Y.Z" --notes "..."
```
> Prova social no README: badges `shields.io/nuget/v` (versão) e `/dt` (downloads) — atualizam sozinhos após a publicação no NuGet.

## 🚦 Requisitos
- **.NET 9 SDK**, `nuget.exe` (em `tools\nuget.exe`), API key do NuGet, `gh` autenticado.
- Versão sincronizada em `.nuspec` **e** `.csproj` (o `build.ps1` garante).

## 🔀 Regras que respeita (GitFlow)
- **Não** fazer deploy de produção a partir de uma **release branch**: validar em develop na release branch → finalizar a release atualizando `develop` e `main` → gerar a **tag** `YYYYMMddhhmm<fase>` → só então publicar.
- Garantir que a `main` seja a branch **default** (reflete produção).
- Não commitar/pushar/publicar sem pedido explícito do usuário.

## 🩺 Troubleshooting
- **NU5101 ao empacotar:** esperado (DLL em `lib\` raiz para o Plugin Manager extrair) — o `build.ps1` filtra.
- **Pacote não gerado:** rode `.\build.ps1 -Force` (sem mudança detectada, o build é pulado).
- **Plugin Manager não lista:** confirme a dependência marcadora `GitExtensions.Extensibility` no `.nuspec`. Ver [[🔗 Dependências]].

## 🔗 Ligações
- [[💻 Ambiente Local (Dev)]]
- [[🛠️ build.ps1]]
- [[🏷️ Versionamento]]
- [[🔗 Dependências]]
