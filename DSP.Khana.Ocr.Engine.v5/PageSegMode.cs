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
using System.Collections.Generic;
using System.Text;

namespace Tesseract
{
    /// <summary>
    /// Represents the possible page layou analysis modes.
    /// </summary>
    public enum PageSegMode : int
    {
        /// <summary>
        /// Orientation and script detection (OSD) only.
        /// </summary>
        OsdOnly,

        /// <summary>
        /// Automatic page sementation with orientantion and script detection (OSD).
        /// </summary>
        AutoOsd,

        /// <summary>
        /// Automatic page segmentation, but no OSD, or OCR.
        /// </summary>
        AutoOnly,

        /// <summary>
        /// Fully automatic page segmentation, but no OSD.
        /// </summary>
        Auto,

        /// <summary>
        /// Assume a single column of text of variable sizes.
        /// </summary>
        SingleColumn,

        /// <summary>
        /// Assume a single uniform block of vertically aligned text.
        /// </summary>
        SingleBlockVertText,

        /// <summary>
        /// Assume a single uniform block of text.
        /// </summary>
        SingleBlock,

        /// <summary>
        /// Treat the image as a single text line.
        /// </summary>
        SingleLine,

        /// <summary>
        /// Treat the image as a single word.
        /// </summary>
        SingleWord,

        /// <summary>
        /// Treat the image as a single word in a circle.
        /// </summary>
        CircleWord,

        /// <summary>
        /// Treat the image as a single character.
        /// </summary>
        SingleChar,

        /// <summary>
        SparseText,

        /// <summary>
        /// Sparse text with orientation and script detection.
        /// </summary>
        SparseTextOsd,

        /// <summary>
        /// Treat the image as a single text line, bypassing hacks that are TesseractEngine-specific.
        /// </summary>
        RawLine,

        /// <summary>        
        /// Number of enum entries.
        /// </summary>
        Count,

    }
}
