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

Para incrementar a versão e gerar um novo `.nupkg`:

```powershell
cd C:\NUGET\ZimerfeldCommitMsg
.\build.ps1
```

O script incrementa o número de build (`major.minor.build`), compila, faz deploy em `C:\Program Files\GitExtensions\Plugins\` (requer Admin) e gera o pacote `.nupkg`.

## Licença

[MIT](LICENSE.txt)
