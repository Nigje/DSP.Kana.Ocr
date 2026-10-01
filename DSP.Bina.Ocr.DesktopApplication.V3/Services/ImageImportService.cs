using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using DSP.Khana.ImageTools.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Services
{
    public sealed class ImageImportService : IDisposable
    {
        private readonly string sessionDirectory = Path.Combine(Path.GetTempPath(), "DSP.Khana.Ocr", Guid.NewGuid().ToString("N"));
        private bool disposed;

        public ImageEntity ImportImage(string filePath, FileType type = FileType.Image)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            using var source = new Bitmap(filePath);
            // Detach from the source file so edits/removal do not keep it locked.
            return new ImageEntity(ImageHelper.CloneImage(source))
            {
                Name = Path.GetFileName(filePath),
                FilePath = filePath,
                FileType = type
            };
        }

        public IReadOnlyList<ImageEntity> ImportPdf(string filePath, string pages)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            _ = global::Bina.Ocr.Wapper.BinaOcr.Instance();
            string outputDirectory = Path.Combine(sessionDirectory, Guid.NewGuid().ToString("N"));
            var imported = new List<ImageEntity>();
            try
            {
                var files = PDFConvert.ConvertPdf2Png(filePath, out string error, outputDirectory, 150, pages);
                if (!string.IsNullOrWhiteSpace(error)) throw new IOException(error);
                foreach (string page in files) imported.Add(ImportImage(page, FileType.Pdf));
                return imported;
            }
            catch
            {
                foreach (var image in imported) image.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            // This path is created exclusively for this service; no user directory is deleted.
            try { if (Directory.Exists(sessionDirectory)) Directory.Delete(sessionDirectory, true); }
            catch (IOException error) { ApplicationErrorService.Log(error); }
            catch (UnauthorizedAccessException error) { ApplicationErrorService.Log(error); }
        }
    }
}
