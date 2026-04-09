using TMPro;
using UnityEngine;

public class PathfindingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text algorithmText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private PathNodeAgent agent;

    private void Update()
    {
        if (algorithmText != null && PathfindingManager.Instance != null)
        {
            algorithmText.text = $"Algorithm: {PathfindingManager.Instance.CurrentAlgorithm}";
        }

        if (costText != null && agent != null)
        {
            costText.text = $"Path Nodes: {agent.CurrentPath.Count}";
        }
    }
}