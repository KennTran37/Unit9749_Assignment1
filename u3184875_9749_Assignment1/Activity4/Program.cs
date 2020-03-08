using System;
using System.Collections.Generic;
using System.Linq;

//This program displays all the possible paths to take from the top left to the bottom right of the grid 
//while avoiding obstacles blocking the direction the agent plans to go
namespace Activity4
{
    class Program
    {
        public static Program actFour = new Program();
        Node[,] gridMap =
        {
          { Obstacles.Empty(0,0),  Obstacles.Empty(0,1), Obstacles.Empty(0,2), Obstacles.HoriRectangleTop(0,3), Obstacles.Empty(0,4)},
          { Obstacles.Empty(1,0), Obstacles.HoriRectangleBottom(1,1), Obstacles.VertRectangleRight(1,2), Obstacles.HoriRectangleBottom(1,3), Obstacles.TopRightTriangle(1,4) },
          { Obstacles.BottomLeftTriangle(2,0), Obstacles.Empty(2,1), Obstacles.Circle(2,2), Obstacles.Empty(2,3), Obstacles.Empty(2,4) },
          { Obstacles.Empty(3,0), Obstacles.VertRectangleMiddle(3,1), Obstacles.SmallBottomLeftTriangle(3,2), Obstacles.Empty(3,3), Obstacles.SmallTopRightTriangle(3,4) },
          { Obstacles.LeftLShape(4,0), Obstacles.Empty(4,1), Obstacles.Pentagon(4,2), Obstacles.Empty(4,3), Obstacles.Empty(4,4) }
        };

        int gridRowSize = 5;
        int gridColSize = 5;

        static void Main(string[] args)
        {
            actFour.gridMap[0, 0].parent = actFour.gridMap[0, 0];
            actFour.DynamicProgram(actFour.gridMap[0, 0], new List<Node>());
            Console.WriteLine("There are " + actFour.possiblePaths + " PossiblePaths");
            Console.ReadKey();
        }

        int possiblePaths;

        //Using dynamic programming to find all possible paths 
        //from the 'top left' to the 'bottom right' of the grid
        void DynamicProgram(Node current, List<Node> visited)
        {
            #region psudo
            //prioritize going right, down and bottom-right 
            //if cant try going left and bottom-left 
            //----
            //add current to visited

            //if end is reached
            //  return

            //check if path is open to the node on the right
            //check if path is open to the node on the bottom
            //check if path is open to the node on the bottom-right

            //if all three return false 
            //  check if path is open to the left
            //  check if path is open to the bottom-left
            #endregion

            if (current == null)
                return;

            visited.Add(current);
            if (current.row.Equals(gridRowSize - 1) && current.col.Equals(gridColSize - 1))
            {
                possiblePaths++;
                //BuildPath(visited);
                return;
            }

            if (current.row.Equals(gridRowSize - 1))
            {
                DynamicProgram(GoRight(current), visited);
                return;
            }

            if (current.col.Equals(gridColSize - 1))
            {
                DynamicProgram(GoDown(current), visited);
                return;
            }

            DynamicProgram(GoRight(current), visited);
            DynamicProgram(GoDown(current), visited);

            DynamicProgram(GoBottomRight(current), visited);
            DynamicProgram(GoBottomLeft(current), visited);
        }

        //Check if the direction we plan to go to is open on the current node first
        //then check if the direction to the next node is open
        Node GoRight(Node current)
        {
            if (current.IsWalkableDirection(Direction.right))
            {
                //check if we have reached the right side of the grid
                if (current.col.Equals(gridColSize - 1))
                    return null;
                Node target = gridMap[current.row, current.col + 1];
                if (target.IsWalkableDirection(Direction.left))
                {
                    target.parent = current;
                    return target;
                }
            }
            return null;
        }

        //Check the adjcent nodes from current and target node to see if at least one of them is open 
        Node GoBottomRight(Node current)
        {
            if (current.IsWalkableDirection(Direction.bottomRight))
            {
                if (current.col.Equals(gridColSize - 1) || current.row.Equals(gridRowSize - 1))
                    return null;
                Node rightNeighbour = gridMap[current.row, current.col + 1];
                Node bottomNeighbour = gridMap[current.row + 1, current.col];
                if (rightNeighbour.IsWalkableDirection(Direction.bottomLeft) || bottomNeighbour.IsWalkableDirection(Direction.topRight))
                {
                    Node target = gridMap[current.row + 1, current.col + 1];
                    if (target.IsWalkableDirection(Direction.topLeft))
                    {
                        target.parent = current;
                        return target;
                    }
                }
            }
            return null;
        }

        Node GoDown(Node current)
        {
            if (current.IsWalkableDirection(Direction.bottom))
            {
                if (current.row.Equals(gridRowSize - 1))
                    return null;
                Node target = gridMap[current.row + 1, current.col];
                if (target.IsWalkableDirection(Direction.top))
                {
                    target.parent = current;
                    return target;
                }
            }
            return null;
        }

        Node GoBottomLeft(Node current)
        {
            if (current.IsWalkableDirection(Direction.bottomLeft))
            {
                if (current.row.Equals(gridRowSize - 1) || current.col.Equals(0))
                    return null;
                Node leftNeighbour = gridMap[current.row, current.col - 1];
                Node bottomNeighbour = gridMap[current.row + 1, current.col];
                if (leftNeighbour.IsWalkableDirection(Direction.bottomRight) || bottomNeighbour.IsWalkableDirection(Direction.topLeft))
                {
                    Node target = gridMap[current.row + 1, current.col - 1];
                    if (target.IsWalkableDirection(Direction.topRight))
                    {
                        target.parent = current;
                        return target;
                    }
                }
            }
            return null;
        }

        public void BuildPath(List<Node> parents)
        {
            List<Node> finalPath = new List<Node>();
            finalPath.Add(parents.Last());
            for (int i = parents.Count - 1; i >= 0; i--)
                if (IsParent(finalPath.Last(), parents[i]))
                    finalPath.Add(parents[i]);

            finalPath.Reverse();
            Console.WriteLine();
            foreach (var p in finalPath)
                Console.Write($"({p.Position()})");
        }

        bool IsParent(Node current, Node parent)
        {
            if (current.parent == parent)
                return true;
            return false;
        }

        //add a new type of node that allows the agent to move to the node depending on the open spaces
        //for example, the sides - left, bottom-left, and the bottom are blocked off, while the others are open
        //create a function to check whether the agent can move to that node depending on the direction they are coming from
        //if they are coming from a diagonal node, check the two adjacent nodes from it to see if they are open as well.
    }


    enum Direction
    {
        topRight, top, topLeft,
        right, left,
        bottomRight, bottom, bottomLeft
    }
}
