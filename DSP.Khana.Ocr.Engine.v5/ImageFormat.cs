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
    public enum ImageFormat : int
    {
        Unknown = 0,
        Bmp = 1,
        JfifJpeg = 2,
        Png = 3,
        Tiff = 4,
        TiffPackBits = 5,
        TiffRle = 6,
        TiffG3 = 7,
        TiffG4 = 8,
        TiffLzw =  9,
        TifZip = 10,
        Pnm = 11,
        Ps = 12,
        Gif = 13,
        Jp2 = 14,
        WebP = 15,
        Lpdf = 16,
        Default = 17,
        Spix = 18
    }
}
