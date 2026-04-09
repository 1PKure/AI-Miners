using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviourSingleton<PathfindingManager>
{
    [Header("Settings")]
    [SerializeField] private PathfindingAlgorithmType currentAlgorithm = PathfindingAlgorithmType.BFS;

    private IPathfindingStrategy currentStrategy;

    public PathfindingAlgorithmType CurrentAlgorithm => currentAlgorithm;

    protected override void Awake()
    {
        base.Awake();
        SetStrategy(currentAlgorithm);
    }

    private void OnValidate()
    {
        SetStrategy(currentAlgorithm);
    }

    public void SetAlgorithm(PathfindingAlgorithmType algorithmType)
    {
        currentAlgorithm = algorithmType;
        SetStrategy(currentAlgorithm);
    }

    public List<PathNode> FindPath(PathNode startNode, PathNode goalNode)
    {
        if (currentStrategy == null)
        {
            Debug.LogWarning("PathfindingManager: currentStrategy es null. Se reasigna automáticamente.");
            SetStrategy(currentAlgorithm);
        }

        return currentStrategy.FindPath(startNode, goalNode);
    }

    private void SetStrategy(PathfindingAlgorithmType algorithmType)
{
    switch (algorithmType)
    {
        case PathfindingAlgorithmType.BFS:
            currentStrategy = new BFSPathfindingStrategy();
            break;

        case PathfindingAlgorithmType.DFS:
            currentStrategy = new DFSPathfindingStrategy();
            break;

        case PathfindingAlgorithmType.Dijkstra:
            currentStrategy = new DijkstraPathfindingStrategy();
            break;

        case PathfindingAlgorithmType.AStar:
            currentStrategy = new AStarPathfindingStrategy();
            break;

        default:
            currentStrategy = new BFSPathfindingStrategy();
            break;
    }
}
}