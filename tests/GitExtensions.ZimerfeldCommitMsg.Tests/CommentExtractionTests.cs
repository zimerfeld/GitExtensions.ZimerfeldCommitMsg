using GitExtensions.ZimerfeldCommitMsg;
using Xunit;

namespace GitExtensions.ZimerfeldCommitMsg.Tests;

/// <summary>
/// Cobre a extração e o saneamento de comentários do diff (Estratégia 1):
/// as várias sintaxes de comentário, os filtros de rejeição e o balanceamento de delimitadores.
/// </summary>
public class CommentExtractionTests
{
    // ── Sintaxes de comentário reconhecidas ─────────────────────────────────

    [Theory]
    [InlineData("// valida o token de acesso",              "valida o token de acesso")]
    [InlineData("/// resume o conteúdo do arquivo",         "resume o conteúdo do arquivo")]
    [InlineData("/* calcula o total do pedido */",          "calcula o total do pedido")]
    [InlineData("/** documenta a função principal */",      "documenta a função principal")]
    [InlineData("/*! observa o estado do componente */",    "observa o estado do componente")]
    [InlineData("<!-- descreve a seção de layout -->",      "descreve a seção de layout")]
    [InlineData("-- seleciona os registros ativos",         "seleciona os registros ativos")]
    public void ExtractCommentText_recognizes_all_syntaxes(string line, string expected) =>
        Assert.Equal(expected, CommitMessageGenerator.ExtractCommentText(line));

    [Theory]
    [InlineData("* continua a descrição do método",         "continua a descrição do método")]
    [InlineData("' inicializa o formulário principal",      "inicializa o formulário principal")]
    [InlineData("# configura o pipeline de build",          "configura o pipeline de build")]
    public void ExtractCommentText_recognizes_non_md_syntaxes(string line, string expected) =>
        Assert.Equal(expected, CommitMessageGenerator.ExtractCommentText(line, isMdFile: false));

    [Theory]
    [InlineData("# Título do Documento")]   // heading Markdown, não comentário
    [InlineData("* item de lista markdown")] // bullet Markdown, não comentário
    public void ExtractCommentText_ignores_markdown_structure(string line) =>
        Assert.Null(CommitMessageGenerator.ExtractCommentText(line, isMdFile: true));

    [Fact]
    public void ExtractCommentText_returns_null_when_no_comment() =>
        Assert.Null(CommitMessageGenerator.ExtractCommentText("var total = soma + resto;"));

    // ── Saneamento (CleanCommentText) ───────────────────────────────────────

    [Theory]
    [InlineData("ok")]                        // curto demais
    [InlineData("curtinho")]                  // < 10 chars
    [InlineData("SemEspacoNenhumAqui")]       // sem espaço = não é frase
    [InlineData("──────────────────────")]    // separador visual
    [InlineData("<summary>algo aqui</summary>")] // tag XML de doc
    [InlineData("if (x) { return y; }")]      // código comentado (chaves)
    [InlineData("chama metodo(argumento)")]   // código comentado (chamada)
    public void CleanCommentText_rejects_noise(string raw) =>
        Assert.Null(CommitMessageGenerator.CleanCommentText(raw));

    [Fact]
    public void CleanCommentText_keeps_valid_sentence() =>
        Assert.Equal("processa a requisição corretamente",
            CommitMessageGenerator.CleanCommentText("processa a requisição corretamente"));

    [Fact]
    public void CleanCommentText_trims_block_residue() =>
        Assert.Equal("valida o token de acesso",
            CommitMessageGenerator.CleanCommentText("valida o token de acesso */"));

    // ── Balanceamento de delimitadores ──────────────────────────────────────

    [Theory]
    [InlineData("processa os dados corretamente")]
    [InlineData("mapeia <chave, valor> no dicionário")]
    [InlineData("trata contração don't sem problema")] // apóstrofo de contração é ignorado
    public void DelimitersBalanced_accepts_balanced(string text) =>
        Assert.True(CommitMessageGenerator.DelimitersBalanced(text));

    [Theory]
    [InlineData("monta a árvore (recursivo")]          // parêntese aberto
    [InlineData("compara usando aspas \" solta")]      // aspa dupla ímpar
    [InlineData("termina com aspa ' solta")]           // aspa simples de delimitação ímpar
    [InlineData("compara a < b sempre")]               // < sem >
    public void DelimitersBalanced_rejects_unbalanced(string text) =>
        Assert.False(CommitMessageGenerator.DelimitersBalanced(text));
}
