using TMPro;
using UnityEngine;

public class MiningUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BaseStorage baseStorage;
    [SerializeField] private MinerAgentController[] miners;

    [Header("Top UI")]
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text redCrystalText;
    [SerializeField] private TMP_Text blueCrystalText;
    [SerializeField] private TMP_Text algorithmText;

    [Header("Miners UI")]
    [SerializeField] private TMP_Text[] minerLines;

    private void Update()
    {
        UpdateTopUI();
        UpdateMinersUI();
        UpdateAlgorithmUI();
    }

    private void UpdateTopUI()
    {
        if (baseStorage == null)
            return;

        if (totalScoreText != null)
            totalScoreText.text = $"Score: {baseStorage.TotalScore}";

        if (goldText != null)
            goldText.text = $"Gold: {baseStorage.TotalGoldUnits}";

        if (redCrystalText != null)
            redCrystalText.text = $"Red Crystal: {baseStorage.TotalRedCrystalUnits}";

        if (blueCrystalText != null)
            blueCrystalText.text = $"Blue Crystal: {baseStorage.TotalBlueCrystalUnits}";
    }

    private void UpdateMinersUI()
    {
        if (minerLines == null)
            return;

        for (int i = 0; i < minerLines.Length; i++)
        {
            if (minerLines[i] == null)
                continue;

            if (miners == null || i >= miners.Length || miners[i] == null)
            {
                minerLines[i].text = "---";
                continue;
            }

            MinerAgentController miner = miners[i];

            string carriedText = miner.HasResource
                ? $"{miner.CarriedResourceType} x{miner.CarriedAmount}"
                : "Empty";

            minerLines[i].text = $"{miner.name} | {miner.CurrentStateName} | {carriedText}";
        }
    }

    private void UpdateAlgorithmUI()
    {
        if (algorithmText == null)
            return;

        if (PathfindingManager.Instance == null)
        {
            algorithmText.text = "Algorithm: None";
            return;
        }

        algorithmText.text = $"Algorithm: {PathfindingManager.Instance.CurrentAlgorithm}";
    }
}