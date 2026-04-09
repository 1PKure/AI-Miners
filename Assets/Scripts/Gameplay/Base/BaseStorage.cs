using UnityEngine;

public class BaseStorage : MonoBehaviour
{
    [SerializeField] private int totalStoredGold;
    [SerializeField] private Transform interactionPoint;

    public int TotalStoredGold => totalStoredGold;
    public Vector3 InteractionPosition => interactionPoint != null ? interactionPoint.position : transform.position;

    public void DepositGold(int amount)
    {
        totalStoredGold += amount;
    }
}