using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using GitExtensions.ZimerfeldCommitMsg.Localization;

namespace GitExtensions.ZimerfeldCommitMsg;

/// <summary>
/// Gera mensagens de commit no formato Conventional Commits v1.0.0
/// https://www.conventionalcommits.org/en/v1.0.0/
///
/// Formato:  &lt;type&gt;: &lt;descrição em pt-BR&gt;
///           [linha em branco]
///           [corpo: frase semântica em pt-BR]
/// </summary>
internal sealed class CommitMessageGenerator
{
    private readonly string _workingDir;
    private readonly MessageLanguage _language;
    private readonly LanguagePack _lang;

    // ── Extension → semantic category ─────────────────────────────────────────

    private static readonly Dictionary<string, string> ExtCategory = new(StringComparer.OrdinalIgnoreCase)
    {
        ["cs"]    = "source", ["vb"]    = "source", ["fs"]     = "source",
        ["cpp"]   = "source", ["c"]     = "source", ["h"]      = "source",
        ["java"]  = "source", ["py"]    = "source", ["rb"]     = "source",
        ["go"]    = "source", ["rs"]    = "source", ["kt"]     = "source",
        ["swift"] = "source", ["php"]   = "source", ["dart"]   = "source",
        ["lua"]   = "source", ["scala"] = "source",
        ["js"]    = "web",    ["ts"]    = "web",    ["jsx"]    = "web",
        ["tsx"]   = "web",    ["vue"]   = "web",    ["svelte"] = "web",
        ["html"]  = "web",    ["htm"]   = "web",    ["razor"]  = "web",
        ["cshtml"]= "web",    ["css"]   = "web",    ["scss"]   = "web",
        ["less"]  = "web",    ["sass"]  = "web",
        ["md"]    = "docs",   ["txt"]   = "docs",   ["rst"]    = "docs",
        ["adoc"]  = "docs",
        ["csproj"]= "build",  ["vbproj"]= "build",  ["fsproj"] = "build",
        ["sln"]   = "build",  ["props"] = "build",  ["targets"]= "build",
        ["dockerfile"]="build",["makefile"]="build",
        ["json"]  = "config", ["xml"]   = "config", ["yaml"]   = "config",
        ["yml"]   = "config", ["toml"]  = "config", ["ini"]    = "config",
        ["env"]   = "config", ["config"]= "config", ["conf"]   = "config",
        ["lock"]  = "config", ["editorconfig"] = "config",
    };

    // ── Suffixes stripped to reach the domain concept ──────────────────────────
    // Ordered longest-first so "ServiceTests" is stripped before "Service"
    private static readonly string[] SemanticSuffixes =
    [
        "ServiceTests","ControllerTests","RepositoryTests","ManagerTests","HandlerTests",
        "Services","Controllers","Repositories","Managers","Handlers","Generators",
        "Service","Controller","Repository","Manager","Handler","Generator",
        "Helper","Helpers","Provider","Providers","Factory","Builder",
        "Middleware","Interceptor","Validator","Validators","Mapper","Mappers",
        "Resolver","Extension","Extensions","Util","Utils","Utility","Utilities",
        "Tests","Test","Specs","Spec","Mock","Mocks",
        "Command","Commands","Query","Queries","Event","Events",
        "Request","Response","Dto","ViewModel","Model","Entity","Schema",
        "Context","DbContext","Configuration","Config","Settings","Options","Constants",
        "Facade","Proxy","Decorator","Adapter","Client","Endpoint","Endpoints",
        "Base","Abstract","Interface","Implementation","Impl",
    ];

    // ── Tradução inglês → pt-BR: frases compostas (mais longas primeiro) ─────────
    private static readonly (string En, string Pt)[] PhraseTranslations =
    [
        // Negações compostas (mais específicas antes das genéricas)
        ("the node and all its descendants don't match the filter",
            "o nó e todos os descendentes não correspondem ao filtro"),
        ("the node and all its descendants", "o nó e todos os descendentes"),
        ("all its descendants",              "todos os descendentes"),
        ("don't match the filter",           "não correspondem ao filtro"),
        ("doesn't match the filter",         "não corresponde ao filtro"),
        ("don't match",                      "não correspondem"),
        ("doesn't match",                    "não corresponde"),
        ("do not match",                     "não correspondem"),
        ("does not match",                   "não corresponde"),
        ("returns null when",                "retorna null quando"),
        // Contexto git
        ("git-history hierarchy",            "hierarquia do histórico git"),
        ("this was created from",            "de onde foi criado"),
        ("then git-history hierarchy within each group",
            "depois hierarquia do histórico git dentro de cada grupo"),
        ("within each group",                "dentro de cada grupo"),
        ("grouped by remote",                "agrupados por remoto"),
        // Padrões relacionais
        ("split by",                         "separado por"),
        ("grouped by",                       "agrupado por"),
        ("sorted by",                        "ordenado por"),
        ("filtered by",                      "filtrado por"),
        ("ordered by",                       "ordenado por"),
        ("based on",                         "baseado em"),
        ("mapped to",                        "mapeado para"),
        ("converted to",                     "convertido para"),
        ("converted into",                   "convertido em"),
        ("created from",                     "criado a partir de"),
        ("derived from",                     "derivado de"),
    ];

    // ── Tradução inglês → pt-BR: palavras individuais ─────────────────────────
    private static readonly Dictionary<string, string> WordTranslations =
        new(StringComparer.OrdinalIgnoreCase)
    {
        // Verbos (3ª pessoa / infinitivo)
        ["returns"]       = "retorna",      ["return"]        = "retornar",
        ["converts"]      = "converte",     ["convert"]       = "converter",
        ["creates"]       = "cria",         ["create"]        = "criar",
        ["removes"]       = "remove",       ["remove"]        = "remover",
        ["adds"]          = "adiciona",     ["add"]           = "adicionar",
        ["updates"]       = "atualiza",     ["update"]        = "atualizar",
        ["builds"]        = "constrói",     ["build"]         = "construir",
        ["gets"]          = "obtém",        ["get"]           = "obter",
        ["sets"]          = "define",       ["set"]           = "definir",
        ["takes"]         = "recebe",       ["take"]          = "receber",
        ["makes"]         = "cria",         ["make"]          = "criar",
        ["uses"]          = "usa",          ["use"]           = "usar",
        ["reads"]         = "lê",           ["read"]          = "ler",
        ["writes"]        = "escreve",      ["write"]         = "escrever",
        ["parses"]        = "processa",     ["parse"]         = "processar",
        ["filters"]       = "filtra",       ["handles"]       = "trata",
        ["handle"]        = "tratar",       ["validates"]     = "valida",
        ["validate"]      = "validar",      ["loads"]         = "carrega",
        ["load"]          = "carregar",     ["saves"]         = "salva",
        ["save"]          = "salvar",       ["checks"]        = "verifica",
        ["check"]         = "verificar",    ["matches"]       = "corresponde",
        ["match"]         = "corresponder", ["wraps"]         = "encapsula",
        ["wrap"]          = "encapsular",   ["extends"]       = "estende",
        ["extend"]        = "estender",     ["represents"]    = "representa",
        ["contains"]      = "contém",       ["contain"]       = "conter",
        ["provides"]      = "fornece",      ["provide"]       = "fornecer",
        ["processes"]     = "processa",     ["initializes"]   = "inicializa",
        ["initialize"]    = "inicializar",  ["initialises"]   = "inicializa",
        ["generates"]     = "gera",         ["generate"]      = "gerar",
        ["calculates"]    = "calcula",      ["calculate"]     = "calcular",
        ["extracts"]      = "extrai",       ["extract"]       = "extrair",
        ["transforms"]    = "transforma",   ["transform"]     = "transformar",
        ["resolves"]      = "resolve",      ["resolve"]       = "resolver",
        ["throws"]        = "lança",        ["throw"]         = "lançar",
        ["delegates"]     = "delega",       ["delegate"]      = "delegar",
        ["exposes"]       = "expõe",        ["expose"]        = "expor",
        ["renders"]       = "renderiza",    ["render"]        = "renderizar",
        ["sends"]         = "envia",        ["send"]          = "enviar",
        ["receives"]      = "recebe",       ["receive"]       = "receber",
        ["maps"]          = "mapeia",       ["map"]           = "mapear",
        ["groups"]        = "agrupa",       ["sorts"]         = "ordena",
        ["joins"]         = "une",          ["splits"]        = "divide",
        ["searches"]      = "busca",        ["search"]        = "buscar",
        // Advérbios
        ["recursively"]   = "recursivamente",
        ["automatically"] = "automaticamente",
        ["manually"]      = "manualmente",
        ["directly"]      = "diretamente",
        ["lazily"]        = "preguiçosamente",
        ["asynchronously"]= "assincronamente",
        ["synchronously"] = "sincronamente",
        // Preposições / conjunções / artigos
        ["the"]           = "",          // artigo omitido (gênero variável)
        ["from"]          = "de",        ["into"]    = "em",
        ["within"]        = "dentro de", ["without"] = "sem",
        ["between"]       = "entre",     ["through"] = "através de",
        ["onto"]          = "sobre",     ["when"]    = "quando",
        ["where"]         = "onde",      ["and"]     = "e",
        ["or"]            = "ou",        ["not"]     = "não",
        ["all"]           = "todos",     ["each"]    = "cada",
        ["its"]           = "seus",      ["their"]   = "seus",
        ["that"]          = "que",       ["which"]   = "que",
        ["this"]          = "este",      ["then"]    = "depois",
        ["with"]          = "com",       ["for"]     = "para",
        ["by"]            = "por",       ["of"]      = "de",
        ["in"]            = "em",        ["at"]      = "em",
        ["on"]            = "em",        ["as"]      = "como",
        ["an"]            = "",          ["a"]       = "",
        // Substantivos técnicos
        ["node"]          = "nó",        ["nodes"]        = "nós",
        ["tree"]          = "árvore",    ["hierarchy"]    = "hierarquia",
        ["history"]       = "histórico", ["ancestor"]     = "ancestral",
        ["descendant"]    = "descendente",["descendants"] = "descendentes",
        ["parent"]        = "pai",       ["child"]        = "filho",
        ["children"]      = "filhos",    ["prefix"]       = "prefixo",
        ["suffix"]        = "sufixo",    ["remote"]       = "remoto",
        ["remotes"]       = "remotos",   ["list"]         = "lista",
        ["grouping"]      = "agrupamento",["filter"]      = "filtro",
        ["entry"]         = "entrada",   ["entries"]      = "entradas",
        ["key"]           = "chave",     ["value"]        = "valor",
        ["values"]        = "valores",   ["index"]        = "índice",
        ["path"]          = "caminho",   ["root"]         = "raiz",
        ["leaf"]          = "folha",     ["depth"]        = "profundidade",
        ["level"]         = "nível",     ["order"]        = "ordem",
        ["result"]        = "resultado", ["output"]       = "saída",
        ["input"]         = "entrada",   ["type"]         = "tipo",
        ["name"]          = "nome",      ["label"]        = "rótulo",
        ["message"]       = "mensagem",  ["error"]        = "erro",
        ["event"]         = "evento",    ["item"]         = "item",
        ["items"]         = "itens",     ["element"]      = "elemento",
        ["elements"]      = "elementos", ["property"]     = "propriedade",
        ["properties"]    = "propriedades",["collection"] = "coleção",
        ["separator"]     = "separador", ["scope"]        = "escopo",
        ["context"]       = "contexto",  ["source"]       = "origem",
        ["target"]        = "destino",   ["state"]        = "estado",
        ["status"]        = "status",    ["mode"]         = "modo",
        ["format"]        = "formato",   ["pattern"]      = "padrão",
        ["rule"]          = "regra",     ["rules"]        = "regras",
        ["option"]        = "opção",     ["options"]      = "opções",
        ["flag"]          = "sinalizador",["flags"]       = "sinalizadores",
        ["limit"]         = "limite",    ["size"]         = "tamanho",
        ["count"]         = "contagem",  ["total"]        = "total",
        ["range"]         = "intervalo", ["step"]         = "passo",
        ["folder"]        = "pasta",     ["file"]         = "arquivo",
        ["task"]          = "tarefa",    ["tasks"]        = "tarefas",
        // Adjetivos
        ["based"]         = "baseado",   ["grouped"]      = "agrupado",
        ["sorted"]        = "ordenado",  ["filtered"]     = "filtrado",
        ["mapped"]        = "mapeado",   ["enabled"]      = "habilitado",
        ["disabled"]      = "desabilitado",["visible"]    = "visível",
        ["hidden"]        = "oculto",    ["selected"]     = "selecionado",
        ["required"]      = "obrigatório",["optional"]    = "opcional",
        ["created"]       = "criado",    ["nested"]       = "aninhado",
        ["merged"]        = "mesclado",  ["loaded"]       = "carregado",
    };

    // Palavras inglesas conhecidas — usadas para detectar idioma do comentário
    private static readonly HashSet<string> EnglishWords =
        new(WordTranslations.Keys, StringComparer.OrdinalIgnoreCase);

    // ── Tokens preservados na tradução (não traduzir) ─────────────────────────
    // Nomes de branch no padrão gitflow (feature/…, release/…, etc.) e os tipos
    // Conventional Commits. Sem esta proteção, a tradução palavra-a-palavra
    // corromperia slugs de branch (ex: "feature/search" → "feature/buscar").
    private const string PreservePattern =
        @"\b(?:feature|release|hotfix|bugfix|support|feat|fix|chore|docs|refactor)/[A-Za-z0-9._\-/]+" + // branches gitflow
        @"|\b(?:feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert)\b";                       // tipos CC

    private static readonly string[] TestPathSegments =
        ["test", "tests", "spec", "specs", "__tests__", "unittest", "unittests"];

    private static readonly HashSet<string> SkippableRoots =
        new(StringComparer.OrdinalIgnoreCase) { "src", "app", "lib", "source", "main", ".", "" };

    public CommitMessageGenerator(string workingDir, MessageLanguage language)
    {
        _workingDir = workingDir;
        _language   = language;
        _lang       = LanguagePack.For(language);
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    public string Generate()
    {
        var changes = GetStagedChanges();
        if (changes.Count == 0)
        {
            // O parse principal pode falhar (timeout do git, index.lock durante o
            // auto-refresh de stage/unstage, saída inesperada) MESMO havendo arquivos
            // em stage. Nesse caso NUNCA retornamos vazio: devolvemos ao menos a
            // contagem de arquivos afetados. Se realmente nada estiver em stage, a
            // contagem vem 0 e a mensagem vazia é o correto (não há o que commitar).
            var staged = CountStagedFiles();
            return staged > 0
                ? TruncateTitle($"{_lang.TypeVerb(string.Empty, false, false, false)} {staged} {_lang.FilesWord(staged)}")
                : string.Empty;
        }

        var types = DetermineAllTypes(changes);

        // Primeira linha (log): verbo CC do tipo dominante + total de arquivos + tipos envolvidos.
        var title = BuildConsolidatedTitle(types[0], changes, types);

        // Corpo: até 5 linhas dos arquivos de maior impacto. Cada linha vem do comentário
        // do diff (traduzido para pt-BR) ou do título do README; na falta, do conceito do nome.
        var commentsByFile = ExtractCommentsByFile();
        var readmeTitle    = ReadStagedReadmeTitle(changes);
        var body           = BuildBody(changes, commentsByFile, readmeTitle);

        var message = body.Length > 0 ? $"{title}\n\n{body}" : title;

        // Garantia final: havendo mudanças em stage, NUNCA retornar vazio.
        // Se por qualquer caminho de borda o resultado vier em branco, devolve
        // ao menos a linha-resumo (verbo do tipo dominante + contagem de arquivos).
        return string.IsNullOrWhiteSpace(message)
            ? TruncateTitle($"{TypeVerb(types[0], changes)} {changes.Count} {_lang.FilesWord(changes.Count)}")
            : message;
    }

    /// <summary>
    /// Primeira linha do commit, em formato de log consolidado: verbo imperativo do
    /// tipo CC dominante + total de arquivos alterados + lista de todos os tipos envolvidos.
    /// Ex.: "Adiciona 6 arquivos (feat, fix, docs)".
    /// </summary>
    private string BuildConsolidatedTitle(string type, List<FileChange> changes, List<string> types)
    {
        var verb     = TypeVerb(type, changes);
        var fileWord = _lang.FilesWord(changes.Count);
        var typeList = string.Join(", ", types);
        return TruncateTitle($"{verb} {changes.Count} {fileWord} ({typeList})");
    }

    /// <summary>
    /// Lê o título (primeira linha com #) do README.md staged, se houver.
    /// </summary>
    private string? ReadStagedReadmeTitle(List<FileChange> changes)
    {
        var readme = changes.FirstOrDefault(c =>
            c.Status != 'D' &&
            Path.GetFileName(c.Path).Equals("README.md", StringComparison.OrdinalIgnoreCase));

        if (readme is null) return null;

        var fullPath = Path.Combine(_workingDir, readme.Path.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath)) return null;

        foreach (var line in File.ReadLines(fullPath))
        {
            if (line.StartsWith('#'))
            {
                var title = line.TrimStart('#').Trim();
                if (string.IsNullOrEmpty(title)) return null;
                // Ignora títulos que são apenas nomes de projeto/repositório (sem espaço ou com ponto)
                if (!title.Contains(' ') || title.Contains('.')) return null;
                return title;
            }
        }

        return null;
    }

    // ── Extração de comentários do diff ───────────────────────────────────────

    /// <summary>
    /// Lê git diff --cached e agrupa os comentários alterados (linhas + e -) por arquivo.
    /// A priorização por impacto entre arquivos é feita depois, em BuildBody.
    /// </summary>
    private Dictionary<string, List<string>> ExtractCommentsByFile()
    {
        var diff   = RunGit("diff", "--cached", "--no-color");
        var byFile = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var seen   = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string? currentFile = null;
        bool isMdFile = false;

        foreach (var line in diff.Split('\n'))
        {
            // Detecta o arquivo atual: "+++ b/caminho"
            if (line.StartsWith("+++ b/"))
            {
                currentFile = line[6..].Trim();
                // Em .md, headings "# Texto" são estrutura Markdown, não comentários de código
                isMdFile = currentFile.EndsWith(".md", StringComparison.OrdinalIgnoreCase);
                continue;
            }
            if (currentFile is null) continue;

            // Processa linhas adicionadas (+) e removidas (-); ignora cabeçalhos +++/---
            bool isAdded   = line.Length >= 2 && line[0] == '+' && !line.StartsWith("+++");
            bool isRemoved = line.Length >= 2 && line[0] == '-' && !line.StartsWith("---");
            if (!isAdded && !isRemoved) continue;

            var text = ExtractCommentText(line[1..].TrimStart(), isMdFile);
            if (text is null || !seen.Add(text)) continue;

            if (!byFile.TryGetValue(currentFile, out var list))
                byFile[currentFile] = list = [];
            list.Add(text);
        }

        return byFile;
    }

    /// <summary>
    /// Melhor comentário para descrever um arquivo: traduz para pt-BR (descartando
    /// traduções insuficientes), escolhe o mais descritivo (maior) e recorta a cláusula
    /// principal. Para o README, usa o título como fallback. Retorna null quando não há
    /// texto útil — BuildBody então deriva a descrição do nome do arquivo.
    /// </summary>
    private string? BestComment(string path, Dictionary<string, List<string>> byFile, string? readmeTitle)
    {
        var raw = byFile.TryGetValue(path, out var list) ? list : [];
        var usable = _language == MessageLanguage.PtBr
            ? raw.Select(TranslateToPortuguese).OfType<string>()
            : raw;

        // Mantém só frases "fechadas" e escolhe a de maior score (não a mais longa).
        var best = usable.Where(IsCleanSentence)
                         .OrderByDescending(ScoreCandidate)
                         .ThenByDescending(c => c.Length)
                         .FirstOrDefault();

        if (best is null && readmeTitle is not null &&
            Path.GetFileName(path).Equals("README.md", StringComparison.OrdinalIgnoreCase) &&
            IsCleanSentence(readmeTitle))
            best = readmeTitle;

        return best is null ? null : ExtractMainClause(best);
    }

    /// <summary>
    /// Prioridade do arquivo para ranquear os comentários:
    /// maior = mais impactante → aparece na primeira linha do commit.
    /// </summary>
    private static int CommentFilePriority(string path)
    {
        // Arquivos de teste têm a menor prioridade
        var lower = path.ToLowerInvariant();
        if (lower.Contains("/test/") || lower.Contains("/tests/") ||
            lower.Contains("/spec/") || lower.EndsWith("test.cs") ||
            lower.EndsWith("tests.cs") || lower.Contains(".test.") ||
            lower.Contains(".spec."))
            return 0;

        return GetCategory(path) switch
        {
            "source" => 4,
            "web"    => 3,
            "build"  => 2,
            "config" => 1,
            "docs"   => 1,
            _        => 2
        };
    }

    /// <summary>Tenta extrair texto de comentário de uma linha de código.</summary>
    private static string? ExtractCommentText(string content, bool isMdFile = false)
    {
        string? raw = null;

        // C# linha única: // ou ///
        var m = Regex.Match(content, @"^\/\/\/?\s+(.+)");
        if (m.Success) raw = m.Groups[1].Value;

        // Python / shell / YAML: # texto
        // Em arquivos .md o padrão "# Texto" é heading Markdown, não comentário — ignorar
        if (raw is null && !isMdFile)
        {
            m = Regex.Match(content, @"^#+\s+([A-Za-zÀ-ú].+)");
            if (m.Success) raw = m.Groups[1].Value;
        }

        return raw is null ? null : CleanCommentText(raw);
    }

    /// <summary>
    /// Valida e limpa o texto extraído de um comentário.
    /// Retorna null para separadores visuais, código comentado e tags XML.
    /// </summary>
    private static string? CleanCommentText(string raw)
    {
        var text = raw.Trim().TrimEnd('.', ':', ';');

        if (text.Length < 10)  return null;   // muito curto
        if (!text.Contains(' ')) return null;  // sem espaço = provavelmente não é frase

        // Separadores visuais (── , ===, ---)
        if (Regex.IsMatch(text, @"^[\-─═=\s]+$")) return null;
        if (text.Count(c => c is '─' or '-' or '=') > text.Length / 3) return null;

        // Tags de documentação XML
        if (text.TrimStart().StartsWith('<')) return null;

        // Código comentado: tem chaves ou chamada de método (palavra seguida de parênteses)
        if (text.Contains('{') || text.Contains('}')) return null;
        if (Regex.IsMatch(text, @"\w+\([^)]*\)")) return null;

        // Delimitadores desbalanceados: abertura sem fechamento (ou o inverso).
        // Frases assim — "monta a árvore (recursivo" — saem sem sentido no commit.
        if (!DelimitersBalanced(text)) return null;

        return text;
    }

    /// <summary>
    /// true se todos os delimitadores estão balanceados: parênteses, colchetes e
    /// chaves casados e na ordem certa; aspas duplas ("), crases (`) e aspas simples (')
    /// em número par; e quantidade igual de &lt; e &gt;. Pega o caso "tem abertura mas
    /// falta o fechamento" (ou vice-versa). Os sinais de menor/maior são contados (não
    /// empilhados) para não confundir comparações em prosa; as aspas simples entre letras
    /// (apóstrofes de contração: "don't", "it's") são ignoradas, contando só as de delimitação.
    /// </summary>
    private static bool DelimitersBalanced(string text)
    {
        int paren = 0, bracket = 0, brace = 0;
        foreach (var ch in text)
        {
            switch (ch)
            {
                case '(': paren++;   break;
                case ')': if (--paren   < 0) return false; break;
                case '[': bracket++; break;
                case ']': if (--bracket < 0) return false; break;
                case '{': brace++;   break;
                case '}': if (--brace   < 0) return false; break;
            }
        }
        if (paren != 0 || bracket != 0 || brace != 0) return false;
        if (text.Count(c => c == '"')  % 2 != 0)                 return false;
        if (text.Count(c => c == '`')  % 2 != 0)                 return false;
        if (text.Count(c => c == '<') != text.Count(c => c == '>')) return false;

        // Aspas simples como delimitador: conta só as que NÃO estão entre duas letras,
        // para não confundir apóstrofes de contração inglesa ("don't", "it's").
        int singleQuotes = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] != '\'') continue;
            bool insideWord = i > 0 && i < text.Length - 1 &&
                              char.IsLetter(text[i - 1]) && char.IsLetter(text[i + 1]);
            if (!insideWord) singleQuotes++;
        }
        if (singleQuotes % 2 != 0) return false;

        return true;
    }

    /// <summary>
    /// true se a frase está "fechada" o bastante para virar uma linha de commit:
    /// delimitadores balanceados E não termina em palavra de ligação solta
    /// (preposição/artigo/conjunção), que denuncia um comentário cortado.
    /// </summary>
    private bool IsCleanSentence(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        if (!DelimitersBalanced(text))       return false;

        var lastWord = text.TrimEnd('.', ',', ';', ':', ' ', '\t')
                           .Split(' ', '\t', StringSplitOptions.RemoveEmptyEntries)
                           .LastOrDefault();
        return lastWord is null || !_lang.DanglingTrailingWords.Contains(lastWord);
    }

    /// <summary>
    /// Pontua um candidato a descrição: premia comprimento próximo do ideal
    /// (~20–72 chars) e frases de ação (começam com verbo conhecido); penaliza
    /// resíduo de código (=, ;) e espaços duplos. Substitui o critério antigo de
    /// "pega o mais comprido", que tendia a escolher justamente o mais truncado.
    /// </summary>
    private int ScoreCandidate(string text)
    {
        int len = text.Length;
        int score = len is >= 20 and <= 72 ? 25 : -Math.Abs(46 - len) / 3;
        if (_lang.LeadingVerb(NormalizeDesc(text)).Verb is not null) score += 15;
        if (text.Contains('=') || text.Contains(';')) score -= 10;
        if (text.Contains("  ", StringComparison.Ordinal)) score -= 5;
        return score;
    }

    /// <summary>
    /// Retorna o comentário completo, preservando a frase inteira.
    /// O corte da "cláusula principal" no primeiro conector de propósito
    /// (" para ", " pois "…) foi desativado porque encurtava as frases e as
    /// deixava sem sentido (ex.: "filtrar stems com ponto" perdia o propósito).
    /// </summary>
    private string ExtractMainClause(string comment) => comment;

    /// <summary>
    /// Normaliza um comentário para uso como descrição: 1ª letra minúscula,
    /// exceto quando a palavra inicial é um acrônimo em MAIÚSCULAS (ex: "HTML", "API").
    /// </summary>
    private static string NormalizeDesc(string text)
    {
        // Preserva maiúsculas quando a palavra inicial é acrônimo (≥2 letras maiúsculas seguidas)
        if (text.Length >= 2 && char.IsUpper(text[0]) && char.IsUpper(text[1]))
            return text;
        return char.ToLowerInvariant(text[0]) + text[1..];
    }

    /// <summary>
    /// Detecta se um texto está predominantemente em inglês.
    /// Critério: ≥25 % das palavras (minúsculas, ≥3 letras) estão no dicionário de tradução.
    /// </summary>
    private static bool IsEnglishText(string text)
    {
        var words = Regex.Matches(text.ToLowerInvariant(), @"\b[a-z]{3,}\b")
                         .Select(m => m.Value)
                         .ToList();
        if (words.Count < 3) return false;
        var englishCount = words.Count(w => EnglishWords.Contains(w));
        return (double)englishCount / words.Count >= 0.25;
    }

    /// <summary>
    /// Traduz um comentário do inglês para pt-BR usando frases e palavras mapeadas.
    /// Preserva identificadores PascalCase/MAIÚSCULAS (código), nomes de branch
    /// (feature/…, release/…, etc.) e os tipos Conventional Commits.
    /// Retorna null quando a tradução é insuficiente (fallback pt-BR será usado).
    /// </summary>
    private static string? TranslateToPortuguese(string text)
    {
        if (!IsEnglishText(text)) return text;  // já em pt-BR

        // Mascara nomes de branch e tipos CC com placeholders N
        // (sem letras → as fases de tradução os ignoram). Restaurados no fim.
        var preserved = new List<string>();
        var result = Regex.Replace(text, PreservePattern,
            m => { preserved.Add(m.Value); return $"{preserved.Count - 1}"; },
            RegexOptions.IgnoreCase);

        // Fase 1 — frases compostas (mais longas primeiro para evitar fragmentação)
        foreach (var (en, pt) in PhraseTranslations)
            result = Regex.Replace(result, $@"(?i){Regex.Escape(en)}", pt);

        // Fase 2 — padrões estruturais
        // "X-based" → "baseado em X"
        result = Regex.Replace(result, @"\b(\w+)-based\b",
            m => $"baseado em {m.Groups[1].Value.ToLowerInvariant()}", RegexOptions.IgnoreCase);
        // "recursively VERBO" → "VERBO recursivamente"
        result = Regex.Replace(result, @"\brecursively\s+(\w+)",
            m =>
            {
                var verb = m.Groups[1].Value;
                var pt   = WordTranslations.TryGetValue(verb, out var v) ? v : verb;
                return $"{pt} recursivamente";
            }, RegexOptions.IgnoreCase);

        // Fase 3 — palavras individuais (preserva PascalCase = identificadores de código)
        result = Regex.Replace(result, @"\b[A-Za-z][a-z]*\b",
            m =>
            {
                var word = m.Value;
                // Preserva PascalCase: primeira maiúscula + tem minúsculas = identificador
                if (char.IsUpper(word[0]) && word.Length > 1 && word.Any(char.IsLower))
                    return word;
                return WordTranslations.TryGetValue(word, out var pt) ? pt : word;
            });

        // Limpeza: remove espaços duplos gerados pela remoção de artigos ("the" → "")
        result = Regex.Replace(result, @"\s{2,}", " ").Trim().TrimStart(',', ';');

        // Avalia qualidade ANTES de restaurar: tokens preservados (branches/tipos CC)
        // não devem contar como "inglês não traduzido".
        if (IsEnglishText(result)) return null;  // descarta — fallback pt-BR será usado

        // Restaura nomes de branch e tipos CC, intactos
        if (preserved.Count > 0)
            result = Regex.Replace(result, "(\\d+)",
                m => preserved[int.Parse(m.Groups[1].Value)]);

        return result;
    }

    // ── Step 1 — Parse git diff --name-status ─────────────────────────────────

    private static List<FileChange> ParseChanges(string output)
    {
        var list = new List<FileChange>();
        foreach (var line in output.Split('\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = line.Split('\t');
            if (parts.Length < 2) continue;
            var status = parts[0].TrimStart()[0];
            var path   = parts[^1].Replace('\\', '/');
            list.Add(new FileChange(status, path));
        }
        return list;
    }

    // ── Step 2 — Conventional Commits type ────────────────────────────────────

    /// <summary>Tipo Conventional Commits de um único arquivo (caminho/categoria/status).</summary>
    private static string TypeOf(FileChange change)
    {
        if (IsTestPath(change)) return "test";

        return GetCategory(change.Path) switch
        {
            "docs"   => "docs",
            "build"  => "build",
            "config" => "chore",
            _ => change.Status switch
            {
                'A' or 'C'        => "feat",
                'D'               => "chore",
                'M' or 'R' or 'T' => "fix",
                _                 => "refactor"
            }
        };
    }

    /// <summary>
    /// Retorna todos os CC types envolvidos nas mudanças, ordenados por prioridade convencional.
    /// </summary>
    private static List<string> DetermineAllTypes(List<FileChange> changes)
    {
        var types = new HashSet<string>(changes.Select(TypeOf), StringComparer.Ordinal);
        var order = new[] { "feat", "fix", "refactor", "perf", "test", "build", "ci", "chore", "docs", "style" };
        return [.. types.OrderBy(t => Array.IndexOf(order, t) is var i && i >= 0 ? i : 99)];
    }

    // ── Step 3 — Subject: descrição funcional em pt-BR ────────────────────────

    // ── Verbo imperativo (idioma-específico) ───────────────────────────────────

    /// <summary>
    /// Mapeia o tipo CC + contexto das mudanças para o verbo imperativo no idioma ativo.
    /// </summary>
    private string TypeVerb(string type, List<FileChange> changes)
    {
        bool hasDeletions  = changes.Any(c => c.Status == 'D');
        bool hasAdditions  = changes.Any(c => c.Status is 'A' or 'C');
        bool onlyAdditions = changes.All(c => c.Status is 'A' or 'C');
        return _lang.TypeVerb(type, onlyAdditions, hasAdditions, hasDeletions);
    }

    // ── Step 4 — Corpo: até 5 linhas dos arquivos de maior impacto ──────────────

    /// <summary>
    /// Monta o corpo: ordena os arquivos por impacto (CommentFilePriority) e descreve
    /// um por linha. Os de maior impacto vêm primeiro; quando não geram texto útil, os
    /// de menor impacto preenchem as linhas restantes (até 5, sem repetir).
    /// </summary>
    private string BuildBody(List<FileChange> changes,
        Dictionary<string, List<string>> commentsByFile, string? readmeTitle)
    {
        var lines = new List<string>();
        var seen  = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var c in changes.OrderByDescending(c => CommentFilePriority(c.Path)))
        {
            var line = FormatFileLine(c, BestComment(c.Path, commentsByFile, readmeTitle));
            if (line.Length == 0 || !seen.Add(line)) continue;

            lines.Add(line);
            if (lines.Count == 5) break;
        }

        return string.Join("\n", lines.Select(l => $"- {l}"));
    }

    /// <summary>
    /// Descreve um arquivo em uma linha. Com comentário: detecta o verbo inicial ou
    /// prefixa o verbo do tipo CC do arquivo. Sem comentário: deriva o conceito do nome,
    /// prefixado pelo verbo de status (Adiciona/Remove/Renomeia/Atualiza).
    /// Retorna "" quando não há nada útil a dizer sobre o arquivo.
    /// </summary>
    private string FormatFileLine(FileChange change, string? desc)
    {
        if (desc is { Length: > 0 } && IsCleanSentence(desc))
        {
            var normalized = NormalizeDesc(desc);

            var (verb, remainder) = _lang.LeadingVerb(normalized);
            if (verb is not null)
                return remainder.Length > 0 ? $"{verb} {remainder}" : verb;

            var typeVerb = _lang.TypeVerb(TypeOf(change),
                change.Status is 'A' or 'C', change.Status is 'A' or 'C', change.Status == 'D');
            return $"{typeVerb} {normalized}";
        }

        var raw = ExtractRawConcept(Path.GetFileNameWithoutExtension(change.Path));
        return raw is null ? string.Empty : $"{_lang.StatusVerb(change.Status)} {MapConcept(raw)}";
    }

    // ── Concept extraction ─────────────────────────────────────────────────────

    private string? ExtractRawConcept(string filename)
    {
        // Stems com ponto são nomes de assembly/projeto (ex: GitExtensions.ZimerfeldCommitMsg) — ignorar
        if (filename.Contains('.')) return null;

        // Filenames com caracteres não-ASCII (emoji, acentos fora do padrão PascalCase) — ignorar
        if (filename.Any(c => c > 127)) return null;

        var name = filename;

        // Remove prefixo "I" de interface: IUserService → UserService
        if (name.Length > 2 && name[0] == 'I' && char.IsUpper(name[1]))
            name = name[1..];

        // Remove o sufixo arquitetural mais longo que corresponder
        foreach (var suffix in SemanticSuffixes)
        {
            if (name.Length > suffix.Length + 1 &&
                name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^suffix.Length];
                break;
            }
        }

        if (name.Length < 2) return null;

        // Se não está no dicionário e tem mais de 2 palavras PascalCase,
        // é provavelmente um nome de projeto/namespace — ignorar
        if (!_lang.HasConcept(name))
        {
            var wordCount = Regex.Matches(name, @"[A-Z][a-z]+").Count;
            if (wordCount > 2) return null;
        }

        return name;
    }

    private string MapConcept(string raw) => _lang.MapConcept(raw, HumanizeName);

    // ── Generic helpers ────────────────────────────────────────────────────────

    private static string GetCategory(string path)
    {
        var ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
        if (ext.Length == 0)
        {
            var name = Path.GetFileName(path).ToLowerInvariant();
            if (name is "dockerfile" or "makefile" or "jenkinsfile" or "vagrantfile")
                return "build";
            if (name is ".env" or ".gitignore" or ".gitattributes" or ".editorconfig")
                return "config";
        }
        return ExtCategory.TryGetValue(ext, out var cat) ? cat : "source";
    }

    private static bool IsTestPath(FileChange c)
    {
        var segments = c.Path.ToLowerInvariant()
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Any(s => TestPathSegments.Contains(s))) return true;
        var fn = segments[^1];
        return fn.Contains(".test.") || fn.Contains(".spec.") ||
               fn.EndsWith("tests.cs") || fn.EndsWith("test.cs");
    }

    /// <summary>
    /// PascalCase → palavras humanizadas. Acrônimos em MAIÚSCULAS são preservados.
    /// Ex: "UserAuthService" → "user auth service"; "HTMLParser" → "HTML parser".
    /// </summary>
    private static string HumanizeName(string name)
    {
        var spaced = Regex.Replace(name, @"([a-z])([A-Z])", "$1 $2")
                          .Replace('-', ' ')
                          .Replace('_', ' ');
        var normalized = Regex.Replace(spaced, @"\s+", " ").Trim();
        // Preserva segmentos em MAIÚSCULAS (acrônimos: ≥2 chars, todos maiúsculos)
        return string.Join(" ", normalized.Split(' ')
            .Select(w => w.Length >= 2 && w.All(char.IsUpper) ? w : w.ToLowerInvariant()));
    }

    // Garante que o título do commit respeita o limite de 80 chars
    private static string TruncateTitle(string title, int maxLen = 80)
    {
        if (title.Length <= maxLen) return title;
        var cut = title.LastIndexOf(' ', maxLen - 2);
        return cut > 8 ? title[..cut] + "…" : title[..(maxLen - 1)] + "…";
    }

    // ── Detecção dos arquivos staged (resiliente) ───────────────────────────────

    /// <summary>
    /// Lê e parseia os arquivos staged via <c>git diff --cached --name-status</c>,
    /// com retentativa em falha transitória (ex.: index.lock durante stage/unstage).
    /// </summary>
    private List<FileChange> GetStagedChanges()
    {
        var nameStatus = RunGitResilient("diff", "--cached", "--name-status");
        return string.IsNullOrWhiteSpace(nameStatus) ? [] : ParseChanges(nameStatus);
    }

    /// <summary>
    /// Conta os arquivos staged de forma robusta, base do fallback "nunca vazio".
    /// Tenta <c>--name-only</c> e, em último caso, <c>status --porcelain</c>
    /// (entradas com mudança staged na 1ª coluna). Retorna 0 quando nada está em stage.
    /// </summary>
    private int CountStagedFiles()
    {
        var nameOnly = RunGitResilient("diff", "--cached", "--name-only");
        var count    = CountNonEmptyLines(nameOnly);
        if (count > 0) return count;

        // Último recurso: porcelain — coluna 1 (staged) diferente de espaço e de '?'.
        var porcelain = RunGitResilient("status", "--porcelain");
        if (string.IsNullOrWhiteSpace(porcelain)) return 0;
        return porcelain.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Count(l => l.Length >= 2 && l[0] != ' ' && l[0] != '?');
    }

    private static int CountNonEmptyLines(string text) =>
        string.IsNullOrWhiteSpace(text)
            ? 0
            : text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;

    // ── Git subprocess ─────────────────────────────────────────────────────────

    /// <summary>
    /// Executa git e retenta apenas quando há FALHA (erro/timeout) — não quando o git
    /// conclui com sucesso e saída vazia (ex.: nada em stage), evitando latência no
    /// caso comum. Cobre o index.lock transitório do auto-refresh.
    /// </summary>
    private string RunGitResilient(params string[] args)
    {
        for (int attempt = 0; ; attempt++)
        {
            var (output, ok) = RunGitChecked(args);
            if (ok || attempt >= 2) return output;   // sucesso, ou esgotou as tentativas
            Thread.Sleep(120);                         // falha transitória → tenta de novo
        }
    }

    private string RunGit(params string[] args) => RunGitChecked(args).Output;

    /// <summary>Executa git; devolve a saída e se o processo concluiu com sucesso (exit 0).</summary>
    private (string Output, bool Ok) RunGitChecked(string[] args)
    {
        try
        {
            var psi = new ProcessStartInfo("git")
            {
                WorkingDirectory       = _workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            foreach (var a in args) psi.ArgumentList.Add(a);
            using var p = Process.Start(psi)!;
            var output  = p.StandardOutput.ReadToEnd();
            if (!p.WaitForExit(8_000))
            {
                try { p.Kill(entireProcessTree: true); } catch { /* best-effort */ }
                return (output, false);                // timeout = falha → permite retentativa
            }
            return (output, p.ExitCode == 0);
        }
        catch { return (string.Empty, false); }
    }
}

internal sealed record FileChange(char Status, string Path);
