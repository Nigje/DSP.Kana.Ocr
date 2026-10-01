namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class BaseDialogForm
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
            this.headerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.headerSpacerPanel = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.maximizeButton = new System.Windows.Forms.Button();
            this.minimizeButton = new System.Windows.Forms.Button();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.titleLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.footerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.tlp_Base.SuspendLayout();
            this.p_headerBase.SuspendLayout();
            this.headerLayoutPanel.SuspendLayout();
            this.headerSpacerPanel.SuspendLayout();
            this.titlePanel.SuspendLayout();
            this.titleLayoutPanel.SuspendLayout();
            this.footerPanel.SuspendLayout();
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
            this.p_headerBase.Controls.Add(this.headerLayoutPanel);
            this.p_headerBase.Dock = System.Windows.Forms.DockStyle.Top;
            this.p_headerBase.Location = new System.Drawing.Point(0, 0);
            this.p_headerBase.Margin = new System.Windows.Forms.Padding(0);
            this.p_headerBase.Name = "p_headerBase";
            this.p_headerBase.Size = new System.Drawing.Size(349, 40);
            this.p_headerBase.TabIndex = 1;
            // 
            // headerLayoutPanel
            // 
            this.headerLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.headerLayoutPanel.ColumnCount = 2;
            this.headerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.headerLayoutPanel.Controls.Add(this.headerSpacerPanel, 1, 0);
            this.headerLayoutPanel.Controls.Add(this.titlePanel, 0, 0);
            this.headerLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.headerLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayoutPanel.Name = "headerLayoutPanel";
            this.headerLayoutPanel.RowCount = 1;
            this.headerLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayoutPanel.Size = new System.Drawing.Size(349, 40);
            this.headerLayoutPanel.TabIndex = 0;
            // 
            // headerSpacerPanel
            // 
            this.headerSpacerPanel.Controls.Add(this.closeButton);
            this.headerSpacerPanel.Controls.Add(this.maximizeButton);
            this.headerSpacerPanel.Controls.Add(this.minimizeButton);
            this.headerSpacerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerSpacerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.headerSpacerPanel.Name = "headerSpacerPanel";
            this.headerSpacerPanel.Size = new System.Drawing.Size(145, 40);
            this.headerSpacerPanel.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.closeButton.Image = global::DSP.Bina.Ocr.DesktopApplication.V3.Properties.Resources.close_custom;
            this.closeButton.Location = new System.Drawing.Point(0, 0);
            this.closeButton.Margin = new System.Windows.Forms.Padding(0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(46, 31);
            this.closeButton.TabIndex = 3;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.CloseButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.CloseButton_MouseLeave);
            // 
            // maximizeButton
            // 
            this.maximizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.maximizeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.maximizeButton.Image = global::DSP.Bina.Ocr.DesktopApplication.V3.Properties.Resources.Maximize;
            this.maximizeButton.Location = new System.Drawing.Point(47, 0);
            this.maximizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.maximizeButton.Name = "maximizeButton";
            this.maximizeButton.Size = new System.Drawing.Size(46, 31);
            this.maximizeButton.TabIndex = 10;
            this.maximizeButton.UseVisualStyleBackColor = true;
            this.maximizeButton.Click += new System.EventHandler(this.MaximizeButton_Click);
            this.maximizeButton.MouseEnter += new System.EventHandler(this.MaximizeButton_MouseEnter);
            this.maximizeButton.MouseLeave += new System.EventHandler(this.MaximizeButton_MouseLeave);
            // 
            // minimizeButton
            // 
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.minimizeButton.Image = global::DSP.Bina.Ocr.DesktopApplication.V3.Properties.Resources.Minimize;
            this.minimizeButton.Location = new System.Drawing.Point(94, 0);
            this.minimizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(46, 31);
            this.minimizeButton.TabIndex = 5;
            this.minimizeButton.UseVisualStyleBackColor = true;
            this.minimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            this.minimizeButton.MouseEnter += new System.EventHandler(this.MinimizeButton_MouseEnter);
            this.minimizeButton.MouseLeave += new System.EventHandler(this.MinimizeButton_MouseLeave);
            // 
            // titlePanel
            // 
            this.titlePanel.Controls.Add(this.titleLayoutPanel);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titlePanel.Location = new System.Drawing.Point(145, 0);
            this.titlePanel.Margin = new System.Windows.Forms.Padding(0);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(204, 40);
            this.titlePanel.TabIndex = 1;
            this.titlePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HeaderPanel_right_MouseDown);
            this.titlePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HeaderPanel_right_MouseMove);
            this.titlePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HeaderPanel_right_MouseUp);
            // 
            // titleLayoutPanel
            // 
            this.titleLayoutPanel.ColumnCount = 2;
            this.titleLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.titleLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.titleLayoutPanel.Controls.Add(this.titleLabel, 0, 0);
            this.titleLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.titleLayoutPanel.Name = "titleLayoutPanel";
            this.titleLayoutPanel.RowCount = 1;
            this.titleLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.titleLayoutPanel.Size = new System.Drawing.Size(204, 40);
            this.titleLayoutPanel.TabIndex = 0;
            this.titleLayoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tlp_header_right_MouseDown);
            this.titleLayoutPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tlp_header_right_MouseMove);
            this.titleLayoutPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tlp_header_right_MouseUp);
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.titleLabel.Font = new System.Drawing.Font("Tahoma", 9.25F);
            this.titleLabel.Location = new System.Drawing.Point(107, 10);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.titleLabel.Size = new System.Drawing.Size(87, 20);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = Properties.Strings.ApplicationTitle;
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // footerPanel
            // 
            this.footerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(241)))), ((int)(((byte)(241)))));
            this.footerPanel.Controls.Add(this.footerLayoutPanel);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(1, 119);
            this.footerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(349, 25);
            this.footerPanel.TabIndex = 7;
            // 
            // footerLayoutPanel
            // 
            this.footerLayoutPanel.ColumnCount = 5;
            this.footerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.footerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.footerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.footerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.footerLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.footerLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.footerLayoutPanel.Name = "footerLayoutPanel";
            this.footerLayoutPanel.RowCount = 1;
            this.footerLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayoutPanel.Size = new System.Drawing.Size(349, 25);
            this.footerLayoutPanel.TabIndex = 0;
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.White;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(1, 41);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(349, 78);
            this.contentPanel.TabIndex = 8;
            // 
            // BaseDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
            this.ClientSize = new System.Drawing.Size(351, 145);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.tlp_Base);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BaseDialogForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = Properties.Strings.ApplicationTitle;
            this.tlp_Base.ResumeLayout(false);
            this.p_headerBase.ResumeLayout(false);
            this.headerLayoutPanel.ResumeLayout(false);
            this.headerSpacerPanel.ResumeLayout(false);
            this.titlePanel.ResumeLayout(false);
            this.titleLayoutPanel.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlp_Base;
        private System.Windows.Forms.Panel p_headerBase;
        private System.Windows.Forms.Button minimizeButton;
        private System.Windows.Forms.Button maximizeButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TableLayoutPanel headerLayoutPanel;
        private System.Windows.Forms.Panel headerSpacerPanel;
        private System.Windows.Forms.Panel titlePanel;
        protected System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TableLayoutPanel titleLayoutPanel;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.TableLayoutPanel footerLayoutPanel;
        public System.Windows.Forms.Panel contentPanel;
    }
}