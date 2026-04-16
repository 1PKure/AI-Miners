using System.Collections;
using UnityEngine;


#pragma warning disable CS0414
[RequireComponent(typeof(PathNodeAgent))]
public class MinerAgentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BaseStorage homeBase;

    [Header("Stats")]
    [SerializeField] private int maxCarryAmount = 10;
    [SerializeField] private float interactionDistance = 1.25f;
    [SerializeField] private int miningPerTick = 1;
    [SerializeField] private float unloadInterval = 0.25f;

    [Header("Debug")]
    [SerializeField] private string currentStateName;
    [SerializeField] private int carriedAmount;
    [SerializeField] private ResourceType carriedResourceType;
    [SerializeField] private bool hasResource;
    [SerializeField] private bool isMiningInProgress;
    [SerializeField] private bool isUnloadingInProgress;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 10;

    [SerializeField] private int currentHealth;
    [SerializeField] private bool isDead;
    [Header("Flee")]
    [SerializeField] private float fleeDistance = 6f;
    [SerializeField] private float safeDistance = 8f;
    [SerializeField] private float fearCooldown = 2f;

    public EnemyAgentController CurrentThreat => currentThreat;
    public float FleeDistance => fleeDistance;
    public float SafeDistance => safeDistance;
    public float FearCooldown => fearCooldown;
    public bool IsFearCooldownActive => fearTimer > 0f;
    private EnemyAgentController currentThreat;
    private float fearTimer;
    private bool wasDamagedRecently;

    public bool WasDamagedRecently => wasDamagedRecently;

    private FiniteStateMachine<MinerAgentController> stateMachine;
    private PathNodeAgent pathNodeAgent;
    private ResourceNode currentResourceNode;
    private int carriedScorePerUnit;
    private Coroutine activeRoutine;

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
    public float UnloadInterval => unloadInterval;
    public string CurrentStateName => currentStateName;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    private ResourceNode forbiddenResourceNode;
    private float forbiddenResourceTimer;

    public bool HasForbiddenResourceNode => forbiddenResourceNode != null;
    private void Awake()
    {
        pathNodeAgent = GetComponent<PathNodeAgent>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        stateMachine = new FiniteStateMachine<MinerAgentController>(this);

        stateMachine.AddState(new MinerIdleState());
        stateMachine.AddState(new MinerGoToMineState());
        stateMachine.AddState(new MinerMiningState());
        stateMachine.AddState(new MinerReturnToBaseState());
        stateMachine.AddState(new MinerUnloadState());
        stateMachine.AddState(new MinerFleeState());
        stateMachine.AddState(new MinerDeadState());

        stateMachine.AddTransition<MinerIdleState, MinerGoToMineState>(MinerFsmEvents.MineAssigned);
        stateMachine.AddTransition<MinerGoToMineState, MinerMiningState>(MinerFsmEvents.ReachedMine);
        stateMachine.AddTransition<MinerGoToMineState, MinerIdleState>(MinerFsmEvents.MineLost);
        stateMachine.AddTransition<MinerMiningState, MinerReturnToBaseState>(MinerFsmEvents.InventoryFull);
        stateMachine.AddTransition<MinerMiningState, MinerReturnToBaseState>(MinerFsmEvents.MineEmpty);
        stateMachine.AddTransition<MinerMiningState, MinerIdleState>(MinerFsmEvents.MineLost);
        stateMachine.AddTransition<MinerReturnToBaseState, MinerUnloadState>(MinerFsmEvents.ReachedBase);
        stateMachine.AddTransition<MinerUnloadState, MinerIdleState>(MinerFsmEvents.UnloadComplete);

        stateMachine.AddTransition<MinerIdleState, MinerFleeState>(MinerFsmEvents.EnemyDetected);
        stateMachine.AddTransition<MinerGoToMineState, MinerFleeState>(MinerFsmEvents.EnemyDetected);
        stateMachine.AddTransition<MinerMiningState, MinerFleeState>(MinerFsmEvents.EnemyDetected);
        stateMachine.AddTransition<MinerReturnToBaseState, MinerFleeState>(MinerFsmEvents.EnemyDetected);
        stateMachine.AddTransition<MinerUnloadState, MinerFleeState>(MinerFsmEvents.EnemyDetected);
        stateMachine.AddTransition<MinerFleeState, MinerIdleState>(MinerFsmEvents.SafeReached);

        stateMachine.AddTransition<MinerIdleState, MinerDeadState>(MinerFsmEvents.Died);
        stateMachine.AddTransition<MinerGoToMineState, MinerDeadState>(MinerFsmEvents.Died);
        stateMachine.AddTransition<MinerMiningState, MinerDeadState>(MinerFsmEvents.Died);
        stateMachine.AddTransition<MinerReturnToBaseState, MinerDeadState>(MinerFsmEvents.Died);
        stateMachine.AddTransition<MinerUnloadState, MinerDeadState>(MinerFsmEvents.Died);
        stateMachine.AddTransition<MinerFleeState, MinerDeadState>(MinerFsmEvents.Died);

        stateMachine.SetInitialState<MinerIdleState>();
    }

    private void Update()
    {
        if (fearTimer > 0f)
        {
            fearTimer -= Time.deltaTime;
        }
        if (stateMachine == null)
            return;

        stateMachine.Update();

        if (forbiddenResourceTimer > 0f)
        {
            forbiddenResourceTimer -= Time.deltaTime;

            if (forbiddenResourceTimer <= 0f)
            {
                forbiddenResourceNode = null;
                forbiddenResourceTimer = 0f;
            }
        }
    }
    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        wasDamagedRecently = true;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            SendEvent(MinerFsmEvents.Died);
            return;
        }

        if (currentThreat != null)
        {
            SendEvent(MinerFsmEvents.EnemyDetected);
        }
    }
    public void ClearRecentDamageFlag()
    {
        wasDamagedRecently = false;
    }
    public void AssignThreat(EnemyAgentController enemy)
    {
        if (isDead || enemy == null)
            return;

        currentThreat = enemy;
        fearTimer = fearCooldown;
    }

    public void ClearThreat()
    {
        currentThreat = null;
    }
    public Vector3 GetFleePosition()
    {
        if (currentThreat == null)
            return transform.position;

        Vector3 direction = (transform.position - currentThreat.transform.position).normalized;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = transform.forward;
        }

        Vector3 rawTarget = transform.position + direction * safeDistance;
        return rawTarget;
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

    public void RemoveCarriedAmount(int amount)
    {
        carriedAmount = Mathf.Max(0, carriedAmount - amount);

        if (carriedAmount == 0)
        {
            carriedScorePerUnit = 0;
            hasResource = false;
        }
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
            if (IsResourceNodeForbidden(node))
                continue;

            if (node.TryReserve(this))
                return node;
        }

        return null;
    }

    public void StopActiveRoutine()
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        isMiningInProgress = false;
        isUnloadingInProgress = false;
    }

    public void StartMiningRoutine()
    {
        StopActiveRoutine();
        activeRoutine = StartCoroutine(MiningRoutine());
    }

    public void StartUnloadRoutine()
    {
        StopActiveRoutine();
        activeRoutine = StartCoroutine(UnloadRoutine());
    }

    private IEnumerator MiningRoutine()
    {
        isMiningInProgress = true;

        while (true)
        {
            if (currentResourceNode == null)
            {
                isMiningInProgress = false;
                activeRoutine = null;
                SendEvent(MinerFsmEvents.MineLost);
                yield break;
            }

            if (!currentResourceNode.IsReservedBy(this) || !currentResourceNode.HasResources)
            {
                currentResourceNode.Release(this);
                ClearAssignedResourceNode();

                isMiningInProgress = false;
                activeRoutine = null;
                SendEvent(MinerFsmEvents.MineLost);
                yield break;
            }

            yield return new WaitForSeconds(currentResourceNode.MineInterval);

            int extractedAmount = currentResourceNode.Extract(miningPerTick);

            if (extractedAmount > 0)
            {
                AddResource(
                    currentResourceNode.ResourceType,
                    extractedAmount,
                    currentResourceNode.ScorePerUnit
                );

                if (IsInventoryFull())
                {
                    isMiningInProgress = false;
                    activeRoutine = null;
                    SendEvent(MinerFsmEvents.InventoryFull);
                    yield break;
                }

                if (!currentResourceNode.HasResources)
                {
                    isMiningInProgress = false;
                    activeRoutine = null;
                    SendEvent(MinerFsmEvents.MineEmpty);
                    yield break;
                }
            }
            else
            {
                isMiningInProgress = false;
                activeRoutine = null;
                SendEvent(MinerFsmEvents.MineEmpty);
                yield break;
            }
        }
    }

    private IEnumerator UnloadRoutine()
    {
        isUnloadingInProgress = true;

        while (carriedAmount > 0)
        {
            yield return new WaitForSeconds(unloadInterval);

            if (homeBase != null)
            {
                homeBase.Deposit(
                    carriedResourceType,
                    1,
                    carriedScorePerUnit
                );
            }

            RemoveCarriedAmount(1);
        }

        if (currentResourceNode != null)
        {
            currentResourceNode.Release(this);
            ClearAssignedResourceNode();
        }

        isUnloadingInProgress = false;
        activeRoutine = null;
        SendEvent(MinerFsmEvents.UnloadComplete);
    }
    public void MarkResourceNodeAsDangerous(ResourceNode node, float duration)
    {
        forbiddenResourceNode = node;
        forbiddenResourceTimer = duration;
    }
    public bool IsResourceNodeForbidden(ResourceNode node)
    {
        return node != null && node == forbiddenResourceNode;
    }
    public void AbandonCurrentResourceNode()
    {
        if (currentResourceNode != null)
        {
            currentResourceNode.Release(this);
            currentResourceNode = null;
        }
    }
}
#pragma warning restore CS0414