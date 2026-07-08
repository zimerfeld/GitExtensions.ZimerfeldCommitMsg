---
tipo: procedimento
projeto: GitExtensions.ZimerfeldCommitMsg
lang: pt-BR
atualizado: 2026-07-04
tags: [operação, dev, build, install, powershell]
---

# 💻 Ambiente Local (Dev)

Como compilar e instalar o plugin no GitExtensions local para desenvolver e testar.

## ⚡ TL;DR — o comando único

```powershell
# na raiz do repo, PowerShell como Administrador (para o deploy em Program Files)
.\build.ps1 -Force
```

Compila em Release, faz deploy da DLL em `C:\Program Files\GitExtensions\Plugins\` e empacota o `.nupkg`. Reinicie o GitExtensions para carregar a nova DLL.

## ⚙️ O que o script faz (em ordem)

`build.ps1` (detalhe em [[🛠️ build.ps1]] e [[🏷️ Versionamento]]):
1. Lê a versão atual do `.nuspec` e calcula `newVersion` (build +1).
2. Detecta mudanças (fontes/docs mais novos que o último `.nupkg`); sem `-Force` e sem mudança → mantém a versão e sai.
3. Fecha o GitExtensions se estiver em execução (libera a DLL).
4. Carimba versão + data nos READMEs e nas notas do cofre; dá o bump no `.nuspec`/`.csproj`.
5. `dotnet build -c Release`.
6. Copia a DLL para `C:\Program Files\GitExtensions\Plugins\` (Admin) e para `tools\net9.0-windows\`.
7. `nuget pack` → `.nupkg` na raiz (filtra o aviso NU5101) e remove `.nupkg` antigos.

## 🧰 Instalação manual (alternativa ao build.ps1)

```powershell
tools\install.ps1      # requer Admin — copia a DLL para a pasta Plugins (também via NuGet PMC)
tools\uninstall.ps1    # requer Admin — remove a DLL
tools\update-dll.ps1   # atualiza apenas a DLL na pasta Plugins
```
`install.ps1` roda de duas formas: standalone (`cd tools; .\install.ps1`) ou via **NuGet Package Manager Console** (`Install-Package GitExtensions.ZimerfeldCommitMsg -Source C:\NUGET`). Se a DLL não existir em `bin\Release`, ele dispara um `build.ps1 -Force`.

## 🧪 Testes

```powershell
dotnet test tests\GitExtensions.ZimerfeldCommitMsg.Tests
```
Suíte **xUnit**: extração de comentários, derivação de conceitos, `.zimerfeldcommitmsg.json`, tradução EN→PT. As funções puras do gerador são expostas por `InternalsVisibleTo`.

## 🚦 Flags e requisitos
- `-Force` — recompila/empacota mesmo sem mudança detectada.
- **.NET 9 SDK** (`dotnet`) obrigatório.
- **Administrador** obrigatório para o deploy em `C:\Program Files\GitExtensions\Plugins\`. Sem Admin, o passo de deploy é pulado com aviso.
- **GitExtensions 6.x (.NET 9)** instalado; `git` no `PATH`.

## 🔀 Regras que respeita (GitFlow)
- Desenvolvimento em **feature branch**; não commitar/pushar sem pedido explícito.
- O carimbo de versão/docs é local — o commit fica a critério do usuário.

## 🩺 Troubleshooting
- **DLL travada / não atualiza:** feche o GitExtensions (o `build.ps1` já tenta fechar) e rode de novo.
- **Deploy pulado:** rode o PowerShell como **Administrador**.
- **Plugin não aparece:** confirme a DLL em `C:\Program Files\GitExtensions\Plugins\` e **reinicie** o GitExtensions. O nó em Configurações → Plugins só aparece após reiniciar.

## 🔗 Ligações
- [[🚀 Deploy em Produção (Prod)]]
- [[🛠️ build.ps1]]
- [[🏷️ Versionamento]]
- [[📖 README — Instalação, Uso e Build]]
