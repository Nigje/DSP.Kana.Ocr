# Development and verification

## Build and run

Use Windows and the .NET 10 SDK. `global.json` selects a stable .NET 10 SDK and allows newer feature bands. For IDE development, open [DSP.Khana.Ocr.sln](../DSP.Khana.Ocr.sln) in Visual Studio 2026 with the .NET desktop development workload and select `DSP.Bina.Ocr.DesktopApplication.V3` as the startup project.

```powershell
dotnet restore DSP.Khana.Ocr.sln
dotnet build DSP.Khana.Ocr.sln -c Debug
dotnet build DSP.Khana.Ocr.sln -c Release
dotnet run --project DSP.Bina.Ocr.DesktopApplication.V3 -c Debug
```

The solution contains six production projects. Namespaces, assembly identities, and historical spellings such as `Wapper` remain for compatibility. The post-processing library targets `net10.0`; Windows-dependent projects target `net10.0-windows`.

NuGet dependencies use PackageReference and the normal user package cache. Keep embedded native DLLs, model archives, and resource images: they are runtime inputs, not generated build output. Desktop builds define `DESKTOP_WITHOUT_LICENSING`, excluding the legacy activation path.

## Repeatable verification

Run from PowerShell on Windows:

```powershell
./scripts/Verify.ps1
./scripts/Verify.ps1 -Suite Forms
./scripts/Verify.ps1 -Suite Workflows -Configuration Debug
```

The script restores the smoke-test project and its references, builds them, and executes the integration harness. Every failed command stops the script. Output is written to `artifacts/bin/verification/<Configuration>/`, avoiding locks held by an application running from its normal desktop output. Do not run two verification processes for the same configuration simultaneously.

`-NoRestore` uses existing restored packages, useful when verifying offline after a successful restore:

```powershell
./scripts/Verify.ps1 -NoRestore
```

| Suite | Coverage |
| --- | --- |
| `All` (default) | Resources, DOCX generation/schema/reload, native English/Persian/mixed model paths, image history, production forms, workflow regressions, production OCR facade, and native PDF rendering. |
| `Forms` | Form initialization, image-list mounting and selection, language/recognition alignment, editor text/formatting/selection, redraw recovery, live About translation, adjustment dialogs, and resizing. |
| `Workflows` | Bitmap ownership and disposal, detached imports, formatted export and output collisions, per-image state, OCR serialization/cancellation/failure, deferred close, and typed error messages. |

The focused workflow suite substitutes controlled OCR completion for native recognition. The full suite additionally validates the actual bundled native dependencies. Forms are transparent and close automatically. The harness sets `Bina.Ocr.RuntimeDirectory` through `AppContext` before native initialization, isolating extracted OCR assets and samples in its unique temporary directory. The PDF helper is written beside the verification executable. Windows may retain loaded DLLs in temporary output until process exit.

The harness has an STA entry point and returns a nonzero exit code on failure. Use the script or `dotnet run --project tests/SmokeTests/SmokeTests.csproj -c Release`. It is not a test-adapter project, so `dotnet test` alone does not execute the checks.

### Local verification recorded October 17, 2026

After a successful restore, `./scripts/Verify.ps1 -NoRestore` completed the Release build and full integration suite with .NET SDK 10.0.401 on Windows. Checks passed for 285 resource lookups, DOCX schema and round-trip validation, all three native model paths, image history, bilingual forms, workflow lifecycle regressions, the production OCR facade, isolated runtime extraction, and native PDF rendering. Existing wrapper warnings remain. This records a local run; hosted CI and clean-machine deployment remain separate checks.

## Continuous integration

[Windows verification](../.github/workflows/windows-verification.yml) runs the same script on pushes, pull requests, and manual dispatch. It installs the .NET 10 SDK and uses read-only repository permissions. The workflow builds and tests; it does not publish a release.

Action configuration follows the official [checkout](https://github.com/actions/checkout) and [setup-dotnet](https://github.com/actions/setup-dotnet) documentation. Local verification does not establish the hosted workflow's result; inspect the first GitHub run after pushing.

## Project map

| Project | Responsibility |
| --- | --- |
| [Desktop](../DSP.Bina.Ocr.DesktopApplication.V3/DSP.Bina.Ocr.DesktopApplication.V3.csproj) | WinForms application, workflow orchestration, editor, document snapshots, application services, localization. |
| [Image tools](../DSP.Khana.Image.Tools/DSP.Khana.ImageTools.csproj) | Image preparation, deskew, conversion, and native PDF rendering helpers. |
| [OCR facade](../DSP.Khana.Ocr.Wapper/DSP.Khana.Ocr.Wapper.csproj) | `BinaOcr`, option mapping, native/model extraction, recognition, and optional cleanup. |
| [Managed engine](../DSP.Khana.Ocr.Engine.v5/DSP.Khana.Ocr.Engine.v5.csproj) | Modified upstream Tesseract .NET wrapper and native interop; see its [attribution guide](../DSP.Khana.Ocr.Engine.v5/README.md). |
| [Post-processing](../DSP.Khana.Ocr.PostPRocessing/DSP.Khana.Ocr.PostProcessing.csproj) | Optional text cleanup rules. |
| [ImageListView](../OtherUsefulLibrary/ImageListView/ImageListView.csproj) | Thumbnail display, caching, selection, and rendering. |
| [Smoke tests](../tests/SmokeTests/SmokeTests.csproj) | Executable integration and regression checks, outside the production solution. |

`MainForm.Workflows.cs` coordinates recognition and resource cleanup; `MainForm.Localization.cs` applies translations and recognition defaults; `MainForm.Layout.cs` handles directional placement. Read the [engineering case study](engineering.md) for the service boundaries and tradeoffs.

## Native assets and deployment

The complete desktop output lives under `DSP.Bina.Ocr.DesktopApplication.V3/bin/<Configuration>/net10.0-windows/`. Distribute the whole folder, including managed dependencies, `.deps.json`, `.runtimeconfig.json`, the `fa-IR` satellite resource folder, notices, licenses, and native subfolders. A framework-dependent deployment needs the .NET 10 Desktop Runtime.

The wrapper extracts OCR libraries and models under `%APPDATA%\.temporalapp`, in version-specific `.app_<version>` and `.model_<version>` directories. It also writes `DSP.Tools32.dll` or `DSP.Tools64.dll` beside the executable. Both locations require write access. PDF import uses a unique session directory under `%TEMP%\DSP.Khana.Ocr`. Local error diagnostics are written to `%LOCALAPPDATA%\DSP.Khana.Ocr\Logs\errors.log`.

An embedding host can call `AppContext.SetData("Bina.Ocr.RuntimeDirectory", absolutePath)` before the first `BinaOcr` use to choose an OCR extraction root; `.temporalapp` is created beneath it. Use a directory owned by that host because initialization recreates its versioned model directory. The smoke harness uses a fresh root for each process, leaving the normal desktop cache untouched. This setting does not relocate the PDF helper.

To prepare self-contained x64 output:

```powershell
dotnet publish DSP.Bina.Ocr.DesktopApplication.V3 -c Release -r win-x64 --self-contained true -o publish/win-x64
```

Keep trimming and Native AOT disabled because the integration uses WinForms resources, reflection, and dynamically emitted interop. Bundled native assets cover x86/x64; ARM64 is not covered. A successful managed build alone does not validate native loading or a published deployment.

## Localization

Add matching keys to `Strings.resx` and `Strings.fa-IR.resx`, regenerate `Strings.Designer.cs`, and reference text through `Properties.Strings`. Update localization bindings and directional layout as needed. The [resource table](../DSP.Bina.Ocr.DesktopApplication.V3/Localization.Resources.md) is a reference; the resource files are authoritative.

Switching UI language selects English or Persian recognition. Recognition language can then be changed manually, including mixed mode. The UI preference is not persisted; startup follows the thread's UI culture. Check both directions with text already edited and formatted, an About dialog open, and more than one window size.

## Current limitations

- Document state and image history are session-only; there is no saved workspace format.
- Cancellation cannot interrupt the active native page. Closing normally waits for it to finish.
- Bounded history limits entry count, not memory in bytes.
- DOCX export preserves supported editor formatting; it does not reproduce scanned page geometry.
- Existing supporting libraries can emit warnings on a clean build. There is no blanket warnings-as-errors policy.
- OCR integration checks are functional smoke checks, without a representative accuracy dataset or performance baseline.

## Release readiness

The repository is useful for source review, but a complete redistribution needs additional decisions and evidence:

1. Select a license for independently authored application code after confirming ownership. No repository-wide license has been chosen.
2. Complete the dependency and asset inventory, including native OCR/Leptonica, trained models, PDF/Ghostscript integration, ImageListView, and artwork. The current [third-party notice](../THIRD-PARTY-NOTICES.md) explicitly has partial scope.
3. Assign a version and release date in [CHANGELOG.md](../CHANGELOG.md).
4. Run the full verification command on the final revision and inspect the hosted Windows workflow result.
5. Regenerate publish output and exercise import, native OCR, PDF rendering, bilingual editing, and export from that output on a clean Windows installation.

The [source comparison](open-source-audit/README.md) is a historical provenance record, not a complete dependency audit. Preserve its dated findings and the upstream notices when preparing a release.
