using DSP.Khana.ImageTools.Models;
using System;
using System.Drawing;
using System.IO;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public sealed class ImageEntity : IDisposable
    {
        private Image currentImage;
        private bool disposed;
        private readonly FixedSizeStack<Image> undoHistory = new FixedSizeStack<Image>(20, image => image.Dispose());
        private readonly FixedSizeStack<Image> redoHistory = new FixedSizeStack<Image>(20, image => image.Dispose());

        public ImageEntity(Bitmap bitmap)
        {
            currentImage = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public string RecognitionResult { get; set; }
        public string RecognitionRtf { get; set; }
        public bool TextRightToLeft { get; set; }
        public string Name { get; set; }
        public string NameWithoutExtension => Path.GetFileNameWithoutExtension(Name);
        public string FilePath { get; set; }
        public FileType FileType { get; set; }

        public Image GetImage()
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            return currentImage;
        }
        public Image GetClonedImage() => ImageHelper.CloneImage((Bitmap)GetImage());

        public string SetRecognitionResult(string text)
        {
            RecognitionResult = text;
            RecognitionRtf = null;
            return text;
        }

        public void SetImage(Image image)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            ArgumentNullException.ThrowIfNull(image);
            Image editedImage = ImageHelper.CloneImage((Bitmap)image);
            undoHistory.Push(currentImage);
            currentImage = editedImage;
            foreach (Image redoImage in redoHistory) redoImage.Dispose();
            redoHistory.Clear();
        }

        public Image Undo()
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (undoHistory.Count == 0) return currentImage;
            redoHistory.Push(currentImage);
            currentImage = undoHistory.Pop();
            return currentImage;
        }

        public Image Redo()
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            if (redoHistory.Count == 0) return currentImage;
            undoHistory.Push(currentImage);
            currentImage = redoHistory.Pop();
            return currentImage;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            currentImage.Dispose();
            currentImage = null;
            while (undoHistory.Count > 0) undoHistory.Pop().Dispose();
            while (redoHistory.Count > 0) redoHistory.Pop().Dispose();
        }

    }

    public enum FileType
    {
        Image = 1,
        Pdf = 2
    }
}
