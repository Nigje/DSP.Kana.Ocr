using System;

namespace DSP.Khana.Ocr.Interop
{
    /// <summary>
    /// Description of Constants.
    /// </summary>
    internal static class Constants
    {
        public const string LeptonicaDllName = "liblept1780";
        public const string KhanaOcrEngineDllName = "BinaOcrEngineV5";
        
        // KhanaOcrEngine uses an int to represent true false values.
        public const int TRUE = 1;
        public const int FALSE = 0;
    }
}