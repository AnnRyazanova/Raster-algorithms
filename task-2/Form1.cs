using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            dialog.Filter = "images|*.png;*.jpg";
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
            Point point = new Point(0, 0);
            do
            {
                int direction = (prevDirection + 6) % 8;
                while (filledColor == ColorAt(p, x + dxs[direction], y + dys[direction]))
                    direction = (direction + 1) % 8;
                if (((5 <= prevDirection) && (prevDirection <= 7) && (1 <= direction) && (direction <= 3)) ||
                    ((5 <= direction) && (direction <= 7) && (1 <= prevDirection) && (prevDirection <= 3)) ||
                    ((0 == prevDirection) && ((direction + 2) % 8 < 3)) ||
                    ((4 == prevDirection) && ((direction + 6) % 8 < 3)) ||
                    ((0 == direction) && (prevDirection < 3)) ||
                    ((4 == direction) && ((prevDirection + 4) % 8 < 3)))
                    points.Add(point);
                x = x + dxs[direction];
                y = y + dys[direction];
                point = new Point(x, y);
                points.Add(point);
                prevDirection = direction;
            } while (point != startPoint);
            b.UnlockBits(data);
            pictureBox1.Invalidate();
            points.Sort((l, r) => l.Y != r.Y ? l.Y - r.Y : l.X - r.X);
			List<String> lines = new List<String>();
            for (int i = 0; i < points.Count; i += 2)
            {
                var x0 = points[i].X + 1;
                var y0 = points[i].Y;
                var x1 = points[i + 1].X - 1;
                var y1 = points[i + 1].Y;
				lines.Add("" + x0 + " " + y0);
				lines.Add("" + x1 + " " + y1);
				if (x1 < x0) continue;
                if (x0 == x1)
                    graphics.DrawRectangle(Pens.Orange, x0, y0, 0.5f, 0.5f);
                else
                    graphics.DrawLine(Pens.Orange, x0, y0, x1, y1);
            }
			for (int i = 0; i < 100; ++i)
			{
				string filename = "output" + i + ".txt";
				if (File.Exists(filename)) continue;
				File.WriteAllLines(filename, lines);
				break;
			}
		}
    }
}
