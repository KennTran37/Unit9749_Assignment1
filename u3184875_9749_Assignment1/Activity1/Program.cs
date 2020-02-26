using System;
using System.Collections.Generic;
using System.Linq;

//This script takes the input of the user and turns it into a 2D graph when valid
//it will then use different Algorithm to find the Shortest, Easiest, Average, and Hardest path
//by going on 'adjacent nodes' to reach the end point
namespace Activity1
{
    class Program
    {
        public static Program actOne = new Program();
        public static Random randomClass = new Random();

        Node[] typesArray =
        {
            new Node("S", 0), new Node("E", 0), new Node("W40", 5), new Node("W90", 9), new Node("W120", 5), new Node("W0", 1),
            new Node("W", 1), new Node("O", int.MaxValue), new Node("Ww", 6), new Node("Wg", 3), new Node("Wr", 4)
        };

        public Node[] TypesArray { get => typesArray; }

        List<List<Node>> gridMap = new List<List<Node>>();

        int gridRowSize = 3;
        int gridColSize = 3;

        //used to look at the adjacent neighbour nodes
        int[] dirRow = { -1, 1, 0, 0 }; //direction in row
        int[] dirCol = { 0, 0, 1, -1 }; //direction in coloumn

        Node startNode;
        Node endNode;

        List<List<Node>> possiblePaths = new List<List<Node>>();

        static void Main(string[] args)
        {
            string userInput = UserInput.GetUserInput(out actOne.gridRowSize, out actOne.gridColSize, out actOne.gridMap);
            Console.Clear();
            Console.WriteLine($"\n\t{userInput}\n");

            actOne.FindStartingPoint();
            Console.WriteLine("\nShortest Path");
            actOne.ShortestPath();

            Console.WriteLine("\nEasiet Path");
            actOne.EasiestPath(new List<Node>(), new List<Node>());
            actOne.PrintPath(actOne.possiblePaths.Last());

            Console.WriteLine("\nAverage Path");
            actOne.AveragePath();

            Console.WriteLine("\nHardest Path");
            actOne.HardestPath();
            Console.WriteLine();

            Graph.BuildGraph(actOne.gridMap, actOne.gridRowSize, actOne.gridColSize);

            Console.ReadKey();
        }

        #region Shortest Path - Breath First Search
        void ShortestPath()
        {
            //starting the search from the neighbours of startPoint because it will give more results
            List<List<Node>> possibleShortestPaths = new List<List<Node>>();
            foreach (Node neighbour in GetAdjacentNeighbours(startNode))
            {
                Queue<Node> toVisit = new Queue<Node>();
                toVisit.Enqueue(neighbour);
                List<Node> path = GetShortestPath(toVisit, new List<Node>() { startNode, neighbour });
                if (path != null)
                    possibleShortestPaths.Add(path);
            }

            List<Node> shortestPath = possibleShortestPaths.Last();
            foreach (var path in possibleShortestPaths)
                if (path.Count < shortestPath.Count)
                    shortestPath = path;

            BuildPath(shortestPath);
            PrintPath(possiblePaths.Last());
        }

        //Finds the shortest path to the End Point using the 'Breadth First Search Algorithm'
        //While the nodesToVisit queue isnt empty, look at the neighbouring node at the first element of the queue
        //Reference: https://www.redblobgames.com/pathfinding/a-star/introduction.html
        //           https://www.youtube.com/watch?v=oDqjPvD54Ss
        //           https://www.youtube.com/watch?v=-L-WgKMFuhE&t=125s
        List<Node> GetShortestPath(Queue<Node> toLookAt, List<Node> visited)
        {
            while (toLookAt.Count > 0)
            {
                Node current = toLookAt.Dequeue();
                foreach (Node neighbour in GetAdjacentNeighbours(current))
                {
                    if (!IsInList(neighbour, visited))
                    {
                        toLookAt.Enqueue(neighbour);
                        neighbour.SetParentPos(current.row, current.col);
                        visited.Add(neighbour);
                    }

                    if (neighbour.type == "E")
                    {
                        endNode = neighbour;
                        //BuildPath(parents);
                        return visited;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Easiest Path - Dijkstra's Algorithm
        //Dijkstra's Algorithm
        //References: https://youtu.be/K_1urzWrzLs
        void EasiestPath(List<Node> toLookAt, List<Node> visited)
        {
            //init
            //new list of tolookat
            //find neighbour nodes from start
            //add them to tolookat list
            //set the cost of reaching them

            //initializing the list with the neighbours of startPoint
            visited.Add(startNode);
            foreach (Node neighbour in GetAdjacentNeighbours(startNode))
            {
                Node neighbourNode = minCostNode(startNode, neighbour);
                toLookAt.Add(neighbourNode);
            }

            //in a while loop
            //loop through the tolookat list and find the cheapest node
            //find the neighbouring nodes of the cheapest nodes
            //check if that neighbour is already visited
            //if not, set the cost to get to that neighbour
            //check if that neighbour is the endPoint
            while (toLookAt.Count > 0)
            {
                Node current = toLookAt.First();
                foreach (Node node in toLookAt)
                    if (node.costToPos < current.costToPos)
                        current = node;

                toLookAt.Remove(current);
                visited.Add(current);

                if (current.type == "E")
                {
                    endNode = current;
                    BuildPath(visited);
                    return;
                }

                foreach (Node neighbour in GetAdjacentNeighbours(current))
                {
                    if (!IsInList(neighbour, visited))
                    {
                        if (!IsInList(neighbour, toLookAt) || CheaperCost(current, neighbour))
                        {
                            Node neighbourNode = minCostNode(current, neighbour);
                            neighbourNode.SetParentPos(current.row, current.col);
                            if (!IsInList(neighbourNode, toLookAt))
                                toLookAt.Add(neighbourNode);
                        }
                    }
                }
            }
        }

        bool CheaperCost(Node parent, Node neighbour)
        {
            if (parent.costToPos + neighbour.cost < neighbour.costToPos)
                return true;
            return false;
        }

        //checking if the cost to the node is smaller than it already is
        Node minCostNode(Node parent, Node neighbour)
        {
            neighbour.costToPos = parent.costToPos + neighbour.cost;
            return neighbour;
        }
        #endregion

        #region Average Path
        //Creating the average path based on the common nodes that the paths used
        //looping through path and checking 
        void AveragePath()
        {
            List<Node> commonNodes = new List<Node>();
            for (int path = 0; path < possiblePaths.Count; path++)
                for (int node = 0; node < possiblePaths[path].Count; node++)
                    if (possiblePaths[path][node].Position() == possiblePaths[path + (path < possiblePaths.Count - 1 ? 1 : 0)][node].Position())
                        if (!ContainsNode(commonNodes, possiblePaths[path][node]))
                            commonNodes.Add(possiblePaths[path][node]);

            foreach (var node in commonNodes)
                Console.Write($"({node.Position()})");
            Console.WriteLine();
        }

        bool ContainsNode(List<Node> commonNodes, Node target)
        {
            foreach (Node commNode in commonNodes)
                if (commNode.Position() == target.Position())
                    return true;
            return false;
        }
        #endregion

        #region Hardest Path - Prim's Algorithm
        void HardestPath()
        {
            HardPath(actOne.startNode, new List<Node>() { actOne.startNode }, new List<Node>());
            PrintPath(actOne.possiblePaths.Last());
        }

        //Using Prim's Algorithm in an Recursive Method
        //References: https://youtu.be/K_1urzWrzLs
        //look at a node and find the most expensive node from the neighbours,
        //if there are multiple expensive nodes recursive function with the other expensive node as the current node in the parameter
        void HardPath(Node current, List<Node> toLookAt, List<Node> visited)
        {
            //add current to visited list
            //if current is end node
            //  return
            //for loop the neighbour nodes of current
            //  find the most expensive node
            //  if neighbour has not been visited
            //      set the neighbour's parent to current
            //find other same cost nodes from expensive node
            //  recursive function with the other nodes

            visited.Add(current);
            toLookAt.Remove(current);

            Node[] neighbourNodes;
            Node expensiveNode = GetExpensiveNode(current, visited, out neighbourNodes);
            if (!string.IsNullOrEmpty(expensiveNode.type))
            {
                if(expensiveNode.type == "E")
                {
                    endNode = expensiveNode;
                    visited.Add(expensiveNode);
                    BuildPath(visited);
                    return;
                }

                foreach (var neighbour in neighbourNodes)
                    if (neighbour.row != expensiveNode.row && neighbour.col != expensiveNode.col)
                        if (!IsInList(neighbour, toLookAt) && !IsInList(neighbour, visited))
                        {
                            neighbour.SetParentPos(current.row, current.col);
                            toLookAt.Add(neighbour);
                        }

                HardPath(expensiveNode, toLookAt, visited);
            }
            else
            {
                if (toLookAt.Count > 0)
                {
                    expensiveNode = new Node();
                    foreach (var node in toLookAt)
                        if (node.cost > expensiveNode.cost)
                            expensiveNode = node;
                    HardPath(expensiveNode, toLookAt, visited);
                }
            }
        }

        Node GetExpensiveNode(Node current, List<Node> visited, out Node[] neighbourNodes)
        {
            Node expensiveNode = new Node();
            neighbourNodes = GetAdjacentNeighbours(current);
            foreach (var neighbour in neighbourNodes)
            {
                if (!IsInList(neighbour, visited))
                {
                    neighbour.SetParentPos(current.row, current.col);
                    if (neighbour.cost > expensiveNode.cost)
                        expensiveNode = neighbour;

                    if (neighbour.type == "E")
                        return neighbour;
                }
            }

            return expensiveNode;
        }
        #endregion

        #region Other Methods
        //loop through the gridMap to find the Start Point and set it as the first nodes to visit
        public void FindStartingPoint()
        {
            for (int row = 0; row < gridRowSize; row++)
                for (int col = 0; col < gridColSize; col++)
                    if (gridMap[row][col].type == "S")
                    {
                        startNode = gridMap[row][col];
                        startNode.costToPos = 0;
                        return;
                    }
        }

        //Creates a path that first reached the End Point by looping through the cameFrom lsit backwards
        //and checking if the current node is connected to any node in the list
        //once the node reaches the starting point, reverse the finalPath and display it
        void BuildPath(List<Node> parents)
        {
            List<Node> finalPath = new List<Node>();
            finalPath.Add(endNode);
            for (int i = parents.Count - 1; i >= 0; i--)
            {
                if (IsParent(finalPath.Last(), parents[i].row, parents[i].col))
                    finalPath.Add(parents[i]);

                if (parents[i].type == "S")
                    break;
            }
            finalPath.Reverse();

            possiblePaths.Add(finalPath);
        }

        void PrintPath(List<Node> path)
        {
            foreach (var p in path)
                Console.Write($"({p.Position()})");
            Console.WriteLine();
        }

        bool IsParent(Node current, int pRow, int pCol)
        {
            if (current.parentRow == pRow && current.parentCol == pCol)
                return true;
            return false;
        }

        bool IsInList(Node neighbour, List<Node> list)
        {
            foreach (Node vNode in list)
                if (vNode.row == neighbour.row && vNode.col == neighbour.col)
                    return true;
            return false;
        }

        //Returns an array of nodes adjacent to the current node
        Node[] GetAdjacentNeighbours(Node current)
        {
            List<Node> unvisitedNeighbours = new List<Node>();

            for (int i = 0; i < 4; i++)
            {
                int newRow = current.row + dirRow[i];
                int newCol = current.col + dirCol[i];

                //check if the positions are in bounds
                if (newRow < 0 || newCol < 0)
                    continue;
                if (newRow > gridRowSize - 1 || newCol > gridColSize - 1)
                    continue;
                //check if the node at that position is an obstacle
                if (gridMap[newRow][newCol].type == "O")
                    continue;

                Node node = gridMap[newRow][newCol];
                node.row = newRow;
                node.col = newCol;
                unvisitedNeighbours.Add(node);
            }

            return unvisitedNeighbours.ToArray();
        }
        #endregion
    }
}
