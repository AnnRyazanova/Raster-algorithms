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
                    g = Graphics.FromImage(pictureBox1.Image);
                    pictureBox1.Invalidate();
                    Form1.ActiveForm.Width = image.Width;
                    Form1.ActiveForm.Height = image.Height + menuStrip1.Height + panel1.Height;
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
            if (image.GetPixel(x, y) == newColor || image.GetPixel(x, y) != curColor)
                return;
            
            int leftX = x - 1, rightX = x + 1;

            Color pix;

            if (rightX != image.Width)
            {
                pix = image.GetPixel(rightX, y);

                while (rightX < image.Width && pix == curColor)
                {
                    rightX++;
                    if (rightX < image.Width)
                        pix = image.GetPixel(rightX, y);
                }
            }


            if (leftX != -1)
            {
                pix = image.GetPixel(leftX, y);

                while (leftX >= 0 && pix == curColor)
                {
                    leftX--;
                    if (leftX >= 0)
                        pix = image.GetPixel(leftX, y);
                }
            }

            rightX--;
            leftX++;

            Pen pen = new Pen(newColor);
            g.DrawLine(pen, leftX, y, rightX, y);

            for (int i = leftX; i < rightX; ++i)
            {
                if (y < image.Height - 1)
                    paint(i, y + 1, curColor, newColor);
              
                if (y > 0)
                    paint(i, y - 1, curColor, newColor);
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                var x = e.Location.X;
                var y = e.Location.Y;
                curPixel = (pictureBox1.Image as Bitmap).GetPixel(e.Location.X, e.Location.Y);
                if (radioButton1.Checked && curPixel.ToArgb() != paletteColor.ToArgb())
                { 
                    paint(x, y, curPixel, paletteColor);
                    pictureBox1.Refresh();
                }
            }
        }
    }
}
