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


namespace Tesseract
{
    /// <summary>
    /// The text lines are read in the given sequence.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For example in English the order is top-to-bottom. Chinese vertical text lines
    /// are read right-to-left. While Mongolian is written in vertical columns
    /// like Chinese but read left-to-right.
    /// </para>
    /// <para>
    /// Note that only some combinations makes sense for example <see cref="WritingDirection.LeftToRight"/> implies
    /// <see cref="TextLineOrder.TopToBottom" />.
    /// </para>
    /// </remarks>
    public enum TextLineOrder : int
    {
    	/// <summary>
    	/// The text lines form vertical columns ordered left to right.
    	/// </summary>
        LeftToRight,
        
    	/// <summary>
    	/// The text lines form vertical columns ordered right to left.
    	/// </summary>
        RightToLeft,   
        
    	/// <summary>
    	/// The text lines form horizontal columns ordered top to bottom.
    	/// </summary>
        TopToBottom
    }
}
