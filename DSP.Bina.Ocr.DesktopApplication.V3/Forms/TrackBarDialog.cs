using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class TrackBarDialog : Form
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
            }
        }

        private int previousValue;

        public delegate void HandleValueChange(object sender, ValueChangedEventArgs e);
        public event HandleValueChange ValueUpdated;

        public TrackBarDialog()
        {
            InitializeComponent();
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
