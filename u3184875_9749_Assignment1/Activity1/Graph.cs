using System;
using System.Collections.Generic;

namespace Activity1
{
    //Source: https://stackoverflow.com/a/45221670
    public class Graph
    {
        public static int spaceInCol = 3;
        public static int spaceInRow = 1;

        static List<List<Node>> gridMap = new List<List<Node>>();
        static List<List<GridNode>> matrixMap = new List<List<GridNode>>();
        public static int gridCol;
        public static int gridRow;

        public static void SetUpMap(List<List<Node>> map, int _gridRow, int _gridCol)
        {
            gridCol = _gridCol;
            gridRow = _gridRow;

            for (int row = 0; row < gridRow; row++)
            {
                matrixMap.Add(new List<GridNode>());
                for (int col = 0; col < gridCol; col++)
                {
                    GridNode gNode = new GridNode();
                    gNode.node = map[row][col];
                    matrixMap[row].Add(gNode);
                }
            }

            BuildMap();
        }

        public static void UpdateMap(int row, int col, NodeState newState)
        {
            GridNode gNode = matrixMap[row][col];
            gNode.state = newState;
            matrixMap[row][col] = gNode;

            Console.Clear();
            BuildMap();
        }

        public static void ResetMap()
        {
            for (int row = 0; row < gridRow; row++)
            {
                for (int col = 0; col < gridCol; col++)
                {
                    GridNode gNode = matrixMap[row][col];
                    gNode.state = NodeState.Null;
                    matrixMap[row][col] = gNode;
                }
            }
        }

        static void BuildMap()
        {
            DrawTop();
            for (int i = 0; i < gridRow; i++)
            {
                DrawMidSpacer(i);
                if (i < gridRow - 1)
                    DrawMiddle();
            }
            DrawBottom();
        }

        public static void BuildGraph(List<List<Node>> map, int gridRow, int _gridCol)
        {
            gridCol = _gridCol;
            gridMap = map;

            DrawTop();
            for (int row = 0; row < gridRow; row++)
            {
                DrawMidSpacer(row);
                if (row < gridRow - 1)
                    DrawMiddle();
            }
            DrawBottom();
        }

        public static void BuildGraph(Node[,] map, int gridRow, int _gridCol)
        {
            gridCol = _gridCol;
            for (int row = 0; row < gridRow; row++)
            {
                gridMap.Add(new List<Node>());
                for (int col = 0; col < gridCol; col++)
                    gridMap[row].Add(map[row, col]);
            }

            DrawTop();
            for (int row = 0; row < gridRow; row++)
            {
                DrawMidSpacer(row);
                if (row < gridRow - 1)
                    DrawMiddle();
            }
            DrawBottom();
        }

        public static void DrawTop()
        {
            Console.Write("┌");
            for (int col = 0; col < gridCol; col++)
            {
                for (int spaceCol = 0; spaceCol < spaceInCol; spaceCol++)
                    Console.Write("─");
                if (col < gridCol - 1)
                    Console.Write("┬");
            }
            Console.Write("┐");
            Console.WriteLine();
        }

        public static void DrawMidSpacer(int row)
        {
            for (int spaceRow = 0; spaceRow < spaceInRow; spaceRow++)
            {
                Console.Write("│");
                for (int col = 0; col < gridCol; col++)
                {
                    int newSpaceInCol = spaceInCol;
                    for (int spaceCol = 0; spaceCol < newSpaceInCol; spaceCol++)
                    {
                        Console.BackgroundColor = SetColour(matrixMap[row][col]);
                        Console.Write(" ");
                        Console.ResetColor();
                    }
                    Console.Write("│");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        public static void DrawMiddle()
        {
            Console.Write("├");
            for (int col = 0; col < gridCol; col++)
            {
                for (int spaceCol = 0; spaceCol < spaceInCol; spaceCol++)
                    Console.Write("─");
                if (col < gridCol - 1)
                    Console.Write("┼");
            }
            Console.Write("┤");
            Console.WriteLine();
        }

        public static void DrawBottom()
        {
            Console.Write("└");
            for (int col = 0; col < gridCol; col++)
            {
                for (int spaceCol = 0; spaceCol < spaceInCol; spaceCol++)
                    Console.Write("─");
                if (col < gridCol - 1)
                    Console.Write("┴");
            }
            Console.Write("┘");
        }

        public static void WriteColour(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static ConsoleColor SetColour(GridNode gridNode)
        {
            if (gridNode.state == NodeState.toVisit)
                return ConsoleColor.DarkRed;
            if (gridNode.state == NodeState.visited)
                return ConsoleColor.DarkGreen;

            string type = gridNode.node.type;
            if (type == "S")
                return ConsoleColor.Yellow;
            if (type == "E")
                return ConsoleColor.Red;
            if (type == "W" || type == "W0")
                return ConsoleColor.Black;
            if (type == "O")
                return ConsoleColor.White;
            if (type == "Ww")
                return ConsoleColor.Blue;
            if (type == "Wg")
                return ConsoleColor.Green;
            if (type == "Wr")
                return ConsoleColor.Cyan;

            if (type[0] == 'W')
            {
                int num = int.Parse(type.Substring(1, type.Length - 1));
                if (num <= 40 || num > 90 && num <= 120)
                    return ConsoleColor.Gray;
                if (num <= 90)
                    return ConsoleColor.DarkGray;
            }

            if (type[0] == 'I')
                return ConsoleColor.DarkMagenta;

            return ConsoleColor.Black;
        }
    }

    public struct GridNode
    {
        public Node node;
        public NodeState state;
    }

    public enum NodeState
    {
        Null, toVisit, visited
    }
}
