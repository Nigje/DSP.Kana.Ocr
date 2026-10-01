using Bina.Ocr.Wapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class PairLanguage
    {
        public string Text { get; set; }
        public LanguageEnum Value { get; set; }
    }
    public class PairEngine
    {
        public string Text { get; set; }
        public EngineModeEnum Value { get; set; }
    }
    public class PairPageSegmentationMode
    {
        public string Text { get; set; }
        public PageSegmentationModeEnum Value { get; set; }
    }
}
