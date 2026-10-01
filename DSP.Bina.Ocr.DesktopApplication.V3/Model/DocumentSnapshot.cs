using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Model
{
    public sealed class DocumentSnapshot
    {
        public List<ParagraphSnapshot> Paragraphs { get; } = new List<ParagraphSnapshot>();
    }

    public sealed class ParagraphSnapshot
    {
        public List<TextRunSnapshot> Runs { get; } = new List<TextRunSnapshot>();
        public bool RightToLeft { get; set; }
        public HorizontalAlignment Alignment { get; set; }
        public bool Bullet { get; set; }
        public float IndentPoints { get; set; }
        public float HangingIndentPoints { get; set; }
    }

    public sealed record TextRunSnapshot(string Text, string FontName, float FontSize, FontStyle Style, Color Color);
}
