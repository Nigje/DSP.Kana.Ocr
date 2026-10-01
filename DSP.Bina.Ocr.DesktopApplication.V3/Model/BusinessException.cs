using System;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class BusinessException : Exception
    {
        public BusinessException(string message, ExceptionType exceptionType) : base(message)
        {
            ExceptionType = exceptionType;
        }

        public ExceptionType ExceptionType { get; }
    }
    public enum ExceptionType
    {
        SelectedImage,
        ProcessingInProgress,
        InvalidFontSize,
        InvalidDirectory,
        InvalidFileDirectory,
    }
}
