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

using Tesseract.Interop;

namespace Tesseract
{
    internal sealed class ResultIterator : PageIterator
    {
        internal ResultIterator(Page page, IntPtr handle)
            : base(page, handle)
        {
        }

        public float GetConfidence(PageIteratorLevel level)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero)
                return 0f;

            return Interop.TessApi.Native.ResultIteratorGetConfidence(handle, level);
        }

        public string GetText(PageIteratorLevel level)
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return String.Empty;
            }

            return Interop.TessApi.ResultIteratorGetUTF8Text(handle, level);
        }
        
        private Dictionary<int, FontInfo> _fontInfoCache = new Dictionary<int, FontInfo>();
        public FontAttributes GetWordFontAttributes() {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return null;
            }

            bool isBold, isItalic, isUnderlined, isMonospace, isSerif, isSmallCaps;
            int pointSize, fontId;

           
            // this return value points to an internal table and should not be deleted.
            IntPtr nameHandle =
                Interop.TessApi.Native.ResultIteratorWordFontAttributes(
                    handle,
                    out isBold, out isItalic, out isUnderlined,
                    out isMonospace, out isSerif, out isSmallCaps,
                    out pointSize, out fontId);

            // This can happen in certain error conditions
            if (nameHandle == IntPtr.Zero) {
                return null;
            }

            FontInfo fontInfo;
            if (!_fontInfoCache.TryGetValue(fontId, out fontInfo)) {
                string fontName = MarshalHelper.PtrToString(nameHandle, Encoding.UTF8);
                fontInfo = new FontInfo(fontName, fontId, isItalic, isBold, isMonospace, isSerif);
                _fontInfoCache.Add(fontId, fontInfo);
            }

            return new FontAttributes(fontInfo, isUnderlined, isSmallCaps, pointSize);
        }

        public string GetWordRecognitionLanguage()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return null;
            }

            return Interop.TessApi.ResultIteratorWordRecognitionLanguage(handle);
        }

        public bool GetWordIsFromDictionary()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.Native.ResultIteratorWordIsFromDictionary(handle);
        }

        public bool GetWordIsNumeric()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.Native.ResultIteratorWordIsNumeric(handle);
        }

        public bool GetSymbolIsSuperscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.Native.ResultIteratorSymbolIsSuperscript(handle);
        }

        public bool GetSymbolIsSubscript()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.Native.ResultIteratorSymbolIsSubscript(handle);
        }

        public bool GetSymbolIsDropcap()
        {
            VerifyNotDisposed();
            if (handle.Handle == IntPtr.Zero) {
                return false;
            }

            return Interop.TessApi.Native.ResultIteratorSymbolIsDropcap(handle);
        }

        /// <summary>
        /// Gets an instance of a choice iterator using the current symbol of interest. The ChoiceIterator allows a one-shot iteration over the
        /// choices for this symbol and after that is is useless.
        /// </summary>
        /// <returns>an instance of a Choice Iterator</returns>
        public ChoiceIterator GetChoiceIterator()
        {
            var choiceIteratorHandle = Interop.TessApi.Native.ResultIteratorGetChoiceIterator(this.handle);
            if (choiceIteratorHandle == IntPtr.Zero)
                return null;
            return new ChoiceIterator(choiceIteratorHandle);
        }
    }
}
