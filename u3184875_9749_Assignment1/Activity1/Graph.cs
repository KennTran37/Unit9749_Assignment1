﻿using System;
using System.Collections.Generic;

namespace Activity1
{
    //Source: https://stackoverflow.com/a/45221670
    public class Graph
    {
        public static int spaceInCol = 7;
        public static int spaceInRow = 3;

        static List<List<Node>> gridMap = new List<List<Node>>();
        public static int gridCol;

        public static void BuildGraph(List<List<Node>> map ,int gridRow, int _gridCol)
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
                    Node current = gridMap[row][col];
                    for (int spaceCol = 0; spaceCol < newSpaceInCol; spaceCol++)
                    {
                        if (spaceRow == 0 && spaceCol == 1)
                        {
                            string position = $"({current.row},{current.col})";
                            Console.Write(position);
                            newSpaceInCol = spaceInCol - position.Length + 1;
                        }
                        else if (spaceRow == 1 && spaceCol == 1)
                        {
                            string type = $"{current.type}";
                            if (type == "S")
                                WriteColour(type, ConsoleColor.Yellow);
                            else if (type == "E")
                                WriteColour(type, ConsoleColor.Red);
                            else
                                Console.Write(type);
                            newSpaceInCol = spaceInCol - type.Length + 1;
                        }
                        else
                        {
                            Console.BackgroundColor = SetColourOnType(current.type);
                            Console.Write(" ");
                            Console.ResetColor();
                        }
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

        public static ConsoleColor SetColourOnType(string type)
        {
            if (type == "S")
                return ConsoleColor.Yellow;
            if (type == "E")
                return ConsoleColor.Red;
            if (type == "W40" || type == "W120")
                return ConsoleColor.Gray;
            if (type == "W90")
                return ConsoleColor.DarkGray;
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

            return ConsoleColor.Black;
        }
    }
}
