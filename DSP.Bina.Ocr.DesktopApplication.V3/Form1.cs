using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void scrollablePictureBox1_MouseEnter(object sender, EventArgs e)
        {
            if (!this.scrollablePictureBox1.Focused && this.FindForm().ContainsFocus)
            {
                this.scrollablePictureBox1.Focus();
            }
        }
    }
}
