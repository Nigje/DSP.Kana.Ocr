using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using DSP.Khana.ImageTools.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Services
{
    public enum ImageEdit { Deskew, Smooth, Sharpen, Invert, Monochrome, Grayscale, RotateRight, RotateLeft }
    public enum ImageAdjustment { Brightness, Gamma, Contrast, Threshold }

    public sealed class ImageEditService
    {
        public void Apply(ImageEntity image, ImageEdit edit)
        {
            Bitmap source = (Bitmap)image.GetImage();
            Image result = edit switch
            {
                ImageEdit.Deskew => ImageHelper.Deskew(source, 0.05d, out _),
                ImageEdit.Smooth => ImageHelper.GaussianBlur(source),
                ImageEdit.Sharpen => ImageHelper.Sharpen(source),
                ImageEdit.Invert => ImageHelper.InvertColor(source),
                ImageEdit.Monochrome => Monochrome(source),
                ImageEdit.Grayscale => ImageHelper.ConvertGrayscale(source),
                ImageEdit.RotateRight => Rotate(source, RotateFlipType.Rotate90FlipNone),
                ImageEdit.RotateLeft => Rotate(source, RotateFlipType.Rotate270FlipNone),
                _ => throw new ArgumentOutOfRangeException(nameof(edit))
            };
            if (ReferenceEquals(source, result)) return;
            using (result) image.SetImage(result);
        }

        public void Crop(ImageEntity image, Rectangle rectangle)
        {
            Image source = image.GetImage();
            rectangle.Intersect(new Rectangle(Point.Empty, source.Size));
            if (rectangle.Width <= 0 || rectangle.Height <= 0) return;
            using Image cropped = ImageHelper.Crop(source, rectangle);
            if (source.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                using Image grayscale = ImageHelper.ConvertGrayscale(cropped);
                image.SetImage(grayscale);
            }
            else image.SetImage(cropped);
        }

        // Caller owns the returned preview; the source is always borrowed.
        public Image Preview(Image source, ImageAdjustment adjustment, float value) => adjustment switch
        {
            ImageAdjustment.Brightness => ImageHelper.Brighten(source, value * 0.005f),
            ImageAdjustment.Gamma => ImageHelper.AdjustGamma(source, value * 0.02f),
            ImageAdjustment.Contrast => ImageHelper.Contrast(source, value * 0.04f),
            ImageAdjustment.Threshold => ImageHelper.AdjustThreshold(source, value * 0.01f),
            _ => throw new ArgumentOutOfRangeException(nameof(adjustment))
        };

        private static Image Monochrome(Bitmap source)
        {
            using var prepared = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppPArgb);
            prepared.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(prepared)) graphics.DrawImage(source, 0, 0);
            return ImageHelper.ConvertMonochrome(prepared);
        }

        private static Image Rotate(Bitmap source, RotateFlipType rotation)
        {
            Bitmap result = ImageHelper.CloneImage(source);
            try { result.RotateFlip(rotation); return result; }
            catch { result.Dispose(); throw; }
        }
    }
}
