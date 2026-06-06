namespace GitExtensions.ZimerfeldCommitMsg.Localization;

/// <summary>
/// Conjunto de mapas e regras específicos de idioma usados pelo gerador de mensagens.
/// Mantém o que NÃO cabe bem em <c>.resx</c>: dicionários grandes (conceitos), verbos do
/// Conventional Commits e conjugação. As strings curtas de UI ficam em <see cref="Strings"/>.
/// </summary>
internal abstract class LanguagePack
{
    public static readonly LanguagePack PtBr = new PtBrLanguagePack();
    public static readonly LanguagePack En   = new EnLanguagePack();

    public static LanguagePack For(MessageLanguage lang) =>
        lang == MessageLanguage.PtBr ? PtBr : En;

    // ── Conceitos de domínio ────────────────────────────────────────────────
    // As CHAVES são idênticas entre idiomas (identificadores em inglês: Auth, Payment…);
    // apenas os VALORES (a frase) mudam. Assim HasConcept se comporta igual nos dois idiomas.
    protected abstract IReadOnlyDictionary<string, string> ConceptPhrases { get; }

    /// <summary>true se o nome tem frase própria no dicionário de conceitos.</summary>
    public bool HasConcept(string raw) => ConceptPhrases.ContainsKey(raw);

    /// <summary>Frase do conceito, se houver; caso contrário <paramref name="fallback"/>.</summary>
    public string MapConcept(string raw, Func<string, string> fallback) =>
        ConceptPhrases.TryGetValue(raw, out var phrase) ? phrase : fallback(raw);

    // ── Verbo do tipo CC (subject) ──────────────────────────────────────────
    public abstract string TypeVerb(string type, bool onlyAdditions, bool hasAdditions, bool hasDeletions);

    // ── Verbo do bullet por status do arquivo (A/C/D/R/…) ───────────────────
    public abstract string StatusVerb(char status);

    // ── Verbo inicial da descrição: detecta/normaliza para imperativo ───────
    /// <summary>
    /// Se <paramref name="desc"/> começa com um verbo conhecido, devolve (verbo capitalizado, resto);
    /// caso contrário (null, desc). Em pt-BR reconhece 3ª pessoa/infinitivo; em inglês, formas verbais comuns.
    /// </summary>
    public abstract (string? Verb, string Remainder) LeadingVerb(string desc);

    // ── Conectores que introduzem justificativa (corta a cláusula principal) ─
    public abstract IReadOnlyList<string> MainClauseConnectors { get; }

    // ── Frase de fallback por categoria de arquivo ──────────────────────────
    public abstract string FallbackPhrase(string category);
}

// ══════════════════════════════════════════════════════════════════════════
//  Português do Brasil
// ══════════════════════════════════════════════════════════════════════════
internal sealed class PtBrLanguagePack : LanguagePack
{
    protected override IReadOnlyDictionary<string, string> ConceptPhrases => _concepts;

    private static readonly Dictionary<string, string> _concepts = new(StringComparer.OrdinalIgnoreCase)
    {
        // Identidade / acesso
        ["Auth"] = "autenticação", ["Authentication"] = "autenticação",
        ["Login"] = "login", ["Logout"] = "logout",
        ["SignIn"] = "login", ["SignUp"] = "cadastro",
        ["Register"] = "cadastro", ["Registration"] = "cadastro",
        ["Password"] = "gerenciamento de senha", ["Token"] = "gerenciamento de token",
        ["Jwt"] = "autenticação JWT", ["Bearer"] = "autenticação por token",
        ["Session"] = "gerenciamento de sessão", ["Permission"] = "permissões",
        ["Permissions"] = "permissões", ["Role"] = "controle de acesso por papel",
        ["Roles"] = "controle de acesso por papel", ["Claim"] = "claims",
        ["OAuth"] = "integração OAuth",
        // Usuário / conta
        ["User"] = "gerenciamento de usuários", ["Users"] = "gerenciamento de usuários",
        ["Account"] = "gerenciamento de conta", ["Profile"] = "perfil do usuário",
        ["Member"] = "associação", ["Customer"] = "gerenciamento de clientes",
        ["Tenant"] = "multi-tenancy",
        // Comércio
        ["Order"] = "processamento de pedidos", ["Cart"] = "carrinho de compras",
        ["Checkout"] = "fluxo de checkout", ["Payment"] = "processamento de pagamento",
        ["Invoice"] = "gerenciamento de faturas", ["Product"] = "gerenciamento de produtos",
        ["Catalog"] = "catálogo de produtos", ["Inventory"] = "estoque",
        ["Shipping"] = "frete", ["Discount"] = "gerenciamento de descontos",
        ["Coupon"] = "gerenciamento de cupons", ["Subscription"] = "assinaturas",
        // Comunicação
        ["Email"] = "serviço de e-mail", ["Mail"] = "serviço de e-mail",
        ["Sms"] = "notificações SMS", ["Notification"] = "notificações",
        ["Push"] = "notificações push", ["Webhook"] = "webhooks",
        ["Message"] = "mensagens", ["Chat"] = "chat",
        // Infraestrutura
        ["Cache"] = "cache", ["Log"] = "registro de log",
        ["Logger"] = "registro de log", ["Logging"] = "registro de log",
        ["Audit"] = "trilha de auditoria", ["Health"] = "verificação de saúde",
        ["Metric"] = "métricas", ["Monitor"] = "monitoramento",
        ["Queue"] = "fila de mensagens", ["Job"] = "tarefas em segundo plano",
        ["Scheduler"] = "agendamento de tarefas", ["Worker"] = "workers em segundo plano",
        ["Event"] = "tratamento de eventos",
        // Dados
        ["Migration"] = "migração de banco de dados", ["Seed"] = "população de dados",
        ["Database"] = "acesso ao banco de dados", ["Db"] = "banco de dados",
        ["Storage"] = "armazenamento", ["File"] = "gerenciamento de arquivos",
        ["Upload"] = "upload de arquivos", ["Download"] = "download de arquivos",
        ["Blob"] = "armazenamento de blobs",
        // Relatórios
        ["Report"] = "relatórios", ["Dashboard"] = "painel",
        ["Analytics"] = "análises", ["Export"] = "exportação de dados",
        ["Import"] = "importação de dados", ["Pdf"] = "geração de PDF",
        ["Excel"] = "exportação para Excel",
        // Busca
        ["Search"] = "busca", ["Filter"] = "filtragem",
        // Configuração
        ["Settings"] = "configurações", ["Config"] = "configuração",
        ["AppSettings"] = "configurações da aplicação",
        // API
        ["Api"] = "API", ["Rest"] = "API REST",
        ["Grpc"] = "serviço gRPC", ["GraphQL"] = "GraphQL",
        ["Swagger"] = "documentação da API", ["Cors"] = "política CORS",
        // Testes
        ["Test"] = "testes unitários", ["Tests"] = "testes unitários",
        ["Integration"] = "testes de integração", ["E2E"] = "testes end-to-end",
        // Docs
        ["Readme"] = "documentação", ["Changelog"] = "changelog", ["Docs"] = "documentação",
        // Plugin / commit
        ["CommitMessage"] = "mensagem de commit", ["CommitMsg"] = "mensagem de commit",
        ["Plugin"] = "plugin", ["GitExtension"] = "extensão do Git",
    };

    public override string TypeVerb(string type, bool onlyAdditions, bool hasAdditions, bool hasDeletions) =>
        type switch
        {
            "feat"     => onlyAdditions ? "Implementa" : "Adiciona",
            "fix"      => "Corrige",
            "refactor" => "Refatora",
            "docs"     => hasAdditions  ? "Documenta"  : "Atualiza",
            "build"    => "Configura",
            "chore"    => hasDeletions  ? "Remove"     : "Configura",
            "test"     => "Adiciona",
            "perf"     => "Otimiza",
            "ci"       => "Configura",
            "style"    => "Padroniza",
            _          => "Atualiza",
        };

    public override string StatusVerb(char status) => status switch
    {
        'A' or 'C' => "Adiciona",
        'D'        => "Remove",
        'R'        => "Renomeia",
        _          => "Atualiza",
    };

    public override (string? Verb, string Remainder) LeadingVerb(string desc)
    {
        var firstSpace = desc.IndexOf(' ');
        if (firstSpace <= 0) return (null, desc);

        var firstWord = desc[..firstSpace].ToLowerInvariant();
        var rest      = desc[(firstSpace + 1)..];

        if (_verbs3rd.Contains(firstWord))
            return (char.ToUpperInvariant(firstWord[0]) + firstWord[1..], rest);

        if (_infinitiveTo3rd.TryGetValue(firstWord, out var imperative))
            return (imperative, rest);

        return (null, desc);
    }

    public override IReadOnlyList<string> MainClauseConnectors { get; } =
    [
        " para ", " pois ", " porque ", " já que ", " a fim de ",
        " quando ", " caso ", " evitando ", " — ", " - ",
    ];

    public override string FallbackPhrase(string category) => category switch
    {
        "docs"   => "documentação",
        "config" => "configuração",
        "build"  => "configuração de build",
        "test"   => "testes unitários",
        "web"    => "componentes web",
        _        => "código-fonte",
    };

    // Presente do indicativo, 3ª pessoa singular — detecta descrição que já começa com verbo.
    private static readonly HashSet<string> _verbs3rd = new(StringComparer.OrdinalIgnoreCase)
    {
        "adiciona", "remove", "corrige", "ajusta", "refatora", "implementa",
        "atualiza", "melhora", "padroniza", "reorganiza", "simplifica", "documenta",
        "valida", "otimiza", "configura", "filtra", "gera", "calcula",
        "extrai", "transforma", "resolve", "expõe", "renderiza", "envia",
        "recebe", "mapeia", "agrupa", "ordena", "une", "divide",
        "busca", "retorna", "converte", "cria", "obtém", "define",
        "lê", "escreve", "processa", "trata", "carrega", "salva",
        "verifica", "corresponde", "encapsula", "estende", "representa", "contém",
        "fornece", "inicializa", "delega", "lança", "constrói",
    };

    // Infinitivo → 3ª pessoa singular presente (capitalizado).
    private static readonly Dictionary<string, string> _infinitiveTo3rd = new(StringComparer.OrdinalIgnoreCase)
    {
        ["adicionar"] = "Adiciona", ["remover"] = "Remove", ["corrigir"] = "Corrige",
        ["ajustar"] = "Ajusta", ["refatorar"] = "Refatora", ["implementar"] = "Implementa",
        ["atualizar"] = "Atualiza", ["melhorar"] = "Melhora", ["padronizar"] = "Padroniza",
        ["reorganizar"] = "Reorganiza", ["simplificar"] = "Simplifica", ["documentar"] = "Documenta",
        ["validar"] = "Valida", ["otimizar"] = "Otimiza", ["configurar"] = "Configura",
        ["filtrar"] = "Filtra", ["gerar"] = "Gera", ["calcular"] = "Calcula",
        ["extrair"] = "Extrai", ["transformar"] = "Transforma", ["resolver"] = "Resolve",
        ["expor"] = "Expõe", ["renderizar"] = "Renderiza", ["enviar"] = "Envia",
        ["receber"] = "Recebe", ["mapear"] = "Mapeia", ["agrupar"] = "Agrupa",
        ["ordenar"] = "Ordena", ["unir"] = "Une", ["dividir"] = "Divide",
        ["buscar"] = "Busca", ["retornar"] = "Retorna", ["converter"] = "Converte",
        ["criar"] = "Cria", ["construir"] = "Constrói", ["obter"] = "Obtém",
        ["definir"] = "Define", ["ler"] = "Lê", ["escrever"] = "Escreve",
        ["processar"] = "Processa", ["tratar"] = "Trata", ["carregar"] = "Carrega",
        ["salvar"] = "Salva", ["verificar"] = "Verifica", ["encapsular"] = "Encapsula",
        ["estender"] = "Estende", ["representar"] = "Representa", ["conter"] = "Contém",
        ["fornecer"] = "Fornece", ["inicializar"] = "Inicializa", ["delegar"] = "Delega",
        ["lançar"] = "Lança", ["mover"] = "Move", ["renomear"] = "Renomeia",
    };
}

// ══════════════════════════════════════════════════════════════════════════
//  Inglês
// ══════════════════════════════════════════════════════════════════════════
internal sealed class EnLanguagePack : LanguagePack
{
    protected override IReadOnlyDictionary<string, string> ConceptPhrases => _concepts;

    private static readonly Dictionary<string, string> _concepts = new(StringComparer.OrdinalIgnoreCase)
    {
        // Identity / access
        ["Auth"] = "authentication", ["Authentication"] = "authentication",
        ["Login"] = "login", ["Logout"] = "logout",
        ["SignIn"] = "sign-in", ["SignUp"] = "sign-up",
        ["Register"] = "registration", ["Registration"] = "registration",
        ["Password"] = "password management", ["Token"] = "token management",
        ["Jwt"] = "JWT authentication", ["Bearer"] = "bearer token authentication",
        ["Session"] = "session management", ["Permission"] = "permissions",
        ["Permissions"] = "permissions", ["Role"] = "role-based access control",
        ["Roles"] = "role-based access control", ["Claim"] = "claims",
        ["OAuth"] = "OAuth integration",
        // User / account
        ["User"] = "user management", ["Users"] = "user management",
        ["Account"] = "account management", ["Profile"] = "user profile",
        ["Member"] = "membership", ["Customer"] = "customer management",
        ["Tenant"] = "multi-tenancy",
        // Commerce
        ["Order"] = "order processing", ["Cart"] = "shopping cart",
        ["Checkout"] = "checkout flow", ["Payment"] = "payment processing",
        ["Invoice"] = "invoice management", ["Product"] = "product management",
        ["Catalog"] = "product catalog", ["Inventory"] = "inventory",
        ["Shipping"] = "shipping", ["Discount"] = "discount management",
        ["Coupon"] = "coupon management", ["Subscription"] = "subscriptions",
        // Communication
        ["Email"] = "email service", ["Mail"] = "email service",
        ["Sms"] = "SMS notifications", ["Notification"] = "notifications",
        ["Push"] = "push notifications", ["Webhook"] = "webhooks",
        ["Message"] = "messaging", ["Chat"] = "chat",
        // Infrastructure
        ["Cache"] = "caching", ["Log"] = "logging",
        ["Logger"] = "logging", ["Logging"] = "logging",
        ["Audit"] = "audit trail", ["Health"] = "health check",
        ["Metric"] = "metrics", ["Monitor"] = "monitoring",
        ["Queue"] = "message queue", ["Job"] = "background jobs",
        ["Scheduler"] = "task scheduling", ["Worker"] = "background workers",
        ["Event"] = "event handling",
        // Data
        ["Migration"] = "database migration", ["Seed"] = "data seeding",
        ["Database"] = "database access", ["Db"] = "database",
        ["Storage"] = "storage", ["File"] = "file management",
        ["Upload"] = "file upload", ["Download"] = "file download",
        ["Blob"] = "blob storage",
        // Reporting
        ["Report"] = "reporting", ["Dashboard"] = "dashboard",
        ["Analytics"] = "analytics", ["Export"] = "data export",
        ["Import"] = "data import", ["Pdf"] = "PDF generation",
        ["Excel"] = "Excel export",
        // Search
        ["Search"] = "search", ["Filter"] = "filtering",
        // Configuration
        ["Settings"] = "settings", ["Config"] = "configuration",
        ["AppSettings"] = "application settings",
        // API
        ["Api"] = "API", ["Rest"] = "REST API",
        ["Grpc"] = "gRPC service", ["GraphQL"] = "GraphQL",
        ["Swagger"] = "API documentation", ["Cors"] = "CORS policy",
        // Tests
        ["Test"] = "unit tests", ["Tests"] = "unit tests",
        ["Integration"] = "integration tests", ["E2E"] = "end-to-end tests",
        // Docs
        ["Readme"] = "documentation", ["Changelog"] = "changelog", ["Docs"] = "documentation",
        // Plugin / commit
        ["CommitMessage"] = "commit message", ["CommitMsg"] = "commit message",
        ["Plugin"] = "plugin", ["GitExtension"] = "Git extension",
    };

    public override string TypeVerb(string type, bool onlyAdditions, bool hasAdditions, bool hasDeletions) =>
        type switch
        {
            "feat"     => onlyAdditions ? "Implement" : "Add",
            "fix"      => "Fix",
            "refactor" => "Refactor",
            "docs"     => hasAdditions  ? "Document"  : "Update",
            "build"    => "Configure",
            "chore"    => hasDeletions  ? "Remove"    : "Configure",
            "test"     => "Add",
            "perf"     => "Optimize",
            "ci"       => "Configure",
            "style"    => "Standardize",
            _          => "Update",
        };

    public override string StatusVerb(char status) => status switch
    {
        'A' or 'C' => "Add",
        'D'        => "Remove",
        'R'        => "Rename",
        _          => "Update",
    };

    public override (string? Verb, string Remainder) LeadingVerb(string desc)
    {
        var firstSpace = desc.IndexOf(' ');
        if (firstSpace <= 0) return (null, desc);

        var firstWord = desc[..firstSpace].ToLowerInvariant();
        var rest      = desc[(firstSpace + 1)..];

        // Normaliza formas verbais comuns (3ª pessoa/base) para o imperativo capitalizado.
        if (_verbToImperative.TryGetValue(firstWord, out var imperative))
            return (imperative, rest);

        return (null, desc);
    }

    public override IReadOnlyList<string> MainClauseConnectors { get; } =
    [
        " to ", " because ", " in order to ", " so that ",
        " when ", " if ", " avoiding ", " — ", " - ",
    ];

    public override string FallbackPhrase(string category) => category switch
    {
        "docs"   => "documentation",
        "config" => "configuration",
        "build"  => "build configuration",
        "test"   => "unit tests",
        "web"    => "web components",
        _        => "source code",
    };

    // Forma verbal (3ª pessoa ou base) → imperativo capitalizado (convenção CC).
    private static readonly Dictionary<string, string> _verbToImperative = new(StringComparer.OrdinalIgnoreCase)
    {
        ["adds"] = "Add", ["add"] = "Add",
        ["removes"] = "Remove", ["remove"] = "Remove",
        ["fixes"] = "Fix", ["fix"] = "Fix",
        ["adjusts"] = "Adjust", ["adjust"] = "Adjust",
        ["refactors"] = "Refactor", ["refactor"] = "Refactor",
        ["implements"] = "Implement", ["implement"] = "Implement",
        ["updates"] = "Update", ["update"] = "Update",
        ["improves"] = "Improve", ["improve"] = "Improve",
        ["standardizes"] = "Standardize", ["standardize"] = "Standardize",
        ["reorganizes"] = "Reorganize", ["reorganize"] = "Reorganize",
        ["simplifies"] = "Simplify", ["simplify"] = "Simplify",
        ["documents"] = "Document", ["document"] = "Document",
        ["validates"] = "Validate", ["validate"] = "Validate",
        ["optimizes"] = "Optimize", ["optimize"] = "Optimize",
        ["configures"] = "Configure", ["configure"] = "Configure",
        ["filters"] = "Filter", ["filter"] = "Filter",
        ["generates"] = "Generate", ["generate"] = "Generate",
        ["calculates"] = "Calculate", ["calculate"] = "Calculate",
        ["extracts"] = "Extract", ["extract"] = "Extract",
        ["transforms"] = "Transform", ["transform"] = "Transform",
        ["resolves"] = "Resolve", ["resolve"] = "Resolve",
        ["exposes"] = "Expose", ["expose"] = "Expose",
        ["renders"] = "Render", ["render"] = "Render",
        ["sends"] = "Send", ["send"] = "Send",
        ["receives"] = "Receive", ["receive"] = "Receive",
        ["maps"] = "Map", ["map"] = "Map",
        ["groups"] = "Group", ["group"] = "Group",
        ["sorts"] = "Sort", ["sort"] = "Sort",
        ["joins"] = "Join", ["join"] = "Join",
        ["splits"] = "Split", ["split"] = "Split",
        ["searches"] = "Search", ["search"] = "Search",
        ["returns"] = "Return", ["return"] = "Return",
        ["converts"] = "Convert", ["convert"] = "Convert",
        ["creates"] = "Create", ["create"] = "Create",
        ["builds"] = "Build", ["build"] = "Build",
        ["gets"] = "Get", ["get"] = "Get",
        ["sets"] = "Set", ["set"] = "Set",
        ["reads"] = "Read", ["read"] = "Read",
        ["writes"] = "Write", ["write"] = "Write",
        ["parses"] = "Parse", ["parse"] = "Parse",
        ["handles"] = "Handle", ["handle"] = "Handle",
        ["loads"] = "Load", ["load"] = "Load",
        ["saves"] = "Save", ["save"] = "Save",
        ["checks"] = "Check", ["check"] = "Check",
        ["wraps"] = "Wrap", ["wrap"] = "Wrap",
        ["extends"] = "Extend", ["extend"] = "Extend",
        ["represents"] = "Represent", ["represent"] = "Represent",
        ["contains"] = "Contain", ["contain"] = "Contain",
        ["provides"] = "Provide", ["provide"] = "Provide",
        ["initializes"] = "Initialize", ["initialize"] = "Initialize",
        ["delegates"] = "Delegate", ["delegate"] = "Delegate",
        ["throws"] = "Throw", ["throw"] = "Throw",
        ["moves"] = "Move", ["move"] = "Move",
        ["renames"] = "Rename", ["rename"] = "Rename",
    };
}
