using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace palmesneo_village
{
    public class PathNodeMap
    {
        private PathNode[,] pathNodes;

        private int width;
        private int height;

        private Pathfinding pathfinding;

        public PathNodeMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            pathfinding = new Pathfinding(width * height);

            CreateGrid();

            MakeConnectionsBetweenNodes();
        }

        private void CreateGrid()
        {
            pathNodes = new PathNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pathNodes[x, y] = new PathNode(x, y, true, 0);
                }
            }
        }

        private void MakeConnectionsBetweenNodes()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNode node = pathNodes[x, y];

                    node.LeftNeighbour = TryGetNode(x - 1, y);
                    node.RightNeighbour = TryGetNode(x + 1, y);
                    node.TopNeighbour = TryGetNode(x, y - 1);
                    node.BottomNeighbour = TryGetNode(x, y + 1);

                    node.LeftTopNeighbour = TryGetNode(x - 1, y - 1);
                    node.LeftBottomNeighbour = TryGetNode(x - 1, y + 1);
                    node.RightTopNeighbour = TryGetNode(x + 1, y - 1);
                    node.RightBottomNeighbour = TryGetNode(x + 1, y + 1);

                    node.Neighbours.Add(node.LeftNeighbour);
                    node.Neighbours.Add(node.RightNeighbour);
                    node.Neighbours.Add(node.TopNeighbour);
                    node.Neighbours.Add(node.BottomNeighbour);

                    node.Neighbours.Add(node.LeftTopNeighbour);
                    node.Neighbours.Add(node.LeftBottomNeighbour);
                    node.Neighbours.Add(node.RightTopNeighbour);
                    node.Neighbours.Add(node.RightBottomNeighbour);

                    node.Neighbours.Remove(null);
                }
            }
        }

        public PathNode TryGetNode(int x, int y)
        {
            if (x < 0 || x >= width) return null;

            if (y < 0 || y >= height) return null;

            return pathNodes[x, y];
        }

        public List<PathNode> FindPath(PathNode startNode, PathNode targetNode, bool adjacent)
        {
            return pathfinding.FindPath(startNode, targetNode, adjacent);
        }
    }
}
