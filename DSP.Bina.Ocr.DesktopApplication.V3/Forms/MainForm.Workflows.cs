using Bina.Ocr.Wapper;
using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using DSP.Bina.Ocr.DesktopApplication.V3.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class MainForm
    {
        private readonly IOcrService ocrService;
        private readonly ImageImportService imageImportService = new ImageImportService();
        private readonly ImageEditService imageEditService = new ImageEditService();
        private readonly DocumentExportService documentExportService = new DocumentExportService();
        private readonly Dictionary<Control, bool> enabledBeforeProcessing = new Dictionary<Control, bool>();
        private CancellationTokenSource recognitionCancellation;
        private bool isProcessing, closeAfterProcessing, loadingEditor, resourcesReleased;
        private Button cancelRecognitionButton;
        private Font ownedEditorFont;

        private void ReplaceEditorFont(Font font)
        {
            Font previous = ownedEditorFont;
            recognizedTextBox.Font = font;
            ownedEditorFont = font;
            previous?.Dispose();
            RecordEditorState();
        }

        private void InitializeWorkflowControls()
        {
            recognizedTextBox.TextChanged += RecognizedTextBox_TextChanged;
            cancelRecognitionButton = new Button { Name = "cancelRecognitionButton", AutoSize = true, MinimumSize = new Size(100, 0), Dock = DockStyle.Right, Text = Properties.Strings.CancelProcessing, Visible = false };
            cancelRecognitionButton.Click += (_, _) => CancelRecognition();
            footerContentPanel.Controls.Add(cancelRecognitionButton);
            cancelRecognitionButton.SendToBack();
        }

        private void RecognizedTextBox_TextChanged(object sender, EventArgs e) => RecordEditorState();

        private void RecordEditorState()
        {
            if (loadingEditor || resourcesReleased) return;
            ImageEntity image = images.FirstOrDefault(item => item.Id == selectedImageId);
            if (image == null) return;
            image.RecognitionResult = recognizedTextBox.Text;
            image.RecognitionRtf = recognizedTextBox.Rtf;
            image.TextRightToLeft = recognizedTextBox.RightToLeft == RightToLeft.Yes;
        }

        private void LoadEditorState(ImageEntity image)
        {
            loadingEditor = true;
            try
            {
                recognizedTextBox.RightToLeft = image?.TextRightToLeft == true ? RightToLeft.Yes : RightToLeft.No;
                if (!string.IsNullOrEmpty(image?.RecognitionRtf)) recognizedTextBox.Rtf = image.RecognitionRtf;
                else recognizedTextBox.Text = image?.RecognitionResult ?? "";
                recognizedTextBox.ClearUndo();
            }
            finally { loadingEditor = false; }
        }

        private void RunImageEdit(ImageEdit edit)
        {
            EnsureNotProcessing();
            ImageEntity selected = GetSelectedImage();
            Cursor = Cursors.WaitCursor;
            try
            {
                imageEditService.Apply(selected, edit);
                SetImage(selected.GetImage());
                if (edit is ImageEdit.RotateLeft or ImageEdit.RotateRight) AdjustPictureBoxAfterFlip();
            }
            finally { Cursor = Cursors.Default; }
        }

        private void ShowImageAdjustment(ImageAdjustment adjustment)
        {
            EnsureNotProcessing();
            ImageEntity selected = GetSelectedImage();
            Image source = selected.GetImage(), preview = null;
            using var dialog = new TrackBarDialog();
            switch (adjustment)
            {
                case ImageAdjustment.Brightness: dialog.LabelText = Properties.Strings.ImageBrightness; break;
                case ImageAdjustment.Gamma: dialog.SetForGamma(); dialog.LabelText = Properties.Strings.Gamma; break;
                case ImageAdjustment.Contrast: dialog.SetForContrast(); dialog.LabelText = Properties.Strings.Contrast; break;
                case ImageAdjustment.Threshold: dialog.SetForThreshold(); dialog.LabelText = Properties.Strings.Threshold; break;
            }
            dialog.ValueUpdated += (_, args) =>
            {
                Image updated = imageEditService.Preview(source, adjustment, args.NewValue);
                if (updated == null || ReferenceEquals(updated, source)) return;
                Image previous = preview;
                preview = updated;
                SetImage(preview);
                previous?.Dispose();
            };
            try
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && preview != null) selected.SetImage(preview);
            }
            finally
            {
                SetImage(selected.GetImage());
                preview?.Dispose();
                Cursor = Cursors.Default;
            }
        }

        private void EnsureNotProcessing()
        {
            if (isProcessing) throw new BusinessException(Properties.Strings.ProcessingAlreadyRunning, ExceptionType.ProcessingInProgress);
        }

        private void CancelRecognition()
        {
            recognitionCancellation?.Cancel();
            if (isProcessing && !IsDisposed)
            {
                cancelRecognitionButton.Enabled = false;
                statusLabel.Text = Properties.Strings.CancelingProcessing;
            }
        }

        private void SetProcessingUi(bool processing)
        {
            if (processing)
            {
                foreach (Control control in new Control[] { recognitionToolbarPanel, imageProcessingTabPage, imageListToolbarPanel, imageListView })
                {
                    enabledBeforeProcessing[control] = control.Enabled;
                    control.Enabled = false;
                }
            }
            else
            {
                foreach (var state in enabledBeforeProcessing) state.Key.Enabled = state.Value;
                enabledBeforeProcessing.Clear();
            }
            recognizedTextBox.ReadOnly = processing;
            processingProgressBar.Style = ProgressBarStyle.Marquee;
            processingProgressBar.Visible = processing;
            cancelRecognitionButton.Enabled = processing;
            cancelRecognitionButton.Visible = processing;
            Cursor = processing ? Cursors.WaitCursor : Cursors.Default;
        }

        private async Task<bool> StartRecognitionAsync(bool allImages)
        {
            if (isProcessing) return false;
            EnsureImageSelected();
            RecordEditorState();
            ImageEntity[] pending = allImages ? images.ToArray() : new[] { GetSelectedImage() };
            var options = new OcrOptions(((PageSegmentationOption)pageSegmentationComboBox.SelectedItem).Value,
                ((OcrLanguageOption)ocrLanguageComboBox.SelectedItem).Value,
                ((OcrEngineOption)ocrEngineComboBox.SelectedItem).Value, postProcessingCheckBox.Checked);
            using var cancellation = new CancellationTokenSource();
            recognitionCancellation = cancellation;
            isProcessing = true;
            try
            {
                SetProcessingUi(true);
                foreach (ImageEntity image in pending)
                {
                    cancellation.Token.ThrowIfCancellationRequested();
                    statusLabel.Text = string.Format(Properties.Strings.ProcessingImageFormat, image.Name);
                    string text = await ocrService.RecognizeAsync((Bitmap)image.GetImage(), options, cancellation.Token);
                    cancellation.Token.ThrowIfCancellationRequested();
                    if (IsDisposed || resourcesReleased) return false;
                    image.TextRightToLeft = options.Language != LanguageEnum.English;
                    SetRecognitionResult(text, image);
                }
                return true;
            }
            catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { return false; }
            finally
            {
                recognitionCancellation = null;
                isProcessing = false;
                if (!IsDisposed && !resourcesReleased)
                {
                    SetProcessingUi(false);
                    statusLabel.Text = "";
                    if (closeAfterProcessing && IsHandleCreated) BeginInvoke(new Action(Close));
                }
            }
        }

        private async Task RunRecognitionAndNotifyAsync(bool allImages)
        {
            try
            {
                if (await StartRecognitionAsync(allImages))
                    MessageBox.Show(this, allImages ? Properties.Strings.BatchProcessingCompleted : Properties.Strings.ProcessingCompleted);
            }
            catch (Exception error)
            {
                if (!IsDisposed && !closeAfterProcessing) ApplicationErrorService.Show(error, this);
                else ApplicationErrorService.Log(error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isProcessing)
            {
                closeAfterProcessing = true;
                CancelRecognition();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ReleaseDesktopResources();
            base.OnFormClosed(e);
        }

        private void ReleaseDesktopResources()
        {
            if (resourcesReleased) return;
            resourcesReleased = true;
            recognitionCancellation?.Cancel();
            if (imagePreviewPictureBox != null) imagePreviewPictureBox.Image = null;
            imageListView?.Dispose();
            foreach (ImageEntity image in images) image.Dispose();
            images.Clear();
            imageImportService.Dispose();
            ownedEditorFont?.Dispose();
        }
    }
}
