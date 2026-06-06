using System.Globalization;

namespace GitExtensions.ZimerfeldCommitMsg.Localization;

/// <summary>Idioma de saída da mensagem de commit.</summary>
internal enum MessageLanguage
{
    /// <summary>Português do Brasil.</summary>
    PtBr,
    /// <summary>Inglês.</summary>
    En,
}

/// <summary>
/// Rótulos do <c>ChoiceSetting</c> de idioma exibidos nas opções do plugin.
/// Compartilhados entre o plugin (registro do setting) e o resolvedor (leitura).
/// </summary>
internal static class LanguageOption
{
    public const string Auto       = "Automático";
    public const string Portugues  = "Português";
    public const string English    = "English";
}

/// <summary>
/// Resolve o idioma efetivo a partir do valor configurado pelo usuário.
/// "Automático" (ou ausente) detecta pelo idioma do sistema operacional/GitExtensions.
/// </summary>
internal static class MessageLanguageResolver
{
    public static MessageLanguage Resolve(string? settingValue)
    {
        if (Equals(settingValue, LanguageOption.Portugues) ||
            string.Equals(settingValue, "Portugues", StringComparison.OrdinalIgnoreCase))
            return MessageLanguage.PtBr;

        if (Equals(settingValue, LanguageOption.English))
            return MessageLanguage.En;

        // "Automático" ou valor desconhecido → detecta pelo SO
        return FromCulture(CultureInfo.CurrentUICulture);
    }

    /// <summary>pt-* → português; qualquer outro → inglês.</summary>
    public static MessageLanguage FromCulture(CultureInfo culture) =>
        culture.TwoLetterISOLanguageName.Equals("pt", StringComparison.OrdinalIgnoreCase)
            ? MessageLanguage.PtBr
            : MessageLanguage.En;

    private static bool Equals(string? value, string option) =>
        string.Equals(value, option, StringComparison.OrdinalIgnoreCase);
}
