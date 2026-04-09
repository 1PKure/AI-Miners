using UnityEngine;

public class PathNodeGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float nodeSpacing = 1.5f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;

    [Header("Node Settings")]
    [SerializeField] private PathNode nodePrefab;
    [SerializeField] private bool generateOnStart = true;

    [Header("Walkable Detection")]
    [SerializeField] private bool useObstacleCheck = false;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleCheckRadius = 0.3f;

    [Header("Node Cost Settings")]
    [SerializeField] private float defaultNodeCost = 1f;
    [SerializeField] private bool useRandomNodeCosts = false;
    [SerializeField] private float minRandomCost = 1f;
    [SerializeField] private float maxRandomCost = 5f;

    [Header("Gizmos")]
    [SerializeField] private bool drawNodes = true;
    [SerializeField] private bool drawConnections = true;
    [SerializeField] private float gizmoNodeRadius = 0.2f;

    [SerializeField] private Transform floorTransform;
    [SerializeField] private Renderer floorRenderer;

    [Header("Auto Fit To Floor")]
    [SerializeField] private bool autoFitToFloor = true;
    [SerializeField] private float borderPadding = 0.5f;
    [SerializeField] private float nodeYOffset = 0f;

    private PathNode[,] grid;

    public PathNode[,] Grid => grid;
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    private void Awake()
    {
        if (generateOnStart)
            GenerateGrid();
    }

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        ClearGrid();

        if (autoFitToFloor)
        {
            FitGridToFloor();
        }

        grid = new PathNode[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPosition = GetWorldPosition(x, y);
                bool isWalkable = CheckIfWalkable(worldPosition);
                float nodeCost = GetNodeCost();

                PathNode newNode = Instantiate(nodePrefab, worldPosition, Quaternion.identity, transform);
                newNode.name = $"Node_{x}_{y}";
                newNode.Initialize(new Vector2Int(x, y), isWalkable, nodeCost);

                grid[x, y] = newNode;
            }
        }

        AssignNeighbors();
    }
    private void FitGridToFloor()
    {
        if (floorRenderer == null)
        {
            Debug.LogWarning("PathNodeGenerator: Auto Fit is enabled but Floor Renderer is missing.");
            return;
        }

        Bounds bounds = floorRenderer.bounds;

        float usableWidth = bounds.size.x - borderPadding * 2f;
        float usableHeight = bounds.size.z - borderPadding * 2f;

        gridWidth = Mathf.Max(1, Mathf.FloorToInt(usableWidth / nodeSpacing) + 1);
        gridHeight = Mathf.Max(1, Mathf.FloorToInt(usableHeight / nodeSpacing) + 1);

        float startX = bounds.min.x + borderPadding;
        float startZ = bounds.min.z + borderPadding;
        float y = bounds.max.y + nodeYOffset;

        gridOrigin = new Vector3(startX, y, startZ) - transform.position;
    }

    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        if (transform.childCount == 0)
            return;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private void AssignNeighbors()
    {
        if (grid == null)
            return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                PathNode currentNode = grid[x, y];
                currentNode.ClearNeighbors();

                TryAddNeighbor(currentNode, x + 1, y);
                TryAddNeighbor(currentNode, x - 1, y);
                TryAddNeighbor(currentNode, x, y + 1);
                TryAddNeighbor(currentNode, x, y - 1);
            }
        }
    }

    private void TryAddNeighbor(PathNode currentNode, int x, int y)
    {
        if (!IsInsideGrid(x, y))
            return;

        if (currentNode == null || !currentNode.IsWalkable)
            return;

        PathNode neighbor = grid[x, y];

        if (neighbor == null || !neighbor.IsWalkable)
            return;

        currentNode.AddNeighbor(neighbor);
    }

    private bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        Vector3 basePosition = gridOrigin + transform.position;
        return new Vector3(
            basePosition.x + x * nodeSpacing,
            basePosition.y,
            basePosition.z + y * nodeSpacing
        );
    }

    private bool CheckIfWalkable(Vector3 worldPosition)
    {
        if (!useObstacleCheck)
            return true;

        bool hasObstacle = Physics.CheckSphere(worldPosition, obstacleCheckRadius, obstacleLayer);
        return !hasObstacle;
    }

    private float GetNodeCost()
    {
        if (!useRandomNodeCosts)
            return defaultNodeCost;

        return Random.Range(minRandomCost, maxRandomCost);
    }

    public PathNode GetNode(int x, int y)
    {
        if (grid == null)
            return null;

        if (!IsInsideGrid(x, y))
            return null;

        return grid[x, y];
    }

    public PathNode GetClosestNode(Vector3 position)
    {
        if (grid == null)
            return null;

        PathNode closestNode = null;
        float minDistance = float.MaxValue;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                PathNode currentNode = grid[x, y];

                if (currentNode == null || !currentNode.IsWalkable)
                    continue;

                float distance = Vector3.Distance(position, currentNode.WorldPosition);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = currentNode;
                }
            }
        }

        return closestNode;
    }

    private void OnDrawGizmos()
    {
        if (grid == null)
            return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                PathNode node = grid[x, y];

                if (node == null)
                    continue;

                if (drawNodes)
                {
                    Gizmos.color = GetNodeColor(node);
                    Gizmos.DrawSphere(node.WorldPosition, gizmoNodeRadius);
                }

                if (drawConnections)
                {
                    Gizmos.color = Color.yellow;

                    foreach (PathNode neighbor in node.Neighbors)
                    {
                        if (neighbor == null)
                            continue;

                        Gizmos.DrawLine(node.WorldPosition, neighbor.WorldPosition);
                    }
                }
            }
        }
    }

    private Color GetNodeColor(PathNode node)
    {
        if (!node.IsWalkable)
            return Color.red;

        float normalizedCost = Mathf.InverseLerp(minRandomCost, maxRandomCost, node.NodeCost);
        return Color.Lerp(Color.green, Color.magenta, normalizedCost);
    }
}