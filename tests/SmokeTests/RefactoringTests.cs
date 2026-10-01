using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Bina.Ocr.Wapper;
using DSP.Bina.Ocr.DesktopApplication.V3;
using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using DSP.Bina.Ocr.DesktopApplication.V3.Services;
using Xceed.Words.NET;

internal static partial class SmokeTests
{
    private static void CheckRefactorings()
    {
        CheckImageOwnership();
        CheckFormattedExports();
        CheckOcrCoordination();
        CheckEditorAndWorkflow();
        Assert(ApplicationErrorService.GetMessage(new SharingViolationException()) == Resource("FileInUse"), "File sharing errors use typed Windows codes.");
        Assert(ApplicationErrorService.GetMessage(new UnauthorizedAccessException()) == Resource("AccessDenied"), "Access errors use a localized message.");
        Console.WriteLine("Refactoring: disposal, detached import, formatted collision-safe exports, editor persistence, OCR serialization/cancellation/failure/close passed.");
    }

    private static string Resource(string name)
    {
        var type = typeof(MainForm).Assembly.GetType("DSP.Bina.Ocr.DesktopApplication.V3.Properties.Strings");
        return (string)type.GetProperty(name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
    }

    private sealed class SharingViolationException : IOException
    {
        public SharingViolationException() : base("A message that does not mention file access") { HResult = unchecked((int)0x80070020); }
    }

    private static void AssertDisposed(Image bitmap, string message)
    {
        bool disposed = false;
        try { _ = bitmap.Width; }
        catch (ArgumentException) { disposed = true; }
        catch (ObjectDisposedException) { disposed = true; }
        Assert(disposed, message);
    }

    private static void CheckImageOwnership()
    {
        var original = new Bitmap(4, 2);
        using var replacement = new Bitmap(4, 2);
        var entity = new ImageEntity(original);
        for (int i = 0; i < 21; i++) entity.SetImage(replacement);
        AssertDisposed(original, "Evicted undo entries are disposed.");
        Image discardedRedo = entity.GetImage();
        entity.Undo();
        entity.SetImage(replacement);
        AssertDisposed(discardedRedo, "Discarded redo entries are disposed.");
        Image current = entity.GetImage();
        entity.Dispose(); entity.Dispose();
        AssertDisposed(current, "Current image is disposed exactly once with its entity.");
        using var importer = new ImageImportService();
        string source = Path.Combine(Work, "detached.png");
        replacement.Save(source);
        using var imported = importer.ImportImage(source);
        using (var unlocked = new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
        File.Delete(source);
        Assert(imported.GetImage().Width == 4, "Imported bitmap survives source-file deletion.");
        var editor = new ImageEditService();
        editor.Apply(imported, ImageEdit.RotateRight);
        Assert(imported.GetImage().Size == new Size(2, 4), "Rotation is committed through the image service.");
        imported.Undo();
        Assert(imported.GetImage().Size == new Size(4, 2), "Editing preserves undo history.");
    }

    private static void CheckFormattedExports()
    {
        using var editor = new RichTextBox { RightToLeft = RightToLeft.Yes };
        _ = editor.Handle;
        editor.Text = "Hello\nسلام";
        using var firstFont = new Font("Arial", 20, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
        editor.Select(0, 5); editor.SelectionFont = firstFont; editor.SelectionColor = Color.Red;
        editor.SelectionAlignment = HorizontalAlignment.Center;
        editor.Select(6, 4); editor.SelectionBullet = true; editor.SelectionIndent = 24;
        using var secondFont = new Font("Tahoma", 16);
        editor.SelectionFont = secondFont;
        var snapshot = RichTextDocumentService.Capture(editor);
        using var image = new Bitmap(20, 20);
        var exporter = new DocumentExportService();
        string directory = Path.Combine(Work, "exports");
        string first = exporter.Export(Path.Combine(directory, "scan"), image, snapshot);
        byte[] originalWord = File.ReadAllBytes(first + ".docx");
        byte[] originalJpeg = File.ReadAllBytes(first + ".jpeg");
        string second = exporter.Export(Path.Combine(directory, "scan"), image, snapshot);
        Assert(second.EndsWith("scan (2)"), "Duplicate names receive a suffix.");
        File.WriteAllText(Path.Combine(directory, "scan (3).docx"), "existing document");
        string fourth = exporter.Export(Path.Combine(directory, "scan"), image, snapshot);
        Assert(fourth.EndsWith("scan (4)"), "A conflicting DOCX alone reserves its base filename.");
        Assert(File.ReadAllBytes(first + ".docx").SequenceEqual(originalWord) && File.ReadAllBytes(first + ".jpeg").SequenceEqual(originalJpeg), "Existing exports are never overwritten.");
        Assert(File.ReadAllText(Path.Combine(directory, "scan (3).docx")) == "existing document", "Conflicting user files stay intact.");
        using var document = DocX.Load(first + ".docx");
        Assert(document.Text.Contains("Hello") && document.Text.Contains("سلام"), "Bilingual edited text round-trips.");
        using var zip = ZipFile.OpenRead(first + ".docx");
        using var xmlStream = zip.GetEntry("word/document.xml").Open();
        var xml = XDocument.Load(xmlStream);
        XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        Assert(xml.Descendants(w + "b").Any() && xml.Descendants(w + "i").Any() && xml.Descendants(w + "u").Any(), "DOCX preserves bold, italic and underline.");
        Assert(xml.Descendants(w + "color").Any(e => (string)e.Attribute(w + "val") == "FF0000"), "DOCX preserves text color.");
        Assert(xml.Descendants(w + "sz").Any(e => (string)e.Attribute(w + "val") == "40"), "DOCX preserves font size.");
        Assert(xml.Descendants(w + "jc").Any(e => (string)e.Attribute(w + "val") == "center"), "DOCX preserves paragraph alignment.");
        Assert(xml.Descendants(w + "numPr").Any() && xml.Descendants(w + "bidi").Any(), "DOCX preserves bullets and RTL direction.");
        Assert(xml.Descendants(w + "ind").Any(), "DOCX preserves indentation.");
    }

    private static T Pump<T>(Task<T> task)
    {
        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (!task.IsCompleted && DateTime.UtcNow < deadline) { Application.DoEvents(); Thread.Sleep(1); }
        Assert(task.IsCompleted, "Asynchronous test completed without deadlocking the UI.");
        return task.GetAwaiter().GetResult();
    }

    private static void CheckOcrCoordination()
    {
        using var image = new Bitmap(20, 20);
        using var firstCancellation = new CancellationTokenSource();
        using var queuedCancellation = new CancellationTokenSource();
        var native = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        int calls = 0;
        Bitmap nativeBitmap = null;
        var service = new OcrService((bitmap, _) => { calls++; nativeBitmap = bitmap; return native.Task; });
        var options = new OcrOptions(PageSegmentationModeEnum.SingleLine, LanguageEnum.English, EngineModeEnum.KhanaDeepOnly, false);
        Task<string> first = service.RecognizeAsync(image, options, firstCancellation.Token);
        Task<string> queued = service.RecognizeAsync(image, options, queuedCancellation.Token);
        Assert(calls == 1, "Native recognition calls are serialized.");
        queuedCancellation.Cancel();
        bool canceled = false;
        try { Pump(queued); } catch (OperationCanceledException) { canceled = true; }
        Assert(canceled && calls == 1, "Queued cancellation never enters native OCR.");
        firstCancellation.Cancel();
        Assert(!first.IsCompleted && nativeBitmap.Width == 20, "Canceling an active native page retains its bitmap until completion.");
        native.SetResult("discarded");
        canceled = false;
        try { Pump(first); } catch (OperationCanceledException) { canceled = true; }
        Assert(canceled, "Canceled native results are discarded.");
        AssertDisposed(nativeBitmap, "Recognition snapshots are disposed after native completion.");
        var following = new OcrService((_, _) => Task.FromResult("next"));
        Assert(Pump(following.RecognizeAsync(image, options, CancellationToken.None)) == "next", "Cancellation releases the native gate.");
    }

    private static object Invoke(MainForm form, string name, params object[] args) => typeof(MainForm).GetMethod(name, PrivateInstance).Invoke(form, args);

    private static void SelectImage(MainForm form, int index)
    {
        var list = Field<Manina.Windows.Forms.ImageListView>(form, "imageListView");
        foreach (var item in list.Items) item.Selected = false;
        list.Items[index].Selected = true;
        Application.DoEvents();
    }

    private static void CheckEditorAndWorkflow()
    {
        TaskCompletionSource<string> native = null;
        int calls = 0;
        bool fail = false, immediate = false;
        var service = new OcrService((_, _) =>
        {
            calls++;
            if (fail) return Task.FromException<string>(new IOException("test failure"));
            if (immediate) return Task.FromResult("fresh result");
            return native.Task;
        });
        using var form = new MainForm(service) { ShowInTaskbar = false, Opacity = 0 };
        form.Size = new Size(1280, 720);
        form.Show();
        var first = new ImageEntity(new Bitmap(300, 100)) { Name = "same.png" };
        var second = new ImageEntity(new Bitmap(300, 100)) { Name = "same.png" };
        Invoke(form, "AddImage", first); Invoke(form, "AddImage", second); SelectImage(form, 0);
        var editor = Field<RichTextBox>(form, "recognizedTextBox");
        editor.Text = "edited text";
        editor.SelectAll(); Invoke(form, "BoldButton_Click", form, EventArgs.Empty);
        Assert(first.RecognitionResult == "edited text" && first.RecognitionRtf.Contains("\\b"), "Editor text and formatting are saved to the selected image.");
        SelectImage(form, 1); editor.Text = "other text"; SelectImage(form, 0);
        string savedRtf = first.RecognitionRtf;
        var captured = RichTextDocumentService.Capture(first);
        Assert(captured.Paragraphs.SelectMany(p => p.Runs).Any(r => r.Text.Contains("edited")), "Stored RTF exports edited text through the hidden reader.");
        Assert(first.RecognitionRtf == savedRtf, "Export leaves saved editor formatting intact.");
        Assert(editor.Text == "edited text", "Switching images restores independently edited text.");
        editor.SelectAll();
        using (Font selected = editor.SelectionFont) Assert(selected.Bold, "Switching images restores text formatting.");

        native = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        var pending = (Task<bool>)Invoke(form, "StartRecognitionAsync", true);
        Assert(!Field<Panel>(form, "recognitionToolbarPanel").Enabled && editor.ReadOnly, "OCR protects editing controls.");
        Assert(!Pump((Task<bool>)Invoke(form, "StartRecognitionAsync", true)) && calls == 1, "Overlapping form runs are rejected.");
        var cancel = Field<Button>(form, "cancelRecognitionButton");
        Assert(cancel.Visible && cancel.Enabled && cancel.Width >= 100 && cancel.Right <= cancel.Parent.ClientSize.Width, "The cancel button remains visible at a 1280-pixel width.");
        cancel.PerformClick(); native.SetResult("not committed");
        Assert(!Pump(pending) && calls == 1 && first.RecognitionResult == "edited text" && second.RecognitionResult == "other text", "Batch cancellation stops before the next image and preserves previous edits.");
        Assert(Field<Panel>(form, "recognitionToolbarPanel").Enabled && !editor.ReadOnly && Field<Label>(form, "statusLabel").Text == "", "Cancellation restores controls and status.");
        fail = true;
        bool failed = false;
        try { Pump((Task<bool>)Invoke(form, "StartRecognitionAsync", false)); } catch (IOException) { failed = true; }
        Assert(failed && !Field<bool>(form, "isProcessing") && !editor.ReadOnly, "OCR failure restores UI state in finally.");
        fail = false; immediate = true;
        Assert(Pump((Task<bool>)Invoke(form, "StartRecognitionAsync", false)) && editor.Text == "fresh result", "A successful run is possible after cancellation/failure.");
        Image removedBitmap = second.GetImage();
        Invoke(form, "RemoveImage", second.Id);
        AssertDisposed(removedBitmap, "Removing an image disposes its bitmap without disposing the retained thumbnail/model.");
        Assert(first.GetImage().Width == 300, "Removing another image leaves the selected image valid.");
        immediate = false; native = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        pending = (Task<bool>)Invoke(form, "StartRecognitionAsync", false);
        form.Close();
        Assert(!form.IsDisposed && Field<bool>(form, "closeAfterProcessing"), "Closing waits for active native OCR.");
        native.SetResult("discard on close"); Pump(pending); Application.DoEvents();
        Assert(form.IsDisposed, "The form closes after cancellation safely drains native work.");
    }
}
