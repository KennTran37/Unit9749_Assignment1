using System;
using System.Collections.Generic;
using System.Linq;
using Activity1;

namespace Activity5
{
    class Program
    {
        public static Program actFive = new Program();

        Node[,] gridMap = {
            {new Node("S", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("W", 0),new Node("O", 0),new Node("W", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("O", 0),new Node("O", 0),new Node("O", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0) }
        };

        int gridRowSize = 5;
        int gridColSize = 5;

        static void Main(string[] args)
        {
            Graph.BuildGraph(actFive.gridMap, actFive.gridRowSize, actFive.gridColSize);

            for (int row = 0; row < actFive.gridRowSize; row++)
            {
                for (int col = 0; col < actFive.gridColSize; col++)
                {

                }
            }
            Console.ReadKey();
        }

        void ViewCurrentNode(Node current)
        {
              
        }
    }

    //loop through the grid
    //~ At each node call a function which can be used to recursed when a obtsacle is found

    //if current is not a visited edge of object/shape
    //look at the neighbour nodes from current (search anti-clockwise)
    //if neighbour is obstacle
    //~Check the obstacle's position
    //  if position is on the topLeft/left from current
    //      call recurse function with node 'down' from current as current
    //  if position is on the bottomLeft/Bottom from current
    //      "                             " 'left' "                     "
    //  if position is on the right/bottomRight from current
    //      "                             " 'up'   "                     "
    //  if position is on the topRight/top from current
    //      "                             " 'right "                     "
    //return the edges of object/shape
}
