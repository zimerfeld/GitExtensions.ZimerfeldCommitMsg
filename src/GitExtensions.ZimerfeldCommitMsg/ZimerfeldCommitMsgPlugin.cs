using System.ComponentModel.Composition;
using GitExtensions.Extensibility.Git;
using GitExtensions.Extensibility.Plugins;

namespace GitExtensions.ZimerfeldCommitMsg;

[Export(typeof(IGitPlugin))]
public sealed class ZimerfeldCommitMsgPlugin : GitPluginBase
{
    private const string TemplateKey = "Zimerfeld: Auto-resumo";

    public ZimerfeldCommitMsgPlugin()
    {
        Name        = "ZimerfeldCommitMsg";
        Description = "Gera automaticamente uma mensagem de commit resumindo as mudanças nos arquivos";
    }

    // Called once per repository when the plugin is loaded / enabled
    public override void Register(IGitUICommands gitUiCommands)
    {
        base.Register(gitUiCommands);

        // Adds a template entry to the commit dialog's template dropdown.
        // The lambda runs at selection time, so it always uses the current repository.
        gitUiCommands.AddCommitTemplate(
            TemplateKey,
            () => new CommitMessageGenerator(gitUiCommands.Module.WorkingDir).Generate(),
            icon: null);
    }

    // Called when the plugin is disabled or GitExtensions shuts down — no DLL left behind
    public override void Unregister(IGitUICommands gitUiCommands)
    {
        gitUiCommands.RemoveCommitTemplate(TemplateKey);
        base.Unregister(gitUiCommands);
    }

    // Called from Plugins menu → opens the commit dialog with the generated message pre-filled
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

        // Open the commit dialog with the generated message already filled in
        return args.GitUICommands.StartCommitDialog(args.OwnerForm, commitMessage: message);
    }
}
