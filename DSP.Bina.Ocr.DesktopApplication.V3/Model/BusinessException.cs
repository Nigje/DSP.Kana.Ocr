using System;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class BusinessException : Exception
    {
        public BusinessException(string message, ExceptionType exceptionType) : base(message)
        {
            ExceptionType = exceptionType;
        }

        public ExceptionType ExceptionType { get; set; }
    }
    public enum ExceptionType
    {
        SelectedImage,
        InvalidFontSize,
        InvalidDirectory,
        InvalidFileDirectory,
    }
}
