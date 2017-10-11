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

namespace LSystems
{
    public partial class Form1 : Form
    {
        List<Tuple<char, string>> rules;
        string axiom;
        double angle;
        string direction;
        int iterations;
        private string file;
        string[] parameters;
        Stack<Tuple<double, double, double, double>> cache;
        //double startx = 10.0, starty = 10.0;
        //double scale;
        Graphics g;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rules = new List<Tuple<char, string>>();

            Image bi = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bi);
            g.Clear(Color.White);
            if (pictureBox1.Image != null)
                pictureBox1.Image.Dispose();
            pictureBox1.Image = bi;

            cache = new Stack<Tuple<double, double, double, double>>();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog1.FileName;

                if (!File.Exists(file))
                {
                    FileStream fs = File.Create(file);
                    fs.Close();
                }

                StreamReader sr = new StreamReader(file.ToString());
                textBox1.Text = sr.ReadToEnd();
                sr.Close();
                openFileDialog1.FileName = System.IO.Path.GetFileName(file);
                generate.Enabled = true;
                rules.Clear();
                parser();
            }
        }

        private void parser()
        {
            StreamReader sr = new StreamReader(file.ToString());
            string line = sr.ReadLine();
            parameters = line.Split(' ');

            while ((line = sr.ReadLine()) != null)
            {
                string[] rule = line.Split('>');
                rules.Add(new Tuple<char, string>(Convert.ToChar(rule[0]), rule[1]));
            }
        }

        private void makePath()
        {
            string current = axiom;
            string next = axiom;
            bool found;
            int depth = 0;
            while (depth < iterations)
            {
                current = next;
                next = "";
                for(int i = 0; i < current.Length; ++i)
                {
                    found = false;
                    for (int j = 0; j < rules.Count; ++j)
                    {
                        if(current[i] == rules[j].Item1)
                        {
                            next = next + rules[j].Item2;
                            found = true;
                        }
                    }
                    if (!found) next = next + current[i];
                }
                depth++;
            }
            textBox2.Text = next;
            drawLSystem(next);
        }

        private void drawLSystem(string ls)
        {
            double x = 0, y = 0, dx = 10, dy = 10;
            switch (direction)
            {
                case "Up":
                    x = pictureBox1.Width / 2;
                    y = pictureBox1.Height;
                    dx = 0;)
                    dy = -(pictureBox1.Height/ Math.Pow(2, iterations + 2));
                    break;
                case "Down":
                    x = pictureBox1.Width / 2;
                    y = 0;
                    dx = 0;
                    dy = (pictureBox1.Height / Math.Pow(2, iterations + 2));
                    break;
                case "Left":
                    x = pictureBox1.Width;
                    y = pictureBox1.Height / 2;
                    dx = -(pictureBox1.Width / Math.Pow(2, iterations + 2));
                    dy = 0;
                    break;
                case "Right":
                    x = 0;
                    y = pictureBox1.Height / 2;
                    dx = (pictureBox1.Width / Math.Pow(2, iterations + 2));
                    dy = 0;
                    break;
            }
            double rx, ry;
            Pen pen = new Pen(Color.Black);
            for (int i = 0; i < ls.Length; i++)
            {
                switch (ls[i])
                {
                    case 'F':
                        g.DrawLine(pen, (float)x, (float)y, (float)(x + dx), (float)(y + dy));
                        x += dx; y += dy;
                        break;
                    case '+':
                        rx = dx; ry = dy;
                        dx = rx * Math.Cos(Math.PI * angle / 180.0) - ry * Math.Sin(Math.PI * angle / 180.0);
                        dy = rx * Math.Sin(Math.PI * angle / 180.0) + ry * Math.Cos(Math.PI * angle / 180.0);
                        break;
                    case '-':
                        rx = dx; ry = dy;
                        dx = rx * Math.Cos(-Math.PI * angle / 180.0) - ry * Math.Sin(-Math.PI * angle / 180.0);
                        dy = rx * Math.Sin(-Math.PI * angle / 180.0) + ry * Math.Cos(-Math.PI * angle / 180.0);
                        break;
                    case '[':
                        cache.Push(new Tuple<double, double, double, double>(x, y, dx, dy));
                        break;
                    case ']':
                        Tuple<double, double, double, double> pos = cache.Peek();
                        cache.Pop();
                        x = pos.Item1; y = pos.Item2; dx = pos.Item3; dy = pos.Item4;
                        break;
                    default: break;
                }
            }
        }

        private void generate_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            iterations = (int)numericUpDown1.Value;
            axiom = parameters[0];
            angle = Convert.ToDouble(parameters[1]);
            direction = parameters[2];
            makePath();
            pictureBox1.Refresh();
        }
    }
}
