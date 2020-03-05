using System;

namespace Activity4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        //Using dynamic programming to find all possible paths 
        //from the 'top left' to the 'bottom right' of the grid
        void DynamicProgramming()
        {

        }

        //add a new type of node that allows the agent to move to the node depending on the open spaces
        //for example, the sides - left, bottom-left, and the bottom are blocked off, while the others are open
        //create a function to check whether the agent can move to that node depending on the direction they are coming from
        //if they are coming from a diagonal node, check the two adjacent nodes from it to see if they are open as well.
    }
}
