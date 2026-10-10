# OCR engine source and attribution comparison

> Historical comparison captured before the upstream namespace/type restoration. The accompanying diffs describe that earlier source snapshot. For the current names and license handling, see `DSP.Khana.Ocr.Engine.v5/README.md` and `THIRD-PARTY-NOTICES.md` at the repository root.

Reviewed 2026-10-03. Scope: managed source and project metadata in `DSP.Khana.Ocr.Engine.v5`; this is not a complete release dependency or binary-license audit.

## Conclusion

The project is a modified copy of Charles Weld's .NET Tesseract wrapper (`charlesw/tesseract`), which embeds Andrey Akinshin's InteropDotNet. Credit both. It contains substantive local differences beyond namespace changes. Differences establish changes in this copy, not who personally authored each change.

The current tree has 61 C# files excluding bin/obj. 60 have same-relative-path counterparts in charlesw/tesseract master. `LanguageModel.cs` is the only C# file without such a counterpart. Same paths alone do not prove provenance, but the extensive matching source does.

## Substantive differences specific to this copy versus inspected upstream

| File | Local change |
| --- | --- |
| `LanguageModel.cs` | Adds language, validity, UID/app key, application name, licensed application name, and expiration data. |
| `TesseractEngine.cs` | Renames the engine to `KhanaOcrEngine`, makes it internal, adds `LanguageModel` and DLL directory parameters to constructors, stores `DllDirectory`, propagates the model to initialization, and validates license/model data. Adds friend assembly access for `Bina.Ocr.Wapper`. |
| `Interop/BaseApi.cs` | Passes the local `LanguageModel` to initialization and performs the same validation checks. Renames interop API types and OCR-system metadata. |
| `Internal/InteropDotNet/LibraryLoader.cs` | Adds `CheckAppDataDirecotry` and loads from `KhanaOcrEngine.DllDirectory` after the existing custom search path and before existing assembly/application paths. |
| `Interop/Constants.cs` | Uses native library names `BinaOcrEngineV5` and `liblept1780`. Renaming a DLL does not change its license or prove its implementation is original. |
| `Internal/InteropDotNet/InteropRuntimeImplementer.cs` | Uses `AssemblyBuilder.DefineDynamicAssembly` in the NETFULL branch, replacing `Thread.GetDomain().DefineDynamicAssembly`. |
| `Internal/ErrorMessage.cs` | Replaces the upstream error-wiki URL with `https://KhanaSoft.com`. |
| Many wrapper types | Changes public types to internal, and renames the engine, API, environment, exception, namespaces, and logging source. The embedded InteropDotNet files still use the `InteropDotNet` namespace. |
| Project/assembly metadata | SDK-style .NET 10 Windows build through Directory.Build.props, System.Drawing.Common package, generated assembly metadata and local test visibility. |

License validation blocks in both engine and BaseApi are enclosed in `#if !DESKTOP_WITHOUT_LICENSING`. Directory.Build.props currently defines `DESKTOP_WITHOUT_LICENSING`, so those checks are excluded from the current desktop build. The model type and parameters remain in the source.

The Version property returns a hard-coded `5.00.00`; this is not evidence that the underlying native library or the original wrapper import is version 5.

## Changes inherited from Charles Weld's wrapper

Do not claim these as newly written solely because they differ from standalone InteropDotNet:

- Logger derived from `LibraryLoaderTrace`, moved into the wrapper's Internal namespace and using TraceSource.
- Expanded DLL search paths, custom search path, and wrapper-specific loader integration.
- LoadLibraryException handling for missing native functions.
- NETFULL/NETSTANDARD conditional code, including CreateType/CreateTypeInfo branches. The local NETFULL dynamic-assembly call is a further change.
- Most Pix/page/iterator/rendering/bitmap functionality.
- Despeckle and its Leptonica calls already appear upstream (commit `54ae30f`, 2018-02-27).
- Bitmap resolution propagation already appears upstream (commit `228c48d`, 2017-08-23).
- ProcessPages/ProcessPage bindings already appear in upstream history (commit `7dd8837`, 2016-05-08).

Against current master, your copy also lacks various later upstream APIs/fixes, including some output formats, image-loading/conversion APIs, platform loading updates, and the executing-assembly null check. These are version differences; the comparison does not establish that you removed them.

## Publishing and attribution

InteropDotNet is MIT: retain its original copyright and distribute the full MIT permission/warranty text. Existing source headers name Andrey Akinshin and link to MIT, but a URL alone should not replace shipping the full license.

Charles Weld's wrapper is Apache-2.0: give recipients the full Apache license, retain applicable source attribution, and put a prominent modification notice in each modified upstream file (namespace/accessibility edits count as modifications). If the imported distribution contains a NOTICE, retain applicable notices. The inspected master tree has LICENSE.txt and README attribution, but no standalone NOTICE file.

Use a structure such as:

- `LICENSE`: chosen license for code you own; Apache-2.0 is a straightforward option for this combined source tree, subject to the other dependencies.
- `LICENSES/InteropDotNet-MIT.txt`: upstream full MIT text, including Copyright (c) 2014 Andrey Akinshin.
- `LICENSES/Tesseract-Apache-2.0.txt`: upstream full Apache 2.0 text.
- `THIRD-PARTY-NOTICES.md`: identify components, source URLs, copied locations, versions/revisions when verified, copyright owners, licenses, and local modifications.
- Ship the corresponding licenses/notices with binary releases as well as repository source.

Preserve the upstream copyright rather than replacing it with only your own name. Add attribution for your modifications where appropriate. The root license must not imply that copied third-party code has lost its original terms. A fork, a namespace rename, or a NuGet reference is not required by these two licenses; retaining modified embedded source is permitted under their terms.

Suggested README/notice wording:

> DSP.Khana.Ocr.Engine.v5 is derived from Charles Weld's Tesseract .NET wrapper (https://github.com/charlesw/tesseract), licensed under Apache-2.0. It includes InteropDotNet by Andrey Akinshin (https://github.com/AndreyAkinshin/InteropDotNet), licensed under MIT. Local modifications include namespace/type names and visibility, language-model integration, native library search paths, and build/runtime updates. Original import revision: not yet verified.

Use the upstream wrapper's copyright attribution, currently `Copyright 2012-2022 Charles Weld`, and InteropDotNet's `Copyright (c) 2014 Andrey Akinshin`, while retaining any applicable notices from the actual imported version.

Suggested modified-file notice (adapt the changes to each file):

```csharp
// Derived from charlesw/tesseract; Apache-2.0.
// Modified for DSP.Khana.Ocr: namespace/type visibility and local integration.
// See THIRD-PARTY-NOTICES.md and LICENSES/Tesseract-Apache-2.0.txt.
```

In MIT InteropDotNet files, preserve the existing MIT copyright headers and use MIT attribution instead of the Apache example.

Sources:

- https://github.com/charlesw/tesseract/blob/b5329d5be92fa670031d94c3875f879651a01f55/ReadMe.md
- https://github.com/charlesw/tesseract/blob/b5329d5be92fa670031d94c3875f879651a01f55/LICENSE.txt
- https://github.com/AndreyAkinshin/InteropDotNet/blob/0b15eee809716c50562458d6fe95d52bea46b9c6/LICENSE.md
- https://www.apache.org/licenses/LICENSE-2.0 (especially section 4)


