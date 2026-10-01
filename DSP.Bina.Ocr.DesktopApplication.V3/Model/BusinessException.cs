using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class BusinessException:Exception
    {
        public BusinessException(string message,ExceptionType exceptionType) :base(message)
        {
            ExceptionType = exceptionType;
            BusinessMessage = message;
        }
        
        public ExceptionType ExceptionType { get; set; }
        public string BusinessMessage { get; set; }
    }
    public enum ExceptionType
    {
        SelectedImage,
        InvalidFontSize,
        InvalidDirectory,
        InvalidFileDirectory,
    }
}
