using DSP.Bina.Ocr.DesktopApplication.V3.Model;
using Manina.Windows.Forms;
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
    public partial class MainFormTemp : Form
    {
        public MainFormTemp()
        {
            InitializeComponent();
            imageListView.ThumbnailSize = new Size(150, 150);
        }
        //**************************************************************************************
        //Variables:
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private List<ImageEntity> ImageEntities = new List<ImageEntity>();
        //const int WM_NCHITTEST = 0x0084;
        //const int HTCLIENT = 1;
        //const int HTCAPTION = 2;
        //**************************************************************************************
        protected override void WndProc(ref Message m)
        {
            const int RESIZE_HANDLE_SIZE = 10;

            switch (m.Msg)
            {
                case 0x0084/*NCHITTEST*/ :
                    base.WndProc(ref m);

                    if ((int)m.Result == 0x01/*HTCLIENT*/)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);
                        if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)13/*HTTOPLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)12/*HTTOP*/ ;
                            else
                                m.Result = (IntPtr)14/*HTTOPRIGHT*/ ;
                        }
                        else if (clientPoint.Y <= (Size.Height - RESIZE_HANDLE_SIZE))
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)10/*HTLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)2/*HTCAPTION*/ ;
                            else
                                m.Result = (IntPtr)11/*HTRIGHT*/ ;
                        }
                        else
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)16/*HTBOTTOMLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)15/*HTBOTTOM*/ ;
                            else
                                m.Result = (IntPtr)17/*HTBOTTOMRIGHT*/ ;
                        }
                    }
                    return;
            }
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
        //**************************************************************************************
        private void btn_loadFiles_Click_1(object sender, EventArgs e)
        {

        }

        private void btn_loadFiles_Click_2(object sender, EventArgs e)
        {
            
           
            ImageList imageList = new ImageList();
            for (int i = 0; i < 2; i++)
            {
                AddImage(new ImageEntity(new Bitmap(@"C:\Users\m.mirzaie\Desktop\_IM_8_8.jpg")) { Name = "asdf"});
                AddImage(new ImageEntity(new Bitmap(@"C:\Users\m.mirzaie\Desktop\IM_4.png")) { Name = "asdf" });
                AddImage(new ImageEntity(new Bitmap(@"C:\Users\m.mirzaie\Desktop\PTE.jpg")) { Name = "asdf" });
                //Bitmap image = new Bitmap(@"C:\Users\m.mirzaie\Desktop\_IM_8_8.jpg");
                //imageListView.Items.Add("1", "2", image);
                // image = new Bitmap(@"C:\Users\m.mirzaie\Desktop\IM_4.png");
                //imageListView.Items.Add("1", "2", image);


            }
            


        }
        private void RemoveImage(Guid guid)
        {
            ImageEntities.Remove(ImageEntities.FirstOrDefault(x=>x.Id== guid));
            imageListView.Items.Remove(imageListView.Items.Where(x=> (Guid)x.VirtualItemKey== guid).FirstOrDefault());

        }
        private void AddImage(ImageEntity imageEntity)
        {
            ImageEntities.Add(imageEntity);
            imageListView.Items.Add(imageEntity.Id, imageEntity.Name, imageEntity.GetImage());
        }

        private void imageListView_SelectionChanged(object sender, EventArgs e)
        {
            ImageListView imageListView = (ImageListView)sender;
            var temp=imageListView.SelectedItems[0];
            ImageEntity imageEntity= ImageEntities.FirstOrDefault(x => x.Id == (Guid)temp.VirtualItemKey);
            pb_MainPicture.Image = imageEntity.GetImage();
            rt_main.Text = imageEntity.RecognitionResult;
        }

        #region Moving windows
        private void p_header_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void p_header_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void p_header_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
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

        private void SetButtonColor(Button button,Color color)
        {
            b_exit.BackColor = color;
        }
#endregion

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
            SetButtonColor(b_exit, BaseBlueColor());
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

        #region Click on base button
        private void b_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void b_maximize_Click(object sender, EventArgs e)
        {

            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void b_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }



        #endregion

        private void tp_help_Click(object sender, EventArgs e)
        {
     
        }
    }
}
