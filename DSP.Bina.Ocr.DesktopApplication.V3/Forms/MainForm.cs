using Bina.Ocr.Wapper;
using DSP.Bina.Ocr.DesktopApplication.V3.Forms;
using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using DSP.Bina.Ocr.DesktopApplication.V3.Services;
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class MainForm : Form
    {
        public MainForm() : this(new OcrService()) { }

        public MainForm(IOcrService ocrService)
        {
            this.ocrService = ocrService ?? throw new ArgumentNullException(nameof(ocrService));
            InitializeComponent();
            InitializeImageListView();
            InitializeDirectionalLayout();
            InitializeOcrOptions();
            InitializeUiLanguageSelector();
            this.CenterToScreen();
            InitializeWorkflowControls();
        }
        private ImageListView imageListView;

        private void InitializeImageListView()
        {
            // Initialize the legacy custom control outside designer-generated code.
            imageListView = new ImageListView
            {
                Name = nameof(imageListView),
                BackColor = Color.FromArgb(241, 241, 241),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 4, 0),
                ThumbnailSize = new Size(150, 150),
                TabIndex = 2
            };
            imageListView.SelectionChanged += ImageListView_SelectionChanged;
            imageListLayoutPanel.Controls.Add(imageListView, 0, 1);
        }

        [System.Reflection.ObfuscationAttribute(Exclude = true, ApplyToMembers = false)]
        private void InitializeOcrOptions()
        {
            versionLabel.Text = string.Format(Properties.Strings.BinaVersionFormat, System.Windows.Forms.Application.ProductVersion);
            foreach (FontFamily oneFontFamily in FontFamily.Families)
            {
                fontFamilyComboBox.Items.Add(oneFontFamily.Name);
            }
            fontFamilyComboBox.Text = this.recognizedTextBox.Font.Name.ToString();
            fontSizeComboBox.Text = this.recognizedTextBox.Font.Size.ToString();
            fontFamilyComboBox.SelectedItem = fontFamilyComboBox.Items.Cast<string>().FirstOrDefault(name => name == "B Mitra");
            foreach (float value in fontSizes)
            {
                fontSizeComboBox.Items.Add(value);
            }

            // Page segmentation modes.

            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutAutomaticWithOrientationAndScriptDetection, Value = PageSegmentationModeEnum.AutoOsd });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutWithoutOrientationAndScriptDetection, Value = PageSegmentationModeEnum.Auto });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutSingleColumn, Value = PageSegmentationModeEnum.SingleColumn });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutSingleVerticalTextBlock, Value = PageSegmentationModeEnum.SingleBlockVertText });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutSingleTextBlock, Value = PageSegmentationModeEnum.SingleBlock });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutSingleLine, Value = PageSegmentationModeEnum.SingleLine });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutCircularWord, Value = PageSegmentationModeEnum.CircleWord });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutSingleCharacter, Value = PageSegmentationModeEnum.SingleChar });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutSparseText, Value = PageSegmentationModeEnum.SparseText });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutSparseTextWithOrientationAndScriptDetection, Value = PageSegmentationModeEnum.SparseTextOsd });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutRawLine, Value = PageSegmentationModeEnum.RawLine });
            pageSegmentationOptions.Add(new PageSegmentationOption { Text = Properties.Strings.LayoutMultipleModes, Value = PageSegmentationModeEnum.Count });
            pageSegmentationComboBox.Items.AddRange(pageSegmentationOptions.ToArray());
            pageSegmentationComboBox.DisplayMember = "Text";
            pageSegmentationComboBox.ValueMember = "Value";
            pageSegmentationComboBox.SelectedItem = pageSegmentationOptions.FirstOrDefault(x => x.Value == PageSegmentationModeEnum.SingleBlock);

            // Engine Mode
            ocrEngineOptions.Add(new OcrEngineOption { Text = Properties.Strings.EngineDeep, Value = EngineModeEnum.KhanaDeepOnly });
            ocrEngineOptions.Add(new OcrEngineOption { Text = Properties.Strings.EngineStructuralAndDeep, Value = EngineModeEnum.KhanaStructuralAndKhanaDeep });
            ocrEngineOptions.Add(new OcrEngineOption { Text = Properties.Strings.EngineStructural, Value = EngineModeEnum.KhanaStructuralOnly });

            //Language
            ocrLanguageOptions.Add(new OcrLanguageOption { Text = Properties.Strings.LanguagePersian, Value = LanguageEnum.Farsi });
            ocrLanguageOptions.Add(new OcrLanguageOption { Text = Properties.Strings.LanguageEnglish, Value = LanguageEnum.English });
            ocrLanguageOptions.Add(new OcrLanguageOption { Text = Properties.Strings.LanguageMixed, Value = LanguageEnum.Mix });
            ocrLanguageComboBox.Items.AddRange(ocrLanguageOptions.ToArray());
            ocrLanguageComboBox.DisplayMember = "Text";
            ocrLanguageComboBox.ValueMember = "Value";
            ocrLanguageComboBox.SelectedItem = ocrLanguageOptions.FirstOrDefault(x => x.Value == LanguageEnum.Farsi);

            LoadPersianLanguage();
        }

        //Variables:
        private bool isDraggingWindow = false;
        private Point dragStartCursorPosition;
        private Point dragStartWindowPosition;
        private readonly List<ImageEntity> images = new List<ImageEntity>();
        private Guid selectedImageId = Guid.Empty;
        protected float scaleX = 1f;
        protected float scaleY = 1f;
        private const float ZoomFactor = 1.25f;
        protected Point previousScrollPosition;
        protected bool isImageFitToWindow;
        private readonly List<float> fontSizes = new List<float>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        private readonly List<OcrEngineOption> ocrEngineOptions = new List<OcrEngineOption>();
        private readonly List<PageSegmentationOption> pageSegmentationOptions = new List<PageSegmentationOption>();
        private readonly List<OcrLanguageOption> ocrLanguageOptions = new List<OcrLanguageOption>();
        #region Window resizing 
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
                Point screenPoint = new Point(unchecked((int)m.LParam.ToInt64()));
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

        private void LoadFilesButton_Click(object sender, EventArgs e)
        {

            ShowImportDialog();

        }

        private void RemoveImage(Guid imageId)
        {
            EnsureNotProcessing();
            RecordEditorState();
            ImageEntity image = images.FirstOrDefault(item => item.Id == imageId);
            if (image == null) return;
            var thumbnail = imageListView.Items.FirstOrDefault(item => (Guid)item.VirtualItemKey == imageId);
            selectedImageId = Guid.Empty;
            imagePreviewPictureBox.Image = null;
            images.Remove(image);
            if (thumbnail != null) imageListView.Items.Remove(thumbnail);
            image.Dispose();
            if (images.Count > 0)
            {
                selectedImageId = images[0].Id;
                SetImage(images[0].GetImage());
                LoadEditorState(images[0]);
                imageListView.Items[0].Selected = true;
            }
            else LoadEditorState(null);
        }

        private void AddImage(ImageEntity imageEntity)
        {
            EnsureNotProcessing();
            // The thumbnail cache takes ownership; it must not own the editable bitmap.
            if (imageEntity.RecognitionResult == null)
                imageEntity.TextRightToLeft = ((OcrLanguageOption)ocrLanguageComboBox.SelectedItem).Value != LanguageEnum.English;
            Image source = imageEntity.GetImage();
            float ratio = Math.Min(150f / source.Width, 150f / source.Height);
            Image thumbnail = source.GetThumbnailImage(Math.Max(1, (int)(source.Width * ratio)), Math.Max(1, (int)(source.Height * ratio)), null, IntPtr.Zero);
            try
            {
                images.Add(imageEntity);
                imageListView.Items.Add(imageEntity.Id, imageEntity.Name, thumbnail);
            }
            catch
            {
                images.Remove(imageEntity);
                thumbnail.Dispose();
                imageEntity.Dispose();
                throw;
            }
        }
        private void ImageListView_SelectionChanged(object sender, EventArgs e)
        {
            if (resourcesReleased) return;
            RecordEditorState();
            if (!imageListView.SelectedItems.Any())
            {
                selectedImageId = Guid.Empty;
                imagePreviewPictureBox.Image = null;
                LoadEditorState(null);
                return;
            }
            selectedImageId = (Guid)imageListView.SelectedItems[0].VirtualItemKey;
            ImageEntity image = images.FirstOrDefault(item => item.Id == selectedImageId);
            if (image == null) return;
            SetImage(image.GetImage());
            LoadEditorState(image);
            FitImageToWindow();
        }

        #region Moving windows
        private void HeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            isDraggingWindow = true;
            dragStartCursorPosition = Cursor.Position;
            dragStartWindowPosition = this.Location;
        }

        private void HeaderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingWindow)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragStartCursorPosition));
                this.Location = Point.Add(dragStartWindowPosition, new Size(dif));
            }
        }

        private void HeaderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDraggingWindow = false;
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
        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(closeButton, RedColor());
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(closeButton, BaseBlueColor());
        }

        private void MaximizeButton_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(maximizeButton, HoverBlueColor());

        }

        private void MaximizeButton_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(maximizeButton, BaseBlueColor());
        }

        private void MinimizeButton_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(minimizeButton, HoverBlueColor());
        }

        private void MinimizeButton_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(minimizeButton, BaseBlueColor());
        }
        #endregion

        #region Click on base button
        private void CloseButton_Click(object sender, EventArgs e) => Close();

        private void MaximizeButton_Click(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Maximized;
            }
            else
                this.WindowState = FormWindowState.Normal;
            if (HasSelectedImage())
                FitImageToWindow();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #endregion

        #region Enter to help menu    

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

        private void UndoImageButton_Click(object sender, EventArgs e)
        {
            EnsureNotProcessing();
            SetImage(GetSelectedImage().Undo());
        }
        private void RedoImageButton_Click(object sender, EventArgs e)
        {
            EnsureNotProcessing();
            SetImage(GetSelectedImage().Redo());
        }
        private void SetImage(Image image)
        {
            imagePreviewPictureBox.Image = image;
            CenterImagePreview();
            this.imagePreviewPictureBox.Deselect();
        }

        private ImageEntity GetSelectedImage()
        {
            EnsureImageSelected();
            return images.First(x => x.Id == selectedImageId);
        }
        # region Other image update
        private void DeskewButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.Deskew);
        private void SmoothButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.Smooth);
        private void SharpenButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.Sharpen);
        private void InvertColorsButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.Invert);
        private void MonochromeButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.Monochrome);
        private void GrayscaleButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.Grayscale);
        #endregion
        #region Update Brightness
        private void BrightnessButton_Click(object sender, EventArgs e) => ShowImageAdjustment(ImageAdjustment.Brightness);

        #endregion
        #region Gamma
        private void GammaButton_Click(object sender, EventArgs e) => ShowImageAdjustment(ImageAdjustment.Gamma);

        #endregion
        #region Contrast
        private void ContrastButton_Click(object sender, EventArgs e) => ShowImageAdjustment(ImageAdjustment.Contrast);

        #endregion
        #region Threshold
        private void ThresholdButton_Click(object sender, EventArgs e) => ShowImageAdjustment(ImageAdjustment.Threshold);

        #endregion
        #region Crop
        private void CropButton_Click(object sender, EventArgs e)
        {
            EnsureNotProcessing();
            ImageEntity image = GetSelectedImage();
            Rectangle selection = imagePreviewPictureBox.GetSelectionRectangle();
            if (selection == Rectangle.Empty) return;
            var crop = new Rectangle((int)(selection.X * scaleX), (int)(selection.Y * scaleY), (int)(selection.Width * scaleX), (int)(selection.Height * scaleY));
            imageEditService.Crop(image, crop);
            SetImage(image.GetImage());
        }
        #endregion
        #region Rotate
        private void RotateRightButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.RotateRight);
        private void RotateLeftButton_Click(object sender, EventArgs e) => RunImageEdit(ImageEdit.RotateLeft);
        private void AdjustPictureBoxAfterFlip()
        {
            this.imagePreviewPictureBox.Size = new Size(this.imagePreviewPictureBox.Height, this.imagePreviewPictureBox.Width);
            this.imagePreviewPictureBox.Refresh();
            // recalculate scale factors if in Fit Image mode
            if (isImageFitToWindow)
            {
                scaleX = (float)this.imagePreviewPictureBox.Image.Width / (float)this.imagePreviewPictureBox.Width;
                scaleY = (float)this.imagePreviewPictureBox.Image.Height / (float)this.imagePreviewPictureBox.Height;
            }
            this.CenterImagePreview();
        }
        #endregion

        #region Zooming
        protected void CenterImagePreview()
        {
            this.previewSplitContainer.Panel1.AutoScrollPosition = Point.Empty;
            int x = 0;
            int y = 0;

            if (this.imagePreviewPictureBox.Width < this.previewSplitContainer.Panel1.Width)
            {
                x = (this.previewSplitContainer.Panel1.Width - this.imagePreviewPictureBox.Width) / 2;
            }

            if (this.imagePreviewPictureBox.Height < this.previewSplitContainer.Panel1.Height)
            {
                y = (this.previewSplitContainer.Panel1.Height - this.imagePreviewPictureBox.Height) / 2;
            }

            this.imagePreviewPictureBox.Location = new Point(x, y);
            this.imagePreviewPictureBox.Invalidate();
        }

        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            EnsureImageSelected();
            this.imagePreviewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.imagePreviewPictureBox.Width = Convert.ToInt32(this.imagePreviewPictureBox.Width * ZoomFactor);
            this.imagePreviewPictureBox.Height = Convert.ToInt32(this.imagePreviewPictureBox.Height * ZoomFactor);
            scaleX = (float)this.imagePreviewPictureBox.Image.Width / (float)this.imagePreviewPictureBox.Width;
            scaleY = (float)this.imagePreviewPictureBox.Image.Height / (float)this.imagePreviewPictureBox.Height;
            this.CenterImagePreview();
            isImageFitToWindow = false;
        }

        private void FitImageButton_Click(object sender, EventArgs e)
        {
            FitImageToWindow();
        }
        private void FitImageToWindow()
        {
            EnsureImageSelected();
            previousScrollPosition = this.previewSplitContainer.Panel1.AutoScrollPosition;
            this.previewSplitContainer.Panel1.AutoScrollPosition = Point.Empty;
            this.imagePreviewPictureBox.Dock = DockStyle.None;
            this.imagePreviewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            Size fitSize = FitImageToContainer(this.imagePreviewPictureBox.Image.Width, this.imagePreviewPictureBox.Image.Height, this.previewSplitContainer.Panel1.Width, this.previewSplitContainer.Panel1.Height);
            this.imagePreviewPictureBox.Width = fitSize.Width - 5;
            this.imagePreviewPictureBox.Height = fitSize.Height - 5;
            SetImageScale();
            this.CenterImagePreview();
        }
        private bool HasSelectedImage()
        {
            return images.Any(image => image.Id == selectedImageId);
        }
        private void EnsureImageSelected()
        {
            if (!HasSelectedImage())
            {

                throw new BusinessException("Please Select Image", ExceptionType.SelectedImage);
            }
        }

        protected Size FitImageToContainer(int w, int h, int maxWidth, int maxHeight)
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
        protected void SetImageScale()
        {
            scaleX = (float)this.imagePreviewPictureBox.Image.Width / (float)this.imagePreviewPictureBox.Width;
            scaleY = (float)this.imagePreviewPictureBox.Image.Height / (float)this.imagePreviewPictureBox.Height;
            if (scaleX > scaleY)
            {
                scaleY = scaleX;
            }
            else
            {
                scaleX = scaleY;
            }
        }

        private void ActualSizeButton_Click(object sender, EventArgs e)
        {
            EnsureImageSelected();
            this.imagePreviewPictureBox.Size = this.imagePreviewPictureBox.Image.Size;
            this.imagePreviewPictureBox.Dock = DockStyle.None;
            this.imagePreviewPictureBox.SizeMode = PictureBoxSizeMode.Normal;
            scaleX = scaleY = 1f;
            this.CenterImagePreview();
            this.previewSplitContainer.Panel1.AutoScrollPosition = new Point(Math.Abs(previousScrollPosition.X), Math.Abs(previousScrollPosition.Y));
            isImageFitToWindow = false;
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            EnsureImageSelected();
            this.imagePreviewPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.imagePreviewPictureBox.Width = Convert.ToInt32(this.imagePreviewPictureBox.Width / ZoomFactor);
            this.imagePreviewPictureBox.Height = Convert.ToInt32(this.imagePreviewPictureBox.Height / ZoomFactor);
            scaleX = (float)this.imagePreviewPictureBox.Image.Width / (float)this.imagePreviewPictureBox.Width;
            scaleY = (float)this.imagePreviewPictureBox.Image.Height / (float)this.imagePreviewPictureBox.Height;
            this.CenterImagePreview();
            isImageFitToWindow = false;
        }
        #endregion
        #region Add and Remove Items
        private void RemoveImageButton_Click(object sender, EventArgs e)
        {
            RemoveImage(GetSelectedImage().Id);
        }
        private void AddImageButton_Click(object sender, EventArgs e)
        {
            ShowImportDialog();
        }
        public void ShowImportDialog()
        {
            EnsureNotProcessing();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = Properties.Strings.ImageAndPdfFilesFilter;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ImportFiles(openFileDialog);
                }
            }
        }
        public void ImportFiles(OpenFileDialog openFileDialog)
        {
            EnsureNotProcessing();
            Cursor = Cursors.WaitCursor;
            try
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (string.Equals(Path.GetExtension(filePath), ".pdf", StringComparison.OrdinalIgnoreCase))
                        ImportPdf(filePath, fileName);
                    else
                        AddImage(imageImportService.ImportImage(filePath));
                }
                if (!images.Any()) return;
                imageListView.Items[0].Selected = true;
            }
            finally { Cursor = Cursors.Default; }
        }
        public void ImportPdf(string filePath, string fileName)
        {
            EnsureNotProcessing();
            using var pageSelectionDialog = new PdfPageSelectionForm(fileName);
            if (pageSelectionDialog.ShowDialog(this) != DialogResult.OK) return;
            Cursor = Cursors.WaitCursor;
            try
            {
                string pages = pageSelectionDialog.allPagesCheckBox.Checked ? "" : pageSelectionDialog.pageRangeTextBox.Text;
                foreach (ImageEntity page in imageImportService.ImportPdf(filePath, pages)) AddImage(page);
            }
            finally { Cursor = Cursors.Default; }
        }
        #endregion
        #region Text formatting
        #region Edit Editor
        private void AlignRightButton_Click(object sender, EventArgs e)
        {
            recognizedTextBox.SelectionAlignment = HorizontalAlignment.Right;
            RecordEditorState();
        }

        private void AlignCenterButton_Click(object sender, EventArgs e)
        {
            recognizedTextBox.SelectionAlignment = HorizontalAlignment.Center;
            RecordEditorState();
        }

        private void AlignLeftButton_Click(object sender, EventArgs e)
        {
            recognizedTextBox.SelectionAlignment = HorizontalAlignment.Left;
            RecordEditorState();
        }

        private void DecreaseFontSizeButton_Click(object sender, EventArgs e)
        {
            SetFontSize(recognizedTextBox.Font.Size - 1);
        }

        private void IncreaseFontSizeButton_Click(object sender, EventArgs e)
        {
            SetFontSize(recognizedTextBox.Font.Size + 1);
        }

        private void ToggleBulletsButton_Click(object sender, EventArgs e)
        {
            this.recognizedTextBox.SelectionBullet = !this.recognizedTextBox.SelectionBullet;
            RecordEditorState();
        }

        private void IncreaseIndentButton_Click(object sender, EventArgs e)
        {
            this.recognizedTextBox.SelectionIndent += 10;
            RecordEditorState();
        }

        private void DecreaseIndentButton_Click(object sender, EventArgs e)
        {
            this.recognizedTextBox.SelectionIndent -= 10;
            RecordEditorState();
        }

        private void UndoTextButton_Click(object sender, EventArgs e)
        {
            if (this.recognizedTextBox.CanUndo)
                this.recognizedTextBox.Undo();
            RecordEditorState();
        }

        private void RedoTextButton_Click(object sender, EventArgs e)
        {
            if (this.recognizedTextBox.CanRedo)
                this.recognizedTextBox.Redo();
            RecordEditorState();
        }

        private void ItalicButton_Click(object sender, EventArgs e)
        {
            using Font selectionFont = recognizedTextBox.SelectionFont ?? (Font)recognizedTextBox.Font.Clone();
            using var updated = new Font(selectionFont.FontFamily, selectionFont.Size, ToggleFontStyle(selectionFont.Style, FontStyle.Italic));
            recognizedTextBox.SelectionFont = updated;
            RecordEditorState();
        }

        private void BoldButton_Click(object sender, EventArgs e)
        {
            using Font selectionFont = recognizedTextBox.SelectionFont ?? (Font)recognizedTextBox.Font.Clone();
            using var updated = new Font(selectionFont.FontFamily, selectionFont.Size, ToggleFontStyle(selectionFont.Style, FontStyle.Bold));
            recognizedTextBox.SelectionFont = updated;
            RecordEditorState();
        }

        private void UnderlineButton_Click(object sender, EventArgs e)
        {
            using Font selectionFont = recognizedTextBox.SelectionFont ?? (Font)recognizedTextBox.Font.Clone();
            using var updated = new Font(selectionFont.FontFamily, selectionFont.Size, ToggleFontStyle(selectionFont.Style, FontStyle.Underline));
            recognizedTextBox.SelectionFont = updated;
            RecordEditorState();
        }
        private FontStyle ToggleFontStyle(FontStyle item, FontStyle toggle)
        {
            return item ^ toggle;
        }
        #endregion
        #region Set Font family and size
        private void FontFamilyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            string fontFamily = (string)comboBox.SelectedItem;
            if (string.IsNullOrWhiteSpace(fontFamily))
                return;
            ReplaceEditorFont(new Font(fontFamily, recognizedTextBox.Font.Size, recognizedTextBox.Font.Style));
        }
        private void FontSizeComboBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fontSizeComboBox.Text)) return;
            if (!float.TryParse(fontSizeComboBox.Text, out float fontSize) || !float.IsFinite(fontSize) || fontSize <= 0)
                throw new BusinessException(Properties.Strings.InvalidFontSize, ExceptionType.InvalidFontSize);
            SetFontSize(fontSize);
        }
        private void SetFontSize(float fontSize)
        {
            if (!float.IsFinite(fontSize) || fontSize <= 0)
                throw new BusinessException(Properties.Strings.InvalidFontSize, ExceptionType.InvalidFontSize);
            ReplaceEditorFont(new Font(recognizedTextBox.Font.FontFamily, fontSize, recognizedTextBox.Font.Style));
            fontSizeComboBox.Text = fontSize.ToString();
        }
        #endregion
        #endregion
        #region Help Menu

        private void AboutUsButton_Click(object sender, EventArgs e)
        {
            AboutUsForm aboutUs = new AboutUsForm();
            aboutUs.Show(this);

        }

        #endregion

        private async void RecognizeButton_Click(object sender, EventArgs e) => await RunRecognitionAndNotifyAsync(false);

        private void SetRecognitionResult(string text, ImageEntity imageEntity = null)
        {
            imageEntity ??= GetSelectedImage();
            imageEntity.SetRecognitionResult(text);
            if (selectedImageId == imageEntity.Id) LoadEditorState(imageEntity);
        }

        private async void RecognizeAllButton_Click(object sender, EventArgs e) => await RunRecognitionAndNotifyAsync(true);

        #region Save
        private void SaveButton_Click(object sender, EventArgs e)
        {
            EnsureNotProcessing();
            RecordEditorState();
            ImageEntity selectedImage = GetSelectedImage();
            string filePath = SelectExportFilePath();
            if (filePath == null) return;
            statusLabel.Text = Properties.Strings.Saving;
            string exportedBasePath;
            try
            {
                exportedBasePath = documentExportService.Export(Path.ChangeExtension(filePath, null), selectedImage.GetImage(), RichTextDocumentService.Capture(selectedImage));
            }
            finally { statusLabel.Text = ""; }
            MessageBox.Show(Properties.Strings.SaveCompleted + Environment.NewLine + exportedBasePath);
        }
        private void SaveAllButton_Click(object sender, EventArgs e)
        {
            EnsureNotProcessing();
            EnsureImageSelected();
            RecordEditorState();
            string directory = SelectExportDirectory();
            if (directory == null) return;
            statusLabel.Text = Properties.Strings.Saving;
            try
            {
                foreach (ImageEntity image in images)
                    documentExportService.Export(Path.Combine(directory, image.NameWithoutExtension), image.GetImage(), RichTextDocumentService.Capture(image));
            }
            finally { statusLabel.Text = ""; }
            MessageBox.Show(Properties.Strings.SaveCompleted + Environment.NewLine + directory);
        }

        private string SelectExportDirectory()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog(this) == DialogResult.OK ? dialog.SelectedPath : null;
        }
        private string SelectExportFilePath()
        {
            using var dialog = new SaveFileDialog { Filter = Properties.Strings.WordFilesFilter, DefaultExt = "docx", AddExtension = true, OverwritePrompt = false };
            return dialog.ShowDialog(this) == DialogResult.OK ? dialog.FileName : null;
        }
        #endregion

        #region Load language
        private void OcrLanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (refreshingUiLanguage || ocrLanguageComboBox.SelectedItem == null)
                return;

            var comboBox = (ComboBox)sender;
            LanguageEnum language = ((OcrLanguageOption)comboBox.SelectedItem).Value;
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
                LoadMixedLanguage();
            }
        }
        private void LoadPersianLanguage()
        {
            ocrEngineComboBox.Items.Clear();
            ocrEngineComboBox.Items.AddRange(ocrEngineOptions.Where(x => x.Value == EngineModeEnum.KhanaDeepOnly || x.Value == EngineModeEnum.KhanaStructuralAndKhanaDeep).ToArray());
            ocrEngineComboBox.DisplayMember = "Text";
            ocrEngineComboBox.ValueMember = "Value";
            ocrEngineComboBox.SelectedItem = ocrEngineOptions.FirstOrDefault(x => x.Value == EngineModeEnum.KhanaDeepOnly);

            recognizedTextBox.RightToLeft = RightToLeft.Yes;
            RecordEditorState();
        }
        private void LoadEnglishLanguage()
        {
            ocrEngineComboBox.Items.Clear();
            ocrEngineComboBox.Items.AddRange(ocrEngineOptions.ToArray());
            ocrEngineComboBox.DisplayMember = "Text";
            ocrEngineComboBox.ValueMember = "Value";
            ocrEngineComboBox.SelectedItem = ocrEngineOptions.FirstOrDefault(x => x.Value == EngineModeEnum.KhanaDeepOnly);

            recognizedTextBox.RightToLeft = RightToLeft.No;
            RecordEditorState();
        }
        private void LoadMixedLanguage()
        {
            ocrEngineComboBox.Items.Clear();
            ocrEngineComboBox.Items.AddRange(ocrEngineOptions.Where(x => x.Value == EngineModeEnum.KhanaDeepOnly).ToArray());
            ocrEngineComboBox.DisplayMember = "Text";
            ocrEngineComboBox.ValueMember = "Value";
            ocrEngineComboBox.SelectedItem = ocrEngineOptions.FirstOrDefault(x => x.Value == EngineModeEnum.KhanaDeepOnly);

            recognizedTextBox.RightToLeft = RightToLeft.Yes;
            RecordEditorState();
        }
        #endregion
    }

}

