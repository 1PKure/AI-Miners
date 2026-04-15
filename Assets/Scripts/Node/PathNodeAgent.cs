using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathNodeAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathNodeGenerator nodeGenerator;
    [SerializeField] private Transform debugTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float reachDistance = 0.05f;
    [SerializeField] private bool requestPathOnStart = false;

    [Header("Debug Controls")]
    [SerializeField] private bool useDebugControls = false;
    [SerializeField] private Key runKey = Key.Space;
    [SerializeField] private Key resetKey = Key.R;

    [Header("Debug")]
    [SerializeField] private bool drawPath = true;
    [SerializeField] private float destinationUpdateThreshold = 0.5f;

    [Header("Cost Movement")]
    [SerializeField] private bool useNodeCostSpeedPenalty = true;
    [SerializeField] private float minimumSpeedMultiplier = 0.35f;

    private List<PathNode> currentPath = new List<PathNode>();
    private int currentPathIndex = 0;
    private bool isMoving = false;
    private Vector3 initialPosition;
    private Vector3 lastRequestedDestination;
    private bool hasRequestedDestination = false;
    private bool hasReachedDestination = false;
    public bool HasReachedDestination => hasReachedDestination;

    public List<PathNode> CurrentPath => currentPath;
    public bool IsMoving => isMoving;
    public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void Start()
    {
        if (requestPathOnStart && useDebugControls && debugTarget != null)
            RequestPathToTarget();
    }

    private void Update()
    {
        if (useDebugControls)
            HandleInput();

        MoveAlongPath();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[runKey].wasPressedThisFrame)
        {
            RequestPathToTarget();
        }

        if (Keyboard.current[resetKey].wasPressedThisFrame)
        {
            ResetAgent();
        }
    }

    [ContextMenu("Reset Agent")]
    public void ResetAgent()
    {
        StopMoving();
        transform.position = initialPosition;
    }

    public void StopMoving()
    {
        isMoving = false;
        currentPathIndex = 0;
        currentPath.Clear();
        hasRequestedDestination = false;
        hasReachedDestination = false;
    }

    [ContextMenu("Request Path To Debug Target")]
    public void RequestPathToTarget()
    {
        if (debugTarget == null)
        {
            Debug.LogWarning("PathNodeAgent: missing debugTarget reference.");
            return;
        }

        SetDestination(debugTarget.position, true);
    }

    public void SetDestination(Vector3 worldPosition)
    {
        SetDestination(worldPosition, false);
    }

    public void SetDestination(Vector3 worldPosition, bool forceRefresh)
    {
        hasReachedDestination = false;

        if (nodeGenerator == null)
        {
            Debug.LogWarning("PathNodeAgent: missing PathNodeGenerator reference.");
            return;
        }

        if (PathfindingManager.Instance == null)
        {
            Debug.LogWarning("PathNodeAgent: PathfindingManager.Instance is null.");
            return;
        }

        if (!forceRefresh && hasRequestedDestination)
        {
            float destinationDelta = Vector3.Distance(lastRequestedDestination, worldPosition);

            if (destinationDelta < destinationUpdateThreshold)
                return;
        }

        PathNode startNode = nodeGenerator.GetClosestNode(transform.position);
        PathNode goalNode = nodeGenerator.GetClosestNode(worldPosition);

        if (startNode == null)
        {
            Debug.LogWarning($"PathNodeAgent: startNode is null. Agent position: {transform.position}");
        }

        if (goalNode == null)
        {
            Debug.LogWarning($"PathNodeAgent: goalNode is null. Destination: {worldPosition}");
        }

        if (startNode == null || goalNode == null)
        {
            return;
        }
        List<PathNode> newPath = PathfindingManager.Instance.FindPath(startNode, goalNode);

        if (newPath == null)
            newPath = new List<PathNode>();

        if (newPath.Count > 0)
        {
            PathNode firstNode = newPath[0];

            if (firstNode != null)
            {
                float distanceToFirstNode = Vector3.Distance(transform.position, firstNode.WorldPosition);

                if (distanceToFirstNode <= 0.1f)
                    newPath.RemoveAt(0);
            }
        }

        currentPath = newPath;
        currentPathIndex = 0;

        if (currentPath.Count == 0)
        {
            isMoving = false;
            hasReachedDestination = true;
        }
        else
        {
            isMoving = true;
            hasReachedDestination = false;
        }

        lastRequestedDestination = worldPosition;
        hasRequestedDestination = true;
    }

    private void MoveAlongPath()
    {
        if (!isMoving)
            return;

        if (currentPath == null || currentPath.Count == 0)
        {
            isMoving = false;
            return;
        }

        if (currentPathIndex >= currentPath.Count)
        {
            isMoving = false;
            hasReachedDestination = true;
            return;
        }

        PathNode currentTargetNode = currentPath[currentPathIndex];

        if (currentTargetNode == null)
        {
            currentPathIndex++;
            return;
        }

        Vector3 targetPosition = currentTargetNode.WorldPosition;
        targetPosition.y = transform.position.y;

        float speedMultiplier = GetSpeedMultiplier(currentTargetNode);
        float finalSpeed = moveSpeed * speedMultiplier;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            finalSpeed * Time.deltaTime
        );

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= reachDistance)
        {
            currentPathIndex++;

            if (currentPathIndex >= currentPath.Count)
            {
                isMoving = false;
                hasReachedDestination = true;
                Debug.Log("PathNodeAgent: destination reached.");
            }
        }
    }

    private float GetSpeedMultiplier(PathNode node)
    {
        if (!useNodeCostSpeedPenalty || node == null)
            return 1f;

        float multiplier = 1f / Mathf.Max(1f, node.NodeCost);
        return Mathf.Clamp(multiplier, minimumSpeedMultiplier, 1f);
    }
    private void OnDrawGizmos()
    {
        if (!drawPath || currentPath == null || currentPath.Count == 0)
            return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < currentPath.Count; i++)
        {
            if (currentPath[i] == null)
                continue;

            Vector3 currentPosition = currentPath[i].WorldPosition + Vector3.up * 0.3f;
            Gizmos.DrawSphere(currentPosition, 0.12f);

            if (i < currentPath.Count - 1 && currentPath[i + 1] != null)
            {
                Vector3 nextPosition = currentPath[i + 1].WorldPosition + Vector3.up * 0.3f;
                Gizmos.DrawLine(currentPosition, nextPosition);
            }
        }
    }
}