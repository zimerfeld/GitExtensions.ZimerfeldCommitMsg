---
tipo: sessao
data: 2026-06-01
hora: 00:00
tags: [sessao, setup, obsidian]
resumo: Criação do cofre Obsidian de memória do Claude para o projeto ZimerfeldCommitMsg
projetos: [GitExtensions.ZimerfeldCommitMsg]
---

# Sessão 2026-06-01 — Criação do cofre de neurônios (CommitMsg)

## 🎯 Pedido do Renato
Criar um "cofre de neurônios" no Obsidian em `C:\NUGET\ZimerfeldCommitMsg\OBSIDIAN\CLAUDE`, usando como modelo o cofre existente em `C:\NUGET\ZimerfeldTree\OBSIDIAN\CLAUDE`. Tudo dali em diante deve ser salvo nesse cofre.

## ✅ O que foi feito
- Copiada a estrutura e a configuração do cofre modelo (`.obsidian/`, pastas 00–07/99/Anexos, templates)
- Mantidas as notas genéricas: [[Renato]], [[RTK]], [[📥 Inbox]], [[🧭 Como usar este cofre]], [[🧠 HOME - Cofre de Neurônios]]
- Adaptados os [[🔑 Fatos-Chave]] para os caminhos de `C:\NUGET\ZimerfeldCommitMsg`
- Criada a nota de projeto [[GitExtensions.ZimerfeldCommitMsg]]
- Criadas as notas de conhecimento [[Plugin MEF para GitExtensions]] e [[Geração de mensagem - Conventional Commits]]
- Atualizado [[Renato]] para listar os dois plugins (CommitMsg e Tree)

## 🧠 Aprendizados / decisões
- O projeto é um **plugin MEF de GitExtensions** que gera mensagens de commit (Conventional Commits, descrição em pt-BR)
- Núcleo da lógica: `CommitMessageGenerator.cs` (~985 linhas), duas estratégias (comentários do diff → fallback por nomes de arquivo)
- Integração via `AddCommitTemplate` ("Zimerfeld: Auto-resumo"), menu `Execute`, e auto-refresh em `PostRepositoryChanged` (sem sobrescrever texto do usuário)
- Existe um utilitário `inspector\` que descobre a API do GitExtensions por reflection
- Build: `build.ps1` incrementa `major.minor.BUILD`, sincroniza nuspec/csproj/FUNCIONALIDADES.md, faz deploy e empacota nupkg

## 📝 Arquivos tocados
- Cofre inteiro em `C:\NUGET\ZimerfeldCommitMsg\OBSIDIAN\CLAUDE\`

## ⏭️ Próximos passos
- [ ] (Opcional) Instalar plugins Dataview e Templater no Obsidian
- [ ] Aprofundar leitura de `CommitMessageGenerator.cs` quando formos evoluir a geração

## 🔗 Notas relacionadas
- [[🧠 HOME - Cofre de Neurônios]]
- [[GitExtensions.ZimerfeldCommitMsg]]
