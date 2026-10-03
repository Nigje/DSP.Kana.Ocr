// Copyright 2012-2022 Charles Weld.
// SPDX-License-Identifier: Apache-2.0
// Derived from https://github.com/charlesw/tesseract.
// Modified for DSP.Khana.Ocr: type visibility, local integration, and build/runtime
// compatibility where applicable; upstream namespaces and type names restored.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy at https://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
// See THIRD-PARTY-NOTICES.md and LICENSES/Tesseract-Apache-2.0.txt.

using System;

namespace Tesseract
{
    internal interface IResultRenderer : IDisposable
    {
        /// <summary>
        /// Begins a new document with the specified <paramref name="title"/>.
        /// </summary>
        /// <param name="title">The title of the new document.</param>
        /// <returns>A handle that when disposed of ends the current document.</returns>
        IDisposable BeginDocument(string title);

        /// <summary>
        /// Add the page to the current document.
        /// </summary>
        /// <param name="page"></param>
        /// <returns><c>True</c> if the page was successfully added to the result renderer; otherwise false.</returns>
        bool AddPage(Page page);

        /// <summary>
        /// Gets the current page number; returning -1 if no page has yet been added otherwise the number
        /// of the last added page (starting from 0).
        /// </summary>
        int PageNumber { get; }
    }
}