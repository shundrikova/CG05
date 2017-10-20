using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Bezier
{
    public partial class Form1 : Form
    {
        Graphics g;

        Font drawFont = new Font("Arial", 10);
        SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);
        Pen pen = new Pen(Color.Black);

        bool moving = false;

       //List<Point> points;

        Dictionary<char, Point> points;

        char c = 'A';

        public Form1()
        {
            InitializeComponent();

            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(bmp);
            pen.StartCap = pen.EndCap = LineCap.Round;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            points = new Dictionary<char, Point>();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            label1.Visible = false;
            if (moving)
            {
                points[(char)listBox1.SelectedItem] = new Point(e.X, e.Y);
                redraw();
                moving = false;
            }
            else
            {
                g.DrawString(c.ToString(), drawFont, drawBrush, e.X, e.Y);
                g.DrawEllipse(pen, new Rectangle(e.X, e.Y, 1, 1));
                points.Add(c, new Point(e.X, e.Y));
                listBox1.Items.Add(c);
                c++;
                pictureBox1.Refresh();
            }
        }

        private void my_drawbeziers()
        {
            Point prev = new Point(-100, -100);
            for (int i = 3; i < points.Count; i += 3)
            {
                for (double t = 0; t <= 1; t += 0.0005)
                {
                    double c0 = (1 - t) * (1 - t) * (1 - t);
                    double c1 = (1 - t) * (1 - t) * 3 * t;
                    double c2 = (1 - t) * t * 3 * t;
                    double c3 = t * t * t;
                    double x = c0 * points.Values.ElementAt(i - 3).X + c1 * points.Values.ElementAt(i - 2).X + c2 * points.Values.ElementAt(i-1).X + c3 * points.Values.ElementAt(i).X;
                    double y = c0 * points.Values.ElementAt(i - 3).Y + c1 * points.Values.ElementAt(i - 2).Y + c2 * points.Values.ElementAt(i - 1).Y + c3 * points.Values.ElementAt(i).Y;
                    g.DrawEllipse(pen, new Rectangle((int)x, (int)y, 1, 1));
                    if (prev.X != -100)
                        g.DrawLine(pen, new Point((int)x, (int)y), prev);
                    prev.X = (int)x;
                    prev.Y = (int)y;
                }
                pictureBox1.Refresh();
            }
        }

        //draw bezier
        private void button1_Click(object sender, EventArgs e)
        {
            if (points.Count < 4)
            {
                label1.Visible = true;
                return;
            }

            if ((points.Count - 1) % 3 == 0)
            {
                my_drawbeziers();
                //pen.Color = Color.Red;
                //g.DrawBeziers(pen, points.Values.ToArray());
            }
            else
                label1.Visible = true;
            pictureBox1.Refresh();
        }

        //delete point
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0)
                return;
            points.Remove((char)listBox1.SelectedItem);
            listBox1.Items.Remove(listBox1.SelectedItem);
            redraw();
           // listBox1.SelectedItem
        }

        //redraw
        private void redraw()
        {
            g.Clear(System.Drawing.Color.White);

            if (!(points == null))
            for (int i = 0; i<points.Count; i++)
            {
                g.DrawEllipse(pen, new Rectangle(points.Values.ElementAt(i).X, points.Values.ElementAt(i).Y, 1, 1));
                g.DrawString(points.Keys.ElementAt(i).ToString(), drawFont, drawBrush, points.Values.ElementAt(i).X, points.Values.ElementAt(i).Y);
            }
            pictureBox1.Refresh();
        }

        //move
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0)
                return;
            moving = true;
        }

        //clear
        private void button4_Click(object sender, EventArgs e)
        {
            points.Clear();
            listBox1.Items.Clear();
            g.Clear(System.Drawing.Color.White);
            c = 'A';
            pictureBox1.Refresh();
        }


        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(bmp);
            redraw();
        }
    }
}
