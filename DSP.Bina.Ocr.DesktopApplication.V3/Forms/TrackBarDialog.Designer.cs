namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    partial class TrackBarDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.adjustmentTrackBar = new System.Windows.Forms.TrackBar();
            this.cancelButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.adjustmentLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.adjustmentTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // adjustmentTrackBar
            // 
            this.adjustmentTrackBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.adjustmentTrackBar.LargeChange = 10;
            this.adjustmentTrackBar.Location = new System.Drawing.Point(29, 39);
            this.adjustmentTrackBar.Maximum = 100;
            this.adjustmentTrackBar.Minimum = -100;
            this.adjustmentTrackBar.Name = "adjustmentTrackBar";
            this.adjustmentTrackBar.Size = new System.Drawing.Size(168, 45);
            this.adjustmentTrackBar.SmallChange = 5;
            this.adjustmentTrackBar.TabIndex = 1;
            this.adjustmentTrackBar.TickFrequency = 20;
            this.adjustmentTrackBar.ValueChanged += new System.EventHandler(this.AdjustmentTrackBar_ValueChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cancelButton.Location = new System.Drawing.Point(122, 88);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applyButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.applyButton.Location = new System.Drawing.Point(29, 88);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // adjustmentLabel
            // 
            this.adjustmentLabel.AutoSize = true;
            this.adjustmentLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.adjustmentLabel.Location = new System.Drawing.Point(39, 9);
            this.adjustmentLabel.Name = "adjustmentLabel";
            this.adjustmentLabel.Size = new System.Drawing.Size(33, 13);
            this.adjustmentLabel.TabIndex = 5;
            this.adjustmentLabel.Text = "Label";
            // 
            // TrackBarDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 123);
            this.Controls.Add(this.adjustmentLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.adjustmentTrackBar);
            this.Name = "TrackBarDialog";
            this.Text = "TrackBarDialog";
            ((System.ComponentModel.ISupportInitialize)(this.adjustmentTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar adjustmentTrackBar;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label adjustmentLabel;
    }
}