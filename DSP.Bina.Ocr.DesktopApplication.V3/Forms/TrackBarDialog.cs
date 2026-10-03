using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class TrackBarDialog : BaseDialogForm
    {
        public class ValueChangedEventArgs : EventArgs
        {
            public float NewValue
            {
                get;
                set;
            }

            public ValueChangedEventArgs(float value)
                : base()
            {
                this.NewValue = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LabelText
        {
            set
            {
                this.adjustmentLabel.Text = value;
                titleLabel.Text = value;
                Text = value;
            }
        }

        private int previousValue;

        public delegate void HandleValueChange(object sender, ValueChangedEventArgs e);
        public event HandleValueChange ValueUpdated;

        public TrackBarDialog() : base(new Size(520, 260), Properties.Strings.ImageProcessingTab)
        {
            InitializeComponent();
            var culture = Properties.Strings.Culture ?? Thread.CurrentThread.CurrentUICulture;
            RightToLeft = culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            RightToLeftLayout = culture.TextInfo.IsRightToLeft;
            titleLabel.RightToLeft = RightToLeft;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = culture.TextInfo.IsRightToLeft ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            // Give the inherited heading all available space instead of its fixed designer column.
            var titleLayout = (TableLayoutPanel)titleLabel.Parent;
            titleLayout.ColumnStyles[0].SizeType = SizeType.Percent;
            titleLayout.ColumnStyles[0].Width = 100;
            titleLayout.ColumnStyles[1].SizeType = SizeType.Absolute;
            titleLayout.ColumnStyles[1].Width = 0;
            LabelText = Properties.Strings.ImageProcessingTab;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AdjustmentTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (this.ValueUpdated != null)
            {
                TrackBar bar = (TrackBar)sender;

                //reduce # of unnecessary value changed events
                if (Math.Abs(bar.Value - previousValue) >= bar.SmallChange)
                {
                    previousValue = bar.Value;
                    ValueChangedEventArgs args = new ValueChangedEventArgs(bar.Value);
                    this.ValueUpdated(this, args);
                }
            }
        }

        public void SetForContrast()
        {
            this.adjustmentTrackBar.Minimum = 5;
            this.adjustmentTrackBar.Value = 25;
            this.adjustmentTrackBar.TickFrequency = 10;
        }

        public void SetForGamma()
        {
            this.adjustmentTrackBar.Minimum = 0;
            this.adjustmentTrackBar.Value = 50;
            this.adjustmentTrackBar.TickFrequency = 10;
        }

        public void SetForThreshold()
        {
            this.adjustmentTrackBar.Minimum = 0;
            this.adjustmentTrackBar.Value = 50;
            this.adjustmentTrackBar.TickFrequency = 10;
        }

    }

}
