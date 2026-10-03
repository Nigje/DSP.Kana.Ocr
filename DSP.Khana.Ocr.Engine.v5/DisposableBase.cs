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
using System.Diagnostics;

namespace Tesseract
{
    public abstract class DisposableBase: IDisposable
	{
		static readonly TraceSource trace = new TraceSource("Tesseract");
		
		protected DisposableBase()
		{
			IsDisposed = false;
		}
		
		~DisposableBase() 
		{
			Dispose(false);
			trace.TraceEvent(TraceEventType.Warning, 0, "{0} was not disposed off.", this);
		}
		
		
		public void Dispose()
		{
			Dispose(true);
			
			IsDisposed = true;			
            GC.SuppressFinalize(this);

            if (Disposed != null) {
                Disposed(this, EventArgs.Empty);
            }
		}
		
		public bool IsDisposed { get; private set; }
        
        public event EventHandler<EventArgs> Disposed;

		
		protected virtual void VerifyNotDisposed()
		{
			if(IsDisposed) throw new ObjectDisposedException(ToString());
		}
		
		protected abstract void Dispose(bool disposing);
	}
}
