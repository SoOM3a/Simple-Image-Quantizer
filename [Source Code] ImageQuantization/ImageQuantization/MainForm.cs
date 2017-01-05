using ImageQuantization.Quantizer.Kmean;
using ImageQuantization.Quantizer.Median_Cut;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        // ----------- Histo & show Pallete Elements Intailzation ---------------
        ShowPallete p;
        Histogram Histo; 
       //---------------------------------------------
        List<RGBPixel> li = new List<RGBPixel>();
        RGBPixel[,] ImageMatrix;
        RGBPixel[, ,] RGB = new RGBPixel[256, 256, 256];
        public ImageQuantization.Quantizer.Quantizer quan;
     
        public MainForm()
        {
            InitializeComponent();
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MST_value.Text = "";
                li.Clear(); // Delete Old Distict Color
                for (int i = 0; i < 256; i++) // Set All Color False (255 , 255 , 255)
                    for (int j = 0; j < 256; j++)
                        for (int k = 0; k < 256; k++)
                            RGB[i, j, k] = new RGBPixel(255, 255, 255);

                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                int height = ImageOperations.GetHeight(ImageMatrix);
                int width = ImageOperations.GetWidth(ImageMatrix);
                RGBPixel False_cmp = new RGBPixel(255, 255, 255); 
                bool check_false_cmp = false;

                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                    {
                        // if Image Have Color (255 ,255,255)
                        if ( ImageMatrix[i, j].red == 255 && ImageMatrix[i, j].green == 255 && ImageMatrix[i, j].blue == 255)
                        {
                            if (!check_false_cmp)
                            {
                                check_false_cmp = true;
                                RGB[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] =
                                                  new RGBPixel(ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue);
                                li.Add(ImageMatrix[i, j]);
                            }

                        }
                        else if ( RGB[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] == False_cmp)
                        {

                            RGB[ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue] =
                                                new RGBPixel(ImageMatrix[i, j].red, ImageMatrix[i, j].green, ImageMatrix[i, j].blue);
                            li.Add(ImageMatrix[i, j]);
                        }
                    }
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1, RGB);

                txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
                txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
                txtDistinctColor.Text = li.Count.ToString();
                txtGaussSigma.Text = "1";

            }
        }
        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            Stopwatch we = new Stopwatch();
            we.Start();
            int sigma  = 0;
            if (txtGaussSigma.Text.ToString() != "") 
                sigma = int.Parse(txtGaussSigma.Text); 
      
            MST_value.Text = "";
            if (sigma > li.Count || sigma == 0)
                MessageBox.Show(" Please Enter a valid Number of cluster .");
            else if (TypeofCluster.Text == "")
            {
                MessageBox.Show(" Please Choose Type of Clustering .");
            }
            else
            {
                RGBPixel[, ,] grid = RGB.Clone() as RGBPixel[, ,]; // Get Clone  to Save True 3dRGP
                if (TypeofCluster.Text == TypeofCluster.Items[0].ToString())
                {
                        quan = new MST(ref li, sigma, grid, Auto_Detect.Checked);
                        quan.getcluster();
                        if (Auto_Detect.Checked == true)
                            txtGaussSigma.Text = quan.Pallete.Count.ToString();
                        MST_value.Text = "MST Value: " + (quan as MST).MST_Value.ToString();
                }
                else if (TypeofCluster.Text == TypeofCluster.Items[1].ToString())
                {
                    quan = new Kmean(ref li, ref sigma, grid);
                    quan.getcluster();
                }
                else if (TypeofCluster.Text == TypeofCluster.Items[2].ToString())
                {
                    if (MedianCutCluster.Text != "")
                    {
                        sigma = int.Parse(MedianCutCluster.Text);
                        if (sigma < li.Count)
                        {
                            quan = new MedianCutHelper(ref li, sigma, grid);
                            quan.getcluster();
                        }
                        else
                            MessageBox.Show("Invalid Number of Cluster .");
                   }
                    else
                     MessageBox.Show("Please Choose Number of Cluster .");
                   
                }

                else
                {
                    MessageBox.Show("Invalid Quantizer Method .");
                }
                ImageOperations.DisplayImage(ImageMatrix, pictureBox2, grid);
                we.Stop();
                MessageBox.Show(we.Elapsed.ToString());
            }

        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void orginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (p != null) p.Close();
            if (li.Count != 0)
            {
                 p = new ShowPallete(ref li);
                p.Show(this);
            }
            else
                MessageBox.Show("Please Select an Image");
        }

        private void quantizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (p != null) p.Close();           
            if (quan!=null && quan.Pallete.Count != 0)
            {
                p = new ShowPallete(ref quan.Pallete);
                p.Show();
            }
           
            else
                MessageBox.Show("Please Select an Image .");

        }

        private void orginalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (li.Count != 0)
            {
                if (Histo != null) Histo.Close();
                if (pictureBox1.Image != null)
                {
                    Histo = new Histogram((Bitmap)pictureBox1.Image, ImageOperations.GetHeight(ImageMatrix), ImageOperations.GetWidth(ImageMatrix));
                    Histo.Show();
                }
                else
                    MessageBox.Show("Please Select an Image .");
            }
           
        }

        private void quantizedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Histo != null)Histo.Close();
            if (pictureBox2.Image != null)
            {
                Histo = new Histogram( (Bitmap)pictureBox2.Image ,ImageOperations.GetHeight(ImageMatrix) , ImageOperations.GetWidth(ImageMatrix));
                Histo.Show();
            }
            else
                MessageBox.Show("Please Select an Image .");

        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string OpenedFilePath = openFileDialog1.FileName;
                OpenedFilePath = OpenedFilePath + ".bmp";
                pictureBox2.Image.Save(OpenedFilePath);

            }
        }

        private void TypeofCluster_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] Methods = {"Greedy Method"} ;
            string[] Number_of_clusterMC = {"2","4","8","16","32","64","256","512","1024","2048" };
            MST_value.Text = "";
            if (TypeofCluster.Text == TypeofCluster.Items[0].ToString())
            {
                Auto_Detect.Checked = false;
                Auto_Detect.Enabled = true;
                txtGaussSigma.Enabled = true;
                MedianCutCluster.Enabled = true;

                MedianCutCluster.DataSource = Methods;
            }
            else if (TypeofCluster.Text == TypeofCluster.Items[2].ToString())
            {
                Auto_Detect.Enabled = false;
                txtGaussSigma.Enabled = false;
                MedianCutCluster.Enabled = true;

                MedianCutCluster.DataSource = Number_of_clusterMC;
            }
            else
            {
                txtGaussSigma.Enabled = true;
                Auto_Detect.Enabled = false;
                MedianCutCluster.Enabled = false;
            }
        }

        private void Auto_Detect_CheckStateChanged(object sender, EventArgs e)
        {

            if (Auto_Detect.Checked == true)
            {
                // if auto detect with 0 or no number of cluster "" catch erorr"invalid numb cluster"
                txtGaussSigma.Text = "1";
                txtGaussSigma.Enabled = false;
            }
            else
                txtGaussSigma.Enabled = true;
        }


    }
}


 
