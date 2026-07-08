---
tipo: meta
projeto: GitExtensions.ZimerfeldCommitMsg
lang: en-US
atualizado: 2026-07-04
tags: [ferramenta, cli, rtk, meta]
---

# 🦀 RTK — Rust Token Killer

## 📝 Summary
CLI proxy that saves **60–90% of tokens** on development operations. It rewrites commands automatically via a Claude Code hook (e.g. `git status` → `rtk git status`, transparent, 0 overhead tokens).

## ⌨️ Meta-commands (use rtk directly)
```bash
rtk gain              # Shows token-saving analytics
rtk gain --history    # Command usage history with savings
rtk discover          # Analyzes Claude Code history for missed opportunities
rtk proxy <cmd>       # Runs a raw command without filtering (debug)
```

## ✅ Install check
```bash
rtk --version         # rtk X.Y.Z
rtk gain              # Should work (not "command not found")
which rtk             # Verify the correct binary
```

> [!warning] Name collision
> If `rtk gain` fails, the reachingforthejack/rtk (Rust Type Kit) may be installed instead of the correct one.

## 🔗 Related
- [[🔑 Fatos-Chave (EN)|Key Facts]]
- [[👤 Renato (EN)|Renato]]
