using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageQuantization.Quantizer;
using ImageQuantization.Quantizer.Kmean;
namespace ImageQuantization
{

    public class MST : ImageQuantization.Quantizer.Quantizer 
    {
        private List<bool> check;    // bool Array use in MST in ' Single Linkage '
        private List<Edge> path;            // MST
        private List<List<RGBPixel>> Clustering; // OutPutClusters
        private List<List<int>> AdjBfs;    // Bfs 
        private SortedSet<int> Big; // comment
        public bool Auto_detect_cluster = false; // if enable Auto Detect
        public double MST_Value = 0;
        private double previter = double.MaxValue;
        private int countr = 0;      // Auto Detect to get Numb of cluster
        /// <summary>
        /// Don't DO Any thing Just Played :V
        /// </summary>
        /// <param ></param>
        public MST()
        {
        } 

        /// <summary>
        /// Juct Intilization Base  ' Quantizer ' Elemntes
        /// </summary>
        /// <param ></param>
        public MST(ref List<RGBPixel> li, int node, RGBPixel[, ,] Grid ,bool Auto_detect ): base(node,  li,  Grid)
        {
            Auto_detect_cluster = Auto_detect;
            Clustering = new List<List<RGBPixel>>();
         
         }
        /// <summary>
        /// ------- Override Method getcluster ----------
        /// [1] Inatilizing All Variables  
        /// [2] ---------Algo :D--------
        ///     Every Edge must Take Least Path then 
        ///     In every Node Calculte Min Path to take it 
        ///     1*Don't take My self   " Give My A cylce Not MSt"
        ///     2*Don't Take Path Itake it " Give My A cylce Not MSt"
        ///     3*Update my in lvalue Value And location in Parent
        /// ------------------------------ Prime Method----------------------
        /// </summary>
        public override void getcluster() // find mst
        {
          path = new List<Edge>(li.Count);
            List<double> Lvalue;
            List<int> parent;
            Lvalue = Enumerable.Repeat(double.MaxValue, li.Count).ToList();
            check = Enumerable.Repeat(false, li.Count).ToList();
            parent = Enumerable.Repeat(-1, li.Count).ToList();
            Lvalue[0] = 0;
            int nextnode = 0;
            for (int i = 0; i < li.Count - 1; i++)  // o(n)
            {
               int idx = nextnode;
                check[idx] = true;
                double mininode = double.MaxValue;
                for (int j = 0; j < li.Count; j++) // o(N)
                {

                    if (!check[j])
                    {
                        // Get Min  IDX in Nextnode
                        if (calculate_cost(li[idx], li[j]) < mininode)
                        {
                            mininode = calculate_cost(li[idx], li[j]);
                            nextnode = j;   
                        }
                        // set Idx for Min Node the Current Node
                       if (Lvalue[j] < mininode)
                        {
                            mininode = Lvalue[j];
                           nextnode = j;
                        }
                        // Update Value of the Edge with new Min val
                        if (calculate_cost(li[idx], li[j]) < Lvalue[j])
                        {
                            parent[j] = idx;
                            Lvalue[j] = calculate_cost(li[idx], li[j]);
                        }
                    }
                }

            }
            MST_Value = 0;
            for (int i = 1; i < parent.Count; i++)
            {
                path.Add(new Edge(parent[i], i, Lvalue[i]));
                MST_Value += Lvalue[i];
            }

            // ------------bouns section Zizo :D---------------
            if (Auto_detect_cluster)
            {
                NO_Clusert = 0; getstandard(path);
                BeginCLustering();
           
            }
            //-------------------------------------------------

            else BeginCLustering(); 
        }
        /// <summary>
        /// Give itTow indx in list of Disinct color return Cost
        /// </summary>
        /// <param return >Double Cost</param>
        private List<RGBPixel> Clustering_BFS(int s) // o(V^2)
        {
            
            List<RGBPixel> ret = new List<RGBPixel>();
            ret.Add(li[s]); // add him in cluster
            Queue<int> q = new Queue<int>();
            q.Enqueue(s);
            while (q.Count != 0) // o(E) worst O(V^2) ~=  o(D)
            {
                int p = q.Dequeue();
                if (!check[p])
                {
                    check[p] = true;   // Just Marked Him as true
                    for (int i = 0; i < AdjBfs[p].Count; i++) //  Loop to push his childs : worst o(V)             
                        if (!check[AdjBfs[p][i]])
                        {
                            ret.Add(li[AdjBfs[p][i]]); // add him in cluster
                            q.Enqueue(AdjBfs[p][i]);
                        }
                }
            }
            return ret ;
        }
       
        /// <summary>
        /// ---Apply Single Linkeage Clustering Algo ---------
        /// [1] Mark cost Max (k-1)  Edge in MST -1
        /// [2] loop on MST if cost != -1 "Not Max Edge " Bulid undirected graph adjbfs
        /// [3] Now We have CLusters in Adjbfs But childs of childs if Applay Bfs then,
        ///      we have a single CLuster foreach color Max in Big.
        /// </summary>
        private void BeginCLustering()
        {
           Clustering = new List<List<RGBPixel>>();
            path = path.OrderBy(Edge => Edge.cost).ToList();      // Sort Quick Sort NlogN
          
            check = Enumerable.Repeat(false, li.Count).ToList();  // Re Use to Check if Take it in Clustering or Not
            AdjBfs = new List<List<int>>(NO_Clusert);
            for (int i = 0; i < li.Count; i++) // memset adjbfs
                AdjBfs.Add( new List<int>());
            
            Big  = new SortedSet<int>();
            for (int i = 0; i < NO_Clusert-1 ; i++) // O(C*C.Length) ~= o(N^2)
            {
                double max = -1;
                int indx = 0;
                for (int ii = 0; ii < path.Count; ii++)
                {
                    if (max < path[ii].cost)
                    {
                        max = path[ii].cost;
                        indx = ii;
                    }
                }
                path[indx].cost = -1;
                Big.Add(path[indx].from);
                Big.Add(path[indx].to);
            }
            for (int i = 0;i< path.Count; i++) // o(C) ~= O(N)
            {
                if (path[i].cost != -1)
                {
                    AdjBfs[path[i].from].Add(path[i].to);
                    AdjBfs[path[i].to].Add(path[i].from);
                }
            }
            foreach(int i in  Big) // o(C) * O(E) ~= o(C*E) : E in worst = D then O(C*D)
                if (!check[i])
                    Clustering.Add(new List<RGBPixel>( Clustering_BFS(i)));
                
            setClusters(); // Change Values o(V^2)
        }
        /// <summary>
        /// ---------Bouns---------
        /// Just Loop until Reach standerdiv -Mean Greater than 0.001 or path == 0
        /// Only Zizo undrasand :D  
        /// </summary>
        /// <param name="path">MST</param>
        /// <returns></returns>
        private int getstandard(List<Edge> path)
        {
            double SanderDiv = 0 , fullres , sum = 0 , mean = 0;
            List<Edge> Copy = new List<Edge>(path);
            double maxcost = 0;
            int maxindx = 0;
            double maxdiff = 0;

            for (int i = 0; i < path.Count; i++)
                mean += path[i].cost;
            
            mean /= path.Count;
            for (int i = 0; i < path.Count; i++)
            {

                sum = mean - path[i].cost;
                sum *= sum;
                SanderDiv += sum;
            }
            SanderDiv = Math.Sqrt(SanderDiv /path.Count);
            
            fullres = SanderDiv;
       
            for (int i = 0; i < path.Count; i++)
            {
                maxdiff = Math.Abs(mean - path[i].cost);
                if (maxcost < maxdiff)
                {
                    maxcost = maxdiff;
                    maxindx = i;
                }
            }
            Copy.RemoveAt(maxindx);

            countr++;
            
            if ((Math.Abs(previter - fullres) <= 0.0001) || Copy.Count == 0)
                return NO_Clusert = countr;
            
            previter = fullres;

            getstandard(Copy);
            countr = 0;
            return NO_Clusert;
        }
        /// <summary>
        /// Get Index of cluster 
        /// </summary>
        /// <param name="cluster_Number">Cluster Idx</param>
        /// <returns>Color Avrg</returns>
        private RGBPixel Avrge(int cluster_Number) // O(C)
        {
            double R = 0 ,G=0 ,B=0 ;

            for (int i = 0; i<Clustering[cluster_Number].Count; i++) // o(C.length)
            {
                R += Clustering[cluster_Number][i].red;
                G += Clustering[cluster_Number][i].green;
                B += Clustering[cluster_Number][i].blue;
            }
            R /= Clustering[cluster_Number].Count;
            G /= Clustering[cluster_Number].Count;
            B /= Clustering[cluster_Number].Count;
            return new RGBPixel(R, G, B);
        }

        /// <summary>
        /// set color in 3d RGB volor Values
        /// </summary>
        private void setClusters() // O(K*D)
        {
            for (int i = 0 ; i<Clustering.Count ;i++) // o(K*D)
            {
                RGBPixel res = Avrge(i);
                Pallete.Add(res);
                for (int j = 0; j < Clustering[i].Count; j++)
                    RGB[Clustering[i][j].red, Clustering[i][j].green, Clustering[i][j].blue] = res;
            }

        }
      
    }
}
