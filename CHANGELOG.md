# Changelog

This file records the changes completed during the standalone desktop modernization and refactoring. The changes below are pending release; no release version has been assigned. Move them under a version and release date when preparing the next release.

## Unreleased

### UI language switching

- Keep the main window alive when switching between English and Persian by applying text direction to content containers instead of recreating the form's native window.
- Batch translation and child layouts with native redraw paused until completion, preventing partially updated controls from flashing. Redraw is restored even if translation fails, and hidden forms stay hidden.
- Added regressions for repeated switches, retained editor text/formatting/selection, selected images, OCR options, and bilingual layout after resizing.

### DOCX export dependency replacement

- Implemented DOCX export using MIT-licensed Open XML SDK (`DocumentFormat.OpenXml` 3.5.1).
- Export retains fonts, sizes, colors, bold/italic/underline, paragraph direction/alignment, bullets, indentation, blank paragraphs, and safe filenames; it also handles strikethrough, tabs, and explicit line breaks.
- Added Open XML schema validation and SDK round-trip checks to production export regressions, including Persian complex-script formatting and companion JPEG output.
- Added the SDK's MIT license to output and updated the English/Persian About credits and third-party notice.

### OCR source attribution and licensing

- Restored the Tesseract wrapper's original namespaces, engine/API/exception/environment names, engine-mode member names, logging identity, and error-help URL. Source filenames already matched upstream. The local project/assembly identity and runtime integration are retained.
- Updated the application facade and smoke tests to use the restored names.
- Credited Charles Weld's Apache-2.0 Tesseract .NET wrapper and Andrey Akinshin's MIT InteropDotNet in source headers, documentation, and the English/Persian About dialogs.
- Added the full upstream license texts and third-party notice to build and publish output. A repository-wide application license and the remaining dependency review are still separate release tasks.

### Release summary

The OCR desktop application now supports English and Persian interfaces with matching left-to-right and right-to-left layouts. The standalone solution has been upgraded to .NET 10 LTS, desktop activation has been removed, and recognition, image editing, and document export have been refactored. This update fixes startup failures, improves resource cleanup and cancellation, preserves edited and formatted recognition results per image, and prevents exported documents from overwriting existing files.

### Interface and localization

- Replaced hardcoded Persian interface text with strongly typed `Properties.Strings` resources: English in `Strings.resx` and Persian in `Strings.fa-IR.resx`.
- Expanded localization to 93 resource keys, including dialogs, file filters, validation errors, processing status, and cancellation messages.
- Added an English/Persian interface language selector in the left side of the footer. Its physical position remains on the left in either language.
- Applied left-to-right arrangement for English and right-to-left arrangement for Persian, including control alignment, margins, option rows, image actions, and dialog text.
- Corrected the layout and wrapping of the Save All and interface-language labels.
- Localized the About Us content and updated open dialogs when the interface language changes.
- Kept interface language independent of the existing Persian, English, and mixed OCR language choices.
- Added a localized Cancel OCR button and processing/cancellation feedback in the footer.

### Recognition and editing workflows

- Extracted recognition into `OcrService` with an `IOcrService` interface, immutable operation options, and an owned bitmap snapshot for each request.
- Serialized access to the shared native OCR engine across forms and initialized the native engine only when OCR or PDF import needs it.
- Prevented overlapping recognition operations within a form and captured batch inputs and options before processing begins.
- Disabled conflicting import, selection, editing, recognition, and export actions while processing; made the text editor read-only during processing and restored controls in `finally` blocks.
- Added cancellation for queued work and subsequent batch pages. Results from a canceled active page are discarded after its native call finishes.
- Deferred closing a form with active recognition until the native call finishes and cancellation cleanup completes.
- Stored recognition text, rich text, and text direction per image. Switching images now preserves the latest edits and formatting.
- Updated recognition results by image identifier instead of filename, so images with identical names maintain independent results.
- Reset obsolete rich-text formatting when a fresh recognition result replaces the previous text.

### Import, image ownership, and undo/redo

- Extracted image and PDF import into `ImageImportService`, with detached bitmap copies that release source-file locks.
- Used a unique temporary directory per import session and cleaned up owned PDF-rendering files, logging cleanup failures.
- Extracted deskew, smoothing, sharpening, inversion, monochrome/grayscale conversion, rotation, cropping, and previews into `ImageEditService`.
- Made `ImageEntity` explicitly own and dispose its current bitmap and bounded undo/redo history.
- Disposed evicted undo entries and invalidated/disposed redo history after a new edit.
- Validated history capacity and reported an appropriate exception for an empty history stack.
- Gave the thumbnail cache independent thumbnail images instead of sharing the editable model bitmap.
- Disposed images, thumbnails, preview replacements, temporary fonts, and remaining history when images are removed or forms are disposed.
- Bound the picture-box animation timer to the control's component lifetime.
- Fixed PDF temporary-file paths and recognized PDF extensions without case sensitivity.
- Replaced brittle filename parsing with `Path` APIs, including support for extensionless filenames and filenames containing multiple dots.
- Restored the import cursor after successful, canceled, empty, and failed import operations.

### Document export

- Extracted rich-text capture into `RichTextDocumentService` and export into `DocumentExportService`.
- Exported the latest edited text and formatting, including font family/size, bold, italic, underline, color, paragraph alignment, bullets, indentation, and text direction.
- Initialized the hidden rich-text reader's handle before loading text/RTF to prevent text loss during right-to-left handle creation.
- Preserved the editor selection while capturing formatting and disposed temporary font objects.
- Added collision-safe export names such as `name (2)` and checked both JPEG and DOCX destinations.
- Used exclusive file creation to prevent overwriting a file created between name selection and saving.
- Cleaned up only files newly created by a failed export and preserved existing destination files.
- Supported image-only JPEG export when no recognition text is present.
- Treated canceling the save dialog as a normal outcome and displayed the actual saved basename or destination directory.

### Reliability fixes

- Fixed the `MainForm` constructor null-reference failure by creating, mounting, and wiring the image-list control outside generated designer code.
- Added localized validation for nonnumeric, nonfinite, zero, and negative font sizes.
- Corrected font-family selection to match the string items in the font selector and guarded empty selections.
- Handled mixed-format text selections whose `SelectionFont` is null.
- Removed invalid null checks on value types and corrected 64-bit window-message coordinate conversion.
- Extracted typed error mapping into `ApplicationErrorService`, including validation, missing paths, access denied, sharing/locking violations, general I/O failures, and cancellation.
- Replaced English exception-message substring detection with exception types and error codes.
- Replaced App Center crash reporting with local diagnostic logging at `%LOCALAPPDATA%\DSP.Khana.Ocr\Logs\errors.log`.
- Added startup/global error handling and deterministic form disposal.

### Code organization and cleanup

- Split UI orchestration and processing state into `MainForm.Workflows.cs`; retained dedicated layout and localization partial files.
- Added document snapshot models for formatted export and dedicated OCR language, engine, and page-segmentation option types.
- Removed 39 unused imports using compiler semantic diagnostics.
- Standardized control fields, local variables, event handlers, and image resource names; corrected spelling in image-editing, zoom, font, and indentation code.
- Removed unused prototype forms, handlers, commented-out code, drawing callbacks, and unnecessary allocations.
- Removed approximately 76 MiB of verified identical native DLL and model archive duplicates from the desktop project. Required wrapper resources remain included.
- Preserved existing project identifiers and root namespaces for compatibility.

| Previous name | Current name |
| --- | --- |
| `NewForm` | `MainForm` |
| `AboutUs` | `AboutUsForm` |
| `SelectPagesForm` | `PdfPageSelectionForm` |
| `TemplateForm` | `BaseDialogForm` |
| `TrackbarDialog` | `TrackBarDialog` |
| `ExtentionMudole` control folder/subnamespace | `Controls` |
| `NameWithoutExtention` | `NameWithoutExtension` |
| `miximize` image resource | `Maximize` |
| `ZommIn_32` image resource | `ZoomIn_32` |
| `icons8-Foraward-arrow-80` asset | `icons8-Forward-arrow-80` |

### Standalone solution and .NET 10 migration

- Created a standalone repository containing the desktop application and its seven required supporting projects and assets.
- Renamed the solution to `DSP.Khana.Ocr.sln`.
- Migrated all eight production projects to SDK-style .NET 10 projects. The post-processing library targets `net10.0`; the desktop and Windows-dependent libraries target `net10.0-windows`.
- Added shared build settings in `Directory.Build.props` and stable .NET 10 SDK selection in `global.json`, allowing newer .NET 10 feature bands.
- Preserved existing assembly metadata and explicit source/resource lists while updating generated attributes for modern builds.
- Migrated package dependencies to `PackageReference` and removed legacy `packages.config` files and bundled package-cache dependencies.
- Updated required drawing and packaging dependencies to .NET 10 versions.
- Removed unused App Center, Newtonsoft.Json, SQLite/SQLitePCLRaw, standalone RuntimeInformation, and desktop System.Web dependencies.
- Updated dynamic assembly creation to `AssemblyBuilder.DefineDynamicAssembly` and retained required engine bitmap support.
- Added explicit designer serialization metadata required by the .NET 10 WinForms designer.
- Removed private signing-key build requirements and updated friend-assembly declarations for unsigned supporting libraries.
- Set the solution's minimum Visual Studio version to 18.0 and resolved migration-related build errors.
- Verified a self-contained Windows x64 publish during the migration. That publish predates the latest workflow refactoring and should be regenerated for the release.
- Installed .NET SDK 10.0.401, runtime/desktop runtime 10.0.12, and Visual Studio 2026 with the desktop workload on the development machine. These are development-environment changes, not files bundled into the application.

### Activation and repository hygiene

- Removed desktop activation dialogs, hardware fingerprinting, license-manager requirements, and licensing assembly dependencies from the desktop build profile (`DESKTOP_WITHOUT_LICENSING`).
- Preserved third-party copyright and license notices. Removing application activation does not change third-party licensing obligations.
- Added a README explaining architecture, required projects/assets, build/run/publish commands, services, resource ownership, and cancellation behavior.
- Added `.gitignore` coverage for build output, IDE state, publish output, package caches, logs, user configuration, and private signing keys.
- Added `.gitattributes` rules for resource-file line endings to preserve localized multiline values.
- Established the GitHub repository and main branch; private signing keys remain excluded from source control.

### Validation completed

- Debug solution build passed. The final incremental build reported zero warnings and zero errors; legacy dependency warnings can still appear on a clean build or another configuration.
- Release smoke tests and focused Debug refactoring tests passed.
- Checked 279 resource lookups across English, US English, and Persian, including strongly typed resource accessors.
- Tested form startup, image-list initialization and selection, live About translations, RTL/LTR switching, and resizing at 1280 × 720 and 1600 × 900.
- Exercised native English, Persian, and mixed OCR paths, production synchronous and asynchronous recognition, and native PDF rendering. These checks are functional smoke tests, not an OCR accuracy benchmark.
- Tested source-file unlocking, image-edit undo/redo, bounded-history disposal, thumbnail ownership, image removal, and form cleanup.
- Tested per-image edited/RTF state, duplicate filenames, formatted bilingual DOCX export/reload, collision-safe saving, and preservation of existing output files.
- Tested queued/running cancellation, native-call serialization, batch stopping, deferred form closure, overlapping-operation prevention, and control restoration after cancellation or failure.
- Checked localized access-denied and file-sharing errors and ran `git diff --check` successfully.

```powershell
dotnet build DSP.Khana.Ocr.sln -c Debug --nologo -v minimal
dotnet run --project tests/SmokeTests/SmokeTests.csproj -c Release
dotnet run --project tests/SmokeTests/SmokeTests.csproj -c Debug -- --refactoring-only
```

### Requirements and known limitations

- The application remains Windows-only. Development requires the .NET 10 SDK; IDE development requires Visual Studio 2026 (18.0 or newer) with the .NET desktop workload.
- Framework-dependent deployment requires the .NET 10 Desktop Runtime. Native OCR/PDF assets must match the process architecture and remain included in deployment.
- Cancellation cannot interrupt a native OCR call already processing a page. It waits for that call to finish, discards its result, and stops subsequent work.
- Interface-language preference is not persisted; startup uses the current thread's UI culture.
- The native wrapper extracts runtime/model files under `%APPDATA%\.temporalapp` and writes DSP.Tools native files beside the executable. These locations need write access.
- Legacy supporting-library warnings remain; the modernization does not represent a complete rewrite of those libraries.
- Before publishing the release, assign a version/date and regenerate deployment output from the final source revision.
