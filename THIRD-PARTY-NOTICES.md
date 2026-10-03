# Third-party notices: managed OCR engine

`DSP.Khana.Ocr.Engine.v5` contains a modified copy of Charles Weld's Tesseract .NET wrapper, which incorporates Andrey Akinshin's InteropDotNet. These authors retain copyright in their original work. Local modifications do not imply their endorsement of this application.

## Tesseract .NET wrapper

- Author/copyright: **Copyright 2012-2022 Charles Weld**, with contributions from the upstream project contributors.
- Source: <https://github.com/charlesw/tesseract>
- License: **Apache License, Version 2.0 (Apache-2.0)**.
- Full license: [LICENSES/Tesseract-Apache-2.0.txt](LICENSES/Tesseract-Apache-2.0.txt).
- Included source: `DSP.Khana.Ocr.Engine.v5`, except the MIT InteropDotNet files identified below and the locally added `LanguageModel.cs`.

The original import revision has not been verified. Comparison reference: upstream commit `b5329d5be92fa670031d94c3875f879651a01f55`; this is an audit reference, not a claim that this application includes that exact version.

Local adaptations include type visibility, friend assembly access, language-model parameters and validation, a custom native-library directory, native library names, .NET build/runtime compatibility, and local assembly metadata. The upstream `Tesseract`, `Tesseract.Internal`, and `Tesseract.Interop` namespaces and original engine/API/type names have been restored. Source filenames follow the upstream wrapper. The project/assembly retains its local identity, `DSP.Khana.Ocr.Engine.v5`. These changes are identified in source-file headers.

The existing `DESKTOP_WITHOUT_LICENSING` build setting excludes the local application-license validation code. That setting does not waive or alter the open-source license obligations.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use the upstream software except in compliance with the License. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the included License for the specific language governing permissions and limitations under the License.

## InteropDotNet

- Author/copyright: **Copyright (c) 2014 Andrey Akinshin**.
- Source: <https://github.com/AndreyAkinshin/InteropDotNet>
- License: **MIT**.
- Full copyright, permission notice, and warranty disclaimer: [LICENSES/InteropDotNet-MIT.txt](LICENSES/InteropDotNet-MIT.txt).
- Included source: `DSP.Khana.Ocr.Engine.v5/Internal/InteropDotNet/*.cs` and `DSP.Khana.Ocr.Engine.v5/Internal/Logger.cs`, derived from InteropDotNet's `LibraryLoaderTrace.cs` through the Tesseract wrapper.

Comparison reference: upstream commit `0b15eee809716c50562458d6fe95d52bea46b9c6`; the original import revision is unverified. The `InteropDotNet` namespace and original filenames within that directory are retained. `Logger.cs` follows the Tesseract wrapper's filename and namespace.

The embedded copy includes wrapper-specific library search and logging integration, native-function error handling, and runtime compatibility changes. Some of these adaptations were inherited from the Tesseract wrapper; they are not claimed as original application code. Original MIT headers remain in the source.

## Distribution and scope

Keep this notice and both full license texts with source and binary distributions containing these components. The engine project copies them into build and publish output, including output reached through project references, under `THIRD-PARTY-NOTICES.md` and `LICENSES/`.

This notice covers the two managed-source components above. It is not a complete license inventory for the solution or for the renamed native binaries, bundled OCR models, Ghostscript integration, Xceed/DocX, ImageListView, and other dependencies/assets. The existing PDF smoke test reports Ghostscript 9.50 and GNU AGPLv3. Those components retain their respective terms and require separate release review. Native binary filenames are retained for compatibility and do not establish ownership or licensing.

No repository-wide license for independently authored application code is selected by this notice. The local `LanguageModel.cs` is application code, not attributed to either upstream author. A license for independently authored code must be chosen before presenting the complete project as open source; these third-party license texts do not make that choice.
