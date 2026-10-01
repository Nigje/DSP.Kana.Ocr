using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using System;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    static class Program
    {
        static MainForm mainForm = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += new ThreadExceptionEventHandler(HandleUiException);

            mainForm = new MainForm();
            Application.Run(mainForm);

        }
        private static void HandleUiException(object sender, ThreadExceptionEventArgs threadExceptionEventArgs)
        {
            mainForm.Cursor = Cursors.Default;
            Exception exception = threadExceptionEventArgs.Exception;
            mainForm.statusLabel.Text = "";
            if (exception.GetType() == typeof(BusinessException))
            {
                if (((BusinessException)exception).ExceptionType == ExceptionType.SelectedImage)
                {
                    MessageBox.Show(Properties.Strings.SelectImageFirst);
                }
                if (((BusinessException)exception).ExceptionType == ExceptionType.InvalidFontSize)
                {
                    MessageBox.Show(Properties.Strings.InvalidFontSize);
                }
                if (((BusinessException)exception).ExceptionType == ExceptionType.InvalidFileDirectory)
                {
                    MessageBox.Show(Properties.Strings.InvalidFilePath);
                }
                if (((BusinessException)exception).ExceptionType == ExceptionType.InvalidDirectory)
                {
                    MessageBox.Show(Properties.Strings.InvalidFolderPath);
                }
            }
            else if (exception.GetType() == typeof(FormatException))
            {
                MessageBox.Show(Properties.Strings.InvalidInputFormat);
            }
            else if (exception.Message.Contains("The process cannot access the file"))
            {
                MessageBox.Show(Properties.Strings.FileInUse);
            }
            else
            {
                MessageBox.Show(Properties.Strings.UnexpectedErrorContactSupport);
            }
            LogException(exception);

        }
        private static void LogException(Exception exception)
        {
            try
            {
                string directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DSP.Khana.Ocr", "Logs");
                Directory.CreateDirectory(directory);
                File.AppendAllText(Path.Combine(directory, "errors.log"),
                    DateTimeOffset.Now.ToString("O") + Environment.NewLine +
                    exception + Environment.NewLine + Environment.NewLine);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
