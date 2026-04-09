using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [SerializeField] private int currentGold = 50;
    [SerializeField] private Transform interactionPoint;

    private MinerAgentController reservedBy;

    public int CurrentGold => currentGold;
    public bool HasGold => currentGold > 0;
    public bool IsOccupied => reservedBy != null;
    public MinerAgentController ReservedBy => reservedBy;
    public Vector3 InteractionPosition => interactionPoint != null ? interactionPoint.position : transform.position;

    public bool TryReserve(MinerAgentController miner)
    {
        if (miner == null)
            return false;

        if (!HasGold)
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
        {
            reservedBy = null;
        }
    }

    public int ExtractGold(int amount)
    {
        if (currentGold <= 0)
            return 0;

        int extracted = Mathf.Min(amount, currentGold);
        currentGold -= extracted;
        return extracted;
    }
}