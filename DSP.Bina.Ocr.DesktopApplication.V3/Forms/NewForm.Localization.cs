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
    public partial class NewForm
    {
        private ComboBox cb_uiLanguage;
        private Label l_uiLanguage;
        private bool refreshingUiLanguage;

        private void InitializeUiLanguageSelector()
        {
            l_uiLanguage = new Label
            {
                Name = "l_uiLanguage",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = l_version.Font,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = SystemColors.ControlText,
                Margin = new Padding(0, 0, 8, 0)
            };
            cb_uiLanguage = new ComboBox
            {
                Name = "cb_uiLanguage",
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Text",
                ValueMember = "Value",
                Width = 140,
                Font = l_version.Font,
                Anchor = AnchorStyles.Left,
                Margin = Padding.Empty,
                RightToLeft = RightToLeft.No,
                TabIndex = 0
            };
            cb_uiLanguage.Items.Add(new UiLanguageOption("en", Properties.Strings.UiLanguageEnglish));
            cb_uiLanguage.Items.Add(new UiLanguageOption("fa-IR", Properties.Strings.UiLanguagePersian));

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
            languagePanel.Controls.Add(l_uiLanguage, 0, 0);
            languagePanel.Controls.Add(cb_uiLanguage, 1, 0);
            p_footerUp.Controls.Add(languagePanel);
            languagePanel.SendToBack();

            cb_uiLanguage.SelectedIndex = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "fa" ? 1 : 0;
            cb_uiLanguage.SelectedIndexChanged += UiLanguage_SelectedIndexChanged;
            ApplyUiLanguage(((UiLanguageOption)cb_uiLanguage.SelectedItem).Value);
        }

        private void UiLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var option = cb_uiLanguage.SelectedItem as UiLanguageOption;
            if (option != null && !refreshingUiLanguage)
                ApplyUiLanguage(option.Value);
        }

        private void ApplyUiLanguage(string cultureName)
        {
            string status = l_processingText.Text;
            bool processing = status == Properties.Strings.Processing;
            bool saving = status == Properties.Strings.Saving;
            var processingImage = ImageEntities.FirstOrDefault(image =>
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
                l_uiLanguage.Text = Properties.Strings.UiLanguageLabel;
                cb_uiLanguage.AccessibleName = Properties.Strings.UiLanguageLabel;
                l_version.Text = string.Format(Properties.Strings.BinaVersionFormat, Application.ProductVersion);
                tp_TextDetection.Text = Properties.Strings.ConvertToTextTab;
                tp_ImageProcessing.Text = Properties.Strings.ImageProcessingTab;
                tp_help.Text = Properties.Strings.HelpTab;
                label20.Text = Properties.Strings.SaveAllTwoLines;
                label19.Text = Properties.Strings.ProcessingEngineLabel;
                label10.Text = Properties.Strings.Font;
                label9.Text = Properties.Strings.ProcessWithLineBreak;
                label8.Text = Properties.Strings.BatchProcessingTwoLines;
                label6.Text = Properties.Strings.Dictionary;
                cb_PostProcessing.Text = Properties.Strings.PostProcessing;
                cb_useDictionary.Text = Properties.Strings.UseDictionary;
                label5.Text = Properties.Strings.LanguageAndLayout;
                label4.Text = Properties.Strings.File;
                label3.Text = Properties.Strings.DocumentLayoutLabel;
                label2.Text = Properties.Strings.RecognitionLanguageLabel;
                label1.Text = Properties.Strings.SaveTwoLines;
                l_load.Text = Properties.Strings.Load;
                label18.Text = Properties.Strings.ImageSize;
                label17.Text = Properties.Strings.Zoom;
                label16.Text = Properties.Strings.Redo;
                label15.Text = Properties.Strings.Undo;
                b_threshold.Text = Properties.Strings.AdjustImageThreshold;
                b_contrast.Text = Properties.Strings.AdjustImageContrast;
                b_gama.Text = Properties.Strings.AdjustImageGamma;
                b_brightness.Text = Properties.Strings.AdjustImageBrightness;
                b_grayScale.Text = Properties.Strings.ConvertImageToGrayscale;
                b_monochrom.Text = Properties.Strings.ConvertImageToMonochrome;
                b_invertColor.Text = Properties.Strings.InvertImageColors;
                b_sharpen.Text = Properties.Strings.SharpenImage;
                b_smooth.Text = Properties.Strings.SmoothImage;
                label14.Text = Properties.Strings.AboutUs;
                label12.Text = Properties.Strings.Add;
                label11.Text = Properties.Strings.Remove;
                l_aboutUs.Text = Properties.Strings.AboutUsDescription;

                RefreshLocalizedOcrOptions();
                ApplyDirectionalLayout(culture.TextInfo.IsRightToLeft);
                foreach (var aboutUs in Application.OpenForms.OfType<Forms.AboutUs>()
                    .Concat(OwnedForms.OfType<Forms.AboutUs>()).Distinct().ToArray())
                    aboutUs.RefreshUiLanguage();

                if (processing)
                    l_processingText.Text = Properties.Strings.Processing;
                else if (saving)
                    l_processingText.Text = Properties.Strings.Saving;
                else if (processingImage != null)
                    l_processingText.Text = string.Format(Properties.Strings.ProcessingImageFormat, processingImage.Name);
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
            foreach (var option in pairPageSegmentationMode)
                option.Text = layouts[option.Value];
            foreach (var option in pairEngines)
                option.Text = engines[option.Value];
            foreach (var option in pairLanguages)
                option.Text = languages[option.Value];

            RefreshComboBoxText(cb_structure);
            RefreshComboBoxText(cb_engineMode);
            RefreshComboBoxText(cb_Language);
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
