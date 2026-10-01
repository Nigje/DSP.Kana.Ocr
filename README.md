# DSP.Khana.Ocr.New

A Windows Forms desktop application that converts images and PDF pages into editable text using Persian, English, or mixed-language OCR. Users can prepare images, recognize individual images or a batch, edit the results, and export them.

This solution contains the desktop application and its seven supporting projects. Existing namespaces and project names are preserved, including the original `Wapper` spelling. Desktop builds use the license-free profile; activation and licensing projects are not required.

## Build and run

### Prerequisites

- Windows and Visual Studio 2022 with the **.NET desktop development** workload.
- .NET Framework **4.8** and **4.7** targeting packs. The desktop and legacy libraries target 4.8; the managed OCR engine targets 4.7.
- A .NET SDK recognized by Visual Studio's MSBuild for the SDK-style OCR engine project.

Open [DSP.Khana.Ocr.New.sln](DSP.Khana.Ocr.New.sln), set **DSP.Bina.Ocr.DesktopApplication.V3** as the startup project, restore NuGet packages, and build **Debug | Any CPU** or **Release | Any CPU**. Press F5 to run.

The required legacy package folders are already included under `packages/`. Restore is still needed to generate the SDK-style engine's `obj/project.assets.json` after a clean checkout.

From a **Developer PowerShell for Visual Studio**, run:

```powershell
Set-Location D:\_Repositories\DSP.Khana.Ocr.New
msbuild .\DSP.Khana.Ocr.Engine.v5\DSP.Khana.Ocr.Engine.v5.csproj /t:Restore
msbuild .\DSP.Khana.Ocr.New.sln /p:Configuration=Debug /m
msbuild .\DSP.Khana.Ocr.New.sln /p:Configuration=Release /m
& .\DSP.Bina.Ocr.DesktopApplication.V3\bin\Debug\DSP.Bina.Ocr.DesktopApplication.V3.exe
```

Use Visual Studio MSBuild for the complete solution: it includes legacy .NET Framework projects and WinForms resources. Both Debug and Release builds were verified when this standalone solution was created; existing compiler warnings remain in the legacy source.

## How the application works

1. **Startup:** [Program.cs](DSP.Bina.Ocr.DesktopApplication.V3/Program.cs) initializes WinForms, registers UI exception handling, creates `NewForm`, starts the existing App Center integration, and enters the Windows message loop.
2. **OCR initialization:** `NewForm` obtains `BinaOcr.Instance()`. The wrapper extracts the embedded native OCR libraries and trained language models, selecting native binaries for the current process architecture.
3. **Input:** The user imports images, pastes a clipboard image, or imports selected PDF pages. `PDFConvert` renders PDF pages to image files through the `DSP.Tools32.dll` / `DSP.Tools64.dll` Ghostscript API. `ImageEntity` holds each image and its associated application state; `ImageListView` presents the image list.
4. **Image preparation:** The desktop uses `DSP.Khana.ImageTools` for operations such as image adjustment and deskew. Preparing an image can improve recognition before it reaches the OCR engine.
5. **Recognition:** The form passes the selected image, OCR language, engine mode, and page segmentation mode to `BinaOcr`. The wrapper maps those options to the managed engine, which calls the native OCR library. Optional post-processing applies text cleanup rules to the recognized result.
6. **Editing and export:** Recognized text is displayed in the rich-text editor. Users can format it and save text as DOCX through `Xceed.Words.NET` and `Xceed.Document.NET`; the application also supports saving images as JPEG.
7. **UI language:** The footer language selector switches resource strings and directional layout between English and Persian. It also refreshes the About Us dialog when open.

Changing the **UI language** changes labels, messages, and arrangement. The separate **OCR language** option selects the recognition model: Persian, English, or mixed. The current UI selection is not saved as a persistent user preference; startup chooses from the thread's UI culture.

## Why each project is included

| Project | Responsibility | Why the desktop needs it |
| --- | --- | --- |
| `DSP.Bina.Ocr.DesktopApplication.V3` | Application entry point, forms, image state, editor, export commands, and localization. | This is the executable and the user interface that coordinates the workflow. |
| `DSP.Khana.Image.Tools` (`DSP.Khana.ImageTools.csproj`) | Image conversion, image adjustment, deskew, and PDF rendering helpers. | Input preparation and PDF-to-image conversion are used by the desktop before recognition. |
| `DSP.Khana.Ocr.Wapper` | The `BinaOcr` facade, option mapping, native/model extraction, recognition calls, and optional cleanup. | Provides the desktop's OCR API and manages the assets needed by the engine. |
| `DSP.Khana.Ocr.Engine.v5` | Managed OCR engine, image/page objects, and native interop. | Bridges the C# wrapper to the native OCR and Leptonica libraries. This copy builds only `net47`, the target consumed by the desktop. |
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
| [Directory.Build.props](Directory.Build.props) | Sets `DesktopWithoutLicensing=true` for the solution, including projects built directly. Wrapper and engine project files use this to define `DESKTOP_WITHOUT_LICENSING` and exclude guarded licensing code. |
| `Forms/NewForm.cs` | Main form behavior: image import, editing, recognition, text formatting, and export. |
| `Forms/NewForm.Designer.cs` and `.resx` | WinForms-generated controls, initial layout, and designer resources. These are part of the form and must accompany its behavior file. |
| `Forms/NewForm.Localization.cs` | Creates the footer language selector, applies `Properties.Strings`, and refreshes language-dependent controls and open About Us windows. |
| `Forms/NewForm.Layout.cs` | Applies RTL/LTR layout and alignment changes, including control arrangements and resizing. Translated text alone cannot rearrange the original Persian layout. |
| `Forms/AboutUs.*`, `Forms/SelectPagesForm.*`, and `Forms/TemplateForm.*` | Supporting dialogs and shared form presentation used by the desktop. Their designer and resource files belong with the C# files. |
| `ExtentionMudole/ScrollablePictureBox.*` and `TrackbarDialog.*` | Supporting WinForms controls/dialogs for image presentation and adjustment. |
| `Model/` | Image state, selection-related values, undo/history support, and application-specific exceptions. Keeps form operations and error handling consistent. |
| `Properties/Strings.resx` | Neutral English strings and fallback translations. |
| `Properties/Strings.fa-IR.resx` | Persian translations using the same keys as the neutral resource file. Built into a `fa-IR` satellite assembly. |
| `Properties/Strings.Designer.cs` | Strongly typed `Properties.Strings` accessors used by application code. Regenerate it when resource keys change. |
| `Properties/Resources.*` and `Resources/` | Embedded UI images and icons referenced by forms. Resource file references must remain valid. |
| `Properties/Settings.*`, `Properties/AssemblyInfo.cs`, and `App.config` | Existing settings infrastructure, assembly metadata, and .NET Framework runtime configuration. |
| `Properties/Data.zip` | Embedded OCR data. The wrapper's archive supplies trained models, including the English, Persian, and mixed-language models used by the UI. Without model data, the engine cannot recognize text. |
| Native DLLs in `Properties/`, and engine `x86/` / `x64/` folders | Architecture-specific OCR, Leptonica, and PDF helper binaries. They are embedded or copied according to the project definitions and loaded at runtime. They are source dependencies, not disposable build output. |
| `Xceed.Document.NET/Resources/*.xml.gz` | Embedded defaults for constructing Word document styles and numbering. |
| Document-library signing | Both document libraries build unsigned in this standalone solution. Their private signing keys stay local, are ignored by Git, and are not needed to build or export DOCX. The friend-assembly declaration allows the unsigned `Xceed.Words.NET` library to access the document library's internals. |
| Third-party license notices | Attribution and license terms for bundled third-party code. Removing desktop activation does not remove these notices. |
| `Form1.*` and `MainFormTemp.*` | Legacy forms still listed in the desktop project. They are not the startup form, but were retained to preserve the existing compilable project. Removing them is a separate cleanup task rather than a requirement for OCR. |

Paths in the source-file table are relative to the desktop project unless they explicitly name another project.

## Included package dependencies

`packages/` is intentionally retained in version control for this standalone copy. Legacy project references use relative `HintPath` entries and imported package targets; the included folders allow those references to resolve without the original repository.

| Package group | Reason it remains |
| --- | --- |
| `Microsoft.AppCenter`, `.Analytics`, and `.Crashes` | Referenced by `Program.cs` for startup and error reporting. This integration is separate from OCR and can be removed in a dedicated change. |
| `Newtonsoft.Json` | Existing desktop assembly reference. Retained for build compatibility; it is not the native OCR engine. |
| `sqlite-net-pcl`, `SQLitePCLRaw.bundle_green`, `.core`, and `.provider.e_sqlite3.net45` | Existing managed SQLite references and their supporting assemblies. They are retained dependencies, not evidence that the current OCR workflow requires a database. |
| `SQLitePCLRaw.lib.e_sqlite3.linux`, `.osx`, and `.v110_xp` | Legacy desktop project imports and package checks require these target files. The Windows package supplies `x86/e_sqlite3.dll` and `x64/e_sqlite3.dll` in the output. |
| `System.Runtime.InteropServices.RuntimeInformation` | Existing desktop runtime-information assembly reference. |

Only the twelve package folders referenced by the copied project were included; unrelated package folders were omitted. The desktop's `packages.config` was reduced to the included packages. Review package references and imports together when removing legacy dependencies.

## Runtime files and deployment

The desktop build output is:

```text
DSP.Bina.Ocr.DesktopApplication.V3/bin/Debug/
DSP.Bina.Ocr.DesktopApplication.V3/bin/Release/
```

Run or distribute the **complete desktop output folder**, including its managed dependencies, `.exe.config`, `fa-IR` satellite resource folder, and native subfolders. Copying only the `.exe` omits required files. The target machine needs .NET Framework 4.8.

The wrapper extracts OCR libraries and language models under `%APPDATA%\.temporalapp`, using version-specific `.app_<version>` and `.model_<version>` directories. It also writes the appropriate `DSP.Tools32.dll` or `DSP.Tools64.dll` beside the executable. The application therefore needs write access to its executable directory as well as the user's application-data directory under the current implementation. PDF conversion also creates temporary rendered page images.

The wrapper and engine build their license-free assemblies under their own `bin/<Configuration>/DesktopWithoutLicensing/` paths; the engine adds `net47/`. Project references copy the needed assemblies into the desktop output.

## Working on localization

Add matching keys to `Strings.resx` and `Strings.fa-IR.resx`, regenerate `Strings.Designer.cs`, and bind UI text through `Properties.Strings`. Update `NewForm.Localization.cs` for new controls and `NewForm.Layout.cs` when a control needs direction-specific placement or alignment. Keep the footer selector on the left and refresh both languages when checking dialogs and resize behavior.

## Version control

[.gitignore](.gitignore) excludes Visual Studio user state, `bin/`, `obj/`, publish output, test/coverage output, and temporary logs. These files are recreated by tools or belong to an individual workstation.

Commit the solution, project files, source, `.resx` files, resource images, `packages/`, embedded model ZIPs, native DLLs, and the other required binary dependencies. The ignore file deliberately does **not** ignore all DLLs, ZIPs, or NuGet package files because this repository contains required binary dependencies. Private signing keys (`.snk`), private certificate/key files, and local environment files are ignored and must not be committed. A fresh checkout builds without private signing keys.
