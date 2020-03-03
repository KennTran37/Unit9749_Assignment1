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

        //used to look at all neighbour nodes     
        int[] dirRow = { 0, 0, -1, 1, -1, 1, 1, -1 }; //direction in row
        int[] dirCol = { 1, -1, 0, 0, 1, 1, -1, -1 }; //direction in coloumn

        int sleepTime = 500; //ms

        Node startNode, endNode;

        public Agent(string name, Node startNode)
        {
            agentName = name;
            this.startNode = startNode;
            visited.Add(startNode);
        }

        public void PathFinder()
        {
            //find neighbour nodes from start
            //get random neighbour and set it as currnet 
            Node neighbour = GetAllNeighbourNodes(false, visited.First()).FirstOrDefault();
            if (!string.IsNullOrEmpty(neighbour.type))
                DepthFirstSearch(neighbour);
        }

        //using Depth First Search Algorithm to travel on the grid
        //once an item is found and has not been picked up by another agent, use A* to back track
        void DepthFirstSearch(Node current)
        {
            visited.Add(current);

            if (current.type[0] == 'I')
            {
                if (!Program.actThree.EndNodeAlreadyFound(current))
                {
                    //once found a new item us A* to construct a better path to the item
                    //foreach (var node in visited)
                    //    Console.Write($"({node.Position()})");
                    //Console.WriteLine(agentName + " End Found");
                    endNode = current;
                    Program.actThree.foundEndNodes.Add(current);
                    AStar();
                    return;
                }
            }

            //find open neighbours and get first element
            Node neighbour = GetAllNeighbourNodes(false, current).FirstOrDefault();
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
                    Node neigbour = GetAllNeighbourNodes(false, visited[i]).FirstOrDefault();
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

        //using A* to backtrack to the start
        void AStar()
        {
            toLookAt = new List<Node>();
            //creating a local visited list so that the other agents can continue to look at the 
            //already visited nodes taken by this agent to find the item
            List<Node> localVisited = new List<Node>();

            toLookAt.Add(startNode);
            while (toLookAt.Count > 0)
            {
                Node current = toLookAt.First();
                foreach (Node node in toLookAt)
                    if (node.costToPos < current.costToPos)
                        current = node;
                toLookAt.Remove(current);
                localVisited.Add(current);

                if (current.type == endNode.type)
                {
                    //Console.WriteLine("Finish");
                    //if (agentName == "Agent_2")
                    //    foreach (var node in localVisited)
                    //        Console.WriteLine($"{node.Position()} - {node.ParentPos()}");
                    BuildPath(localVisited, agentName);
                    return;
                }

                foreach (Node neighbour in GetAllNeighbourNodes(true, current))
                {
                    if (!IsInList(neighbour, localVisited))
                    {
                        int fcost = Fcost(neighbour, 0);
                        if (!IsInList(neighbour, toLookAt) || neighbour.costToPos < fcost)
                        {
                            Node node = neighbour;
                            node.costToPos = fcost;
                            node.SetParentPos(current.row, current.col);
                            if (!IsInList(node, toLookAt))
                                toLookAt.Add(node);
                        }
                    }
                }
            }
        }

        //parentCost is used to calculate the difficulty to get to the neighbour
        int Fcost(Node neighbour, int parentCost)
        {   //8 is the number of directions the agent can go 
            int distFromStart = 8 * (Math.Abs(neighbour.row - startNode.row) + Math.Abs(neighbour.col - startNode.col));
            int distFromEnd = 8 * (Math.Abs(neighbour.row - endNode.row) + Math.Abs(neighbour.col - endNode.col));
            return distFromStart + distFromEnd + parentCost;
        }

        bool IsInList(Node neighbour, List<Node> list)
        {
            foreach (Node vNode in list)
                if (vNode.IsEqualPosition(neighbour.row, neighbour.col))
                    return true;
            return false;
        }

        //getting all unvisited neighbours from current and prioritizing end node first
        Node[] GetAllNeighbourNodes(bool usingAStar, Node current)
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
                //if (IsInList(node, visited))
                //    continue;
                if (!usingAStar && Program.actThree.OtherAgentsHasVisited(agentName, node))
                    continue;

                node.row = newRow;
                node.col = newCol;
                //in the situation where there are other visitable nodes from current and end point is there
                //set it as first element/index because we are getting the first element from the caller anyways
                if (!usingAStar && node.type[0] == 'I')
                    unvisitedNeighbours.Insert(0, node);
                else
                    unvisitedNeighbours.Add(node);
            }
            return unvisitedNeighbours.ToArray();
        }

        public void BuildPath(List<Node> parents, string agentName)
        {
            List<Node> finalPath = new List<Node>();
            finalPath.Add(parents.Last());
            for (int i = parents.Count - 1; i >= 0; i--)
            {
                if (IsParent(finalPath.Last(), parents[i].row, parents[i].col))
                    finalPath.Add(parents[i]);

                if (parents[i].type == "S")
                    break;
            }
            finalPath.Reverse();

            Console.WriteLine(agentName + "Found: ");
            foreach (var p in finalPath)
                Console.Write($"({p.Position()})");
            //possiblePaths.Add(finalPath);
        }

        bool IsParent(Node current, int pRow, int pCol)
        {
            if (current.parentRow == pRow && current.parentCol == pCol)
                return true;
            return false;
        }
    }
}
