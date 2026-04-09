using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfindingStrategy : IPathfindingStrategy
{
    public List<PathNode> FindPath(PathNode startNode, PathNode goalNode)
    {
        List<PathNode> path = new List<PathNode>();

        if (!PathfindingUtils.IsNodeValid(startNode) || !PathfindingUtils.IsNodeValid(goalNode))
        {
            Debug.LogWarning("DijkstraPathfindingStrategy: startNode or goalNode is invalid.");
            return path;
        }

        PriorityQueue<PathNode> openQueue = new PriorityQueue<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();
        Dictionary<PathNode, float> costSoFar = new Dictionary<PathNode, float>();

        openQueue.Enqueue(startNode, 0f);
        costSoFar[startNode] = 0f;
        cameFrom[startNode] = null;

        while (!openQueue.IsEmpty())
        {
            PathNode currentNode = openQueue.Dequeue();

            if (closedSet.Contains(currentNode))
                continue;

            if (currentNode == goalNode)
                return PathfindingUtils.ReconstructPath(cameFrom, goalNode);

            closedSet.Add(currentNode);

            foreach (PathNode neighbor in currentNode.Neighbors)
            {
                if (!PathfindingUtils.IsNodeValid(neighbor))
                    continue;

                if (closedSet.Contains(neighbor))
                    continue;

                float newCost = costSoFar[currentNode] + PathfindingUtils.GetTraversalCost(currentNode, neighbor);

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    cameFrom[neighbor] = currentNode;
                    openQueue.Enqueue(neighbor, newCost);
                }
            }
        }

        Debug.LogWarning("DijkstraPathfindingStrategy: no path found.");
        return path;
    }
}