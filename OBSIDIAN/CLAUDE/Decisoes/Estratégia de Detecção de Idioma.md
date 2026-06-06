---
tipo: decisão
tags: [decisão, idioma, tradução, português, inglês]
status: estável
---

# Decisão: Estratégia de Detecção de Idioma

> [!info] Escopo: idioma do **comentário** (entrada), não da **saída**
> Esta nota trata de detectar o idioma de um *comentário do código* para decidir se traduz para
> pt-BR. A escolha do **idioma de saída** da mensagem (Português/Inglês, automático pelo SO +
> override) é outra decisão: ver [[Suporte Multilíngue PT-EN]]. A tradução EN→PT descrita aqui
> **só roda quando o idioma de saída é pt-BR**.

## Contexto

Comentários no código podem estar em inglês ou português. Quando a saída é pt-BR, o plugin precisa:
1. Detectar o idioma do comentário
2. Traduzir inglês → pt-BR
3. Descartar traduções de má qualidade

## Decisão

**Usar um limiar de 25% de palavras inglesas conhecidas para classificar o texto como inglês.**

```csharp
private static bool IsEnglishText(string text)
{
    var words = Regex.Matches(text.ToLowerInvariant(), @"\b[a-z]{3,}\b")
                     .Select(m => m.Value)
                     .ToList();
    if (words.Count < 3) return false;
    var englishCount = words.Count(w => EnglishWords.Contains(w));
    return (double)englishCount / words.Count >= 0.25;
}
```

`EnglishWords` = todas as chaves do `WordTranslations` (≈120 palavras).

## Pipeline de tradução (TranslateToPortuguese)

```
1. IsEnglishText? → não → retorna texto como está (já pt-BR)

2. Fase 1 — Frases compostas (PhraseTranslations, ~25 entradas)
   Longest-match first para evitar fragmentação:
   "doesn't match the filter" → "não corresponde ao filtro"

3. Fase 2 — Padrões estruturais
   "X-based" → "baseado em X"
   "recursively VERB" → "VERB recursivamente"

4. Fase 3 — Palavras individuais (WordTranslations, ~120 entradas)
   Preserva PascalCase = identificadores de código
   "returns" → "retorna", "the" → "" (artigo omitido)

5. Limpeza: remove espaços duplos gerados por artigos removidos

6. IsEnglishText novamente?
   Ainda ≥25% inglês após tradução → descarta (retorna null)
   Qualidade insuficiente — será ignorado pelo pipeline
```

> **Preservação de branches/tipos CC (branch `feature/modelo`):** antes da Fase 1, nomes de branch gitflow e tipos Conventional Commits são **mascarados** (`PreservePattern`) e restaurados só no fim. A checagem de qualidade do passo 6 roda sobre o texto **mascarado**, para que slugs como `feature/search` não inflem o percentual de inglês e descartem uma boa tradução. Detalhes em [[Preservação de Branches e Tipos CC]].

## Preservação de identificadores

```csharp
// Preserva PascalCase: primeira maiúscula + tem minúsculas = identificador de código
if (char.IsUpper(word[0]) && word.Length > 1 && word.Any(char.IsLower))
    return word;  // não traduz: "UserService", "GitExtensions", "FormCommit"
```

## Razão para o limiar de 25%

- Muito baixo (ex: 10%) → falsos positivos: texto pt-BR com palavras cognatas seria "detectado" como inglês
- Muito alto (ex: 50%) → falsos negativos: frases mistas inglês+código não seriam traduzidas
- 25% equilibra os dois extremos para comentários típicos de código C#

## Exemplo

Comentário inglês: `"filters stems with dot to avoid assembly names"`
- Palavras: filters, stems, with, dot, to, avoid, assembly, names → 8 palavras
- Inglesas reconhecidas: filters, with, to, avoid → 4 = 50% → inglês detectado
- Tradução: `"filtra stems com ponto para evitar nomes de assembly"`

## Relacionado

- [[../Fluxos/Geração da Mensagem]]
- [[../Arquivos-Chave/CommitMessageGenerator]]
- [[Prioridade de Comentários]]
- [[Preservação de Branches e Tipos CC]]
