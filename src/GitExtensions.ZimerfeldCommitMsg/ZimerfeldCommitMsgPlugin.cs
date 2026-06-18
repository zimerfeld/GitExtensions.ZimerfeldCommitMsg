using System.ComponentModel.Composition;
using System.Globalization;
using GitExtensions.Extensibility.Git;
using GitExtensions.Extensibility.Plugins;
using GitExtensions.Extensibility.Settings;
using GitExtensions.ZimerfeldCommitMsg.Localization;

namespace GitExtensions.ZimerfeldCommitMsg;

[Export(typeof(IGitPlugin))]
public sealed class ZimerfeldCommitMsgPlugin : GitPluginBase
{
    private const string TemplatePrefix = "Zimerfeld Commit Msg";

    // Icone exibido no menu Plugins e no dropdown do dialogo de commit
    private static readonly Image? PluginIcon = LoadIcon();

    // Itens do dropdown de templates de commit: rótulo + idioma forçado.
    // A API de templates do GitExtensions é PLANA (sem submenu aninhado), então expomos
    // um item por idioma. Lang == null → "Automático": segue o setting/idioma do SO.
    private static readonly (string Label, MessageLanguage? Lang)[] _templateItems =
    [
        ($"{TemplatePrefix} — {LanguageOption.Auto}",      null),
        ($"{TemplatePrefix} — {LanguageOption.Portugues}", MessageLanguage.PtBr),
        ($"{TemplatePrefix} — {LanguageOption.English}",   MessageLanguage.En),
    ];

    // Idioma da mensagem: Automático (detecta pelo SO) ou forçado pelo usuário.
    private static readonly ChoiceSetting _languageSetting = new(
        "ZimerfeldCommitMsg_Language",
        "Idioma da mensagem / Message language",
        new[] { LanguageOption.Auto, LanguageOption.Portugues, LanguageOption.English },
        LanguageOption.Auto);

    // Capturado no Register() (roda na UI thread) para marshalling seguro
    private SynchronizationContext? _syncContext;
    private string _lastGeneratedMessage = string.Empty;

    // Fonte do working dir para o gatilho de Application.Idle (que não traz GitUIEventArgs).
    private IGitUICommands? _gitUiCommands;

    // Instância de FormCommit já preenchida ao abrir — evita reprocessar a cada Idle
    // (o evento dispara muitas vezes). WeakReference para não prender o form na memória.
    private WeakReference<Form>? _handledCommitForm;

    // Idioma escolhido explicitamente num item do dropdown (null = seguir o setting/SO).
    // Faz o auto-refresh manter o idioma que o usuário acabou de escolher.
    private MessageLanguage? _sessionLanguage;

    public ZimerfeldCommitMsgPlugin()
    {
        Name = "ZimerfeldCommitMsg";
        // Na construção o container de settings ainda não está disponível; a descrição do
        // menu segue o idioma do SO. O override manual afeta a mensagem gerada e os diálogos.
        Description = Strings.PluginDescription(MessageLanguageResolver.FromCulture(CultureInfo.CurrentUICulture));
        if (PluginIcon is not null) Icon = PluginIcon;
    }

    /// <summary>Expõe o seletor de idioma nas configurações do plugin.</summary>
    public override IEnumerable<ISetting> GetSettings()
    {
        yield return _languageSetting;
    }

    /// <summary>
    /// Idioma do setting: lê o valor configurado (quando disponível) e resolve "Automático" pelo SO.
    /// </summary>
    private MessageLanguage CurrentLanguage()
    {
        var value = Settings is null ? null : _languageSetting.ValueOrDefault(Settings);
        return MessageLanguageResolver.Resolve(value);
    }

    /// <summary>
    /// Idioma efetivo: a escolha explícita num item do dropdown tem prioridade sobre o setting/SO.
    /// </summary>
    private MessageLanguage EffectiveLanguage() => _sessionLanguage ?? CurrentLanguage();

    /// <summary>
    /// Gera a mensagem para um item do dropdown e fixa o idioma escolhido (<paramref name="forced"/>)
    /// para que o auto-refresh subsequente mantenha o mesmo idioma.
    /// </summary>
    private string GenerateForTemplate(string workingDir, MessageLanguage? forced)
    {
        _sessionLanguage = forced;
        var msg = new CommitMessageGenerator(workingDir, forced ?? CurrentLanguage()).Generate();
        _lastGeneratedMessage = msg;
        return msg;
    }

    private static Image? LoadIcon()
    {
        try
        {
            using var stream = typeof(ZimerfeldCommitMsgPlugin).Assembly
                .GetManifestResourceStream("GitExtensions.ZimerfeldCommitMsg.Resources.icon.png");
            if (stream is null) return null;
            using var img = Image.FromStream(stream);
            return new Bitmap(img);   // copia independente do stream (que sera descartado)
        }
        catch { return null; }
    }

    public override void Register(IGitUICommands gitUiCommands)
    {
        base.Register(gitUiCommands);
        _syncContext   = SynchronizationContext.Current;
        _gitUiCommands = gitUiCommands;

        // Um item de template por idioma (Automático/Português/Inglês) — só preenche
        // quando o usuário selecionar explicitamente no dropdown.
        foreach (var (label, lang) in _templateItems)
        {
            var forced = lang;   // captura por iteração
            gitUiCommands.AddCommitTemplate(
                label,
                () => GenerateForTemplate(gitUiCommands.Module.WorkingDir, forced),
                icon: PluginIcon);
        }

        // Atualiza a mensagem automaticamente sempre que arquivos entram/saem do stage
        gitUiCommands.PostRepositoryChanged += OnPostRepositoryChanged;

        // Preenche também ao ABRIR o diálogo com arquivos já em stage: não há evento de
        // "diálogo aberto" na API, então detectamos o FormCommit recém-aberto no Idle da UI.
        Application.Idle += OnAppIdle;
    }

    public override void Unregister(IGitUICommands gitUiCommands)
    {
        gitUiCommands.PostRepositoryChanged -= OnPostRepositoryChanged;
        Application.Idle -= OnAppIdle;
        foreach (var (label, _) in _templateItems)
            gitUiCommands.RemoveCommitTemplate(label);
        _lastGeneratedMessage = string.Empty;
        _handledCommitForm    = null;
        _gitUiCommands        = null;
        base.Unregister(gitUiCommands);
    }

    // Plugins menu → abre o diálogo de commit com a mensagem já preenchida
    public override bool Execute(GitUIEventArgs args)
    {
        var lang = CurrentLanguage();

        if (!args.GitModule.IsValidGitWorkingDir())
        {
            MessageBox.Show(
                Strings.RepoInvalido(lang),
                Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        var message = new CommitMessageGenerator(args.GitModule.WorkingDir, lang).Generate();

        if (string.IsNullOrWhiteSpace(message))
        {
            MessageBox.Show(
                Strings.SemMudancasStaged(lang),
                Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        return args.GitUICommands.StartCommitDialog(args.OwnerForm, commitMessage: message);
    }

    // ── Stage/unstage handler ──────────────────────────────────────────────────

    private void OnPostRepositoryChanged(object? sender, GitUIEventArgs e)
    {
        try
        {
            var workingDir = e.GitModule?.WorkingDir;
            if (string.IsNullOrEmpty(workingDir)) return;

            // Sempre executa na UI thread para acesso seguro a Application.OpenForms
            if (_syncContext is not null)
                _syncContext.Post(_ => RefreshOpenCommitDialog(workingDir), null);
            else
                RefreshOpenCommitDialog(workingDir);
        }
        catch { /* nunca deixar o plugin derrubar o GitExtensions */ }
    }

    /// <summary>
    /// Preenche/atualiza a mensagem no diálogo de commit aberto. Retorna <c>true</c> quando o
    /// form e a caixa de texto já existem e o caso foi tratado (preenchido, ou deixado intacto
    /// por conter texto do usuário/mensagem vazia); <c>false</c> quando ainda não há diálogo ou
    /// a caixa não foi localizada (UI ainda montando) — sinaliza ao Idle para tentar de novo.
    /// </summary>
    private bool RefreshOpenCommitDialog(string workingDir)
    {
        try
        {
            // Localiza o diálogo de commit aberto, se houver
            Form? commitForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f.GetType().Name == "FormCommit") { commitForm = f; break; }
            }
            if (commitForm == null) return false;

            var tb = FindCommitTextBox(commitForm);
            if (tb == null) return false;

            // Não sobrescreve texto digitado manualmente pelo usuário:
            // só atualiza se a caixa estiver vazia ou contiver a última mensagem gerada por nós
            var current = tb.Text.Trim();
            if (current.Length > 0 && current != _lastGeneratedMessage.Trim()) return true;

            var msg = new CommitMessageGenerator(workingDir, EffectiveLanguage()).Generate();
            if (string.IsNullOrEmpty(msg)) return true;

            _lastGeneratedMessage = msg;
            tb.Text = msg;
            // O GitExtensions colore a caixa (linhas longas, assunto, …) ao alterar o texto.
            // Zeramos esse realce logo após escrever, deixando tudo na cor padrão do tema.
            ResetTextColors(tb);
            tb.SelectionStart = 0;
            tb.SelectionLength = 0;
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Idle da UI: preenche o diálogo de commit que abriu já com arquivos em stage (sem evento
    /// de "diálogo aberto" na API). Trata cada instância de FormCommit uma só vez — o Idle
    /// dispara com altíssima frequência, então só roda o gerador quando um form NOVO aparece e
    /// ainda não foi tratado, evitando custo de processo git a cada ciclo ocioso.
    /// </summary>
    private void OnAppIdle(object? sender, EventArgs e)
    {
        try
        {
            Form? commitForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f.GetType().Name == "FormCommit") { commitForm = f; break; }
            }

            // Sem diálogo aberto: limpa o marcador para que a próxima abertura seja tratada.
            if (commitForm is null) { _handledCommitForm = null; return; }

            // Mesma instância já tratada → nada a fazer (o auto-refresh de stage cuida do resto).
            if (_handledCommitForm is not null &&
                _handledCommitForm.TryGetTarget(out var prev) && ReferenceEquals(prev, commitForm))
                return;

            var workingDir = _gitUiCommands?.Module.WorkingDir;
            if (string.IsNullOrEmpty(workingDir)) return;

            // Só marca como tratada quando o form/caixa já existem (true); se a UI ainda está
            // montando (false), deixa para o próximo Idle tentar de novo.
            if (RefreshOpenCommitDialog(workingDir))
                _handledCommitForm = new WeakReference<Form>(commitForm);
        }
        catch { /* nunca deixar o plugin derrubar o GitExtensions */ }
    }

    /// <summary>
    /// Remove qualquer efeito de cor/realce do texto da caixa de commit, voltando à cor
    /// padrão do controle. Aplica-se ao RichTextBox usado pelo GitExtensions; outros tipos
    /// de caixa não têm formatação por trecho e são ignorados. O host pode repintar numa
    /// edição posterior — aqui só garantimos a mensagem que acabamos de gerar sem cor.
    /// </summary>
    private static void ResetTextColors(TextBoxBase tb)
    {
        if (tb is not RichTextBox rtb) return;

        rtb.SelectAll();
        rtb.SelectionColor     = rtb.ForeColor;
        rtb.SelectionBackColor = rtb.BackColor;
        rtb.Select(0, 0);
    }

    // ── UI helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Localiza a caixa de texto da mensagem de commit no FormCommit.
    /// Tenta nomes conhecidos de versões do GitExtensions; fallback: maior área editável.
    /// </summary>
    private static TextBoxBase? FindCommitTextBox(Form form)
    {
        // Nomes usados em versões diferentes do GitExtensions
        foreach (var name in new[] { "Message", "commitMessageEditor", "_commitMessage", "commitMessage" })
        {
            if (FindDescendantByName(form, name) is TextBoxBase tb) return tb;
        }

        // Fallback: maior TextBox multiline editável (a caixa de mensagem é a maior)
        return EnumerateDescendants(form)
            .OfType<TextBoxBase>()
            .Where(t => t.Multiline && !t.ReadOnly && t.Visible)
            .OrderByDescending(t => t.Width * t.Height)
            .FirstOrDefault();
    }

    private static Control? FindDescendantByName(Control parent, string name)
    {
        foreach (Control child in parent.Controls)
        {
            if (string.Equals(child.Name, name, StringComparison.OrdinalIgnoreCase))
                return child;
            var found = FindDescendantByName(child, name);
            if (found is not null) return found;
        }
        return null;
    }

    private static IEnumerable<Control> EnumerateDescendants(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            yield return child;
            foreach (var desc in EnumerateDescendants(child))
                yield return desc;
        }
    }
}
