using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using System;
using System.IO;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Services
{
    public static class ApplicationErrorService
    {
        public static string GetMessage(Exception error) => error switch
        {
            BusinessException validation => validation.ExceptionType switch
            {
                ExceptionType.ProcessingInProgress => Properties.Strings.ProcessingAlreadyRunning,
                ExceptionType.SelectedImage => Properties.Strings.SelectImageFirst,
                ExceptionType.InvalidFontSize => Properties.Strings.InvalidFontSize,
                ExceptionType.InvalidDirectory => Properties.Strings.InvalidFolderPath,
                ExceptionType.InvalidFileDirectory => Properties.Strings.InvalidFilePath,
                _ => Properties.Strings.InvalidInputFormat
            },
            FormatException => Properties.Strings.InvalidInputFormat,
            FileNotFoundException or DirectoryNotFoundException => Properties.Strings.InvalidFilePath,
            UnauthorizedAccessException => Properties.Strings.AccessDenied,
            IOException io when (io.HResult & 0xffff) is 32 or 33 => Properties.Strings.FileInUse,
            IOException => Properties.Strings.FileOperationFailed,
            OperationCanceledException => Properties.Strings.ProcessingCanceled,
            _ => Properties.Strings.UnexpectedErrorContactSupport
        };

        public static void Show(Exception error, IWin32Window owner = null)
        {
            Log(error);
            MessageBox.Show(owner, GetMessage(error), Properties.Strings.ApplicationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Log(Exception error)
        {
            try
            {
                string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DSP.Khana.Ocr", "Logs");
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "errors.log"), DateTimeOffset.Now.ToString("O") + Environment.NewLine + error + Environment.NewLine + Environment.NewLine);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
