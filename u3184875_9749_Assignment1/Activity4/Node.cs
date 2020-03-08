using System.Collections.Generic;
using System.Linq;

namespace Activity4
{
    class Node
    {
        public string name = null;
        public int row = 0;
        public int col = 0;
        public Node parent;

        public Dictionary<Direction, bool> walkableDirections = new Dictionary<Direction, bool>();

        public Node(string _name, bool topRight, bool top, bool topLeft, bool right, bool left, bool bottomRight, bool bottom, bool bottomLeft)
        {
            name = _name;
            walkableDirections.Add(Direction.topRight, topRight);
            walkableDirections.Add(Direction.top, top);
            walkableDirections.Add(Direction.topLeft, topLeft);
            walkableDirections.Add(Direction.right, right);
            walkableDirections.Add(Direction.left, left);
            walkableDirections.Add(Direction.bottomRight, bottomRight);
            walkableDirections.Add(Direction.bottom, bottom);
            walkableDirections.Add(Direction.bottomLeft, bottomLeft);
        }

        public Node(Node _node, int row, int col)
        {
            name = _node.name;
            SetPosition(row, col);
            walkableDirections = _node.walkableDirections;
        }

        public void SetPosition(int _row, int _col)
        {
            row = _row;
            col = _col;
        }
        public string Position() => $"{row},{col}";

        public bool IsWalkableDirection(Direction direct)
        {
            return walkableDirections.Single(s => s.Key == direct).Value;
        }
    }
}
