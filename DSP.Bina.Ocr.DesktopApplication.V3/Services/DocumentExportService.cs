using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using W = DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;
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
            using (var document = WordprocessingDocument.Create(output, WordprocessingDocumentType.Document))
            {
                var main = document.AddMainDocumentPart();
                var body = new W.Body();
                main.Document = new W.Document(body);
                if (snapshot.Paragraphs.Any(p => p.Bullet)) AddBulletNumbering(main);

                foreach (var source in snapshot.Paragraphs)
                {
                    var properties = new W.ParagraphProperties
                    {
                        BiDi = new W.BiDi { Val = source.RightToLeft },
                        Justification = new W.Justification
                        {
                            Val = source.Alignment switch
                            {
                                System.Windows.Forms.HorizontalAlignment.Center => W.JustificationValues.Center,
                                System.Windows.Forms.HorizontalAlignment.Right => W.JustificationValues.Right,
                                _ => W.JustificationValues.Left
                            }
                        }
                    };
                    if (source.Bullet)
                        properties.NumberingProperties = new W.NumberingProperties(
                            new W.NumberingLevelReference { Val = 0 }, new W.NumberingId { Val = 1 });
                    if (source.IndentPoints != 0 || source.HangingIndentPoints != 0)
                    {
                        properties.Indentation = new W.Indentation { Start = Twips(source.IndentPoints) };
                        if (source.HangingIndentPoints >= 0)
                            properties.Indentation.Hanging = Twips(source.HangingIndentPoints);
                        else
                            properties.Indentation.FirstLine = Twips(-source.HangingIndentPoints);
                    }
                    var paragraph = new W.Paragraph(properties);
                    foreach (var sourceRun in source.Runs)
                    {
                        bool bold = sourceRun.Style.HasFlag(FontStyle.Bold);
                        bool italic = sourceRun.Style.HasFlag(FontStyle.Italic);
                        string halfPoints = Math.Max(1, (int)Math.Round(sourceRun.FontSize * 2, MidpointRounding.AwayFromZero))
                            .ToString(CultureInfo.InvariantCulture);
                        var run = new W.Run(new W.RunProperties
                        {
                            RunFonts = new W.RunFonts
                            {
                                Ascii = sourceRun.FontName, HighAnsi = sourceRun.FontName,
                                EastAsia = sourceRun.FontName, ComplexScript = sourceRun.FontName
                            },
                            Bold = new W.Bold { Val = bold },
                            BoldComplexScript = new W.BoldComplexScript { Val = bold },
                            Italic = new W.Italic { Val = italic },
                            ItalicComplexScript = new W.ItalicComplexScript { Val = italic },
                            Strike = new W.Strike { Val = sourceRun.Style.HasFlag(FontStyle.Strikeout) },
                            Color = new W.Color { Val = $"{sourceRun.Color.R:X2}{sourceRun.Color.G:X2}{sourceRun.Color.B:X2}" },
                            FontSize = new W.FontSize { Val = halfPoints },
                            FontSizeComplexScript = new W.FontSizeComplexScript { Val = halfPoints },
                            Underline = new W.Underline
                            {
                                Val = sourceRun.Style.HasFlag(FontStyle.Underline) ? W.UnderlineValues.Single : W.UnderlineValues.None
                            },
                            RightToLeftText = new W.RightToLeftText { Val = source.RightToLeft }
                        });
                        AppendText(run, sourceRun.Text ?? "");
                        paragraph.Append(run);
                    }
                    body.Append(paragraph);
                }
                main.Document.Save();
            }
            return output.ToArray();
        }

        private static string Twips(float points) =>
            ((int)Math.Round(points * 20, MidpointRounding.AwayFromZero)).ToString(CultureInfo.InvariantCulture);

        private static void AddBulletNumbering(MainDocumentPart main)
        {
            var part = main.AddNewPart<NumberingDefinitionsPart>();
            var level = new W.Level(
                new W.StartNumberingValue { Val = 1 },
                new W.NumberingFormat { Val = W.NumberFormatValues.Bullet },
                new W.LevelText { Val = "•" },
                new W.LevelJustification { Val = W.LevelJustificationValues.Left },
                new W.PreviousParagraphProperties(new W.Indentation { Start = "720", Hanging = "360" }))
            { LevelIndex = 0 };
            part.Numbering = new W.Numbering(
                new W.AbstractNum(new W.MultiLevelType { Val = W.MultiLevelValues.SingleLevel }, level) { AbstractNumberId = 1 },
                new W.NumberingInstance(new W.AbstractNumId { Val = 1 }) { NumberID = 1 });
            part.Numbering.Save();
        }

        private static void AppendText(W.Run run, string text)
        {
            int start = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char character = text[i];
                if (character != '\t' && character != '\r' && character != '\n') continue;
                if (i > start) run.Append(new W.Text(text.Substring(start, i - start)) { Space = SpaceProcessingModeValues.Preserve });
                if (character == '\t') run.Append(new W.TabChar());
                else
                {
                    run.Append(new W.Break());
                    if (character == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                }
                start = i + 1;
            }
            if (start < text.Length) run.Append(new W.Text(text.Substring(start)) { Space = SpaceProcessingModeValues.Preserve });
        }
    }
}
