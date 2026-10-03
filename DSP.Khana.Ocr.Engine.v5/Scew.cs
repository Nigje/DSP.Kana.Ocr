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
    public struct Scew
    {
        private float angle;
        private float confidence;

        public Scew(float angle, float confidence)
        {
            this.angle = angle;
            this.confidence = confidence;
        }

        public float Angle
        {
            get { return angle; }
        }


        public float Confidence
        {
            get { return confidence; }
        }

        #region ToString

        public override string ToString()
        {
            return String.Format("Scew: {0} [conf: {1}]", Angle, Confidence);
        }

        #endregion

        #region Equals and GetHashCode implementation
        public override bool Equals(object obj)
        {
            return (obj is Scew) && Equals((Scew)obj);
        }

        public bool Equals(Scew other)
        {
            return this.confidence == other.confidence && this.angle == other.angle;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked {
                hashCode += 1000000007 * angle.GetHashCode();
                hashCode += 1000000009 * confidence.GetHashCode();
            }
            return hashCode;
        }

        public static bool operator ==(Scew lhs, Scew rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Scew lhs, Scew rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
        
    }
}
