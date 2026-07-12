# GitExtensions.ZimerfeldCommitMsg

![Icone](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/src/GitExtensions.ZimerfeldCommitMsg/Resources/icon-128.png)

[![NuGet version](https://img.shields.io/nuget/v/GitExtensions.ZimerfeldCommitMsg?style=for-the-badge&logo=nuget&label=NuGet)](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg/) &nbsp; [![NuGet downloads](https://img.shields.io/nuget/dt/GitExtensions.ZimerfeldCommitMsg?style=for-the-badge&logo=nuget&label=Downloads)](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg/)

Este plugin se construye y se mantiene en mi tiempo libre. Si te ahorra tiempo en cada commit, un patrocinio ayuda a mantenerlo actualizado para las nuevas versiones de GitExtensions. 💜

[![GitHub Sponsor](https://img.shields.io/badge/Sponsor-zimerfeld-EA4AAA?style=for-the-badge&logo=githubsponsors&logoColor=white)](https://github.com/sponsors/zimerfeld) &nbsp;&nbsp;&nbsp;&nbsp; [![Ko-fi](https://img.shields.io/badge/Ko--fi-Buy%20me%20a%20coffee-FF5E2B?style=for-the-badge&logo=ko-fi&logoColor=white)](https://ko-fi.com/C0D621FCGD)

**Versión:** 1.0.97
**Actualizado:** 2026-07-04

Plugin para **[GitExtensions](https://gitextensions.github.io/)** que genera automáticamente mensajes de commit analizando el contenido real de los cambios en stage. Los cambios se clasifican por los tipos de **Conventional Commits** (`feat`/`fix`/`docs`/`test`/`chore`/`build`/`refactor`) para elegir el **verbo** adecuado, y el mensaje resultante es una **frase encabezada por un verbo** seguida de un cuerpo con viñetas — **sin** el prefijo `tipo:`. **Multilingüe**: genera la salida en **portugués de Brasil, inglés o español**, detectado automáticamente a partir del idioma del sistema operativo, con **anulación manual** en la configuración del plugin.

![Screenshot](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUsage.png)

[English](README.en-US.md) | [Português-BR](README.pt-BR.md) | [Español](README.es-ES.md)

[...Más información](https://www.nuget.org/packages/GitExtensions.ZimerfeldCommitMsg "Más información sobre el paquete GitExtensions.ZimerfeldTree")

---

## Funcionalidades de alto nivel

- **Generación automática** del mensaje de commit a partir del contenido real del diff en stage, no solo de los nombres de archivo.
- **Verbo guiado por Conventional Commits** — clasifica los cambios en tipos (`feat`, `fix`, `docs`, `test`, `chore`, `build`, `refactor`) y antepone el **verbo** correspondiente (3ª persona del presente en pt-BR / imperativo en inglés). El tipo en sí **no** aparece en el mensaje.
- **Multilingüe (Portugués / Inglés / Español)** — idioma seleccionado automáticamente a partir del SO, con un selector de anulación manual.
- **Dos estrategias de contenido**: basada en comentarios del diff (principal) y basada en nombres de archivo (fallback). La extracción de comentarios reconoce muchas sintaxis — `//`, `///`, bloques C-style `/* */` `/** */`, JSDoc `* `, HTML `<!-- -->`, SQL/Lua `--`, VB `'` y `#`.
- **Vocabulario por repositorio** — un archivo opcional `.zimerfeldcommitmsg.json` amplía el vocabulario conocido/rechazado y las frases de concepto sin recompilar.
- **Cuerpo con viñetas** — hasta 5 frases de una línea, cada una resumiendo el cambio más significativo de un archivo; **siempre al menos una viñeta**, incluso con un único archivo modificado.
- **Traducción inglés → español** de los comentarios (solo cuando la salida es es-ES); para la salida en inglés, los comentarios se mantienen tal cual.
- **Saneamiento de las frases** — descarta comentarios con **delimitadores desbalanceados** (`()`, `[]`, `{}`, comillas `"` `'` `` ` ``, `<>`) o que **terminen en un conector suelto** (`of`, `to`, `with`…); entre los candidatos válidos elige el de **mayor calidad** (no el más largo).
- **Nunca vacío** — cuando hay archivos en stage, siempre produce al menos la línea-resumen (`<verbo> N archivos`).
- **Tres modos de integración**: plantilla en el diálogo de commit, menú Plugins y autorrelleno (al abrir el diálogo y al hacer stage/unstage).
- **No destructivo** — nunca sobrescribe el texto escrito manualmente por el usuario.

---

## Multilingüe (Portugués / Inglés / Español)

El plugin genera todo el mensaje (descripción, cuerpo y verbos) **en el idioma seleccionado**, y también localiza los mensajes de la interfaz (diálogos de aviso).

### Selección del idioma

Hay **dos formas** de elegir el idioma, usando las mismas etiquetas bilingües para que sigan siendo claras independientemente del idioma del sistema:

**1. En el dropdown de plantillas de la pantalla de commit** — cuatro elementos, uno por idioma (elección rápida por commit):

```text
Zimerfeld Commit Msg — Automático/Automatic
Zimerfeld Commit Msg — Português/Portuguese
Zimerfeld Commit Msg — Inglês/English
Zimerfeld Commit Msg — Espanhol/Español
```

![Dropdown de plantillas de commit con los elementos de idioma](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUsage.png)

**2. En Configuración → Plugins → ZimerfeldCommitMsg** — el selector **"Idioma da mensagem / Message language"** define el **valor por defecto** usado por el menú Plugins y el autorrefresco.

| Opción                 | Comportamiento                                                                                                                          |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| `Automático/Automatic` | **Por defecto.** Detecta a partir del idioma del sistema operativo/GitExtensions (`pt-*` → portugués; `es-*` → español; cualquier otro → inglés). |
| `Português/Portuguese` | Fuerza la salida en portugués de Brasil.                                                                                               |
| `Inglês/English`       | Fuerza la salida en inglés.                                                                                                            |
| `Espanhol/Español`     | Fuerza la salida en español (España).                                                                                                 |

> Elegir un elemento en el **dropdown** fija ese idioma para el **autorrefresco** mientras el diálogo está abierto (tiene prioridad sobre la configuración/SO). El selector de Configuración define el valor por defecto usado cuando no se ha elegido ningún elemento del dropdown (y por el menú Plugins). La detección automática usa `CultureInfo.CurrentUICulture`.
>
> **Nota:** el nodo **ZimerfeldCommitMsg** solo aparece en el árbol de **Configuración → Plugins** después de instalar la DLL con el selector (≥ 1.0.36) y reiniciar GitExtensions.

### Ejemplo lado a lado

| Português-BR                            | English                    | Español                          |
| --------------------------------------- | -------------------------- | -------------------------------- |
| `Implementa autenticação`               | `Implement authentication` | `Implementa autenticación`       |
| `- Adiciona autenticação`               | `- Add authentication`     | `- Añade autenticación`          |
| `- Adiciona processamento de pagamento` | `- Add payment processing` | `- Añade procesamiento de pagos` |
| `- Adiciona gerenciamento de token`     | `- Add token management`   | `- Añade gestión de tokens`      |

---

## Modos de integración

### Plantilla en el diálogo de commit

El dropdown de plantillas de la ventana de commit incluye un elemento por idioma — **"Zimerfeld Commit Msg — Automático/Automatic"**, **"— Português/Portuguese"**, **"— Inglês/English"** y **"— Espanhol/Español"**. Selecciona uno y el mensaje se genera en ese idioma y se rellena automáticamente en el campo de texto.

> **Abrir** el dropdown genera los cuatro idiomas en el momento (mensajes frescos a partir del stage actual); **hacer clic** en un elemento **reemplaza** el contenido del campo por el mensaje de ese idioma — incluido el texto escrito manualmente. (Esto difiere del autorrefresco, que preserva el texto del usuario.)

### Menú Plugins

Abre **Plugins → ZimerfeldCommitMsg**. El diálogo de commit se abre con el mensaje ya rellenado.

### Autorrelleno al abrir y al hacer stage/unstage

Cuando **abres** el diálogo de commit con archivos ya en stage, el mensaje se rellena automáticamente (sin necesidad de tocar el stage). Y mientras el diálogo está abierto, se actualiza siempre que se añaden o quitan archivos del stage. El texto escrito manualmente nunca se sobrescribe.

---

## Formato del mensaje generado

```text
<Contexto> - <Verbo> <N> <archivos> (<tipos>)

- <viñeta 1>
- <viñeta 2>
```

- **Prefijo de contexto (hasta 5 palabras)** — la primera línea comienza con un concepto derivado del **nombre del archivo de mayor impacto**, seguido de ` - `. En **inglés** es el concepto humanizado (p. ej. `OverlayController` → `Overlay`). En **pt-BR** y **es-ES** es una frase de acción nominalizada: el sustantivo de acción por status (`Adición`/`Eliminación`/`Actualización`/`Renombrado` en español) + `de` + concepto traducido y reordenado (p. ej. `New Text Document` eliminado → `Eliminación de documento de texto`). Esto garantiza que el título **nunca** sea genérico (p. ej. nunca solo `Add 1 file (fix)`), dándole identidad al commit para poder distinguirlo después. Se omite únicamente cuando ningún archivo produce un concepto legible.
- **Resumen consolidado** — tras el contexto viene el **verbo** del tipo dominante + el **número de archivos** + la lista de **tipos** involucrados (p. ej. `Implement 4 files (feat, build, docs)`).
- **Sin prefijo `tipo:`** — el tipo de Conventional Commits **no se imprime**; solo selecciona el verbo, como `Implement`, `Fix` o `Update`.
- **Sin scope** — evita la redundancia con el nombre del proyecto.
- **Sin resaltado de color** — usa `git diff --no-color` para evitar códigos ANSI.
- **Límite de 80 caracteres** en la primera línea, recortando en el último espacio y añadiendo `…`.
- **Descripción y verbos en el idioma activo** (portugués de Brasil, inglés o español).
- **Cuerpo siempre presente** — hasta 5 viñetas de una línea; siempre que haya algún archivo en stage, produce **al menos una** viñeta (incluso con un único archivo).

![Mensaje de commit generado en el diálogo de commit de GitExtensions](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotCommitMsg.png)

### Tipos detectados (definen el verbo)

Cada archivo en stage recibe un tipo. El **verbo** de la primera línea proviene del tipo de **mayor prioridad** entre todos los archivos (orden: `feat` → `fix` → `refactor` → `perf` → `test` → `build` → `ci` → `chore` → `docs` → `style`). **El tipo no se imprime** — solo selecciona el verbo.

| Tipo       | Asignado a un archivo cuando…                                                            |
| ---------- | ---------------------------------------------------------------------------------------- |
| `feat`     | archivo de código **añadido** (status `A`/`C`, categoría source/web)                     |
| `fix`      | archivo de código **modificado/renombrado** (status `M`/`R`/`T`)                         |
| `docs`     | archivo de documentación (`.md`, `.txt`, `.rst`, `.adoc`)                                |
| `test`     | ruta de test (carpeta `test`/`tests`/`spec` o sufijo `Test`/`Spec`)                      |
| `chore`    | archivo de configuración (`.json`, `.yml`, etc.) **o** cualquier archivo **eliminado** (status `D`) |
| `build`    | archivo de build (`.csproj`, `.sln`, `Dockerfile`, etc.)                                 |
| `refactor` | resto de casos sin un patrón claro                                                       |

---

## Cómo se genera el mensaje

### Estrategia 1 — Basada en comentarios del diff (principal)

Ejecuta `git diff --cached --no-color` y recoge las líneas de **comentario** que fueron **añadidas** (`+`) o **eliminadas** (`-`). Los comentarios añadidos se priorizan por **categoría del archivo** (source = 4 > web = 3 > build = 2 / config = 1 / docs = 1; archivos de test = 0); los eliminados reciben un nivel de prioridad menor. Se usan hasta **5 comentarios** (uno por archivo, del más al menos relevante). Dentro de un mismo archivo, en lugar de tomar el comentario más largo, el plugin **puntúa** los candidatos — premiando una frase cerrada, una longitud equilibrada (~20–72 chars) y un verbo inicial — y elige el de mayor puntuación.

#### Patrones reconocidos

| Sintaxis                           | Lenguajes                            |
| ---------------------------------- | ------------------------------------ |
| `// texto` o `/// texto`           | C#, Java, JavaScript, TypeScript, Go |
| `/* texto */`, `/** texto */`      | Bloque C-style (C#, Java, JS, C/C++…) |
| `* texto`                          | Continuación de bloque JSDoc/Javadoc |
| `<!-- texto -->`                   | HTML, XML                            |
| `-- texto`                         | SQL, Lua, Haskell, Ada               |
| `' texto`                          | VB, VBScript                         |
| `# texto`                          | Python, Shell, YAML, Ruby            |

#### Comentarios rechazados

| Condición                        | Ejemplo rechazado               |
| -------------------------------- | ------------------------------- |
| Separador visual                 | `// ─────────────────────`      |
| Etiqueta de documentación XML    | `/// <summary>`                 |
| Código comentado (tiene `{` `}`) | `// if (x) { return; }`         |
| Código comentado (llamada a método) | `// method(argument)`        |
| Demasiado corto (< 10 chars)     | `// ok`                         |
| Sin espacios (no es una frase)   | `// TODO`                       |
| **Delimitador desbalanceado**    | `// builds the tree (recursive` |
| **Termina en un conector suelto** | `// maps the token to`         |

#### Cómo se usan los comentarios

El comentario más impactante se convierte en la **descripción** de la primera línea (con el verbo inicial normalizado a 3ª persona/imperativo). Los comentarios restantes aparecen en el **cuerpo** como elementos de lista:

```text
Validate the token before processing the request

- Filter requests without an authentication header
```

> Si el comentario seleccionado contiene un conector de justificación (`para`, `pois`, `porque`, …), la parte posterior al conector se descarta de la primera línea, y la descripción usa la frase funcional de los nombres de archivo (Estrategia 2) para evitar repetir las viñetas.

Cuando la salida es **español**, los comentarios en inglés se traducen automáticamente antes de usarse (y se descartan si la traducción sigue teniendo más de un 25% de inglés). Cuando la salida es **inglés**, los comentarios se mantienen tal cual (y los comentarios en portugués permanecen en portugués). Los nombres de rama (`feature/…`, `release/…`) y los tipos de Conventional Commits se preservan durante la traducción.

---

### Estrategia 2 — Basada en nombres de archivo (fallback)

Se usa cuando no se encuentra ningún comentario válido en el diff.

#### Extracción de conceptos semánticos

Para cada archivo en stage, el nombre (sin extensión) pasa por:

1. **Eliminación del prefijo de interfaz** — `IUserService` → `UserService`
2. **Eliminación del sufijo arquitectónico** (coincidencia más larga primero):

   `ServiceTests`, `ControllerTests`, `RepositoryTests` …
   `Service`, `Controller`, `Repository`, `Manager`, `Handler`, `Generator` …
   `Middleware`, `Validator`, `Mapper`, `Factory`, `Builder` …
   `Tests`, `Test`, `Spec`, `Mock`, `Command`, `Query`, `Event` …
   `Dto`, `ViewModel`, `Model`, `Entity`, `Config`, `Settings` …
   `Facade`, `Adapter`, `Client`, `Endpoint`, `Base`, `Impl` …

3. **Filtros de calidad:**
   - Nombre con un punto en el stem → rechazado (por ejemplo, `GitExtensions.ZimerfeldCommitMsg`)
   - **Vocabulario rechazado** (`RejectedVocabulary`) — si **cualquier** palabra del nombre está en este conjunto, el nombre se rechaza (tiene prioridad sobre todo). Para nombres propios/namespaces (p. ej. `zimerfeld`, `git`, `extensions`). Amplíalo por proyecto.
   - Nombre con 3+ palabras → rechazado como namespace, **excepto** cuando es un concepto del diccionario **o** todas sus palabras son vocabulario reconocido (`KnownVocabulary` + diccionario de traducción + conceptos). Ejemplo: `New Text Document` **pasa** (new/text/document son conocidos); `ZimerfeldCommitMsg` **no** (`zimerfeld` está en el vocabulario rechazado). Amplía `KnownVocabulary` para cubrir más términos.

#### Vocabulario por repositorio (`.zimerfeldcommitmsg.json`)

Puedes ampliar la extracción de conceptos **sin recompilar** colocando un archivo opcional
`.zimerfeldcommitmsg.json` en la raíz del repositorio:

```json
{
  "knownVocabulary":    ["widget", "gadget"],
  "rejectedVocabulary": ["acme", "contoso"],
  "concepts":           { "widget": "componente", "gadget": "accesorio" }
}
```

- **`knownVocabulary`** — palabras extra aceptadas como parte de un nombre descriptivo (añadidas al `KnownVocabulary` integrado; se aplica a todos los idiomas).
- **`rejectedVocabulary`** — palabras que fuerzan el rechazo del nombre como concepto (nombres propios/namespaces del proyecto; añadidas al `RejectedVocabulary` integrado).
- **`concepts`** — concepto-palabra → frase usada en el prefijo nominal del título (tiene prioridad sobre el diccionario integrado).

Un archivo ausente o malformado se ignora silenciosamente — nunca rompe la generación.

#### Mapeo de concepto → frase (ejemplos)

| Concepto extraído         | Frase en pt-BR                            | Frase en es-ES                           |
| ------------------------- | ----------------------------------------- | ---------------------------------------- |
| `Auth` / `Authentication` | autenticação                              | autenticación                            |
| `User` / `Users`          | gerenciamento de usuários                 | gestión de usuarios                      |
| `Token` / `Jwt`           | gerenciamento de token / autenticação JWT | gestión de tokens / autenticación JWT    |
| `Payment`                 | processamento de pagamento                | procesamiento de pagos                   |
| `Order`                   | processamento de pedidos                  | procesamiento de pedidos                 |
| `Notification`            | notificações                              | notificaciones                           |
| `Cache`                   | cache                                     | caché                                    |
| `Migration`               | migração de banco de dados                | migración de base de datos               |
| `Report`                  | relatórios                                | informes                                 |
| `CommitMessage`           | mensagem de commit                        | mensaje de commit                        |

#### Verbos por tipo

El verbo se selecciona por tipo y, en algunos casos, por el contexto del cambio:

| Tipo       | Verbo (pt-BR)         | Verbo (en)         | Verbo (es-ES)          | Condición                                                        |
| ---------- | --------------------- | ------------------ | ---------------------- | ---------------------------------------------------------------- |
| `feat`     | Implementa / Adiciona | Implement / Add    | Implementa / Añade     | `Implementa` cuando solo hay adiciones; `Añade` en caso contrario |
| `fix`      | Corrige               | Fix                | Corrige                | —                                                                |
| `refactor` | Refatora              | Refactor           | Refactoriza            | —                                                                |
| `docs`     | Documenta / Atualiza  | Document / Update  | Documenta / Actualiza  | `Documenta` cuando hay adiciones; `Actualiza` en caso contrario  |
| `build`    | Configura             | Configure          | Configura              | —                                                                |
| `chore`    | Remove / Configura    | Remove / Configure | Elimina / Configura    | `Elimina` cuando hay eliminaciones; `Configura` en caso contrario |
| `test`     | Adiciona              | Add                | Añade                  | —                                                                |
| `perf`     | Otimiza               | Optimize           | Optimiza               | —                                                                |
| `ci`       | Configura             | Configure          | Configura              | —                                                                |
| `style`    | Padroniza             | Standardize        | Estandariza            | —                                                                |

> Si la descripción ya comienza con un verbo conocido (por ejemplo, el comentario `filter stems…`), se **normaliza** (pt-BR: 3ª persona del presente → `Filtra`; en: imperativo → `Filter`; es: 3ª persona del presente → `Filtra`) en lugar de anteponer un nuevo verbo basado en el tipo.

#### Cuerpo del mensaje

El cuerpo lista hasta **5 viñetas** — **al menos una, incluso con un único archivo** — cada una con una frase de una línea que resume el cambio más significativo de un archivo (ordenadas por relevancia del archivo). El verbo de cada viñeta sigue el status de git (añadido/eliminado/renombrado/modificado):

```text
- Añade autenticación
- Añade procesamiento de pagos
- Añade gestión de tokens
```

> **Fallback de nombre de archivo:** cuando el nombre no produce un concepto legible (nombres con punto, multipalabra o namespaces) y no hay comentario en el diff, la viñeta recae en el **propio nombre del archivo** (p. ej. `Remove New Text Document.txt`). Así **ningún archivo se queda sin línea** — siempre hay un título **y** un cuerpo.

---

## Ejemplos de mensajes generados

| Archivos en stage                                                                | Mensaje generado (pt-BR)                                                                                                | Mensaje generado (en)                                                              | Mensaje generado (es-ES)                                                              |
| -------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- |
| `AuthService.cs` añadido                                                         | `Implementa autenticação`                                                                                               | `Implement authentication`                                                         | `Implementa autenticación`                                                            |
| `PaymentService.cs` añadido                                                      | `Implementa processamento de pagamento`                                                                                 | `Implement payment processing`                                                     | `Implementa procesamiento de pagos`                                                   |
| `UserService.cs` modificado                                                      | `Corrige gerenciamento de usuários`                                                                                     | `Fix user management`                                                              | `Corrige gestión de usuarios`                                                         |
| `README.md` modificado                                                           | `Atualiza documentação`                                                                                                 | `Update documentation`                                                             | `Actualiza documentación`                                                             |
| `UserService.cs` + `TokenService.cs` añadidos                                    | `Implementa gerenciamento de usuários`<br>`- Adiciona gerenciamento de usuários`<br>`- Adiciona gerenciamento de token` | `Implement user management`<br>`- Add user management`<br>`- Add token management` | `Implementa gestión de usuarios`<br>`- Añade gestión de usuarios`<br>`- Añade gestión de tokens` |
| `.cs` modificado con comentario `// Valida el token antes de procesar la petición` | `Valida o token antes de processar a requisição`                                                                        | _(el comentario en pt se mantiene tal cual)_                                       | `Valida el token antes de procesar la petición`                                       |

---

## Requisitos

- PowerShell 5.1 o posterior
- Permiso de **Administrador** para instalar/desinstalar

---

## Instalación

### Opción A — Gestor de Plugins de GitExtensions (recomendado)

Dentro de GitExtensions, ve a **Plugins → Plugin Manager**, busca
**GitExtensions.ZimerfeldCommitMsg** en el feed de nuget.org y haz clic en instalar.
Reinicia GitExtensions. No requiere PowerShell ni permiso de Administrador.

### Opción B — Vía PowerShell

Ejecuta PowerShell **como Administrador**:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\install.ps1
```

![Salida de install.ps1 confirmando la instalación del plugin](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotInstall.png)

### Opción C — Manual

Copia `GitExtensions.Plugins.ZimerfeldCommitMsg.dll` a:

```text
C:\Program Files\GitExtensions\Plugins\
```

Reinicia GitExtensions.

---

## Desinstalación

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\uninstall.ps1
```

![Salida de uninstall.ps1 confirmando la eliminación del plugin](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUninstall.png)

Eliminar la DLL no afecta a ninguna otra parte de GitExtensions.

---

## Build y versionado

Cada vez que se ejecuta `build.ps1`, el script:

1. Lee la versión actual del `.nuspec`
2. Calcula la nueva versión: incrementa `build` en +1 → `major.minor.build`
3. Escribe la nueva versión y fecha **primero en los docs**: los READMEs y el vault de Obsidian
4. Solo entonces sube la versión en el `.nuspec` y el `.csproj`
5. Compila en Release
6. Copia la DLL a `C:\Program Files\GitExtensions\Plugins\` _(requiere Admin)_
7. Actualiza `tools\net9.0-windows\` con la nueva DLL
8. Genera `GitExtensions.ZimerfeldCommitMsg.X.Y.Z.nupkg`
9. Elimina los archivos `.nupkg` de versiones anteriores

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg
.\build.ps1
```

![Salida de build.ps1 con incremento de versión y empaquetado](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotBuild.png)

### Deploy rápido (sin incrementar versión)

Para actualizar solo la DLL durante el desarrollo:

```powershell
cd C:\GitExtensions\ZimerfeldCommitMsg\tools
.\update-dll.ps1
```

![Salida de update-dll.ps1 actualizando solo la DLL](https://raw.githubusercontent.com/zimerfeld/ZimerfeldCommitMsg/main/screenshots/screenshotUpdate.png)

---

## Plugins integrados

### [GitExtensions.ZimerfeldTree](https://github.com/zimerfeld/GitExtensions.ZimerfeldTree)

Plugin para GitExtensions que muestra las ramas jerárquicamente en una ventana de árbol persistente y no modal. Las ramas separadas por `/` se muestran como nodos de carpeta anidados bajo tres secciones fijas — LOCAL, REMOTES y tags. Por **zimerfeld**.

### [GitExtensions.ZimerfeldLFS](https://github.com/zimerfeld/GitExtensions.ZimerfeldLFS)

Plugin para GitExtensions que integra **Git LFS** (Large File Storage) en el flujo de trabajo, facilitando el versionado de archivos grandes. Por **zimerfeld**.

---

## Licencia

Copyright © 2026 Renato Zimerfeld — **CC BY-NC-ND 4.0** (ver [`LICENSE.txt`](LICENSE.txt)).
