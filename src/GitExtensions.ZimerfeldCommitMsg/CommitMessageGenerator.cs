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

    private static readonly string[] TestPathSegments =
        ["test", "tests", "spec", "specs", "__tests__", "unittest", "unittests"];

    private static readonly HashSet<string> SkippableRoots =
        new(StringComparer.OrdinalIgnoreCase) { "src", "app", "lib", "source", "main", ".", "" };

    public CommitMessageGenerator(string workingDir) => _workingDir = workingDir;

    // ── Public API ─────────────────────────────────────────────────────────────

    public string Generate()
    {
        var raw = RunGit("diff", "--cached", "--name-status");
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

        var changes = ParseChanges(raw);
        if (changes.Count == 0) return string.Empty;

        var type = DetermineType(changes);
        var desc = BuildSubject(type, changes);
        var body = BuildBody(changes);

        var header = $"{type}: {desc}";
        return body.Length > 0 ? $"{header}\n\n{body}" : header;
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

        var phrases = concepts
            .Select(MapConcept)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList();

        return phrases.Count switch
        {
            1 => phrases[0],
            2 => $"{phrases[0]} e {phrases[1]}",
            _ => $"{phrases[0]}, {phrases[1]} e {phrases[2]}"
        };
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

    /// <summary>PascalCase → palavras em minúsculas. "UserAuthService" → "user auth service".</summary>
    private static string HumanizeName(string name)
    {
        var spaced = Regex.Replace(name, @"([a-z])([A-Z])", "$1 $2")
                          .Replace('-', ' ')
                          .Replace('_', ' ');
        return Regex.Replace(spaced, @"\s+", " ").Trim().ToLowerInvariant();
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
