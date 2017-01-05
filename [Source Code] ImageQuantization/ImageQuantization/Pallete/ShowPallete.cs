using ImageQuantization.Pallete;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class ShowPallete : Form
    {
        List<RGBPixel> li; // Distinct Color
        Graphics paint; // Varaible Grapghics to print in picture Box
        Bitmap bmp; // Image 
        ShowColor color; // if Click on color in Pallete
        /// <summary>
        /// Intializtion 
        /// Just Take Disntict Color
        /// </summary>
        /// <param name="Disnict_Color"></param>
        public ShowPallete(ref List<RGBPixel> Disnict_Color)
        {
            li = Disnict_Color;
            InitializeComponent();

        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (color != null)
                color.Close();

            this.Close();

        }

        /// <summary>
        /// Just Draw color in Rectangle aand put it in Image
        /// </summary>
        private void Painting()
        {
            pictureBox1.InitialImage = null;
            pictureBox1.Width = 200 * 25;

            if ((li.Count / 100) * 25 != 0)
                pictureBox1.Height = (li.Count / 100) * 25;
            else
                pictureBox1.Height = 200 * 25;

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            paint = Graphics.FromImage(bmp);
          
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
            paint.Dispose(); // close graphics
        }
        /// <summary>
        /// paint And put in text colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Painting();
            txtDistinctColor.Text = li.Count.ToString();
        }

        /// <summary>
        /// Save Pallete if User want
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// If press on image show the coloe he pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (color != null) 
            color.Close();

            Point Poin = (e as MouseEventArgs).Location;
            Color PressedColor = bmp.GetPixel(Poin.X, Poin.Y);
          
              color= new ShowColor(PressedColor);
                color.Show();
            
        }
    }
}
