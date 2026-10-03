# DSP.Khana.Ocr.New

A Windows Forms desktop application that converts images and PDF pages into editable text using Persian, English, or mixed-language OCR. Users can prepare images, recognize individual images or a batch, edit the results, and export them.

This solution contains the desktop application and its seven supporting projects. Application namespaces and project names are preserved, including the original `Wapper` spelling; the managed OCR engine uses the upstream Tesseract namespaces. Desktop builds use the license-free profile; activation and licensing projects are not required.

See [CHANGELOG.md](CHANGELOG.md) for the accumulated changes and notes for the next release.

## OCR engine attribution and licensing

`DSP.Khana.Ocr.Engine.v5` is a modified copy of [Charles Weld's Tesseract .NET wrapper](https://github.com/charlesw/tesseract), licensed under **Apache-2.0**, incorporating [Andrey Akinshin's InteropDotNet](https://github.com/AndreyAkinshin/InteropDotNet), licensed under **MIT**. Copyright 2012-2022 Charles Weld; Copyright (c) 2014 Andrey Akinshin.

The engine uses the upstream namespaces and type names, with local requirements documented in its [README](DSP.Khana.Ocr.Engine.v5/README.md). See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) and the full license texts in [LICENSES](LICENSES). Attribution is also shown in the English and Persian About dialogs, and the notice and license files accompany build and publish output.

These notices cover the managed OCR components. A license for independently authored application code has not yet been selected, and the remaining bundled dependencies and assets need separate license review before publishing the complete solution as open source.

## Build and run

### Prerequisites

- Windows and the **.NET 10 SDK**. `global.json` selects a stable .NET 10 SDK and allows newer .NET 10 feature bands.
- For Visual Studio, use **Visual Studio 2026** with the .NET desktop development workload. Visual Studio 2022 does not provide the .NET 10 development toolchain.
- For framework-dependent deployment, install the **.NET 10 Desktop Runtime** on the target machine.

Open [DSP.Khana.Ocr.sln](DSP.Khana.Ocr.sln) and set **DSP.Bina.Ocr.DesktopApplication.V3** as the startup project. All eight application projects use SDK-style project files. The post-processing library targets `net10.0`; the desktop and its Windows-dependent libraries target `net10.0-windows`.

From PowerShell in the solution directory:

```powershell
dotnet restore DSP.Khana.Ocr.sln
dotnet build DSP.Khana.Ocr.sln -c Debug
dotnet build DSP.Khana.Ocr.sln -c Release
dotnet run --project DSP.Bina.Ocr.DesktopApplication.V3 -c Debug
```

NuGet dependencies use `PackageReference` and restore into the standard user cache. No legacy `packages/` folder or .NET Framework targeting pack is needed. Keep the native DLLs, embedded OCR model archives, and resource images checked in.

## Migration checks

Run the integration smoke checks on Windows with:

```powershell
dotnet run --project tests/SmokeTests/SmokeTests.csproj -c Release
```

The checks exercise all 279 English/Persian resource lookups, DOCX export and reload, unsigned document assemblies, real native OCR with the English/Persian/mixed models, the production OCR wrapper and post-processing, native PDF rendering, production form initialization, live About Us translations, RTL/LTR switching, and resizing. The test forms are transparent and close automatically. As during normal recognition and PDF import, the production wrapper extracts its runtime assets into the user's application-data folder. Test samples use a unique temporary folder; Windows can retain loaded native DLLs there until the process exits.

## How the application works

1. **Startup:** [Program.cs](DSP.Bina.Ocr.DesktopApplication.V3/Program.cs) initializes WinForms, registers UI exception handling, creates `MainForm`, and enters the Windows message loop.
2. **OCR initialization:** `OcrService` initializes `BinaOcr.Instance()` on the first recognition request. PDF import also initializes the wrapper before rendering. The wrapper extracts the embedded native OCR libraries and trained language models, selecting native binaries for the current process architecture.
3. **Input:** The user imports images, pastes a clipboard image, or imports selected PDF pages. `PDFConvert` renders PDF pages to image files through the `DSP.Tools32.dll` / `DSP.Tools64.dll` Ghostscript API. `ImageEntity` holds each image and its associated application state; `ImageListView` presents the image list.
4. **Image preparation:** The desktop uses `DSP.Khana.ImageTools` for operations such as image adjustment and deskew. Preparing an image can improve recognition before it reaches the OCR engine.
5. **Recognition:** The form passes the selected image and a snapshot of OCR options to `OcrService`, which serializes calls to `BinaOcr`. The wrapper maps those options to the managed engine, which calls the native OCR library. Optional post-processing applies text cleanup rules to the recognized result.
6. **Editing and export:** Text edits, RTF formatting, and text direction are stored per image and restored when selection changes. `RichTextDocumentService` captures font family/size, bold, italic, underline, color, paragraph alignment, bullets, indentation, and direction. `DocumentExportService` writes that snapshot as DOCX and saves the image as JPEG. Existing base filenames receive ` (2)`, ` (3)`, and subsequent suffixes; exclusive file creation prevents accidental overwrites. The completion message shows the actual export name or output directory.
7. **UI language:** The footer language selector switches resource strings and directional layout between English and Persian. It also refreshes the About Us dialog when open.

Changing the **UI language** changes labels, messages, and arrangement. The separate **OCR language** option selects the recognition model: Persian, English, or mixed. The current UI selection is not saved as a persistent user preference; startup chooses from the thread's UI culture.

## Recognition cancellation and resource ownership

Only one recognition run can be started per form, and a shared service gate permits only one native call across forms. Import, editing, export, and image selection are disabled during recognition. The footer offers a Cancel OCR button. Cancellation stops queued work and prevents a canceled result from replacing text; the native engine cannot interrupt a page already in progress, so that page is allowed to finish before its owned snapshot is disposed. Closing the form requests cancellation and waits for active recognition before releasing resources.

`ImageEntity` owns its current bitmap and a bounded undo/redo history. Evicted entries, discarded redo states, and all remaining images are disposed when no longer needed. Imported bitmaps are detached from their source files. The thumbnail cache receives a separate, small image so disposing an editable image does not invalidate its thumbnail. Adjustment previews are replaced and disposed as the slider moves and when the dialog closes. Form closure or disposal releases the image list, entities, temporary session directory, and owned editor font.

For the focused deterministic regression checks, run:

```powershell
dotnet run --project tests/SmokeTests/SmokeTests.csproj -c Debug -- --refactoring-only
```

The focused checks cover image ownership, unlocked imported files, formatted export and collision preservation, per-image edited text/RTF, serialized OCR calls, cancellation, failure recovery, and closing while OCR is active. The full smoke suite also runs native OCR, PDF conversion, and live bilingual layout checks.

## Why each project is included

| Project | Responsibility | Why the desktop needs it |
| --- | --- | --- |
| `DSP.Bina.Ocr.DesktopApplication.V3` | Application entry point, forms, image state, editor, export commands, and localization. | This is the executable and the user interface that coordinates the workflow. |
| `DSP.Khana.Image.Tools` (`DSP.Khana.ImageTools.csproj`) | Image conversion, image adjustment, deskew, and PDF rendering helpers. | Input preparation and PDF-to-image conversion are used by the desktop before recognition. |
| `DSP.Khana.Ocr.Wapper` | The `BinaOcr` facade, option mapping, native/model extraction, recognition calls, and optional cleanup. | Provides the desktop's OCR API and manages the assets needed by the engine. |
| `DSP.Khana.Ocr.Engine.v5` | Managed OCR engine, image/page objects, and native interop. | Bridges the C# wrapper to the native OCR and Leptonica libraries. The Windows-dependent engine now targets `net10.0-windows`; its bitmap conversion and native interop remain available. |
| `DSP.Khana.Ocr.PostPRocessing` (`DSP.Khana.Ocr.PostProcessing.csproj`) | Text cleanup rules for spacing, line breaks, and word filtering. | Required by the wrapper's optional post-processing path. |
| `OtherUsefulLibrary/Docs/Xceed.Document.NET` | Document objects, paragraphs, formatting, tables, and embedded default document resources. | Supplies the document implementation used by the DOCX export library. |
| `OtherUsefulLibrary/Docs/Xceed.Words.NET` | The `DocX` API for creating and saving Word documents. | The desktop calls this API when exporting recognized text to `.docx`. It depends on `Xceed.Document.NET`. |
| `OtherUsefulLibrary/ImageListView` | WinForms image-list control, thumbnail caching, selection, and rendering. | Displays imported images and PDF pages and lets the user select them for editing and recognition. |

The direct project dependencies are:

```text
DSP.Bina.Ocr.DesktopApplication.V3
├── DSP.Khana.ImageTools
├── Bina.Ocr.Wapper
│   ├── DSP.Khana.Ocr.Engine.v5
│   └── DSP.Khana.Ocr.PostProcessing
├── Xceed.Words.NET
│   └── Xceed.Document.NET
├── Xceed.Document.NET
└── ImageListView
```

## Important source files and assets

| Part | Purpose and reason to retain it |
| --- | --- |
| [Directory.Build.props](Directory.Build.props) | Shared .NET 10 target and build settings, platform metadata, preserved assembly attributes, and the `DESKTOP_WITHOUT_LICENSING` constant. Projects built directly also exclude guarded licensing code. |
| [global.json](global.json) | Keeps CLI builds on the .NET 10 SDK rather than accidentally selecting an older SDK. |
| `tests/SmokeTests/` | Reproducible migration integration checks using the actual assemblies, native libraries, models, and forms. |
| `Forms/MainForm.cs` | Main form behavior: image import, editing, recognition, text formatting, and export. |
| `Forms/MainForm.Designer.cs` and `.resx` | WinForms-generated controls, initial layout, and designer resources. These are part of the form and must accompany its behavior file. |
| `Forms/MainForm.Localization.cs` | Creates the footer language selector, applies `Properties.Strings`, and refreshes language-dependent controls and open About Us windows. |
| `Forms/MainForm.Layout.cs` | Applies RTL/LTR layout and alignment changes, including control arrangements and resizing. Translated text alone cannot rearrange the original Persian layout. |
| `Forms/MainForm.Workflows.cs` | Coordinates OCR busy/cancel/close state, per-image editor persistence, adjustment preview ownership, and lifetime cleanup. |
| `Services/ImageImportService.cs` | Loads detached bitmaps, renders PDF pages into a private session directory, and cleans up temporary files. |
| `Services/ImageEditService.cs` | Performs image operations and creates previews; temporary images are disposed after committing or canceling an edit. |
| `Services/OcrService.cs` | Takes an owned bitmap snapshot, serializes native OCR calls, and implements cooperative cancellation. Its interface allows deterministic workflow tests without native OCR. |
| `Services/RichTextDocumentService.cs` and `Model/DocumentSnapshot.cs` | Convert saved RTF into paragraph/run formatting suitable for DOCX export without modifying the visible editor. |
| `Services/DocumentExportService.cs` | Produces formatted DOCX/JPEG pairs, chooses collision-free names, and creates new files exclusively. |
| `Services/ApplicationErrorService.cs` | Maps validation and typed filesystem errors to localized messages and records diagnostic details in the local error log. |
| `Forms/AboutUsForm.*`, `Forms/PdfPageSelectionForm.*`, and `Forms/BaseDialogForm.*` | Supporting dialogs and shared form presentation used by the desktop. Their designer and resource files belong with the C# files. |
| `Controls/ScrollablePictureBox.*` and `Forms/TrackBarDialog.*` | Supporting WinForms controls/dialogs for image presentation and adjustment. |
| `Model/` | Image state, selection-related values, undo/history support, and application-specific exceptions. Keeps form operations and error handling consistent. |
| `Properties/Strings.resx` | Neutral English strings and fallback translations. |
| `Properties/Strings.fa-IR.resx` | Persian translations using the same keys as the neutral resource file. Built into a `fa-IR` satellite assembly. |
| `Properties/Strings.Designer.cs` | Strongly typed `Properties.Strings` accessors used by application code. Regenerate it when resource keys change. |
| `Properties/Resources.*` and `Resources/` | Embedded UI images and icons referenced by forms. Resource file references must remain valid. |
| `Properties/Settings.*`, `Properties/AssemblyInfo.cs`, and `App.config` | Existing settings infrastructure, assembly metadata, and application configuration. |
| `DSP.Khana.Ocr.Wapper/Properties/Data.zip` | Embedded OCR data. The wrapper's archive supplies trained models, including the English, Persian, and mixed-language models used by the UI. Without model data, the engine cannot recognize text. The desktop references the wrapper instead of embedding duplicate archives or native libraries. |
| Native DLLs embedded from the wrapper's `Properties/` folder | Architecture-specific OCR, Leptonica, and PDF helper binaries. The wrapper extracts them when OCR or PDF import initializes it, so they also travel inside its DLL when publishing. They are source dependencies, not disposable build output. |
| `Xceed.Document.NET/Resources/*.xml.gz` | Embedded defaults for constructing Word document styles and numbering. |
| Document-library signing | Both document libraries build unsigned in this standalone solution. Their private signing keys stay local, are ignored by Git, and are not needed to build or export DOCX. The friend-assembly declaration allows the unsigned `Xceed.Words.NET` library to access the document library's internals. |
| Third-party license notices | Attribution and license terms for bundled third-party code. Removing desktop activation does not remove these notices. |

Paths in the source-file table are relative to the desktop project unless they explicitly name another project.

## NuGet dependencies and removed legacy packages

| Dependency | Why it is needed |
| --- | --- |
| `System.Drawing.Common` 10.0.12 | Bitmap, font, image conversion, and graphics APIs used by the OCR, image-tool, wrapper, and document libraries. These operations remain Windows-only. |
| `System.IO.Packaging` 10.0.12 | Open Packaging Convention APIs used to read and write DOCX document parts. |
| .NET 10 Windows Desktop framework | WinForms, designer APIs, resources, and configuration APIs used by the executable and image-list control. These are provided by `UseWindowsForms`; redundant package references are not needed. |

The old App Center, Newtonsoft.Json, SQLite/SQLitePCLRaw, and RuntimeInformation assembly references and imported targets were removed. SQLite and Newtonsoft.Json had no active source usage in this copied desktop workflow; RuntimeInformation is supplied by modern .NET. App Center startup and crash reporting were replaced with local error logging to `%LOCALAPPDATA%\DSP.Khana.Ocr\Logs\errors.log`. The old `packages.config` and bundled legacy package cache are no longer part of the solution.

The OCR engine's dynamic native interop now uses `AssemblyBuilder.DefineDynamicAssembly`, supported by modern .NET. WinForms control properties explicitly declare their designer serialization behavior to satisfy .NET 10's checks while keeping the image-list designer integration.

## Runtime files and deployment

The desktop build output is:

```text
DSP.Bina.Ocr.DesktopApplication.V3/bin/Debug/net10.0-windows/
DSP.Bina.Ocr.DesktopApplication.V3/bin/Release/net10.0-windows/
```

Run or distribute the **complete desktop output folder**, including its managed dependencies, `.deps.json`, `.runtimeconfig.json`, `fa-IR` satellite resource folder, and native subfolders. Copying only the `.exe` omits required files. A framework-dependent build needs the .NET 10 Desktop Runtime on the target machine.

The wrapper extracts OCR libraries and language models under `%APPDATA%\.temporalapp`, using version-specific `.app_<version>` and `.model_<version>` directories. It also writes the appropriate `DSP.Tools32.dll` or `DSP.Tools64.dll` beside the executable. The application therefore needs write access to its executable directory as well as the user's application-data directory under the current implementation. PDF conversion creates rendered page images in a unique session directory under `%TEMP%\DSP.Khana.Ocr`.

Libraries build into `bin/<Configuration>/<TargetFramework>/`; project references copy their required output into the desktop folder.

To create a self-contained x64 release that includes the .NET runtime:

```powershell
dotnet publish DSP.Bina.Ocr.DesktopApplication.V3 -c Release -r win-x64 --self-contained true -o publish/win-x64
```

Publish the complete folder. Keep trimming and Native AOT disabled: the application uses WinForms resources, reflection, and dynamically emitted OCR interop. The bundled native libraries support x86/x64; ARM64 deployment is not covered by these assets.

## Working on localization

Add matching keys to `Strings.resx` and `Strings.fa-IR.resx`, regenerate `Strings.Designer.cs`, and bind UI text through `Properties.Strings`. Update `MainForm.Localization.cs` for new controls and `MainForm.Layout.cs` when a control needs direction-specific placement or alignment. Keep the footer selector on the left and refresh both languages when checking dialogs and resize behavior.

## Version control

[.gitignore](.gitignore) excludes Visual Studio user state, `bin/`, `obj/`, publish output, test/coverage output, and temporary logs. These files are recreated by tools or belong to an individual workstation.

Commit the solution, project files, source, `.resx` files, resource images, embedded model ZIPs, native DLLs, and the smoke checks. NuGet packages restore automatically and should not be committed. The ignore file deliberately does **not** ignore all DLLs or ZIPs because this repository contains required binary dependencies. Private signing keys (`.snk`), private certificate/key files, and local environment files are ignored and must not be committed. A fresh checkout builds without private signing keys.
