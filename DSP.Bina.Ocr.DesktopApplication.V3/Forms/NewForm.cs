
using Bina.Ocr.Wapper;
using DSP.Bina.Ocr.DesktopApplication.V3.Forms;
using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using DSP.Khana.ImageTools;
using DSP.Khana.ImageTools.Models;
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Words.NET;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class NewForm : Form
    {
        public NewForm()
        {
            InitializeComponent();
            InitializeDirectionalLayout();
            imageListView.ThumbnailSize = new Size(150, 150);
            InlineTempDirectory = AppDomain.CurrentDomain.BaseDirectory + $".TempData\\{UniqeID.ToString()}\\";
            InitiateVariables();
            InitializeUiLanguageSelector();
            this.CenterToScreen();
            BinaOcr binaOcr = BinaOcr.Instance();

        }
        [System.Reflection.ObfuscationAttribute(Exclude = true, ApplyToMembers = false)]
        private void InitiateVariables()
        {
            l_version.Text = string.Format(Properties.Strings.BinaVersionFormat, System.Windows.Forms.Application.ProductVersion);
            foreach (FontFamily oneFontFamily in FontFamily.Families)
            {
                cb_fontName.Items.Add(oneFontFamily.Name);
            }
            cb_fontName.Text = this.rt_main.Font.Name.ToString();
            cb_fontSize.Text = this.rt_main.Font.Size.ToString();
            cb_fontName.SelectedItem = FontFamily.Families.FirstOrDefault(x => x.Name == "B Mitra");
            foreach (float value in FontSizes)
            {
                cb_fontSize.Items.Add(value);
            }

            //PageSegmentatiionMode

            //pairPageSegmentationMode.Add(new PairValue { Text = Properties.Strings.LayoutOrientationAndScriptDetection, Value = PageSegmentationModeEnum.OsdOnly.ToString() });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutAutomaticWithOrientationAndScriptDetection, Value = PageSegmentationModeEnum.AutoOsd });
            //pairPageSegmentationMode.Add(new PairValue { Text = Properties.Strings.LayoutAutomaticWithoutOrientationAndScriptDetection, Value = PageSegmentationModeEnum.AutoOnly.ToString() });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutWithoutOrientationAndScriptDetection, Value = PageSegmentationModeEnum.Auto });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutSingleColumn, Value = PageSegmentationModeEnum.SingleColumn });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutSingleVerticalTextBlock, Value = PageSegmentationModeEnum.SingleBlockVertText });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutSingleTextBlock, Value = PageSegmentationModeEnum.SingleBlock });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutSingleLine, Value = PageSegmentationModeEnum.SingleLine });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutCircularWord, Value = PageSegmentationModeEnum.CircleWord });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutSingleCharacter, Value = PageSegmentationModeEnum.SingleChar });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutSparseText, Value = PageSegmentationModeEnum.SparseText });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutSparseTextWithOrientationAndScriptDetection, Value = PageSegmentationModeEnum.SparseTextOsd });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutRawLine, Value = PageSegmentationModeEnum.RawLine });
            pairPageSegmentationMode.Add(new PairPageSegmentationMode { Text = Properties.Strings.LayoutMultipleModes, Value = PageSegmentationModeEnum.Count });
            cb_structure.Items.AddRange(pairPageSegmentationMode.ToArray());
            cb_structure.DisplayMember = "Text";
            cb_structure.ValueMember = "Value";
            cb_structure.SelectedItem = pairPageSegmentationMode.FirstOrDefault(x => x.Value == PageSegmentationModeEnum.SingleBlock);

            // Engine Mode
            pairEngines.Add(new PairEngine { Text = Properties.Strings.EngineDeep, Value = EngineModeEnum.KhanaDeepOnly });
            pairEngines.Add(new PairEngine { Text = Properties.Strings.EngineStructuralAndDeep, Value = EngineModeEnum.KhanaStructuralAndKhanaDeep });
            pairEngines.Add(new PairEngine { Text = Properties.Strings.EngineStructural, Value = EngineModeEnum.KhanaStructuralOnly });
            //pairEngines.Add(new PairValue { Text = Properties.Strings.Default, Value = EngineModeEnum.Default.ToString() });
            


            //Language
            pairLanguages.Add(new PairLanguage { Text = Properties.Strings.LanguagePersian, Value = LanguageEnum.Farsi });
            pairLanguages.Add(new PairLanguage { Text = Properties.Strings.LanguageEnglish, Value = LanguageEnum.English });
            pairLanguages.Add(new PairLanguage { Text = Properties.Strings.LanguageMixed, Value = LanguageEnum.Mix });
            cb_Language.Items.AddRange(pairLanguages.ToArray());
            cb_Language.DisplayMember = "Text";
            cb_Language.ValueMember = "Value";
            cb_Language.SelectedItem = pairLanguages.FirstOrDefault(x => x.Value == LanguageEnum.Farsi);
            
            LoadPersianLanguage();
        }
        private void ComboBoxFonts_DrawItem(object sender, DrawItemEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var fontFamily = (FontFamily)comboBox.Items[e.Index];
            var font = new Font(fontFamily, comboBox.Font.SizeInPoints);

            e.DrawBackground();
            e.Graphics.DrawString(font.Name, font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
        }
        //**************************************************************************************
        //Variables:
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private List<ImageEntity> ImageEntities = new List<ImageEntity>();
        private Guid SelectedImageGuid = new Guid();
        const double MINIMUM_DESKEW_THRESHOLD = 0.05d;
        Image UpdatedBrightnessImage = null;
        Image UpdatedGamaImage = null;
        Image UpdatedContrastImage = null;
        Image UpdatedThresholdImage = null;
        protected float scaleX = 1f;
        protected float scaleY = 1f;
        private const float ZOOM_FACTOR = 1.25f;
        protected Point curScrollPos;
        protected bool IsFitImageSelected;
        private string UniqeID = Guid.NewGuid().ToString();
        private string InlineTempDirectory = "";
        private List<float> FontSizes = new List<float>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        //const int WM_NCHITTEST = 0x0084;
        //const int HTCLIENT = 1;
        //const int HTCAPTION = 2;
        List<PairEngine> pairEngines = new List<PairEngine>();
        List<PairPageSegmentationMode> pairPageSegmentationMode = new List<PairPageSegmentationMode>();
        List<PairLanguage> pairLanguages = new List<PairLanguage>();
        //**************************************************************************************
        #region Resize Windowsfrom 
        protected override void WndProc(ref Message m)
        {
            const UInt32 WM_NCHITTEST = 0x0084;
            const UInt32 WM_MOUSEMOVE = 0x0200;

            const UInt32 HTLEFT = 10;
            const UInt32 HTRIGHT = 11;
            const UInt32 HTBOTTOMRIGHT = 17;
            const UInt32 HTBOTTOM = 15;
            const UInt32 HTBOTTOMLEFT = 16;
            const UInt32 HTTOP = 12;
            const UInt32 HTTOPLEFT = 13;
            const UInt32 HTTOPRIGHT = 14;

            const int RESIZE_HANDLE_SIZE = 10;
            bool handled = false;
            if (m.Msg == WM_NCHITTEST || m.Msg == WM_MOUSEMOVE)
            {
                Size formSize = this.Size;
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point clientPoint = this.PointToClient(screenPoint);

                Dictionary<UInt32, Rectangle> boxes = new Dictionary<UInt32, Rectangle>() {
            {HTBOTTOMLEFT, new Rectangle(0, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTBOTTOM, new Rectangle(RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTBOTTOMRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE)},
            {HTTOPRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTTOP, new Rectangle(RESIZE_HANDLE_SIZE, 0, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTTOPLEFT, new Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTLEFT, new Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE) }
        };

                foreach (KeyValuePair<UInt32, Rectangle> hitBox in boxes)
                {
                    if (hitBox.Value.Contains(clientPoint))
                    {
                        m.Result = (IntPtr)hitBox.Key;
                        handled = true;
                        break;
                    }
                }
            }

            if (!handled)
                base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- use 0x20000
                return cp;
            }
        }
        #endregion
        //**************************************************************************************
        private void btn_loadFiles_Click_1(object sender, EventArgs e)
        {

        }

        private void btn_loadFiles_Click_2(object sender, EventArgs e)
        {

            OpenFileDialogForm();
            //ImageList imageList = new ImageList();
            //for (int i = 0; i < 2; i++)
            //{
            //    AddImage(new ImageEntity(new Bitmap(@"C:\Users\m.mirzaie\Desktop\_IM_8_8.jpg")) { Name = "asdf" });
            //    AddImage(new ImageEntity(new Bitmap(@"C:\Users\m.mirzaie\Desktop\IM_4.png")) { Name = "asdf" });
            //    AddImage(new ImageEntity(new Bitmap(@"C:\Users\m.mirzaie\Desktop\PTE.jpg")) { Name = "asdf" });

            //}
        }

        //**************************************************************************************
        private void RemoveImage(Guid guid)
        {
            ValidateImageIsSelected();
            var temp = imageListView.Items.Where(x => (Guid)x.VirtualItemKey == guid).FirstOrDefault();
            imageListView.Items.Remove(temp);
            ImageEntity imageEntity = ImageEntities.FirstOrDefault(x => x.Id == guid);
            ImageEntities.Remove(ImageEntities.FirstOrDefault(x => x.Id == guid));
            if (ImageEntities.Any())
            {
                SelectedImageGuid = ImageEntities.FirstOrDefault().Id;
                SetImage(ImageEntities.FirstOrDefault().GetImage());
            }
            else
            {
                SelectedImageGuid = new Guid();
                pb_MainPicture.Image = null;
            }


        }
        //**************************************************************************************
        private void RemoveFile(string fullFileName)
        {
            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
            }
        }

        //**************************************************************************************
        private void AddImage(ImageEntity imageEntity)
        {
            ImageEntities.Add(imageEntity);
            imageListView.Items.Add(imageEntity.Id, imageEntity.Name, imageEntity.GetImage());
        }
        //**************************************************************************************
        private void imageListView_SelectionChanged(object sender, EventArgs e)
        {
            ImageListView imageListView = (ImageListView)sender;

            if (!imageListView.SelectedItems.Any())
            {
                pb_MainPicture.Image = null;
                return;
            }
            var temp = imageListView.SelectedItems[0];
            SelectedImageGuid = (Guid)temp.VirtualItemKey;
            ImageEntity imageEntity = ImageEntities.FirstOrDefault(x => x.Id == SelectedImageGuid);
            SetImage(imageEntity.GetImage());
            rt_main.Text = imageEntity.RecognitionResult;
            FoomFit();
        }

        #region Moving windows
        private void p_header_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void p_header_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void p_header_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        #endregion

        #region Set color
        private Color RedColor()
        {
            Color color = Color.FromArgb(232, 17, 35);
            return color;
        }
        private Color BaseBlueColor()
        {
            Color color = Color.FromArgb(42, 87, 154);
            return color;
        }
        private Color HoverBlueColor()
        {
            Color color = Color.FromArgb(42, 87, 154);
            return color;
        }

        private void SetButtonColor(Button button, Color color)
        {
            button.BackColor = color;
        }
        #endregion

        #region set Enter and Leave color for title
        private void b_exit_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(b_exit, RedColor());
        }

        private void b_exit_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(b_exit, BaseBlueColor());
        }

        private void b_maximize_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(b_maximize, HoverBlueColor());

        }

        private void b_maximize_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(b_maximize, BaseBlueColor());
        }

        private void b_minimize_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(b_minimize, HoverBlueColor());
        }

        private void b_minimize_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(b_minimize, BaseBlueColor());
        }
        #endregion

        #region Click on base button
        private void b_exit_Click(object sender, EventArgs e)
        {
            this.Close();
            if (Directory.Exists(InlineTempDirectory))
                Directory.Delete(InlineTempDirectory, true);
        }

        private void b_maximize_Click(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Maximized;
            }
            else
                this.WindowState = FormWindowState.Normal;
            if (ImageIsSelected())
                FoomFit();
        }

        private void b_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }



        #endregion

        #region Enter to help menu    
        private void tp_help_Enter(object sender, EventArgs e)
        {
            //p_bodyBase.Visible = false;
            //p_bodyBaseLable.Visible = true;

        }

        private void tp_help_Leave(object sender, EventArgs e)
        {
            // p_bodyBase.Visible = true;
            //p_bodyBaseLable.Visible = false;
        }
        #endregion
        /// <summary>
        /// Gets an Image from Clipboard.
        /// </summary>
        /// <returns></returns>
        public static Image GetClipboardImage()
        {
            if (Clipboard.ContainsImage())
            {
                return Clipboard.GetImage();
            }
            return null;
        }

        private void b_backWard_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ImageEntity imageEntity = GetCurrentImageEntity();
            if (imageEntity != null)
            {
                SetImage(imageEntity.Backward());
            }
            this.Cursor = Cursors.Default;
        }
        private void b_forward_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ImageEntity imageEntity = GetCurrentImageEntity();
            if (imageEntity != null)
            {
                SetImage(imageEntity.ImageForward());
            }
            this.Cursor = Cursors.Default;
        }
        private void SetImage(Image image)
        {
            pb_MainPicture.Image = image;
            centerPicturebox();
            this.pb_MainPicture.Deselect();
        }
        private void SetImageWithPushToStack(Image image)
        {
            GetCurrentImageEntity().SetImage(image);
            SetImage(image);
        }
        private ImageEntity GetCurrentImageEntity()
        {
            ValidateImageIsSelected();
            return ImageEntities.FirstOrDefault(x => x.Id == SelectedImageGuid);
        }
        # region Other image update
        private void b_deskew_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            bool ischanged = false;
            Image image = ImageHelper.Deskew((Bitmap)GetCurrentImageEntity().GetImage(), MINIMUM_DESKEW_THRESHOLD, out ischanged);
            if (ischanged)
                SetImageWithPushToStack(image);
            this.Cursor = Cursors.Default;
        }
        //*********************************************************************************
        private void b_smooth_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var image = ImageHelper.GaussianBlur((Bitmap)GetCurrentImageEntity().GetImage());
            SetImageWithPushToStack(image);
            this.Cursor = Cursors.Default;
        }
        //*********************************************************************************
        private void b_sharpen_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var image = ImageHelper.Sharpen((Bitmap)GetCurrentImageEntity().GetImage());
            SetImageWithPushToStack(image);
            this.Cursor = Cursors.Default;
        }
        //*********************************************************************************
        private void b_invertColor_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var image = ImageHelper.InvertColor((Bitmap)GetCurrentImageEntity().GetImage());
            SetImageWithPushToStack(image);
            this.Cursor = Cursors.Default;
        }
        //*********************************************************************************
        private void b_monochrom_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var image = ImageHelper.ConvertMonochrome((Bitmap)GetCurrentImageEntity().GetImage());
            SetImageWithPushToStack(image);
            this.Cursor = Cursors.Default;
        }
        //*********************************************************************************
        private void b_grayScale_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var image = ImageHelper.ConvertGrayscale((Bitmap)GetCurrentImageEntity().GetImage());
            SetImageWithPushToStack(image);
            this.Cursor = Cursors.Default;
        }
        //*********************************************************************************
        #endregion
        #region Update Brightness
        //*********************************************************************************
        private void b_brightness_Click(object sender, EventArgs e)
        {
            TrackbarDialog dialog = new TrackbarDialog();
            dialog.LabelText = Properties.Strings.ImageBrightness;
            dialog.ValueUpdated += new TrackbarDialog.HandleValueChange(UpdatedBrightness);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SetImageWithPushToStack(UpdatedBrightnessImage);
            }
            else
            {
                SetImage(GetCurrentImageEntity().GetImage());
            }
        }
        //*********************************************************************************
        private void UpdatedBrightness(object sender, TrackbarDialog.ValueChangedEventArgs e)
        {
            UpdatedBrightnessImage = GetCurrentImageEntity().GetImage();
            Image image = ImageHelper.Brighten(GetCurrentImageEntity().GetImage(), e.NewValue * 0.005f);
            if (image != null)
            {
                UpdatedBrightnessImage = image;
                SetImage(image);
            }
        }
        //*********************************************************************************
        #endregion
        #region Gama
        private void b_gama_Click(object sender, EventArgs e)
        {
            TrackbarDialog dialog = new TrackbarDialog();
            dialog.SetForGamma();
            dialog.LabelText = Properties.Strings.Gamma;
            dialog.ValueUpdated += new TrackbarDialog.HandleValueChange(UpdatedGamma);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SetImageWithPushToStack(UpdatedGamaImage);
            }
            else
            {
                SetImage(GetCurrentImageEntity().GetImage());
            }

        }
        //*********************************************************************************
        private void UpdatedGamma(object sender, TrackbarDialog.ValueChangedEventArgs e)
        {
            UpdatedGamaImage = GetCurrentImageEntity().GetImage();
            Image image = ImageHelper.AdjustGamma(GetCurrentImageEntity().GetImage(), e.NewValue * 0.02f);
            if (image != null)
            {
                UpdatedGamaImage = image;
                SetImage(image);
            }
        }
        #endregion
        #region Contrast
        //*********************************************************************************
        private void b_contrast_Click(object sender, EventArgs e)
        {
            TrackbarDialog dialog = new TrackbarDialog();
            dialog.LabelText = Properties.Strings.Contrast;
            dialog.SetForContrast();
            dialog.ValueUpdated += new TrackbarDialog.HandleValueChange(UpdatedContrast);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SetImageWithPushToStack(UpdatedContrastImage);
            }
            else
            {
                SetImage(GetCurrentImageEntity().GetImage());
            }
        }
        //*********************************************************************************
        private void UpdatedContrast(object sender, TrackbarDialog.ValueChangedEventArgs e)
        {
            UpdatedContrastImage = GetCurrentImageEntity().GetImage();
            Image image = ImageHelper.Contrast(GetCurrentImageEntity().GetImage(), e.NewValue * 0.04f);
            if (image != null)
            {
                UpdatedContrastImage = image;
                SetImage(image);
            }
        }
        #endregion
        #region Threshold
        //*********************************************************************************
        private void b_threshold_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            TrackbarDialog dialog = new TrackbarDialog();
            dialog.SetForThreshold();
            dialog.LabelText = Properties.Strings.Threshold;
            dialog.ValueUpdated += new TrackbarDialog.HandleValueChange(UpdatedThreshold);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SetImageWithPushToStack(UpdatedThresholdImage);
            }
            else
            {
                SetImage(GetCurrentImageEntity().GetImage());
            }
            this.Cursor = Cursors.Default;
        }
        //*********************************************************************************
        private void UpdatedThreshold(object sender, TrackbarDialog.ValueChangedEventArgs e)
        {
            UpdatedThresholdImage = GetCurrentImageEntity().GetImage();
            Image image = ImageHelper.AdjustThreshold(GetCurrentImageEntity().GetImage(), e.NewValue * 0.01f);
            if (image != null)
            {
                UpdatedThresholdImage = image;
                SetImage(image);
            }
        }
        //*********************************************************************************
        #endregion
        #region Crop
        //*********************************************************************************
        private void b_crop_Click(object sender, EventArgs e)
        {
            Rectangle rect = this.pb_MainPicture.GetRect();

            if (rect == Rectangle.Empty)
            {
                return;
            }
            rect = new Rectangle((int)(rect.X * scaleX), (int)(rect.Y * scaleY), (int)(rect.Width * scaleX), (int)(rect.Height * scaleY));
            Image croppedImage = ImageHelper.Crop(GetCurrentImageEntity().GetImage(), rect);
            if (GetCurrentImageEntity().GetImage().PixelFormat == PixelFormat.Format8bppIndexed)
            {
                croppedImage = ImageHelper.ConvertGrayscale(croppedImage);
            }
            SetImageWithPushToStack(croppedImage);
        }
        #endregion
        #region Rotate
        //*********************************************************************************
        private void b_rotatrRight_Click(object sender, EventArgs e)
        {
            Image image = GetCurrentImageEntity().GetClonedImage();
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            SetImageWithPushToStack(image);
            AdjustPictureBoxAfterFlip();
        }
        //*********************************************************************************
        private void b_rotateLeft_Click(object sender, EventArgs e)
        {
            Image image = GetCurrentImageEntity().GetClonedImage();
            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            SetImageWithPushToStack(image);
            AdjustPictureBoxAfterFlip();
        }
        //*********************************************************************************
        private void AdjustPictureBoxAfterFlip()
        {
            this.pb_MainPicture.Size = new Size(this.pb_MainPicture.Height, this.pb_MainPicture.Width);
            this.pb_MainPicture.Refresh();
            // recalculate scale factors if in Fit Image mode
            if (IsFitImageSelected)
            {
                scaleX = (float)this.pb_MainPicture.Image.Width / (float)this.pb_MainPicture.Width;
                scaleY = (float)this.pb_MainPicture.Image.Height / (float)this.pb_MainPicture.Height;
            }
            this.centerPicturebox();
        }
        //*********************************************************************************
        #endregion

        #region Zooming
        protected void centerPicturebox()
        {
            this.sc_main.Panel1.AutoScrollPosition = Point.Empty;
            int x = 0;
            int y = 0;

            if (this.pb_MainPicture.Width < this.sc_main.Panel1.Width)
            {
                x = (this.sc_main.Panel1.Width - this.pb_MainPicture.Width) / 2;
            }

            if (this.pb_MainPicture.Height < this.sc_main.Panel1.Height)
            {
                y = (this.sc_main.Panel1.Height - this.pb_MainPicture.Height) / 2;
            }

            this.pb_MainPicture.Location = new Point(x, y);
            this.pb_MainPicture.Invalidate();
        }

        private void b_zoomIn_Click(object sender, EventArgs e)
        {
            this.pb_MainPicture.SizeMode = PictureBoxSizeMode.Zoom;
            this.pb_MainPicture.Width = Convert.ToInt32(this.pb_MainPicture.Width * ZOOM_FACTOR);
            this.pb_MainPicture.Height = Convert.ToInt32(this.pb_MainPicture.Height * ZOOM_FACTOR);
            scaleX = (float)this.pb_MainPicture.Image.Width / (float)this.pb_MainPicture.Width;
            scaleY = (float)this.pb_MainPicture.Image.Height / (float)this.pb_MainPicture.Height;
            this.centerPicturebox();
            IsFitImageSelected = false;
        }

        private void b_zoom_Click(object sender, EventArgs e)
        {
            FoomFit();
        }
        private void FoomFit()
        {
            ValidateImageIsSelected();
            curScrollPos = this.sc_main.Panel1.AutoScrollPosition;
            this.sc_main.Panel1.AutoScrollPosition = Point.Empty;
            this.pb_MainPicture.Dock = DockStyle.None;
            this.pb_MainPicture.SizeMode = PictureBoxSizeMode.Zoom;
            Size fitSize = fitImagetoContainer(this.pb_MainPicture.Image.Width, this.pb_MainPicture.Image.Height, this.sc_main.Panel1.Width, this.sc_main.Panel1.Height);
            this.pb_MainPicture.Width = fitSize.Width - 5;
            this.pb_MainPicture.Height = fitSize.Height - 5;
            setScale();
            this.centerPicturebox();
        }
        private bool ImageIsSelected()
        {
            return SelectedImageGuid != new Guid();
        }
        private void ValidateImageIsSelected()
        {
            if (!ImageIsSelected())
            {

                throw new BusinessException("Please Select Image", ExceptionType.SelectedImage);
            }
        }

        protected Size fitImagetoContainer(int w, int h, int maxWidth, int maxHeight)
        {
            float ratio = (float)w / h;

            w = maxWidth;
            h = (int)Math.Floor(maxWidth / ratio);

            if (h > maxHeight)
            {
                h = maxHeight;
                w = (int)Math.Floor(maxHeight * ratio);
            }

            return new Size(w, h);
        }
        protected void setScale()
        {
            scaleX = (float)this.pb_MainPicture.Image.Width / (float)this.pb_MainPicture.Width;
            scaleY = (float)this.pb_MainPicture.Image.Height / (float)this.pb_MainPicture.Height;
            if (scaleX > scaleY)
            {
                scaleY = scaleX;
            }
            else
            {
                scaleX = scaleY;
            }
        }

        private void b_realSize_Click(object sender, EventArgs e)
        {
            this.pb_MainPicture.Size = this.pb_MainPicture.Image.Size;
            this.pb_MainPicture.Dock = DockStyle.None;
            this.pb_MainPicture.SizeMode = PictureBoxSizeMode.Normal;
            scaleX = scaleY = 1f;
            this.centerPicturebox();
            this.sc_main.Panel1.AutoScrollPosition = new Point(Math.Abs(curScrollPos.X), Math.Abs(curScrollPos.Y));
            IsFitImageSelected = false;
        }

        private void b_zoomOut_Click(object sender, EventArgs e)
        {
            this.pb_MainPicture.SizeMode = PictureBoxSizeMode.Zoom;
            this.pb_MainPicture.Width = Convert.ToInt32(this.pb_MainPicture.Width / ZOOM_FACTOR);
            this.pb_MainPicture.Height = Convert.ToInt32(this.pb_MainPicture.Height / ZOOM_FACTOR);
            scaleX = (float)this.pb_MainPicture.Image.Width / (float)this.pb_MainPicture.Width;
            scaleY = (float)this.pb_MainPicture.Image.Height / (float)this.pb_MainPicture.Height;
            this.centerPicturebox();
            IsFitImageSelected = false;
        }
        #endregion
        #region Add and Remove Items
        //**************************************************************************************
        private void b_removePicture_Click(object sender, EventArgs e)
        {
            RemoveImage(GetCurrentImageEntity().Id);
        }
        //**************************************************************************************
        private void b_addPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialogForm();
        }
        //**************************************************************************************
        public void OpenFileDialogForm()
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image or PDF Files (*.bmp;*.jpg;*.jpeg,*.png,*.pdf)|*.BMP;*.JPG;*.JPEG;*.PNG;*.pdf";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ParsInputFile(openFileDialog);
                }
            }
        }
        //**************************************************************************************
        public void ParsInputFile(OpenFileDialog openFileDialog)
        {
            this.Cursor = Cursors.WaitCursor;
            for (int index = 0; index < openFileDialog.FileNames.Length; index++)
            {
                if (openFileDialog.SafeFileNames[index].EndsWith(".pdf"))
                {
                    ParsePDFFile(openFileDialog.FileNames[index], openFileDialog.SafeFileNames[index]);
                }
                else
                {
                    AddImage(new ImageEntity(new Bitmap(openFileDialog.FileNames[index])) { Name = openFileDialog.SafeFileNames[index] });
                }
            }
            if (!ImageEntities.Any()) return;
            SelectedImageGuid = ImageEntities.FirstOrDefault().Id;
            SetImage(ImageEntities.FirstOrDefault().GetImage());
            this.Cursor = Cursors.Default;
        }
        //**************************************************************************************
        public void ParsePDFFile(string fullPath, string Name)
        {
            SelectPagesForm selectPagesForm = new SelectPagesForm(Name);

            if (selectPagesForm.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                string pages = "";
                if (!selectPagesForm.cb_Allpages.Checked)
                {
                    pages = selectPagesForm.tb_pages.Text;
                }
                string error = "";
                string tempFilesDirectory = InlineTempDirectory + "." + DateTime.Now.Ticks;
                List<string> fileFullNames = PDFConvert.ConvertPdf2Png(fullPath, out error, tempFilesDirectory, 150, pages);
                foreach (var fileName in fileFullNames)
                {
                    var imageNames = fileName.Split('\\');
                    AddImage(new ImageEntity(new Bitmap(fileName)) { Name = imageNames[imageNames.Length - 1], FileFullName = fileName, FileType = FileType.PDF });
                }
            }
            else
            {
            }
            selectPagesForm.Close();

        }
        //**************************************************************************************
        #endregion
        #region chnage tich text box area
        #region Edit Editor
        private void b_alignRight_Click(object sender, EventArgs e)
        {
            rt_main.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void b_alingCenter_Click(object sender, EventArgs e)
        {
            rt_main.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void b_alignLeft_Click(object sender, EventArgs e)
        {
            rt_main.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void b_decreaseFontSize_Click(object sender, EventArgs e)
        {
            SetFontSize(rt_main.Font.Size - 1);
        }

        private void b_inceaseFontSize_Click(object sender, EventArgs e)
        {
            SetFontSize(rt_main.Font.Size + 1);
        }

        private void b_ToggleBullets_Click(object sender, EventArgs e)
        {
            this.rt_main.SelectionBullet = !this.rt_main.SelectionBullet;
        }

        private void b_IncreaceIndetation_Click(object sender, EventArgs e)
        {
            this.rt_main.SelectionIndent += 10;
        }

        private void b_decreaseIndentation_Click(object sender, EventArgs e)
        {
            this.rt_main.SelectionIndent -= 10;
        }

        private void b_undo_Click(object sender, EventArgs e)
        {
            if (this.rt_main.CanUndo)
                this.rt_main.Undo();
        }

        private void b_Redo_Click(object sender, EventArgs e)
        {
            if (this.rt_main.CanRedo)
                this.rt_main.Redo();
        }

        private void b_italic_Click(object sender, EventArgs e)
        {
            this.rt_main.SelectionFont = new Font(this.rt_main.SelectionFont.FontFamily, this.rt_main.SelectionFont.Size, this.ToggleFontStyle(this.rt_main.SelectionFont.Style, FontStyle.Italic));
        }

        private void b_bold_Click(object sender, EventArgs e)
        {
            this.rt_main.SelectionFont = new Font(this.rt_main.SelectionFont.FontFamily, this.rt_main.SelectionFont.Size, this.ToggleFontStyle(this.rt_main.SelectionFont.Style, FontStyle.Bold));
        }

        private void b_underLine_Click(object sender, EventArgs e)
        {
            this.rt_main.SelectionFont = new Font(this.rt_main.SelectionFont.FontFamily, this.rt_main.SelectionFont.Size, this.ToggleFontStyle(this.rt_main.SelectionFont.Style, FontStyle.Underline));
        }
        private FontStyle ToggleFontStyle(FontStyle item, FontStyle toggle)
        {
            return item ^ toggle;
        }
        #endregion
        #region Set Font family and size
        //**************************************************************************************
        private void cb_fontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            string fontFamily = (string)comboBox.SelectedItem;
            if (fontFamily == "")
                return;
            rt_main.Font = new Font(fontFamily, rt_main.Font.Size);
        }
        //**************************************************************************************
        private void cb_fontSize_TextChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            string fontSize = comboBox.Text;
            if (fontSize == "")
                return;
            float fontSizefloat = 0;
            try
            {
                fontSizefloat = float.Parse(fontSize);

            }
            catch (BusinessException ex)
            {
                throw new BusinessException("Invalid Font Value", ExceptionType.InvalidFontSize);
                return;
            }
            SetFontSize(fontSizefloat);

        }
        //**************************************************************************************
        private void SetFontSize(float fontSize)
        {
            cb_fontSize.Text = fontSize.ToString();
            this.rt_main.Font = new Font(this.rt_main.Font.FontFamily, fontSize, this.rt_main.Font.Style);
        }
        //**************************************************************************************
        #endregion
        #endregion
        #region Help Menu
        //**************************************************************************************

        //**************************************************************************************
        private void b_aboutUs_Click(object sender, EventArgs e)
        {
            AboutUs aboutUs = new AboutUs();
            aboutUs.Show(this);

        }

        //**************************************************************************************
        #endregion

        private async void B_processing_Click(object sender, EventArgs e)
        {

            l_processingText.Text = Properties.Strings.Processing;
            BinaOcr binaOcr = BinaOcr.Instance();
            ImageEntity imageEntity = GetCurrentImageEntity();
            PageSegmentationModeEnum pageSegmentationModeEnum = ((PairPageSegmentationMode)cb_structure.SelectedItem).Value;
            EngineModeEnum engineModeEnum = ((PairEngine)cb_engineMode.SelectedItem).Value;
            LanguageEnum languageEnum = ((PairLanguage)cb_Language.SelectedItem).Value;
            string result = await binaOcr.GetStringAsync((Bitmap)imageEntity.GetImage(), pageSegmentationModeEnum, languageEnum, engineModeEnum, cb_PostProcessing.Checked);
            SetRecognitionResult(result, imageEntity);
            l_processingText.Text = "";
            MessageBox.Show(Properties.Strings.ProcessingCompleted);


        }
        private void UpdateRichTextFont()
        {
            this.rt_main.Font = new Font(this.rt_main.Font.FontFamily, this.rt_main.Font.Size, this.rt_main.Font.Style);
        }
        private void SetRecognitionResult(string text,ImageEntity imageEntity=null)
        {
            if (imageEntity == null)
            {
                imageEntity = GetCurrentImageEntity();
            }
            imageEntity.SetRecognitionResult(text);
            if (GetCurrentImageEntity().Name == imageEntity.Name)
                rt_main.Text = text;
            UpdateRichTextFont();
        }

        private async void B_processingBulk_Click(object sender, EventArgs e)
        {
            l_processingText.Text = Properties.Strings.Processing;
            BinaOcr binaOcr = BinaOcr.Instance();
            PageSegmentationModeEnum pageSegmentationModeEnum = ((PairPageSegmentationMode)cb_structure.SelectedItem).Value;
            EngineModeEnum engineModeEnum = ((PairEngine)cb_engineMode.SelectedItem).Value;
            LanguageEnum languageEnum = ((PairLanguage)cb_Language.SelectedItem).Value;
            foreach (var image in ImageEntities)
            {
                l_processingText.Text = string.Format(Properties.Strings.ProcessingImageFormat, image.Name);
                string result = await binaOcr.GetStringAsync((Bitmap)image.GetImage(), pageSegmentationModeEnum, languageEnum, engineModeEnum, cb_PostProcessing.Checked);
                SetRecognitionResult(result, image);
            }
            l_processingText.Text = "";
            MessageBox.Show(Properties.Strings.BatchProcessingCompleted);
        }

        #region Save
        //************************************************************************************************************
        private void b_save_Click(object sender, EventArgs e)
        {
            var location = GetSaveWordFileLocation();
            BeforeAction(Properties.Strings.Saving);
            string fileName = location.FileName.Substring(0, location.FileName.LastIndexOf(".docx"));
            SaveResult(fileName, GetCurrentImageEntity().RecognitionResult, GetCurrentImageEntity().GetImage());
            AfterAction();
            MessageBox.Show(Properties.Strings.SaveCompleted);
        }
        //************************************************************************************************************
        private void b_saveAll_Click(object sender, EventArgs e)
        {
            var location = GetSaveWordFileDirectory();
            BeforeAction(Properties.Strings.Saving);
            foreach (var entity in ImageEntities)
            {
                string fileName = location.SelectedPath + "\\" + entity.NameWithoutExtention;
                SaveResult(fileName, entity.RecognitionResult, entity.GetImage());
            }
            AfterAction();
            MessageBox.Show(Properties.Strings.SaveCompleted);
        }
        //************************************************************************************************************
        private void SaveResult(string fileNameWithpath, string recognitionResult, Image image)
        {

            image.Save(fileNameWithpath + ".jpeg", ImageFormat.Jpeg);
            if (recognitionResult == null || recognitionResult == "")
            {
                return;
            }
            // Create a new document.
            using (var document = DocX.Create(fileNameWithpath + ".docx"))
            {
                document.SetDefaultFont(new Xceed.Document.NET.Font("B Nazanin"), 14d, Color.Black);
                document.PageBackground = Color.LightGray;
                //document.PageBorders = new Borders(new Border(BorderStyle.Tcbs_double, BorderSize.five, 20, Color.Blue));

                // Add a title
                //document.InsertParagraph("Formatted paragraphs").FontSize(15d).SpacingAfter(50d).Alignment = Xceed.Document.NET.Alignment.center;

                // Insert a Paragraph into this document.
                var p = document.InsertParagraph();

                // Append some text and add formatting.
                p.Append(recognitionResult)
                .Font(new Xceed.Document.NET.Font("B Nazanin"))
                .FontSize(14)
                .Color(Color.Black).SpacingAfter(40).Direction = Xceed.Document.NET.Direction.RightToLeft;

                // Insert another Paragraph into this document.


                // Save this document to disk.
                document.Save();
            }
        }
        //************************************************************************************************************
        private FolderBrowserDialog GetSaveWordFileDirectory()
        {
            FolderBrowserDialog brwsr = new FolderBrowserDialog();
            try
            {
                //Check to see if the user clicked the cancel button
                if (brwsr.ShowDialog() == DialogResult.OK)
                {
                    return brwsr;
                }
                throw new BusinessException("Please select valid directory.", ExceptionType.InvalidDirectory);
            }
            catch (BusinessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //************************************************************************************************************
        private SaveFileDialog GetSaveWordFileLocation()
        {
            try
            {
                var sfdSaveFile = new SaveFileDialog();
                sfdSaveFile.Filter = "Microsoft Word Files | *.docx";
                if (sfdSaveFile.ShowDialog() == DialogResult.OK)
                {
                    if (sfdSaveFile != null)
                    {
                        return sfdSaveFile;
                    }
                }
                throw new BusinessException("Please select valid file directory.", ExceptionType.InvalidFileDirectory);
            }
            catch (BusinessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //************************************************************************************************************
        #endregion
        //************************************************************************************************************
        private void BeforeAction(string message = "")
        {
            l_processingText.Text = message;
        }
        //************************************************************************************************************
        private void AfterAction(string message = "")
        {
            l_processingText.Text = message;
        }
        //************************************************************************************************************

        #region Load language
        //************************************************************************************************************
        private void cb_Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (refreshingUiLanguage || cb_Language.SelectedItem == null)
                return;

            var comboBox = (ComboBox)sender;
            LanguageEnum language = ((PairLanguage)comboBox.SelectedItem).Value;
            if (language == LanguageEnum.English)
            {
                LoadEnglishLanguage();
            }
            else if (language == LanguageEnum.Farsi)
            {
                LoadPersianLanguage();
            }
            else if (language == LanguageEnum.Mix)
            {
                LoadMixLanguage();
            }
        }
        //************************************************************************************************************
        private void LoadPersianLanguage()
        {
            cb_engineMode.Items.Clear();
            cb_engineMode.Items.AddRange(pairEngines.Where(x => x.Value == EngineModeEnum.KhanaDeepOnly || x.Value == EngineModeEnum.KhanaStructuralAndKhanaDeep).ToArray());
            cb_engineMode.DisplayMember = "Text";
            cb_engineMode.ValueMember = "Value";
            cb_engineMode.SelectedItem = pairEngines.FirstOrDefault(x => x.Value == EngineModeEnum.KhanaDeepOnly);

            rt_main.RightToLeft = RightToLeft.Yes;
        }
        //************************************************************************************************************
        private void LoadEnglishLanguage()
        {
            cb_engineMode.Items.Clear();
            cb_engineMode.Items.AddRange(pairEngines.ToArray());
            cb_engineMode.DisplayMember = "Text";
            cb_engineMode.ValueMember = "Value";
            cb_engineMode.SelectedItem = pairEngines.FirstOrDefault(x => x.Value == EngineModeEnum.KhanaDeepOnly);

            rt_main.RightToLeft = RightToLeft.No;
        }
        //************************************************************************************************************
        private void LoadMixLanguage()
        {
            cb_engineMode.Items.Clear();
            cb_engineMode.Items.AddRange(pairEngines.Where(x => x.Value == EngineModeEnum.KhanaDeepOnly).ToArray());
            cb_engineMode.DisplayMember = "Text";
            cb_engineMode.ValueMember = "Value";
            cb_engineMode.SelectedItem = pairEngines.FirstOrDefault(x => x.Value == EngineModeEnum.KhanaDeepOnly);

            rt_main.RightToLeft = RightToLeft.Yes;
        }
        //************************************************************************************************************
        #endregion
        //************************************************************************************************************
    }


}

