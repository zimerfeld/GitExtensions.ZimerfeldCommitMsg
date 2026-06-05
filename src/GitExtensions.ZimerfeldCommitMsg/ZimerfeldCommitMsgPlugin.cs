using System.ComponentModel.Composition;
using GitExtensions.Extensibility.Git;
using GitExtensions.Extensibility.Plugins;

namespace GitExtensions.ZimerfeldCommitMsg;

[Export(typeof(IGitPlugin))]
public sealed class ZimerfeldCommitMsgPlugin : GitPluginBase
{
    private const string TemplateKey = "Zimerfeld Commit Msg";

    // Icone exibido no menu Plugins e no dropdown do dialogo de commit
    private static readonly Image? PluginIcon = LoadIcon();

    // Capturado no Register() (roda na UI thread) para marshalling seguro
    private SynchronizationContext? _syncContext;
    private string _lastGeneratedMessage = string.Empty;

    public ZimerfeldCommitMsgPlugin()
    {
        Name        = "ZimerfeldCommitMsg";
        Description = "Gera automaticamente uma mensagem de commit resumindo as mudanças nos arquivos";
        if (PluginIcon is not null) Icon = PluginIcon;
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
        _syncContext = SynchronizationContext.Current;

        // Template no dropdown — só preenche quando o usuário selecionar explicitamente
        gitUiCommands.AddCommitTemplate(
            TemplateKey,
            () => new CommitMessageGenerator(gitUiCommands.Module.WorkingDir).Generate(),
            icon: PluginIcon);

        // Atualiza a mensagem automaticamente sempre que arquivos entram/saem do stage
        gitUiCommands.PostRepositoryChanged += OnPostRepositoryChanged;
    }

    public override void Unregister(IGitUICommands gitUiCommands)
    {
        gitUiCommands.PostRepositoryChanged -= OnPostRepositoryChanged;
        gitUiCommands.RemoveCommitTemplate(TemplateKey);
        _lastGeneratedMessage = string.Empty;
        base.Unregister(gitUiCommands);
    }

    // Plugins menu → abre o diálogo de commit com a mensagem já preenchida
    public override bool Execute(GitUIEventArgs args)
    {
        if (!args.GitModule.IsValidGitWorkingDir())
        {
            MessageBox.Show(
                "Nenhum repositório Git válido encontrado.",
                Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        var message = new CommitMessageGenerator(args.GitModule.WorkingDir).Generate();

        if (string.IsNullOrWhiteSpace(message))
        {
            MessageBox.Show(
                "Nenhuma mudança staged encontrada.\n\nFaça o stage dos arquivos antes de gerar a mensagem.",
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

    private void RefreshOpenCommitDialog(string workingDir)
    {
        try
        {
            // Localiza o diálogo de commit aberto, se houver
            Form? commitForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f.GetType().Name == "FormCommit") { commitForm = f; break; }
            }
            if (commitForm == null) return;

            var tb = FindCommitTextBox(commitForm);
            if (tb == null) return;

            // Não sobrescreve texto digitado manualmente pelo usuário:
            // só atualiza se a caixa estiver vazia ou contiver a última mensagem gerada por nós
            var current = tb.Text.Trim();
            if (current.Length > 0 && current != _lastGeneratedMessage.Trim()) return;

            var msg = new CommitMessageGenerator(workingDir).Generate();
            if (string.IsNullOrEmpty(msg)) return;

            _lastGeneratedMessage = msg;
            tb.Text = msg;
            tb.SelectionStart = 0;
            tb.SelectionLength = 0;
        }
        catch { }
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
