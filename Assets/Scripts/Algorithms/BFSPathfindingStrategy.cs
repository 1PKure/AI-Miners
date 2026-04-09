using System.Collections.Generic;
using UnityEngine;

public class BFSPathfindingStrategy : IPathfindingStrategy
{
    public List<PathNode> FindPath(PathNode startNode, PathNode goalNode)
    {
        List<PathNode> path = new List<PathNode>();

        if (!PathfindingUtils.IsNodeValid(startNode) || !PathfindingUtils.IsNodeValid(goalNode))
        {
            Debug.LogWarning("BFSPathfindingStrategy: startNode o goalNode inválidos.");
            return path;
        }

        Queue<PathNode> frontier = new Queue<PathNode>();
        HashSet<PathNode> visited = new HashSet<PathNode>();
        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();

        frontier.Enqueue(startNode);
        visited.Add(startNode);
        cameFrom[startNode] = null;

        while (frontier.Count > 0)
        {
            PathNode currentNode = frontier.Dequeue();

            if (currentNode == goalNode)
                return PathfindingUtils.ReconstructPath(cameFrom, goalNode);

            foreach (PathNode neighbor in currentNode.Neighbors)
            {
                if (!PathfindingUtils.IsNodeValid(neighbor))
                    continue;

                if (visited.Contains(neighbor))
                    continue;

                frontier.Enqueue(neighbor);
                visited.Add(neighbor);
                cameFrom[neighbor] = currentNode;
            }
        }

        Debug.LogWarning("BFSPathfindingStrategy: no se encontró un camino.");
        return path;
    }
}