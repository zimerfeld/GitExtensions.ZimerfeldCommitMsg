---
tipo: arquivo
tags: [arquivo, inspector, ferramenta, desenvolvimento, reflection]
arquivo: inspector/Program.cs
linhas: 39
atualizado: 2026-05-22
---

# inspector/Program.cs

Ferramenta de desenvolvimento standalone que introspecta os assemblies do GitExtensions sem carregá-los em runtime.

**Caminho:** `inspector/Program.cs`
**Projeto:** `inspector/inspector.csproj`

---

## Propósito

Quando é necessário descobrir quais interfaces, métodos e eventos o GitExtensions expõe para plugins, o inspector lê os assemblies via `MetadataLoadContext` (reflexão sem execução de código).

Útil para:
- Descobrir assinaturas de métodos do `IGitUICommands`
- Mapear membros de `IGitPlugin`, `GitPluginBase`, `GitUIEventArgs`
- Listar todos os tipos exportados do `GitUIPluginInterfaces.dll`

---

## Como funciona

```csharp
// Carrega assemblies do diretório de instalação do GitExtensions
var allDlls = Directory
    .GetFiles(@"C:\Program Files\GitExtensions", "*.dll")
    .Concat(Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll"));

var resolver = new PathAssemblyResolver(allDlls);
using var ctx = new MetadataLoadContext(resolver);

// Analisa GitExtensions.Extensibility.dll
var asm = ctx.LoadFromAssemblyPath(".../GitExtensions.Extensibility.dll");
```

## Targets inspecionados

```csharp
var targets = new[] {
    "IGitPlugin", "GitPluginBase", "IGitUICommands", "GitUIEventArgs",
    "GitUIBaseEventArgs", "IGitModule", "ICommitMessageManager",
    "GitUIPostActionEventArgs", "GitUIPreActionEventArgs"
};
```

Também lista qualquer tipo cujo nome contenha `"Commit"` ou `"commit"`.

## Saída

Para cada tipo encontrado, imprime:
```
=== Namespace.TypeName (interface|abstract|class) ===
  ReturnType MethodName(Params)
  EventType EventName
  ...
```

E lista todos os tipos públicos de `GitUIPluginInterfaces.dll`.

---

## Como executar

```powershell
cd inspector
dotnet run
```

Não requer Admin. Não modifica nada — somente leitura.

---

## Relacionado

- [[../Sistema/Dependências]]
- [[ZimerfeldCommitMsgPlugin]]
