namespace Activity1
{
    //Used to contain the data of a node on the graph
    struct Node
    {
        public string type;
        public int col;         //column
        public int row;         //row
        public int cost;
        public int costToPos;   //setting the cost to maxValue, means that the noed has not been visited

        public Node(string type, int col, int row)
        {
            this.type = type;
            this.col = col;
            this.row = row;

            cost = 0;
            costToPos = int.MaxValue;
        }

        public Node(string type, int cost)
        {
            this.type = type;
            this.col = 0;
            this.row = 0;

            this.cost = cost;
            costToPos = int.MaxValue;
        }

        public string Position() => $"{row},{col}";
    }
}
