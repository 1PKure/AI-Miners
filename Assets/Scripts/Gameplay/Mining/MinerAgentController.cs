using UnityEngine;

[RequireComponent(typeof(PathNodeAgent))]
public class MinerAgentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BaseStorage homeBase;

    [Header("Stats")]
    [SerializeField] private int maxCarryAmount = 10;
    [SerializeField] private float interactionDistance = 1.25f;
    [SerializeField] private int miningPerTick = 1;

    [Header("Debug")]
    [SerializeField] private string currentStateName;
    [SerializeField] private int carriedAmount;
    [SerializeField] private ResourceType carriedResourceType;
    [SerializeField] private bool hasResource;

    private FiniteStateMachine<MinerAgentController> stateMachine;
    private PathNodeAgent pathNodeAgent;
    private ResourceNode currentResourceNode;
    private int carriedScorePerUnit;

    public BaseStorage HomeBase => homeBase;
    public PathNodeAgent PathNodeAgent => pathNodeAgent;
    public ResourceNode CurrentResourceNode => currentResourceNode;

    public int CarriedAmount => carriedAmount;
    public ResourceType CarriedResourceType => carriedResourceType;
    public bool HasResource => hasResource;
    public int CarriedScorePerUnit => carriedScorePerUnit;

    public int MaxCarryAmount => maxCarryAmount;
    public float InteractionDistance => interactionDistance;
    public int MiningPerTick => miningPerTick;
    public string CurrentStateName => currentStateName;

    private void Awake()
    {
        pathNodeAgent = GetComponent<PathNodeAgent>();
    }

    private void Start()
    {
        stateMachine = new FiniteStateMachine<MinerAgentController>(this);

        stateMachine.AddState(new MinerIdleState());
        stateMachine.AddState(new MinerGoToMineState());
        stateMachine.AddState(new MinerMiningState());
        stateMachine.AddState(new MinerReturnToBaseState());
        stateMachine.AddState(new MinerUnloadState());

        stateMachine.AddTransition<MinerIdleState, MinerGoToMineState>(MinerFsmEvents.MineAssigned);
        stateMachine.AddTransition<MinerGoToMineState, MinerMiningState>(MinerFsmEvents.ReachedMine);
        stateMachine.AddTransition<MinerGoToMineState, MinerIdleState>(MinerFsmEvents.MineLost);

        stateMachine.AddTransition<MinerMiningState, MinerReturnToBaseState>(MinerFsmEvents.InventoryFull);
        stateMachine.AddTransition<MinerMiningState, MinerReturnToBaseState>(MinerFsmEvents.MineEmpty);
        stateMachine.AddTransition<MinerMiningState, MinerIdleState>(MinerFsmEvents.MineLost);

        stateMachine.AddTransition<MinerReturnToBaseState, MinerUnloadState>(MinerFsmEvents.ReachedBase);
        stateMachine.AddTransition<MinerUnloadState, MinerIdleState>(MinerFsmEvents.UnloadComplete);

        stateMachine.SetInitialState<MinerIdleState>();
    }

    private void Update()
    {
        if (stateMachine == null)
            return;

        stateMachine.Update();
    }

    public void SetCurrentStateName(string stateName)
    {
        currentStateName = stateName;
    }

    public void AssignResourceNode(ResourceNode node)
    {
        currentResourceNode = node;
    }

    public void ClearAssignedResourceNode()
    {
        currentResourceNode = null;
    }

    public void AddResource(ResourceType type, int amount, int scorePerUnit)
    {
        carriedResourceType = type;
        carriedAmount += amount;
        carriedAmount = Mathf.Min(carriedAmount, maxCarryAmount);
        carriedScorePerUnit = scorePerUnit;
        hasResource = carriedAmount > 0;
    }

    public void ClearCarriedResource()
    {
        carriedAmount = 0;
        carriedScorePerUnit = 0;
        hasResource = false;
    }

    public bool IsInventoryFull()
    {
        return carriedAmount >= maxCarryAmount;
    }

    public bool IsNearTarget(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) <= interactionDistance;
    }

    public void SendEvent(string trigger)
    {
        stateMachine.SendEvent(trigger);
    }

    public ResourceNode FindAvailableResourceNode()
    {
        ResourceNode[] nodes = Object.FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);

        foreach (ResourceNode node in nodes)
        {
            if (node.TryReserve(this))
                return node;
        }

        return null;
    }
}