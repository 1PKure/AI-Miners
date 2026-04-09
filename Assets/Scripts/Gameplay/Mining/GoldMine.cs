using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [SerializeField] private int currentGold = 50;
    [SerializeField] private bool isOccupied;

    public bool IsOccupied => isOccupied;
    public int CurrentGold => currentGold;
    public bool HasGold => currentGold > 0;

    public bool TryReserve()
    {
        if (isOccupied || !HasGold)
            return false;

        isOccupied = true;
        return true;
    }

    public void Release()
    {
        isOccupied = false;
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