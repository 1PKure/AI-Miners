public class EnemyAttackState : FsmState<EnemyAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Attacking");
        owner.PathNodeAgent.StopMoving();
        owner.ResetAttackTimer();
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
            owner.SendEvent(EnemyFsmEvents.MinerLost);
            return;
        }

        if (owner.IsTargetOutsideAttackExitRange())
        {
            owner.SendEvent(EnemyFsmEvents.MinerOutOfRange);
            return;
        }

        if (owner.CanAttackNow())
        {
            owner.AttackTarget();
        }
    }

    public override void Exit()
    {
    }
}