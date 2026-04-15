using UnityEngine;

[RequireComponent(typeof(PathNodeAgent))]
public class EnemyAgentController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float threatRange = 2f;
    [SerializeField] private float attackRange = 1.25f;
    [SerializeField] private LayerMask minerLayer;

    [Header("Combat")]
    [SerializeField] private int damagePerHit = 1;
    [SerializeField] private float attackInterval = 1f;

    [Header("Debug")]
    [SerializeField] private string currentStateName;

    private FiniteStateMachine<EnemyAgentController> stateMachine;
    private PathNodeAgent pathNodeAgent;
    private MinerAgentController currentTarget;
    private float attackTimer;

    public PathNodeAgent PathNodeAgent => pathNodeAgent;
    public MinerAgentController CurrentTarget => currentTarget;
    public float DetectionRadius => detectionRadius;
    public float ThreatRange => threatRange;
    public float AttackRange => attackRange;
    public int DamagePerHit => damagePerHit;
    public float AttackInterval => attackInterval;
    public string CurrentStateName => currentStateName;

    private void Awake()
    {
        pathNodeAgent = GetComponent<PathNodeAgent>();
    }

    private void Start()
    {
        stateMachine = new FiniteStateMachine<EnemyAgentController>(this);

        stateMachine.AddState(new EnemyIdleState());
        stateMachine.AddState(new EnemyChaseState());
        stateMachine.AddState(new EnemyAttackState());

        stateMachine.AddTransition<EnemyIdleState, EnemyChaseState>(EnemyFsmEvents.MinerDetected);
        stateMachine.AddTransition<EnemyChaseState, EnemyAttackState>(EnemyFsmEvents.MinerInRange);
        stateMachine.AddTransition<EnemyChaseState, EnemyIdleState>(EnemyFsmEvents.MinerLost);
        stateMachine.AddTransition<EnemyChaseState, EnemyIdleState>(EnemyFsmEvents.TargetDead);
        stateMachine.AddTransition<EnemyAttackState, EnemyChaseState>(EnemyFsmEvents.MinerOutOfRange);
        stateMachine.AddTransition<EnemyAttackState, EnemyIdleState>(EnemyFsmEvents.MinerLost);
        stateMachine.AddTransition<EnemyAttackState, EnemyIdleState>(EnemyFsmEvents.TargetDead);

        stateMachine.SetInitialState<EnemyIdleState>();
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

    public void AssignTarget(MinerAgentController miner)
    {
        currentTarget = miner;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    public void SendEvent(string trigger)
    {
        stateMachine.SendEvent(trigger);
    }

    public MinerAgentController FindClosestMiner()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, minerLayer);

        MinerAgentController closestMiner = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            MinerAgentController miner = hits[i].GetComponent<MinerAgentController>();

            if (miner == null || miner.IsDead)
                continue;

            float distance = Vector3.Distance(transform.position, miner.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestMiner = miner;
            }
        }

        return closestMiner;
    }

    public bool HasValidTarget()
    {
        return currentTarget != null && !currentTarget.IsDead;
    }

    public bool IsTargetInsideDetection()
    {
        if (!HasValidTarget())
            return false;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance <= detectionRadius;
    }

    public bool IsTargetInThreatRange()
    {
        if (!HasValidTarget())
            return false;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance <= threatRange;
    }

    public bool IsTargetInAttackRange()
    {
        if (!HasValidTarget())
            return false;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance <= attackRange;
    }

    public void UpdatePathToTarget()
    {
        if (!HasValidTarget())
            return;

        pathNodeAgent.SetDestination(currentTarget.transform.position);
    }

    public void ResetAttackTimer()
    {
        attackTimer = attackInterval;
    }

    public bool CanAttackNow()
    {
        attackTimer -= Time.deltaTime;
        return attackTimer <= 0f;
    }

    public void AttackTarget()
    {
        if (!HasValidTarget())
            return;

        currentTarget.TakeDamage(damagePerHit);
        currentTarget.AssignThreat(this);
        attackTimer = attackInterval;
    }
}