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
        public static Program mainClass = new Program();
        public static Random randomClass = new Random();

        Node[] typesArray =
        {
            new Node("S", 0), new Node("E", 0), new Node("W40", 5), new Node("W90", 9), new Node("W120", 5), new Node("W0", 1),
            new Node("W", 1), new Node("O", int.MaxValue), new Node("Ww", 6), new Node("Wg", 3), new Node("Wr", 4)
        };

        public Node[] TypesArray { get => typesArray; }

        List<List<Node>> gridMap = new List<List<Node>>();
        public static List<List<Node>> GridMap 
        { 
            get => mainClass.gridMap; 
            set => mainClass.gridMap = value; 
        }

        int gridRowSize = 3;
        int gridColSize = 3;
        public int GridColSize { get => gridColSize; set => gridColSize = value; }
        public int GridRowSize { get => gridRowSize; set => gridRowSize = value; }

        //used to look at the neighbouring nodes
        int[] dirRow = { -1, 1, 0, 0 }; //direction in row
        int[] dirCol = { 0, 0, 1, -1 }; //direction in coloumn

        Node startNode;
        Node endNode;

        List<List<Node>> possiblePaths = new List<List<Node>>();
        List<Node> visitedNodes = new List<Node>();
        List<Node> cameFrom = new List<Node>();
        Queue<Node> nodesToVisit = new Queue<Node>();
        bool endFound = false;

        static void Main(string[] args)
        {
            string userInput = UserInput.GetUserInput();
            Console.Clear();
            Console.WriteLine($"\n\t{userInput}\n");

            mainClass.FindStartingPoint();
            Console.WriteLine("\nShortest Path");
            mainClass.ShortestPath();

            Console.WriteLine("\nEasiet Path");
            mainClass.EasiestPath();
            mainClass.PrintPath(mainClass.possiblePaths.Last());

            Console.WriteLine("\nAverage Path");
            mainClass.AveragePath();

            Console.WriteLine("\nHardest Path");
            mainClass.HardestPath();
            Console.WriteLine();

            Graph.DrawTop();
            for (int row = 0; row < mainClass.gridRowSize; row++)
            {
                Graph.DrawMidSpacer(row);
                if (row < mainClass.gridRowSize - 1)
                    Graph.DrawMiddle();
            }
            Graph.DrawBottom();

            Console.ReadKey();
        }

        #region Shortest Path - Breath First Search
        void ShortestPath()
        {
            //starting the search from the neighbours of startPoint because it will give more results
            List<List<Node>> possibleShortestPaths = new List<List<Node>>();
            foreach (Node neighbour in GetNeighbours(startNode))
            {
                Queue<Node> toVisit = new Queue<Node>();
                toVisit.Enqueue(neighbour);

                List<Node> path = GetShortestPath(toVisit, new List<Node>() { startNode, neighbour }, new List<Node>() { startNode, neighbour });
                if (path != null)
                    possibleShortestPaths.Add(path);
            }

            List<Node> shortestPath = possibleShortestPaths.First();
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
        List<Node> GetShortestPath(Queue<Node> toVisit, List<Node> visited, List<Node> parents)
        {
            while (toVisit.Count > 0)
            {
                Node current = toVisit.Dequeue();
                foreach (Node neighbour in GetNeighbours(current))
                {
                    if (!HasVisited(neighbour, visited))
                    {
                        toVisit.Enqueue(neighbour);
                        visited.Add(neighbour);
                        if (!parents.Contains(current))
                            parents.Add(current);
                    }

                    if (neighbour.type == "E")
                    {
                        endNode = neighbour;
                        //BuildPath(parents);
                        return parents;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Easiest Path - Dijkstra's Algorithm
        //Dijkstra's Algorithm
        //References: https://youtu.be/K_1urzWrzLs
        void EasiestPath()
        {
            //init
            //new list of tolookat
            //find neighbour nodes from start
            //add them to tolookat list
            //set the cost of reaching them

            //initializing the list with the neighbours of startPoint
            List<Node> toLookAt = new List<Node>();
            foreach (Node neighbour in GetNeighbours(startNode))
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
                Node cheapestNode = toLookAt[0];
                foreach (Node toLookNode in toLookAt)
                    if (toLookNode.cost < cheapestNode.cost)
                        cheapestNode = toLookNode;

                foreach (Node neighbour in GetNeighbours(cheapestNode))
                {
                    if (!HasVisited(neighbour, visitedNodes))
                    {
                        Node neighbourNode = minCostNode(cheapestNode, neighbour);
                        toLookAt.Add(neighbourNode);

                        if (!cameFrom.Contains(cheapestNode))
                            cameFrom.Add(cheapestNode);
                    }

                    if (neighbour.type == "E")
                    {
                        endNode = neighbour;
                        endFound = true;
                        break;
                    }
                }

                toLookAt.Remove(cheapestNode);
                visitedNodes.Add(cheapestNode);

                if (endFound)
                {
                    BuildPath(cameFrom);
                    break;
                }
            }
        }

        //checking if the cost to the node is smaller than it already is
        Node minCostNode(Node parent, Node current)
        {
            if (parent.costToPos + current.cost < current.costToPos)
                current.costToPos = parent.costToPos + current.cost;
            return current;
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
            GetHardestPath(null, new List<Node>());

            List<Node> hardestPath = possiblePaths.First();
            foreach (var path in possiblePaths)
            {
                if (path.Count == hardestPath.Count)
                {
                    if (totalPathCost(path) > totalPathCost(hardestPath))
                        hardestPath = path;
                    continue;
                }

                if (path.Count > hardestPath.Count)
                    hardestPath = path;
            }

            BuildPath(hardestPath);
            PrintPath(possiblePaths.Last());
        }

        int totalPathCost(List<Node> path)
        {
            int totalCost = 0;
            foreach (var node in path)
                totalCost += node.cost;

            return totalCost;
        }

        //Using Prim's Algorithm in an Recursive Method
        void GetHardestPath(List<Node> toLookAt, List<Node> visited)
        {
            //init toLookAt list with startNode neighbours
            //add startNode to visited list
            //while toLookAt isn't empty
            //  find the most expensive node assigned to expensiveNode
            //  add it to visited list and remove from toLookAt list
            //  look at the neighbours from expensiveNode
            //      check if neighbour hasn't been visited
            //          add neighbour to toLookAt list
            //      check if neighbour is end point
            //          assign neighbour to endNode
            //          add to visited list then call BuildPath with visited as the param 
            //  looping through the toLookAt list
            //      check if other nodes have the same cost as expensiveNode
            //          Recursive funcation with toLookAt and visited lists as the param

            if (toLookAt is null)
            {
                toLookAt = new List<Node>();
                visited.Add(startNode);
                foreach (Node neighbour in GetNeighbours(startNode))
                    toLookAt.Add(neighbour);
            }

            while (toLookAt.Count > 0)
            {
                Node expensiveNode = toLookAt.Last();
                foreach (Node lookAtNode in toLookAt)
                    if (lookAtNode.cost > expensiveNode.cost)
                        expensiveNode = lookAtNode;

                visited.Add(expensiveNode);
                toLookAt.Remove(expensiveNode);

                foreach (Node neighbour in GetNeighbours(expensiveNode))
                {
                    if (!HasVisited(neighbour, visited))
                        toLookAt.Add(neighbour);

                    if (neighbour.type == "E")
                    {
                        endNode = neighbour;
                        visited.Add(neighbour);
                        BuildPath(visited);
                        return;
                    }
                }

                for (int i = toLookAt.Count - 1; i >= 0; i--)
                {
                    if (toLookAt.Count == 0)
                        break;
                    if (toLookAt[i].cost == expensiveNode.cost)
                        GetHardestPath(toLookAt, visited);
                }
            }
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
                        nodesToVisit.Enqueue(startNode);
                        visitedNodes.Add(startNode);
                        cameFrom.Add(startNode);
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
                //if the last added element(node) is connected to the current i node
                if (IsParent(finalPath.Last(), parents[i]))
                    finalPath.Add(parents[i]);

                if (gridMap[parents[i].row][parents[i].col].type == "S")
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

        bool IsParent(Node current, Node parent)
        {
            for (int i = 0; i < 4; i++)
                if (current.col + dirCol[i] == parent.col && current.row + dirRow[i] == parent.row)
                    return true;

            return false;
        }

        bool HasVisited(Node neighbour, List<Node> visitedList)
        {
            foreach (Node vNode in visitedList)
                if (vNode.Position() == neighbour.Position())
                    return true;
            return false;
        }

        //Returns an array of nodes adjacent to the current node
        Node[] GetNeighbours(Node current)
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

        public Node GetNodeType(string type)
        {
            foreach (Node node in typesArray)
                if (node.type == type)
                    return node;
            return new Node();
        }
        #endregion
    }
}
