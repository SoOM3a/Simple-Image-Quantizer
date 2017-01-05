using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ImageQuantization
{
    public partial class Histogram : Form
    {
       public Dictionary<byte, long> Red;
        public Dictionary<byte, long> Green;
        public Dictionary<byte, long> Blue;

        /// <summary>
        /// Intialize 3 Dictionaries Red & Green & blue
        /// and call Add Color to increse Red&green&blue Conters " dictionaries"
        /// </summary>
        /// <param name="bmp">Image</param>
        /// <param name="Hieght">Width</param>
        /// <param name="width">Heights</param>
        public Histogram(Bitmap bmp, int Hieght, int width)
        {
            Red = new Dictionary<byte, long>();
            Green = new Dictionary<byte, long>();
            Blue = new Dictionary<byte, long>();
            for (int i = 0; i < width; i++)
                for (int ii = 0; ii < Hieght; ii++)
                    this.AddColor(bmp.GetPixel(i, ii));
            InitializeComponent();
        }

        /// <summary>
 ///  Take Element from Type Color
 ///  [1] if he Red exist ++ else ceat it and put it 1
 ///  [2] if he Green exist ++ else ceat it and put it 1
 ///  [3] if he Blue exist ++ else ceat it and put it   
 /// </summary>
 /// <param name="color"></param>
        public void AddColor(Color color)
        {
            if (Red.ContainsKey(color.R))
                Red[color.R]++;
            else
                Red.Add(color.R, 1);

            if (Green.ContainsKey(color.G))
                Green[color.G]++;
            else
                Green.Add(color.G, 1);

            if (Blue.ContainsKey(color.B))
                Blue[color.B]++;
            else
                Blue.Add(color.B, 1);

        }

        /// <summary>
        /// [1] clear chart
        /// [2] creat charting series t odraw in chart Give him Name And Color type of chart & Don'e Indexvalue and cleae the key of chart  
        /// [3] Add series in chart
        /// [4] Addl in Key increment His value
        ///  </summary>
        /// <param name="Pic">Image</param>
        /// <param name="Print">Print Can be Red ,Green ,blue</param>
        /// <param name="X">color of chart</param>
        private void Draw(Chart Pic, Dictionary<byte, long> Print, System.Drawing.Color X)
        { 
                Pic.Series.Clear(); // clear chart 
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series1",
                Color = X,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.StackedArea
            };

            Pic.Series.Add(series1);
           
            foreach (var i in Print)
                series1.Points.AddXY(i.Key, i.Value);

             Pic.Invalidate(); // refresh photo and invalidate chart Region 
        }
   /// <summary>
   /// Just Draw 3 charts
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
        private void Histogram_Load(object sender, EventArgs e)
        {
           
                Draw(Red_chart,Red, System.Drawing.Color.Red);
                Draw(Green_chart, Green, System.Drawing.Color.Green);
                Draw(Blue_chart,Blue, System.Drawing.Color.Blue);
        
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        
    }
}
