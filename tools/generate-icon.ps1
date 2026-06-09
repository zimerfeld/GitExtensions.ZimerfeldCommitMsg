<#
.SYNOPSIS
    Gera o icone do plugin ZimerfeldCommitMsg via codigo (GDI+ / C#).

.DESCRIPTION
    Cartao arredondado teal com um dicionario/livro aberto ocupando todo o espaco,
    com as paginas rotuladas BR / EN, evocando a documentacao/mensagem de commit
    traduzida. Sem a letra "Z" (removida); o livro e o elemento dominante.

    Renderiza em qualquer tamanho. Por padrao gera:
      - Resources\icon-128.png  (exibido no nuget.org)
      - Resources\icon.png      (16x16, usado na UI do GitExtensions)

.EXAMPLE
    # Use o Windows PowerShell (powershell.exe). No pwsh 7 o System.Drawing e
    # fragmentado (tipos em System.Drawing.Primitives) e o Add-Type inline falha.
    powershell.exe -NoProfile -ExecutionPolicy Bypass -File ./tools/generate-icon.ps1
#>
[CmdletBinding()]
param(
    [string]$OutDir = (Join-Path $PSScriptRoot '..\src\GitExtensions.ZimerfeldCommitMsg\Resources'),
    [int[]]$Sizes = @(128, 16)
)

Add-Type -AssemblyName System.Drawing

$csharp = @'
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

public static class IconGen
{
    // Paleta extraida do icone original (fundo)
    static readonly Color TealDark  = Color.FromArgb(255, 38, 110, 140);
    static readonly Color Teal      = Color.FromArgb(255, 45, 125, 154);
    static readonly Color TealLight = Color.FromArgb(255, 66, 138, 164);
    static readonly Color White     = Color.FromArgb(255, 255, 255, 255);

    // Livro: paginas, pilha e borda
    static readonly Color Cream      = Color.FromArgb(255, 246, 243, 234);
    static readonly Color PageEdge   = Color.FromArgb(255, 198, 206, 212);
    static readonly Color BookBorder = Color.FromArgb(255, 26, 82, 104);

    // Idiomas: PT (verde + amarelo)  /  EN (vermelho + azul)
    static readonly Color Green     = Color.FromArgb(255,   0, 151,  57);
    static readonly Color GreenDark = Color.FromArgb(255,   0,  92,  35);
    static readonly Color Yellow    = Color.FromArgb(255, 255, 205,   0);
    static readonly Color Amber     = Color.FromArgb(255, 173, 130,   0);
    static readonly Color Red       = Color.FromArgb(255, 206,  43,  55);
    static readonly Color RedDark   = Color.FromArgb(255, 130,  20,  30);
    static readonly Color Blue      = Color.FromArgb(255,  18,  86, 160);
    static readonly Color BlueDark  = Color.FromArgb(255,   9,  46,  96);

    static GraphicsPath Rounded(RectangleF r, float radius)
    {
        var p = new GraphicsPath();
        float d = radius * 2;
        p.AddArc(r.X,         r.Y,          d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y,          d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d,   0, 90);
        p.AddArc(r.X,         r.Bottom - d, d, d,  90, 90);
        p.CloseFigure();
        return p;
    }

    // Quadrilatero de uma pagina (esquerda ou direita do vinco central)
    static PointF[] Page(float S, bool left)
    {
        float cx = S * 0.5f;
        float spineTopY = S * 0.09f, spineBotY = S * 0.80f;
        float outX = left ? S * 0.05f : S * 0.95f;
        float topOuterY = S * 0.18f, botOuterY = S * 0.88f;
        return new[] {
            new PointF(cx,   spineTopY),
            new PointF(outX, topOuterY),
            new PointF(outX, botOuterY),
            new PointF(cx,   spineBotY)
        };
    }

    static PointF[] Tr(PointF[] pts, float dx, float dy)
    {
        var r = new PointF[pts.Length];
        for (int i = 0; i < pts.Length; i++) r[i] = new PointF(pts[i].X + dx, pts[i].Y + dy);
        return r;
    }

    // Glifo unico, preenchido + contorno (boa legibilidade do amarelo no branco)
    static void Glyph(Graphics g, string s, FontFamily ff, float emPx,
                      Color fill, Color stroke, float strokeW, float cxp, float cyp, float S)
    {
        using (var path = new GraphicsPath())
        {
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var lr = new RectangleF(cxp - S * 0.25f, cyp - S * 0.25f, S * 0.5f, S * 0.5f);
            path.AddString(s, ff, (int)FontStyle.Bold, emPx, lr, sf);
            using (var b = new SolidBrush(fill)) g.FillPath(b, path);
            if (strokeW > 0)
                using (var p = new Pen(stroke, strokeW) { LineJoin = LineJoin.Round })
                    g.DrawPath(p, path);
        }
    }

    // Renderiza o design completo num master de alta resolucao e reduz para o
    // tamanho pedido com reamostragem de qualidade. Em 16px o desenho direto
    // some; reduzir uma miniatura mantem a identidade visual legivel.
    public static void Save(string path, int size)
    {
        int master = Math.Max(size, 256);
        using (var hi = Render(master))
        using (var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb))
        {
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode     = SmoothingMode.HighQuality;
                g.PixelOffsetMode   = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);
                g.DrawImage(hi, new Rectangle(0, 0, size, size));
            }
            bmp.Save(path, ImageFormat.Png);
        }
    }

    static Bitmap Render(int size)
    {
        float S = size;
        var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        {
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode     = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);

                // Fundo: quadrado arredondado com gradiente teal
                float pad = S * 0.045f;
                var rect = new RectangleF(pad, pad, S - 2 * pad, S - 2 * pad);
                var bg = Rounded(rect, S * 0.22f);
                using (var grad = new LinearGradientBrush(rect, TealLight, TealDark, LinearGradientMode.Vertical))
                    g.FillPath(grad, bg);

                // Recorta tudo o que vem a seguir ao cartão arredondado: o livro ocupa todo o
                // espaço da imagem sem vazar para fora das bordas/cantos do cartão.
                g.SetClip(bg);

                // ===== Livro aberto =====
                float cx = S * 0.5f;
                var leftTop  = Page(S, true);
                var rightTop = Page(S, false);

                // Segundo plano: pilha de paginas (>3 cada lado) deslocada para baixo/fora
                int   stack = 5;
                float ox = S * 0.008f;   // deslocamento lateral (para fora)
                float oy = S * 0.014f;   // deslocamento vertical (para baixo)
                using (var creamBrush = new SolidBrush(Cream))
                using (var edgePen    = new Pen(PageEdge, S * 0.006f) { LineJoin = LineJoin.Round })
                {
                    for (int i = stack; i >= 1; i--)
                    {
                        var lp = Tr(leftTop,  -ox * i, oy * i);
                        var rp = Tr(rightTop,  ox * i, oy * i);
                        g.FillPolygon(creamBrush, lp); g.DrawPolygon(edgePen, lp);
                        g.FillPolygon(creamBrush, rp); g.DrawPolygon(edgePen, rp);
                    }
                }

                // Primeiro plano: as duas paginas abertas (brancas) com borda
                using (var whiteBrush = new SolidBrush(White))
                using (var border = new Pen(BookBorder, S * 0.016f) { LineJoin = LineJoin.Round })
                {
                    g.FillPolygon(whiteBrush, leftTop);
                    g.FillPolygon(whiteBrush, rightTop);
                    g.DrawPolygon(border, leftTop);
                    g.DrawPolygon(border, rightTop);
                    // vinco central (lombada)
                    g.DrawLine(border, cx, S * 0.09f, cx, S * 0.80f);
                }

                // Rotulos: BR (verde + amarelo) | EN (vermelho + azul)
                // ty = centro vertical das paginas (topo ~0.13, base ~0.84 na posicao dos rotulos)
                var ff = new FontFamily("Segoe UI");
                float em = S * 0.22f;
                float ty = S * 0.485f;
                Glyph(g, "B", ff, em, Green,  GreenDark, S * 0.013f, S * 0.215f, ty, S);
                Glyph(g, "R", ff, em, Yellow, Amber,     S * 0.017f, S * 0.360f, ty, S);
                Glyph(g, "E", ff, em, Red,    RedDark,   S * 0.013f, S * 0.640f, ty, S);
                Glyph(g, "N", ff, em, Blue,   BlueDark,  S * 0.013f, S * 0.785f, ty, S);

                g.ResetClip();
                bg.Dispose();
            }
        }
        return bmp;
    }
}
'@

Add-Type -TypeDefinition $csharp -ReferencedAssemblies System.Drawing -ErrorAction Stop

if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Force -Path $OutDir | Out-Null }
$OutDir = (Resolve-Path $OutDir).Path

foreach ($sz in $Sizes) {
    $name = if ($sz -eq 16) { 'icon.png' } else { "icon-$sz.png" }
    $path = Join-Path $OutDir $name
    [IconGen]::Save($path, $sz)
    Write-Host ("Gerado: {0} ({1}x{1})" -f $path, $sz)
}
