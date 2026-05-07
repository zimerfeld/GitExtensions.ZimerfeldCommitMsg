using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace GitExtensions.ZimerfeldCommitMsg;

/// <summary>
/// Gera mensagens de commit no formato Conventional Commits v1.0.0
/// https://www.conventionalcommits.org/en/v1.0.0/
///
/// Formato:  &lt;type&gt;[(&lt;scope&gt;)]: &lt;description&gt;
///           [blank line]
///           [body: bullet list of changed files]
/// </summary>
internal sealed class CommitMessageGenerator
{
    private readonly string _workingDir;

    // ── Extension → semantic category ─────────────────────────────────────────

    private static readonly Dictionary<string, string> ExtCategory = new(StringComparer.OrdinalIgnoreCase)
    {
        // Source code
        ["cs"]    = "source", ["vb"]    = "source", ["fs"]     = "source",
        ["cpp"]   = "source", ["c"]     = "source", ["h"]      = "source",
        ["java"]  = "source", ["py"]    = "source", ["rb"]     = "source",
        ["go"]    = "source", ["rs"]    = "source", ["kt"]     = "source",
        ["swift"] = "source", ["php"]   = "source", ["dart"]   = "source",
        ["lua"]   = "source", ["scala"] = "source", ["groovy"] = "source",
        // Web / UI
        ["js"]     = "web", ["ts"]     = "web", ["jsx"]    = "web",
        ["tsx"]    = "web", ["vue"]    = "web", ["svelte"] = "web",
        ["html"]   = "web", ["htm"]    = "web", ["razor"]  = "web",
        ["cshtml"] = "web", ["css"]    = "web", ["scss"]   = "web",
        ["less"]   = "web", ["sass"]   = "web",
        // Documentation
        ["md"]   = "docs", ["txt"]  = "docs", ["rst"]  = "docs",
        ["adoc"] = "docs", ["wiki"] = "docs", ["pdf"]  = "docs",
        // Build / CI
        ["csproj"]    = "build", ["vbproj"]  = "build", ["fsproj"]  = "build",
        ["sln"]       = "build", ["props"]   = "build", ["targets"] = "build",
        ["dockerfile"]= "build", ["makefile"]= "build", ["cmake"]   = "build",
        ["gradle"]    = "build", ["pom"]     = "build",
        // Config / chore
        ["json"]  = "config", ["xml"]   = "config", ["yaml"] = "config",
        ["yml"]   = "config", ["toml"]  = "config", ["ini"]  = "config",
        ["env"]   = "config", ["config"]= "config", ["conf"] = "config",
        ["lock"]  = "config", ["editorconfig"] = "config",
    };

    // Directories / name patterns that signal test files
    private static readonly string[] TestPathSegments =
        ["test", "tests", "spec", "specs", "__tests__", "unittest", "unittests"];

    // Folders generic enough to skip when choosing scope
    private static readonly HashSet<string> SkippableRoots =
        new(StringComparer.OrdinalIgnoreCase) { "src", "app", "lib", "source", "main", ".", "" };

    // Max files listed in body before truncation
    private const int MaxBodyLines = 15;

    // ── Constructor ────────────────────────────────────────────────────────────

    public CommitMessageGenerator(string workingDir) => _workingDir = workingDir;

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Generates a Conventional Commit message from the current staged diff.
    /// Returns an empty string when nothing is staged.
    /// </summary>
    public string Generate()
    {
        var raw = RunGit("diff", "--cached", "--name-status");
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

        var changes = ParseChanges(raw);
        if (changes.Count == 0) return string.Empty;

        var type  = DetermineType(changes);
        var scope = DetermineScope(changes);
        var desc  = BuildSubject(type, changes);
        var body  = BuildBody(changes);

        // Conventional Commits header:  type(scope): description
        var prefix = scope.Length > 0 ? $"{type}({scope})" : type;
        var header = $"{prefix}: {desc}";

        return body.Length > 0
            ? $"{header}\n\n{body}"
            : header;
    }

    // ── Step 1 — Parse git diff --name-status output ───────────────────────────

    private static List<FileChange> ParseChanges(string output)
    {
        var list = new List<FileChange>();
        foreach (var line in output.Split('\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = line.Split('\t');
            if (parts.Length < 2) continue;

            var statusCode = parts[0].TrimStart();
            var status     = statusCode[0]; // A M D R C T

            // Renames: R90\told_path\tnew_path → take new path
            var path = parts[^1].Replace('\\', '/');

            list.Add(new FileChange(status, path));
        }
        return list;
    }

    // ── Step 2 — Determine Conventional Commits type ───────────────────────────

    private static string DetermineType(List<FileChange> changes)
    {
        var categories = changes.Select(c => GetCategory(c.Path)).ToList();

        // Unanimous categories take priority
        if (categories.All(c => c == "test") || changes.All(IsTestPath)) return "test";
        if (categories.All(c => c == "docs"))                             return "docs";
        if (categories.All(c => c == "build"))                            return "build";
        if (categories.All(c => c == "config"))                           return "chore";

        // Mixed — decide by change type
        int added    = changes.Count(c => c.Status is 'A' or 'C');
        int deleted  = changes.Count(c => c.Status == 'D');
        int modified = changes.Count(c => c.Status is 'M' or 'R' or 'T');

        if (added > 0 && added >= modified && deleted == 0) return "feat";
        if (deleted > 0 && deleted > added && modified == 0) return "chore";
        if (modified > 0 && added == 0 && deleted == 0)     return "fix";

        // Mixed additions + modifications: feat if mostly new, refactor otherwise
        return added > modified ? "feat" : "refactor";
    }

    // ── Step 3 — Derive optional scope from common path ────────────────────────

    private static string DetermineScope(List<FileChange> changes)
    {
        var commonDir = changes.Count == 1
            ? (Path.GetDirectoryName(changes[0].Path) ?? string.Empty).Replace('\\', '/')
            : GetCommonDirectory(changes.Select(c => c.Path).ToList());

        return ExtractScope(commonDir);
    }

    /// <summary>
    /// Returns the most meaningful directory segment as scope, or empty string.
    /// e.g. "src/Services/Auth" → "services"
    ///      "src"               → ""  (too generic)
    ///      ""                  → ""  (files at root)
    /// </summary>
    private static string ExtractScope(string dir)
    {
        if (string.IsNullOrEmpty(dir)) return string.Empty;

        var segments = dir.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Skip generic roots, take the first meaningful segment
        var meaningful = segments.FirstOrDefault(s => !SkippableRoots.Contains(s));
        return meaningful?.ToLowerInvariant() ?? string.Empty;
    }

    // ── Step 4 — Build the subject line (description) ─────────────────────────

    private static string BuildSubject(string type, List<FileChange> changes)
    {
        int added    = changes.Count(c => c.Status is 'A' or 'C');
        int deleted  = changes.Count(c => c.Status == 'D');
        int modified = changes.Count(c => c.Status is 'M' or 'R' or 'T');

        var verb = type switch
        {
            "feat"     => "add",
            "fix"      => "fix",
            "docs"     => "update",
            "test"     => added > modified ? "add" : "update",
            "chore"    => deleted > 0 && added == 0 ? "remove" : "update",
            "build"    => added > modified ? "add" : "update",
            "refactor" => "refactor",
            _          => added >= deleted ? "add" : "update"
        };

        // ── Single file: derive human-readable name from filename ──────────────
        if (changes.Count == 1)
        {
            var name = HumanizeName(Path.GetFileNameWithoutExtension(changes[0].Path));
            return $"{verb} {name}";
        }

        // ── Multiple files ─────────────────────────────────────────────────────
        // Describe what kind of files changed (dominant extension)
        var topExt = changes
            .Select(c => Path.GetExtension(c.Path).TrimStart('.').ToLowerInvariant())
            .Where(e => e.Length > 0)
            .GroupBy(e => e)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? "files";

        var fileNoun = FileNoun(topExt, changes.Count);
        return $"{verb} {fileNoun}";
    }

    // ── Step 5 — Build the body (bullet list of changed files) ────────────────

    private static string BuildBody(List<FileChange> changes)
    {
        // Single file: header is already self-explanatory; no body needed
        if (changes.Count == 1) return string.Empty;

        var sb   = new StringBuilder();
        var list = changes.Take(MaxBodyLines).ToList();

        foreach (var c in list)
        {
            var action = c.Status switch
            {
                'A' or 'C' => "add",
                'D'        => "remove",
                'R'        => "rename",
                _          => "update"
            };
            sb.AppendLine($"- {action} {Path.GetFileName(c.Path)}");
        }

        if (changes.Count > MaxBodyLines)
            sb.AppendLine($"- ... and {changes.Count - MaxBodyLines} more file(s)");

        return sb.ToString().TrimEnd();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static string GetCategory(string path)
    {
        var ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();

        // Dockerfile has no extension — check filename
        if (ext.Length == 0)
        {
            var name = Path.GetFileName(path).ToLowerInvariant();
            if (name is "dockerfile" or "makefile" or "jenkinsfile" or "vagrantfile")
                return "build";
            if (name is ".env" or ".gitignore" or ".gitattributes" or ".editorconfig")
                return "config";
        }

        return ExtCategory.TryGetValue(ext, out var cat) ? cat : "source";
    }

    private static bool IsTestPath(FileChange c)
    {
        var segments = c.Path.ToLowerInvariant()
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Any path segment matches a test folder name
        if (segments.Any(s => TestPathSegments.Contains(s))) return true;

        var fileName = segments[^1];
        return fileName.Contains(".test.") || fileName.Contains(".spec.") ||
               fileName.EndsWith("tests.cs") || fileName.EndsWith("test.cs") ||
               fileName.EndsWith("spec.cs");
    }

    /// <summary>
    /// Converts a PascalCase / camelCase filename into lowercase words.
    /// e.g. "UserAuthService" → "user auth service"
    ///      "README"          → "readme"
    ///      "appsettings"     → "appsettings"
    /// </summary>
    private static string HumanizeName(string name)
    {
        // Insert space before uppercase letters that follow lowercase (PascalCase boundary)
        var spaced = Regex.Replace(name, @"([a-z])([A-Z])", "$1 $2");
        // Treat hyphens and underscores as word separators
        spaced = spaced.Replace('-', ' ').Replace('_', ' ');
        // Collapse multiple spaces and lowercase everything
        return Regex.Replace(spaced, @"\s+", " ").Trim().ToLowerInvariant();
    }

    /// <summary>Returns a human-friendly noun for a file extension.</summary>
    private static string FileNoun(string ext, int count)
    {
        var noun = ext switch
        {
            "cs" or "vb" or "fs"              => "source files",
            "js" or "ts" or "jsx" or "tsx"    => "scripts",
            "md" or "txt" or "rst" or "adoc"  => "documentation",
            "json" or "yaml" or "yml" or "xml" => "configuration files",
            "html" or "htm" or "razor" or "cshtml" => "templates",
            "css" or "scss" or "less" or "sass"    => "stylesheets",
            "csproj" or "sln" or "props"           => "project files",
            _                                       => "files"
        };

        // Prepend count only when relevant (not for uncountable nouns like "documentation")
        var uncountable = new[] { "documentation" };
        return uncountable.Contains(noun) ? noun : $"{count} {noun}";
    }

    private static string GetCommonDirectory(List<string> paths)
    {
        var dirs = paths
            .Select(p => (Path.GetDirectoryName(p) ?? string.Empty)
                .Replace('\\', '/')
                .Split('/', StringSplitOptions.RemoveEmptyEntries))
            .ToList();

        if (dirs.Count == 0 || dirs[0].Length == 0) return string.Empty;

        var common = dirs[0].ToList();
        foreach (var parts in dirs.Skip(1))
        {
            var limit = Math.Min(common.Count, parts.Length);
            var i = 0;
            while (i < limit && common[i] == parts[i]) i++;
            common = common.Take(i).ToList();
            if (common.Count == 0) break;
        }

        return string.Join("/", common);
    }

    // ── Git subprocess ─────────────────────────────────────────────────────────

    private string RunGit(params string[] args)
    {
        try
        {
            var psi = new ProcessStartInfo("git")
            {
                WorkingDirectory       = _workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            foreach (var a in args) psi.ArgumentList.Add(a);

            using var p = Process.Start(psi)!;
            var output  = p.StandardOutput.ReadToEnd();
            p.WaitForExit(8_000);
            return output;
        }
        catch { return string.Empty; }
    }
}

internal sealed record FileChange(char Status, string Path);
