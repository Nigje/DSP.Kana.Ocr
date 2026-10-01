using System.Drawing;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Forms
{
    public partial class AboutUsForm : BaseDialogForm
    {
        public AboutUsForm() : base(new Size(1280, 720), Properties.Strings.AboutUs)
        {
            InitializeComponent();
            RefreshUiLanguage();
        }

        internal void RefreshUiLanguage()
        {
            var culture = Properties.Strings.Culture ?? System.Threading.Thread.CurrentThread.CurrentUICulture;
            var direction = culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            Text = Properties.Strings.AboutUs;
            titleLabel.Text = Properties.Strings.AboutUs;
            RightToLeft = direction;
            RightToLeftLayout = culture.TextInfo.IsRightToLeft;
            descriptionTextBox.RightToLeft = direction;
            descriptionTextBox.ReadOnly = true;
            descriptionTextBox.WordWrap = true;
            descriptionTextBox.Text = Properties.Strings.AboutUsDescription;
            descriptionTextBox.SelectAll();
            descriptionTextBox.SelectionAlignment = culture.TextInfo.IsRightToLeft ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            descriptionTextBox.Select(0, 0);
        }
    }
}
