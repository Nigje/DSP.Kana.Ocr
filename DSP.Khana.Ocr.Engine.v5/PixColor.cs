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
using System.Runtime.InteropServices;
using System.Text;

namespace Tesseract
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct PixColor : IEquatable<PixColor>
    {
        private byte red;
        private byte blue;
        private byte green;
        private byte alpha;

        public PixColor(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        public byte Red { get { return red; } }
        public byte Green { get { return green; } }
        public byte Blue { get { return blue; } }
        public byte Alpha { get { return alpha; } }

        public static PixColor FromRgba(uint value)
        {
            return new PixColor(
               (byte)((value >> 24) & 0xFF),
               (byte)((value >> 16) & 0xFF),
               (byte)((value >> 8) & 0xFF),
               (byte)(value & 0xFF));
        }

        public static PixColor FromRgb(uint value)
        {
            return new PixColor(
               (byte)((value >> 24) & 0xFF),
               (byte)((value >> 16) & 0xFF),
               (byte)((value >> 8) & 0xFF),
               (byte)0xFF);
        }

        public uint ToRGBA()
        {
            return (uint)((red << 24) |
               (green << 16) |
               (blue << 8) |
               alpha);
        }

#if NETFULL
        public static explicit operator System.Drawing.Color(PixColor color)
        {
            return System.Drawing.Color.FromArgb(color.alpha, color.red, color.green, color.blue);
        }

        public static explicit operator PixColor(System.Drawing.Color color)
        {
            return new PixColor(color.R, color.G, color.B, color.A);
        }
#endif


#region Equals and GetHashCode implementation
        public override bool Equals(object obj)
		{
			return (obj is PixColor) && Equals((PixColor)obj);
		}
        
		public bool Equals(PixColor other)
		{
			return this.red == other.red && this.blue == other.blue && this.green == other.green && this.alpha == other.alpha;
		}
        
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * red.GetHashCode();
				hashCode += 1000000009 * blue.GetHashCode();
				hashCode += 1000000021 * green.GetHashCode();
				hashCode += 1000000033 * alpha.GetHashCode();
			}
			return hashCode;
		}
        
		public static bool operator ==(PixColor lhs, PixColor rhs)
		{
			return lhs.Equals(rhs);
		}
        
		public static bool operator !=(PixColor lhs, PixColor rhs)
		{
			return !(lhs == rhs);
		}
#endregion

        public override string ToString()
        {
            return String.Format("Color(0x{0:X})", ToRGBA());
        }

    }
}
