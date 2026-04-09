using UnityEngine;

[RequireComponent(typeof(PathNodeAgent))]
public class MinerAgentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BaseStorage homeBase;

    [Header("Stats")]
    [SerializeField] private int maxCarryGold = 10;
    [SerializeField] private float interactionDistance = 1.25f;
    [SerializeField] private int miningPerTick = 1;

    [Header("Debug")]
    [SerializeField] private string currentStateName;
    [SerializeField] private int carriedGold;

    private FiniteStateMachine<MinerAgentController> stateMachine;
    private PathNodeAgent pathNodeAgent;
    private GoldMine currentMine;

    public BaseStorage HomeBase => homeBase;
    public PathNodeAgent PathNodeAgent => pathNodeAgent;
    public GoldMine CurrentMine => currentMine;
    public int CarriedGold => carriedGold;
    public int MaxCarryGold => maxCarryGold;
    public float InteractionDistance => interactionDistance;
    public int MiningPerTick => miningPerTick;


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
        stateMachine.Update();
    }

    public void SetCurrentStateName(string stateName)
    {
        currentStateName = stateName;
    }

    public void AssignMine(GoldMine mine)
    {
        currentMine = mine;
    }

    public void ClearAssignedMine()
    {
        currentMine = null;
    }

    public void AddGold(int amount)
    {
        carriedGold += amount;
        carriedGold = Mathf.Min(carriedGold, maxCarryGold);
    }

    public void ClearGold()
    {
        carriedGold = 0;
    }

    public bool IsInventoryFull()
    {
        return carriedGold >= maxCarryGold;
    }

    public bool IsNearTarget(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) <= interactionDistance;
    }

    public void SendEvent(string trigger)
    {
        stateMachine.SendEvent(trigger);
    }

    public GoldMine FindAvailableMine()
    {
        GoldMine[] mines = Object.FindObjectsByType<GoldMine>(FindObjectsSortMode.None);

        foreach (GoldMine mine in mines)
        {
            if (mine.TryReserve(this))
            {
                return mine;
            }
        }

        return null;
    }
}