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

    // Working dir do último preenchimento via Idle. O GitExtensions pode REAPROVEITAR o
    // mesmo FormCommit ao trocar de repositório; sem rastrear o working dir, o gate por
    // instância (acima) bloquearia o repreenchimento e a caixa ficaria vazia/desatualizada.
    private string? _handledWorkingDir;

    // Idioma fixado pela escolha de um item do dropdown (null = seguir o setting/SO).
    // Faz o auto-refresh manter o idioma que o usuário acabou de escolher.
    private MessageLanguage? _sessionLanguage;

    // Mensagens que GERAMOS para os itens do dropdown na última abertura do menu, mapeadas ao
    // idioma do item (null = Automático). A API de template do host NÃO dá callback de clique:
    // ele invoca nosso Func de TODOS os itens ao ABRIR o dropdown e, no clique, só aplica o texto
    // já materializado. Então detectamos a escolha observando a caixa virar exatamente um destes
    // textos (via TextChanged) — aí fixamos _sessionLanguage no idioma correspondente.
    private readonly Dictionary<string, MessageLanguage?> _templateMessages =
        new(StringComparer.Ordinal);

    // Caixa de mensagem na qual já assinamos o TextChanged (para detectar a escolha do dropdown).
    // WeakReference para não prender o controle; reassina se a instância mudar.
    private WeakReference<TextBoxBase>? _subscribedTextBox;

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
    /// Idioma efetivo: a escolha de um item do dropdown (<see cref="_sessionLanguage"/>) tem
    /// prioridade sobre o setting/SO. Usado pelo auto-refresh para regenerar sempre no idioma
    /// que o usuário escolheu no menu.
    /// </summary>
    private MessageLanguage EffectiveLanguage() => _sessionLanguage ?? CurrentLanguage();

    /// <summary>
    /// Gera a mensagem para um item do dropdown, no idioma do item (<paramref name="forced"/>).
    ///
    /// IMPORTANTE: o host (CommitTemplateManager.RegisteredTemplates) invoca este Func ao ABRIR
    /// o dropdown — uma vez para CADA item de idioma, em sequência — e guarda o texto já
    /// materializado; o clique só aplica esse texto via ReplaceMessage, SEM rechamar o Func.
    /// Por isso NÃO fixamos o idioma aqui (rodaria para todos os itens, fixando sempre o último).
    /// Em vez disso, registramos cada texto gerado (msg → idioma do item) para depois reconhecer,
    /// pela caixa, QUAL item o usuário clicou — ver <see cref="DetectTemplateSelection"/>.
    /// </summary>
    private string GenerateForTemplate(string workingDir, MessageLanguage? forced)
    {
        var msg = new CommitMessageGenerator(workingDir, forced ?? CurrentLanguage()).Generate();
        _lastGeneratedMessage = msg;
        RememberTemplateMessage(msg, forced);
        return msg;
    }

    /// <summary>
    /// Registra um texto gerado para um item do dropdown, mapeado ao idioma do item, para que a
    /// escolha possa ser reconhecida depois pela caixa de mensagem. Mensagens idênticas (ex.: o
    /// item "Automático" coincide com "Português" num SO pt-BR) colapsam na mesma chave — o que é
    /// inofensivo: ambos resolvem para o mesmo idioma efetivo. Limpa o mapa se crescer além do
    /// esperado (defensivo; em uso normal são ≤ 3 entradas).
    /// </summary>
    private void RememberTemplateMessage(string msg, MessageLanguage? forced)
    {
        if (string.IsNullOrEmpty(msg)) return;
        if (_templateMessages.Count > 12) _templateMessages.Clear();
        _templateMessages[NormalizeNewlines(msg)] = forced;
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
        if (_subscribedTextBox is not null && _subscribedTextBox.TryGetTarget(out var tb))
            try { tb.TextChanged -= OnCommitTextChanged; } catch { /* controle pode ter morrido */ }
        _subscribedTextBox    = null;
        _templateMessages.Clear();
        _sessionLanguage      = null;
        _lastGeneratedMessage = string.Empty;
        _handledCommitForm    = null;
        _handledWorkingDir    = null;
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

            // Assina o TextChanged desta caixa (uma vez) para reconhecer a escolha de um item do
            // dropdown — a API de template do host não expõe o clique.
            EnsureTextChangedHook(tb);

            // Não sobrescreve texto digitado manualmente pelo usuário: só atualiza se a caixa
            // estiver vazia ou contiver a última mensagem gerada por nós. A comparação normaliza
            // as quebras de linha — a caixa do host pode devolver \r\n enquanto geramos com \n;
            // sem isso a NOSSA própria mensagem passaria por "texto do usuário" e as atualizações
            // automáticas travariam, deixando a mensagem do working dir anterior repetida ao
            // trocar de repositório ou ao fazer stage/unstage.
            var current = NormalizeNewlines(tb.Text);
            if (current.Length > 0 && current != NormalizeNewlines(_lastGeneratedMessage)) return true;

            var msg = new CommitMessageGenerator(workingDir, EffectiveLanguage()).Generate();
            if (string.IsNullOrEmpty(msg))
            {
                // Novo working dir sem nada em stage: limpa a NOSSA mensagem anterior para não
                // repetir a do repositório anterior. Só age quando a caixa ainda contém o que
                // geramos (texto do usuário já teria retornado acima).
                if (current.Length > 0)
                {
                    _lastGeneratedMessage = string.Empty;
                    tb.Text = string.Empty;
                    ResetTextColors(tb);
                }
                return true;
            }

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
    /// de "diálogo aberto" na API). O Idle dispara com altíssima frequência, então só roda o
    /// gerador quando há algo NOVO a tratar — um FormCommit recém-aberto OU uma troca de working
    /// dir no mesmo form (o GitExtensions reaproveita a instância ao trocar de repositório).
    /// Fora desses casos não faz nada, evitando custo de processo git a cada ciclo ocioso.
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

            // Sem diálogo aberto: limpa os marcadores para que a próxima abertura seja tratada e
            // reinicia o idioma da sessão — a fixação por dropdown vale só enquanto o diálogo vive.
            if (commitForm is null)
            {
                _handledCommitForm = null;
                _handledWorkingDir = null;
                _sessionLanguage   = null;
                _templateMessages.Clear();
                _subscribedTextBox = null;
                return;
            }

            var workingDir = _gitUiCommands?.Module.WorkingDir;
            if (string.IsNullOrEmpty(workingDir)) return;

            // Pula só quando NADA mudou: mesma instância de form E mesmo working dir. Trocar de
            // repositório (mesmo reaproveitando o form) muda o working dir e força o repreenchimento.
            bool sameForm = _handledCommitForm is not null &&
                _handledCommitForm.TryGetTarget(out var prev) && ReferenceEquals(prev, commitForm);
            bool sameDir = string.Equals(_handledWorkingDir, workingDir, StringComparison.OrdinalIgnoreCase);
            if (sameForm && sameDir) return;

            // Só marca como tratado quando o form/caixa já existem (true); se a UI ainda está
            // montando (false), deixa para o próximo Idle tentar de novo.
            if (RefreshOpenCommitDialog(workingDir))
            {
                _handledCommitForm = new WeakReference<Form>(commitForm);
                _handledWorkingDir = workingDir;
            }
        }
        catch { /* nunca deixar o plugin derrubar o GitExtensions */ }
    }

    // ── Detecção da escolha do dropdown ─────────────────────────────────────────

    /// <summary>
    /// Assina (uma única vez por instância) o <c>TextChanged</c> da caixa de mensagem para
    /// reconhecer quando o usuário aplica um item do dropdown. Se a instância mudou (novo
    /// FormCommit), desfaz a assinatura anterior antes de assinar a nova.
    /// </summary>
    private void EnsureTextChangedHook(TextBoxBase tb)
    {
        if (_subscribedTextBox is not null && _subscribedTextBox.TryGetTarget(out var prev))
        {
            if (ReferenceEquals(prev, tb)) return;   // já assinada
            try { prev.TextChanged -= OnCommitTextChanged; } catch { /* controle pode ter morrido */ }
        }
        tb.TextChanged += OnCommitTextChanged;
        _subscribedTextBox = new WeakReference<TextBoxBase>(tb);
    }

    private void OnCommitTextChanged(object? sender, EventArgs e)
    {
        if (sender is TextBoxBase tb) DetectTemplateSelection(tb);
    }

    /// <summary>
    /// Se a caixa passou a conter exatamente uma das mensagens que geramos para os itens do
    /// dropdown, o usuário escolheu aquele item: fixa <see cref="_sessionLanguage"/> no idioma
    /// correspondente e marca o texto como NOSSO (para o auto-refresh regenerar nesse idioma a
    /// cada stage/unstage, em vez de tratá-lo como texto do usuário). Não escreve na caixa — só
    /// observa —, então não há risco de loop com o próprio <c>TextChanged</c>.
    /// </summary>
    private void DetectTemplateSelection(TextBoxBase tb)
    {
        if (_templateMessages.Count == 0) return;
        var current = NormalizeNewlines(tb.Text);
        if (current.Length == 0) return;
        if (!_templateMessages.TryGetValue(current, out var forced)) return;

        _sessionLanguage      = forced;
        _lastGeneratedMessage = tb.Text;
    }

    /// <summary>
    /// Remove qualquer efeito de cor/realce do texto da caixa de commit, voltando à cor
    /// padrão do controle. Aplica-se ao RichTextBox usado pelo GitExtensions; outros tipos
    /// de caixa não têm formatação por trecho e são ignorados. O host pode repintar numa
    /// edição posterior — aqui só garantimos a mensagem que acabamos de gerar sem cor.
    /// </summary>
    /// <summary>
    /// Normaliza quebras de linha para <c>\n</c> e remove espaços nas pontas. Usado para comparar
    /// a NOSSA mensagem (gerada com <c>\n</c>) com o que a caixa do host devolve, que pode vir com
    /// <c>\r\n</c>. Sem essa normalização a comparação acusaria diferença e a mensagem que nós
    /// mesmos escrevemos seria tratada como texto do usuário, congelando o auto-refresh.
    /// </summary>
    private static string NormalizeNewlines(string s) =>
        s.Replace("\r\n", "\n").Replace('\r', '\n').Trim();

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
