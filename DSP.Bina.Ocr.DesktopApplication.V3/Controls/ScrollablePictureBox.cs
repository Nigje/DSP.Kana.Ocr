using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DSP.Bina.Ocr.DesktopApplication.V3.Controls
{
    public partial class ScrollablePictureBox : PictureBox
    {
        private Point startPoint;
        private int selectionDragOffsetX, selectionDragOffsetY;
        private Rectangle selectionRectangle;
        private bool isCreatingSelection = false;
        private bool isMovingSelection;
        private bool isDragging;

        protected int frameWidth = 5;
        protected int minimumSelectionSize = 5;
        protected int dragStartX, dragStartY;
        protected bool resizeLeft, resizeTop, resizeRight, resizeBottom, moveSelection;
        private int initialSelectionX, initialSelectionY, initialSelectionWidth, initialSelectionHeight;
        private int selectionBorderOffset;
        private Point previousScrollPosition;

        public ScrollablePictureBox()
        {
            InitializeComponent();

            selectionRectangle = Rectangle.Empty;

            components ??= new Container();
            System.Windows.Forms.Timer selectionAnimationTimer = new System.Windows.Forms.Timer(components);
            selectionAnimationTimer.Tick += new EventHandler(SelectionAnimationTimer_Tick);

            // Sets the timer interval to .5 seconds.
            selectionAnimationTimer.Interval = 500;
            selectionAnimationTimer.Start();

        }
        // This is the method to run when the timer is raised.
        private void SelectionAnimationTimer_Tick(Object sender, EventArgs e)
        {
            if (selectionRectangle != Rectangle.Empty)
            {
                selectionBorderOffset += 3;

                if (selectionBorderOffset > 9)
                {
                    selectionBorderOffset = 0;
                }

                // redraw only the region
                this.Invalidate(new Rectangle(selectionRectangle.X, selectionRectangle.Y, selectionRectangle.Width + 1, selectionRectangle.Height + 1));
            }
        }

        public Rectangle GetSelectionRectangle()
        {
            return selectionRectangle;
        }

        public void Deselect()
        {
            startPoint = Point.Empty;
            selectionRectangle = Rectangle.Empty;
        }

        /// <summary>
        /// Segmented regions.
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Dictionary<Color, List<Rectangle>> SegmentedRegions
        { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            if (this.Image == null) return;

            // draw segmented regions
            if (SegmentedRegions != null)
            {

                Graphics g = (Graphics)pe.Graphics;

                foreach (Color color in SegmentedRegions.Keys)
                {
                    // Create pen
                    Pen pen = new Pen(color);

                    foreach (Rectangle region in SegmentedRegions[color])
                    {
                        g.DrawRectangle(pen, region);
                    }

                    pen.Dispose();
                }
            }

            if (selectionRectangle != Rectangle.Empty)
            {
                Graphics g = (Graphics)pe.Graphics;

                // Create pen
                Pen blackPen = new Pen(Color.Black);

                List<Rectangle> squares = createSquares(selectionRectangle);
                foreach (Rectangle square in squares)
                {
                    g.DrawRectangle(blackPen, square);
                }

                blackPen.DashCap = DashCap.Round;
                blackPen.LineJoin = LineJoin.Round;
                blackPen.MiterLimit = 0;
                blackPen.DashPattern = new float[] { 6, 6 };
                blackPen.DashOffset = selectionBorderOffset;

                try
                {
                    g.DrawRectangle(blackPen, selectionRectangle);
                }
                catch (OutOfMemoryException e)
                {
                    Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + selectionRectangle.ToString());
                }

                blackPen.Dispose();
            }

        }

        /// <summary>
        /// For picturebox's mousewheel support
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollablePictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (!this.Focused && this.FindForm().ContainsFocus)
            {
                previousScrollPosition = ((Panel)this.Parent).AutoScrollPosition;
            }
        }

        private void ScrollablePictureBox_GotFocus(object sender, EventArgs e)
        {
            ((Panel)this.Parent).AutoScrollPosition = new Point(Math.Abs(previousScrollPosition.X), Math.Abs(previousScrollPosition.Y));
        }

        /**
         * Creates grip squares.
         *
         */
        List<Rectangle> createSquares(Rectangle selectionRectangle)
        {
            List<Rectangle> ar = new List<Rectangle>();
            if (isMovingSelection)
            {
                return ar;
            }

            int wh = 6;

            int x = selectionRectangle.X - wh / 2;
            int y = selectionRectangle.Y - wh / 2;
            int w = selectionRectangle.Width;
            int h = selectionRectangle.Height;

            ar.Add(new Rectangle(x, y, wh, wh));
            ar.Add(new Rectangle(x + w / 2, y, wh, wh));
            ar.Add(new Rectangle(x + w, y, wh, wh));
            ar.Add(new Rectangle(x + w, y + h / 2, wh, wh));
            ar.Add(new Rectangle(x + w, y + h, wh, wh));
            ar.Add(new Rectangle(x + w / 2, y + h, wh, wh));
            ar.Add(new Rectangle(x, y + h, wh, wh));
            ar.Add(new Rectangle(x, y + h / 2, wh, wh));

            return ar;
        }

        private void ScrollablePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;

                if (selectionRectangle == Rectangle.Empty)
                {
                    startPoint = e.Location;
                    isCreatingSelection = true;
                }
                else
                {
                    initialSelectionX = selectionRectangle.X;
                    initialSelectionY = selectionRectangle.Y;
                    initialSelectionWidth = selectionRectangle.Width;
                    initialSelectionHeight = selectionRectangle.Height;

                    Rectangle leftFrame = new Rectangle(initialSelectionX, initialSelectionY, frameWidth, initialSelectionHeight);
                    Rectangle topFrame = new Rectangle(initialSelectionX, initialSelectionY, initialSelectionWidth, frameWidth);
                    Rectangle rightFrame = new Rectangle(initialSelectionX + initialSelectionWidth - frameWidth, initialSelectionY, frameWidth, initialSelectionHeight);
                    Rectangle bottomFrame = new Rectangle(initialSelectionX, initialSelectionY + initialSelectionHeight - frameWidth, initialSelectionWidth, frameWidth);

                    Point p = e.Location;

                    bool isInside = selectionRectangle.Contains(p);
                    bool isLeft = leftFrame.Contains(p);
                    bool isTop = topFrame.Contains(p);
                    bool isRight = rightFrame.Contains(p);
                    bool isBottom = bottomFrame.Contains(p);

                    if (isLeft && isTop)
                    {
                        resizeLeft = true;
                        resizeTop = true;
                        resizeRight = false;
                        resizeBottom = false;
                        moveSelection = false;
                    }
                    else if (isTop && isRight)
                    {
                        resizeLeft = false;
                        resizeTop = true;
                        resizeRight = true;
                        resizeBottom = false;
                        moveSelection = false;
                    }
                    else if (isRight && isBottom)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = true;
                        resizeBottom = true;
                        moveSelection = false;
                    }
                    else if (isBottom && isLeft)
                    {
                        resizeLeft = true;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = true;
                        moveSelection = false;
                    }
                    else if (isLeft)
                    {
                        resizeLeft = true;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = false;
                        moveSelection = false;
                    }
                    else if (isTop)
                    {
                        resizeLeft = false;
                        resizeTop = true;
                        resizeRight = false;
                        resizeBottom = false;
                        moveSelection = false;
                    }
                    else if (isRight)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = true;
                        resizeBottom = false;
                        moveSelection = false;
                    }
                    else if (isBottom)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = true;
                        moveSelection = false;
                    }
                    else if (isInside)
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = false;
                        moveSelection = true;
                    }
                    else
                    {
                        resizeLeft = false;
                        resizeTop = false;
                        resizeRight = false;
                        resizeBottom = false;
                        moveSelection = false;
                    }

                    int x = e.X;
                    int y = e.Y;

                    dragStartX = x;
                    dragStartY = y;

                    selectionDragOffsetX = selectionRectangle.X - dragStartX;
                    selectionDragOffsetY = selectionRectangle.Y - dragStartY;

                    if (!selectionRectangle.Contains(p))
                    {
                        startPoint = p;
                        isCreatingSelection = true;
                    }
                }
            }
        }

        private void ScrollablePictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectionRectangle != Rectangle.Empty)
                {
                    Rectangle testRect = new Rectangle(selectionRectangle.X - 1, selectionRectangle.Y - 1, selectionRectangle.Width + 2, selectionRectangle.Height + 2);
                    if (!testRect.Contains(e.Location))
                    {
                        Deselect();
                        this.Invalidate();
                    }
                }
            }
        }

        private void ScrollablePictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                HandleSelectionDrag(sender, e);
            }

            if (selectionRectangle != Rectangle.Empty)
            {
                int initialSelectionX = selectionRectangle.X;
                int initialSelectionY = selectionRectangle.Y;
                int initialSelectionWidth = selectionRectangle.Width;
                int initialSelectionHeight = selectionRectangle.Height;

                Rectangle leftFrame = new Rectangle(initialSelectionX, initialSelectionY, frameWidth, initialSelectionHeight);
                Rectangle topFrame = new Rectangle(initialSelectionX, initialSelectionY, initialSelectionWidth, frameWidth);
                Rectangle rightFrame = new Rectangle(initialSelectionX + initialSelectionWidth - frameWidth, initialSelectionY, frameWidth, initialSelectionHeight);
                Rectangle bottomFrame = new Rectangle(initialSelectionX, initialSelectionY + initialSelectionHeight - frameWidth, initialSelectionWidth, frameWidth);

                Point p = e.Location;

                bool isInside = selectionRectangle.Contains(p);
                bool isLeft = leftFrame.Contains(p);
                bool isTop = topFrame.Contains(p);
                bool isRight = rightFrame.Contains(p);
                bool isBottom = bottomFrame.Contains(p);

                if (isLeft && isTop)
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if (isTop && isRight)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (isRight && isBottom)
                {
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if (isBottom && isLeft)
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (isLeft)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (isTop)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (isRight)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (isBottom)
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (isInside)
                {
                    this.Cursor = Cursors.SizeAll;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ScrollablePictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;

                if (selectionRectangle != Rectangle.Empty)
                {
                    isMovingSelection = false;
                    isCreatingSelection = false;
                    this.Invalidate();
                }
            }

        }

        public void HandleSelectionDrag(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X;
                int y = e.Y;

                if (isCreatingSelection)
                {
                    selectionRectangle.X = Math.Min(startPoint.X, x);
                    selectionRectangle.Y = Math.Min(startPoint.Y, y);
                    selectionRectangle.Width = Math.Abs(x - startPoint.X);
                    selectionRectangle.Height = Math.Abs(y - startPoint.Y);
                    isMovingSelection = true;
                    this.Invalidate();
                }
                else
                {
                    int diffX = dragStartX - x;
                    int diffY = dragStartY - y;

                    if (resizeLeft)
                    {
                        selectionRectangle.X = initialSelectionX - diffX;
                        selectionRectangle.Width = initialSelectionWidth + diffX;
                    }
                    if (resizeTop)
                    {
                        selectionRectangle.Y = initialSelectionY - diffY;
                        selectionRectangle.Height = initialSelectionHeight + diffY;
                    }
                    if (resizeRight)
                    {
                        selectionRectangle.Width = initialSelectionWidth - diffX;
                    }
                    if (resizeBottom)
                    {
                        selectionRectangle.Height = initialSelectionHeight - diffY;
                    }
                    if (moveSelection)
                    {
                        isMovingSelection = true;
                        selectionRectangle.Location = new Point(selectionDragOffsetX + x, selectionDragOffsetY + y);
                    }

                    if (selectionRectangle.Width > minimumSelectionSize && selectionRectangle.Height > minimumSelectionSize)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

    }
}
