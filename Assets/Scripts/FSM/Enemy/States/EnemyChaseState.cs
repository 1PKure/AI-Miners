public class EnemyChaseState : FsmState<EnemyAgentController>
{
    public override void Enter()
    {
        owner.SetCurrentStateName("Chasing");

        if (!owner.HasValidTarget())
        {
            owner.SendEvent(EnemyFsmEvents.TargetDead);
            return;
        }

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

        owner.UpdatePathToTarget();

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