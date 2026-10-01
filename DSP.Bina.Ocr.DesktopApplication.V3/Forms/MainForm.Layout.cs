using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class MainForm
    {
        private readonly List<RtlControlPlacement> rtlControlPlacements = new List<RtlControlPlacement>();
        private Padding rtlSplitContainerMargin;

        private void InitializeDirectionalLayout()
        {
            // Capture the Persian designer layout before translating AutoSize labels.
            foreach (Control parent in new Control[] { headerPanel, recognitionToolbarPanel, imageProcessingTabPage, helpTabPage, imageListToolbarPanel })
                foreach (Control control in parent.Controls)
                    if (control.Dock == DockStyle.None)
                        rtlControlPlacements.Add(new RtlControlPlacement(control));
            rtlSplitContainerMargin = previewSplitContainer.Margin;
            previewSplitContainer.Panel2.SizeChanged += MainAboutUsMessage_SizeChanged;
        }

        private void ApplyDirectionalLayout(bool rightToLeft)
        {
            var direction = rightToLeft ? RightToLeft.Yes : RightToLeft.No;
            var parents = new Control[] { headerPanel, recognitionToolbarPanel, imageProcessingTabPage, helpTabPage, imageListToolbarPanel, bodyLayoutPanel, statusLayoutPanel, previewSplitContainer };
            foreach (Control parent in parents)
                parent.SuspendLayout();
            try
            {
                RightToLeftLayout = false;
                RightToLeft = direction;
                rootPanel.RightToLeft = direction;
                mainTabControl.RightToLeft = direction;
                mainTabControl.RightToLeftLayout = rightToLeft;
                bodyLayoutPanel.RightToLeft = direction;
                imageListLayoutPanel.RightToLeft = direction;
                previewSplitContainer.RightToLeft = direction;
                previewSplitContainer.Panel1.RightToLeft = direction;
                previewSplitContainer.Panel2.RightToLeft = direction;
                previewSplitContainer.Margin = rightToLeft ? rtlSplitContainerMargin : MirrorPadding(rtlSplitContainerMargin);
                footerContentPanel.RightToLeft = RightToLeft.No;
                statusLayoutPanel.RightToLeft = direction;
                processingProgressBar.RightToLeft = direction;
                processingProgressBar.RightToLeftLayout = rightToLeft;
                uiLanguageLabel.RightToLeft = direction;
                uiLanguageLabel.Margin = rightToLeft ? new Padding(8, 0, 0, 0) : new Padding(0, 0, 8, 0);
                uiLanguageComboBox.Parent.RightToLeft = direction;
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
            int padding = aboutDescriptionLabel.Font.Height;
            aboutDescriptionLabel.MaximumSize = new Size(System.Math.Max(1, previewSplitContainer.Panel2.ClientSize.Width - 2 * padding), 0);
            aboutDescriptionLabel.Left = System.Math.Max(padding, (previewSplitContainer.Panel2.ClientSize.Width - aboutDescriptionLabel.Width) / 2);
            aboutDescriptionLabel.Top = System.Math.Max(padding, (previewSplitContainer.Panel2.ClientSize.Height - aboutDescriptionLabel.Height) / 2);
        }

        private void ArrangeFileCaptions()
        {
            var buttons = new[] { loadFilesButton, saveButton, saveAllButton };
            var captions = new[] { loadCaptionLabel, saveCaptionLabel, saveAllCaptionLabel };
            int width = System.Math.Min(System.Math.Abs(loadFilesButton.Left - saveButton.Left),
                System.Math.Abs(saveButton.Left - saveAllButton.Left)) - 2;
            int top = System.Math.Max(loadFilesButton.Bottom, System.Math.Max(saveButton.Bottom, saveAllButton.Bottom)) + 2;
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
            var labels = new[] { recognitionLanguageLabel, documentLayoutLabel, processingEngineLabel };
            var comboBoxes = new[] { ocrLanguageComboBox, pageSegmentationComboBox, ocrEngineComboBox };
            var resourceKeys = new[] { "RecognitionLanguageLabel", "DocumentLayoutLabel", "ProcessingEngineLabel" };
            int gap = System.Math.Max(6, ocrLanguageComboBox.Font.Height / 2);
            int left = (rightToLeft ? ocrOptionsSeparator.Right : fileToolbarSeparator.Right) + gap;
            int right = (rightToLeft ? fileToolbarSeparator.Left : ocrOptionsSeparator.Left) - gap;
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
            int gap = System.Math.Max(6, addImageCaptionLabel.Font.Height / 2);
            if (rightToLeft)
            {
                addImageButton.Left = imageListToolbarPanel.ClientSize.Width - gap - addImageButton.Width;
                addImageCaptionLabel.Left = addImageButton.Left - gap - addImageCaptionLabel.Width;
                removeImageButton.Left = addImageCaptionLabel.Left - 2 * gap - removeImageButton.Width;
                removeImageCaptionLabel.Left = removeImageButton.Left - gap - removeImageCaptionLabel.Width;
            }
            else
            {
                addImageButton.Left = gap;
                addImageCaptionLabel.Left = addImageButton.Right + gap;
                removeImageButton.Left = addImageCaptionLabel.Right + 2 * gap;
                removeImageCaptionLabel.Left = removeImageButton.Right + gap;
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
