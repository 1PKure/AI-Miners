using UnityEngine;

public class BaseStorage : MonoBehaviour
{
    [SerializeField] private int totalStoredGold;

    public int TotalStoredGold => totalStoredGold;

    public void DepositGold(int amount)
    {
        totalStoredGold += amount;
    }
}