using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace palmesneo_village
{
    public class PathNode : IHeapItem<PathNode>
    {

        public int X { get; private set; }
        public int Y { get; private set; }

        public bool IsWalkable { get; set; }
        public int MovementPenalty { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }

        public PathNode Parent { get; set; }

        public HashSet<PathNode> Neighbours { get; private set; } = new HashSet<PathNode>();

        public PathNode TopNeighbour { get; set; }
        public PathNode BottomNeighbour { get; set; }
        public PathNode LeftNeighbour { get; set; }
        public PathNode RightNeighbour { get; set; }

        public PathNode LeftTopNeighbour { get; set; }
        public PathNode RightTopNeighbour { get; set; }
        public PathNode LeftBottomNeighbour { get; set; }
        public PathNode RightBottomNeighbour { get; set; }

        private Vector2 vector2;
        private Point point;

        public PathNode(int x, int y, bool isWalkable, int movementPenalty)
        {
            X = x;
            Y = y;
            IsWalkable = isWalkable;
            MovementPenalty = movementPenalty;

            vector2 = new Vector2(x, y);
            point = new Point(x, y);
        }

        public int FCost => GCost + HCost;

        public int HeapIndex { get; set; }

        public int CompareTo(PathNode nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(nodeToCompare.HCost);
            }
            return -compare;
        }

        public Vector2 ToVector2()
        {
            return vector2;
        }

        public Point ToPoint()
        {
            return point;
        }
    }
}
