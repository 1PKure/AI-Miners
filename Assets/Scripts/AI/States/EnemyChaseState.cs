using UnityEngine;

public class EnemyChaseState : FsmState<EnemyAgentController>
{
    private float repathTimer;
    private const float repathInterval = 0.4f;

    public override void Enter()
    {
        owner.SetCurrentStateName("Chase");
        repathTimer = 0f;
    }

    public override void Update()
    {
        if (owner.Player == null)
            return;

        float distanceToPlayer = Vector3.Distance(owner.transform.position, owner.Player.position);

        if (distanceToPlayer <= owner.ChaseStopDistance)
        {
            owner.PathNodeAgent.StopMoving();
            return;
        }

        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            owner.PathNodeAgent.SetDestination(owner.Player.position);
            repathTimer = repathInterval;
        }
    }

    public override void Exit()
    {
        owner.PathNodeAgent.StopMoving();
    }
}