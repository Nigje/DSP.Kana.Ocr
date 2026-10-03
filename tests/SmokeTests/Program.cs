using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Bina.Ocr.Wapper;
using DSP.Bina.Ocr.DesktopApplication.V3;
using DSP.Bina.Ocr.DesktopApplication.V3.Forms;
using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using DSP.Khana.ImageTools.Models;
using Tesseract;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using DSP.Bina.Ocr.DesktopApplication.V3.Services;
using W = DocumentFormat.OpenXml.Wordprocessing;

internal static partial class SmokeTests
{
    private static readonly string Work = Path.Combine(Path.GetTempPath(), "DSP.Khana.Ocr.SmokeTests", Guid.NewGuid().ToString("N"));
    private static readonly BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            Directory.CreateDirectory(Work);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Contains("--forms-only")) { CheckForms(); Console.WriteLine("PASS: bilingual form checks."); return 0; }
            if (args.Contains("--refactoring-only")) { CheckRefactorings(); Console.WriteLine("PASS: desktop refactoring checks."); return 0; }
            CheckResources();
            CheckDocx();
            CheckOcr();
            CheckImageHistory();
            CheckForms();
            CheckRefactorings();
            CheckWrapper();
            CheckPdf();
            Console.WriteLine("PASS: .NET 10 resources, Open XML DOCX export, native OCR, PDF import, and bilingual forms.");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
        finally
        {
            // Work is a fresh, unique child of the smoke-test temporary directory.
            try
            {
                if (Directory.Exists(Work)) Directory.Delete(Work, true);
            }
            catch (IOException) { } // Windows retains loaded native libraries until process exit.
            catch (UnauthorizedAccessException) { }
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void CheckResources()
    {
        var assembly = typeof(MainForm).Assembly;
        var type = assembly.GetType("DSP.Bina.Ocr.DesktopApplication.V3.Properties.Strings", true);
        var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var manager = (System.Resources.ResourceManager)type.GetProperty("ResourceManager", flags).GetValue(null);
        var workspace = new DirectoryInfo(AppContext.BaseDirectory);
        while (workspace != null && !File.Exists(Path.Combine(workspace.FullName, "DSP.Khana.Ocr.sln"))) workspace = workspace.Parent;
        Assert(workspace != null, "Find the solution for resource-source checks.");
        string project = Path.Combine(workspace.FullName, "DSP.Bina.Ocr.DesktopApplication.V3");
        int count = 0;
        foreach (string language in new[] { "en", "en-US", "fa-IR" })
        {
            var culture = CultureInfo.GetCultureInfo(language);
            type.GetProperty("Culture", flags).SetValue(null, culture);
            string resourceFile = language == "fa-IR" ? "Strings.fa-IR.resx" : "Strings.resx";
            foreach (var entry in XDocument.Load(Path.Combine(project, "Properties", resourceFile)).Root.Elements("data"))
            {
                string key = (string)entry.Attribute("name");
                string expected = entry.Element("value").Value;
                Assert(manager.GetString(key, culture) == expected, language + " resource " + key);
                Assert((string)type.GetProperty(key, flags).GetValue(null) == expected, language + " typed property " + key);
                count++;
            }
        }
        Console.WriteLine("Resources: " + count + " lookups passed.");
    }

    private static void CheckDocx()
    {
        var snapshot = new DocumentSnapshot();
        var paragraph = new ParagraphSnapshot { RightToLeft = true };
        paragraph.Runs.Add(new TextRunSnapshot("Hello / سلام", "Tahoma", 12, FontStyle.Regular, Color.Black));
        snapshot.Paragraphs.Add(paragraph);
        using var image = new Bitmap(10, 10);
        string exported = new DocumentExportService().Export(Path.Combine(Work, "export"), image, snapshot);
        using (var archive = ZipFile.OpenRead(exported + ".docx"))
        using (var reader = new StreamReader(archive.GetEntry("word/document.xml").Open()))
            Assert(reader.ReadToEnd().Contains("سلام"), "DOCX export preserves Persian text.");
        using (var document = WordprocessingDocument.Open(exported + ".docx", false))
        {
            Assert(document.MainDocumentPart.Document.Body.InnerText.Contains("Hello / سلام"), "DOCX round-trip.");
            AssertValidDocx(document);
        }
        Assert(!typeof(MainForm).Assembly.GetReferencedAssemblies().Any(a => a.Name.StartsWith("Xceed", StringComparison.Ordinal)),
            "Desktop no longer references Xceed assemblies.");
        Console.WriteLine("DOCX: production bilingual export, SDK round-trip, and schema validation passed.");
    }

    private static void AssertValidDocx(WordprocessingDocument document)
    {
        var errors = new OpenXmlValidator(FileFormatVersions.Office2010).Validate(document).Take(10).ToArray();
        Assert(errors.Length == 0, "DOCX schema validation: " + string.Join("; ", errors.Select(e => e.Description)));
    }

    private static void Extract(string resource, string destination)
    {
        var assembly = typeof(BinaOcr).Assembly;
        string name = assembly.GetManifestResourceNames().Single(n => n.EndsWith("." + resource, StringComparison.Ordinal));
        using (var input = assembly.GetManifestResourceStream(name))
        using (var output = File.Create(destination)) input.CopyTo(output);
    }

    private static void CheckOcr()
    {
        var wrapper = typeof(BinaOcr).Assembly;
        Assert(wrapper.GetType("Bina.Ocr.Wapper.LicenseManager") == null, "No desktop license manager.");
        Assert(!wrapper.GetReferencedAssemblies().Any(a => new[] { "DSP.License", "LicenseModel", "FingerPrint" }.Contains(a.Name)), "No licensing assembly dependencies.");
        string native = Path.Combine(Work, "native");
        string models = Path.Combine(Work, "models");
        Directory.CreateDirectory(native);
        Extract(Environment.Is64BitProcess ? "BinaOcrEngineV5_64.dll" : "BinaOcrEngineV5_86.dll", Path.Combine(native, "BinaOcrEngineV5.dll"));
        Extract(Environment.Is64BitProcess ? "liblept1780_64.dll" : "liblept1780_86.dll", Path.Combine(native, "liblept1780.dll"));
        string data = Path.Combine(Work, "Data.zip");
        Extract("Data.zip", data);
        ZipFile.ExtractToDirectory(data, models);
        using (var image = new Bitmap(900, 180))
        {
            using (var graphics = Graphics.FromImage(image))
            using (var font = new Font("Arial", 40))
            {
                graphics.Clear(Color.White);
                graphics.DrawString("HELLO 123", font, Brushes.Black, new PointF(25, 40));
            }
            foreach (string language in new[] { "English", "Farsi", "Mix" })
            {
                using (var engine = new TesseractEngine(null, models, language, EngineMode.LstmOnly, native))
                using (var page = engine.Process(image, PageSegMode.SingleLine))
                {
                    string text = page.GetText();
                    if (language == "English") Assert(text.ToUpperInvariant().Contains("HELLO"), "Native English OCR result.");
                    Console.WriteLine(language + " OCR: " + text.Trim());
                }
            }
        }
    }

    private static T Field<T>(object owner, string name) => (T)owner.GetType().GetField(name, PrivateInstance).GetValue(owner);

    private static void CheckImageHistory()
    {
        using var original = new Bitmap(2, 2);
        using var firstEdit = new Bitmap(2, 2);
        using var secondEdit = new Bitmap(2, 2);
        original.SetPixel(0, 0, Color.Red);
        firstEdit.SetPixel(0, 0, Color.Blue);
        secondEdit.SetPixel(0, 0, Color.Green);
        using var image = new ImageEntity(original) { Name = "image" };
        Assert(image.NameWithoutExtension == "image", "Extensionless image name.");
        image.Name = "scan.page.png";
        Assert(image.NameWithoutExtension == "scan.page", "Only the final extension is removed.");
        image.SetImage(firstEdit);
        Assert(((Bitmap)image.Undo()).GetPixel(0, 0).ToArgb() == Color.Red.ToArgb(), "Undo restores the original image.");
        Assert(((Bitmap)image.Redo()).GetPixel(0, 0).ToArgb() == Color.Blue.ToArgb(), "Redo restores the edit.");
        image.Undo();
        image.SetImage(secondEdit);
        Assert(((Bitmap)image.Redo()).GetPixel(0, 0).ToArgb() == Color.Green.ToArgb(), "A new edit discards stale redo history.");
        var history = new FixedSizeStack<int>(2);
        history.Push(1); history.Push(2); history.Push(3);
        Assert(history.Count == 2 && history.Pop() == 3 && history.Pop() == 2, "History capacity discards the oldest entry.");
        Console.WriteLine("Image history: undo, redo invalidation, bounded history, and filename handling passed.");
    }

    [DllImport("user32.dll", EntryPoint = "GetPropW", CharSet = CharSet.Unicode)]
    private static extern IntPtr GetWindowProperty(IntPtr handle, string name);

    private static void CheckForms()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        using (var main = new MainForm())
        using (var about = new AboutUsForm())
        {
            main.ShowInTaskbar = false;
            main.Opacity = 0;
            int loadCount = 0;
            main.Load += (_, _) => loadCount++;
            main.Show();
            about.ShowInTaskbar = false;
            about.Opacity = 0;
            about.Show(main);
            var imageList = Field<Manina.Windows.Forms.ImageListView>(main, "imageListView");
            var imageListPanel = Field<TableLayoutPanel>(main, "imageListLayoutPanel");
            Assert(imageList != null && imageList.Parent == imageListPanel, "Startup creates and mounts the image list.");
            Assert(imageListPanel.GetPositionFromControl(imageList) == new TableLayoutPanelCellPosition(0, 1), "Image list occupies the thumbnail row.");
            Assert(imageList.ThumbnailSize == new Size(150, 150), "Image list thumbnail configuration.");
            using var sampleImage = new Bitmap(300, 100);
            var sample = new ImageEntity(sampleImage) { Name = "startup-test.png" };
            typeof(MainForm).GetMethod("AddImage", PrivateInstance).Invoke(main, new object[] { sample });
            Assert(imageList.Items.Count == 1, "Image import reaches the initialized image list.");
            imageList.Items[0].Selected = true;
            Application.DoEvents();
            Assert(Field<Guid>(main, "selectedImageId") == sample.Id, "Image list selection handler is wired.");
            var selector = Field<ComboBox>(main, "uiLanguageComboBox");
            var editor = Field<RichTextBox>(about, "descriptionTextBox");
            var recognitionEditor = Field<RichTextBox>(main, "recognizedTextBox");
            recognitionEditor.Text = "English and فارسی edits";
            recognitionEditor.Select(0, 7);
            using (var bold = new Font(recognitionEditor.Font, FontStyle.Bold)) recognitionEditor.SelectionFont = bold;
            recognitionEditor.Select(3, 2);
            string documentText = recognitionEditor.Text;
            var mainHandle = main.Handle;
            int recreatedHandles = 0;
            main.HandleCreated += (_, _) => recreatedHandles++;
            var ocrLanguage = Field<ComboBox>(main, "ocrLanguageComboBox");
            var ocrEngine = Field<ComboBox>(main, "ocrEngineComboBox");
            var segmentation = Field<ComboBox>(main, "pageSegmentationComboBox");
            var selectedLanguage = ocrLanguage.SelectedItem;
            var selectedEngine = ocrEngine.SelectedItem;
            var selectedSegmentation = segmentation.SelectedItem;
            int redrawChecks = 0;
            foreach (var containerName in new[] { "rootPanel", "headerPanel", "recognitionToolbarPanel", "previewSplitContainer" })
                Field<Control>(main, containerName).Layout += (_, _) =>
                {
                    if (!Field<bool>(main, "refreshingUiLanguage")) return;
                    redrawChecks++;
                    Assert(GetWindowProperty(main.Handle, "SysSetRedraw") != IntPtr.Zero,
                        "Language-switch layout runs while native window redraw is disabled.");
                };
            foreach (int selection in new[] { 0, 1, 0, 1 })
            {
                selector.SelectedIndex = selection;
                Application.DoEvents();
                Assert(GetWindowProperty(main.Handle, "SysSetRedraw") == IntPtr.Zero && main.Visible,
                    "Language switching restores native redraw and keeps the form visible.");
                bool persian = selection == 1;
                Assert(main.Handle == mainHandle && recreatedHandles == 0, "Switching UI language keeps the main window handle alive.");
                Assert(loadCount == 1, "Switching UI language does not reload the form.");
                Assert(Field<Panel>(main, "rootPanel").RightToLeft == (persian ? RightToLeft.Yes : RightToLeft.No), "Main content direction.");
                Assert(about.RightToLeft == (persian ? RightToLeft.Yes : RightToLeft.No), "Live About Us direction.");
                Assert(recognitionEditor.Text == documentText && recognitionEditor.SelectionStart == 3 && recognitionEditor.SelectionLength == 2
                    && recognitionEditor.RightToLeft == RightToLeft.Yes, "UI language switching preserves editor text, direction and selection.");
                recognitionEditor.Select(0, 7);
                using (var selectedFont = recognitionEditor.SelectionFont) Assert(selectedFont.Bold, "Language switching preserves bold formatting.");
                recognitionEditor.Select(8, 3);
                using (var selectedFont = recognitionEditor.SelectionFont) Assert(!selectedFont.Bold, "Language switching preserves plain formatting.");
                recognitionEditor.Select(3, 2);
                Assert(Field<Guid>(main, "selectedImageId") == sample.Id && ReferenceEquals(ocrLanguage.SelectedItem, selectedLanguage)
                    && ReferenceEquals(ocrEngine.SelectedItem, selectedEngine) && ReferenceEquals(segmentation.SelectedItem, selectedSegmentation),
                    "UI language switching preserves the selected image and OCR options.");
                Assert(editor.Text.Contains(persian ? "بینا" : "Bina"), "Live About Us translation.");
                var languagePanel = selector.Parent;
                Assert(languagePanel.Dock == DockStyle.Left, "Footer selector stays left.");
                foreach (var size in new[] { new Size(1280, 720), new Size(1600, 900) })
                {
                    main.Size = size;
                    Application.DoEvents();
                    Assert(selector.Width > 0 && selector.Height > 0, "Language selector layout after resize.");
                }
            }
            Assert(redrawChecks > 0, "The redraw regression observes language-switch layouts.");
            EventHandler failTranslation = (_, _) => throw new InvalidOperationException("Injected translation failure");
            var tab = Field<TabPage>(main, "recognitionTabPage");
            tab.TextChanged += failTranslation;
            bool updateFailed = false;
            try { selector.SelectedIndex = 0; }
            catch (InvalidOperationException error) { updateFailed = error.Message == "Injected translation failure"; }
            finally { tab.TextChanged -= failTranslation; }
            Assert(updateFailed && GetWindowProperty(main.Handle, "SysSetRedraw") == IntPtr.Zero && !Field<bool>(main, "refreshingUiLanguage"),
                "Failed translation restores redraw and releases the language-switch guard.");
            selector.SelectedIndex = 1;
            Application.DoEvents();
            Assert(main.Visible && main.Handle == mainHandle, "Language switching recovers after a failed translation.");
            var setFontSize = typeof(MainForm).GetMethod("SetFontSize", PrivateInstance);
            foreach (float invalidSize in new[] { 0f, -1f, float.NaN, float.PositiveInfinity })
            {
                bool rejected = false;
                try { setFontSize.Invoke(main, new object[] { invalidSize }); }
                catch (TargetInvocationException error)
                {
                    rejected = error.InnerException is BusinessException validation && validation.ExceptionType == ExceptionType.InvalidFontSize;
                }
                Assert(rejected, "Invalid font size produces the localized validation error.");
            }
            setFontSize.Invoke(main, new object[] { 16f });
            Assert(Field<RichTextBox>(main, "recognizedTextBox").Font.Size == 16f, "Valid font size is applied.");
            about.Close();
            main.Close();
        }
        using (var hidden = new MainForm())
        {
            var handle = hidden.Handle;
            var selector = Field<ComboBox>(hidden, "uiLanguageComboBox");
            selector.SelectedIndex = selector.SelectedIndex == 0 ? 1 : 0;
            Assert(!hidden.Visible && hidden.Handle == handle && GetWindowProperty(handle, "SysSetRedraw") == IntPtr.Zero,
                "Updating a hidden form neither shows it nor leaves redraw disabled.");
        }
        using (var pages = new PdfPageSelectionForm("sample.pdf")) { pages.CreateControl(); }
        CheckAdjustmentDialogs();
        Console.WriteLine("Forms: production forms, resources, live About Us translation, RTL/LTR, and resizing passed.");
    }

    private static void CheckAdjustmentDialogs()
    {
        var strings = typeof(MainForm).Assembly.GetType("DSP.Bina.Ocr.DesktopApplication.V3.Properties.Strings");
        var cultureProperty = strings.GetProperty("Culture", BindingFlags.Static | BindingFlags.NonPublic);
        var originalCulture = cultureProperty.GetValue(null);
        try
        {
            foreach (string language in new[] { "en", "fa-IR" })
            {
                cultureProperty.SetValue(null, CultureInfo.GetCultureInfo(language));
                foreach (string adjustment in new[] { "ImageBrightness", "Gamma", "Contrast", "Threshold" })
                {
                    using var dialog = new TrackBarDialog { ShowInTaskbar = false, Opacity = 0 };
                    Assert(dialog is BaseDialogForm && dialog.FormBorderStyle == FormBorderStyle.None, "Adjustment dialogs use the shared dialog chrome.");
                    if (adjustment == "Gamma") dialog.SetForGamma();
                    if (adjustment == "Contrast") dialog.SetForContrast();
                    if (adjustment == "Threshold") dialog.SetForThreshold();
                    string caption = Resource(adjustment);
                    dialog.LabelText = caption;
                    var heading = dialog.Controls.Find("titleLabel", true).Single() as Label;
                    var label = Field<Label>(dialog, "adjustmentLabel");
                    var slider = Field<TrackBar>(dialog, "adjustmentTrackBar");
                    var apply = Field<Button>(dialog, "applyButton");
                    var cancel = Field<Button>(dialog, "cancelButton");
                    Assert(dialog.Text == caption && heading.Text == caption && label.Text == caption, "Every adjustment names the window, shared heading and content.");
                    Assert(dialog.RightToLeft == (language == "fa-IR" ? RightToLeft.Yes : RightToLeft.No), "Adjustment dialog follows the UI language.");
                    Assert(apply.Text == Resource("ApplyAdjustment") && cancel.Text == Resource("CancelAdjustment"), "Adjustment actions are localized.");
                    Assert(dialog.AcceptButton == apply && dialog.CancelButton == cancel, "Enter applies and Escape cancels the adjustment.");
                    int initialValue = adjustment == "ImageBrightness" ? 0 : adjustment == "Contrast" ? 25 : 50;
                    Assert(slider.Value == initialValue && slider.Maximum == 100, "Adjustment initialization retains the existing slider values.");
                    float changedValue = float.NaN;
                    dialog.ValueUpdated += (_, args) => changedValue = args.NewValue;
                    dialog.Shown += (_, _) => dialog.BeginInvoke((Action)(() =>
                    {
                        Assert(TextRenderer.MeasureText(caption, heading.Font).Width <= heading.ClientSize.Width,
                            "The shared heading has enough space for the localized adjustment name.");
                        int sliderWidth = slider.Width;
                        dialog.Size = new Size(640, 320);
                        dialog.PerformLayout();
                        Assert(slider.Width > sliderWidth && slider.Parent == Field<TableLayoutPanel>(dialog, "adjustmentLayout"),
                            "Adjustment content stays in the shared body and expands with the dialog.");
                        slider.Value = initialValue + 5;
                        Assert(changedValue == slider.Value, "Moving the slider still raises image-preview updates.");
                        apply.PerformClick();
                    }));
                    Assert(dialog.ShowDialog() == DialogResult.OK, "Apply confirms the adjustment.");
                }
                using var canceled = new TrackBarDialog { ShowInTaskbar = false, Opacity = 0 };
                canceled.Shown += (_, _) => canceled.BeginInvoke((Action)(() => Field<Button>(canceled, "cancelButton").PerformClick()));
                Assert(canceled.ShowDialog() == DialogResult.Cancel, "Cancel discards the adjustment.");
            }
        }
        finally { cultureProperty.SetValue(null, originalCulture); }
    }

    private static void CheckWrapper()
    {
        using (var image = new Bitmap(900, 180))
        {
            using (var graphics = Graphics.FromImage(image))
            using (var font = new Font("Arial", 40))
            {
                graphics.Clear(Color.White);
                graphics.DrawString("HELLO 123", font, Brushes.Black, new PointF(25, 40));
            }
            string text = BinaOcr.Instance().GetString(image, PageSegmentationModeEnum.SingleLine,
                LanguageEnum.English, EngineModeEnum.KhanaDeepOnly, true);
            Assert(text.ToUpperInvariant().Contains("HELLO"), "Production wrapper recognition with post-processing.");
            var options = new DSP.Bina.Ocr.DesktopApplication.V3.Services.OcrOptions(PageSegmentationModeEnum.SingleLine,
                LanguageEnum.English, EngineModeEnum.KhanaDeepOnly, true);
            string asyncText = Pump(new DSP.Bina.Ocr.DesktopApplication.V3.Services.OcrService().RecognizeAsync(image, options, CancellationToken.None));
            Assert(asyncText.ToUpperInvariant().Contains("HELLO"), "Production asynchronous OCR service and owned snapshot.");
        }
        Console.WriteLine("Wrapper: production OCR API and optional post-processing passed.");
    }

    private static void CheckPdf()
    {
        string pdf = Path.Combine(Work, "input.pdf");
        string stream = "BT /F1 30 Tf 40 150 Td (HELLO 123) Tj ET\n";
        string[] objects = {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 500 250] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>",
            "<< /Length " + Encoding.ASCII.GetByteCount(stream) + " >>\nstream\n" + stream + "endstream",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>" };
        var content = new StringBuilder("%PDF-1.4\n");
        var offsets = new System.Collections.Generic.List<int>();
        for (int i = 0; i < objects.Length; i++)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(content.ToString()));
            content.Append((i + 1) + " 0 obj\n" + objects[i] + "\nendobj\n");
        }
        int xref = Encoding.ASCII.GetByteCount(content.ToString());
        content.Append("xref\n0 6\n0000000000 65535 f \n");
        foreach (int offset in offsets) content.Append(offset.ToString("D10") + " 00000 n \n");
        content.Append("trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n" + xref + "\n%%EOF\n");
        File.WriteAllText(pdf, content.ToString(), Encoding.ASCII);
        var pages = PDFConvert.ConvertPdf2Png(pdf, out string errors, Path.Combine(Work, "pages"), 150, "1");
        Assert(pages.Count == 1 && File.Exists(pages[0]), "Native PDF rendering: " + errors);
        using (var image = Image.FromFile(pages[0])) Assert(image.Width > 0 && image.Height > 0, "Rendered PDF page.");
        Console.WriteLine("PDF: native one-page import passed.");
    }
}
