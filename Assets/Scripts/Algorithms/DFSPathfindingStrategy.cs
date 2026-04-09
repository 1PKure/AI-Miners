using System.Collections.Generic;
using UnityEngine;

public class DFSPathfindingStrategy : IPathfindingStrategy
{
    public List<PathNode> FindPath(PathNode startNode, PathNode goalNode)
    {
        List<PathNode> path = new List<PathNode>();

        if (!PathfindingUtils.IsNodeValid(startNode) || !PathfindingUtils.IsNodeValid(goalNode))
        {
            Debug.LogWarning("DFSPathfindingStrategy: startNode o goalNode inválidos.");
            return path;
        }

        Stack<PathNode> frontier = new Stack<PathNode>();
        HashSet<PathNode> visited = new HashSet<PathNode>();
        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();

        frontier.Push(startNode);
        visited.Add(startNode);
        cameFrom[startNode] = null;

        while (frontier.Count > 0)
        {
            PathNode currentNode = frontier.Pop();

            if (currentNode == goalNode)
                return PathfindingUtils.ReconstructPath(cameFrom, goalNode);

            foreach (PathNode neighbor in currentNode.Neighbors)
            {
                if (!PathfindingUtils.IsNodeValid(neighbor))
                    continue;

                if (visited.Contains(neighbor))
                    continue;

                frontier.Push(neighbor);
                visited.Add(neighbor);
                cameFrom[neighbor] = currentNode;
            }
        }

        Debug.LogWarning("DFSPathfindingStrategy: no se encontró un camino.");
        return path;
    }
}