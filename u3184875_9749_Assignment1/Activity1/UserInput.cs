using System;
using System.Collections.Generic;

namespace Activity1
{
    //Separate class so that it is more readable and easy to scroll through
    public class UserInput
    {
        static Node[] types = { new Node("S", 0), new Node("E", 0), new Node("O", int.MaxValue), new Node("Ww", 6), new Node("Wg", 3), new Node("Wr", 4) };

        static List<List<Node>> gridMap = new List<List<Node>>();
        static int newRow;
        static int newCol;

        //Returns the user's input after checking in a while loop, if the input is null or empty,
        //if the rows and columns have a valid amount, and whether the types are valid
        public static string GetUserInput(out int row, out int col, out List<List<Node>> newMap)
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

            row = newRow;
            col = newCol;
            newMap = gridMap;
            return input;
        }

        public static void DisplayError(string errorOutput)
        {
            Console.WriteLine(errorOutput + "\n Press any Key to continiue");
            Console.ReadKey();
            Console.Clear();
        }

        //Checks if the number of rows is greater then 1
        static bool IsValidAmountOfRows(string input, out List<string> rowList)
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

        static bool IsValidColumnsAndTypes(List<string> rowList)
        {
            int prevColCount = 0;
            int startIndex = 0;
            string sub;
            //making sure that there is only one Start and End Point
            int numOfS = 0;
            int numOfE = 0;

            gridMap = new List<List<Node>>();
            foreach (string row in rowList)
            {
                gridMap.Add(new List<Node>());
                startIndex = 0;
                int currentColCount = 0;

                for (int i = 0; i < row.Length; i++)
                {
                    //cutting up the row of strings then checking to see if it is a valid type
                    if (row[i] == ',' || i == row.Length - 1)
                    {
                        sub = row.Substring(startIndex, (i + (i == row.Length - 1 ? 1 : 0)) - startIndex);
                        Node node = new Node();
                        if (!IsValidType(false, sub, out node))
                        {
                            DisplayError($"Your Input: [{sub}] is invalid -- \n {row}");
                            return false;
                        }

                        if (sub == "S")
                            numOfS++;
                        if (sub == "E")
                            numOfE++;

                        node.row = rowList.IndexOf(row);
                        node.col = currentColCount;
                        gridMap[gridMap.Count - 1].Add(node);

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

            newCol = prevColCount;
            newRow = rowList.Count;

            return true;
        }

        //Loop through the array of types and check if the user's type is valid
        public static bool IsValidType(bool isActThree, string userType, out Node nodeType)
        {
            userType = userType.ToUpper();
            foreach (Node type in types)
                if (userType == type.type)
                {
                    nodeType = type;
                    return true;
                }

            if (userType[0] == 'W')
            {
                if (userType.Length == 1 || userType[1] == '0')
                {
                    nodeType = new Node(userType, 1);
                    return true;
                }

                int inputNumber;
                if (int.TryParse(userType.Substring(1, userType.Length - 1), out inputNumber))
                {
                    if (inputNumber <= 40 || inputNumber <= 120)
                    {
                        nodeType = new Node(userType, 5);
                        return true;
                    }
                    if (inputNumber <= 90)
                    {
                        nodeType = new Node(userType, 9);
                        return true;
                    }
                }
            }

            if (isActThree)
            {
                if (userType[0] == 'I')
                {
                    if (int.TryParse(userType.Substring(1, userType.Length - 1), out _))
                    {
                        nodeType = new Node(userType, 0);
                        return true;
                    }
                }
            }

            nodeType = new Node();
            return false;
        }
    }
}
