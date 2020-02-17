using System;

namespace Activity1
{
    //Source: https://stackoverflow.com/a/45221670
    public static class Graph
    {
        public static int spaceInCol = 11;
        public static int spaceInRow = (spaceInCol / 2) - 1;

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
                    for (int spaceCol = 0; spaceCol < newSpaceInCol; spaceCol++)
                    {
                        if (spaceRow == 1 && spaceCol == 3)
                        {
                            string position = $"({Program.mainClass.GridMap[row][col].row},{Program.mainClass.GridMap[row][col].col})";
                            Console.Write(position);
                            newSpaceInCol = spaceInCol - position.Length + 1;
                        }
                        else if (spaceRow == 2 && spaceCol == 3)
                        {
                            string type = $"{Program.mainClass.GridMap[row][col].type}";
                            if (type == "S")
                                WriteYellow(type);
                            else if (type == "E")
                                WriteRed(type);
                            else
                                Console.Write(type);
                            newSpaceInCol = spaceInCol - type.Length + 1;
                        }
                        else
                            Console.Write(" ");
                    }
                    Console.Write("│");
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
    }
}
