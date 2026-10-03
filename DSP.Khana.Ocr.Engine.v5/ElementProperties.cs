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
    /// Represents properties that describe a text block's orientation.
    /// </summary>
    internal struct ElementProperties
    {
        private Orientation orientation;
        private TextLineOrder textLineOrder;
        private WritingDirection writingDirection;
        private float deskewAngle;

        public ElementProperties(Orientation orientation, TextLineOrder textLineOrder, WritingDirection writingDirection, float deskewAngle)
        {
            this.orientation = orientation;
            this.textLineOrder = textLineOrder;
            this.writingDirection = writingDirection;
            this.deskewAngle = deskewAngle;
        }

        /// <summary>
        /// Gets the <see cref="Orientation" /> for corresponding text block.
        /// </summary>
        public Orientation Orientation
        {
            get { return orientation; }
        }

        /// <summary>
        /// Gets the <see cref="TextLineOrder" /> for corresponding text block.
        /// </summary>
        public TextLineOrder TextLineOrder
        {
            get { return textLineOrder; }
        }

        /// <summary>
        /// Gets the <see cref="WritingDirection" /> for corresponding text block.
        /// </summary>
        public WritingDirection WritingDirection
        {
            get { return writingDirection; }
        }

        /// <summary>
        /// Gets the angle the page would need to be rotated to deskew the text block.
        /// </summary>
        public float DeskewAngle
        {
            get { return deskewAngle; }
        }
    }
}
