# CLAUDE.md

Diretrizes persistentes para o Claude neste repositório.

## Idioma

- **Responder sempre no chat em português do Brasil (pt-BR).** Vale para todas
  as respostas, resumos e perguntas ao usuário, independentemente do idioma do
  código, dos arquivos ou da mensagem recebida.

## Publicação e Pull Requests

- **Não pedir para criar nem aprovar Pull Requests.** Não sugerir abrir PR, não
  perguntar se deve abrir/aprovar/mesclar PR, e não abrir PR por conta própria.
- O processo de publicação é baseado em **gitflow**: desenvolver na branch de
  trabalho designada, commitar e fazer push. A partir daí, a publicação é
  responsabilidade do fluxo estabelecido, não do Claude.
- A produção é publicada a partir da branch **`main`** por **GitHub Actions**
  (pelo GitHub) ou via **`wrangler`** pelo terminal. Deixe a promoção para
  produção a cargo desse processo.
