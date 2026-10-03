namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    partial class TrackBarDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            adjustmentTrackBar = new System.Windows.Forms.TrackBar();
            cancelButton = new System.Windows.Forms.Button();
            applyButton = new System.Windows.Forms.Button();
            adjustmentLabel = new System.Windows.Forms.Label();
            adjustmentLayout = new System.Windows.Forms.TableLayoutPanel();
            actionLayout = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)adjustmentTrackBar).BeginInit();
            contentPanel.SuspendLayout();
            adjustmentLayout.SuspendLayout();
            actionLayout.SuspendLayout();
            SuspendLayout();

            contentPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            contentPanel.Controls.Add(adjustmentLayout);
            adjustmentLayout.Name = "adjustmentLayout";
            adjustmentLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            adjustmentLayout.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            adjustmentLayout.ColumnCount = 1;
            adjustmentLayout.RowCount = 3;
            adjustmentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            adjustmentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            adjustmentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            adjustmentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            adjustmentLayout.Controls.Add(adjustmentLabel, 0, 0);
            adjustmentLayout.Controls.Add(adjustmentTrackBar, 0, 1);
            adjustmentLayout.Controls.Add(actionLayout, 0, 2);

            adjustmentLabel.Name = "adjustmentLabel";
            adjustmentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            adjustmentLabel.Font = new System.Drawing.Font("Tahoma", 10F);
            adjustmentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            adjustmentTrackBar.Name = "adjustmentTrackBar";
            adjustmentTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            adjustmentTrackBar.Minimum = -100;
            adjustmentTrackBar.Maximum = 100;
            adjustmentTrackBar.LargeChange = 10;
            adjustmentTrackBar.SmallChange = 5;
            adjustmentTrackBar.TickFrequency = 20;
            adjustmentTrackBar.TabIndex = 0;
            adjustmentTrackBar.ValueChanged += AdjustmentTrackBar_ValueChanged;

            actionLayout.Name = "actionLayout";
            actionLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            actionLayout.ColumnCount = 2;
            actionLayout.RowCount = 1;
            actionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            actionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            actionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            actionLayout.Controls.Add(applyButton, 0, 0);
            actionLayout.Controls.Add(cancelButton, 1, 0);

            applyButton.Name = "applyButton";
            applyButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            applyButton.AutoSize = true;
            applyButton.MinimumSize = new System.Drawing.Size(100, 32);
            applyButton.Font = new System.Drawing.Font("Tahoma", 9F);
            applyButton.Text = Properties.Strings.ApplyAdjustment;
            applyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            applyButton.UseVisualStyleBackColor = true;
            applyButton.TabIndex = 1;
            applyButton.Click += ApplyButton_Click;

            cancelButton.Name = "cancelButton";
            cancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            cancelButton.AutoSize = true;
            cancelButton.MinimumSize = new System.Drawing.Size(100, 32);
            cancelButton.Font = new System.Drawing.Font("Tahoma", 9F);
            cancelButton.Text = Properties.Strings.CancelAdjustment;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.TabIndex = 2;
            cancelButton.Click += CancelButton_Click;

            AcceptButton = applyButton;
            CancelButton = cancelButton;
            MinimumSize = new System.Drawing.Size(480, 240);
            ClientSize = new System.Drawing.Size(520, 260);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Name = "TrackBarDialog";
            ((System.ComponentModel.ISupportInitialize)adjustmentTrackBar).EndInit();
            actionLayout.ResumeLayout(false);
            actionLayout.PerformLayout();
            adjustmentLayout.ResumeLayout(false);
            contentPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.TrackBar adjustmentTrackBar;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label adjustmentLabel;
        private System.Windows.Forms.TableLayoutPanel adjustmentLayout;
        private System.Windows.Forms.TableLayoutPanel actionLayout;
    }
}
