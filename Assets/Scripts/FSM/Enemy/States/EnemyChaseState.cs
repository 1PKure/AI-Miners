using UnityEngine;

public class EnemyChaseState : FsmState<EnemyAgentController>
{
    private float repathTimer;
    private const float repathInterval = 0.25f;

    public override void Enter()
    {
        owner.SetCurrentStateName("Chasing");

        if (!owner.HasValidTarget())
        {
            owner.SendEvent(EnemyFsmEvents.TargetDead);
            return;
        }

        repathTimer = 0f;
        owner.UpdatePathToTarget();
    }

    public override void Update()
    {
        if (!owner.HasValidTarget())
        {
            owner.SendEvent(EnemyFsmEvents.TargetDead);
            return;
        }

        if (!owner.IsTargetInsideDetection())
        {
            owner.ClearTarget();
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(EnemyFsmEvents.MinerLost);
            return;
        }

        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            owner.UpdatePathToTarget();
        }

        if (owner.IsTargetInAttackRange())
        {
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(EnemyFsmEvents.MinerInRange);
        }
    }

    public override void Exit()
    {
    }
}