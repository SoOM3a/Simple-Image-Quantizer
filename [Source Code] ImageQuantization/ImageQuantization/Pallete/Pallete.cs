using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class Pallete : Form
    {
        List<RGBPixel> li;
        Graphics paint;
        Bitmap bmp;
        public Pallete(List<RGBPixel> Disnict_Color)
        {
            li = Disnict_Color;
            InitializeComponent();

        }

        private void OK_Click(object sender, EventArgs e)
        {
            pictureBox1.InitialImage = null;
            this.Close();

        }

        private void Painting()
        {
            pictureBox1.InitialImage = null;
            //  paint = pictureBox1.CreateGraphics();
            pictureBox1.Width = 200 * 25;

            if ((li.Count / 100) * 25 != 0)
                pictureBox1.Height = (li.Count / 100) * 28;
            else
                pictureBox1.Height = 200 * 25;

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            paint = Graphics.FromImage(bmp);
            Graphics pp = pictureBox1.CreateGraphics();
            int k = 0, j = 0;
            for (int i = 0; i < li.Count; i++)
            {
                paint.FillRectangle(new SolidBrush(Color.FromArgb(li[i].red, li[i].green, li[i].blue)), k, j, 20, 20);
                k += 25;
                if ((i + 1) % 200 == 0)
                {
                    k = 0;
                    j += 25;
                }
            }
            pictureBox1.Image = bmp;
            paint.Dispose();

        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

            MessageBox.Show(sender.GetType().ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Painting();
        }

        private void button3_Click(object sender, EventArgs e)
        {


            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string OpenedFilePath = openFileDialog1.FileName;
                OpenedFilePath = OpenedFilePath + ".png";
                pictureBox1.Image.Save(OpenedFilePath);

            }


        }
    }
}
