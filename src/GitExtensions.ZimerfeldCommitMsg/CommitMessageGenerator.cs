using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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

    // ── Domain concept → frase em pt-BR ───────────────────────────────────────
    private static readonly Dictionary<string, string> ConceptPhrases = new(StringComparer.OrdinalIgnoreCase)
    {
        // Identidade / acesso
        ["Auth"]           = "autenticação",
        ["Authentication"] = "autenticação",
        ["Login"]          = "login",
        ["Logout"]         = "logout",
        ["SignIn"]         = "login",
        ["SignUp"]         = "cadastro",
        ["Register"]       = "cadastro",
        ["Registration"]   = "cadastro",
        ["Password"]       = "gerenciamento de senha",
        ["Token"]          = "gerenciamento de token",
        ["Jwt"]            = "autenticação JWT",
        ["Bearer"]         = "autenticação por token",
        ["Session"]        = "gerenciamento de sessão",
        ["Permission"]     = "permissões",
        ["Permissions"]    = "permissões",
        ["Role"]           = "controle de acesso por papel",
        ["Roles"]          = "controle de acesso por papel",
        ["Claim"]          = "claims",
        ["OAuth"]          = "integração OAuth",
        // Usuário / conta
        ["User"]           = "gerenciamento de usuários",
        ["Users"]          = "gerenciamento de usuários",
        ["Account"]        = "gerenciamento de conta",
        ["Profile"]        = "perfil do usuário",
        ["Member"]         = "associação",
        ["Customer"]       = "gerenciamento de clientes",
        ["Tenant"]         = "multi-tenancy",
        // Comércio
        ["Order"]          = "processamento de pedidos",
        ["Cart"]           = "carrinho de compras",
        ["Checkout"]       = "fluxo de checkout",
        ["Payment"]        = "processamento de pagamento",
        ["Invoice"]        = "gerenciamento de faturas",
        ["Product"]        = "gerenciamento de produtos",
        ["Catalog"]        = "catálogo de produtos",
        ["Inventory"]      = "estoque",
        ["Shipping"]       = "frete",
        ["Discount"]       = "gerenciamento de descontos",
        ["Coupon"]         = "gerenciamento de cupons",
        ["Subscription"]   = "assinaturas",
        // Comunicação
        ["Email"]          = "serviço de e-mail",
        ["Mail"]           = "serviço de e-mail",
        ["Sms"]            = "notificações SMS",
        ["Notification"]   = "notificações",
        ["Push"]           = "notificações push",
        ["Webhook"]        = "webhooks",
        ["Message"]        = "mensagens",
        ["Chat"]           = "chat",
        // Infraestrutura
        ["Cache"]          = "cache",
        ["Log"]            = "registro de log",
        ["Logger"]         = "registro de log",
        ["Logging"]        = "registro de log",
        ["Audit"]          = "trilha de auditoria",
        ["Health"]         = "verificação de saúde",
        ["Metric"]         = "métricas",
        ["Monitor"]        = "monitoramento",
        ["Queue"]          = "fila de mensagens",
        ["Job"]            = "tarefas em segundo plano",
        ["Scheduler"]      = "agendamento de tarefas",
        ["Worker"]         = "workers em segundo plano",
        ["Event"]          = "tratamento de eventos",
        // Dados
        ["Migration"]      = "migração de banco de dados",
        ["Seed"]           = "população de dados",
        ["Database"]       = "acesso ao banco de dados",
        ["Db"]             = "banco de dados",
        ["Storage"]        = "armazenamento",
        ["File"]           = "gerenciamento de arquivos",
        ["Upload"]         = "upload de arquivos",
        ["Download"]       = "download de arquivos",
        ["Blob"]           = "armazenamento de blobs",
        // Relatórios
        ["Report"]         = "relatórios",
        ["Dashboard"]      = "painel",
        ["Analytics"]      = "análises",
        ["Export"]         = "exportação de dados",
        ["Import"]         = "importação de dados",
        ["Pdf"]            = "geração de PDF",
        ["Excel"]          = "exportação para Excel",
        // Busca
        ["Search"]         = "busca",
        ["Filter"]         = "filtragem",
        // Configuração
        ["Settings"]       = "configurações",
        ["Config"]         = "configuração",
        ["AppSettings"]    = "configurações da aplicação",
        // API
        ["Api"]            = "API",
        ["Rest"]           = "API REST",
        ["Grpc"]           = "serviço gRPC",
        ["GraphQL"]        = "GraphQL",
        ["Swagger"]        = "documentação da API",
        ["Cors"]           = "política CORS",
        // Testes
        ["Test"]           = "testes unitários",
        ["Tests"]          = "testes unitários",
        ["Integration"]    = "testes de integração",
        ["E2E"]            = "testes end-to-end",
        // Docs
        ["Readme"]         = "documentação",
        ["Changelog"]      = "changelog",
        ["Docs"]           = "documentação",
        // Plugin / commit
        ["CommitMessage"]  = "mensagem de commit",
        ["CommitMsg"]      = "mensagem de commit",
        ["Plugin"]         = "plugin",
        ["GitExtension"]   = "extensão do Git",
    };

    // ── Architectural layers detected from file suffixes ───────────────────────
    private static readonly Dictionary<string, string> ArchLayers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Controller"]  = "controlador",  ["Controllers"]  = "controlador",
        ["Service"]     = "serviço",      ["Services"]     = "serviço",
        ["Repository"]  = "repositório",  ["Repositories"] = "repositório",
        ["Manager"]     = "gerenciador",  ["Handler"]      = "handler",
        ["Middleware"]  = "middleware",    ["Validator"]    = "validador",
        ["Mapper"]      = "mapeador",     ["Factory"]      = "fábrica",
        ["Tests"]       = "teste",        ["Test"]         = "teste",
        ["Dto"]         = "DTO",          ["ViewModel"]    = "view model",
        ["Entity"]      = "entidade",     ["Model"]        = "modelo",
        ["Generator"]   = "gerador",      ["Generators"]   = "gerador",
    };

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

    private static readonly string[] TestPathSegments =
        ["test", "tests", "spec", "specs", "__tests__", "unittest", "unittests"];

    private static readonly HashSet<string> SkippableRoots =
        new(StringComparer.OrdinalIgnoreCase) { "src", "app", "lib", "source", "main", ".", "" };

    public CommitMessageGenerator(string workingDir) => _workingDir = workingDir;

    // ── Public API ─────────────────────────────────────────────────────────────

    public string Generate()
    {
        var nameStatus = RunGit("diff", "--cached", "--name-status");
        if (string.IsNullOrWhiteSpace(nameStatus)) return string.Empty;

        var changes = ParseChanges(nameStatus);
        if (changes.Count == 0) return string.Empty;

        var type     = DetermineType(changes);
        // Traduz comentários ingleses para pt-BR; descarta apenas quando a tradução é insuficiente
        var comments = ExtractDiffComments()
            .Select(TranslateToPortuguese)
            .OfType<string>()
            .ToList();

        string desc, body;

        var readmeTitle = ReadStagedReadmeTitle(changes);

        if (readmeTitle is not null)
        {
            desc = readmeTitle;
            var bodyComments = comments.Count > 0
                ? string.Join("\n", comments.Select(c => $"- {NormalizeDesc(c)}"))
                : BuildBody(changes);
            body = bodyComments;
        }
        else if (comments.Count > 0)
        {
            // Sujeito: cláusula principal do primeiro comentário (antes de conectores de propósito)
            var mainClause = ExtractMainClause(comments[0]);
            desc = NormalizeDesc(mainClause);

            // Corpo: demais comentários como marcadores
            // Quando o sujeito foi abreviado, o primeiro comentário aparece completo no corpo (para contexto);
            // caso contrário não é repetido, pois já é a própria descrição.
            bool wasShortened = mainClause.Length < comments[0].Length;
            var bodyComments = wasShortened ? comments : comments.Skip(1).ToList();
            body = bodyComments.Count > 0
                ? string.Join("\n", bodyComments.Select(c => $"- {NormalizeDesc(c)}"))
                : BuildBody(changes);
        }
        else
        {
            // Fallback: derivar da análise de nomes de arquivo
            desc = BuildSubject(type, changes);
            body = BuildBody(changes);
        }

        var header = TruncateTitle($"{type}: {desc}");
        return body.Length > 0 ? $"{header}\n\n{body}" : header;
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
                return string.IsNullOrEmpty(title) ? null : title;
            }
        }

        return null;
    }

    // ── Extração de comentários do diff ───────────────────────────────────────

    /// <summary>
    /// Lê git diff --cached e coleta comentários alterados (linhas + e -).
    /// Linhas adicionadas (+): prioridade = tipo do arquivo (source=4, web=3, …).
    /// Linhas removidas  (-): prioridade = tipo do arquivo − 1 (contexto do que mudou).
    /// Retorna ordenado por prioridade → comentário mais impactante primeiro.
    /// </summary>
    private List<string> ExtractDiffComments()
    {
        var diff    = RunGit("diff", "--cached", "--no-color");
        var buckets = new SortedDictionary<int, List<string>>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        var seen    = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int total   = 0;
        int filePriority = 2;

        foreach (var line in diff.Split('\n'))
        {
            // Detecta o arquivo atual: "+++ b/caminho"
            if (line.StartsWith("+++ b/"))
            {
                filePriority = CommentFilePriority(line[6..].Trim());
                continue;
            }

            // Processa linhas adicionadas (+) e removidas (-); ignora cabeçalhos +++/---
            bool isAdded   = line.Length >= 2 && line[0] == '+' && !line.StartsWith("+++");
            bool isRemoved = line.Length >= 2 && line[0] == '-' && !line.StartsWith("---");
            if (!isAdded && !isRemoved) continue;

            var text = ExtractCommentText(line[1..].TrimStart());
            if (text is null || !seen.Add(text)) continue;

            // Linhas removidas têm prioridade um grau menor que as adicionadas —
            // assim só dominam quando não há comentários adicionados do mesmo nível
            int priority = isAdded ? filePriority : Math.Max(0, filePriority - 1);

            if (!buckets.TryGetValue(priority, out var list))
                buckets[priority] = list = [];

            list.Add(text);
            if (++total >= 15) break;
        }

        // Dentro do mesmo nível de prioridade, comentários mais longos primeiro
        // (comprimento como proxy do impacto/valor descritivo da alteração)
        return buckets.Values
            .SelectMany(l => l.OrderByDescending(c => c.Length))
            .Take(5)
            .ToList();
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
    private static string? ExtractCommentText(string content)
    {
        string? raw = null;

        // C# linha única: // ou ///
        var m = Regex.Match(content, @"^\/\/\/?\s+(.+)");
        if (m.Success) raw = m.Groups[1].Value;

        // Python / shell / YAML: # texto
        if (raw is null)
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

        return text;
    }

    /// <summary>
    /// Extrai a cláusula principal de um comentário, descartando justificativas introduzidas
    /// por conectores como " para ", " pois ", " porque ", " — ", etc.
    /// Exemplo: "filtrar stems com ponto para evitar nomes de assembly"
    ///       →  "filtrar stems com ponto"
    /// </summary>
    private static string ExtractMainClause(string comment)
    {
        ReadOnlySpan<string> connectors =
        [
            " para ", " pois ", " porque ", " já que ", " a fim de ",
            " quando ", " caso ", " evitando ", " — ", " - "
        ];
        foreach (var conn in connectors)
        {
            var idx = comment.IndexOf(conn, StringComparison.OrdinalIgnoreCase);
            if (idx > 8) return comment[..idx].Trim();   // mínimo de 8 chars antes do conector
        }
        return comment;
    }

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
    /// Preserva identificadores PascalCase/MAIÚSCULAS (código).
    /// Retorna null quando a tradução é insuficiente (fallback pt-BR será usado).
    /// </summary>
    private static string? TranslateToPortuguese(string text)
    {
        if (!IsEnglishText(text)) return text;  // já em pt-BR

        var result = text;

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

        // Se ainda predominantemente inglês após tradução, descarta (qualidade insuficiente)
        return IsEnglishText(result) ? null : result;
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

    private static string DetermineType(List<FileChange> changes)
    {
        var categories = changes.Select(c => GetCategory(c.Path)).ToList();

        if (categories.All(c => c == "test") || changes.All(IsTestPath)) return "test";
        if (categories.All(c => c == "docs"))                             return "docs";
        if (categories.All(c => c == "build"))                            return "build";
        if (categories.All(c => c == "config"))                           return "chore";

        int added    = changes.Count(c => c.Status is 'A' or 'C');
        int deleted  = changes.Count(c => c.Status == 'D');
        int modified = changes.Count(c => c.Status is 'M' or 'R' or 'T');

        if (added > 0 && added >= modified && deleted == 0) return "feat";
        if (deleted > 0 && deleted > added && modified == 0) return "chore";
        if (modified > 0 && added == 0 && deleted == 0)     return "fix";
        return added > modified ? "feat" : "refactor";
    }

    // ── Step 3 — Subject: descrição funcional em pt-BR ────────────────────────

    private static string BuildSubject(string type, List<FileChange> changes)
    {
        int added   = changes.Count(c => c.Status is 'A' or 'C');
        int deleted = changes.Count(c => c.Status == 'D');
        int modified= changes.Count(c => c.Status is 'M' or 'R' or 'T');

        // Verbo em pt-BR; null = omitir (tipo já descreve a ação)
        string? verb = type switch
        {
            "feat"     => "adicionar",
            "fix"      => "corrigir",
            "docs"     => "atualizar",
            "test"     => added > modified ? "adicionar" : "atualizar",
            "chore"    => deleted > 0 && added == 0 ? "remover" : "atualizar",
            "build"    => added > modified ? "adicionar" : "atualizar",
            "refactor" => null,   // "refactor: refatorar X" seria redundante
            _          => added >= deleted ? "adicionar" : "atualizar"
        };

        var phrase = BuildFunctionalPhrase(changes);
        return verb is not null ? $"{verb} {phrase}" : phrase;
    }

    // ── Step 4 — Body: frase das camadas arquiteturais em pt-BR ───────────────

    private static string BuildBody(List<FileChange> changes)
    {
        if (changes.Count == 1) return string.Empty;

        var concepts = ExtractUniqueConcepts(changes);
        var layers   = ExtractArchLayerNames(changes);

        if (layers.Count < 2 && concepts.Count < 2) return string.Empty;

        var sb = new StringBuilder();

        if (concepts.Count >= 2)
        {
            var phrases = concepts
                .Select(MapConcept)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();

            sb.Append("Abrange ");
            sb.Append(JoinPhrases(phrases));

            if (layers.Count >= 2)
                sb.Append($" nas camadas de {JoinPhrases(layers)}");
        }
        else if (layers.Count >= 2)
        {
            sb.Append($"Inclui as camadas de {JoinPhrases(layers)}");
        }

        if (sb.Length == 0) return string.Empty;

        sb.Append('.');
        return sb.ToString();
    }

    // ── Concept extraction ─────────────────────────────────────────────────────

    private static List<string> ExtractUniqueConcepts(List<FileChange> changes)
    {
        var freq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var c in changes)
        {
            var raw = ExtractRawConcept(Path.GetFileNameWithoutExtension(c.Path));
            if (raw is null) continue;
            freq[raw] = freq.TryGetValue(raw, out var n) ? n + 1 : 1;
        }

        return freq
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .Take(3)
            .ToList();
    }

    private static string? ExtractRawConcept(string filename)
    {
        // Stems com ponto são nomes de assembly/projeto (ex: GitExtensions.ZimerfeldCommitMsg) — ignorar
        if (filename.Contains('.')) return null;

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
        if (!ConceptPhrases.ContainsKey(name))
        {
            var wordCount = Regex.Matches(name, @"[A-Z][a-z]+").Count;
            if (wordCount > 2) return null;
        }

        return name;
    }

    private static string BuildFunctionalPhrase(List<FileChange> changes)
    {
        if (changes.Count == 1)
        {
            var raw = ExtractRawConcept(Path.GetFileNameWithoutExtension(changes[0].Path));
            return raw is not null ? MapConcept(raw) : FallbackPhrase(changes);
        }

        var concepts = ExtractUniqueConcepts(changes);
        if (concepts.Count == 0) return FallbackPhrase(changes);

        // Título: conceito mais dominante (frequente) — síntese global das mudanças
        return concepts
            .Select(MapConcept)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .First();
    }

    private static string MapConcept(string raw) =>
        ConceptPhrases.TryGetValue(raw, out var mapped) ? mapped : HumanizeName(raw);

    private static string FallbackPhrase(List<FileChange> changes)
    {
        var category = changes
            .Select(c => GetCategory(c.Path))
            .GroupBy(c => c)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "source";

        return category switch
        {
            "docs"   => "documentação",
            "config" => "configuração",
            "build"  => "configuração de build",
            "test"   => "testes unitários",
            "web"    => "componentes web",
            _        => "código-fonte"
        };
    }

    // ── Architectural layer detection ──────────────────────────────────────────

    private static List<string> ExtractArchLayerNames(List<FileChange> changes)
    {
        var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var change in changes)
        {
            var name = Path.GetFileNameWithoutExtension(change.Path);
            foreach (var (suffix, layer) in ArchLayers)
            {
                if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) &&
                    name.Length > suffix.Length)
                {
                    found.Add(layer);
                    break;
                }
            }
        }

        // Ordem canônica
        var order = new[] { "serviço", "repositório", "controlador", "handler",
                            "middleware", "validador", "mapeador", "fábrica",
                            "gerenciador", "gerador", "DTO", "view model",
                            "entidade", "modelo", "teste" };
        return found.OrderBy(l => Array.IndexOf(order, l)).ThenBy(l => l).ToList();
    }

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

    // Garante que o título do commit respeita o limite recomendado de 72 chars
    private static string TruncateTitle(string title, int maxLen = 72)
    {
        if (title.Length <= maxLen) return title;
        var cut = title.LastIndexOf(' ', maxLen - 2);
        return cut > 8 ? title[..cut] + "…" : title[..(maxLen - 1)] + "…";
    }

    private static string JoinPhrases(List<string> items) => items.Count switch
    {
        0 => string.Empty,
        1 => items[0],
        2 => $"{items[0]} e {items[1]}",
        _ => $"{string.Join(", ", items[..^1])} e {items[^1]}"
    };

    private static string GetCommonDirectory(List<string> paths)
    {
        var dirs = paths
            .Select(p => (Path.GetDirectoryName(p) ?? string.Empty)
                .Replace('\\', '/')
                .Split('/', StringSplitOptions.RemoveEmptyEntries))
            .ToList();

        if (dirs.Count == 0 || dirs[0].Length == 0) return string.Empty;

        var common = dirs[0].ToList();
        foreach (var parts in dirs.Skip(1))
        {
            var limit = Math.Min(common.Count, parts.Length);
            var i = 0;
            while (i < limit && common[i] == parts[i]) i++;
            common = common.Take(i).ToList();
            if (common.Count == 0) break;
        }
        return string.Join("/", common);
    }

    // ── Git subprocess ─────────────────────────────────────────────────────────

    private string RunGit(params string[] args)
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
            p.WaitForExit(8_000);
            return output;
        }
        catch { return string.Empty; }
    }
}

internal sealed record FileChange(char Status, string Path);
