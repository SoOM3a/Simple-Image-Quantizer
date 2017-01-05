using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization.Quantizer
{
   public class Quantizer
    {
        protected int NO_Clusert;    // Number of CLuster
        protected List<RGBPixel> li;       // Distinct Color
        protected RGBPixel[, ,] RGB;       // 3D Array ro Change Values of Colors 
        public List<RGBPixel> Pallete; // save new Coolor to show It
       
       /// <summary>
       /// Just Constrauctor :D 
       /// </summary>
        public Quantizer()
        {

        }

       /// <summary>
       /// Intailize member Varaible 
       /// </summary>
       /// <param name="NOClusert">Number of CLuster</param>
       /// <param name="li">Distinct Color</param>
       /// <param name="Grid">3D Array To change values of Colors</param>
        public Quantizer(int NOClusert, List<RGBPixel> li,  RGBPixel[, ,] Grid) // x -> #cluster
        {
            this.RGB = Grid;
            this.li = li;
            this.NO_Clusert = NOClusert;
            Pallete = new List<RGBPixel>(NO_Clusert);
            
        }
    /// <summary>
    /// Take To RGPPIXEL ANd Get Distance Between Theem     
    /// </summary>
    /// <param name="i"> First RGBPixel</param>
    /// <param name="j">Second RGBPixel</param>
    /// <returns></returns>
        public double calculate_cost(RGBPixel i, RGBPixel j)
        {
            return Math.Sqrt((i.red - j.red) * (i.red - j.red) +
                                            (i.green - j.green) * (i.green - j.green) +
                                            (i.blue - j.blue) * (i.blue - j.blue));

        }

       /// <summary>
       /// Virtual Fumction to overide in every inhert
       /// </summary>
        public virtual void getcluster() { }
   
      

        
    }

}
