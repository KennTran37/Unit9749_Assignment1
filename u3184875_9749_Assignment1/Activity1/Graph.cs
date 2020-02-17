using System;

namespace Activity1
{
    //Source: https://stackoverflow.com/a/45221670
    public static class Graph
    {
        public static int spaceInCol = 7;
        public static int spaceInRow = 3;

        public static int gridCol = Program.mainClass.GridColSize;

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
                    Node current = Program.mainClass.GridMap[row][col];
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
                                WriteYellow(type);
                            else if (type == "E")
                                WriteRed(type);
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

        public static void WriteRed(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void WriteYellow(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
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
