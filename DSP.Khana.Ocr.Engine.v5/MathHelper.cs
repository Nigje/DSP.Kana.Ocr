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
    internal static class MathHelper
    {
        /// <summary>
        /// Convert a degrees to radians.
        /// </summary>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static float ToRadians(float angleInDegrees)
        {
            return (float)ToRadians((double)angleInDegrees);
        }

        /// <summary>
        /// Convert a degrees to radians.
        /// </summary>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static double ToRadians(double angleInDegrees)
        {
            return angleInDegrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Calculates the smallest integer greater than the quotant of dividend and divisor.
        /// </summary>
        /// <see href="http://stackoverflow.com/questions/921180/how-can-i-ensure-that-a-division-of-integers-is-always-rounded-up"/>
        public static int DivRoundUp(int dividend, int divisor)
        {
            var result = dividend / divisor;
            
            
            return (dividend % divisor != 0 && divisor > 0 == dividend > 0) ? result + 1 : result;
        }
    }
}
