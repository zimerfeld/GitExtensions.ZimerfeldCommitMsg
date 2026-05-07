using System.Reflection;
using System.Runtime.InteropServices;

var ge = @"C:\Program Files\GitExtensions";
var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
var allDlls = Directory.GetFiles(ge, "*.dll")
    .Concat(Directory.GetFiles(runtimeDir, "*.dll"))
    .ToArray();

var resolver = new PathAssemblyResolver(allDlls);
using var ctx = new MetadataLoadContext(resolver);
var asm = ctx.LoadFromAssemblyPath(Path.Combine(ge, "GitExtensions.Extensibility.dll"));

// Targets relevantes para commit message plugin
var targets = new[] {
    "IGitPlugin", "GitPluginBase", "IGitUICommands", "GitUIEventArgs",
    "GitUIBaseEventArgs", "IGitModule", "ICommitMessageManager",
    "GitUIPostActionEventArgs", "GitUIPreActionEventArgs"
};

foreach (var t in asm.GetTypes().Where(t => t.IsPublic)
    .Where(t => targets.Contains(t.Name) ||
                t.Name.Contains("Commit") ||
                t.Name.Contains("commit")))
{
    Console.WriteLine($"\n=== {t.FullName} ({(t.IsInterface ? "interface" : t.IsAbstract ? "abstract" : "class")}) ===");
    var members = t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    foreach (var m in members.OrderBy(m => m.Name))
        Console.WriteLine($"  {m}");
}

// Also check GitUIPluginInterfaces
var uiAsm = ctx.LoadFromAssemblyPath(Path.Combine(ge, "GitUIPluginInterfaces.dll"));
Console.WriteLine("\n\n=== GitUIPluginInterfaces exports ===");
foreach (var t in uiAsm.GetTypes().Where(t => t.IsPublic))
{
    Console.WriteLine($"  {t.FullName}");
}
