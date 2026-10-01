namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class TemplateForm
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
            this.tlp_Base = new System.Windows.Forms.TableLayoutPanel();
            this.p_headerBase = new System.Windows.Forms.Panel();
            this.tlp_headerBase = new System.Windows.Forms.TableLayoutPanel();
            this.p_left = new System.Windows.Forms.Panel();
            this.b_exit = new System.Windows.Forms.Button();
            this.b_maximize = new System.Windows.Forms.Button();
            this.b_minimize = new System.Windows.Forms.Button();
            this.p_header_right = new System.Windows.Forms.Panel();
            this.tlp_header_right = new System.Windows.Forms.TableLayoutPanel();
            this.l_title = new System.Windows.Forms.Label();
            this.p_footerBase = new System.Windows.Forms.Panel();
            this.tlp_footerBase = new System.Windows.Forms.TableLayoutPanel();
            this.p_bodyBase = new System.Windows.Forms.Panel();
            this.tlp_Base.SuspendLayout();
            this.p_headerBase.SuspendLayout();
            this.tlp_headerBase.SuspendLayout();
            this.p_left.SuspendLayout();
            this.p_header_right.SuspendLayout();
            this.tlp_header_right.SuspendLayout();
            this.p_footerBase.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Base
            // 
            this.tlp_Base.BackColor = System.Drawing.Color.White;
            this.tlp_Base.ColumnCount = 1;
            this.tlp_Base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_Base.Controls.Add(this.p_headerBase, 0, 0);
            this.tlp_Base.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlp_Base.Location = new System.Drawing.Point(1, 1);
            this.tlp_Base.Name = "tlp_Base";
            this.tlp_Base.RowCount = 1;
            this.tlp_Base.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlp_Base.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlp_Base.Size = new System.Drawing.Size(349, 40);
            this.tlp_Base.TabIndex = 6;
            // 
            // p_headerBase
            // 
            this.p_headerBase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.p_headerBase.Controls.Add(this.tlp_headerBase);
            this.p_headerBase.Dock = System.Windows.Forms.DockStyle.Top;
            this.p_headerBase.Location = new System.Drawing.Point(0, 0);
            this.p_headerBase.Margin = new System.Windows.Forms.Padding(0);
            this.p_headerBase.Name = "p_headerBase";
            this.p_headerBase.Size = new System.Drawing.Size(349, 40);
            this.p_headerBase.TabIndex = 1;
            // 
            // tlp_headerBase
            // 
            this.tlp_headerBase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.tlp_headerBase.ColumnCount = 2;
            this.tlp_headerBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_headerBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tlp_headerBase.Controls.Add(this.p_left, 1, 0);
            this.tlp_headerBase.Controls.Add(this.p_header_right, 0, 0);
            this.tlp_headerBase.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlp_headerBase.Location = new System.Drawing.Point(0, 0);
            this.tlp_headerBase.Margin = new System.Windows.Forms.Padding(0);
            this.tlp_headerBase.Name = "tlp_headerBase";
            this.tlp_headerBase.RowCount = 1;
            this.tlp_headerBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_headerBase.Size = new System.Drawing.Size(349, 40);
            this.tlp_headerBase.TabIndex = 0;
            // 
            // p_left
            // 
            this.p_left.Controls.Add(this.b_exit);
            this.p_left.Controls.Add(this.b_maximize);
            this.p_left.Controls.Add(this.b_minimize);
            this.p_left.Location = new System.Drawing.Point(0, 0);
            this.p_left.Margin = new System.Windows.Forms.Padding(0);
            this.p_left.Name = "p_left";
            this.p_left.Size = new System.Drawing.Size(145, 40);
            this.p_left.TabIndex = 0;
            // 
            // b_exit
            // 
            this.b_exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_exit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.b_exit.Image = global::DSP.Bina.Ocr.DesktopApplication.V3.Properties.Resources.close_custom;
            this.b_exit.Location = new System.Drawing.Point(0, 0);
            this.b_exit.Margin = new System.Windows.Forms.Padding(0);
            this.b_exit.Name = "b_exit";
            this.b_exit.Size = new System.Drawing.Size(46, 31);
            this.b_exit.TabIndex = 3;
            this.b_exit.UseVisualStyleBackColor = true;
            this.b_exit.Click += new System.EventHandler(this.b_exit_Click);
            this.b_exit.MouseEnter += new System.EventHandler(this.b_exit_MouseEnter);
            this.b_exit.MouseLeave += new System.EventHandler(this.b_exit_MouseLeave);
            // 
            // b_maximize
            // 
            this.b_maximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_maximize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.b_maximize.Image = global::DSP.Bina.Ocr.DesktopApplication.V3.Properties.Resources.miximize;
            this.b_maximize.Location = new System.Drawing.Point(47, 0);
            this.b_maximize.Margin = new System.Windows.Forms.Padding(0);
            this.b_maximize.Name = "b_maximize";
            this.b_maximize.Size = new System.Drawing.Size(46, 31);
            this.b_maximize.TabIndex = 10;
            this.b_maximize.UseVisualStyleBackColor = true;
            this.b_maximize.Click += new System.EventHandler(this.b_maximize_Click);
            this.b_maximize.MouseEnter += new System.EventHandler(this.b_maximize_MouseEnter);
            this.b_maximize.MouseLeave += new System.EventHandler(this.b_maximize_MouseLeave);
            // 
            // b_minimize
            // 
            this.b_minimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_minimize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.b_minimize.Image = global::DSP.Bina.Ocr.DesktopApplication.V3.Properties.Resources.Minimize;
            this.b_minimize.Location = new System.Drawing.Point(94, 0);
            this.b_minimize.Margin = new System.Windows.Forms.Padding(0);
            this.b_minimize.Name = "b_minimize";
            this.b_minimize.Size = new System.Drawing.Size(46, 31);
            this.b_minimize.TabIndex = 5;
            this.b_minimize.UseVisualStyleBackColor = true;
            this.b_minimize.Click += new System.EventHandler(this.b_minimize_Click);
            this.b_minimize.MouseEnter += new System.EventHandler(this.b_minimize_MouseEnter);
            this.b_minimize.MouseLeave += new System.EventHandler(this.b_minimize_MouseLeave);
            // 
            // p_header_right
            // 
            this.p_header_right.Controls.Add(this.tlp_header_right);
            this.p_header_right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_header_right.Location = new System.Drawing.Point(145, 0);
            this.p_header_right.Margin = new System.Windows.Forms.Padding(0);
            this.p_header_right.Name = "p_header_right";
            this.p_header_right.Size = new System.Drawing.Size(204, 40);
            this.p_header_right.TabIndex = 1;
            this.p_header_right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.p_header_right_MouseDown);
            this.p_header_right.MouseMove += new System.Windows.Forms.MouseEventHandler(this.p_header_right_MouseMove);
            this.p_header_right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.p_header_right_MouseUp);
            // 
            // tlp_header_right
            // 
            this.tlp_header_right.ColumnCount = 2;
            this.tlp_header_right.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlp_header_right.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_header_right.Controls.Add(this.l_title, 0, 0);
            this.tlp_header_right.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlp_header_right.Location = new System.Drawing.Point(0, 0);
            this.tlp_header_right.Name = "tlp_header_right";
            this.tlp_header_right.RowCount = 1;
            this.tlp_header_right.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_header_right.Size = new System.Drawing.Size(204, 40);
            this.tlp_header_right.TabIndex = 0;
            this.tlp_header_right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlp_header_right_MouseDown);
            this.tlp_header_right.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlp_header_right_MouseMove);
            this.tlp_header_right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlp_header_right_MouseUp);
            // 
            // l_title
            // 
            this.l_title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.l_title.Font = new System.Drawing.Font("Tahoma", 9.25F);
            this.l_title.Location = new System.Drawing.Point(107, 10);
            this.l_title.Margin = new System.Windows.Forms.Padding(10);
            this.l_title.Name = "l_title";
            this.l_title.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.l_title.Size = new System.Drawing.Size(87, 20);
            this.l_title.TabIndex = 1;
            this.l_title.Text = Properties.Strings.ApplicationTitle;
            this.l_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // p_footerBase
            // 
            this.p_footerBase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.p_footerBase.Controls.Add(this.tlp_footerBase);
            this.p_footerBase.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_footerBase.Location = new System.Drawing.Point(1, 119);
            this.p_footerBase.Margin = new System.Windows.Forms.Padding(0);
            this.p_footerBase.Name = "p_footerBase";
            this.p_footerBase.Size = new System.Drawing.Size(349, 25);
            this.p_footerBase.TabIndex = 7;
            // 
            // tlp_footerBase
            // 
            this.tlp_footerBase.ColumnCount = 5;
            this.tlp_footerBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlp_footerBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tlp_footerBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_footerBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tlp_footerBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlp_footerBase.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tlp_footerBase.Location = new System.Drawing.Point(0, 0);
            this.tlp_footerBase.Margin = new System.Windows.Forms.Padding(0);
            this.tlp_footerBase.Name = "tlp_footerBase";
            this.tlp_footerBase.RowCount = 1;
            this.tlp_footerBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_footerBase.Size = new System.Drawing.Size(349, 25);
            this.tlp_footerBase.TabIndex = 0;
            // 
            // p_bodyBase
            // 
            this.p_bodyBase.BackColor = System.Drawing.Color.White;
            this.p_bodyBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.p_bodyBase.Location = new System.Drawing.Point(1, 41);
            this.p_bodyBase.Name = "p_bodyBase";
            this.p_bodyBase.Size = new System.Drawing.Size(349, 78);
            this.p_bodyBase.TabIndex = 8;
            // 
            // TemplateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.ClientSize = new System.Drawing.Size(351, 145);
            this.Controls.Add(this.p_bodyBase);
            this.Controls.Add(this.p_footerBase);
            this.Controls.Add(this.tlp_Base);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TemplateForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = Properties.Strings.ApplicationTitle;
            this.tlp_Base.ResumeLayout(false);
            this.p_headerBase.ResumeLayout(false);
            this.tlp_headerBase.ResumeLayout(false);
            this.p_left.ResumeLayout(false);
            this.p_header_right.ResumeLayout(false);
            this.tlp_header_right.ResumeLayout(false);
            this.p_footerBase.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlp_Base;
        private System.Windows.Forms.Panel p_headerBase;
        private System.Windows.Forms.Button b_minimize;
        private System.Windows.Forms.Button b_maximize;
        private System.Windows.Forms.Button b_exit;
        private System.Windows.Forms.TableLayoutPanel tlp_headerBase;
        private System.Windows.Forms.Panel p_left;
        private System.Windows.Forms.Panel p_header_right;
        protected System.Windows.Forms.Label l_title;
        private System.Windows.Forms.TableLayoutPanel tlp_header_right;
        private System.Windows.Forms.Panel p_footerBase;
        private System.Windows.Forms.TableLayoutPanel tlp_footerBase;
        public System.Windows.Forms.Panel p_bodyBase;
    }
}