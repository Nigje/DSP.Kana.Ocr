// SPDX-License-Identifier: MIT
// Modified for DSP.Khana.Ocr: wrapper integration and .NET runtime compatibility.
// See THIRD-PARTY-NOTICES.md and LICENSES/InteropDotNet-MIT.txt.

//  Copyright (c) 2014 Andrey Akinshin
//  Project URL: https://github.com/AndreyAkinshin/InteropDotNet
//  Distributed under the MIT License: http://opensource.org/licenses/MIT
using System;

namespace InteropDotNet
{
    interface ILibraryLoaderLogic
    {
        IntPtr LoadLibrary(string fileName);
        bool FreeLibrary(IntPtr libraryHandle);
        IntPtr GetProcAddress(IntPtr libraryHandle, string functionName);
        string FixUpLibraryName(string fileName);
    }
}