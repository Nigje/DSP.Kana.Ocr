using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Forms
{
    public partial class SelectPagesForm : TemplateForm
    {
        public SelectPagesForm(string fileName) : base(new Size(400, 200), string.Format(Properties.Strings.SelectFilePagesFormat, fileName))
        {
            InitializeComponent();
        }

        private void B_loadSelectedPages_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
