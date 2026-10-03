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
    public enum PolyBlockType : int
    {
        /// <summary>
        /// The type is not known yet, keep as first element.
        /// </summary>
        Unknown,
        /// <summary>
        /// The text is inside a column.
        /// </summary>
        FlowingText,
        /// <summary>
        /// The text spans more than one column.
        /// </summary>
        HeadingText,
        /// <summary>
        /// The text is in a cross-column pull-out region.
        /// </summary>
        PullOutText,
        /// <summary>
        /// The partion belongs to an equation region..
        /// </summary>
        Equation,
        /// <summary>
        /// The partion has an inline equation.
        /// </summary>
        InlineEquation,
        /// <summary>
        /// The partion belongs to a Table region.
        /// </summary>
        Table,
        /// <summary>
        /// Text line runs vertically.
        /// </summary>
        VerticalText,
        /// <summary>
        /// Text that belongs to an image.
        /// </summary>
        CaptionText,
        /// <summary>
        /// Image that lives inside a column.
        /// </summary>
        FlowingImage, 
        /// <summary>
        /// Image that spans more than one column.
        /// </summary>
        HeadingImage,
        /// <summary>
        /// Image that is in a cross-column pull-out region.
        /// </summary>
        PullOutImage,
        HorizontalLine, 
        VerticalLine,
        /// <summary>
        /// Lies outside any column.
        /// </summary>
        Noise,
        Count
    }
}
