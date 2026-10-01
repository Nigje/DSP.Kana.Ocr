using DSP.Khana.ImageTools;
using DSP.Khana.ImageTools.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public class ImageEntity
    {
        public ImageEntity(Bitmap bitmap)
        {
            Bitmap = bitmap;
            LastStateImage = bitmap;
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string RecognitionResult { get; set; }
        public string Name { get; set; }
        public string NameWithoutExtention { get => Name.Substring(0, Name.LastIndexOf(".")); }
        private Bitmap Bitmap { get; set; }
        private Image LastStateImage { get; set; }
        public string FileFullName { get; set; }
        public FileType FileType { get; set; }
        FixedSizeStack<Image> LastStack = new FixedSizeStack<Image>(20);
        FixedSizeStack<Image> NextStack = new FixedSizeStack<Image>(20);
        public Image GetImage()
        {
            if (LastStateImage == null)
                return Bitmap;
            return LastStateImage;
        }
        public Image GetClonedImage()
        {
            return ImageHelper.CloneImage((Bitmap)GetImage());
        }
        public string SetRecognitionResult(string text)
        {
            RecognitionResult = text;
            return RecognitionResult;
        }
        public void SetImage(Image image)
        {
            Image newimage=ImageHelper.CloneImage((Bitmap)image);

            LastStack.Push((Image)ImageHelper.CloneImage((Bitmap)LastStateImage));
            
            LastStateImage = newimage;
        }
        public Image Backward()
        {
            
            if (LastStack.Count == 0)
            {
                return LastStateImage;
            }
            NextStack.Push(LastStateImage);
            LastStateImage = LastStack.Pop();
            return LastStateImage;
        }
        public Image ImageForward()
        {
            if (NextStack.Count == 0)
            {
                return LastStateImage;
            }
            LastStack.Push(LastStateImage);
            LastStateImage = NextStack.Pop();
            return LastStateImage;
        }
    }
    public enum FileType
    {
        Image=1,
        PDF=2
    }
}
