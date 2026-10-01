using System;
using System.Collections.Generic;
using System.Text;

namespace DSP.Khana.Ocr.Engine.v5
{
    internal class LanguageModel
    {
        public string Language { get; set; }

        public bool IsValid { get; set; }
        public string UID { get; set; }
        public string AppKey { get; set; }
        public string AppName { get; set; }
        public string LicenseAppName { get; set; }
        public DateTime ExpireDateTime { get; set; }
    }
}
