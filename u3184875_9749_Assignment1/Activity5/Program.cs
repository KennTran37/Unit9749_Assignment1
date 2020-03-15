using Activity1;
using System;
using System.Collections.Generic;

namespace Activity5
{
    class Program
    {
        public static Program actFive = new Program();

        Node[,] gridMap = {
            {new Node("S", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("O", 0),new Node("W", 0),new Node("O", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("O", 0),new Node("O", 0),new Node("O", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("O", 0),new Node("W", 0),new Node("O", 0),new Node("W", 0) },
            {new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0),new Node("W", 0) }
        };

        int gridRowSize = 5;
        int gridColSize = 5;

        //up, down, left, right, topLeft, bottomLeft, bottomRight, topRight
        //int[] dirRow = { -1, 1, 0, 0, -1, 1, 1, -1 };
        //int[] dirCol = { 0, 0, -1, 1, -1, -1, 1, 1 };

        int[] dirRow = { -1, 0, 1, 1, 1, 0, -1, -1 };
        int[] dirCol = { -1, -1, -1, 0, 1, 1, 1, 0 };

        //number of directions to look at
        const int numOfDir = 8;

        List<Node> obstacleEdges = new List<Node>();

        //because the AI is starting at (0,0) at the top left, and looping through each row from left to right
        //we are prioitizing to go down the obstacles/shapes left edge first 
        Directions directionToCheckFirst = Directions.down;

        static void Main(string[] args)
        {
            Graph.BuildGraph(actFive.gridMap, actFive.gridRowSize, actFive.gridColSize);
            actFive.SetNodePositions();
            actFive.LoopThroughMap();

            //int iLooped = 0;
            //for (int i = actFive.prioDirection(); i < 8; i = i < 7 ? i + 1 : 0)
            //{
            //    iLooped++;
            //    Console.WriteLine($"({actFive.dirRow[i]}, {actFive.dirCol[i]})");
            //    if (iLooped == 8)
            //        break;
            //}


            Console.WriteLine();
            foreach (var node in actFive.obstacleEdges)
                Console.WriteLine(node.Position());

            Console.ReadKey();
        }

        void SetNodePositions()
        {
            for (int row = 0; row < gridRowSize; row++)
            {
                for (int col = 0; col < gridColSize; col++)
                {
                    gridMap[row, col].row = row;
                    gridMap[row, col].col = col;
                }
            }
        }

        void LoopThroughMap()
        {
            Node obstacleNeighbour;
            for (int row = 0; row < gridRowSize; row++)
                for (int col = 0; col < gridColSize; col++)
                    if (actFive.NeighbourIsObstacle(gridMap[row, col], out obstacleNeighbour))
                    {
                        actFive.ViewCurrentNode(gridMap[row, col], obstacleNeighbour);
                        return;
                    }
        }

        void ViewCurrentNode(Node current, Node neighbour)
        {
            if (obstacleEdges.Contains(current))
                return;

            if (NextToEdge(current, neighbour))
                obstacleEdges.Add(current);

            //Console.WriteLine(current.Position() + "- " + directionToCheckFirst.ToString());

            int rowPos = neighbour.row - current.row;
            int colPos = neighbour.col - current.col;
            //  topLeft                         left              
            if (rowPos == -1 && colPos == -1 || rowPos == 0 && colPos == -1)
            {
                directionToCheckFirst = Directions.left;
                NewDirectionToGo(current, current.row - 1, current.col, Directions.up);
            }

            //  bottomLeft                     bottom
            if (rowPos == 1 && colPos == -1 || rowPos == 1 && colPos == 0)
            {
                directionToCheckFirst = Directions.down;
                NewDirectionToGo(current, current.row, current.col - 1, Directions.left);
            }

            //  bottomRight                   right
            if (rowPos == 1 && colPos == 1 || rowPos == 0 && colPos == 1)
            {
                directionToCheckFirst = Directions.right;
                NewDirectionToGo(current, current.row + 1, current.col, Directions.down);
            }

            //  topRight                       top
            if (rowPos == -1 && colPos == 1 || rowPos == -1 && colPos == 0)
            {
                directionToCheckFirst = Directions.up;
                NewDirectionToGo(current, current.row, current.col + 1, Directions.right);
            }
        }

        void DirectionToPrioitize(Node current, Node neighbour)
        {

        }

        void NewDirectionToGo(Node current, int newRow, int newCol, Directions toGo)
        {
            if (gridMap[newRow, newCol].type != "O" && !obstacleEdges.Contains(gridMap[newRow, newCol]))
                ViewCurrentNode(gridMap[newRow, newCol], GetNextObstacle(gridMap[newRow, newCol]));
            else
                AltDirections(current, toGo);
        }

        void AltDirections(Node current, Directions originalDirection)
        {
            switch (originalDirection)
            {
                case Directions.up:     //alt of UP == RIGHT
                    if (IsInBounds(current.row, current.col + 1))
                    {
                        if (obstacleEdges.Contains(gridMap[current.row, current.col + 1]) || gridMap[current.row, current.col + 1].type == "O")
                            AltDirections(current, Directions.right);
                        else
                            ViewCurrentNode(gridMap[current.row, current.col + 1], GetNextObstacle(gridMap[current.row, current.col + 1]));
                    }
                    break;

                case Directions.down:   //alt of DOWN == LEFT
                    if (IsInBounds(current.row, current.col - 1))
                    {
                        if (obstacleEdges.Contains(gridMap[current.row, current.col - 1]) || gridMap[current.row, current.col - 1].type == "O")
                            AltDirections(current, Directions.left);
                        else
                            ViewCurrentNode(gridMap[current.row, current.col - 1], GetNextObstacle(gridMap[current.row, current.col - 1]));
                    }
                    break;

                case Directions.left:   //alt of LEFT == UP
                    if (IsInBounds(current.row - 1, current.col))
                    {
                        if (obstacleEdges.Contains(gridMap[current.row - 1, current.col]) || gridMap[current.row - 1, current.col].type == "O")
                            AltDirections(current, Directions.up);
                        else
                            ViewCurrentNode(gridMap[current.row - 1, current.col], GetNextObstacle(gridMap[current.row - 1, current.col]));
                    }
                    break;

                case Directions.right:  //alt of RIGHT == DOWN
                    if (IsInBounds(current.row + 1, current.col))
                    {
                        if (obstacleEdges.Contains(gridMap[current.row + 1, current.col]) || gridMap[current.row + 1, current.col].type == "O")
                            AltDirections(current, Directions.down);
                        else
                            ViewCurrentNode(gridMap[current.row + 1, current.col], GetNextObstacle(gridMap[current.row + 1, current.col]));
                    }
                    break;
            }
        }

        bool IsInBounds(int row, int col)
        {
            if (row < 0 || col < 0)
                return false;
            if (row > gridRowSize - 1 || col > gridColSize - 1)
                return false;

            return true;
        }

        bool NextToEdge(Node current, Node neighbour)
        {
            int row = Math.Abs(neighbour.row - current.row);
            int col = Math.Abs(neighbour.col - current.col);
            if ((row == 1 && col == 0) || (row == 0 && col == 1))
                return true;
            return false;
        }

        bool NeighbourIsObstacle(Node current, out Node neighbour)
        {
            int iLooped = 0;
            for (int i = actFive.prioDirection(); i < 8; i = i < 7 ? i + 1 : 0)
            {
                iLooped++;

                int newRow = current.row + dirRow[i];
                int newCol = current.col + dirCol[i];
                if (!IsInBounds(newRow, newCol))
                    continue;
                if (gridMap[newRow, newCol].type == "O")
                {
                    neighbour = gridMap[newRow, newCol];
                    return true;
                }

                if (iLooped == 8)
                    break;
            }

            neighbour = new Node();
            return false;
        }

        Node GetNextObstacle(Node current)
        {
            int iLooped = 0;
            for (int i = actFive.prioDirection(); i < 8; i = i < 7 ? i + 1 : 0)
            {
                iLooped++;

                int newRow = current.row + dirRow[i];
                int newCol = current.col + dirCol[i];
                if (!IsInBounds(newRow, newCol))
                    continue;
                if (gridMap[newRow, newCol].type == "O")
                    return gridMap[newRow, newCol];

                if (iLooped == 8)
                    break;
            }

            //for (int i = 0; i < numOfDir; i++)
            //{
            //    int newRow = current.row + dirRow[i];
            //    int newCol = current.col + dirCol[i];
            //    if (newRow < 0 || newCol < 0)
            //        continue;
            //    if (newRow > gridRowSize - 1 || newCol > gridColSize - 1)
            //        continue;
            //    if (gridMap[newRow, newCol].type == "O")
            //        return gridMap[newRow, newCol];
            //}
            return new Node();
        }

        int prioDirection()
        {
            switch (directionToCheckFirst)
            {
                case Directions.up:
                    return 0;
                case Directions.down:
                    return 4;
                case Directions.left:
                    return 2;
                case Directions.right:
                    return 6;
            }

            return 0;
        }
    }

    enum Directions { up, down, left, right }

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

    //Before going into the direction, check if that node is an obstacle
}
