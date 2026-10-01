using DSP.Khana.ImageTools.Models;
using System;
using System.Drawing;
using System.IO;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class ImageEntity
    {
        private Image currentImage;
        private readonly FixedSizeStack<Image> undoHistory = new FixedSizeStack<Image>(20);
        private readonly FixedSizeStack<Image> redoHistory = new FixedSizeStack<Image>(20);

        public ImageEntity(Bitmap bitmap)
        {
            currentImage = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string RecognitionResult { get; set; }
        public string Name { get; set; }
        public string NameWithoutExtension => Path.GetFileNameWithoutExtension(Name);
        public string FilePath { get; set; }
        public FileType FileType { get; set; }

        public Image GetImage() => currentImage;
        public Image GetClonedImage() => ImageHelper.CloneImage((Bitmap)currentImage);

        public string SetRecognitionResult(string text)
        {
            RecognitionResult = text;
            return text;
        }

        public void SetImage(Image image)
        {
            ArgumentNullException.ThrowIfNull(image);
            Image editedImage = ImageHelper.CloneImage((Bitmap)image);
            undoHistory.Push(currentImage);
            currentImage = editedImage;
            foreach (Image redoImage in redoHistory) redoImage.Dispose();
            redoHistory.Clear();
        }

        public Image Undo()
        {
            if (undoHistory.Count == 0) return currentImage;
            redoHistory.Push(currentImage);
            currentImage = undoHistory.Pop();
            return currentImage;
        }

        public Image Redo()
        {
            if (redoHistory.Count == 0) return currentImage;
            undoHistory.Push(currentImage);
            currentImage = redoHistory.Pop();
            return currentImage;
        }
    }

    public enum FileType
    {
        Image = 1,
        Pdf = 2
    }
}
