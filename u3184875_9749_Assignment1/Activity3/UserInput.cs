using System;
using System.Collections.Generic;
using System.Linq;
using Activity1;

//multiple AI's work together to find items located somewhere on the user's graph
namespace Activity3
{
    public class UserInput
    {
        static Node[] types = { new Node("S", 0), new Node("E", 0), new Node("O", int.MaxValue), new Node("Ww", 6), new Node("Wg", 3), new Node("Wr", 4) };

        static List<List<Node>> gridMap = new List<List<Node>>();
        static int newRow, newCol, numOfAgents;

        public static string GetUserInput(out int agents, out int row, out int col, out List<List<Node>> newMap)
        {
            string input = null;

            while (true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Activity1.UserInput.DisplayError("Your Input is Empty \n Please enter a valid Input");
                    continue;
                }

                List<string> rowList = new List<string>();
                if (!IsValidAmountOfRowsAndAgents(input, out rowList))
                    continue;
                if (!IsValidColumnAndTypes(rowList))
                    continue;

                break;
            }

            row = newRow;
            col = newCol;
            agents = numOfAgents;
            newMap = gridMap;
            return input;
        }

        static bool IsValidAmountOfRowsAndAgents(string input, out List<string> rowList)
        {
            int startIndex = 0;
            rowList = new List<string>();
            //getting the number of agents
            for (int i = 0; i < input.Length; i++)
                if (input[i] == ' ')
                {
                    string sub = input.Substring(startIndex, i - startIndex);
                    if (int.TryParse(sub, out numOfAgents))
                    {
                        if (numOfAgents < 2)
                        {
                            Activity1.UserInput.DisplayError("The minimum number of Agents is 2");
                            return false;
                        }
                        startIndex += sub.Length + 1;
                        break;
                    }
                    Activity1.UserInput.DisplayError("Please enter the number of Agents at the Start of the input");
                    return false;
                }
            //getting the number of rows the user inputted
            for (int i = startIndex; i < input.Length; i++)
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
                Activity1.UserInput.DisplayError("Invalid grid \n grid cannot have 1 row");
                return false;
            }

            return true;
        }

        static bool IsValidColumnAndTypes(List<string> rowList)
        {
            int prevColCount = 0, startIndex = 0, numOfS = 0;
            string sub;
            gridMap = new List<List<Node>>();
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
                        Node node = new Node();
                        if (!Activity1.UserInput.IsValidType(true, sub, out node))
                        {
                            Activity1.UserInput.DisplayError($"Your Input: [{sub}] is invalid -- \n {row}");
                            return false;
                        }

                        if (sub == "S")
                            numOfS++;

                        node.row = rowList.IndexOf(row);
                        node.col = currentColCount;
                        gridMap.Last().Add(node);

                        startIndex += sub.Length + 1;
                        currentColCount++;
                    }
                }

                if (prevColCount == 0)
                    prevColCount = currentColCount;
                if (prevColCount < 0)
                {
                    Activity1.UserInput.DisplayError("A Column in your input is smaller then 2.. \n Columns can only be a minimum of 2 ");
                    return false;
                }

                if (prevColCount != currentColCount)
                {
                    Activity1.UserInput.DisplayError("Each Row's Column must be the same Length");
                    return false;
                }
            }

            if (numOfS != 1)
            {
                Activity1.UserInput.DisplayError("There must be only one Starting Point (S)");
                return false;
            }

            newCol = prevColCount;
            newRow = rowList.Count;

            return true;
        }
    }
}
