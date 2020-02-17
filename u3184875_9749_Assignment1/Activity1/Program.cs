using System;
using System.Collections.Generic;
using System.Linq;

//This script takes the input of the user and turns it into a 2D graph when valid
//it will then use different Algorithm to find the Shortest, Easiest, Average, and Hardest path
//to tavel to reach the End Point from the Start Point in the user's graph
namespace Activity1
{
    class Program
    {
        public static Program mainClass = new Program();

        Node[] typesArray =
        {
            new Node("S", 0), new Node("E", 0), new Node("W40", 5), new Node("W90", 9), new Node("W120", 5), new Node("W0", 1),
            new Node("W", 1), new Node("O", int.MaxValue), new Node("Ww", 6), new Node("Wg", 3), new Node("Wr", 4)
        };

        List<List<Node>> gridMap = new List<List<Node>>();
        public List<List<Node>> GridMap { get => gridMap; }

        int gridRowSize = 3;
        int gridColSize = 3;
        public int GridColSize { get => gridColSize; }
        public int GridRowSize { get => gridRowSize; }

        //used to look at the neighbouring nodes
        int[] dirRow = { -1, 1, 0, 0 }; //direction in row
        int[] dirCol = { 0, 0, 1, -1 }; //direction in coloumn

        Node startNode;
        Node endNode;

        List<Node> visitedNodes = new List<Node>();
        List<Node> cameFrom = new List<Node>();
        Queue<Node> nodesToVisit = new Queue<Node>();
        bool endFound = false;

        static void Main(string[] args)
        {
            string userInput = mainClass.GetUserInput();

            //mainClass.FindStartingPoint();
            //mainClass.ShortestPath();

            //mainClass.ResetLists();
            //mainClass.EasiestPath();

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

        //Resetting lists and queue for the next Algorithm to use
        void ResetLists()
        {
            endFound = false;
            nodesToVisit = new Queue<Node>();
            visitedNodes = new List<Node>();
            cameFrom = new List<Node>();
            nodesToVisit.Enqueue(startNode);
            visitedNodes.Add(startNode);
            cameFrom.Add(startNode);
        }

        #region Checking User's Input
        //Returns the user's input after checking in a while loop,if the input is null or empty,
        //if the rows and columns have a valid amount, and whether the types are valid
        string GetUserInput()
        {
            string input = null;

            while (true)
            {
                Console.WriteLine("--Enter your grid for the AI to travel--" +
                    "\n Your grid should have columns next to each other, seperated by commas(,) and the rows should be seperated by spaces( )" +
                    "\n The grid should have a minimum of 2 columns or rows" +
                    "\n The grid MUST contain a Starting Point(marked as S) and an Ending Point(marked as E)" +
                    "\n For Example: S,W0,W0 W0,O,E W0,W0,W0");
                //cut up the input into rows and columns and check if the types(symbols) are valid
                //check if the input only contains a single Starting Point and an Ending Point
                //check if each row's columns are the same length

                input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    DisplayError("Your Input is Empty \n Please enter a valid Input");
                    continue;
                }

                List<string> rowsList = new List<string>();
                if (!IsValidAmountOfRows(input, out rowsList))
                    continue;

                if (!IsValidColumnsAndTypes(rowsList))
                    continue;

                break;
            }

            return input;
        }

        void DisplayError(string errorOutput)
        {
            Console.WriteLine(errorOutput);
            Console.ReadKey();
            Console.Clear();
        }

        //Checks if the number of rows is greater then 1
        bool IsValidAmountOfRows(string input, out List<string> rowList)
        {
            int startIndex = 0;
            rowList = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {   //finds the space within the input or when the loop reaches the end
                //cutting everything before the space and putting it into the list
                if (input[i] == ' ' || i == input.Length - 1)
                {
                    string sub = input.Substring(startIndex, (i + (i == input.Length - 1 ? 1 : 0)) - startIndex);
                    rowList.Add(sub);
                    startIndex += sub.Length + 1;
                }
            }

            if (rowList.Count < 2)
            {
                DisplayError("Invalid grid \n grid cannot have 1 row");
                return false;
            }

            return true;
        }

        bool IsValidColumnsAndTypes(List<string> rowList)
        {
            int prevColCount = 0;
            int startIndex = 0;
            string sub = null;
            //making sure that there is only one Start and End Point
            int numOfS = 0;
            int numOfE = 0;

            mainClass.gridMap = new List<List<Node>>();
            foreach (string row in rowList)
            {
                gridMap.Add(new List<Node>());
                startIndex = 0;
                int currentColCount = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] == ',' || i == row.Length - 1)
                    {
                        sub = row.Substring(startIndex, (i + (i == row.Length - 1 ? 1 : 0)) - startIndex);
                        //cutting up the row of strings then checking to see if it is a valid type
                        if (!IsValidType(sub))
                        {
                            DisplayError($"Your Input: [{sub}] is invalid -- \n {row}");
                            return false;
                        }

                        if (sub == "S")
                            numOfS++;
                        if (sub == "E")
                            numOfE++;

                        Node node = GetNodeType(sub);
                        node.row = rowList.IndexOf(row);
                        node.col = currentColCount;
                        mainClass.gridMap.Last().Add(node);

                        startIndex += sub.Length + 1;
                        currentColCount++;
                    }
                }

                if (prevColCount == 0)
                    prevColCount = currentColCount;

                if (prevColCount < 2)
                {
                    DisplayError("A Column in your input is smaller then 2.. \n Columns can only be a minimum of 2 ");
                    return false;
                }
                else
                {
                    if (prevColCount != currentColCount)
                    {
                        DisplayError("Each Row's Column must be the same Length");
                        return false;
                    }
                }
            }

            if (numOfE != 1 || numOfS != 1)
            {
                DisplayError("There must be only one Starting Point (S) and one Ending Point (E)");
                return false;
            }

            mainClass.gridColSize = prevColCount;
            mainClass.gridRowSize = rowList.Count;

            return true;
        }

        //Loop through the array of types and check if the user's type is valid
        bool IsValidType(string userType)
        {
            foreach (Node type in typesArray)
                if (userType == type.type)
                    return true;
            return false;
        }
        #endregion

        #region Shortest Path w/ Breath First Search
        //Finds the shortest path to the End Point using the 'Breadth First Search Algorithm'
        //While the nodesToVisit queue isnt empty, look at the neighbouring node at the first element of the queue
        //Reference: https://www.redblobgames.com/pathfinding/a-star/introduction.html
        //           https://www.youtube.com/watch?v=oDqjPvD54Ss
        //           https://www.youtube.com/watch?v=-L-WgKMFuhE&t=125s
        void ShortestPath()
        {
            while (nodesToVisit.Count > 0)
            {
                Node current = nodesToVisit.Dequeue();
                foreach (Node neighbour in GetNeighbours(current))
                {
                    if (!CheckIfVisited(neighbour))
                    {
                        nodesToVisit.Enqueue(neighbour);
                        visitedNodes.Add(neighbour);
                        if (!cameFrom.Contains(current))
                            cameFrom.Add(current);
                    }

                    if (neighbour.type == "E")
                    {
                        endNode = neighbour;
                        endFound = true;
                        break;
                    }
                }

                if (endFound)
                {
                    ConstructFinalPath("Steps taken for shortest path");
                    break;
                }
            }
        }

        bool IsParent(Node current, Node parent)
        {
            for (int i = 0; i < 4; i++)
                if (current.col + dirCol[i] == parent.col && current.row + dirRow[i] == parent.row)
                    return true;

            return false;
        }

        bool CheckIfVisited(Node neighbour)
        {
            //vNode = visited Node
            foreach (Node vNode in visitedNodes)
                if (vNode.row == neighbour.row && vNode.col == neighbour.col)
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
        #endregion

        #region Easiest Path \w Prim's Algorithm
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
                    if (!CheckIfVisited(neighbour))
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
                    ConstructFinalPath("Steps taken for Easiest Path");
                    break;
                }
            }
        }

        //checking if the cost to the node smaller than it already is
        Node minCostNode(Node parent, Node current)
        {
            if (parent.costToPos + current.cost < current.costToPos)
                current.costToPos = parent.costToPos + current.cost;
            return current;
        }
        #endregion

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
        void ConstructFinalPath(string message)
        {
            Console.WriteLine("\n\n");
            List<Node> finalPath = new List<Node>();
            finalPath.Add(endNode);
            for (int i = cameFrom.Count - 1; i >= 0; i--)
            {
                //if the last added element(node) is connected to the current i node
                if (IsParent(finalPath.Last(), cameFrom[i]))
                    finalPath.Add(cameFrom[i]);

                if (gridMap[cameFrom[i].row][cameFrom[i].col].type == "S")
                    break;
            }
            finalPath.Reverse();

            Console.WriteLine($"{message}: {finalPath.Count}");
            foreach (var p in finalPath)
                Console.Write($"{p.row}, {p.col} |");
        }

        Node GetNodeType(string type)
        {
            foreach (Node node in typesArray)
                if (node.type == type)
                    return node;
            return new Node();
        }

        void DisplayLegend()
        {
            //S - Represents the STARTING Square
            //E - Represents the ENDING Sqaure
            //W# - 'W' Represents walkable squares and '#' represents gradient in degrees
            // # = 40 - Means walkable squares are UPHILLS
            // # = 90 - Means walkable squares are WALLS
            // # = 120 - Means walkable squares are DOWNHILLS
            // W0 - Means walkable squares are zero degrees gradient
            //O - Represents OBSTACLES Sqaures
            //Ww - Represents walkable squares via WATER
            //Wg - Represents walkable squares via GRASS
            //Wr - Represents ROCKY walkable squares
        }
    }
}
