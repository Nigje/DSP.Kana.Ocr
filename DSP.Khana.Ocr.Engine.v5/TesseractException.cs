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
using System.Runtime.Serialization;

namespace Tesseract
{
	/// <summary>
	/// Desctiption of TesseractException.
	/// </summary>
	[Serializable]
    internal class TesseractException : Exception, ISerializable
	{
		public TesseractException()
		{
		}

		public TesseractException(string message) : base(message)
		{
		}

		public TesseractException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected TesseractException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
