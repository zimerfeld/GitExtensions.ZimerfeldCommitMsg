---
tipo: moc
tags: [home, moc, zimerfeld]
atualizado: 2026-06-01
---

# ZimerfeldCommitMsg — Mapa do Cofre

<<<<<<< HEAD
![[Anexos/ScreenshotDropDown.png]]
=======
![[Anexos/ScreenshotDropdown.png]]
>>>>>>> feature/build

Plugin para **GitExtensions** que gera mensagens de commit automáticas no formato **Conventional Commits v1.0.0**, em português-BR, analisando o conteúdo das alterações staged.

---

## Navegação

### Sistema
- [[Sistema/Visão Geral]] — o que o plugin faz, stack, versão atual
- [[Sistema/Arquitetura]] — duas classes, como se relacionam
- [[Sistema/Versionamento]] — ciclo build.ps1 / nuspec / csproj
- [[Sistema/Dependências]] — assemblies do GitExtensions

### Fluxos
- [[Fluxos/Geração da Mensagem]] — pipeline completo do `Generate()`
- [[Fluxos/Template Dropdown (Auto-resumo)]] — passos ao selecionar o template no dropdown
- [[Fluxos/Stage Trigger]] — auto-atualização ao dar stage/unstage
- [[Fluxos/Instalação e Deploy]] — install / uninstall / update-dll

### Arquivos-Chave
- [[Arquivos-Chave/CommitMessageGenerator]] — núcleo gerador (1052 linhas)
- [[Arquivos-Chave/ZimerfeldCommitMsgPlugin]] — ponto de entrada do plugin
- [[Arquivos-Chave/build.ps1]] — script de build e versionamento
- [[Arquivos-Chave/inspector]] — ferramenta de introspecção de assemblies

### Decisões Arquiteturais
- [[Decisoes/Título como Lista de Types]] — branch `feature/titulo` (2026-05-22)
- [[Decisoes/Prioridade de Comentários]] — ranqueamento por tipo de arquivo
- [[Decisoes/Estratégia de Detecção de Idioma]] — critério 25% de palavras inglesas
- [[Decisoes/Preservação de Branches e Tipos CC]] — branch `feature/modelo` (2026-06-01)

---

## Estrutura de Pastas do Repo

```
ZimerfeldCommitMsg/
├── src/GitExtensions.ZimerfeldCommitMsg/
│   ├── CommitMessageGenerator.cs   ← núcleo
│   ├── ZimerfeldCommitMsgPlugin.cs ← entry point
│   ├── *.csproj / *.nuspec
├── inspector/
│   └── Program.cs                  ← introspecção de DLLs
├── tools/
│   ├── install.ps1
│   ├── uninstall.ps1
│   ├── update-dll.ps1
│   └── net9.0-windows/             ← DLL para nupkg
├── build.ps1                       ← build + versão + pack
├── FUNCIONALIDADES.md
└── README.md
```

---

## Versão Atual

`1.0.16` — compilada em `net9.0-windows`, target `AnyCPU`
