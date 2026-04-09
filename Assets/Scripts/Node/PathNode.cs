using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    public bool IsWalkable { get; private set; } = true;
    public float NodeCost { get; private set; } = 1f;

    private readonly List<PathNode> neighbors = new List<PathNode>();
    public List<PathNode> Neighbors => neighbors;

    public Vector3 WorldPosition => transform.position;

    public void Initialize(Vector2Int gridPosition, bool isWalkable, float nodeCost = 1f)
    {
        GridPosition = gridPosition;
        IsWalkable = isWalkable;
        NodeCost = Mathf.Max(0.01f, nodeCost);
    }

    public void SetWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
    }

    public void SetNodeCost(float nodeCost)
    {
        NodeCost = Mathf.Max(0.01f, nodeCost);
    }

    public void AddNeighbor(PathNode neighbor)
    {
        if (neighbor == null)
            return;

        if (neighbors.Contains(neighbor))
            return;

        neighbors.Add(neighbor);
    }

    public void ClearNeighbors()
    {
        neighbors.Clear();
    }
}