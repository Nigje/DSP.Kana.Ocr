namespace DSP.Bina.Ocr.DesktopApplication.V3.Forms
{
    partial class PdfPageSelectionForm
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
            this.pageRangeDescriptionLabel = new System.Windows.Forms.Label();
            this.pageRangeTextBox = new System.Windows.Forms.TextBox();
            this.loadSelectedPagesButton = new System.Windows.Forms.Button();
            this.allPagesCheckBox = new System.Windows.Forms.CheckBox();
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.allPagesCheckBox);
            this.contentPanel.Controls.Add(this.loadSelectedPagesButton);
            this.contentPanel.Controls.Add(this.pageRangeTextBox);
            this.contentPanel.Controls.Add(this.pageRangeDescriptionLabel);
            this.contentPanel.Size = new System.Drawing.Size(598, 123);
            // 
            // pageRangeDescriptionLabel
            // 
            this.pageRangeDescriptionLabel.AutoSize = true;
            this.pageRangeDescriptionLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageRangeDescriptionLabel.ForeColor = System.Drawing.Color.Black;
            this.pageRangeDescriptionLabel.Location = new System.Drawing.Point(12, 27);
            this.pageRangeDescriptionLabel.Name = "pageRangeDescriptionLabel";
            this.pageRangeDescriptionLabel.Size = new System.Drawing.Size(563, 14);
            this.pageRangeDescriptionLabel.TabIndex = 0;
            this.pageRangeDescriptionLabel.Text = Properties.Strings.PageSelectionInstructions;
            // 
            // pageRangeTextBox
            // 
            this.pageRangeTextBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageRangeTextBox.Location = new System.Drawing.Point(295, 64);
            this.pageRangeTextBox.Name = "pageRangeTextBox";
            this.pageRangeTextBox.Size = new System.Drawing.Size(277, 21);
            this.pageRangeTextBox.TabIndex = 1;
            // 
            // loadSelectedPagesButton
            // 
            this.loadSelectedPagesButton.Font = new System.Drawing.Font("Tahoma", 9F);
            this.loadSelectedPagesButton.ForeColor = System.Drawing.Color.Black;
            this.loadSelectedPagesButton.Location = new System.Drawing.Point(41, 58);
            this.loadSelectedPagesButton.Name = "loadSelectedPagesButton";
            this.loadSelectedPagesButton.Size = new System.Drawing.Size(104, 35);
            this.loadSelectedPagesButton.TabIndex = 2;
            this.loadSelectedPagesButton.Text = Properties.Strings.LoadSelectedPages;
            this.loadSelectedPagesButton.UseVisualStyleBackColor = true;
            this.loadSelectedPagesButton.Click += new System.EventHandler(this.LoadSelectedPagesButton_Click);
            // 
            // allPagesCheckBox
            // 
            this.allPagesCheckBox.AutoSize = true;
            this.allPagesCheckBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.allPagesCheckBox.ForeColor = System.Drawing.Color.Black;
            this.allPagesCheckBox.Location = new System.Drawing.Point(151, 65);
            this.allPagesCheckBox.Name = "allPagesCheckBox";
            this.allPagesCheckBox.Size = new System.Drawing.Size(125, 18);
            this.allPagesCheckBox.TabIndex = 3;
            this.allPagesCheckBox.Text = Properties.Strings.SelectAllPages;
            this.allPagesCheckBox.UseVisualStyleBackColor = true;
            // 
            // PdfPageSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 190);
            this.Name = "PdfPageSelectionForm";
            this.Text = "PdfPageSelectionForm";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label pageRangeDescriptionLabel;
        public System.Windows.Forms.TextBox pageRangeTextBox;
        private System.Windows.Forms.Button loadSelectedPagesButton;
        public System.Windows.Forms.CheckBox allPagesCheckBox;
    }
}