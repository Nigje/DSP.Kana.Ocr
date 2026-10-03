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
    /// Represents the parameters for a sweep search used by scew algorithms.
    /// </summary>
    public struct ScewSweep
    {
        public static ScewSweep Default = new ScewSweep(DefaultReduction, DefaultRange, DefaultDelta);

        #region Constants and Fields

        public const int DefaultReduction = 4; // Sweep part; 4 is good
        public const float DefaultRange = 7.0F;
        public const float DefaultDelta = 1.0F;
        
        private int reduction;
        private float range;
        private float delta;

        #endregion

        #region Factory Methods + Constructor
        
        public ScewSweep(int reduction = DefaultReduction, float range = DefaultRange, float delta = DefaultDelta)
        {
            this.reduction = reduction;
            this.range = range;
            this.delta = delta;
        }

        #endregion

        #region Properties

        public int Reduction
        {
            get { return reduction; }
        }
        
        public float Range
        {
            get { return range; }
        }

        public float Delta
        {
            get { return delta; }
        }

        #endregion

    }
}
