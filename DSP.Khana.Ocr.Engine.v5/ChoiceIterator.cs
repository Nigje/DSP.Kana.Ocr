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
using System.Runtime.InteropServices;

namespace Tesseract
{
    /// <summary>
    /// Class to iterate over the classifier choices for a single symbol.
    /// </summary>
    internal sealed class ChoiceIterator : DisposableBase
    {
        private readonly HandleRef _handleRef;
        
        internal ChoiceIterator(IntPtr handle)
        {
            this._handleRef = new HandleRef(this, handle);
        }

        /// <summary>
        /// Moves to the next choice for the symbol and returns false if there are none left.
        /// </summary>
        /// <returns>true|false</returns>
        public bool Next()
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return false;
            return Interop.TessApi.Native.ChoiceIteratorNext(_handleRef) != 0;
        }

        /// <summary>
        /// Returns the confidence of the current choice.        
        /// </summary>
        /// <remarks>
        /// The number should be interpreted as a percent probability. (0.0f-100.0f)
        /// </remarks>
        /// <returns>float</returns>
        public float GetConfidence()
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)
                return 0f;

            return Interop.TessApi.Native.ChoiceIteratorGetConfidence(_handleRef);
        }

        /// <summary>
        /// Returns the text string for the current choice.
        /// </summary>
        /// <returns>string</returns>
        public string GetText()
        {
            VerifyNotDisposed();
            if (_handleRef.Handle == IntPtr.Zero)            
                return String.Empty;
            
            return Interop.TessApi.ChoiceIteratorGetUTF8Text(_handleRef);
        }

        protected override void Dispose(bool disposing)
        {
            if (_handleRef.Handle != IntPtr.Zero)
            {
                Interop.TessApi.Native.ChoiceIteratorDelete(_handleRef);
            }
        }
    }
}