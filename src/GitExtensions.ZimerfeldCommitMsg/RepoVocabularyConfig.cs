using System.Text.Json;

namespace GitExtensions.ZimerfeldCommitMsg;

/// <summary>
/// Vocabulário adicional carregado por repositório, permitindo estender a extração de
/// conceitos sem recompilar o plugin. É lido de um arquivo <c>.zimerfeldcommitmsg.json</c>
/// na raiz do diretório de trabalho (opcional). Formato:
/// <code>
/// {
///   "knownVocabulary":    ["widget", "gadget"],
///   "rejectedVocabulary": ["acme", "contoso"],
///   "concepts":           { "widget": "componente", "gadget": "acessório" }
/// }
/// </code>
/// <list type="bullet">
/// <item><c>knownVocabulary</c> — palavras que passam a ser aceitas como parte de um nome
/// descritivo (soma-se ao <c>KnownVocabulary</c> embutido; vale para os dois idiomas).</item>
/// <item><c>rejectedVocabulary</c> — palavras que forçam a rejeição do nome como conceito
/// (nomes próprios/namespaces do projeto; soma-se ao <c>RejectedVocabulary</c> embutido).</item>
/// <item><c>concepts</c> — tradução de palavra-de-conceito → frase pt-BR, usada no prefixo
/// nominal do título (tem prioridade sobre o dicionário embutido).</item>
/// </list>
/// Falhas de leitura/parse são silenciosas (config vazia) — nunca quebram a geração.
/// </summary>
internal sealed class RepoVocabularyConfig
{
    public const string FileName = ".zimerfeldcommitmsg.json";

    public HashSet<string> Known { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Rejected { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> ConceptPt { get; } = new(StringComparer.OrdinalIgnoreCase);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    /// <summary>Carrega a config do diretório; devolve uma instância vazia se não houver/for inválida.</summary>
    public static RepoVocabularyConfig Load(string workingDir)
    {
        var cfg = new RepoVocabularyConfig();
        try
        {
            if (string.IsNullOrWhiteSpace(workingDir)) return cfg;
            var path = Path.Combine(workingDir, FileName);
            if (!File.Exists(path)) return cfg;

            var dto = JsonSerializer.Deserialize<Dto>(File.ReadAllText(path), JsonOptions);
            if (dto is null) return cfg;

            foreach (var w in dto.KnownVocabulary ?? [])
                if (!string.IsNullOrWhiteSpace(w)) cfg.Known.Add(w.Trim());

            foreach (var w in dto.RejectedVocabulary ?? [])
                if (!string.IsNullOrWhiteSpace(w)) cfg.Rejected.Add(w.Trim());

            foreach (var kv in dto.Concepts ?? [])
                if (!string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value))
                    cfg.ConceptPt[kv.Key.Trim()] = kv.Value.Trim();
        }
        catch
        {
            // Config malformada não deve impedir a geração — ignora e segue com os defaults.
        }
        return cfg;
    }

    private sealed class Dto
    {
        public List<string>? KnownVocabulary { get; set; }
        public List<string>? RejectedVocabulary { get; set; }
        public Dictionary<string, string>? Concepts { get; set; }
    }
}
