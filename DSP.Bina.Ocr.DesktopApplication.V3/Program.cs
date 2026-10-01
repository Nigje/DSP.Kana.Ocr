using DSP.Bina.Ocr.DesktopApplication.V3.Services;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    internal static class Program
    {
        private static MainForm mainForm;

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += HandleUiException;
            try
            {
                using (mainForm = new MainForm()) Application.Run(mainForm);
            }
            catch (Exception error) { ApplicationErrorService.Show(error); }
            finally { mainForm = null; }
        }

        private static void HandleUiException(object sender, ThreadExceptionEventArgs args)
        {
            if (mainForm != null && !mainForm.IsDisposed)
            {
                mainForm.Cursor = Cursors.Default;
                mainForm.statusLabel.Text = "";
                ApplicationErrorService.Show(args.Exception, mainForm);
            }
            else ApplicationErrorService.Show(args.Exception);
        }
    }
}
