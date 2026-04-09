using UnityEngine;

[RequireComponent(typeof(PathNodeAgent))]
public class EnemyAgentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float loseRange = 7f;
    [SerializeField] private float chaseStopDistance = 1.25f;

    [Header("Debug")]
    [SerializeField] private string currentStateName;


    public float ChaseStopDistance => chaseStopDistance;

    private FiniteStateMachine<EnemyAgentController> stateMachine;
    private PathNodeAgent pathNodeAgent;

    public Transform Player => player;
    public float DetectionRange => detectionRange;
    public float LoseRange => loseRange;
    public PathNodeAgent PathNodeAgent => pathNodeAgent;
    public string CurrentStateName => currentStateName;

    private void Awake()
    {
        pathNodeAgent = GetComponent<PathNodeAgent>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        stateMachine = new FiniteStateMachine<EnemyAgentController>(this);

        stateMachine.AddState(new EnemyIdleState());
        stateMachine.AddState(new EnemyChaseState());

        stateMachine.AddTransition<EnemyIdleState, EnemyChaseState>(EnemyFsmEvents.PlayerDetected);
        stateMachine.AddTransition<EnemyChaseState, EnemyIdleState>(EnemyFsmEvents.PlayerLost);

        stateMachine.SetInitialState<EnemyIdleState>();
    }

    private void Update()
    {
        UpdateDetection();
        stateMachine.Update();
    }

    private void UpdateDetection()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (stateMachine.CurrentState is EnemyIdleState && distance <= detectionRange)
        {
            stateMachine.SendEvent(EnemyFsmEvents.PlayerDetected);
        }
        else if (stateMachine.CurrentState is EnemyChaseState && distance > loseRange)
        {
            stateMachine.SendEvent(EnemyFsmEvents.PlayerLost);
        }
    }

    public void SetCurrentStateName(string stateName)
    {
        currentStateName = stateName;
    }
}