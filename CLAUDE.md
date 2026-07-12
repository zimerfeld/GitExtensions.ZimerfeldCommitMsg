# CLAUDE.md

Diretrizes persistentes para o Claude neste repositório.

## Idioma

- **Responder sempre no chat em português do Brasil (pt-BR).** Vale para todas
  as respostas, resumos e perguntas ao usuário, independentemente do idioma do
  código, dos arquivos ou da mensagem recebida.

## Paridade de idiomas (PT / EN / ES)

- **Todo conteúdo de documentação deve existir e ser mantido em paridade nos três
  idiomas: português do Brasil (pt-BR), inglês (en-US) e espanhol (es-ES).** Ao
  criar ou alterar um documento em um idioma, criar/atualizar imediatamente as
  versões equivalentes nos outros dois — nunca deixar um idioma para trás.
- Abrange os READMEs (`README.pt-BR.md`, `README.en-US.md`, `README.es-ES.md`, e o
  índice `README.md`) e o cofre Obsidian (nota base pt-BR + variantes ` (EN)` e
  ` (ES)`).
- Manter também a **paridade estrutural**: mesmos títulos, seções, tabelas, blocos
  de código, links e banners de troca de idioma entre as três versões.

## Publicação e Pull Requests

- **Não pedir para criar nem aprovar Pull Requests.** Não sugerir abrir PR, não
  perguntar se deve abrir/aprovar/mesclar PR, e não abrir PR por conta própria.
- O processo de publicação é baseado em **gitflow**: desenvolver na branch de
  trabalho designada, commitar e fazer push. A partir daí, a publicação é
  responsabilidade do fluxo estabelecido, não do Claude.
- A produção é publicada a partir da branch **`main`** por **GitHub Actions**
  (pelo GitHub) ou via **`wrangler`** pelo terminal. Deixe a promoção para
  produção a cargo desse processo.
