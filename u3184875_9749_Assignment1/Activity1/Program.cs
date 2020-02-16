using System;
using System.Collections.Generic;
using System.Linq;

namespace Activity1
{
    class Program
    {
        public static Program mainClass = new Program();

        Position[] typesArray =
        {
            new Position("S", 0), new Position("E", 0), new Position("W40", 5), new Position("W90", 9), new Position("W120", 5), new Position("W0", 1),
            new Position("W", 1), new Position("O", int.MaxValue), new Position("Ww", 6), new Position("Wg", 3), new Position("Wr", 4)
        };

        //public List<List<string>> gridMap = new List<List<string>>();
        public List<List<Position>> posGridMap = new List<List<Position>>();

        int gridRowSize = 5;
        int gridColSize = 5;

        int[] dirRow = { -1, 1, 0, 0 }; //direction in row
        int[] dirCol = { 0, 0, 1, -1 }; //direction in coloumn

        Position startPosition;
        Position endPosition;

        List<Position> visitedCells = new List<Position>();
        List<Position> cameFrom = new List<Position>();
        Queue<Position> cellsToVisit = new Queue<Position>();
        bool endFound = false;

        static void Main(string[] args)
        {
            string userInput = mainClass.GetUserInput();

            mainClass.FindStartingPoint();
            mainClass.ShortestPath();

            mainClass.ResetLists();
            mainClass.EasiestPath();

            Console.ReadKey();
        }

        //Resetting lists and queue for the next Algorithm to use
        void ResetLists()
        {
            endFound = false;
            cellsToVisit = new Queue<Position>();
            visitedCells = new List<Position>();
            cameFrom = new List<Position>();
            cellsToVisit.Enqueue(startPosition);
            visitedCells.Add(startPosition);
            cameFrom.Add(startPosition);
        }

        #region Checking User's Input
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
            int startIndex = 0;
            int prevColCount = 0;
            string sub = null;
            int numOfS = 0;
            int numOfE = 0;

            mainClass.posGridMap = new List<List<Position>>();
            foreach (string row in rowList)
            {
                posGridMap.Add(new List<Position>());
                startIndex = 0;
                int currentColCount = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i] == ',' || i == row.Length - 1)
                    {
                        sub = row.Substring(startIndex, (i + (i == row.Length - 1 ? 1 : 0)) - startIndex);
                        if (!IsValidType(sub))
                        {
                            DisplayError($"Your Input: [{sub}] is invalid -- \n {row}");
                            return false;
                        }

                        if (sub == "S")
                            numOfS++;
                        if (sub == "E")
                            numOfE++;

                        Position pos = GetPositionType(sub);
                        pos.row = rowList.IndexOf(row);
                        pos.col = currentColCount;
                        mainClass.posGridMap.Last().Add(pos);

                        startIndex += sub.Length + 1;
                        currentColCount++;
                    }
                }

                if (prevColCount == 0)
                    prevColCount = currentColCount;

                if (prevColCount < 2)
                {
                    DisplayError("A Column in your input is smaller then 2 \n Columns can only be a minimum of 2 ");
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
            foreach (Position type in typesArray)
                if (userType == type.type)
                    return true;
            return false;
        }
        #endregion

        #region Shortest Path w/ Breath First Search
        //Finds the shortest path to the End Point using the 'Breadth First Search Algorithm'
        //While the cellsToVisit queue isnt empty, look at the neighbouring cells at the first element of the queue
        //
        //Reference: https://www.redblobgames.com/pathfinding/a-star/introduction.html
        //           https://www.youtube.com/watch?v=oDqjPvD54Ss
        //           https://www.youtube.com/watch?v=-L-WgKMFuhE&t=125s
        void ShortestPath()
        {
            while (cellsToVisit.Count > 0)
            {
                Position current = cellsToVisit.Dequeue();
                foreach (Position pos in GetNeighbours(current))
                {
                    if (!CheckIfVisited(pos))
                    {
                        cellsToVisit.Enqueue(pos);
                        visitedCells.Add(pos);
                        if (!cameFrom.Contains(current))
                            cameFrom.Add(current);
                    }

                    if (pos.type == "E")
                    {
                        endPosition = pos;
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

        //Creates the path that first reached the End Point by looping backwards
        //and checking if the position is connected to the parent
        //once the position reaches the starting point, reverse the finalPath and display it
        void ConstructFinalPath(string message)
        {
            Console.WriteLine("\n\n");
            List<Position> finalPath = new List<Position>();
            finalPath.Add(endPosition);
            for (int i = cameFrom.Count - 1; i >= 0; i--)
            {
                //if the last added element(position) is connected to the current i position
                if (IsParent(finalPath.Last(), cameFrom[i]))
                    finalPath.Add(cameFrom[i]);

                if (posGridMap[cameFrom[i].row][cameFrom[i].col].type == "S")
                    break;
            }
            finalPath.Reverse();

            Console.WriteLine($"{message}: {finalPath.Count}");
            foreach (var p in finalPath)
                Console.Write($"{p.row}, {p.col} |");
        }

        bool IsParent(Position current, Position parent)
        {
            for (int i = 0; i < 4; i++)
                if (current.col + dirCol[i] == parent.col && current.row + dirRow[i] == parent.row)
                    return true;

            return false;
        }

        bool CheckIfVisited(Position neighbour)
        {
            foreach (Position vPos in visitedCells)
                if (vPos.row == neighbour.row && vPos.col == neighbour.col)
                    return true;
            return false;
        }

        //Returns an array of positions adjacent to the current position
        Position[] GetNeighbours(Position current)
        {
            List<Position> unvisitedNeighbours = new List<Position>();

            for (int i = 0; i < 4; i++)
            {
                int newRow = current.row + dirRow[i];
                int newCol = current.col + dirCol[i];

                //check if the positions are in bounds
                if (newRow < 0 || newCol < 0)
                    continue;
                if (newRow > gridRowSize - 1 || newCol > gridColSize - 1)
                    continue;
                //check if position is an obstacle
                if (posGridMap[newRow][newCol].type == "O")
                    continue;

                Position pos = posGridMap[newRow][newCol];
                pos.row = newRow;
                pos.col = newCol;
                unvisitedNeighbours.Add(pos);
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
            //find neighbour cells from start
            //add them to tolookat list
            //set the cost of reaching them

            //initializing the list with the neighbours of Start Point
            List<Position> toLookAt = new List<Position>();
            foreach (Position neighbour in GetNeighbours(startPosition))
            {
                Position nPos = minCostPosition(startPosition, neighbour);
                toLookAt.Add(nPos);
            }

            //in a while loop
            //loop through the tolookat list and find the cheapest cell
            //check the cost of reaching that cell
            //if it's cheap set it as that value
            //find neighbour cells from cheapest
            while (toLookAt.Count > 0)
            {
                Position cheapestPos = toLookAt[0];
                foreach (Position toLookPos in toLookAt)
                    if (toLookPos.cost < cheapestPos.cost)
                        cheapestPos = toLookPos;

                foreach (Position neighbour in GetNeighbours(cheapestPos))
                {
                    if (!CheckIfVisited(neighbour))
                    {
                        Position nPos = minCostPosition(cheapestPos, neighbour);
                        toLookAt.Add(nPos);

                        if (!cameFrom.Contains(cheapestPos))
                            cameFrom.Add(cheapestPos);
                    }

                    if (neighbour.type == "E")
                    {
                        endPosition = neighbour;
                        endFound = true;
                        break;
                    }
                }

                toLookAt.Remove(cheapestPos);
                visitedCells.Add(cheapestPos);

                if (endFound)
                {
                    ConstructFinalPath("Steps taken for Easiest Path");
                    break;
                }
            }
        }

        Position minCostPosition(Position parent, Position current)
        {
            Position pos = current;
            if (parent.costToPos + current.cost < current.costToPos)
                pos.costToPos = parent.costToPos + current.cost;
            //Console.WriteLine($"{pos.type} || {pos.row},{pos.col} || {pos.costToPos}");
            return pos;
        }
        #endregion

        //loop through the gridMap to find the Start Point and set it as the first cell to visit
        public void FindStartingPoint()
        {
            for (int row = 0; row < gridRowSize; row++)
                for (int col = 0; col < gridColSize; col++)
                    if (posGridMap[row][col].type == "S")
                    {
                        startPosition = posGridMap[row][col];
                        startPosition.costToPos = 0;
                        cellsToVisit.Enqueue(startPosition);
                        visitedCells.Add(startPosition);
                        cameFrom.Add(startPosition);
                        return;
                    }
        }

        Position GetPositionType(string type)
        {
            foreach (Position pos in typesArray)
                if (pos.type == type)
                    return pos;

            return new Position();
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

    struct Position
    {
        public string type;
        public int col; //column
        public int row; //row
        public int cost;
        public int costToPos;

        public Position(string type, int col, int row)
        {
            this.type = type;
            this.col = col;
            this.row = row;

            cost = 0;
            costToPos = int.MaxValue;
        }

        public Position(string type, int cost)
        {
            this.type = type;
            this.col = 0;
            this.row = 0;

            this.cost = cost;
            costToPos = int.MaxValue;
        }
    }
}
