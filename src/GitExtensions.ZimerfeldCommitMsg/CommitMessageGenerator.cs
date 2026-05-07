using System.Diagnostics;

namespace GitExtensions.ZimerfeldCommitMsg;

internal sealed class CommitMessageGenerator
{
    private readonly string _workingDir;

    // File extension → semantic category
    private static readonly Dictionary<string, string> ExtCategory = new(StringComparer.OrdinalIgnoreCase)
    {
        // Source code
        ["cs"]   = "source", ["vb"]  = "source", ["fs"]   = "source",
        ["cpp"]  = "source", ["c"]   = "source",  ["h"]    = "source",
        ["java"] = "source", ["py"]  = "source",  ["rb"]   = "source",
        ["go"]   = "source", ["rs"]  = "source",  ["kt"]   = "source",
        ["swift"]= "source", ["php"] = "source",  ["dart"] = "source",
        // Web / UI
        ["js"]   = "web",    ["ts"]   = "web",    ["jsx"]  = "web",
        ["tsx"]  = "web",    ["vue"]  = "web",    ["html"] = "web",
        ["htm"]  = "web",    ["css"]  = "web",    ["scss"] = "web",
        ["less"] = "web",    ["razor"]= "web",    ["cshtml"]="web",
        // Docs
        ["md"]   = "docs",   ["txt"]  = "docs",   ["rst"]  = "docs",
        ["adoc"] = "docs",
        // Config / build
        ["json"] = "config", ["xml"]  = "config", ["yaml"] = "config",
        ["yml"]  = "config", ["toml"] = "config", ["ini"]  = "config",
        ["env"]  = "config", ["props"]= "config", ["targets"]="config",
        ["csproj"]="config", ["sln"]  = "config", ["config"]="config",
        ["dockerfile"]="config",
        // Tests (by extension suffix pattern — also handled by path)
        ["spec"] = "test",   ["test"] = "test",
    };

    public CommitMessageGenerator(string workingDir) => _workingDir = workingDir;

    // ── Public API ─────────────────────────────────────────────────────────────

    public string Generate()
    {
        var raw = RunGit("diff", "--cached", "--name-status");
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;

        var changes = ParseChanges(raw);
        if (changes.Count == 0)
            return string.Empty;

        return BuildMessage(changes);
    }

    // ── Parsing ────────────────────────────────────────────────────────────────

    private static List<FileChange> ParseChanges(string output)
    {
        var list = new List<FileChange>();
        foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = line.Split('\t');
            if (parts.Length < 2) continue;

            var statusCode = parts[0].TrimStart();
            var status = statusCode[0]; // A M D R C

            // Renames: R90\told_path\tnew_path — take new path
            var path = parts[^1].Replace('\\', '/');

            list.Add(new FileChange(status, path));
        }
        return list;
    }

    // ── Message building ───────────────────────────────────────────────────────

    private static string BuildMessage(List<FileChange> changes)
    {
        var type = DetermineType(changes);
        var desc = BuildDescription(changes);
        return $"{type}: {desc}";
    }

    private static string DetermineType(List<FileChange> changes)
    {
        var categories = changes.Select(c => GetCategory(c.Path)).ToList();

        if (categories.All(c => c == "test")   || changes.All(IsTestPath))  return "test";
        if (categories.All(c => c == "docs"))                                return "docs";
        if (categories.All(c => c == "config"))                              return "chore";

        int added    = changes.Count(c => c.Status is 'A' or 'C');
        int deleted  = changes.Count(c => c.Status == 'D');
        int modified = changes.Count(c => c.Status is 'M' or 'R');

        if (added > modified && added >= deleted) return "feat";
        if (deleted > added && deleted > modified) return "chore";
        if (modified > 0 && added == 0 && deleted == 0) return "fix";
        return "refactor";
    }

    private static string BuildDescription(List<FileChange> changes)
    {
        int added   = changes.Count(c => c.Status is 'A' or 'C');
        int deleted = changes.Count(c => c.Status == 'D');

        var verb = added > deleted ? "add"
                 : deleted > added ? "remove"
                 : "update";

        // Single file — use exact name
        if (changes.Count == 1)
            return $"{verb} {Path.GetFileName(changes[0].Path)}";

        var paths = changes.Select(c => c.Path).ToList();
        var commonDir = GetCommonDirectory(paths);
        var dirSuffix = string.IsNullOrEmpty(commonDir) ? string.Empty : $" in {commonDir}";

        // Dominant extension
        var topExt = paths
            .Select(p => Path.GetExtension(p).TrimStart('.').ToLowerInvariant())
            .Where(e => e.Length > 0)
            .GroupBy(e => e)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        var fileDesc = topExt is { Length: > 0 } ? $".{topExt} files" : "files";

        // Show change breakdown when mixed
        var parts = new List<string>();
        if (added   > 0) parts.Add($"{added} added");
        if (changes.Count(c => c.Status is 'M' or 'R') > 0)
            parts.Add($"{changes.Count(c => c.Status is 'M' or 'R')} modified");
        if (deleted > 0) parts.Add($"{deleted} removed");

        var breakdown = parts.Count > 1 ? $"({string.Join(", ", parts)}) " : $"{changes.Count} ";

        return $"{verb} {breakdown}{fileDesc}{dirSuffix}";
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static string GetCategory(string path)
    {
        var ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
        return ExtCategory.TryGetValue(ext, out var cat) ? cat : "source";
    }

    private static bool IsTestPath(FileChange c)
    {
        var p = c.Path.ToLowerInvariant();
        return p.Contains("/test/") || p.Contains("/tests/") || p.Contains("/spec/") ||
               p.Contains(".test.") || p.Contains(".spec.") ||
               p.EndsWith("tests.cs") || p.EndsWith("test.cs");
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
                WorkingDirectory            = _workingDir,
                RedirectStandardOutput      = true,
                RedirectStandardError       = true,
                UseShellExecute             = false,
                CreateNoWindow              = true,
                StandardOutputEncoding      = System.Text.Encoding.UTF8
            };
            foreach (var a in args) psi.ArgumentList.Add(a);

            using var p = Process.Start(psi)!;
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit(8_000);
            return output;
        }
        catch { return string.Empty; }
    }
}

internal sealed record FileChange(char Status, string Path);
