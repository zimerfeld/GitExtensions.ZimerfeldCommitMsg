# GitExtensions.ZimerfeldCommitMsg

Plugin para **GitExtensions** que gera automaticamente uma mensagem de commit resumindo em uma frase as mudanças nos arquivos staged, usando o formato **Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`refactor`).

## Funcionalidades

- **Template no diálogo de commit**: selecione "Zimerfeld: Auto-resumo" no dropdown de templates da janela de commit para preencher a mensagem automaticamente.
- **Atalho pelo menu Plugins**: clique em _Plugins → ZimerfeldCommitMsg_ para abrir o diálogo de commit com a mensagem já preenchida.
- **Desinstalação limpa**: a remoção da DLL não afeta nenhuma outra parte do GitExtensions.

## Exemplos de mensagens geradas

| Mudanças staged | Mensagem gerada |
|---|---|
| Novo arquivo `UserService.cs` | `feat: add UserService.cs` |
| 3 arquivos `.cs` adicionados em `src/Services` | `feat: add 3 .cs files in src/Services` |
| Modificações em `appsettings.json` | `chore: update appsettings.json` |
| Arquivo `README.md` | `docs: update README.md` |
| Mix de adições e modificações em `.cs` | `feat: add (2 added, 1 modified) .cs files in src/Auth` |

## Instalação

### Opção A — Via PowerShell (recomendado)

```powershell
cd C:\NUGET\ZimerfeldCommitMsg\tools
.\install.ps1
```

### Opção B — Manual

Copie `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` para:

```
C:\Program Files\GitExtensions\Plugins\
```

Reinicie o GitExtensions.

## Desinstalação

```powershell
cd C:\NUGET\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```

## Build / atualização

A cada mudança, execute `build.ps1` — ele **incrementa automaticamente** o número de build (`major.minor.build`), compila, faz deploy e gera o `.nupkg`.

### Opção A — PowerShell (recomendado, requer Admin para deploy)

```powershell
cd C:\NUGET\ZimerfeldCommitMsg
.\build.ps1
```

### Opção B — Git Bash / Bash tool (sem elevação de Admin)

```bash
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "C:/NUGET/ZimerfeldCommitMsg/build.ps1"
```

> **O que o script faz, a cada execução:**
> 1. Lê a versão atual do `.nuspec`
> 2. Incrementa o `build` em +1 → `major.minor.build`
> 3. Atualiza `.nuspec` e `.csproj` com a nova versão
> 4. Compila em Release
> 5. Copia o DLL para `C:\Program Files\GitExtensions\Plugins\` *(requer Admin)*
> 6. Atualiza `tools\net9.0-windows\` com o DLL novo
> 7. Gera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`
> 8. Remove `.nupkg` de versões anteriores

## Licença

[MIT](LICENSE.txt)
