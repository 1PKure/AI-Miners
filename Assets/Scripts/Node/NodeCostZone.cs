using UnityEngine;

public class NodeCostZone : MonoBehaviour
{
    [SerializeField] private float nodeCost = 3f;

    public float NodeCost => Mathf.Max(1f, nodeCost);
}