using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class BaseDialogForm : Form
    {
        private BaseDialogForm()
        {
            InitializeComponent();
        }
        public BaseDialogForm(Size size, string titleName)
        {
            InitializeComponent();
            this.Size = size;
            titleLabel.Text = titleName;
            this.CenterToScreen();
        }
        //Variables:
        private bool isDraggingWindow = false;
        private Point dragStartCursorPosition;
        private Point dragStartWindowPosition;
        #region Window resizing 
        protected override void WndProc(ref Message m)
        {
            const UInt32 WM_NCHITTEST = 0x0084;
            const UInt32 WM_MOUSEMOVE = 0x0200;

            const UInt32 HTLEFT = 10;
            const UInt32 HTRIGHT = 11;
            const UInt32 HTBOTTOMRIGHT = 17;
            const UInt32 HTBOTTOM = 15;
            const UInt32 HTBOTTOMLEFT = 16;
            const UInt32 HTTOP = 12;
            const UInt32 HTTOPLEFT = 13;
            const UInt32 HTTOPRIGHT = 14;

            const int RESIZE_HANDLE_SIZE = 10;
            bool handled = false;
            if (m.Msg == WM_NCHITTEST || m.Msg == WM_MOUSEMOVE)
            {
                Size formSize = this.Size;
                Point screenPoint = new Point(unchecked((int)m.LParam.ToInt64()));
                Point clientPoint = this.PointToClient(screenPoint);

                Dictionary<UInt32, Rectangle> boxes = new Dictionary<UInt32, Rectangle>() {
            {HTBOTTOMLEFT, new Rectangle(0, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTBOTTOM, new Rectangle(RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTBOTTOMRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE)},
            {HTRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE)},
            {HTTOPRIGHT, new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTTOP, new Rectangle(RESIZE_HANDLE_SIZE, 0, formSize.Width - 2*RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTTOPLEFT, new Rectangle(0, 0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE) },
            {HTLEFT, new Rectangle(0, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, formSize.Height - 2*RESIZE_HANDLE_SIZE) }
        };

                foreach (KeyValuePair<UInt32, Rectangle> hitBox in boxes)
                {
                    if (hitBox.Value.Contains(clientPoint))
                    {
                        m.Result = (IntPtr)hitBox.Key;
                        handled = true;
                        break;
                    }
                }
            }

            if (!handled)
                base.WndProc(ref m);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- use 0x20000
                return cp;
            }
        }
        #endregion
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void MaximizeButton_Click(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Maximized;
            }
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #region set Enter and Leave color for title
        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(closeButton, RedColor());
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(closeButton, BaseBlueColor());
        }

        private void MaximizeButton_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(maximizeButton, HoverBlueColor());

        }

        private void MaximizeButton_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(maximizeButton, BaseBlueColor());
        }

        private void MinimizeButton_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(minimizeButton, HoverBlueColor());
        }

        private void MinimizeButton_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(minimizeButton, BaseBlueColor());
        }

        #endregion
        #region Set color
        private Color RedColor()
        {
            Color color = Color.FromArgb(232, 17, 35);
            return color;
        }
        private Color BaseBlueColor()
        {
            Color color = Color.FromArgb(42, 87, 154);
            return color;
        }
        private Color HoverBlueColor()
        {
            Color color = Color.FromArgb(42, 87, 154);
            return color;
        }

        private void SetButtonColor(Button button, Color color)
        {
            button.BackColor = color;
        }
        #endregion
        #region Moving windows
        private void HeaderPanel_right_MouseDown(object sender, MouseEventArgs e)
        {
            HeaderMouseDown();
        }

        private void HeaderPanel_right_MouseMove(object sender, MouseEventArgs e)
        {
            HeaderMouseMove();
        }

        private void HeaderPanel_right_MouseUp(object sender, MouseEventArgs e)
        {
            HeaderMouseUp();
        }

        private void tlp_header_right_MouseUp(object sender, MouseEventArgs e)
        {
            HeaderMouseUp();
        }

        private void tlp_header_right_MouseMove(object sender, MouseEventArgs e)
        {
            HeaderMouseMove();
        }

        private void tlp_header_right_MouseDown(object sender, MouseEventArgs e)
        {
            HeaderMouseDown();
        }
        private void HeaderMouseDown()
        {
            isDraggingWindow = true;
            dragStartCursorPosition = Cursor.Position;
            dragStartWindowPosition = this.Location;
        }
        private void HeaderMouseMove()
        {
            if (isDraggingWindow)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragStartCursorPosition));
                this.Location = Point.Add(dragStartWindowPosition, new Size(dif));
            }
        }
        private void HeaderMouseUp()
        {
            isDraggingWindow = false;

        }
        #endregion
    }
}
