using GitExtensions.ZimerfeldCommitMsg;
using Xunit;

namespace GitExtensions.ZimerfeldCommitMsg.Tests;

/// <summary>Detecção de idioma e tradução inglês → pt-BR dos comentários.</summary>
public class TranslationTests
{
    [Theory]
    [InlineData("returns the node when found")]
    [InlineData("validates the token before processing the request")]
    public void IsEnglishText_detects_english(string text) =>
        Assert.True(CommitMessageGenerator.IsEnglishText(text));

    [Theory]
    [InlineData("processa dados locais corretamente")]
    [InlineData("valida o pedido antes de continuar")]
    public void IsEnglishText_rejects_portuguese(string text) =>
        Assert.False(CommitMessageGenerator.IsEnglishText(text));

    [Fact]
    public void TranslateToPortuguese_translates_simple_phrase() =>
        Assert.Equal("retorna valor", CommitMessageGenerator.TranslateToPortuguese("returns the value"));

    [Fact]
    public void TranslateToPortuguese_passes_portuguese_through()
    {
        const string pt = "processa dados locais corretamente";
        Assert.Equal(pt, CommitMessageGenerator.TranslateToPortuguese(pt));
    }

    [Fact]
    public void TranslateToPortuguese_preserves_conventional_commit_types()
    {
        // "feat" e "fix" são tipos CC — devem sobreviver intactos à tradução.
        var result = CommitMessageGenerator.TranslateToPortuguese("adds the feat and fix support");
        Assert.NotNull(result);
        Assert.Contains("feat", result);
        Assert.Contains("fix", result);
    }
}
