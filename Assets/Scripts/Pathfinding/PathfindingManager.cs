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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetAlgorithm(PathfindingAlgorithmType.BFS);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetAlgorithm(PathfindingAlgorithmType.DFS);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetAlgorithm(PathfindingAlgorithmType.Dijkstra);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetAlgorithm(PathfindingAlgorithmType.AStar);
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