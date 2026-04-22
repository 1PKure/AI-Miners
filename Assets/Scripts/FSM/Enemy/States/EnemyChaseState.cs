using UnityEngine;

public class EnemyChaseState : FsmState<EnemyAgentController>
{
    private float repathTimer;
    private float lostTargetTimer;

    private const float repathInterval = 0.5f;

    public override void Enter()
    {
        owner.SetCurrentStateName("Chasing");

        if (!owner.HasValidTarget())
        {
            owner.SendEvent(EnemyFsmEvents.TargetDead);
            return;
        }

        repathTimer = 0f;
        lostTargetTimer = 0f;
        owner.UpdatePathToTarget();
    }

    public override void Update()
    {
        if (!owner.HasValidTarget())
        {
            owner.SendEvent(EnemyFsmEvents.TargetDead);
            return;
        }

        if (owner.IsTargetInAttackRange())
        {
            owner.PathNodeAgent.StopMoving();
            owner.SendEvent(EnemyFsmEvents.MinerInRange);
            return;
        }

        if (!owner.IsTargetInsideLoseRadius())
        {
            lostTargetTimer += Time.deltaTime;

            if (lostTargetTimer >= owner.LoseTargetDelay)
            {
                owner.PathNodeAgent.StopMoving();
                owner.ClearTarget();
                owner.StartReacquireCooldown();
                owner.SendEvent(EnemyFsmEvents.MinerLost);
            }

            return;
        }

        lostTargetTimer = 0f;

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            owner.UpdatePathToTarget();
        }
    }

    public override void Exit()
    {
    }
}