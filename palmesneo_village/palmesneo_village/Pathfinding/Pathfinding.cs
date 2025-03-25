using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class Pathfinding
    {
        private int pathNodesAmount;

        public Pathfinding(int pathNodesAmount)
        {
            this.pathNodesAmount = pathNodesAmount;
        }

        public List<PathNode> FindPath(PathNode startNode, PathNode targetNode, bool adjacent)
        {
            Heap<PathNode> openSet = new Heap<PathNode>(pathNodesAmount);
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            while(openSet.Count > 0)
            {
                PathNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode.Equals(targetNode))
                {
                    return RetracePath(startNode, currentNode);
                }

                foreach(PathNode neighbourNode in currentNode.Neighbours)
                {
                    if (IsClippingCorner(currentNode, neighbourNode)) continue;

                    if(adjacent && neighbourNode.Equals(targetNode))
                    {
                        return RetracePath(startNode, currentNode);
                    }

                    if (neighbourNode.IsWalkable == false) continue;

                    if (closedSet.Contains(neighbourNode)) continue;

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbourNode) + neighbourNode.MovementPenalty;
                    if (newMovementCostToNeighbour < neighbourNode.GCost || !openSet.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = newMovementCostToNeighbour;
                        neighbourNode.HCost = GetDistance(neighbourNode, targetNode);
                        neighbourNode.Parent = currentNode;

                        if (!openSet.Contains(neighbourNode))
                        {
                            openSet.Add(neighbourNode);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbourNode);
                        }
                    }
                }
            }

            return null;
        }

        private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = endNode;

            while (currentNode.Equals(startNode) == false)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            return path;
        }

        private int GetDistance(PathNode nodeA, PathNode nodeB)
        {
            int dstX = Math.Abs(nodeA.X - nodeB.X);
            int dstY = Math.Abs(nodeA.Y - nodeB.Y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);

            return 14 * dstX + 10 * (dstY - dstX);
        }

        private bool IsClippingCorner(PathNode curr, PathNode neigh)
        {
            int dX = curr.X - neigh.X;
            int dY = curr.Y - neigh.Y;

            if (Math.Abs(dX) + Math.Abs(dY) == 2)
            {
                PathNode firstNode;

                if (dX < 0)
                    firstNode = curr.RightNeighbour;
                else
                    firstNode = curr.LeftNeighbour;

                if (!firstNode.IsWalkable)
                    return true;

                PathNode secondNode;

                if (dY < 0)
                    secondNode = curr.BottomNeighbour;
                else
                    secondNode = curr.TopNeighbour;

                if (!secondNode.IsWalkable)
                    return true;
            }

            return false;
        }

    }
}
