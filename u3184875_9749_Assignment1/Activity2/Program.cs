using System;
using System.Collections.Generic;
using System.Linq;
using Activity1;

//This script takes the input of the user and turns it into a 2D graph when valid
//it will then use different Algorithm to find the Shortest, Easiest, Average, and Hardest path
//by going on 'adjacent nodes' and 'diagonal nodes' to reach the end point
namespace Activity2
{
    class Program
    {
        public static Program actTwo = new Program();
        List<List<Node>> gridMap = new List<List<Node>>();
        int gridRowSize = 0;
        int gridColSize = 0;

        Node startNode;
        Node endNode;

        //used to look at all directions
        int[] dirRow = { -1, 1, 0, 0, -1, 1, 1, -1 };
        int[] dirCol = { 0, 0, 1, -1, 1, 1, -1, -1 };

        List<List<Node>> possiblePaths;

        static void Main(string[] args)
        {
            string userInput = UserInput.GetUserInput(out actTwo.gridRowSize, out actTwo.gridColSize, out actTwo.gridMap);
            actTwo.FindStartAndEndNodes();

            Console.WriteLine("Shortest");
            actTwo.ShortestPath(new List<Node>(), new List<Node>());

            Console.WriteLine("Easiest");
            actTwo.EasiestPath(new List<Node>(), new List<Node>());

            Console.WriteLine("Average Number of Steps");
            actTwo.AveragePath();

            Console.WriteLine("Hardest");
            actTwo.HardestPath(new List<Node>(), new List<Node>());

            Graph.BuildGraph(actTwo.gridMap, actTwo.gridRowSize, actTwo.gridColSize);

            Console.ReadKey();
        }

        void FindStartAndEndNodes()
        {
            for (int row = 0; row < gridRowSize; row++)
                for (int col = 0; col < gridColSize; col++)
                {
                    if (gridMap[row][col].type == "S")
                    {
                        startNode = gridMap[row][col];
                        startNode.costToPos = 0;
                    }

                    if (gridMap[row][col].type == "E")
                        endNode = gridMap[row][col];
                }
        }

        //A* Algorithm
        //References: https://www.youtube.com/watch?v=-L-WgKMFuhE
        void ShortestPath(List<Node> toLookAt, List<Node> visited)
        {
            //add start node to toLookAt list
            //while toLookAt list isn't empty
            //  get node with lowest cost
            //      remove it from toLookAt list and add it to visited list
            //  if node is end point
            //      return
            //  loop through the neighbours of node
            //      if neighbour is not in visited list
            //          if new path to neighbour is shorter or if neighbour is not in toLookAt list
            //              set cost to neighbour to new path
            //              set parent pos to node
            //              if neighbour is not in toLookAt
            //                  add neighbour to toLookAt
            toLookAt.Add(startNode);
            while (toLookAt.Count > 0)
            {
                Node current = toLookAt.First();
                foreach (Node node in toLookAt)
                    if (node.costToPos < current.costToPos)
                        current = node;
                toLookAt.Remove(current);
                visited.Add(current);

                if (current.type == endNode.type)
                {
                    BuildPath(visited, visited.IndexOf(current));
                    return;
                }

                foreach (Node neighbour in GetAllNeighbours(current))
                {
                    if (!IsInList(neighbour, visited))
                    {
                        //current TO neighbour
                        int cost_cTOn = d(current, neighbour);
                        int fcost = Fcost(neighbour, cost_cTOn);
                        if (!IsInList(neighbour, toLookAt) || neighbour.costToPos < fcost)
                        {
                            Node node = neighbour;
                            node.costToPos = fcost;
                            node.SetParentPos(current.row, current.col);
                            if (!IsInList(node, toLookAt))
                                toLookAt.Add(node);
                        }
                    }
                }
            }
        }

        void EasiestPath(List<Node> toLookAt, List<Node> visited)
        {
            toLookAt.Add(startNode);
            while (toLookAt.Count > 0)
            {
                Node current = toLookAt.First();
                foreach (Node node in toLookAt)
                    if (node.costToPos < current.costToPos)
                        current = node;
                toLookAt.Remove(current);
                visited.Add(current);

                if (current.type == endNode.type)
                {
                    BuildPath(visited, visited.IndexOf(current));
                    return;
                }

                foreach (Node neighbour in GetAllNeighbours(current))
                {
                    if (!IsInList(neighbour, visited))
                    {
                        int fcost = Fcost(neighbour, neighbour.cost + d(current, neighbour));
                        if (!IsInList(neighbour, toLookAt) || neighbour.costToPos < fcost)
                        {
                            Node node = neighbour;
                            node.costToPos = fcost;
                            node.SetParentPos(current.row, current.col);
                            if (!IsInList(node, toLookAt))
                                toLookAt.Add(node);
                        }
                    }
                }
            }
        }

        public void AveragePath()
        {
            List<int> pathLengths = new List<int>();
            foreach (var path in possiblePaths)
                pathLengths.Add(path.Count);
            Console.WriteLine(pathLengths.Average());
        }

        void HardestPath(List<Node> toLookAt, List<Node> visited)
        {
            toLookAt.Add(startNode);
            while (toLookAt.Count > 0)
            {
                Node current = toLookAt.First();
                foreach (Node node in toLookAt)
                    if (node.costToPos > current.costToPos)
                        current = node;
                toLookAt.Remove(current);
                visited.Add(current);

                if (current.type == endNode.type)
                {
                    BuildPath(visited, visited.IndexOf(current));
                    return;
                }

                foreach (Node neighbour in GetAllNeighbours(current))
                {
                    if (!IsInList(neighbour, visited))
                    {
                        //does not include the directional cost because it is only focusing
                        //on the cost of the terrain and from the start to the end
                        int fcost = Fcost(neighbour, neighbour.cost);
                        if (!IsInList(neighbour, toLookAt) || neighbour.costToPos > fcost)
                        {
                            Node node = neighbour;
                            node.costToPos = fcost;
                            node.SetParentPos(current.row, current.col);
                            if (!IsInList(node, toLookAt))
                                toLookAt.Add(node);
                        }
                    }
                }
            }
        }

        void BuildPath(List<Node> parents, int endPointIndex)
        {
            List<Node> finalPath = new List<Node>();
            finalPath.Add(parents[endPointIndex]);
            for (int i = parents.Count - 1; i >= 0; i--)
            {
                if (IsParent(finalPath.Last(), parents[i].row, parents[i].col))
                    finalPath.Add(parents[i]);

                if (parents[i].type == "S")
                    break;
            }
            finalPath.Reverse();
            possiblePaths.Add(finalPath);

            foreach (var node in finalPath)
                Console.Write($"({node.Position()})");
            Console.WriteLine();
        }

        bool IsParent(Node current, int pRow, int pCol)
        {
            if (current.parentRow == pRow && current.parentCol == pCol)
                return true;
            return false;
        }

        bool IsInList(Node neighbour, List<Node> visitedList)
        {
            foreach (Node vNode in visitedList)
                if (vNode.IsEqualPosition(neighbour.row, neighbour.col))
                    return true;
            return false;
        }

        //parentCost is used to calculate the difficulty to get to the neighbour
        int Fcost(Node neighbour, int parentCost)
        {
            int distFromStart = (Math.Abs(neighbour.row - startNode.row) + Math.Abs(neighbour.col - startNode.col));
            int distFromEnd = (Math.Abs(neighbour.row - endNode.row) + Math.Abs(neighbour.col - endNode.col));
            return (distFromStart + distFromEnd + parentCost);
        }

        //getting the directional cost from current to the neighbour
        int d(Node current, Node neighbour)
        {   //returning 14 meanings that it is a diagonal node from current
            return (Math.Abs(current.row - neighbour.row) == 1 && Math.Abs(current.col - neighbour.col) == 1) ? 14 : 10;
        }

        Node[] GetAllNeighbours(Node current)
        {
            List<Node> unvisitedNeighbours = new List<Node>();
            //8 possible directions to look at
            for (int i = 0; i < 8; i++)
            {
                int newRow = current.row + dirRow[i];
                int newCol = current.col + dirCol[i];

                if (newRow < 0 || newCol < 0)
                    continue;
                if (newRow > gridRowSize - 1 || newCol > gridColSize - 1)
                    continue;
                if (gridMap[newRow][newCol].type == "O")
                    continue;

                Node node = gridMap[newRow][newCol];
                node.row = newRow;
                node.col = newCol;
                unvisitedNeighbours.Add(node);
            }
            return unvisitedNeighbours.ToArray();
        }
    }

}