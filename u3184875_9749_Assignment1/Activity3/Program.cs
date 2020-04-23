using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Activity1;

//multiple AI's work together to find items located somewhere on the user's graph
namespace Activity3
{
    class Program
    {
        public static Program actThree = new Program();
        public List<List<Node>> gridMap = new List<List<Node>>();

        public int gridRowSize = 3;
        public int gridColSize = 3;
        public int numOfAgents;

        //used to look at all neighbour nodes     
        public int[] dirRow = { 0, 0, -1, 1, -1, 1, 1, -1 }; //direction in row
        public int[] dirCol = { 1, -1, 0, 0, 1, 1, -1, -1 }; //direction in coloumn

        Node startNode;

        List<Agent> agentList = new List<Agent>();
        List<List<Node>> possiblePaths = new List<List<Node>>();
        public List<Node> foundEndNodes = new List<Node>();

        static void Main(string[] args)
        {
            string userInput = UserInput.GetUserInput(out actThree.numOfAgents, out actThree.gridRowSize, out actThree.gridColSize, out actThree.gridMap);
            actThree.FindStartingPoint();


            Node[] neighbours = actThree.GetNeighboursFromStart();
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (i == actThree.numOfAgents)
                    break;
                Node node = neighbours[i];
                node.SetParentPos(actThree.startNode.row, actThree.startNode.col);
                actThree.agentList.Add(new Agent($"Agent_{i + 1}", node));
            }

            Graph.SetUpMap(actThree.gridMap, actThree.gridRowSize, actThree.gridColSize);

            Thread[] threadArray = new Thread[actThree.agentList.Count];

            for (int i = 0; i < actThree.agentList.Count; i++)
            {
                ThreadStart threadStart = new ThreadStart(actThree.agentList[i].PathFinder);
                threadArray[i] = new Thread(threadStart);
            }

            threadArray.ToList().ForEach(t => { t.Start(); /*Thread.Sleep(1000);*/ });

            //actThree.WaitForMultiThread(threadArray);

            Console.ReadKey();
        }

        void WaitForMultiThread(Thread[] threadArray)
        {
            bool notFinish = true;
            while (notFinish)
                foreach (Thread thread in threadArray)
                    notFinish = thread.IsAlive;
        }

        void FindStartingPoint()
        {
            gridMap.ForEach(list =>
            {
                list.ForEach(node =>
                {
                    if (node.type == "S")
                    {
                        startNode = node;
                        startNode.costToPos = 0;
                        return;
                    }
                });
            });
        }

        Node[] GetNeighboursFromStart()
        {
            List<Node> neighbours = new List<Node>();
            for (int i = 0; i < 8; i++)
            {
                int newRow = startNode.row + dirRow[i];
                int newCol = startNode.col + dirCol[i];

                if (newRow < 0 || newCol < 0)
                    continue;
                if (newRow > gridRowSize - 1 || newCol > gridColSize - 1)
                    continue;

                Node node = gridMap[newRow][newCol];
                if (node.type == "O")
                    continue;

                node.row = newRow;
                node.col = newCol;
                neighbours.Add(node);
            }

            return neighbours.ToArray();
        }

        //checking if agents has visited the node already
        //including itself
        public bool OtherAgentsHasVisited(Node current)
        {
            foreach (Agent agent in agentList)
                foreach (var node in agent.visited)
                    if (node.row.Equals(current.row) && node.col.Equals(current.col))
                        return true;
            return false;
        }

        public bool EndNodeAlreadyFound(Node endNode)
        {
            List<Node> foundEnds = new List<Node>(foundEndNodes);
            foreach (Node node in foundEnds)
                if (node.row.Equals(endNode.row) && node.col.Equals(endNode.col))
                    return true;
            return false;
        }
    }
}
