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
    // This class is the return type of
    // ResultIterator.GetWordFontAttributes().  We can't
    // use FontInfo directly because there are properties
    // here that are not accounted for in FontInfo
    // (smallcaps, underline, etc.)  Because of the caching
    // scheme we're using for FontInfo objects, we can't simply
    // augment that class since these extra properties are not
    // accounted for by the FontInfo's unique ID.
    internal class FontAttributes
    {
        public FontInfo FontInfo { get; private set; }

        public bool IsUnderlined { get; private set; }
        public bool IsSmallCaps { get; private set; }
        public int PointSize { get; private set; }

        public FontAttributes(
            FontInfo fontInfo, bool isUnderlined, bool isSmallCaps, int pointSize)
        {
            FontInfo = fontInfo;
            IsUnderlined = isUnderlined;
            IsSmallCaps = isSmallCaps;
            PointSize = pointSize;
        }
    }
}
