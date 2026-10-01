using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bina.Ocr.Wapper
{
    //*****************************************************************************************************
    public enum EngineModeEnum
    {
        /// <summary>
        /// ساختاری
        /// </summary>
        KhanaStructuralOnly,
        /// <summary>
        /// پردازش عمیق
        /// </summary>
        KhanaDeepOnly,
        /// <summary>
        /// ساختاری و پردازش عمیق
        /// </summary>
        KhanaStructuralAndKhanaDeep,
        /// <summary>
        /// پیشفرض
        /// </summary>
        Default
    }
    //*****************************************************************************************************
    public enum LanguageEnum
    {
        Mix,
        Farsi,
        English

    }
    //*****************************************************************************************************
    public enum PageSegmentationModeEnum
    {
        /// <summary>
        /// بازشناسی تصویر با فرض وجود چرخش و متن های اسکریپت وار در متن
        /// Orientation and script detection (OSD) only.
        /// </summary>
        OsdOnly,

        /// <summary>
        /// بازشناسی تصویر با تشخیص ساختار اتوماتیک و با فرض وجود چرخش و متن های اسکریپتی
        /// Automatic page sementation with orientantion and script detection (OSD).
        /// </summary>
        AutoOsd,

        /// <summary>
        /// تشخیص ساختار اتوماتیک بدون تشخیص چرخش و نواحی اسکریپ وار 
        /// Automatic page segmentation, but no OSD, or OCR.
        /// </summary>
        AutoOnly,

        /// <summary>
        /// بازشناسی تصویر بدون تشخیص چرخش یا نواحی اسکریپت وار
        /// Fully automatic page segmentation, but no OSD.
        /// </summary>
        Auto,

        /// <summary>
        /// بازشناسی تصویر با فرض آنکه تصویر متن یک ستونه است.
        /// Assume a single column of text of variable sizes.
        /// </summary>
        SingleColumn,

        /// <summary>
        /// بازشناسی تصویر با فرض یک بلاک متنی که از یک سمت مرتب است
        /// Assume a single uniform block of vertically aligned text.
        /// </summary>
        SingleBlockVertText,

        /// <summary>
        /// بازشناسی تصویر با فرض یک بلاک متنی
        /// Assume a single uniform block of text.
        /// </summary>
        SingleBlock,

        /// <summary>
        /// بازشناسی تصویر با فرض آنکه تصویر از یک خط نوشته تشکیل شده است
        /// Treat the image as a single text line.
        /// </summary>
        SingleLine,

        /// <summary>
        /// بازشناسی تصویر با فرض آنکه تصویر از یک کلمه تشکیل شده است.
        /// Treat the image as a single word.
        /// </summary>
        SingleWord,

        /// <summary>
        /// بازشناسی تصویر با فرض آنکه تصویر از یک کلمه با انحنا تشکیل شده است.
        /// Treat the image as a single word in a circle.
        /// </summary>
        CircleWord,

        /// <summary>
        /// بازشناسی تصویر با فرض آنکه که تصویر از یک کاراکتر تشکیل شده است
        /// Treat the image as a single character.
        /// </summary>
        SingleChar,

        /// <summary>
        /// بازشناسی تصویر با فرض آنکه که تصویر از متن های پراکنده تشکیل شده است.
        /// </summary>
        SparseText,

        /// <summary>
        /// بازشناسی تصویر با فرض آنکه تصویر از متن های پراکنده و کج و اسکریپت وار تشکیل شده است.
        /// Sparse text with orientation and script detection.
        /// </summary>
        SparseTextOsd,

        /// <summary>
        /// باز شناسی تصویر با فرض آنکه تصویر ورودی از یک خط تشکیل شده است - حالت دوم
        /// Treat the image as a single text line, bypassing hacks that are KhanaOcrEngine-specific.
        /// </summary>
        RawLine,

        /// <summary>        
        /// بازشناسی تصویر با فرض تعدادی از حالت ها
        /// Number of enum entries.
        /// </summary>
        Count,
    }
}
