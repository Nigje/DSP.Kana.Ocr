using Bina.Ocr.Wapper;
using DSP.Bina.Ocr.DesktopApplication.V3.Forms;
using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    static class Program
    {
        static NewForm newForm = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += new ThreadExceptionEventHandler(MyCommonExceptionHandlingMethod);

            newForm = new NewForm();
            AppCenter.Start("1065278b-fb89-49f4-9cf1-5f84812ff8a6",
                   typeof(Analytics), typeof(Crashes));

            Application.Run(newForm);
            //Application.Run(new AboutUs());

        }
        private static void MyCommonExceptionHandlingMethod(object sender, ThreadExceptionEventArgs threadExceptionEventArgs)
        {
            newForm.Cursor = Cursors.Default;
            Exception exception = threadExceptionEventArgs.Exception;
            newForm.l_processingText.Text = "";
            //newForm.rt_main.Text = exception.Message;
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
                //if(exception.Message.Contains("The input is not a valid Base-64 string as it"))
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
            //var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //string version1 = System.Windows.Forms.Application.ProductVersion;
            LogCrashes(exception);

        }
        private static void LogCrashes(Exception exception)
        {
            try
            {
                string appName = "";
                string userName = "";
                try
                {
                    appName = BinaOcr.GetApplicationName();
                    userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
                catch
                { }
                Crashes.TrackError(exception, new Dictionary<string, string> { { "AppName", appName }, { "UserName", userName } });
            }
            catch
            { }

        }
    }
}
