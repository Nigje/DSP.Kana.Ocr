using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using System.Drawing;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Services
{
    public static class RichTextDocumentService
    {
        public static DocumentSnapshot Capture(ImageEntity image)
        {
            using var editor = new RichTextBox { RightToLeft = image.TextRightToLeft ? RightToLeft.Yes : RightToLeft.No };
            _ = editor.Handle;
            if (!string.IsNullOrEmpty(image.RecognitionRtf)) editor.Rtf = image.RecognitionRtf;
            else editor.Text = image.RecognitionResult ?? "";
            return Capture(editor);
        }

        public static DocumentSnapshot Capture(RichTextBox editor)
        {
            var document = new DocumentSnapshot();
            int selectionStart = editor.SelectionStart, selectionLength = editor.SelectionLength;
            try
            {
                string text = editor.Text;
                int offset = 0;
                foreach (string line in text.Split('\n'))
                {
                    editor.Select(offset, 0);
                    var paragraph = new ParagraphSnapshot
                    {
                        RightToLeft = editor.RightToLeft == RightToLeft.Yes,
                        Alignment = editor.SelectionAlignment,
                        Bullet = editor.SelectionBullet,
                        IndentPoints = editor.SelectionIndent * 72f / editor.DeviceDpi,
                        HangingIndentPoints = editor.SelectionHangingIndent * 72f / editor.DeviceDpi
                    };
                    int runStart = 0;
                    TextRunSnapshot current = null;
                    for (int i = 0; i < line.Length; i++)
                    {
                        editor.Select(offset + i, 1);
                        using Font font = editor.SelectionFont ?? (Font)editor.Font.Clone();
                        var style = new TextRunSnapshot("", font.Name, font.SizeInPoints, font.Style, editor.SelectionColor);
                        if (current != null && current != style)
                        {
                            paragraph.Runs.Add(current with { Text = line.Substring(runStart, i - runStart) });
                            runStart = i;
                        }
                        current = style;
                    }
                    if (current != null) paragraph.Runs.Add(current with { Text = line.Substring(runStart) });
                    document.Paragraphs.Add(paragraph);
                    offset += line.Length + 1;
                }
                return document;
            }
            finally { editor.Select(selectionStart, selectionLength); }
        }
    }
}
