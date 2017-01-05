using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ImageQuantization.Quantizer.Kmean
{
    class Kmean: Quantizer
    {
        private List<MiniRGB> sum;  // Get Averge of every CLuster Kmean
        private List<List<RGBPixel>> KmeanCluster;  // save Cluster output in Kmean 
        private List<List<RGBPixel>> newCend; // look like tmp use in kmean Cluster 

        /// <summary>
        /// Class to GetAverge Beacuse RGBPixel Didn't Accpeted + 'Byte'
        /// </summary>
        private class MiniRGB
        {
            public double red, green, blue;

            public MiniRGB(double R, double G, double B)
            {
                red = R;
                green = G;
                blue = B;
            }

        }
       /// <summary>
       /// Just Intilization Base ' Quantizer ' Elements
       /// </summary>
       /// <param name="li">Distinct Color</param>
       /// <param name="NumberofCluster"> Numer of Cluster</param>
       /// <param name="RGB">3D RJB Color Values</param>
        public Kmean(ref List<RGBPixel> li, ref int NumberofCluster, RGBPixel[, ,] RGB) :base(NumberofCluster , li ,  RGB)
        {
      
        }  
        /// <summary>
        ///  override Fuction to call clustering
        /// </summary>
        public override void getcluster()
        {
            ClusterKmean();
        }
        /// <summary>
        /// [1] conter is not equal 0 then Auto detect Acivate NO_Clusert = countr
        /// [2] Loop ant put K from disnct color into KmeanCLuster & set Avege foreach Cluster 0
        /// [3] Recrsive To Fill optimal Cluster
        /// [4] change 3D RGP Color Values to new Values
        /// </summary>
        private void ClusterKmean()
        {
            // List save KmeanCluster befor delete it
            newCend = new List<List<RGBPixel>>(NO_Clusert);

            // Update every itertiom new mean
            KmeanCluster = new List<List<RGBPixel>>(NO_Clusert);
            // Only sum for Evege
            sum = new List<MiniRGB>(NO_Clusert);
            // intializetiaon Kmean by No_cluster in first iteration 
            for (int i = 0; i < NO_Clusert; i++) // o(cluster) ~= o(C)
            {
                KmeanCluster.Add(new List<RGBPixel>());
                KmeanCluster[i].Add(li[i]);
                sum.Add(new MiniRGB(0, 0, 0));
            }

            Recrsive(0);
            for (int i = 0; i < NO_Clusert; i++) // o(cluster * length every cluster) ~= o(C*L)
            {
                Pallete.Add(KmeanCluster[i][0]);
                for (int j = 1; j < KmeanCluster[i].Count; j++)
                {
                    RGB[KmeanCluster[i][j].red, KmeanCluster[i][j].green, KmeanCluster[i][j].blue]
                        = new RGBPixel(KmeanCluster[i][0].red, KmeanCluster[i][0].green, KmeanCluster[i][0].blue);
                }
            }
            
        }

        /// <summary>
        /// Fill Cluster 
        /// [1] After Put every Color in the min cluster
        /// [2] check if Newcendroid "cnt == NO_Clusert" if No Update Centriod and recus=rsive again
        /// [3] Do Step 1,2 untill reach the optimal cendriod
        /// </summary>
        /// <param name="cnt">Number of cluster have Down</param>
        private void Recrsive(int cnt)
        {
            if (cnt == NO_Clusert)
                return;

            for (int i = 0; i < li.Count; i++) // o(D) * o(C) = o(D*C)
            {

                double cost = double.MaxValue; int idx = -1;
                for (int j = 0; j < NO_Clusert; j++)
                {
                    if (calculate_cost(KmeanCluster[j][0], li[i]) < cost)
                    {
                        cost = calculate_cost(KmeanCluster[j][0], li[i]);
                        idx = j;
                    }

                }
                KmeanCluster[idx].Add(li[i]);
                sum[idx].red += li[i].red;
                sum[idx].green += li[i].green;
                sum[idx].blue += li[i].blue;

            }

            for (int i = 0; i < NO_Clusert; i++) //o(Cluster)
            {
                sum[i].red /= (KmeanCluster[i].Count - 1);
                sum[i].green /= (KmeanCluster[i].Count - 1);
                sum[i].blue /= (KmeanCluster[i].Count - 1);
                RGBPixel col = new RGBPixel(sum[i].red, sum[i].green, sum[i].blue);
                if (KmeanCluster[i][0] == col)
                {
                    newCend.Add(new List<RGBPixel>(KmeanCluster[i])); // save if cnt == Num of cluster " Get Copy " push elemnt if eaul 
                    cnt++;
                }
                KmeanCluster[i] = new List<RGBPixel>();
                KmeanCluster[i].Add(col);
                sum[i] = new MiniRGB(0, 0, 0);   // set sum[IDX] Zero  if  repeat 
            }
            if (cnt == NO_Clusert)   // if  cnt == CLusterNumber 
            {
                KmeanCluster = new List<List<RGBPixel>>(newCend); // if equal mov newcend in actully kmean cluster
                newCend = new List<List<RGBPixel>>();
            }
            else       // make cnt 0 and repeat 
            {
                cnt = 0;
                newCend = new List<List<RGBPixel>>(NO_Clusert);
            }

            Recrsive(cnt);  // recursive O(i) : i th Number of itertion Needs to reach mean
            // total o(DCI) : D = Disnict color ,
            // C = Number of cluster , I = Number of itertion to reach optimal mean
        }

    }
}
