using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Activity1;

namespace Activity3
{
    class Agent
    {
        public string agentName = null;
        public List<Node> toLookAt = new List<Node>();
        public List<Node> visited = new List<Node>();
        public List<Node> toAvoid = new List<Node>();

        //used to look at all neighbour nodes     
        int[] dirRow = { 0, 0, -1, 1, -1, 1, 1, -1 }; //direction in row
        int[] dirCol = { 1, -1, 0, 0, 1, 1, -1, -1 }; //direction in coloumn

        int sleepTime = 500; //ms

        public Agent(string name, Node startNode)
        {
            agentName = name;
            visited.Add(startNode);
        }

        //using Depth First Search Algorithm
        public void PathFinder()
        {
            //find neighbour nodes from start
            //get random neighbour and set it as currnet 
            Node neighbour = GetAllNeighbourNodes(visited.First()).FirstOrDefault();
            DepthFirstSearch(neighbour);
        }

        void DepthFirstSearch(Node current)
        {
            visited.Add(current);

            if (current.type[0] == 'I')
            {
                if (!Program.actThree.EndNodeAlreadyFound(current))
                {
                    //once found a new item us A* to construct a better path to the item
                    foreach (var node in visited)
                        Console.Write($"({node.Position()})");
                    Console.WriteLine(agentName + " End Found");
                    Program.actThree.foundEndNodes.Add(current);
                    return;
                }
            }

            //find open neighbours and get first element
            Node neighbour = GetAllNeighbourNodes(current).FirstOrDefault();
            if (!string.IsNullOrEmpty(neighbour.type))
            {
                neighbour.SetParentPos(current.row, current.col);
                Thread.Sleep(sleepTime);
                DepthFirstSearch(neighbour);
            }
            else
            {   //backtrack and find open neighbours to go to
                for (int i = visited.Count - 1; i >= 0; i--)
                {
                    Node neigbour = GetAllNeighbourNodes(visited[i]).FirstOrDefault();
                    if (!string.IsNullOrEmpty(neigbour.type))
                    {
                        neighbour.SetParentPos(visited[i].row, visited[i].col);
                        Thread.Sleep(sleepTime);
                        DepthFirstSearch(neigbour);
                        return;
                    }
                }
            }
        }

        bool IsInList(Node neighbour, List<Node> list)
        {
            foreach (Node vNode in list)
                if (vNode.row.Equals(neighbour.row) && vNode.col.Equals(neighbour.col))
                    return true;
            return false;
        }

        //getting all unvisited neighbours from current and prioritizing end node first
        Node[] GetAllNeighbourNodes(Node current)
        {
            List<Node> unvisitedNeighbours = new List<Node>();
            //8 possible directions to look at
            for (int i = 0; i < 8; i++)
            {
                int newRow = current.row + dirRow[i];
                int newCol = current.col + dirCol[i];

                if (newRow < 0 || newCol < 0)
                    continue;
                if (newRow > Program.actThree.gridRowSize - 1 || newCol > Program.actThree.gridColSize - 1)
                    continue;

                Node node = Program.actThree.gridMap[newRow][newCol];
                if (node.type == "O")
                    continue;
                if (IsInList(node, visited))
                    continue;
                if (Program.actThree.OtherAgentsHasVisited(agentName, node))
                    continue;

                node.row = newRow;
                node.col = newCol;
                //in the situation where there are other visitable nodes from current and end point is there
                //set it as first element/index because we are getting the first element from the caller anyways
                if (node.type[0] == 'I')
                    unvisitedNeighbours.Insert(0, node);
                else
                    unvisitedNeighbours.Add(node);
            }
            return unvisitedNeighbours.ToArray();
        }
    }
}
