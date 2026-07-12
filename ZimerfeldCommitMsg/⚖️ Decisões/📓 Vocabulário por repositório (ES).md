---
tipo: decisao
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [decisao, adr, vocabulário, configuração, json]
status: aceita
---

# 📓 ADR — Vocabulario por repositorio (.zimerfeldcommitmsg.json)

> 🇧🇷 Portugués → [[📓 Vocabulário por repositório]] · 🇺🇸 English → [[📓 Vocabulário por repositório (EN)]]

## 🎯 Contexto
Cada proyecto tiene su propia jerga: nombres de dominio ("widget", "overlay") que deberían convertirse en conceptos, y nombres propios/namespaces ("Acme", "Contoso") que **no** deberían. Incrustar todo ese vocabulario en el plugin exigiría recompilar para cada proyecto.

## ✅ Decisión
Leer un archivo **opcional** `.zimerfeldcommitmsg.json` en la raíz del working dir, con tres listas que se suman a los valores por defecto incrustados:
- `knownVocabulary` — palabras aceptadas como parte de un nombre descriptivo.
- `rejectedVocabulary` — palabras que fuerzan el rechazo del nombre como concepto.
- `concepts` — traducción palabra-de-concepto → frase pt-BR (prioridad sobre el diccionario incrustado).

Los fallos de lectura/parseo son **silenciosos** (config vacía) — nunca rompen la generación. Ver [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]].

## 🔀 Alternativas consideradas
- **Solo vocabulario incrustado** — no escala a la jerga de cada proyecto; exigiría recompilar.
- **Setting global en GitExtensions** — no es por repositorio; difícil de versionar junto al código.
- **Archivo por repo versionado (elegida)** — viaja con el proyecto, editable por cualquier miembro del equipo, sin recompilar.

## ⚖️ Consecuencias
**Positivas:**
- Personalización por proyecto sin recompilar; versionable junto al código.
- Robusto ante JSON malformado (lo ignora y sigue con los valores por defecto).

**Negativas / trade-offs:**
- Un archivo de convención más que el equipo debe conocer.
- Sin validación/feedback explícito de error (por diseño — silencioso).

## 🔗 Relacionado
- [[📓 RepoVocabularyConfig (ES)|RepoVocabularyConfig]]
- [[🔀 Duas estratégias - comentários e nomes de arquivo (ES)|Dos estrategias: comentarios y nombres de archivo]]
- [[⚙️ Geração de mensagem a partir do diff (ES)|Generación del mensaje a partir del diff]]
