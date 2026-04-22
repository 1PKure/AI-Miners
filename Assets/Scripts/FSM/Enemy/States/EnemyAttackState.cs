using UnityEngine;
public class EnemyAttackState : FsmState<EnemyAgentController>
{
    private float lostTargetTimer;

    public override void Enter()
    {
        owner.SetCurrentStateName("Attacking");
        owner.PathNodeAgent.StopMoving();
        owner.ResetAttackTimer();
        lostTargetTimer = 0f;
    }

    public override void Update()
    {
        if (!owner.HasValidTarget())
        {
            owner.SendEvent(EnemyFsmEvents.TargetDead);
            return;
        }

        if (!owner.IsTargetInsideLoseRadius())
        {
            lostTargetTimer += Time.deltaTime;

            if (lostTargetTimer >= owner.LoseTargetDelay)
            {
                owner.ClearTarget();
                owner.StartReacquireCooldown();
                owner.SendEvent(EnemyFsmEvents.MinerLost);
            }

            return;
        }

        lostTargetTimer = 0f;

        if (owner.IsTargetOutsideAttackExitRange())
        {
            owner.SendEvent(EnemyFsmEvents.MinerOutOfRange);
            return;
        }

        owner.PathNodeAgent.StopMoving();

        if (owner.CanAttackNow())
        {
            owner.AttackTarget();
        }
    }

    public override void Exit()
    {
    }
}