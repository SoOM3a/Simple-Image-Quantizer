using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ImageQuantization
{

    public class MST
    {
        public List<bool> check;
        public List<Edge> path;
        public List<RGBPixel> li;
      public List<int>[,,] Graph;
        public List<List<int>> Cluster;
        public RGBPixel[,,] RGB;
        /// <summary>
        /// Don't DO Any thing Just Played :V
        /// </summary>
        /// <param ></param>
        public MST()
        {
        }

        /// <summary>
        /// Give itTow indx in list of Disinct color return Cost
        /// </summary>
        /// <param return >Double Cost</param>
        private double calculate_cost(int i, int j)
        {
            return Math.Sqrt((li[i].red - li[j].red) * (li[i].red - li[j].red) +
                                            (li[i].green - li[j].green) * (li[i].green - li[j].green) +
                                            (li[i].blue - li[j].blue) * (li[i].blue - li[j].blue));

        }

        /// <summary>
        /// [1] Inatilizing All Variables  
        /// [2] ---------Algo :D--------
        ///     Every Edge must Take Least Path then 
        ///     In every Node Calculte Min Path to take it 
        ///     1*Don't take My self   " Give My A cylce Not MSt"
        ///     2*Don't Take Path Itake it " Give My A cylce Not MSt"
        ///     3*Update my in Queue Value And location
        /// ------------------------------ Prime Method----------------------
        /// </summary>
        /// <param ></param>

        public MST(ref List<RGBPixel> li, int nodes, ref RGBPixel[,,] Grid)
        {
            this.li = li; // To use List of Disnct Color In Display image
            RGB = Grid;
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
                        if (calculate_cost(idx, j) < mininode)
                        {
                            mininode = calculate_cost(idx, j);
                            nextnode = j;
                        }
                        if (Lvalue[j] < mininode)
                        {
                            mininode = Lvalue[j];
                            nextnode = j;
                        }
                        if (calculate_cost(idx, j) < Lvalue[j])
                        {

                            parent[j] = idx;
                            Lvalue[j] = calculate_cost(idx, j);

                        }
                    }
                }

            }
            for (int i = 1; i < parent.Count; i++)
                path.Add(new Edge(parent[i], i, Lvalue[i]));
        }

        public static void Clustering_BFS(int s, ref int Cluster_num, ref MST m)
        {

            int Red = 0, Green = 0, Blue = 0;
            m.Cluster.Add(new List<int>());  //new cluster for New Node

            Queue<int> q = new Queue<int>();
            q.Enqueue(s);
            while (q.Count != 0)
            {
                int p = q.Dequeue();
                if (m.Graph[m.li[p].red, m.li[p].green, m.li[p].blue] != null) // if he not Visited and Have a childs
                {
                    if (!m.check[p])
                    {
                        m.check[p] = true;   // Just Marked Him as true
                        m.Cluster[Cluster_num].Add(p); // add him in cluster
                        Red += m.li[p].red;
                        Green += m.li[p].green;
                        Blue += m.li[p].blue;
                    }
                    for (int i = 0; i < m.Graph[m.li[p].red, m.li[p].green, m.li[p].blue].Count; i++) // Loop to push his childs             
                        if (!m.check[m.Graph[m.li[p].red, m.li[p].green, m.li[p].blue][i]])
                            q.Enqueue(m.Graph[m.li[p].red, m.li[p].green, m.li[p].blue][i]);

                }
                else if (!m.check[p])  // Not Visited and no childs
                {
                    m.check[p] = true;
                    m.Cluster[Cluster_num].Add(p);
                    Red += m.li[p].red;
                    Green += m.li[p].green;
                    Blue += m.li[p].blue;
                }

            }
            Red = Red / m.Cluster[Cluster_num].Count;
            Green = Green / m.Cluster[Cluster_num].Count;
            Blue = Blue / m.Cluster[Cluster_num].Count;

            for (int i = 0; i < m.Cluster[Cluster_num].Count; i++)
            {
                m.RGB[m.li[m.Cluster[Cluster_num][i]].red, m.li[m.Cluster[Cluster_num][i]].green, m.li[m.Cluster[Cluster_num][i]].blue]
                    = new RGBPixel(Red, Green, Blue);

            }
            Cluster_num++;
        }

        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, ref List<RGBPixel> li, ref int sigma, ref MST m)
        {
            int Graph_constrauction = 0;             // Cnt TO COnstrauct garph
            int Cluster_num = 0;
            m.Cluster = new List<List<int>>(sigma);
            m.path = m.path.OrderBy(Edge => Edge.cost).ToList();      // Sort Quick Sort NlogN
            m.check = Enumerable.Repeat(false, li.Count).ToList();  // Re Use to Check if Take it in Clustering or Not
            m.Graph = new List<int>[256, 256, 256];

            foreach (Edge i in m.path)
            {
                if (Graph_constrauction < m.path.Count - (sigma - 1))
                {
                    if (m.Graph[li[i.from].red, li[i.from].green, li[i.from].blue] == null)
                        m.Graph[li[i.from].red, li[i.from].green, li[i.from].blue] = new List<int>();
                    if (m.Graph[li[i.to].red, li[i.to].green, li[i.to].blue] == null)
                        m.Graph[li[i.to].red, li[i.to].green, li[i.to].blue] = new List<int>();

                    m.Graph[li[i.from].red, li[i.from].green, li[i.from].blue].Add(i.to);
                    m.Graph[li[i.to].red, li[i.to].green, li[i.to].blue].Add(i.from);
                    Graph_constrauction++;
                }
                else
                {
                    if (!m.check[i.from])
                        Clustering_BFS(i.from, ref Cluster_num, ref m);
                    if (!m.check[i.to])
                        Clustering_BFS(i.to, ref Cluster_num, ref m);
                }
            }
            return ImageMatrix;
        }

    }
}
