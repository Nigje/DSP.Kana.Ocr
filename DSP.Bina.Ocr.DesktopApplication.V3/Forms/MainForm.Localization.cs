using Bina.Ocr.Wapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class MainForm
    {
        private ComboBox uiLanguageComboBox;
        private Label uiLanguageLabel;
        private bool refreshingUiLanguage;

        private void InitializeUiLanguageSelector()
        {
            uiLanguageLabel = new Label
            {
                Name = "uiLanguageLabel",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = versionLabel.Font,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = SystemColors.ControlText,
                Margin = new Padding(0, 0, 8, 0)
            };
            uiLanguageComboBox = new ComboBox
            {
                Name = "uiLanguageComboBox",
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Text",
                ValueMember = "Value",
                Width = 140,
                Font = versionLabel.Font,
                Anchor = AnchorStyles.Left,
                Margin = Padding.Empty,
                RightToLeft = RightToLeft.No,
                TabIndex = 0
            };
            uiLanguageComboBox.Items.Add(new UiLanguageOption("en", Properties.Strings.UiLanguageEnglish));
            uiLanguageComboBox.Items.Add(new UiLanguageOption("fa-IR", Properties.Strings.UiLanguagePersian));

            var languagePanel = new TableLayoutPanel
            {
                Name = "p_uiLanguage",
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(8, 0, 8, 0),
                ColumnCount = 2,
                RowCount = 1,
                RightToLeft = RightToLeft.No,
            };
            languagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            languagePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            languagePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            languagePanel.Controls.Add(uiLanguageLabel, 0, 0);
            languagePanel.Controls.Add(uiLanguageComboBox, 1, 0);
            footerContentPanel.Controls.Add(languagePanel);
            languagePanel.SendToBack();

            uiLanguageComboBox.SelectedIndex = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "fa" ? 1 : 0;
            uiLanguageComboBox.SelectedIndexChanged += UiLanguageComboBox_SelectedIndexChanged;
            ApplyUiLanguage(((UiLanguageOption)uiLanguageComboBox.SelectedItem).Value);
        }

        private void UiLanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var option = uiLanguageComboBox.SelectedItem as UiLanguageOption;
            if (option != null && !refreshingUiLanguage)
                ApplyUiLanguage(option.Value);
        }

        private void ApplyUiLanguage(string cultureName)
        {
            string status = statusLabel.Text;
            bool processing = status == Properties.Strings.Processing;
            bool saving = status == Properties.Strings.Saving;
            bool canceling = status == Properties.Strings.CancelingProcessing;
            var processingImage = images.FirstOrDefault(image =>
                status == string.Format(Properties.Strings.ProcessingImageFormat, image.Name));

            var culture = CultureInfo.GetCultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Properties.Strings.Culture = culture;

            refreshingUiLanguage = true;
            SuspendLayout();
            try
            {
                Text = Properties.Strings.ApplicationTitle;
                if (cancelRecognitionButton != null) cancelRecognitionButton.Text = Properties.Strings.CancelProcessing;
                uiLanguageLabel.Text = Properties.Strings.UiLanguageLabel;
                uiLanguageComboBox.AccessibleName = Properties.Strings.UiLanguageLabel;
                versionLabel.Text = string.Format(Properties.Strings.BinaVersionFormat, Application.ProductVersion);
                recognitionTabPage.Text = Properties.Strings.ConvertToTextTab;
                imageProcessingTabPage.Text = Properties.Strings.ImageProcessingTab;
                helpTabPage.Text = Properties.Strings.HelpTab;
                saveAllCaptionLabel.Text = Properties.Strings.SaveAllTwoLines;
                processingEngineLabel.Text = Properties.Strings.ProcessingEngineLabel;
                fontGroupLabel.Text = Properties.Strings.Font;
                processingCaptionLabel.Text = Properties.Strings.ProcessWithLineBreak;
                batchProcessingCaptionLabel.Text = Properties.Strings.BatchProcessingTwoLines;
                dictionaryGroupLabel.Text = Properties.Strings.Dictionary;
                postProcessingCheckBox.Text = Properties.Strings.PostProcessing;
                useDictionaryCheckBox.Text = Properties.Strings.UseDictionary;
                languageAndLayoutGroupLabel.Text = Properties.Strings.LanguageAndLayout;
                fileGroupLabel.Text = Properties.Strings.File;
                documentLayoutLabel.Text = Properties.Strings.DocumentLayoutLabel;
                recognitionLanguageLabel.Text = Properties.Strings.RecognitionLanguageLabel;
                saveCaptionLabel.Text = Properties.Strings.SaveTwoLines;
                loadCaptionLabel.Text = Properties.Strings.Load;
                imageSizeGroupLabel.Text = Properties.Strings.ImageSize;
                zoomGroupLabel.Text = Properties.Strings.Zoom;
                redoImageCaptionLabel.Text = Properties.Strings.Redo;
                undoImageCaptionLabel.Text = Properties.Strings.Undo;
                thresholdButton.Text = Properties.Strings.AdjustImageThreshold;
                contrastButton.Text = Properties.Strings.AdjustImageContrast;
                gammaButton.Text = Properties.Strings.AdjustImageGamma;
                brightnessButton.Text = Properties.Strings.AdjustImageBrightness;
                grayscaleButton.Text = Properties.Strings.ConvertImageToGrayscale;
                monochromeButton.Text = Properties.Strings.ConvertImageToMonochrome;
                invertColorsButton.Text = Properties.Strings.InvertImageColors;
                sharpenButton.Text = Properties.Strings.SharpenImage;
                smoothButton.Text = Properties.Strings.SmoothImage;
                aboutUsCaptionLabel.Text = Properties.Strings.AboutUs;
                addImageCaptionLabel.Text = Properties.Strings.Add;
                removeImageCaptionLabel.Text = Properties.Strings.Remove;
                aboutDescriptionLabel.Text = Properties.Strings.AboutUsDescription;

                RefreshLocalizedOcrOptions();
                ApplyDirectionalLayout(culture.TextInfo.IsRightToLeft);
                foreach (var aboutUs in Application.OpenForms.OfType<Forms.AboutUsForm>()
                    .Concat(OwnedForms.OfType<Forms.AboutUsForm>()).Distinct().ToArray())
                    aboutUs.RefreshUiLanguage();

                if (canceling)
                    statusLabel.Text = Properties.Strings.CancelingProcessing;
                else if (processing)
                    statusLabel.Text = Properties.Strings.Processing;
                else if (saving)
                    statusLabel.Text = Properties.Strings.Saving;
                else if (processingImage != null)
                    statusLabel.Text = string.Format(Properties.Strings.ProcessingImageFormat, processingImage.Name);
            }
            finally
            {
                ResumeLayout(true);
                refreshingUiLanguage = false;
            }
        }

        private void RefreshLocalizedOcrOptions()
        {
            var layouts = new Dictionary<PageSegmentationModeEnum, string>
            {
                { PageSegmentationModeEnum.AutoOsd, Properties.Strings.LayoutAutomaticWithOrientationAndScriptDetection },
                { PageSegmentationModeEnum.Auto, Properties.Strings.LayoutWithoutOrientationAndScriptDetection },
                { PageSegmentationModeEnum.SingleColumn, Properties.Strings.LayoutSingleColumn },
                { PageSegmentationModeEnum.SingleBlockVertText, Properties.Strings.LayoutSingleVerticalTextBlock },
                { PageSegmentationModeEnum.SingleBlock, Properties.Strings.LayoutSingleTextBlock },
                { PageSegmentationModeEnum.SingleLine, Properties.Strings.LayoutSingleLine },
                { PageSegmentationModeEnum.CircleWord, Properties.Strings.LayoutCircularWord },
                { PageSegmentationModeEnum.SingleChar, Properties.Strings.LayoutSingleCharacter },
                { PageSegmentationModeEnum.SparseText, Properties.Strings.LayoutSparseText },
                { PageSegmentationModeEnum.SparseTextOsd, Properties.Strings.LayoutSparseTextWithOrientationAndScriptDetection },
                { PageSegmentationModeEnum.RawLine, Properties.Strings.LayoutRawLine },
                { PageSegmentationModeEnum.Count, Properties.Strings.LayoutMultipleModes }
            };
            var engines = new Dictionary<EngineModeEnum, string>
            {
                { EngineModeEnum.KhanaDeepOnly, Properties.Strings.EngineDeep },
                { EngineModeEnum.KhanaStructuralAndKhanaDeep, Properties.Strings.EngineStructuralAndDeep },
                { EngineModeEnum.KhanaStructuralOnly, Properties.Strings.EngineStructural }
            };
            var languages = new Dictionary<LanguageEnum, string>
            {
                { LanguageEnum.Farsi, Properties.Strings.LanguagePersian },
                { LanguageEnum.English, Properties.Strings.LanguageEnglish },
                { LanguageEnum.Mix, Properties.Strings.LanguageMixed }
            };
            foreach (var option in pageSegmentationOptions)
                option.Text = layouts[option.Value];
            foreach (var option in ocrEngineOptions)
                option.Text = engines[option.Value];
            foreach (var option in ocrLanguageOptions)
                option.Text = languages[option.Value];

            RefreshComboBoxText(pageSegmentationComboBox);
            RefreshComboBoxText(ocrEngineComboBox);
            RefreshComboBoxText(ocrLanguageComboBox);
        }

        private static void RefreshComboBoxText(ComboBox comboBox)
        {
            var selectedItem = comboBox.SelectedItem;
            var items = comboBox.Items.Cast<object>().ToArray();
            comboBox.BeginUpdate();
            try
            {
                comboBox.Items.Clear();
                comboBox.Items.AddRange(items);
                comboBox.SelectedItem = selectedItem;
            }
            finally
            {
                comboBox.EndUpdate();
            }
        }

        [System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true)]
        private sealed class UiLanguageOption
        {
            public UiLanguageOption(string value, string text)
            {
                Value = value;
                Text = text;
            }

            public string Value { get; private set; }
            public string Text { get; private set; }
        }
    }
}
