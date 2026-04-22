using UnityEngine;

[RequireComponent(typeof(PathNodeAgent))]
public class EnemyAgentController : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float loseTargetRadius = 7f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackExitRange = 1.5f;
    [SerializeField] private LayerMask minerLayer;

    [Header("Combat")]
    [SerializeField] private int damagePerHit = 1;
    [SerializeField] private float attackInterval = 1f;

    [Header("Target Memory")]
    [SerializeField] private float loseTargetDelay = 0.75f;
    [SerializeField] private float idleReacquireCooldown = 0.4f;

    [Header("Debug")]
    [SerializeField] private string currentStateName;

    private FiniteStateMachine<EnemyAgentController> stateMachine;
    private PathNodeAgent pathNodeAgent;
    private MinerAgentController currentTarget;
    private float attackTimer;
    private float reacquireTimer;

    public float DetectionRadius => detectionRadius;
    public float LoseTargetRadius => loseTargetRadius;
    public float AttackRange => attackRange;
    public float AttackExitRange => attackExitRange;
    public float AttackInterval => attackInterval;
    public int DamagePerHit => damagePerHit;
    public string CurrentStateName => currentStateName;
    public PathNodeAgent PathNodeAgent => pathNodeAgent;
    public MinerAgentController CurrentTarget => currentTarget;
    public float LoseTargetDelay => loseTargetDelay;

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
        if (reacquireTimer > 0f)
            reacquireTimer -= Time.deltaTime;

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

    public void StartReacquireCooldown()
    {
        reacquireTimer = idleReacquireCooldown;
    }

    public bool CanReacquireTarget()
    {
        return reacquireTimer <= 0f;
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

    public bool IsTargetInsideLoseRadius()
    {
        if (!HasValidTarget())
            return false;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance <= loseTargetRadius;
    }

    public bool IsTargetInAttackRange()
    {
        if (!HasValidTarget())
            return false;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance <= attackRange;
    }

    public bool IsTargetOutsideAttackExitRange()
    {
        if (!HasValidTarget())
            return true;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance > attackExitRange;
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

        currentTarget.AssignThreat(this);
        currentTarget.TakeDamage(damagePerHit);
        attackTimer = attackInterval;
    }
}