namespace Activity1
{
    //Used to contain the data of a node on the graph
    public struct Node
    {
        public string type;
        public int col;         //column
        public int row;         //row
        public int cost;
        public int costToPos;   //setting the cost to maxValue, means that the noed has not been visited

        public int parentCol;
        public int parentRow;

        public Node(string type, int col, int row)
        {
            this.type = type;
            this.col = col;
            this.row = row;

            cost = 0;
            costToPos = int.MaxValue;
            parentCol = 0;
            parentRow = 0;
        }

        public Node(string type, int cost)
        {
            this.type = type;
            this.col = 0;
            this.row = 0;

            this.cost = cost;
            costToPos = int.MaxValue;
            parentCol = 0;
            parentRow = 0;
        }

        public string Position() => $"{row},{col}";
        public string ParentPos() => $"{parentRow},{parentCol}";
        public void SetParentPos(int pRow, int pCol)
        {
            parentRow = pRow;
            parentCol = pCol;
        }

        public bool IsEqualPosition(int _row, int _col)
        {
            if (row.Equals(_row) && col.Equals(_col))
                return true;
            return false;
        }
    }
}
