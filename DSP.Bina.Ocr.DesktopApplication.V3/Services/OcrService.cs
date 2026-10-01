using Bina.Ocr.Wapper;
using DSP.Khana.ImageTools.Models;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Services
{
    public sealed record OcrOptions(PageSegmentationModeEnum Layout, LanguageEnum Language,
        EngineModeEnum Engine, bool PostProcessing);

    public interface IOcrService
    {
        Task<string> RecognizeAsync(Bitmap image, OcrOptions options, CancellationToken cancellationToken);
    }

    public sealed class OcrService : IOcrService
    {
        // The native wrapper is shared by all windows; only one call may use it at a time.
        private static readonly SemaphoreSlim NativeGate = new SemaphoreSlim(1, 1);
        private readonly Func<Bitmap, OcrOptions, Task<string>> recognize;

        public OcrService() : this((image, options) => BinaOcr.Instance().GetStringAsync(image,
            options.Layout, options.Language, options.Engine, options.PostProcessing))
        { }

        public OcrService(Func<Bitmap, OcrOptions, Task<string>> recognize)
        {
            this.recognize = recognize ?? throw new ArgumentNullException(nameof(recognize));
        }

        public async Task<string> RecognizeAsync(Bitmap image, OcrOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using Bitmap snapshot = ImageHelper.CloneImage(image);
            await NativeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // The native API cannot interrupt a page. Await it before releasing its bitmap/gate.
                string text = await recognize(snapshot, options).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                return text;
            }
            finally { NativeGate.Release(); }
        }
    }
}
