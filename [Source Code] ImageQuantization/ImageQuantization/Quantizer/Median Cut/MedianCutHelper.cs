using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization.Quantizer.Median_Cut
{
    public class MedianCutHelper : Quantizer
    {
      
        List<MedianCutHelper> CubesList; // save All Cubic in list 
        public List<RGBPixel> colorList; // every cubic Have one List of color
        // Red ,Green And Blue  Upper&Lower bounds 
        private byte RedLowBound , RedHighBound , GreenLowBound ,
                     GreenHighBound ,BlueLowBound , BlueHighBound;

        private byte GetRedsz
        {
            get
            {
                return (byte)(RedHighBound - RedLowBound);
            }
        }
        private byte GetGreensz
        {
            get
            {
                return (byte)(GreenHighBound - GreenLowBound);
            }
        }
        private byte GetBluesz
        {
            get
            {
                return (byte)(BlueHighBound - BlueLowBound);
            }
        }
        private RGBPixel Averge
        {
            get
            {
                int red = 0, green = 0, blue = 0;

                foreach (RGBPixel argb in colorList)
                {

                    red += argb.red;
                    green += argb.green;
                    blue += argb.blue;
                }

                red = colorList.Count == 0 ? 0 : red / colorList.Count;
                green = colorList.Count == 0 ? 0 : green / colorList.Count;
                blue = colorList.Count == 0 ? 0 : blue / colorList.Count;

                // ColorModelHelper.HSBtoRGB(Convert.ToInt32(red/ColorModelHelper.HueFactor), green / 255.0f, blue / 255.0f);

                RGBPixel result = new RGBPixel(red, green, blue);
                return result;
            }
        }
       
        /// <summary>
        /// Intialization Base Distinct color & Number of CLuster & 3D RGP
        /// </summary>
        /// <param name="Distinct">Distinct color</param>
        /// <param name="NO_Claster">Number of CLuster</param>
        /// <param name="Grid">3D RGP Color Values</param>
        public MedianCutHelper(ref List<RGBPixel> Distinct, int NO_Claster, RGBPixel[, ,] Grid) : base(NO_Claster, Distinct, Grid)
        {
            CubesList = new List<MedianCutHelper>();
            MedianCutHelper a = new MedianCutHelper(li);
            CubesList.Add(a);
            RedLowBound = GreenLowBound = BlueLowBound = 0;
            RedHighBound = GreenHighBound = BlueHighBound = 0;
        }
       /// <summary>
       /// Intilzation Only List of color  Hint : List wase created once
       /// </summary>
       /// <param name="colors">new list of color to this box</param>
        public MedianCutHelper(List<RGBPixel> colors)
        {
            colorList = colors;
            Minmize();
        }
        /// <summary>
        /// Get Max and min RGB to Get the Box contain tham colors
        /// </summary>
        private void Minmize()
        {

            foreach (RGBPixel argb in colorList)
            {
                byte red = argb.red;
                byte green = argb.green;
                byte blue = argb.blue;

                if (red < RedLowBound) RedLowBound = red;
                if (red > RedHighBound) RedHighBound = red;
                if (green < GreenLowBound) GreenLowBound = green;
                if (green > GreenHighBound) GreenHighBound = green;
                if (blue < BlueLowBound) BlueLowBound = blue;
                if (blue > BlueHighBound) BlueHighBound = blue;
            }
        }
        /// <summary>
        /// [1] sort List of color in this Box And Split it
        /// [2]save tow box & his color 
        /// </summary>
        /// <param name="Index">0 refer red , 1 refer Green ,2 refer to Blue </param>
        /// <param name="firstMedianCutCube">out element Like ref</param>
        /// <param name="secondMedianCutCube">out element Like ref</param>
        private void SplitAtMedian(Byte Index, out MedianCutHelper firstMedianCutCube, out MedianCutHelper secondMedianCutCube)
        {
            List<RGBPixel> colors = null;
            if (Index == 0)  //Red
                colors = colorList.OrderBy(RGBPixel => RGBPixel.red).ToList(); // NlogN
            else if (Index == 1) //Green
                colors = colorList.OrderBy(RGBPixel => RGBPixel.green).ToList();// NlogN
            else //blue
                colors = colorList.OrderBy(RGBPixel => RGBPixel.blue).ToList();// NlogN
            int medianIndex = colorList.Count / 2;

            firstMedianCutCube = new MedianCutHelper(colors.GetRange(0, medianIndex)); // O(N/2)
            secondMedianCutCube = new MedianCutHelper(colors.GetRange(medianIndex, colors.Count - medianIndex));
        }
        /// <summary>
        ///  Give Number of cluster
        /// </summary>
        /// <param namxe="colorCount"></param>
        private void SplitBigCubes()
        {
           
            List<MedianCutHelper> NewSplitedCubes = new List<MedianCutHelper>();

            foreach (MedianCutHelper cube in CubesList) // o(2 || 4 || 8 || 16 ||....||1024 ) = O(1)*NlogN = o(NlogN)
            {
                // if another new cubes should be over the top; don't do it and just stop here
                if (NewSplitedCubes.Count >= NO_Clusert) break;

                MedianCutHelper CubeA, CubeB;

                // splits the cube along the red axis
                if (cube.GetRedsz >= cube.GetGreensz && cube.GetRedsz >= cube.GetBluesz) // o(NlogN)
                {
                    cube.SplitAtMedian(0, out CubeA, out CubeB); 
                }
                else if (cube.GetGreensz >= cube.GetBluesz) // splits the cube along the green axis
                {
                    cube.SplitAtMedian(1, out CubeA, out CubeB);
                }
                else // splits the cube along the blue axis
                {
                    cube.SplitAtMedian(2, out CubeA, out CubeB);
                }

                // adds newly created cubes to our list; but one by one and if there's enough cubes stops the process
                NewSplitedCubes.Add(CubeA);
                NewSplitedCubes.Add(CubeB);
            }

            CubesList = new List<MedianCutHelper>();
            // adds the new cubes to the official cube list
            foreach (MedianCutHelper medianCutCube in NewSplitedCubes)
                CubesList.Add(medianCutCube);

        }
        /// <summary>
        /// [1] Loop to Number of Cluster 
        /// [2] In every itertion go to loop in Cubic and splites all cubic
        /// [3] After return Change in 3D RJB volors Values
        /// </summary>
        public override void getcluster()
        {
            for (int i = 0; i < NO_Clusert; i++) //o(2 || 4 || 8 || 16 ||....||2042 ) =o(1)*NlogN
            {
                if (CubesList.Count == NO_Clusert) break;
                SplitBigCubes(); // o(NlogN)
            }
            foreach (MedianCutHelper Q in CubesList) // o(Cluster*cluster.size)~= o(1)*o(D)=o(D)
            {
                RGBPixel res = Q.Averge;
                this.Pallete.Add(res);

                for (int i = 0; i < Q.colorList.Count; i++)
                {
                    RGB[Q.colorList[i].red, Q.colorList[i].green, Q.colorList[i].blue]
                        = new RGBPixel(res.red, res.green, res.blue);
                }
            }
        }
    }
}
