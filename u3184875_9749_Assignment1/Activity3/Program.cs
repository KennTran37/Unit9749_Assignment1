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

        Node startNode;

        List<Agent> agentList = new List<Agent>();
        List<List<Node>> possiblePaths = new List<List<Node>>();
        public List<Node> foundEndNodes = new List<Node>();

        static void Main(string[] args)
        {
            string userInput = UserInput.GetUserInput(out actThree.numOfAgents, out actThree.gridRowSize, out actThree.gridColSize, out actThree.gridMap);
            actThree.FindStartingPoint();

            for (int i = 0; i < actThree.numOfAgents; i++)
            {
                actThree.agentList.Add(new Agent($"Agent_{i + 1}", actThree.startNode));
            }

            Graph.BuildGraph(actThree.gridMap, actThree.gridRowSize, actThree.gridColSize);

            Thread[] threadArray = new Thread[actThree.agentList.Count];

            for (int i = 0; i < actThree.agentList.Count; i++)
            {
                ThreadStart threadStart = new ThreadStart(actThree.agentList[i].PathFinder);
                threadArray[i] = new Thread(threadStart);
            }

            threadArray.ToList().ForEach(t => { t.Start(); Thread.Sleep(1000); });

            actThree.WaitForMultiThread(threadArray);

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

        //checking if agents has visited the node already
        //including itself
        public bool OtherAgentsHasVisited(string agentName, Node currnet)
        {
            foreach (Agent agent in agentList)
                foreach (var node in agent.visited)
                    if (node.row.Equals(currnet.row) && node.col.Equals(currnet.col))
                        return true;
            return false;
        }

        public bool EndNodeAlreadyFound(Node endNode)
        {
            foreach (Node node in foundEndNodes)
                if (node.row.Equals(endNode.row) && node.col.Equals(endNode.col))
                    return true;
            return false;
        }
    }
}
