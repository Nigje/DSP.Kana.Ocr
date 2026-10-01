using System;
using System.Drawing;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Forms
{
    public partial class PdfPageSelectionForm : BaseDialogForm
    {
        public PdfPageSelectionForm(string fileName) : base(new Size(400, 200), string.Format(Properties.Strings.SelectFilePagesFormat, fileName))
        {
            InitializeComponent();
        }

        private void LoadSelectedPagesButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
