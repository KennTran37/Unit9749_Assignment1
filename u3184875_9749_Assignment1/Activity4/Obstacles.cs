namespace Activity4
{
    public class Obstacles
    {
        static Node empty = new Node("Empty", true, true, true, true, true, true, true, true);
        static Node circle = new Node("Circle", true, false, true, false, false, true, false, true);
        static Node pentagon = new Node("Pentagon", true, false, true, false, false, false, false, false);

        static Node horiRectangleTop = new Node("HoriRectTop", false, false, false, true, true, true, true, true);
        static Node horiRectangleBottom = new Node("HoriRecBot", true, true, true, true, true, false, false, false);
        static Node vertRectangleMiddle = new Node("VertRectMid", true, false, true, true, true, true, false, true);
        static Node vertRectangleRight = new Node("VertRectRight", false, true, true, false, true, false, true, true);

        static Node topRightTriangle = new Node("TopRightTri", false, false, false, false, true, false, true, true);
        static Node bottomLeftTriangle = new Node("BotLeftTri", true, true, false, true, false, false, false, false);
        static Node smallTopRightTriangle = new Node("SmallTopRightTri", false, true, true, true, true, true, true, true);
        static Node smallBottomLeftTriangle = new Node("SmallBotLeftTri", true, true, true, true, true, true, true, false);

        static Node leftLShape = new Node("LeftL", true, true, false, true, false, true, true, false);

        internal static Node Empty(int row, int col)
        { return new Node(empty, row, col); }
        internal static Node Circle(int row, int col)
        { return new Node(circle, row, col); }
        internal static Node Pentagon(int row, int col)
        { return new Node(pentagon, row, col); }
        internal static Node HoriRectangleTop(int row, int col)
        { return new Node(horiRectangleTop, row, col); }
        internal static Node HoriRectangleBottom(int row, int col)
        { return new Node(horiRectangleBottom, row, col); }
        internal static Node VertRectangleMiddle(int row, int col)
        { return new Node(vertRectangleMiddle, row, col); }
        internal static Node VertRectangleRight(int row, int col)
        { return new Node(vertRectangleRight, row, col); }
        internal static Node TopRightTriangle(int row, int col)
        { return new Node(topRightTriangle, row, col); }
        internal static Node BottomLeftTriangle(int row, int col)
        { return new Node(bottomLeftTriangle, row, col); }
        internal static Node SmallTopRightTriangle(int row, int col)
        { return new Node(smallTopRightTriangle, row, col); }
        internal static Node SmallBottomLeftTriangle(int row, int col)
        { return new Node(smallBottomLeftTriangle, row, col); }
        internal static Node LeftLShape(int row, int col)
        { return new Node(leftLShape, row, col); }
    }
}
