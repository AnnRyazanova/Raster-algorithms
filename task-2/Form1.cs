using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace task_2
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private bool needDrawing = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "images | *.png ; *.jpg";
            if (DialogResult.OK != dialog.ShowDialog()) return;
            var b = new Bitmap(dialog.FileName);
            var good_b = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);
            graphics = Graphics.FromImage(good_b);
            graphics.DrawImage(b, 0, 0);
            pictureBox1.Image = good_b;
            b.Dispose();
        }

        private void newImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(64, 48, PixelFormat.Format32bppRgb);
            graphics = Graphics.FromImage(pictureBox1.Image);
            graphics.FillRectangle(Brushes.White, 0, 0, pictureBox1.Image.Width, pictureBox1.Image.Height);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!e.Button.HasFlag(MouseButtons.Left)) return;
            if (null == graphics) return;
            if (!needDrawing) return;
            var x = e.X * pictureBox1.Image.Width / pictureBox1.Width;
            var y = e.Y * pictureBox1.Image.Height / pictureBox1.Height;
            graphics.FillRectangle(Brushes.Black, x, y, 1, 1);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!e.Button.HasFlag(MouseButtons.Left)) return;
            if (null == graphics) return;
            needDrawing = true;
            var x = e.X * pictureBox1.Image.Width / pictureBox1.Width;
            var y = e.Y * pictureBox1.Image.Height / pictureBox1.Height;
            graphics.FillRectangle(Brushes.Black, x, y, 1, 1);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            needDrawing = false;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (null == graphics) return;
            if (!e.Button.HasFlag(MouseButtons.Right)) return;
            var x = e.X * pictureBox1.Image.Width / pictureBox1.Width;
            var y = e.Y * pictureBox1.Image.Height / pictureBox1.Height;
            FillAt(x, y);
        }

        private uint filledColor;
        private int bitmapWidth;
        private int bitmapHeight;
        private List<Point> points;

        private unsafe uint ColorAt(uint* p, int x, int y)
        {
            if (x < 0 || bitmapWidth <= x) return filledColor + 1;
            if (y < 0 || bitmapHeight <= y) return filledColor + 1;
            return p[y * bitmapWidth + x];
        }

        private unsafe void FillAt(int x, int y)
        {
            points = new List<Point>();
            var b = (Bitmap) pictureBox1.Image;
            bitmapWidth = b.Width;
            bitmapHeight = b.Height;
            var data = b.LockBits(new Rectangle(0, 0, bitmapWidth, bitmapHeight), ImageLockMode.ReadWrite, b.PixelFormat);
            uint* p = (uint*) data.Scan0.ToPointer();
            filledColor = ColorAt(p, x, y);
            if (filledColor == 0x00ff0000)
            {
                b.UnlockBits(data);
                return;
            }
            // Find starting point
            ++x;
            while (ColorAt(p, x, y) == filledColor) ++x;
            Point startPoint = new Point(x, y);
            /*
             * 7 6 5
             * 0   4
             * 1 2 3 
             */
            var dxs = new int[] { -1, -1,  0,  1,  1,  1,  0, -1 };
            var dys = new int[] {  0,  1,  1,  1,  0, -1, -1, -1 };
            var prevDirection = 2;
            while (true)
            {
                int direction = (prevDirection + 6) % 8;
                while (filledColor == ColorAt(p, x + dxs[direction], y + dys[direction]))
                    direction = (direction + 1) % 8;
                var thisPoint = new Point(x, y);
                if (0 != points.Count && thisPoint == startPoint && 1 <= prevDirection && prevDirection <= 3) break;
                points.Add(thisPoint);
                p[y * bitmapWidth + x] = 0xff0000;
                x = x + dxs[direction];
                y = y + dys[direction];
                prevDirection = direction;   
            }
            b.UnlockBits(data);
            pictureBox1.Invalidate();
            points.Sort((l, r) => l.Y != r.Y ? l.Y - r.Y : l.Y - r.Y);
        }
    }
}
