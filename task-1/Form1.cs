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

namespace task_1
{
    public partial class Form1 : Form
    {
        private Graphics g;
        private Bitmap image;
        private Color paletteColor;
        private Color curPixel;
        

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            paletteColor = Color.White;
        }
        

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image Files(*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image = new Bitmap(openDialog.FileName);
                    pictureBox1.Image = image;
                    pictureBox1.Invalidate();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void paletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                paletteColor = colorDialog1.Color;
            }
        }


        private void paint(int x, int y, Color curColor, Color newColor)
        {
            if (curColor == newColor)
                return;
            if (image.GetPixel(x, y) != curColor)
                return;

            image.SetPixel(x, y, newColor);

            /* Исправить
            if (y >= 0 && y < image.Height)
            {
                
                    int leftX, rightX;

                    if (x == 0)
                        leftX = 0;
                    else
                        leftX = x - 1;

                    if (x == (image.Width - 1))
                        rightX = (image.Width - 1);
                    else
                        rightX = x + 1;

                    var pix = image.GetPixel(leftX, y);
                    while (leftX > 0 && pix == curColor)
                    {
                        leftX--;
                        pix = image.GetPixel(leftX, y);
                    }
                    if (leftX == 0 && image.GetPixel(0, y) == curColor)
                        leftX = 0;

                    pix = image.GetPixel(rightX, y);
                    while (rightX < (image.Width - 1) && pix == curColor)
                    {
                        rightX++;
                        pix = image.GetPixel(rightX, y);
                    }
                    if (rightX == (image.Width - 1) && image.GetPixel(0, y) == curColor)
                        rightX = (image.Width - 1);

                    Pen pen = new Pen(newColor);
                    g.DrawLine(pen, leftX, y, rightX, y);

                    for (int i = leftX; i <= rightX; ++i)
                    {
                        var newY = y + 1;
                        pix = image.GetPixel(i, newY);
                        if (pix == curColor)
                            paint(i, newY, curColor, newColor);

                        newY = y - 1;
                        pix = image.GetPixel(i, newY);
                        if (pix == curColor)
                            paint(i, newY, curColor, newColor);
                    }
            }
            */
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                var x = e.Location.X;
                var y = e.Location.Y;
                curPixel = (pictureBox1.Image as Bitmap).GetPixel(e.Location.X, e.Location.Y);
                paint(x, y, curPixel, paletteColor);
                pictureBox1.Image = image;
            }
        }
    }
}
