using GitExtensions.ZimerfeldCommitMsg;
using GitExtensions.ZimerfeldCommitMsg.Localization;
using Xunit;

namespace GitExtensions.ZimerfeldCommitMsg.Tests;

/// <summary>
/// Extração de conceito a partir do nome do arquivo (Estratégia 2) e utilitários de texto:
/// humanização, saneamento de frase, pontuação de candidatos e limite de 80 chars.
/// </summary>
public class ConceptExtractionTests
{
    private readonly CommitMessageGenerator _pt = new(string.Empty, MessageLanguage.PtBr);

    [Theory]
    [InlineData("IUserService",   "User")]   // prefixo de interface + sufixo arquitetural
    [InlineData("OrderController", "Order")]  // sufixo removido → conceito de domínio
    [InlineData("New Text Document", "New Text Document")] // nome descritivo (vocabulário conhecido)
    public void ExtractRawConcept_extracts_readable_concept(string filename, string expected) =>
        Assert.Equal(expected, _pt.ExtractRawConcept(filename));

    [Theory]
    [InlineData("ZimerfeldCommitMsg")]              // vocabulário de rejeição ("zimerfeld")
    [InlineData("GitExtensions.ZimerfeldCommitMsg")] // stem com ponto = namespace
    [InlineData("SomeRandomNamespaceThing")]        // 4 palavras não reconhecidas = namespace
    public void ExtractRawConcept_rejects_namespaces(string filename) =>
        Assert.Null(_pt.ExtractRawConcept(filename));

    [Theory]
    [InlineData("UserAuthService", "user auth service")]
    [InlineData("ParseHTML",       "parse HTML")]   // acrônimo em MAIÚSCULAS preservado
    public void HumanizeName_splits_pascal_case(string name, string expected) =>
        Assert.Equal(expected, CommitMessageGenerator.HumanizeName(name));

    [Fact]
    public void IsCleanSentence_accepts_closed_sentence() =>
        Assert.True(_pt.IsCleanSentence("processa a requisição corretamente"));

    [Theory]
    [InlineData("mapeia o token para")]       // termina em palavra de ligação solta
    [InlineData("monta a árvore (recursivo")] // delimitador desbalanceado
    public void IsCleanSentence_rejects_truncated(string text) =>
        Assert.False(_pt.IsCleanSentence(text));

    [Fact]
    public void ScoreCandidate_prefers_action_sentence_over_fragment() =>
        Assert.True(_pt.ScoreCandidate("valida o pedido corretamente") > _pt.ScoreCandidate("curto"));

    [Fact]
    public void TruncateTitle_keeps_short_title()
    {
        const string t = "Corrige 2 arquivos (fix)";
        Assert.Equal(t, CommitMessageGenerator.TruncateTitle(t));
    }

    [Fact]
    public void TruncateTitle_clips_long_title_with_ellipsis()
    {
        var longTitle = string.Join(" ", Enumerable.Repeat("palavra", 20)); // ~159 chars
        var result = CommitMessageGenerator.TruncateTitle(longTitle);
        Assert.True(result.Length <= 80);
        Assert.EndsWith("…", result);
    }
}
