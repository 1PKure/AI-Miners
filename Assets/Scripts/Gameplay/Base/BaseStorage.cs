using UnityEngine;

public class BaseStorage : MonoBehaviour
{
    [SerializeField] private int totalScore;
    [SerializeField] private int totalGoldUnits;
    [SerializeField] private int totalRedCrystalUnits;
    [SerializeField] private int totalBlueCrystalUnits;
    [SerializeField] private Transform interactionPoint;

    public int TotalScore => totalScore;
    public int TotalGoldUnits => totalGoldUnits;
    public int TotalRedCrystalUnits => totalRedCrystalUnits;
    public int TotalBlueCrystalUnits => totalBlueCrystalUnits;

    public Vector3 InteractionPosition => interactionPoint != null ? interactionPoint.position : transform.position;

    public void Deposit(ResourceType type, int amount, int scorePerUnit)
    {
        totalScore += amount * scorePerUnit;

        switch (type)
        {
            case ResourceType.Gold:
                totalGoldUnits += amount;
                break;

            case ResourceType.RedCrystal:
                totalRedCrystalUnits += amount;
                break;

            case ResourceType.BlueCrystal:
                totalBlueCrystalUnits += amount;
                break;
        }
    }
}