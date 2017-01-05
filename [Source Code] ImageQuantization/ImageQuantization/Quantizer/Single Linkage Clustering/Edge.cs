using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class Edge
    {
        public int from, to;
        public double cost;
        public Edge(int From, int To, double Cost)
        {
            from = From; to = To; cost = Cost;
        }
    };

}
