using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        Bitmap image = new Bitmap(800, 400);
        int СreateListPoint(string filename, List<Point> l_p)
        {
            FileStream file1 = new FileStream(filename, FileMode.Open); 
            StreamReader reader = new StreamReader(file1); 

            string[] p_cont = reader.ReadToEnd().Split('\n');
            int size_old_contours = l_p.Count;// = 0;

            for (int i = 0; i < p_cont.Length; i++)
            {
                string p1, p2;

                if (i < p_cont.Length - 1)
                {
                    p1 = p_cont[i];
                    p2 = p_cont[i + 1];
                }
                else
                {
                    p1 = p_cont[0];
                    p2 = p_cont[p_cont.Length - 1];
                }

                int p1_zpt = p1.IndexOf(',');
                int X1 = int.Parse(p1.Remove(p1_zpt, p1.Length - p1_zpt));
                int Y1 = int.Parse(p1.Remove(0, p1_zpt + 1));

                int p2_zpt = p2.IndexOf(',');
                int X2 = int.Parse(p2.Remove(p2_zpt, p2.Length - p2_zpt));
                int Y2 = int.Parse(p2.Remove(0, p2_zpt + 1));

                l_p.Add(new Point(X1, Y1));
                int x0 = X1;
                int y0 = Y1;

                while (X1 != X2 || Y1 != Y2 || (Math.Abs(X2 - X1) == Math.Abs(Y2 - Y1) && X2 - X1 != 0))
                {

                        if (X1 < X2)
                            ++X1;

                        if (X1 > X2)
                            --X1;

                        if (Y1 < Y2)
                            ++Y1;

                        if (Y1 > Y2)
                            --Y1;
                    l_p.Add(new Point(X1, Y1));
                }
            }
            reader.Close();

            return l_p.Count - size_old_contours;
        }

        private void GetPixLine(Bitmap bm, Point p1, Point p2, Color c)
        {
            for (int i = (p1.X < p2.X ? p1.X : p2.X); i < (p1.X > p2.X ? p1.X : p2.X); i++)
                bm.SetPixel(i, p1.Y, c);
        }

        private List<Point> get_oth_Pix(List<Point> l_p, int i_p)
        {
            List<Point> res = new List<Point>();
            res.Add(l_p[i_p]);
            int sh = 1;
            for (int i = 1; i < l_p.Count-1; i++)
            {

                if (i != i_p && l_p[i].Y == l_p[i_p].Y)
                    if (Math.Abs(l_p[i].X - res[res.Count - 1].X) > sh)
                    {
                        res.Add(new Point(l_p[i].X, l_p[i].Y));
                        sh = 1;
                    }
                    else
                        sh++;
            }

            return res;
        }

        private void sort_list_Pix(List<Point> l)
        {
            for (int i = 1; i < l.Count - 1; i++)
                for (int j = i+1; j < l.Count; j++)
                    if (l[i].X > l[j].X)
                    {
                        Point p = l[i];
                        l[i] = l[j];
                        l[j] = p;
                    }
        }
        
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            List<Point> l_p = new List<Point>();
            int size = 0;

            this.pictureBox1.Size = image.Size;
            pictureBox1.Image = image;

            size = СreateListPoint("max_contour.txt", l_p);
            СreateListPoint("contour1.txt", l_p);
            СreateListPoint("contour2.txt", l_p);
            СreateListPoint("contour3.txt", l_p);
            СreateListPoint("contour4.txt", l_p);
            // label2.Text = l_p.Count.ToString();
            for (int i = 0; i < l_p.Count; i++)
            {
                image.SetPixel(l_p[i].X, l_p[i].Y, Color.White);
            }

            for (int i = 0; i < size / 2; i += 1)
            {
                List<Point> eq_y = get_oth_Pix(l_p, i);
                sort_list_Pix(eq_y);

                for (int j = 0; j < eq_y.Count - 1; j += 2)
                {
                    GetPixLine(image, eq_y[j], eq_y[j + 1], Color.Red);
                }

                if (eq_y.Count % 2 != 0 )
                {
                   for (int j = eq_y.Count - 1; j > 1; j -= 2)
                    {
                        GetPixLine(image, eq_y[j], eq_y[j - 1], Color.Red);
                    }

                    /*for (int j = eq_y.Count - 2; j > 0; j -= 2)
                    {
                        GetPixLine(image, eq_y[j], eq_y[j - 1], Color.White);
                    }*/
                }
            }
        }

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}
	}
}
