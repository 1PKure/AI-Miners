using System.Collections.Generic;
using UnityEngine;

public class AStarPathfindingStrategy : IPathfindingStrategy
{
    public List<PathNode> FindPath(PathNode startNode, PathNode goalNode)
    {
        List<PathNode> path = new List<PathNode>();

        if (!PathfindingUtils.IsNodeValid(startNode) || !PathfindingUtils.IsNodeValid(goalNode))
        {
            Debug.LogWarning("AStarPathfindingStrategy: startNode or goalNode is invalid.");
            return path;
        }

        PriorityQueue<PathNode> openQueue = new PriorityQueue<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();
        Dictionary<PathNode, float> gCost = new Dictionary<PathNode, float>();

        gCost[startNode] = 0f;
        cameFrom[startNode] = null;
        openQueue.Enqueue(startNode, PathfindingUtils.GetHeuristic(startNode, goalNode));

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

                float tentativeGCost = gCost[currentNode] + PathfindingUtils.GetTraversalCost(currentNode, neighbor);

                if (!gCost.ContainsKey(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    gCost[neighbor] = tentativeGCost;
                    cameFrom[neighbor] = currentNode;

                    float fCost = tentativeGCost + PathfindingUtils.GetHeuristic(neighbor, goalNode);
                    openQueue.Enqueue(neighbor, fCost);
                }
            }
        }

        Debug.LogWarning("AStarPathfindingStrategy: no path found.");
        return path;
    }
}