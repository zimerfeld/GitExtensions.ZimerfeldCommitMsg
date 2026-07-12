using System.Globalization;

namespace GitExtensions.ZimerfeldCommitMsg.Localization;

/// <summary>Idioma de saída da mensagem de commit.</summary>
internal enum MessageLanguage
{
    /// <summary>Português do Brasil.</summary>
    PtBr,
    /// <summary>Inglês.</summary>
    En,
    /// <summary>Espanhol (Espanha).</summary>
    EsEs,
}

/// <summary>
/// Rótulos bilíngues do <c>ChoiceSetting</c> de idioma exibidos nas opções do plugin.
/// O rótulo mostra os idiomas (ex.: "Português/Portuguese") para ser claro
/// independentemente do idioma do sistema. Compartilhados entre o plugin (registro do
/// setting) e o resolvedor (leitura). A correspondência no resolvedor é por subtrecho,
/// então o formato exato do rótulo pode mudar sem quebrar a leitura.
/// </summary>
internal static class LanguageOption
{
    public const string Auto       = "Automático/Automatic";
    public const string Portugues  = "Português/Portuguese";
    public const string English    = "Inglês/English";
    public const string Espanol    = "Espanhol/Español";
}

/// <summary>
/// Resolve o idioma efetivo a partir do valor configurado pelo usuário.
/// "Automático" (ou ausente/desconhecido) detecta pelo idioma do sistema operacional/GitExtensions.
/// A correspondência é por subtrecho (tolerante a rótulos bilíngues e a valores antigos).
/// </summary>
internal static class MessageLanguageResolver
{
    public static MessageLanguage Resolve(string? settingValue)
    {
        var v = (settingValue ?? string.Empty).ToLowerInvariant();

        // "Português" / "Portuguese"
        if (v.Contains("portug")) return MessageLanguage.PtBr;
        // "Espanhol" / "Español" / "Spanish"
        if (v.Contains("espa") || v.Contains("spanish")) return MessageLanguage.EsEs;
        // "Inglês" / "English"
        if (v.Contains("ingl") || v.Contains("english")) return MessageLanguage.En;

        // "Automático" / "Automatic" / valor desconhecido → detecta pelo SO
        return FromCulture(CultureInfo.CurrentUICulture);
    }

    /// <summary>pt-* → português; es-* → espanhol; qualquer outro → inglês.</summary>
    public static MessageLanguage FromCulture(CultureInfo culture)
    {
        var iso = culture.TwoLetterISOLanguageName;
        if (iso.Equals("pt", StringComparison.OrdinalIgnoreCase)) return MessageLanguage.PtBr;
        if (iso.Equals("es", StringComparison.OrdinalIgnoreCase)) return MessageLanguage.EsEs;
        return MessageLanguage.En;
    }
}
