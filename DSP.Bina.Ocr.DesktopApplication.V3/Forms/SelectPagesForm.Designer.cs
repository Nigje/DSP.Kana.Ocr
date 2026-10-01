namespace DSP.Bina.Ocr.DesktopApplication.V3.Forms
{
    partial class SelectPagesForm
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
            this.l_pagesDescription = new System.Windows.Forms.Label();
            this.tb_pages = new System.Windows.Forms.TextBox();
            this.b_loadSelectedPages = new System.Windows.Forms.Button();
            this.cb_Allpages = new System.Windows.Forms.CheckBox();
            this.p_bodyBase.SuspendLayout();
            this.SuspendLayout();
            // 
            // p_bodyBase
            // 
            this.p_bodyBase.Controls.Add(this.cb_Allpages);
            this.p_bodyBase.Controls.Add(this.b_loadSelectedPages);
            this.p_bodyBase.Controls.Add(this.tb_pages);
            this.p_bodyBase.Controls.Add(this.l_pagesDescription);
            this.p_bodyBase.Size = new System.Drawing.Size(598, 123);
            // 
            // l_pagesDescription
            // 
            this.l_pagesDescription.AutoSize = true;
            this.l_pagesDescription.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_pagesDescription.ForeColor = System.Drawing.Color.Black;
            this.l_pagesDescription.Location = new System.Drawing.Point(12, 27);
            this.l_pagesDescription.Name = "l_pagesDescription";
            this.l_pagesDescription.Size = new System.Drawing.Size(563, 14);
            this.l_pagesDescription.TabIndex = 0;
            this.l_pagesDescription.Text = Properties.Strings.PageSelectionInstructions;
            // 
            // tb_pages
            // 
            this.tb_pages.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_pages.Location = new System.Drawing.Point(295, 64);
            this.tb_pages.Name = "tb_pages";
            this.tb_pages.Size = new System.Drawing.Size(277, 21);
            this.tb_pages.TabIndex = 1;
            // 
            // b_loadSelectedPages
            // 
            this.b_loadSelectedPages.Font = new System.Drawing.Font("Tahoma", 9F);
            this.b_loadSelectedPages.ForeColor = System.Drawing.Color.Black;
            this.b_loadSelectedPages.Location = new System.Drawing.Point(41, 58);
            this.b_loadSelectedPages.Name = "b_loadSelectedPages";
            this.b_loadSelectedPages.Size = new System.Drawing.Size(104, 35);
            this.b_loadSelectedPages.TabIndex = 2;
            this.b_loadSelectedPages.Text = Properties.Strings.LoadSelectedPages;
            this.b_loadSelectedPages.UseVisualStyleBackColor = true;
            this.b_loadSelectedPages.Click += new System.EventHandler(this.B_loadSelectedPages_Click);
            // 
            // cb_Allpages
            // 
            this.cb_Allpages.AutoSize = true;
            this.cb_Allpages.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_Allpages.ForeColor = System.Drawing.Color.Black;
            this.cb_Allpages.Location = new System.Drawing.Point(151, 65);
            this.cb_Allpages.Name = "cb_Allpages";
            this.cb_Allpages.Size = new System.Drawing.Size(125, 18);
            this.cb_Allpages.TabIndex = 3;
            this.cb_Allpages.Text = Properties.Strings.SelectAllPages;
            this.cb_Allpages.UseVisualStyleBackColor = true;
            // 
            // SelectPagesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 190);
            this.Name = "SelectPagesForm";
            this.Text = "SelectPagesForm";
            this.p_bodyBase.ResumeLayout(false);
            this.p_bodyBase.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label l_pagesDescription;
        public System.Windows.Forms.TextBox tb_pages;
        private System.Windows.Forms.Button b_loadSelectedPages;
        public System.Windows.Forms.CheckBox cb_Allpages;
    }
}