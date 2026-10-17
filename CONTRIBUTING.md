# Contributing to Bina OCR

Start with the [developer guide](docs/development.md) for Windows setup and verification, and the [engineering case study](docs/engineering.md) for the design boundaries.

## Reporting a problem

Include the revision, Windows version, process architecture, selected interface and recognition languages, reproduction steps, and expected versus actual behavior. For recognition problems, describe the source script and image quality. Share only sample documents you have permission to publish; replace private content with a minimal reproduction.

## Making a change

Keep each change focused on a concrete behavior. Explain its trigger, the resulting behavior, and the verification performed. Use descriptive commit subjects such as `Preserve editor formatting when switching recognition language`.

Use the existing service boundaries: forms coordinate interaction; services handle import, edits, recognition, capture, and export. Document who owns a bitmap or native resource when adding a new path. Keep OCR options captured for each run, honor the shared native gate, and account for cancellation and shutdown.

For a behavioral fix, extend the relevant existing smoke checks when it guards a meaningful regression. The test harness is an executable, not a `dotnet test` project:

```powershell
./scripts/Verify.ps1
```

Use `-Suite Forms` or `-Suite Workflows` while iterating, then run the full suite when native integration or shared behavior changes. Report the command and result; distinguish executed checks from checks still pending.

For UI changes, inspect English and Persian, LTR/RTL direction, edited document content, and resizing. Keep resource keys paired and retain text formatting and selection through language changes.

## Attribution and release scope

Preserve upstream copyright and modification notices. Changes to copied engine code must retain its provenance; see [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) and the [engine README](DSP.Khana.Ocr.Engine.v5/README.md). Do not infer a native binary or model's license from its filename.

The repository has no selected license for independently authored application code. Discuss licensing and contribution terms with the maintainer before submitting code; this guide does not grant a license or define a contributor agreement.
