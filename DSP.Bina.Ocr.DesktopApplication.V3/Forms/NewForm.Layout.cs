using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class NewForm
    {
        private readonly List<RtlControlPlacement> rtlControlPlacements = new List<RtlControlPlacement>();
        private Padding rtlSplitContainerMargin;

        private void InitializeDirectionalLayout()
        {
            // Capture the Persian designer layout before translating AutoSize labels.
            foreach (Control parent in new Control[] { p_header, panel3, tp_ImageProcessing, tp_help, panel5 })
                foreach (Control control in parent.Controls)
                    if (control.Dock == DockStyle.None)
                        rtlControlPlacements.Add(new RtlControlPlacement(control));
            rtlSplitContainerMargin = sc_main.Margin;
            sc_main.Panel2.SizeChanged += MainAboutUsMessage_SizeChanged;
        }

        private void ApplyDirectionalLayout(bool rightToLeft)
        {
            var direction = rightToLeft ? RightToLeft.Yes : RightToLeft.No;
            var parents = new Control[] { p_header, panel3, tp_ImageProcessing, tp_help, panel5, tlp_body, tlp_footerFront, sc_main };
            foreach (Control parent in parents)
                parent.SuspendLayout();
            try
            {
                // Absolute panels are mirrored below; native form mirroring would mirror them twice.
                RightToLeftLayout = false;
                RightToLeft = direction;
                p_base.RightToLeft = direction;
                tc_mainTabs.RightToLeft = direction;
                tc_mainTabs.RightToLeftLayout = rightToLeft;
                tlp_body.RightToLeft = direction;
                tlp_pictureList.RightToLeft = direction;
                sc_main.RightToLeft = direction;
                sc_main.Panel1.RightToLeft = direction;
                sc_main.Panel2.RightToLeft = direction;
                sc_main.Margin = rightToLeft ? rtlSplitContainerMargin : MirrorPadding(rtlSplitContainerMargin);
                // The selector stays on the physical left; the remaining footer follows the UI language.
                p_footerUp.RightToLeft = RightToLeft.No;
                tlp_footerFront.RightToLeft = direction;
                pb_main.RightToLeft = direction;
                pb_main.RightToLeftLayout = rightToLeft;
                l_uiLanguage.RightToLeft = direction;
                l_uiLanguage.Margin = rightToLeft ? new Padding(8, 0, 0, 0) : new Padding(0, 0, 8, 0);
                cb_uiLanguage.Parent.RightToLeft = direction;
                foreach (var placement in rtlControlPlacements)
                    placement.Apply(rightToLeft);
                ArrangeOcrOptionRows(rightToLeft);
                ArrangeImageListActions(rightToLeft);
                ArrangeFileCaptions();
                ArrangeAboutUsMessage();
            }
            finally
            {
                for (int index = parents.Length - 1; index >= 0; index--)
                    parents[index].ResumeLayout(true);
            }
        }

        private static Padding MirrorPadding(Padding padding)
        {
            return new Padding(padding.Right, padding.Top, padding.Left, padding.Bottom);
        }

        private void MainAboutUsMessage_SizeChanged(object sender, System.EventArgs e)
        {
            ArrangeAboutUsMessage();
        }

        private void ArrangeAboutUsMessage()
        {
            int padding = l_aboutUs.Font.Height;
            l_aboutUs.MaximumSize = new Size(System.Math.Max(1, sc_main.Panel2.ClientSize.Width - 2 * padding), 0);
            l_aboutUs.Left = System.Math.Max(padding, (sc_main.Panel2.ClientSize.Width - l_aboutUs.Width) / 2);
            l_aboutUs.Top = System.Math.Max(padding, (sc_main.Panel2.ClientSize.Height - l_aboutUs.Height) / 2);
        }

        private void ArrangeFileCaptions()
        {
            var buttons = new[] { btn_loadFiles, b_save, b_saveAll };
            var captions = new[] { l_load, label1, label20 };
            int width = System.Math.Min(System.Math.Abs(btn_loadFiles.Left - b_save.Left),
                System.Math.Abs(b_save.Left - b_saveAll.Left)) - 2;
            int top = System.Math.Max(btn_loadFiles.Bottom, System.Math.Max(b_save.Bottom, b_saveAll.Bottom)) + 2;
            for (int index = 0; index < captions.Length; index++)
            {
                var caption = captions[index];
                caption.AutoSize = false;
                caption.TextAlign = ContentAlignment.TopCenter;
                caption.SetBounds(buttons[index].Left + (buttons[index].Width - width) / 2,
                    top, width, caption.Font.Height * 2 + 4);
            }
        }

        private void ArrangeOcrOptionRows(bool rightToLeft)
        {
            var labels = new[] { label2, label3, label19 };
            var comboBoxes = new[] { cb_Language, cb_structure, cb_engineMode };
            var resourceKeys = new[] { "RecognitionLanguageLabel", "DocumentLayoutLabel", "ProcessingEngineLabel" };
            int gap = System.Math.Max(6, cb_Language.Font.Height / 2);
            int left = (rightToLeft ? pictureBox2.Right : pb.Right) + gap;
            int right = (rightToLeft ? pb.Left : pictureBox2.Left) - gap;
            int labelWidth = 0;
            for (int index = 0; index < labels.Length; index++)
            {
                foreach (string cultureName in new[] { "en", "fa-IR" })
                {
                    string caption = Properties.Strings.ResourceManager.GetString(resourceKeys[index], CultureInfo.GetCultureInfo(cultureName));
                    labelWidth = System.Math.Max(labelWidth, TextRenderer.MeasureText(caption, labels[index].Font).Width);
                }
            }
            labelWidth = System.Math.Min(labelWidth, System.Math.Max(0, (right - left - gap) / 2));
            for (int index = 0; index < labels.Length; index++)
            {
                var label = labels[index];
                var comboBox = comboBoxes[index];
                label.AutoSize = false;
                label.Width = labelWidth;
                label.TextAlign = ContentAlignment.MiddleLeft;
                label.Left = rightToLeft ? right - labelWidth : left;
                comboBox.Width = System.Math.Max(0, right - left - labelWidth - gap);
                comboBox.Left = rightToLeft ? left : label.Right + gap;
            }
        }

        private void ArrangeImageListActions(bool rightToLeft)
        {
            int gap = System.Math.Max(6, label12.Font.Height / 2);
            if (rightToLeft)
            {
                b_addPicture.Left = panel5.ClientSize.Width - gap - b_addPicture.Width;
                label12.Left = b_addPicture.Left - gap - label12.Width;
                b_removePicture.Left = label12.Left - 2 * gap - b_removePicture.Width;
                label11.Left = b_removePicture.Left - gap - label11.Width;
            }
            else
            {
                b_addPicture.Left = gap;
                label12.Left = b_addPicture.Right + gap;
                b_removePicture.Left = label12.Right + 2 * gap;
                label11.Left = b_removePicture.Right + gap;
            }
        }

        private sealed class RtlControlPlacement
        {
            private readonly Control control;
            private readonly AnchorStyles rtlAnchor;
            private readonly int rtlLeft;
            private readonly int rtlRight;
            private readonly Padding rtlMargin;

            public RtlControlPlacement(Control control)
            {
                this.control = control;
                rtlAnchor = control.Anchor;
                rtlLeft = control.Left;
                rtlRight = control.Parent.ClientSize.Width - control.Right;
                rtlMargin = control.Margin;
            }

            public void Apply(bool rightToLeft)
            {
                int parentWidth = control.Parent.ClientSize.Width;
                bool anchoredLeft = (rtlAnchor & AnchorStyles.Left) != 0;
                bool anchoredRight = (rtlAnchor & AnchorStyles.Right) != 0;
                var anchor = rtlAnchor & ~(AnchorStyles.Left | AnchorStyles.Right);
                if (anchoredLeft)
                    anchor |= rightToLeft ? AnchorStyles.Left : AnchorStyles.Right;
                if (anchoredRight)
                    anchor |= rightToLeft ? AnchorStyles.Right : AnchorStyles.Left;
                int width = control.Width;
                if (anchoredLeft && anchoredRight)
                    width = System.Math.Max(0, parentWidth - rtlLeft - rtlRight);
                int left;
                if (anchoredRight)
                    left = rightToLeft ? parentWidth - rtlRight - width : rtlRight;
                else
                    left = rightToLeft ? rtlLeft : parentWidth - rtlLeft - width;
                control.Anchor = anchor;
                control.Margin = rightToLeft ? rtlMargin : MirrorPadding(rtlMargin);
                control.SetBounds(left, control.Top, width, control.Height);
            }
        }
    }
}
