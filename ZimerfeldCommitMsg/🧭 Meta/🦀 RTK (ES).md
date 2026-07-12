---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: es-ES
atualizado: 2026-07-04
tags: [ferramenta, cli, rtk, meta]
---

# 🦀 RTK — Rust Token Killer

> 🇧🇷 Portugués → [[🦀 RTK]] · 🇺🇸 English → [[🦀 RTK (EN)]]

## 📝 Resumen
Proxy CLI que ahorra **60–90% de tokens** en operaciones de desarrollo. Reescribe comandos automáticamente mediante un hook de Claude Code (p. ej.: `git status` → `rtk git status`, transparente, 0 tokens de overhead).

## ⌨️ Meta-comandos (usar rtk directamente)
```bash
rtk gain              # Muestra analytics de ahorro de tokens
rtk gain --history    # Historial de uso de comandos con ahorro
rtk discover          # Analiza el historial de Claude Code en busca de oportunidades perdidas
rtk proxy <cmd>       # Ejecuta un comando crudo sin filtrar (debug)
```

## ✅ Verificación de instalación
```bash
rtk --version         # rtk X.Y.Z
rtk gain              # Debe funcionar (no "command not found")
which rtk             # Verificar el binario correcto
```

> [!warning] Colisión de nombre
> Si `rtk gain` falla, puede que esté instalado el reachingforthejack/rtk (Rust Type Kit) en lugar del correcto.

## 🔗 Relacionado
- [[🔑 Fatos-Chave (ES)|Hechos Clave]]
- [[👤 Renato (ES)|Renato]]
