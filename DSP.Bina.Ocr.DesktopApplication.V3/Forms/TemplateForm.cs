using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3
{
    public partial class TemplateForm : Form
    {
        private TemplateForm()
        {
            InitializeComponent();
        }
        public TemplateForm(Size size,string titleName)
        {
            InitializeComponent();
            if (size != null)
                this.Size = size;
            l_title.Text = titleName;
            this.CenterToScreen();
        }
        //**************************************************************************************
        //Variables:
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        //**************************************************************************************
        #region Resize Windowsfrom 
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
                Point screenPoint = new Point(m.LParam.ToInt32());
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
        //**************************************************************************************
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
        //**************************************************************************************
        private void b_exit_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void b_maximize_Click(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
            {
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Maximized;
            }
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void b_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #region set Enter and Leave color for title
        private void b_exit_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(b_exit, RedColor());
        }

        private void b_exit_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(b_exit, BaseBlueColor());
        }

        private void b_maximize_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(b_maximize, HoverBlueColor());

        }

        private void b_maximize_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(b_maximize, BaseBlueColor());
        }

        private void b_minimize_MouseEnter(object sender, EventArgs e)
        {
            SetButtonColor(b_minimize, HoverBlueColor());
        }

        private void b_minimize_MouseLeave(object sender, EventArgs e)
        {
            SetButtonColor(b_minimize, BaseBlueColor());
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
        private void p_header_right_MouseDown(object sender, MouseEventArgs e)
        {
            HeaderMouseDown();
        }

        private void p_header_right_MouseMove(object sender, MouseEventArgs e)
        {
            HeaderMouseMove();
        }

        private void p_header_right_MouseUp(object sender, MouseEventArgs e)
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
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }
        private void HeaderMouseMove()
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }
        private void HeaderMouseUp()
        {
            dragging = false;

        }
        #endregion
    }
}
