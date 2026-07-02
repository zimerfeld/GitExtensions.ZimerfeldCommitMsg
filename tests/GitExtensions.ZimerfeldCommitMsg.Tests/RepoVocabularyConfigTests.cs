using GitExtensions.ZimerfeldCommitMsg;
using GitExtensions.ZimerfeldCommitMsg.Localization;
using Xunit;

namespace GitExtensions.ZimerfeldCommitMsg.Tests;

/// <summary>Config de vocabulário por repositório (.zimerfeldcommitmsg.json) e seu efeito no gerador.</summary>
public class RepoVocabularyConfigTests
{
    /// <summary>Cria um diretório temporário com o arquivo de config e executa a ação, limpando ao fim.</summary>
    private static void WithConfig(string json, Action<string> action)
    {
        var dir = Path.Combine(Path.GetTempPath(), "zcm-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            File.WriteAllText(Path.Combine(dir, RepoVocabularyConfig.FileName), json);
            action(dir);
        }
        finally
        {
            try { Directory.Delete(dir, recursive: true); } catch { /* best-effort */ }
        }
    }

    [Fact]
    public void Load_reads_all_sections() => WithConfig(
        """
        {
          "knownVocabulary":    ["widget", "gadget"],
          "rejectedVocabulary": ["acme"],
          "concepts":           { "widget": "componente" }
        }
        """,
        dir =>
        {
            var cfg = RepoVocabularyConfig.Load(dir);
            Assert.Contains("widget", cfg.Known);
            Assert.Contains("gadget", cfg.Known);
            Assert.Contains("acme", cfg.Rejected);
            Assert.Equal("componente", cfg.ConceptPt["widget"]);
        });

    [Fact]
    public void Load_returns_empty_for_missing_file()
    {
        var cfg = RepoVocabularyConfig.Load(Path.Combine(Path.GetTempPath(), "zcm-missing-" + Guid.NewGuid().ToString("N")));
        Assert.Empty(cfg.Known);
        Assert.Empty(cfg.Rejected);
        Assert.Empty(cfg.ConceptPt);
    }

    [Fact]
    public void Load_is_resilient_to_malformed_json() => WithConfig("{ not valid json ", dir =>
    {
        var cfg = RepoVocabularyConfig.Load(dir);
        Assert.Empty(cfg.Known);
        Assert.Empty(cfg.Rejected);
    });

    [Fact]
    public void RejectedVocabulary_from_config_blocks_concept() => WithConfig(
        """{ "rejectedVocabulary": ["acme"] }""",
        dir =>
        {
            var withCfg = new CommitMessageGenerator(dir, MessageLanguage.PtBr);
            var noCfg   = new CommitMessageGenerator(string.Empty, MessageLanguage.PtBr);

            Assert.Null(withCfg.ExtractRawConcept("AcmeService"));   // rejeitado pela config
            Assert.Equal("Acme", noCfg.ExtractRawConcept("AcmeService")); // aceito sem config
        });

    [Fact]
    public void KnownVocabulary_from_config_accepts_multiword_name() => WithConfig(
        """{ "knownVocabulary": ["foo", "bar", "baz"] }""",
        dir =>
        {
            var withCfg = new CommitMessageGenerator(dir, MessageLanguage.PtBr);
            var noCfg   = new CommitMessageGenerator(string.Empty, MessageLanguage.PtBr);

            Assert.Equal("FooBarBaz", withCfg.ExtractRawConcept("FooBarBaz")); // reconhecido via config
            Assert.Null(noCfg.ExtractRawConcept("FooBarBaz"));                 // namespace sem config
        });
}
