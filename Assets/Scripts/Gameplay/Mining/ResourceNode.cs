using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Resource")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int currentAmount = 50;
    [SerializeField] private int scorePerUnit = 1;
    [SerializeField] private float mineInterval = 0.5f;
    [SerializeField] private Transform interactionPoint;

    private MinerAgentController reservedBy;

    public ResourceType ResourceType => resourceType;
    public int CurrentAmount => currentAmount;
    public int ScorePerUnit => scorePerUnit;
    public float MineInterval => mineInterval;
    public bool HasResources => currentAmount > 0;
    public bool IsOccupied => reservedBy != null;
    public Vector3 InteractionPosition => interactionPoint != null ? interactionPoint.position : transform.position;

    public bool TryReserve(MinerAgentController miner)
    {
        if (miner == null)
            return false;

        if (!HasResources)
            return false;

        if (reservedBy != null && reservedBy != miner)
            return false;

        reservedBy = miner;
        return true;
    }

    public bool IsReservedBy(MinerAgentController miner)
    {
        return reservedBy == miner;
    }

    public void Release(MinerAgentController miner)
    {
        if (reservedBy == miner)
            reservedBy = null;
    }

    public int Extract(int amount)
    {
        if (currentAmount <= 0)
            return 0;

        int extracted = Mathf.Min(amount, currentAmount);
        currentAmount -= extracted;
        return extracted;
    }
}