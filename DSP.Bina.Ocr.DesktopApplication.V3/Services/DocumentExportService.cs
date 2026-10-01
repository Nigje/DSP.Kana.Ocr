using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Image = System.Drawing.Image;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Services
{
    public sealed class DocumentExportService
    {
        // Returns the actual base path, including any suffix needed to avoid overwriting files.
        public string Export(string requestedBasePath, Image image, DocumentSnapshot text)
        {
            string fullPath = Path.GetFullPath(requestedBasePath);
            string directory = Path.GetDirectoryName(fullPath);
            Directory.CreateDirectory(directory);
            string name = Path.GetFileName(fullPath);
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("An export filename is required.", nameof(requestedBasePath));
            bool hasText = text.Paragraphs.Any(p => p.Runs.Any(r => !string.IsNullOrEmpty(r.Text)));
            using var jpeg = new MemoryStream();
            image.Save(jpeg, ImageFormat.Jpeg);
            byte[] documentBytes = hasText ? CreateDocument(text) : null;
            for (int suffix = 1; ; suffix++)
            {
                string basePath = Path.Combine(directory, suffix == 1 ? name : $"{name} ({suffix})");
                string jpegPath = basePath + ".jpeg", wordPath = basePath + ".docx";
                if (File.Exists(jpegPath) || File.Exists(wordPath)) continue;
                FileStream jpegFile = null, wordFile = null;
                bool jpegCreated = false, wordCreated = false, success = false;
                try
                {
                    jpegFile = new FileStream(jpegPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    jpegCreated = true;
                    if (hasText)
                    {
                        wordFile = new FileStream(wordPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        wordCreated = true;
                    }
                    jpeg.Position = 0;
                    jpeg.CopyTo(jpegFile);
                    wordFile?.Write(documentBytes);
                    success = true;
                    return basePath;
                }
                catch (IOException error) when ((error.HResult & 0xffff) is 80 or 183) { }
                finally
                {
                    wordFile?.Dispose();
                    jpegFile?.Dispose();
                    if (!success)
                    {
                        if (wordCreated) File.Delete(wordPath);
                        if (jpegCreated) File.Delete(jpegPath);
                    }
                }
            }
        }

        private static byte[] CreateDocument(DocumentSnapshot snapshot)
        {
            using var output = new MemoryStream();
            using (var document = DocX.Create(output))
            {
                foreach (var source in snapshot.Paragraphs)
                {
                    Paragraph paragraph;
                    if (source.Bullet)
                    {
                        var list = document.AddList("", listType: ListItemType.Bulleted);
                        document.InsertList(list);
                        paragraph = list.Items[0];
                    }
                    else paragraph = document.InsertParagraph();
                    paragraph.Direction = source.RightToLeft ? Direction.RightToLeft : Direction.LeftToRight;
                    paragraph.Alignment = source.Alignment switch
                    {
                        System.Windows.Forms.HorizontalAlignment.Center => Alignment.center,
                        System.Windows.Forms.HorizontalAlignment.Right => Alignment.right,
                        _ => Alignment.left
                    };
                    paragraph.IndentationBefore = source.IndentPoints;
                    paragraph.IndentationHanging = source.HangingIndentPoints;
                    foreach (var run in source.Runs)
                    {
                        paragraph.Append(run.Text).Font(new Xceed.Document.NET.Font(run.FontName)).FontSize(run.FontSize)
                            .Color(run.Color).Bold(run.Style.HasFlag(FontStyle.Bold)).Italic(run.Style.HasFlag(FontStyle.Italic))
                            .UnderlineStyle(run.Style.HasFlag(FontStyle.Underline) ? UnderlineStyle.singleLine : UnderlineStyle.none);
                    }
                }
                document.Save();
            }
            return output.ToArray();
        }
    }
}
