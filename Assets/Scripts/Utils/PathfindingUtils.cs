using System.Collections.Generic;
using UnityEngine;

public static class PathfindingUtils
{
    public static bool IsNodeValid(PathNode node)
    {
        return node != null && node.IsWalkable;
    }

    public static List<PathNode> ReconstructPath(Dictionary<PathNode, PathNode> cameFrom, PathNode goalNode)
    {
        List<PathNode> path = new List<PathNode>();

        if (goalNode == null)
            return path;

        PathNode currentNode = goalNode;

        while (currentNode != null)
        {
            path.Add(currentNode);

            if (cameFrom.ContainsKey(currentNode))
                currentNode = cameFrom[currentNode];
            else
                currentNode = null;
        }

        path.Reverse();
        return path;
    }

    public static float GetDistance(PathNode a, PathNode b)
    {
        if (a == null || b == null)
            return float.MaxValue;

        return Vector3.Distance(a.WorldPosition, b.WorldPosition);
    }

    public static float GetTraversalCost(PathNode fromNode, PathNode toNode)
    {
        if (fromNode == null || toNode == null)
            return float.MaxValue;

        return GetDistance(fromNode, toNode) * toNode.NodeCost;
    }

    public static float GetHeuristic(PathNode currentNode, PathNode goalNode)
    {
        if (currentNode == null || goalNode == null)
            return float.MaxValue;

        return Vector2Int.Distance(currentNode.GridPosition, goalNode.GridPosition);
    }
}