using System.Globalization;
using System.Resources;

namespace GitExtensions.ZimerfeldCommitMsg.Localization;

/// <summary>
/// Acesso às strings de UI localizadas, embutidas no assembly principal como recursos
/// neutros (sem satellite assemblies, para preservar o deploy de DLL única).
/// A seleção é feita pelo idioma resolvido — não pela cultura global da thread —,
/// de modo que o override manual de idioma é honrado.
/// </summary>
internal static class Strings
{
    private static readonly ResourceManager EnRm =
        new("GitExtensions.ZimerfeldCommitMsg.Resources.Strings", typeof(Strings).Assembly);

    private static readonly ResourceManager PtRm =
        new("GitExtensions.ZimerfeldCommitMsg.Resources.StringsPtBr", typeof(Strings).Assembly);

    private static readonly ResourceManager EsRm =
        new("GitExtensions.ZimerfeldCommitMsg.Resources.StringsEsEs", typeof(Strings).Assembly);

    /// <summary>Lê a string pela chave no idioma indicado; cai para inglês e, por fim, para a chave.</summary>
    private static string Get(string key, MessageLanguage lang)
    {
        var rm = lang switch
        {
            MessageLanguage.PtBr => PtRm,
            MessageLanguage.EsEs => EsRm,
            _                    => EnRm,
        };
        // InvariantCulture evita probing de satellite assemblies — os recursos são neutros.
        return rm.GetString(key, CultureInfo.InvariantCulture)
            ?? EnRm.GetString(key, CultureInfo.InvariantCulture)
            ?? key;
    }

    public static string RepoInvalido(MessageLanguage lang)       => Get(nameof(RepoInvalido), lang);
    public static string SemMudancasStaged(MessageLanguage lang)  => Get(nameof(SemMudancasStaged), lang);
    public static string PluginDescription(MessageLanguage lang)  => Get(nameof(PluginDescription), lang);
}
