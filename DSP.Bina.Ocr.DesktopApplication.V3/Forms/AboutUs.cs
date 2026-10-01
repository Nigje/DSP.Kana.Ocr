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
    public partial class AboutUs : TemplateForm
    {
        public AboutUs():base(new Size(1280, 720),Properties.Strings.AboutUs)
        {
            InitializeComponent();
            RefreshUiLanguage();
        }

        internal void RefreshUiLanguage()
        {
            var culture = Properties.Strings.Culture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
            var direction = culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            Text = Properties.Strings.AboutUs;
            l_title.Text = Properties.Strings.AboutUs;
            RightToLeft = direction;
            RightToLeftLayout = culture.TextInfo.IsRightToLeft;
            rtb_AboutUs.RightToLeft = direction;
            rtb_AboutUs.ReadOnly = true;
            rtb_AboutUs.WordWrap = true;
            rtb_AboutUs.Text = Properties.Strings.AboutUsDescription;
            rtb_AboutUs.SelectAll();
            rtb_AboutUs.SelectionAlignment = culture.TextInfo.IsRightToLeft ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            rtb_AboutUs.Select(0, 0);
        }
    }
}
