using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class PeriorityQueue
    {

        public List<Edge> Heap; // List of Elemnts
        public List<int> Locations;
        public bool[] Check_Nodes;

        /// <summary>
        /// Delete All Elements the Queue
        /// </summary>
        /// <param ></param>
        public void Delete_All()
        {

            Heap = new List<Edge>();
        }

        /// <summary>
        /// Get Two Elements And swap in one line 
        /// </summary>
        /// <param ></param>
        private void swap(Edge x, Edge y)
        {
            // a  =  a    +  b    - ( b   =  a  )
            x.to = (x.to + y.to) - (y.to = x.to);
            x.from = (x.from + y.from) - (y.from = x.from);
            x.cost = (x.cost + y.cost) - (y.cost = x.cost);
        }

        private void swap(int x, int y)
        {
            // a  =  a    +  b    - ( b   =  a  )
            x = (x + y) - (y = x);
        }

        /// <summary>
        /// Constrauctor to intilaize the Queue
        /// </summary>
        /// <param name="Node"></param>
        public PeriorityQueue()
        {
            Heap = new List<Edge>(); // creation the list
        }
        public PeriorityQueue(int Capacity )
        {
            Heap = new List<Edge>(Capacity); // creation the list
            Check_Nodes = new bool[Capacity];
            Check_Nodes = Enumerable.Repeat(false, Capacity).ToArray();
            Locations = new List<int>(Capacity);
            Locations = Enumerable.Repeat(-1, Capacity).ToList();

        }
        public PeriorityQueue(int Capacity , double val)
        {
            Heap = new List<Edge>(Capacity); // creation the list
            Heap = Enumerable.Repeat(new Edge(0,0,val), Capacity).ToList();
            Check_Nodes = new bool[Capacity];
            Check_Nodes = Enumerable.Repeat(false, Capacity).ToArray();
            Locations = new List<int>(Capacity);
            Locations = Enumerable.Repeat(-1, Capacity).ToList();
        }
        /// <summary>
        /// the function that take index i and return Left child in tree
        /// </summary>
        /// <param name="Node"></param>
        private int Left_Child(int Node)
        {
            //private function returns the index of the expected left child of the node at index Node 
            return Node * 2 + 1;
            //
        }
        /// <summary>
        /// the function that take index i and return Right child in tree
        /// </summary>
        /// <param name="Node"></param>
        private int Right_Child(int Node)
        {
            return Node * 2 + 2;
        }
        /// <summary>
        /// return Paernt of child of index N
        /// </summary>
        /// <param name="Node"></param>
        private int Parent(int Node)
        {
            return (Node - 1) / 2;
        }
        /// <summary>
        ///  modify our Heap tree By shift up element and return his new indx
        /// </summary>
        /// <param name="index"></param>
        private int ShiftUp(int Node)
        {
            //[1] if you a root ( 
            //[2] My parent Bigger than me value 
            // Just Goo Back :V :D  
            if (Node == 0 || Heap[Node].cost >= Heap[Parent(Node)].cost)
                return Node; // return exactly place in heap 

            // if I'm Greater Than my parent then [1] swap location , [2] swap me and my parent :D 
            // [1]
            int tmp = Locations[Heap[Node].to];
            Locations[Heap[Node].to] = Locations[Heap[Parent(Node)].to];
            Locations[Heap[Parent(Node)].to] = tmp;
            // [2]
            swap(Heap[Parent(Node)], Heap[Node]);
            /*---------------------------------------------*/
            //let's sent parent to check if in right position   ( that become a paraent )
            return ShiftUp(Parent(Node));
        }
        /// <summary>
        ///  modify our Heap tree By shift Down element and return his new idx
        /// </summary>
        /// <param name="index"></param>
        private int ShiftDown(int Node)
        {

            // There ara Three  Cases to Return Nothing:
            // [1] if you haven't Left Child then abslutly you haven't Right child
            // [2] if you have Left child and no right child and Left child >= curr then Left in this Position
            // [3] you have two children and left & right greate than you
            if (Left_Child(Node) >= Heap.Count
                || (Left_Child(Node) < Heap.Count && Right_Child(Node) >= Heap.Count && Heap[Left_Child(Node)].cost >= Heap[Node].cost)
                || (Left_Child(Node) < Heap.Count && Right_Child(Node) < Heap.Count && Heap[Left_Child(Node)].cost >= Heap[Node].cost && Heap[Right_Child(Node)].cost >= Heap[Node].cost))
                return Node;  // new idx 

            //In Case of you have right child  Smaller than '<='   left child  :: then shift right child  Down
            if (Right_Child(Node) < Heap.Count && Heap[Right_Child(Node)].cost <= Heap[Left_Child(Node)].cost)
            {
                // if i'm Greater than my right child then swap me with him [1] locations & Heap
                int tmp = Locations[Heap[Right_Child(Node)].to];
                Locations[Heap[Right_Child(Node)].to] = Locations[Heap[Node].to];
                Locations[Heap[Node].to] = tmp;
                swap(Heap[Right_Child(Node)], Heap[Node]);
                /*---------------------------------*/

                //let's sent parent to check if in right position   ( that become a paraent )
                return ShiftDown(Right_Child(Node));
            }
            //In Onther case of you have not a right child shift Down  your Left child
            else
            {
                int tmp = Locations[Heap[Left_Child(Node)].to];
                Locations[Heap[Left_Child(Node)].to] = Locations[Heap[Node].to];
                Locations[Heap[Node].to] = tmp;

                swap(Heap[Left_Child(Node)], Heap[Node]);
                return ShiftDown(Left_Child(Node));
            }

        }

        /// <summary>
        /// Add element to the periority queue
        /// </summary>
        /// <param name="Node"></param>
        public void Push_back(Edge Node)
        {
            if (!Check_Nodes[Node.to])
            {
                /*---------- Add in Queue O(log N Base 2) ----------*/
                Check_Nodes[Node.to] = true;
                Heap.Add(Node);//add him at the end
                Locations[Node.to] = Heap.Count - 1; // save intaily location 
                Locations[Node.to] = ShiftUp(Heap.Count - 1);//now modify the heap, as the last position made the heap not well modified
            }
            else
            {
                /*--------- Only Update and shift----------*/
                if (Heap[Locations[Node.to]].cost > Node.cost)
                {
                    Heap[Locations[Node.to]] = Node;  //Update the Low cost 
                    ShiftUp(Locations[Node.to]); // Now , Check if i'm big  than my parent
                }
            }
        }
        /// <summary>
        /// Remove the smallest element from your heap, and then modify it to have the next smallest element in the front
        /// </summary>
        /// <returns></returns>
        public Edge Delete_Min()
        {
            if (Heap.Count == 0)
                return null;
            Edge temp = Heap[0];//hold the element before killing him :D
            Locations[Heap[0].to] = -1; // make His node location -1 
            Heap[0] = Heap[Heap.Count - 1];//remove the smallest one,overwrite it the last element in the heap
            Locations[Heap[0].to] = 0; // make new node "overwite node" intialy 0 "root"
            Heap.RemoveAt(Heap.Count - 1);//remove the Element from the Heap
            if (Heap.Count != 0)
                Locations[Heap[0].to] = ShiftDown(0);//NOW, we have a non-accurate heap, now will put him in his expected position :D
            return temp;//return elemnt you deleted now :V 
        }
        /// <summary>
        /// returns if the heap still has elements or not
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return (Heap.Count == 0);//if you had run out of them , say true :D
        }
        /// <summary>
        ///  return size of your queue
        /// </summary>
        /// <returns> int </returns>
        public int Size()
        {
            return Heap.Count;
        }
        /// <summary>
        /// returns the first and smallest element in the heap
        /// </summary>
        /// <returns>Edge</returns>
        public Edge Top()
        {
            return Heap[0];
        }
    }

}


