# Managed Tesseract wrapper

This project is derived from [Charles Weld's Tesseract .NET wrapper](https://github.com/charlesw/tesseract), licensed under Apache-2.0. It embeds [Andrey Akinshin's InteropDotNet](https://github.com/AndreyAkinshin/InteropDotNet), licensed under MIT. Credit and the applicable full license texts are in [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md) and [LICENSES](../LICENSES).

Upstream source namespaces and names are retained: `Tesseract`, `Tesseract.Internal`, `Tesseract.Interop`, `InteropDotNet`, `TesseractEngine`, `TesseractException`, `TesseractEnviornment`, `TessApi`, and `ITessApiSignatures`. Source filenames already follow the upstream wrapper, including its original `Enviornment` spelling. The solution project and assembly remain `DSP.Khana.Ocr.Engine.v5`.

Application-specific integration remains: internal type visibility and friend assemblies, the locally added `DSP.Khana.Ocr.Engine.v5.LanguageModel`, custom DLL-directory loading, the `BinaOcrEngineV5` and `liblept1780` native filenames, and the .NET 10 Windows build. The public application facade remains `Bina.Ocr.Wapper.BinaOcr`. Native/model extraction is managed by that facade.

The local validation blocks are excluded by the existing `DESKTOP_WITHOUT_LICENSING` build setting. This affects application activation only; it does not change the upstream open-source licenses. The exact original imported upstream revision has not yet been verified.

Modified upstream files carry attribution and modification notices. Both full licenses and the third-party notice are copied to build and publish output through this project.
