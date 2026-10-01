using System;
using System.Collections.Generic;
using System.Text;

namespace DSP.Khana.Ocr
{
    public enum PageIteratorLevel : int
    {
        Block,
        Para, 
        TextLine, 
        Word, 
        Symbol
    }
}
