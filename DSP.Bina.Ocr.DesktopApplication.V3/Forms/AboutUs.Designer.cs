namespace DSP.Bina.Ocr.DesktopApplication.V3.Forms
{
    partial class AboutUs
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
            this.rtb_AboutUs = new System.Windows.Forms.RichTextBox();
            this.p_bodyBase.SuspendLayout();
            this.SuspendLayout();
            // 
            // p_bodyBase
            // 
            this.p_bodyBase.Controls.Add(this.rtb_AboutUs);
            // 
            // rtb_AboutUs
            // 
            this.rtb_AboutUs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtb_AboutUs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb_AboutUs.Font = new System.Drawing.Font("Tahoma", 9.25F);
            this.rtb_AboutUs.Location = new System.Drawing.Point(0, 0);
            this.rtb_AboutUs.Margin = new System.Windows.Forms.Padding(10);
            this.rtb_AboutUs.Name = "rtb_AboutUs";
            this.rtb_AboutUs.Size = new System.Drawing.Size(648, 333);
            this.rtb_AboutUs.TabIndex = 0;
            this.rtb_AboutUs.Text = Properties.Strings.AboutUsDescription;
            // 
            // AboutUs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 400);
            this.Name = "AboutUs";
            this.Text = "AboutUs";
            this.p_bodyBase.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb_AboutUs;
    }
}
